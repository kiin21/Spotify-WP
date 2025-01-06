using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;

namespace Spotify.Views;

/// <summary>
/// A page that displays the details of a song and allows interaction with playback controls.
/// </summary>
public sealed partial class SongDetailPage : Page
{
    /// <summary>
    /// Gets or sets the view model for the song detail page.
    /// </summary>
    private SongDetailViewModel ViewModel { get; set; }

    /// <summary>
    /// Gets the view model for playback control.
    /// </summary>
    private PlaybackControlViewModel PlaybackViewModel = PlaybackControlViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="SongDetailPage"/> class.
    /// </summary>
    public SongDetailPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Get the song passed during navigation
        if (e.Parameter is SongDTO song)
        {
            var commentService = (App.Current as App).Services.GetRequiredService<CommentService>();

            // Create ViewModel with the song
            ViewModel = new SongDetailViewModel(song, commentService);
            DataContext = ViewModel;
        }
    }

    /// <summary>
    /// Handles the click event of the back button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Navigate back
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}
