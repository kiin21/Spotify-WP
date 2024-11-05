using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;
using System.Linq;
using System.Diagnostics;
using Spotify.Models.DTOs;

namespace Spotify.Views;

public sealed partial class PlaylistPage : Page
{
    public PlaylistPageViewModel PlaylistPageVM { get; set; }

    public LeftSidebarPageViewModel LeftSidebarPageVM { get; set; }

    public PlaylistPage()
    {
        this.InitializeComponent();

        var playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
        var playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();
        LeftSidebarPageVM = (App.Current as App).Services.GetRequiredService<LeftSidebarPageViewModel>();

        PlaylistPageVM = new PlaylistPageViewModel(playlistService, playlistSongDetailService);
        DataContext = PlaylistPageVM;  // Đảm bảo DataContext là PlaylistPageVM
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is string playlistId)
        {
            await PlaylistPageVM.LoadSelectedPlaylist(playlistId);
        }
    }

    private void OnPlayClick(object sender, RoutedEventArgs e)
    {
        // TODO: Thêm logic nút Play
    }

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


}
