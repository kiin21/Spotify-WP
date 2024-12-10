// HeaderPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Services;
using Spotify.Contracts.Services;
using Microsoft.UI.Xaml;
using Spotify.Models.DTOs;
using System.Diagnostics;

namespace Spotify.Views;
public sealed partial class HeaderPage : Page
{
    public HeaderViewModel ViewModel { get; }

    public HeaderPage()
    {
        this.InitializeComponent();

        var songService = (App.Current as App).Services.GetRequiredService<SongService>();
        var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();
        var userService = (App.Current as App).Services.GetRequiredService<UserService>();

        ViewModel = new HeaderViewModel(songService, artistService, userService);
        DataContext = ViewModel;

        // Xử lý sự kiện khi HeaderPage được tải
        this.Loaded += HeaderPage_Loaded;
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel.SearchCommand.CanExecute(null))
        {
            ViewModel.SearchCommand.Execute(null);
        }
    }

    private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            if (ViewModel.SearchCommand.CanExecute(null))
            {
                ViewModel.SearchCommand.Execute(null);
            }
        }
    }

    private async void BackToHomePage_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var shellWindow = App.Current.ShellWindow;

        // Làm mới dữ liệu trong HeaderPage
        await ViewModel.CheckForSongUpdatesAsync();

        shellWindow.GetNavigationService().Navigate(typeof(MainPanelPage));
    }

    private async void HeaderPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Kiểm tra và cập nhật thông báo
        await ViewModel.CheckForSongUpdatesAsync();
    }

    private void NotificationButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.HasNotification = false;
    }

    private async void NotificationList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is NotificationDTO notification && !string.IsNullOrEmpty(notification.ArtistId))
        {
            var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();

            // Truy vấn Artist từ ArtistId
            var artist = await artistService.GetArtistByIdAsync(notification.ArtistId);

            if (artist != null)
            {
                var shellWindow = App.Current.ShellWindow;

                // Điều hướng đến ArtistPage và truyền artist
                shellWindow.GetNavigationService().Navigate(typeof(ArtistPage), artist);
            }
        }
    }

    private void ExplorePremium_Click(object sender, RoutedEventArgs e)
    {
        var shellWindow = App.Current.ShellWindow;

        shellWindow.GetNavigationService().Navigate(typeof(PremiumPage));
    }
}
