using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Threading.Tasks;

/// <summary>
/// ViewModel for managing songs in a playlist.
/// </summary>
public class PlaylistSongViewModel : INotifyPropertyChanged
{
    private readonly PlaylistSongService _playlistSongService;
    private ObservableCollection<PlaylistSongDTO> _playlistSongs;

    /// <summary>
    /// Gets or sets the collection of songs in the playlist.
    /// </summary>
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

    /// <summary>
    /// Gets the count of songs in the playlist.
    /// </summary>
    public string SongCount => $"{PlaylistSongs?.Count ?? 0} bài hát";

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSongViewModel"/> class.
    /// </summary>
    /// <param name="playlistSongService">The service for managing playlist songs.</param>
    /// <param name="playlistViewModel">The ViewModel for managing playlists.</param>
    public PlaylistSongViewModel(PlaylistSongService playlistSongService, PlaylistViewModel playlistViewModel)
    {
        _playlistSongService = playlistSongService;
        playlistViewModel.SelectedPlaylistIdChanged += async (sender, playlistId) =>
        {
            await LoadPlaylistSongs(playlistId);
        };
    }

    /// <summary>
    /// Loads the songs for the specified playlist asynchronously.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadPlaylistSongs(string playlistId)
    {
        if (!string.IsNullOrEmpty(playlistId))
        {
            var songs = await _playlistSongService.GetSongsForPlaylistAsync(playlistId);
            PlaylistSongs = new ObservableCollection<PlaylistSongDTO>(songs);
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
