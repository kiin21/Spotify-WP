// SearchResultPageViewModel.cs
using Microsoft.UI.Xaml.Controls;
using Spotify.Helpers;
using Spotify.Models.DTOs;
using Spotify.Views;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using Spotify.Contracts.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Spotify.ViewModels;

/// <summary>
/// ViewModel for managing search results and interactions with playlists.
/// </summary>
public class SearchResultPageViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;

    private ObservableCollection<SongDTO> _searchResults;

    /// <summary>
    /// Gets the command to execute when a song is selected.
    /// </summary>
    public ICommand SongSelectedCommand { get; private set; }

    /// <summary>
    /// Gets or sets the collection of search results.
    /// </summary>
    public ObservableCollection<SongDTO> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged(nameof(SearchResults));
        }
    }

    private readonly PlaylistService _playlistService;

    private ObservableCollection<PlaylistDTO> _playlists = new ObservableCollection<PlaylistDTO>();
    /// <summary>
    /// Gets or sets the collection of playlists.
    /// </summary>
    public ObservableCollection<PlaylistDTO> Playlists
    {
        get => _playlists;
        set
        {
            _playlists = value;
            OnPropertyChanged(nameof(Playlists));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResultPageViewModel"/> class.
    /// </summary>
    /// <param name="searchResults">The collection of search results.</param>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="playlistService">The service for managing playlists.</param>
    public SearchResultPageViewModel(
        ObservableCollection<SongDTO> searchResults,
        INavigationService navigationService,
        PlaylistService playlistService)
    {
        SearchResults = searchResults;
        _navigationService = navigationService;
        _playlistService = playlistService;

        SongSelectedCommand = new RelayCommand(
            execute: param => NavigateToSongDetail(param as SongDTO),
            canExecute: param => param is SongDTO);

        // Load Playlists
        LoadPlaylistsAsync();
    }

    /// <summary>
    /// Loads the playlists asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadPlaylistsAsync()
    {
        try
        {
            var userId = (App.Current as App)?.CurrentUser?.Id;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("CurrentUser is null or userId is empty.");
                return;
            }

            var playlists = await _playlistService.GetPlaylistsByUserIdAsync(userId);

            if (playlists != null)
            {
                Playlists = new ObservableCollection<PlaylistDTO>(playlists);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading playlists: {ex.Message}");
        }
    }

    /// <summary>
    /// Navigates to the song detail page.
    /// </summary>
    /// <param name="song">The song to display details for.</param>
    private void NavigateToSongDetail(SongDTO song)
    {
        if (song != null)
        {
            _navigationService.Navigate(typeof(SongDetailPage), song);
        }
    }

    /// <summary>
    /// Adds a song to a playlist asynchronously.
    /// </summary>
    /// <param name="song">The song to add.</param>
    /// <param name="playlistId">The ID of the playlist to add the song to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the song or playlist ID is invalid.</exception>
    public async Task AddSongToPlaylistAsync(SongDTO song, string playlistId)
    {
        if (song == null || string.IsNullOrEmpty(playlistId))
        {
            throw new ArgumentException("Song or Playlist ID is invalid.");
        }

        try
        {
            // Create a song detail object
            var songDetail = new PlaylistSongDetailDTO
            {
                SongId = song.Id.ToString(),
                AddedBy = (App.Current as App)?.CurrentUser?.Username,
                Avatar = song.CoverArtUrl
            };

            // Call PlaylistSongDetailService to add the song to the playlist
            var playlistSongService = App.Current.Services.GetService<PlaylistSongDetailService>();
            await playlistSongService.AddSongToPlaylistAsync(playlistId, songDetail);

            System.Diagnostics.Debug.WriteLine($"Song '{song.title}' added to playlist '{playlistId}'.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding song to playlist: {ex.Message}");
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
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
