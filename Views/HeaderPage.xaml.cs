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
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;
using System;

namespace Spotify.Views;
/// <summary>
/// A page that displays the header, including search functionality and notifications.
/// </summary>
public sealed partial class HeaderPage : Page
{
    /// <summary>
    /// Gets the view model for the header page.
    /// </summary>
    public HeaderViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderPage"/> class.
    /// </summary>
    public HeaderPage()
    {
        this.InitializeComponent();

        var songService = (App.Current as App).Services.GetRequiredService<SongService>();
        var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();
        var userService = (App.Current as App).Services.GetRequiredService<UserService>();
        var playHistoryService = (App.Current as App).Services.GetRequiredService<PlayHistoryService>();

        ViewModel = new HeaderViewModel(songService, artistService, userService, playHistoryService);
        DataContext = ViewModel;

        // Xử lý sự kiện khi HeaderPage được tải
        this.Loaded += HeaderPage_Loaded;
    }

    /// <summary>
    /// Executes the ShowHistoryCommand when the history button is clicked.
    /// </summary>
    private void ShowHistoryCommand(object sender, RoutedEventArgs e)
    {
        if(ViewModel.ShowHistoryCommand.CanExecute(null))
        {
            ViewModel.ShowHistoryCommand.Execute(null);
        }
    }

    /// <summary>
    /// Executes the LogoutCommand when the logout button is clicked.
    /// </summary>
    public async void LogoutCommand(object sender, RoutedEventArgs e)
    {
        if (ViewModel.LogoutCommand.CanExecute(null))
        {
            ViewModel.LogoutCommand.Execute(null);
            await ShowContentDialogAsync("Logout Successful", "You have successfully logged out.");
            // Sử dụng WindowManager để chuyển đổi sang ShellWindow
            Spotify.Services.WindowManager.Instance.SwitchToLoginSignupWindow();
        }
    }

    /// <summary>
    /// Executes the ShowWrappedCommand when the wrapped button is clicked.
    /// </summary>
    public void ShowWrappedCommand(object sender, RoutedEventArgs e)
    {
        if(ViewModel.ShowWrappedCommand.CanExecute(null))
        {
            ViewModel.ShowWrappedCommand.Execute(null);
        }
    }

    /// <summary>
    /// Executes the SearchCommand when the search text changes.
    /// </summary>
    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel.SearchCommand.CanExecute(null))
        {
            ViewModel.SearchCommand.Execute(null);
        }
    }

    /// <summary>
    /// Executes the SearchCommand when the Enter key is pressed in the search box.
    /// </summary>
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

    /// <summary>
    /// Navigates back to the home page when the home button is clicked.
    /// </summary>
    private async void BackToHomePage_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var shellWindow = App.Current.ShellWindow;

        // Làm mới dữ liệu trong HeaderPage
        await ViewModel.CheckForSongUpdatesAsync();

        shellWindow.GetNavigationService().Navigate(typeof(MainPanelPage));
    }

    /// <summary>
    /// Handles the Loaded event for the HeaderPage.
    /// </summary>
    private async void HeaderPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Kiểm tra và cập nhật thông báo
        await ViewModel.CheckForSongUpdatesAsync();
    }

    /// <summary>
    /// Handles the click event for the notification button.
    /// </summary>
    private void NotificationButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.HasNotification = false;
    }

    /// <summary>
    /// Handles the item click event for the notification list.
    /// </summary>
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

    /// <summary>
    /// Navigates to the PremiumPage when the explore premium button is clicked.
    /// </summary>
    private void ExplorePremium_Click(object sender, RoutedEventArgs e)
    {
        var shellWindow = App.Current.ShellWindow;

        shellWindow.GetNavigationService().Navigate(typeof(PremiumPage));
    }

    /// <summary>
    /// Shows a content dialog with the specified title and content.
    /// </summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="content">The content of the dialog.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ShowContentDialogAsync(string title, string content)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            CloseButtonText = "Ok",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}
