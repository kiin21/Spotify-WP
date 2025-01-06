//SearchResultPage.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Spotify.Views;

/// <summary>
/// A page that displays search results and allows interaction with playlists.
/// </summary>
public sealed partial class SearchResultsPage : Page
{
    /// <summary>
    /// Gets the view model for the search results page.
    /// </summary>
    public SearchResultPageViewModel ViewModel { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResultsPage"/> class.
    /// </summary>
    public SearchResultsPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var shellWindow = (App.Current as App).ShellWindow;
        var navigationService = shellWindow?.GetNavigationService();
        var playlistService = App.Current.Services.GetService<PlaylistService>();

        if (e.Parameter is ObservableCollection<SongDTO> searchResults)
        {
            ViewModel = new SearchResultPageViewModel(searchResults, navigationService, playlistService);
            DataContext = ViewModel;
        }
    }

    /// <summary>
    /// Updates the flyout menu with playlists.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void UpdateFlyout(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        // Get the related song from DataContext
        if (button?.DataContext is SongDTO selectedSong)
        {
            if (button.Flyout is MenuFlyout menuFlyout)
            {
                menuFlyout.Items.Clear();

                foreach (var playlist in ViewModel.Playlists)
                {
                    var menuItem = new MenuFlyoutItem
                    {
                        Text = $"Add to {playlist.Title}",
                        Tag = playlist.Id // Attach Playlist ID to MenuItem
                    };

                    // Attach Click event
                    menuItem.Click += async (s, args) =>
                    {
                        var selectedPlaylistId = (s as MenuFlyoutItem)?.Tag as string;
                        await AddSongToPlaylistAsync(selectedSong, selectedPlaylistId);
                    };

                    menuFlyout.Items.Add(menuItem);
                }
            }
        }
    }

    /// <summary>
    /// Adds a song to a playlist asynchronously.
    /// </summary>
    /// <param name="song">The song to add.</param>
    /// <param name="playlistId">The ID of the playlist to add the song to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task AddSongToPlaylistAsync(SongDTO song, string playlistId)
    {
        try
        {
            await ViewModel.AddSongToPlaylistAsync(song, playlistId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding song to playlist: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the item click event for the song list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongDTO song)
        {
            ViewModel.SongSelectedCommand.Execute(song);
        }
    }
}
