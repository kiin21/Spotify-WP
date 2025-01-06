using Spotify.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Services;
using Spotify.Models.DTOs;

namespace Spotify.Views;

/// <summary>
/// A page that displays the main panel, including a list of songs.
/// </summary>
public sealed partial class MainPanelPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the main panel page.
    /// </summary>
    public MainPanelViewModel ViewModel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPanelPage"/> class.
    /// </summary>
    public MainPanelPage()
    {
        this.InitializeComponent();
        var songService = (App.Current as App).Services.GetRequiredService<SongService>();
        ViewModel = new MainPanelViewModel(songService);
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Handles the item click event for the song list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Item_Selected(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongDTO song)
        {
            Frame.Navigate(typeof(SongDetailPage), song);
        }
    }

    /// <summary>
    /// Handles the click event for the add to queue button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void AddToQueueItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is SongDTO song)
        {
            ViewModel.AddToQueueCommand(song);
        }
    }
}
