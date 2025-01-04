using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;
using System.Linq;
using System.Diagnostics;
using Spotify.Models.DTOs;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

namespace Spotify.Views;

/// <summary>
/// A page that displays artist details and their songs.
/// </summary>
public sealed partial class ArtistPage : Page
{
    /// <summary>
    /// Gets the view model for the artist page.
    /// </summary>
    public ArtistViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistPage"/> class.
    /// </summary>
    public ArtistPage()
    {
        var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();
        var songService = (App.Current as App).Services.GetRequiredService<SongService>();
        var userService = (App.Current as App).Services.GetRequiredService<UserService>();

        ViewModel = new ArtistViewModel(artistService, songService);
        this.InitializeComponent();
        this.DataContext = ViewModel;

        // Register the ArtistFollowStatusChanged event
        userService.ArtistFollowStatusChanged += OnArtistFollowStatusChanged;
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is ArtistDTO artist)
        {
            await ViewModel.InitializeAsync(artist.Id.ToString());
            ViewModel.Artist = artist;

            var currentUser = (App.Current as App).CurrentUser;
            FollowButton.Content = currentUser.FollowArtist.Contains(artist.Id.ToString()) ? "Followed" : "Follow";
        }
    }

    /// <summary>
    /// Handles the ArtistFollowStatusChanged event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="artistId">The ID of the artist whose follow status changed.</param>
    private void OnArtistFollowStatusChanged(object sender, string artistId)
    {
        if (ViewModel.Artist != null && ViewModel.Artist.Id.ToString() == artistId)
        {
            var currentUser = (App.Current as App).CurrentUser;
            FollowButton.Content = currentUser.FollowArtist.Contains(artistId) ? "Followed" : "Follow";
        }
    }

    /// <summary>
    /// Handles the click event of the follow button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnFollowClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Artist != null)
        {
            var userService = (App.Current as App).Services.GetRequiredService<UserService>();
            await userService.ToggleFollowArtistAsync(ViewModel.Artist.Id.ToString());
            // The button will automatically update when the event is triggered
        }
    }

    /// <summary>
    /// Handles the tapped event of a song item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnSongTapped(object sender, TappedRoutedEventArgs e)
    {
        // Check if sender is a FrameworkElement and has a DataContext of SongDTO
        if (sender is FrameworkElement element && element.DataContext is SongDTO song)
        {
            var songService = (App.Current as App).Services.GetRequiredService<SongService>();
            SongDTO songDetails = await songService.GetSongByIdAsync(song.Id.ToString());

            if (songDetails != null)
            {
                // Navigate to the SongDetailPage with the song details
                Frame.Navigate(typeof(SongDetailPage), songDetails);
            }
        }
    }

    /// <summary>
    /// Handles the click event of the play button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnPlayClick(object sender, RoutedEventArgs e)
    {
        // TODO: Implement play functionality
    }
}
