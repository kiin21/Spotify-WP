using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation; // Thêm dòng này
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;

namespace Spotify.Views
{
    public sealed partial class PlaylistPage : Page
    {
        public PlaylistViewModel PlaylistVM { get; set; }
        public PlaylistSongViewModel PlaylistSongVM { get; set; }

        public PlaylistPage()
        {
            this.InitializeComponent();

            var playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
            var playlistSongService = (App.Current as App).Services.GetRequiredService<PlaylistSongService>();

            PlaylistVM = new PlaylistViewModel(playlistService);
            PlaylistSongVM = new PlaylistSongViewModel(playlistSongService, PlaylistVM);

            DataContext = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Nhận playlistId từ tham số điều hướng
            if (e.Parameter is string playlistId)
            {
                // Tải thông tin playlist dựa trên playlistId
                await PlaylistVM.LoadSelectedPlaylist(playlistId);
            }
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            // TODO: Thêm logic nút Play
        }

        private void OnRemoveFromLikedSongsClick(object sender, RoutedEventArgs e)
        {
            // TODO: Thêm logic xóa bài hát khỏi danh sách yêu thích
        }

        private void OnGoToArtistClick(object sender, RoutedEventArgs e)
        {
            // TODO: Thêm logic chuyển tới trang nghệ sĩ
        }
    }
}
