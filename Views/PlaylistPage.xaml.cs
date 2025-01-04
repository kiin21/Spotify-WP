using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;
using System.Linq;
using System.Diagnostics;
using Spotify.Models.DTOs;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Windows.UI.Notifications;
using System;
using System.Threading.Tasks;
using Catel.MVVM;

namespace Spotify.Views;

/// <summary>
/// A page that displays the playlist details and allows interaction with the playlist.
/// </summary>
public sealed partial class PlaylistPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the playlist page.
    /// </summary>
    public PlaylistPageViewModel PlaylistPageVM { get; set; }

    /// <summary>
    /// Gets the view model for playback control.
    /// </summary>
    public PlaybackControlViewModel PlaybackControlViewModel;

    /// <summary>
    /// Gets or sets the view model for the left sidebar page.
    /// </summary>
    public LeftSidebarPageViewModel LeftSidebarPageVM { get; set; }

    /// <summary>
    /// Gets or sets the play/pause glyph.
    /// </summary>
    string PlayPauseGlyph;

    /// <summary>
    /// Gets or sets the notification text.
    /// </summary>
    public string NotificationText { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistPage"/> class.
    /// </summary>
    public PlaylistPage()
    {
        this.InitializeComponent();

        var playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
        var playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();
        LeftSidebarPageVM = (App.Current as App).Services.GetRequiredService<LeftSidebarPageViewModel>();
        PlaybackControlViewModel = PlaybackControlViewModel.Instance;

        PlaylistPageVM = new PlaylistPageViewModel(playlistService, playlistSongDetailService);
        DataContext = PlaylistPageVM;  // Ensure DataContext is PlaylistPageVM
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is string playlistId)
        {
            await PlaylistPageVM.LoadSelectedPlaylist(playlistId);
        }
    }

    /// <summary>
    /// Handles the click event for removing a playlist.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnRemovePlaylistClick(object sender, RoutedEventArgs e)
    {
        if (PlaylistPageVM.SelectedPlaylist != null)
        {
            // Store the playlist ID to remove and find the current index
            var playlistIdToRemove = PlaylistPageVM.SelectedPlaylist.Id;
            int currentIndex = LeftSidebarPageVM.Playlists.ToList().FindIndex(p => p.Id == playlistIdToRemove);

            // Remove playlist in database and update PlaylistPage's state
            await PlaylistPageVM.RemoveSelectedPlaylist();

            // Update LeftSidebar's UI
            LeftSidebarPageVM.RemovePlaylist(playlistIdToRemove);

            // Determine the closest remaining playlist to navigate to
            PlaylistDTO closestPlaylist = null;
            if (currentIndex >= 0 && LeftSidebarPageVM.Playlists.Count > 0)
            {
                if (currentIndex < LeftSidebarPageVM.Playlists.Count)
                {
                    closestPlaylist = LeftSidebarPageVM.Playlists[currentIndex];
                }
                else if (currentIndex - 1 >= 0)
                {
                    closestPlaylist = LeftSidebarPageVM.Playlists[currentIndex - 1];
                }
            }

            // Navigate to the closest playlist
            if (closestPlaylist != null)
            {
                LeftSidebarPageVM.SelectedPlaylist = closestPlaylist;
                await PlaylistPageVM.LoadSelectedPlaylist(closestPlaylist.Id);
            }
        }
    }


    /// <summary>
    /// Handles the tapped event for a song item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnSongTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is PlaylistSongDetailDTO songDetail)
        {
            // Find or create a SongDTO object from songDetail
            var songId = songDetail.SongId;
            var songService = (App.Current as App).Services.GetRequiredService<SongService>();

            // Assume SongService has a GetSongById method to fetch song data from the database
            SongDTO song = await songService.GetSongByIdAsync(songId);

            if (song != null)
            {
                // Navigate to SongDetailPage with the song information
                Frame.Navigate(typeof(SongDetailPage), song);
            }
        }
    }

    /// <summary>
    /// Handles the tapped event for an artist item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnArtistTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is PlaylistSongDetailDTO songDetail)
        {
            var artistName = songDetail.Artist;
            var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();

            ArtistDTO artist = await artistService.GetArtistByNameAsync(artistName);

            if (artist != null)
            {
                Frame.Navigate(typeof(ArtistPage), artist);
            }
        }
    }

    /// <summary>
    /// Handles the click event for the play button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnPlayClick(object sender, RoutedEventArgs e)
    {
        // TODO: Add play button logic
    }

    /// <summary>
    /// Handles the click event for the more options button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnMoreOptionsClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            var selectedSong = button.DataContext as PlaylistSongDetailDTO;
            if (selectedSong != null)
            {
                // Show menu or perform other actions
                ShowMoreOptionsMenu(selectedSong);
            }
        }
    }

    /// <summary>
    /// Shows the more options menu for the specified song.
    /// </summary>
    /// <param name="song">The song for which to show more options.</param>
    private void ShowMoreOptionsMenu(PlaylistSongDetailDTO song)
    {
        Debug.WriteLine($"More options for: {song.SongTitle}");
    }

    /// <summary>
    /// Handles the pointer entered event for an item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnItemPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement element &&
            element.FindName("MoreOptionsButton") is Button moreOptionsButton)
        {
            moreOptionsButton.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Handles the pointer exited event for an item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnItemPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement element &&
            element.FindName("MoreOptionsButton") is Button moreOptionsButton)
        {
            moreOptionsButton.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Handles the click event for removing a song from the playlist.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnRemoveFromPlaylistClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuFlyoutItem &&
            menuFlyoutItem.DataContext is PlaylistSongDetailDTO songDetail)
        {
            var playlistId = PlaylistPageVM.SelectedPlaylist.Id;

            // Call service to remove the song from the playlist
            var _playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();
            await _playlistSongDetailService.RemoveSongFromPlaylistAsync(playlistId, songDetail.SongId);

            // Update the list of songs in the playlist
            await PlaylistPageVM.LoadPlaylistSongs(playlistId);

            // Show notification
            NotificationTextBlock.Text = $"'{songDetail.SongTitle}' has been removed";
            NotificationTextBlock.Visibility = Visibility.Visible;

            // Auto-hide after 3 seconds
            await Task.Delay(3000);
            NotificationTextBlock.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Handles the click event for adding a song to liked songs.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnAddToLikedSongsClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuFlyoutItem &&
            menuFlyoutItem.DataContext is PlaylistSongDetailDTO songDetail)
        {
            var _playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
            var _playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();

            var userId = (App.Current as App).CurrentUser.Id;

            var likedSongsPlaylist = await _playlistService.GetLikedSongsPlaylistAsync(userId);

            // Call service to add the song to the liked songs playlist
            await _playlistSongDetailService.AddSongToPlaylistAsync(likedSongsPlaylist.Id, songDetail);

            // Show notification
            NotificationTextBlock.Text = $"'{songDetail.SongTitle}' has been added to Liked Songs!";
            NotificationTextBlock.Visibility = Visibility.Visible;

            // Auto-hide after 3 seconds
            await Task.Delay(3000);
            NotificationTextBlock.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Handles the click event for sharing a playlist.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnSharePlaylistClick(object sender, RoutedEventArgs e)
    {
        var playlist = PlaylistPageVM.SelectedPlaylist;

        // Open the SharePlaylistDialog
        var dialog = new SharePlaylistDialog(playlist) { XamlRoot = this.XamlRoot };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await dialog.SharePlaylistViewModel.SharePlaylistAsync();
        }
    }

    /// <summary>
    /// Handles the event when the notification is closed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Notification_Closed(object sender, EventArgs e)
    {
        NotificationText = string.Empty;
    }
}
