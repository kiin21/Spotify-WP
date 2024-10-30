using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Linq;
using System.Threading.Tasks;

public class PlaylistViewModel : INotifyPropertyChanged
{
    private readonly PlaylistService _playlistService;
    private ObservableCollection<PlaylistDTO> _playlists;
    private PlaylistDTO _selectedPlaylist;

    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<PlaylistDTO> Playlists
    {
        get => _playlists;
        set
        {
            _playlists = value;
            OnPropertyChanged(nameof(Playlists));
        }
    }

    public PlaylistDTO SelectedPlaylist
    {
        get => _selectedPlaylist;
        set
        {
            _selectedPlaylist = value;
            OnPropertyChanged(nameof(SelectedPlaylist));
            // Check if _selectedPlaylist is not null before accessing Id
            SelectedPlaylistIdChanged?.Invoke(this, _selectedPlaylist != null ? _selectedPlaylist.Id.ToString() : string.Empty);
        }
    }

    public event EventHandler<string> SelectedPlaylistIdChanged;

    public PlaylistViewModel(PlaylistService playlistService)
    {
        _playlistService = playlistService;
        _playlists = new ObservableCollection<PlaylistDTO>();
        LoadPlaylists();
    }

    private async void LoadPlaylists()
    {
        var playlists = await _playlistService.GetPlaylistsAsync();
        Playlists = new ObservableCollection<PlaylistDTO>(playlists);
        SelectedPlaylist = Playlists.FirstOrDefault(); // Tự động chọn playlist đầu tiên nếu cần
    }

    public async Task LoadSelectedPlaylist(string playlistId)
    {
        var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
        if (playlist != null)
        {
            SelectedPlaylist = playlist;
        }
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
