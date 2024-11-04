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
            // Lấy instance của LeftSidebarPageViewModel từ DI container
            ViewModel = (App.Current as App).Services.GetRequiredService<LeftSidebarPageViewModel>();
            this.DataContext = ViewModel;
        }

        public LeftSidebarPage(LeftSidebarPageViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel; // Đảm bảo viewModel được truyền vào
        }

        private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
            {
                if (sender is ListViewItem item && item.DataContext is PlaylistDTO selectedPlaylist)
                {
                    // Set selected playlist trước khi navigate
                    ViewModel.SelectedPlaylist = selectedPlaylist;
                    var shellWindow = (App.Current as App).ShellWindow as ShellWindow;
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


