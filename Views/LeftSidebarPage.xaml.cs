using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

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

        // Thêm phương thức OnPlaylistButtonClick
        private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            // Truy cập đến MainFrame và điều hướng đến PlaylistPage
            var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
            var mainFrame = shellWindow.getMainFrame();
            //shellWindow.NavigateToPage(typeof(PlaylistPage), mainFrame);
            shellWindow.GetNavigationService().Navigate(typeof(PlaylistPage));
        }
        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement add playlist functionality
        }

        private void OnArrowButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Implement arrow button functionality
        }

        private void OnLikedSongButtonClick(object sender, TappedRoutedEventArgs e)
        {
            var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
            var mainFrame = shellWindow.getMainFrame();
            //shellWindow.NavigateToPage(typeof(LikedSongPage), mainFrame);
            shellWindow.GetNavigationService().Navigate(typeof(LikedSongPage));
        }
    }
}
