using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LeftSidebarPage : Page
    {
        public LeftSidebarPage()
        {
            this.InitializeComponent();
        }

        // Thêm phương thức OnLikedSongsButtonClick
        private void OnLikedSongButtonClick(object sender, TappedRoutedEventArgs e)
        {
            var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
            var mainFrame = shellWindow.getMainFrame();
            //shellWindow.NavigateToPage(typeof(LikedSongPage), mainFrame);
            shellWindow.GetNavigationService().Navigate(typeof(LikedSongPage));
        }

        // Thêm phương thức OnPlaylistButtonClick
        private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            // Truy cập đến MainFrame và điều hướng đến PlaylistPage
            var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
            var mainFrame = shellWindow.getMainFrame();
            //shellWindow.NavigateToPage(typeof(PlaylistPage), mainFrame);
            shellWindow.GetNavigationService().Navigate(typeof(PlaylistPage));
        }

        private async void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement add playlist functionality
            await CreatePlaylistDialog.ShowAsync();
        }

        // Sự kiện khi nhấn nút "Create" trong ContentDialog
        private void OnCreatePlaylistDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string playlistName = PlaylistNameTextBox.Text;

            // Kiểm tra tên playlist
            if (string.IsNullOrWhiteSpace(playlistName))
            {
                args.Cancel = true; // Hủy nếu tên trống
                return;
            }

            // Logic để tạo playlist mới có thể được thêm vào đây
            System.Diagnostics.Debug.WriteLine($"Playlist created: {playlistName}");
            // Bạn có thể thêm mã để lưu playlist mới vào cơ sở dữ liệu hoặc danh sách
        }

        private void OnArrowButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement arrow button functionality
        }



    }
}
