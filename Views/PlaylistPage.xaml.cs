using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;

namespace Spotify.Views
{
    public sealed partial class PlaylistPage : Page
    {
        public PlaylistPageViewModel PlaylistPageVM { get; set; }

        public PlaylistPage()
        {
            this.InitializeComponent();

            var playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();
            var playlistSongService = (App.Current as App).Services.GetRequiredService<PlaylistSongService>();

            PlaylistPageVM = new PlaylistPageViewModel(playlistService, playlistSongService);
            DataContext = PlaylistPageVM;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is string playlistId)
            {
                await PlaylistPageVM.LoadSelectedPlaylist(playlistId);
            }
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            // TODO: Thêm logic nút Play
        }

        private async void OnRemovePlaylistClick(object sender, RoutedEventArgs e)
        {
            if (PlaylistPageVM.SelectedPlaylist != null)
            {
                string playlistId = PlaylistPageVM.SelectedPlaylist.Id;
                await PlaylistPageVM.RemovePlaylistAsync(playlistId);
            }
        }
    }
}
