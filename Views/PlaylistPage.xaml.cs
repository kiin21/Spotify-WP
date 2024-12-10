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

public sealed partial class PlaylistPage : Page
{
    public PlaylistPageViewModel PlaylistPageVM { get; set; }
    public PlaybackControlViewModel PlaybackControlViewModel;
    public LeftSidebarPageViewModel LeftSidebarPageVM { get; set; }
    string PlayPauseGlyph;
    public string NotificationText { get; set; }

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

    private void OnPlayClick(object sender, RoutedEventArgs e)
    {
        // TODO: Thêm logic nút Play
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
            var playlistId = PlaylistPageVM.SelectedPlaylist.Id;

            // Gọi service để xóa bài hát khỏi playlist
            var _playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();
            await _playlistSongDetailService.RemoveSongFromPlaylistAsync(playlistId, songDetail.SongId);

            // Cập nhật danh sách bài hát trong playlist
            await PlaylistPageVM.LoadPlaylistSongs(playlistId);

            // Hiển thị thông báo
            NotificationTextBlock.Text = $"'{songDetail.SongTitle}' has been removed";
            NotificationTextBlock.Visibility = Visibility.Visible;

            // Tự động ẩn sau 3 giây
            await Task.Delay(3000);
            NotificationTextBlock.Visibility = Visibility.Collapsed;
        }
    }

    private async void OnAddToLikedSongsClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuFlyoutItem &&
            menuFlyoutItem.DataContext is PlaylistSongDetailDTO songDetail)
        {
            var _playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
            var _playlistSongDetailService = (App.Current as App).Services.GetRequiredService<PlaylistSongDetailService>();

            var userId = (App.Current as App).CurrentUser.Id;

            var likedSongsPlaylist = await _playlistService.GetLikedSongsPlaylistAsync(userId);

            // Gọi service để thêm bài hát vào danh sách yêu thích
            await _playlistSongDetailService.AddSongToPlaylistAsync(likedSongsPlaylist.Id, songDetail);

            // Hiển thị thông báo
            NotificationTextBlock.Text = $"'{songDetail.SongTitle}' has been added to Liked Songs!";
            NotificationTextBlock.Visibility = Visibility.Visible;

            // Tự động ẩn sau 3 giây
            await Task.Delay(3000);
            NotificationTextBlock.Visibility = Visibility.Collapsed;
        }
    }

    private async void OnSharePlaylistClick(object sender, RoutedEventArgs e)
    {
        var playlist = PlaylistPageVM.SelectedPlaylist;


        // Mở dialog SharePlaylistDialog
        var dialog = new SharePlaylistDialog(playlist) { XamlRoot = this.XamlRoot };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await dialog.SharePlaylistViewModel.SharePlaylistAsync();
        }
    }

    private void Notification_Closed(object sender, EventArgs e)
    {
        NotificationText = string.Empty;
    }
}
