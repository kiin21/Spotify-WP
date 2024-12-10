// HeaderViewModel.cs
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Spotify.Helpers;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Windows.UI.Notifications;

namespace Spotify.ViewModels;

/// <summary>
/// ViewModel for managing the header section, including search functionality and notifications.
/// </summary>
public partial class HeaderViewModel : INotifyPropertyChanged
{
    private string _searchQuery;
    private readonly SongService _songService;
    private ObservableCollection<SongDTO> _searchResults;

    /// <summary>
    /// Gets or sets the user's avatar.
    /// </summary>
    public string UserAvatar { get; set; } = "../Assets/defaultAvt.jpg";

    private readonly ArtistService _artistService;
    private readonly UserService _userService;
    private readonly PlayHistoryService _playHistoryService;

    /// <summary>
    /// Gets or sets the search results.
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

    /// <summary>
    /// Gets or sets the search query.
    /// </summary>
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged(nameof(SearchQuery));
        }
    }

    /// <summary>
    /// Gets the collection of notifications.
    /// </summary>
    public ObservableCollection<NotificationDTO> Notifications { get; } = new ObservableCollection<NotificationDTO>();

    private bool _hasNotification;

    /// <summary>
    /// Gets or sets a value indicating whether there are notifications.
    /// </summary>
    public bool HasNotification
    {
        get => _hasNotification;
        set
        {
            _hasNotification = value;
            OnPropertyChanged(nameof(HasNotification));
        }
    }

    /// <summary>
    /// Gets the command for executing a search.
    /// </summary>
    public ICommand SearchCommand { get; }

    /// <summary>
    /// Gets the command for showing the play history.
    /// </summary>
    public ICommand ShowHistoryCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderViewModel"/> class.
    /// </summary>
    /// <param name="songService">The song service.</param>
    /// <param name="artistService">The artist service.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="playHistoryService">The play history service.</param>
    public HeaderViewModel(SongService songService, ArtistService artistService, UserService userService, PlayHistoryService playHistoryService)
    {
        SearchCommand = new RelayCommand(async _ => await ExecuteSearchAsync(), _ => CanExecuteSearch());
        ShowHistoryCommand = new RelayCommand(async _ => await ExecuteShowHistoryAsync(), _ => CanExecuteShowHistory());
        _songService = songService;
        SearchResults = new ObservableCollection<SongDTO>();

        _artistService = artistService;
        _userService = userService;
        _playHistoryService = playHistoryService;
    }

    /// <summary>
    /// Executes the show history command asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ExecuteShowHistoryAsync()
    {
        List<PlayHistoryWithSongDTO> history = await _playHistoryService.GetUserHistoryAsync(App.Current.CurrentUser.Id);
        var shellWindow = (App.Current as App).ShellWindow;
        var mainFrame = shellWindow.getMainFrame();
        shellWindow.GetNavigationService().Navigate(typeof(HistoryPage), history);
    }

    /// <summary>
    /// Determines whether the show history command can execute.
    /// </summary>
    /// <returns><c>true</c> if the command can execute; otherwise, <c>false</c>.</returns>
    private bool CanExecuteShowHistory()
    {
        if (App.Current.CurrentUser == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Executes the search command asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ExecuteSearchAsync()
    {
        if (SearchQuery == "")
        {
            var results = await _songService.GetAllSongs();
            SearchResults = new ObservableCollection<SongDTO>(results);
        }
        else
        {
            var results = await _songService.SearchSongs(SearchQuery);
            SearchResults = new ObservableCollection<SongDTO>(results);
        }

        var shellWindow = (App.Current as App).ShellWindow;
        var mainFrame = shellWindow.getMainFrame();
        shellWindow.GetNavigationService().Navigate(typeof(SearchResultsPage), SearchResults);
    }

    /// <summary>
    /// Determines whether the search command can execute.
    /// </summary>
    /// <returns><c>true</c> if the command can execute; otherwise, <c>false</c>.</returns>
    private bool CanExecuteSearch()
    {
        return SearchQuery != null;
    }

    /// <summary>
    /// Checks for song updates asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task CheckForSongUpdatesAsync()
    {
        var currentUser = App.Current.CurrentUser;
        if (currentUser == null || currentUser.FollowArtist == null) return;

        bool hasNewSongs = false;
        var newNotifications = new ObservableCollection<NotificationDTO>();

        // If no artists are followed
        if (!currentUser.FollowArtist.Any())
        {
            if (!Notifications.Any(n => n.Message == "Follow some artists to get updates!"))
            {
                Notifications.Add(new NotificationDTO
                {
                    Message = "Follow some artists to get updates!",
                    Timestamp = DateTime.Now,
                    ArtistName = "",
                    ArtistId = "",
                });
            }
            return;
        }

        foreach (var artistId in currentUser.FollowArtist)
        {
            var artist = await _artistService.GetArtistByIdAsync(artistId);
            if (artist == null) continue;

            // Get current cache
            var cachedSongs = _songService.GetCachedSongs(artistId);

            // If cache is empty, do not consider all as new songs
            if (!cachedSongs.Any())
            {
                _songService.UpdateCache(artistId, artist.SongIds);
                continue;
            }

            // Check for new songs
            var newSongIds = artist.SongIds.Except(cachedSongs).ToList();

            if (newSongIds.Any())
            {
                hasNewSongs = true;

                // Get details of new songs
                foreach (var songId in newSongIds)
                {
                    var newSong = await _songService.GetSongByIdAsync(songId);

                    // Create new notification
                    var notification = new NotificationDTO
                    {
                        Message = $"'{newSong.title}' was just released by {artist.Name}",
                        ArtistId = artist.Id.ToString(),
                        ArtistName = artist.Name,
                        Timestamp = DateTime.Now
                    };

                    newNotifications.Add(notification);
                }

                // Update cache
                _songService.UpdateCache(artistId, artist.SongIds);
            }
        }

        // If no new notifications, keep the old ones
        if (!hasNewSongs)
        {
            // If there is no "No new updates" notification, add it
            if (!Notifications.Any())
            {
                Notifications.Add(new NotificationDTO
                {
                    Message = "No messages",
                    ArtistName = "",
                    ArtistId = "",
                    Timestamp = DateTime.Now
                });
            }
        }
        else
        {
            // Remove default notification if exists
            var noUpdateNotification = Notifications.FirstOrDefault(n => n.Message == "No messages");
            if (noUpdateNotification != null)
            {
                Notifications.Remove(noUpdateNotification);
            }

            // Add new notifications at the beginning of the list
            foreach (var notification in newNotifications.Reverse())
            {
                Notifications.Insert(0, notification);
            }
        }

        HasNotification = hasNewSongs;
        OnPropertyChanged(nameof(Notifications));
        OnPropertyChanged(nameof(HasNotification));
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
