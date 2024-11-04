    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Spotify.Models.DTOs;
    using Spotify.Services;
    using Spotify.ViewModels;
    using System;

    namespace Spotify.Views;

    public sealed partial class LeftSidebarPage : Page
    {
        public LeftSidebarPageViewModel ViewModel { get; set; }

        public LeftSidebarPage()
        {
            this.InitializeComponent();
            var playlistService = (App.Current as App).Services.GetService<PlaylistService>();
            var playlistSongService = (App.Current as App).Services.GetService<PlaylistSongService>(); // Khởi tạo playlistSongService
            var playlistPageViewModel = new PlaylistPageViewModel(playlistService, playlistSongService);
            ViewModel = new LeftSidebarPageViewModel(playlistService, playlistPageViewModel);
            this.DataContext = ViewModel;
    }

        private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
        {
            // Truy cập đến MainFrame và điều hướng đến PlaylistPage
            //    var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
            // Get ShellWindow from App.Current directly
            //var shellWindow = (App.Current as App).ShellWindow;
            //var mainFrame = shellWindow.getMainFrame();
            //shellWindow.NavigateToPage(typeof(PlaylistPage), mainFrame);
            //shellWindow.GetNavigationService().Navigate(typeof(PlaylistPage));
            if (sender is ListViewItem item && item.DataContext is PlaylistDTO selectedPlaylist)
            {
                // Set selected playlist trước khi navigate
                ViewModel.SelectedPlaylist = selectedPlaylist;
            //    var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
                var shellWindow = (App.Current as App).ShellWindow;
                shellWindow?.GetNavigationService().Navigate(typeof(PlaylistPage), selectedPlaylist.Id);
            }
        }

        private void OnArrowButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private async void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPlaylistDialog { XamlRoot = this.XamlRoot };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string playlistName = dialog.PlaylistName;
                if (!string.IsNullOrEmpty(playlistName))
                {
                    await ViewModel.AddPlaylistAsync(playlistName);
                }
            }
        }
    }


