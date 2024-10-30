using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;
using System;

namespace Spotify.Views
{
    public sealed partial class LeftSidebarPage : Page
    {
        public PlaylistViewModel ViewModel { get; set; }

        public LeftSidebarPage()
        {
            this.InitializeComponent();
            var playlistService = (App.Current as App).Services.GetService<PlaylistService>();
            ViewModel = new PlaylistViewModel(playlistService);
            this.DataContext = ViewModel;
        }

        // Phương thức xử lý sự kiện khi click vào playlist
        private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            // Lấy playlist được chọn từ ListView
            var selectedPlaylist = (sender as ListViewItem)?.DataContext as PlaylistDTO;
            if (selectedPlaylist != null)
            {
                // Điều hướng sang PlaylistPage với ID của playlist
                var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
                shellWindow?.GetNavigationService().Navigate(typeof(PlaylistPage), selectedPlaylist.Id.ToString());
            }
        }

        private void OnArrowButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement arrow button functionality
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement add button functionality
        }
    }
}
