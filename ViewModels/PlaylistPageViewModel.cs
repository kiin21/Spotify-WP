using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spotify.Helpers;

public class PlaylistPageViewModel : INotifyPropertyChanged
{
    private readonly PlaylistService _playlistService;
    private readonly PlaylistSongService _playlistSongService;
    private ObservableCollection<PlaylistDTO> _playlists;
    private ObservableCollection<PlaylistSongDTO> _playlistSongs;
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

    public ObservableCollection<PlaylistSongDTO> PlaylistSongs
    {
        get => _playlistSongs;
        set
        {
            _playlistSongs = value;
            OnPropertyChanged(nameof(PlaylistSongs));
            OnPropertyChanged(nameof(SongCount)); // Notify when PlaylistSongs is updated
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

    public PlaylistPageViewModel(PlaylistService playlistService, PlaylistSongService playlistSongService)
    {
        _playlistService = playlistService;
        _playlistSongService = playlistSongService;
        _playlists = new ObservableCollection<PlaylistDTO>();
        _playlistSongs = new ObservableCollection<PlaylistSongDTO>();
        //_ = LoadPlaylists();
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
            var songs = await _playlistSongService.GetSongsForPlaylistAsync(playlistId);
            PlaylistSongs = new ObservableCollection<PlaylistSongDTO>(songs);
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

        // Check if the requested playlist is "Liked Songs" by checking its IsLikedSong property
        var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
        if (playlist != null && playlist.IsLikedSong)
        {
            SelectedPlaylist = playlist;
        }
        else if (playlist != null)
        {
            SelectedPlaylist = playlist;

            // Load the songs for the selected playlist
            var songs = await _playlistSongService.GetSongsForPlaylistAsync(playlistId);
            PlaylistSongs = new ObservableCollection<PlaylistSongDTO>(songs);
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
            Avatar = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRXX6SuxBncRoZdYpU9mt8u7hveYoHzeq9vPg&s"
        };

        await _playlistService.AddPlaylistAsync(newPlaylist);
        Playlists.Add(newPlaylist);
    }

    public async Task RemoveSelectedPlaylist()
    {
        if (SelectedPlaylist != null)
        {
            await _playlistService.UpdatePlaylistStatusAsync(SelectedPlaylist.Id, true);
            SelectedPlaylist = null;
            PlaylistSongs.Clear();
            OnPropertyChanged(nameof(SelectedPlaylist));
        }
    }


    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
