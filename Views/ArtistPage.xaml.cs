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

namespace Spotify.Views
{
    public sealed partial class ArtistPage : Page
    {
        public ArtistViewModel ViewModel { get; }

        public ArtistPage()
        {
            var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();
            var songService = (App.Current as App).Services.GetRequiredService<SongService>();
            ViewModel = new ArtistViewModel(artistService, songService);
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ArtistDTO artist)
            {
                // Await InitializeAsync to load artist and songs
                await ViewModel.InitializeAsync(artist.Id.ToString());
            }
        }

        private void OnPlayClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Logic to play the song or handle play button click
        }

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
    }
}
