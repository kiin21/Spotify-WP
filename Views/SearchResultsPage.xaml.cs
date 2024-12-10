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

public sealed partial class SearchResultsPage : Page
{
    public SearchResultPageViewModel ViewModel { get; private set; }

    public SearchResultsPage()
    {
        this.InitializeComponent();
    }

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

    private void UpdateFlyout(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        // Lấy bài hát liên quan từ DataContext
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
                        Tag = playlist.Id // Gắn Playlist ID vào MenuItem
                    };

                    // Gắn sự kiện Click
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
    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongDTO song)
        {
            ViewModel.SongSelectedCommand.Execute(song);
        }
    }

}
