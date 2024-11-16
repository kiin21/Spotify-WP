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

namespace Spotify.Views
{
    public sealed partial class ArtistPage : Page
    {
        public ArtistViewModel ViewModel { get; }

        public ArtistPage()
        {
            var artistService = (App.Current as App).Services.GetRequiredService<ArtistService>();
            var songService = (App.Current as App).Services.GetRequiredService<SongService>();
            var userService = (App.Current as App).Services.GetRequiredService<UserService>();

            ViewModel = new ArtistViewModel(artistService, songService);
            this.InitializeComponent();
            this.DataContext = ViewModel;

            // Đăng ký sự kiện ArtistFollowStatusChanged
            userService.ArtistFollowStatusChanged += OnArtistFollowStatusChanged;
        }

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

        private void OnArtistFollowStatusChanged(object sender, string artistId)
        {
            if (ViewModel.Artist != null && ViewModel.Artist.Id.ToString() == artistId)
            {
                var currentUser = (App.Current as App).CurrentUser;
                FollowButton.Content = currentUser.FollowArtist.Contains(artistId) ? "Followed" : "Follow";
            }
        }

        private async void OnFollowClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Artist != null)
            {
                var userService = (App.Current as App).Services.GetRequiredService<UserService>();
                await userService.ToggleFollowArtistAsync(ViewModel.Artist.Id.ToString());
                // Nút tự động cập nhật khi sự kiện được kích hoạt
            }
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

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
