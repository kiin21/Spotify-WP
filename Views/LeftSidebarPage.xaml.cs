using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using System;

namespace Spotify.Views;

/// <summary>
/// A page that displays the left sidebar, including playlists.
/// </summary>
public sealed partial class LeftSidebarPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the left sidebar page.
    /// </summary>
    public LeftSidebarPageViewModel ViewModel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeftSidebarPage"/> class.
    /// </summary>
    public LeftSidebarPage()
    {
        this.InitializeComponent();
        // Get instance of LeftSidebarPageViewModel from DI container
        ViewModel = (App.Current as App).Services.GetRequiredService<LeftSidebarPageViewModel>();
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeftSidebarPage"/> class with the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to use.</param>
    public LeftSidebarPage(LeftSidebarPageViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel; // Ensure viewModel is passed in
    }

    /// <summary>
    /// Handles the click event for a playlist button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is ListViewItem item && item.DataContext is PlaylistDTO selectedPlaylist)
        {
            // Set selected playlist before navigating
            ViewModel.SelectedPlaylist = selectedPlaylist;
            var shellWindow = (App.Current as App).ShellWindow;
            shellWindow?.GetNavigationService().Navigate(typeof(PlaylistPage), selectedPlaylist.Id);
        }
    }

    /// <summary>
    /// Handles the click event for the arrow button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnArrowButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: implement this function
    }

    /// <summary>
    /// Handles the click event for the add button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
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


