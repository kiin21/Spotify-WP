using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

public class PlaylistSongViewModel : INotifyPropertyChanged
{
    private readonly PlaylistSongService _playlistSongService;
    private ObservableCollection<PlaylistSongDTO> _playlistSongs;

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

    // New property to count songs
    public string SongCount => $"{PlaylistSongs?.Count ?? 0} bài hát";

    public PlaylistSongViewModel(PlaylistSongService playlistSongService, PlaylistViewModel playlistViewModel)
    {
        _playlistSongService = playlistSongService;
        playlistViewModel.SelectedPlaylistIdChanged += async (sender, playlistId) =>
        {
            await LoadPlaylistSongs(playlistId);
        };
    }

    private async Task LoadPlaylistSongs(string playlistId)
    {
        if (!string.IsNullOrEmpty(playlistId))
        {
            var songs = await _playlistSongService.GetSongsForPlaylistAsync(playlistId);
            PlaylistSongs = new ObservableCollection<PlaylistSongDTO>(songs);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
