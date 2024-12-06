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

namespace Spotify.Views;

public sealed partial class PlaylistPage : Page
{
    public PlaylistPageViewModel PlaylistPageVM { get; set; }
    public PlaybackControlViewModel PlaybackControlViewModel;
    public LeftSidebarPageViewModel LeftSidebarPageVM { get; set; }
    string PlayPauseGlyph;

    public PlaylistPage()
    {
        this.InitializeComponent();

        var playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
        var playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();
        LeftSidebarPageVM = (App.Current as App).Services.GetRequiredService<LeftSidebarPageViewModel>();
        PlaybackControlViewModel = PlaybackControlViewModel.Instance;

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

    private async void OnSongTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is PlaylistSongDetailDTO songDetail)
        {
            // Tìm hoặc tạo đối tượng SongDTO từ songDetail
            var songId = songDetail.SongId;
            var songService = (App.Current as App).Services.GetRequiredService<SongService>();

            // Giả sử SongService có phương thức GetSongById để lấy dữ liệu bài hát từ cơ sở dữ liệu
            SongDTO song = await songService.GetSongByIdAsync(songId);

            if (song != null)
            {
                // Điều hướng đến SongDetailPage với thông tin bài hát
                Frame.Navigate(typeof(SongDetailPage), song);
            }
        }
    }



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

    private void OnMoreOptionsClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            var selectedSong = button.DataContext as PlaylistSongDetailDTO;
            if (selectedSong != null)
            {
                // Hiển thị menu hoặc thực hiện các hành động khác
                ShowMoreOptionsMenu(selectedSong);
            }
        }
    }

    private void ShowMoreOptionsMenu(PlaylistSongDetailDTO song)
    {

        Debug.WriteLine($"More options for: {song.SongTitle}");
    }

    private void OnItemPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement element &&
            element.FindName("MoreOptionsButton") is Button moreOptionsButton)
        {
            moreOptionsButton.Visibility = Visibility.Visible;
        }
    }

    private void OnItemPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement element &&
            element.FindName("MoreOptionsButton") is Button moreOptionsButton)
        {
            moreOptionsButton.Visibility = Visibility.Collapsed;
        }
    }

    private async void OnRemoveFromPlaylistClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuFlyoutItem &&
            menuFlyoutItem.DataContext is PlaylistSongDetailDTO songDetail)
        {
            //// Gọi service để xóa bài hát khỏi playlist
            //await PlaylistPageVM.RemoveSongFromPlaylist(songDetail);

            //// Cập nhật danh sách bài hát trong playlist
            //await PlaylistPageVM.LoadSelectedPlaylist(PlaylistPageVM.SelectedPlaylist.Id);

            Debug.WriteLine($"Removed {songDetail.SongTitle} from playlist.");
        }
    }

    private async void OnAddToLikedSongsClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuFlyoutItem &&
            menuFlyoutItem.DataContext is PlaylistSongDetailDTO songDetail)
        {
            //var likedSongsService = (App.Current as App).Services.GetRequiredService<LikedSongsService>();

            //// Gọi service để thêm bài hát vào danh sách yêu thích
            //await likedSongsService.AddToLikedSongsAsync(songDetail.SongId);

            Debug.WriteLine($"Added {songDetail.SongTitle} to Liked Songs.");
        }
    }
}
