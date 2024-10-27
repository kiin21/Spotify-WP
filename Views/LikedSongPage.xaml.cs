using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Services;
using Spotify.ViewModels;
using Spotify.Models.DTOs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LikedSongPage : Page
    {
        public LikedSongViewModel ViewModel { get; }

        public LikedSongPage()
        {
            this.InitializeComponent();

            // Resolve LikedSongService via DI
            var likedSongService = (App.Current as App).Services.GetRequiredService<LikedSongService>();
            ViewModel = new LikedSongViewModel(likedSongService);
            DataContext = ViewModel;
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement play button functionality
        }

        private void OnRemoveFromLikedSongsClick(object sender, RoutedEventArgs e)
        {
            // Thêm logic xóa bài hát khỏi danh sách yêu thích
        }

        private void OnGoToArtistClick(object sender, RoutedEventArgs e)
        {
            // Thêm logic để chuyển tới trang nghệ sĩ
        }
    }
}
