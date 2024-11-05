using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

public class PlaylistPageViewModel : INotifyPropertyChanged
{
    private readonly PlaylistService _playlistService;
    private readonly PlaylistSongDetailService _playlistSongDetailService;
    private ObservableCollection<PlaylistDTO> _playlists;
    private ObservableCollection<PlaylistSongDetailDTO> _playlistSongs;
    private PlaylistDTO _selectedPlaylist;

    public ObservableCollection<PlaylistDTO> Playlists
    {
        get => _playlists;
        set
        {
            _playlists = value;
            OnPropertyChanged(nameof(Playlists));
        }
    }

    public ObservableCollection<PlaylistSongDetailDTO> PlaylistSongs
    {
        get => _playlistSongs;
        set
        {
            _playlistSongs = value;
            OnPropertyChanged(nameof(PlaylistSongs));
            OnPropertyChanged(nameof(SongCount)); // Notify when Songs is updated
        }
    }

    public string SongCount => $"{PlaylistSongs?.Count ?? 0} bài hát";

    public PlaylistDTO SelectedPlaylist
    {
        get => _selectedPlaylist;
        set
        {
            if (_selectedPlaylist != value)
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
                SelectedPlaylistIdChanged?.Invoke(this, _selectedPlaylist?.Id.ToString() ?? string.Empty);
                _ = LoadPlaylistSongs(_selectedPlaylist?.Id);
            }
        }
    }

    public event EventHandler<string> SelectedPlaylistIdChanged;

    public PlaylistPageViewModel(PlaylistService playlistService, PlaylistSongDetailService playlistSongDetailService)
    {
        _playlistService = playlistService;
        _playlistSongDetailService = playlistSongDetailService;
        _playlists = new ObservableCollection<PlaylistDTO>();
        _playlistSongs = new ObservableCollection<PlaylistSongDetailDTO>();
    }

    private async Task LoadPlaylists()
    {
        var playlists = await _playlistService.GetPlaylistsAsync();
        var filteredPlaylists = playlists
            .Where(p => !p.IsDeleted && !p.IsLikedSong) // Exclude Liked Songs
            .ToList();

        Playlists = new ObservableCollection<PlaylistDTO>(filteredPlaylists);
        SelectedPlaylist = Playlists.FirstOrDefault();
    }

    private async Task LoadPlaylistSongs(string playlistId)
    {
        if (!string.IsNullOrEmpty(playlistId))
        {
            var songs = await _playlistSongDetailService.GetPlaylistSongDetailAsync(playlistId);
            PlaylistSongs = new ObservableCollection<PlaylistSongDetailDTO>(songs);
        }
        else
        {
            PlaylistSongs.Clear();
        }
    }

    public void ClearSelectedPlaylist()
    {
        SelectedPlaylist = null;
        PlaylistSongs.Clear();
    }

    public async Task LoadSelectedPlaylist(string playlistId)
    {
        // Check if the selected playlist is already loaded
        if (SelectedPlaylist != null && SelectedPlaylist.Id == playlistId)
            return;

        ClearSelectedPlaylist();

        var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
        if (playlist != null)
        {
            SelectedPlaylist = playlist;
            //var songs = await _playlistSongDetailService.GetPlaylistSongDetailAsync(playlistId);
            //PlaylistSongs = new ObservableCollection<PlaylistSongDetailDTO>(songs);
            await LoadPlaylistSongs(playlistId);
        }
    }

    public async Task AddPlaylistAsync(string playlistName)
    {
        var newPlaylist = new PlaylistDTO
        {
            Title = playlistName,
            CreatedBy = "Current User",
            CreatedAt = DateTime.Now,
            IsLikedSong = false,
            IsDeleted = false,
            Avatar = "https://example.com/default-cover.jpg"
        };

        await _playlistService.AddPlaylistAsync(newPlaylist);
        Playlists.Add(newPlaylist);
    }

    public async Task RemoveSelectedPlaylist()
    {
        if (SelectedPlaylist != null)
        {
            // Mark the playlist as deleted in the database
            await _playlistService.UpdatePlaylistStatusAsync(SelectedPlaylist.Id, true);

            // Remove the playlist from local collection
            Playlists.Remove(SelectedPlaylist);

            // Clear the currently selected playlist
            ClearSelectedPlaylist();
        }
    }


    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
