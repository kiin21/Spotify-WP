using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Windows;

namespace Spotify.Views
{
    public sealed partial class AddPlaylistDialog : ContentDialog
    {
        public string PlaylistName => PlaylistNameTextBox.Text;

        public AddPlaylistDialog()
        {
            this.InitializeComponent();
        }

        private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            var playlistService = (App.Current as App).Services.GetService<PlaylistService>();

            var newPlaylist = new PlaylistDTO
            {
                Title = PlaylistNameTextBox.Text,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "User" // Gán theo user hiện tại hoặc thông tin cần thiết
                                   // Gán các thuộc tính khác nếu cần
            };

            await playlistService.AddPlaylistAsync(newPlaylist);
            Hide(); // Đóng dialog sau khi lưu thành công
        }
    }
}
