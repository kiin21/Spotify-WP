using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spotify.Services;
using Spotify.ViewModels;

namespace Spotify.Views
{
    public sealed partial class PlaybackControlPage : Page
    {
        public PlaybackControlViewModel ViewModel { get; }

        public PlaybackControlPage()
        {
            this.InitializeComponent();
            var playbackControlService = (App.Current as App).Services.GetRequiredService<PlaybackControlService>();
            ViewModel = new PlaybackControlViewModel(playbackControlService);
            DataContext = ViewModel;
        }

        private void Slider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var vm = (PlaybackControlViewModel)DataContext;
            vm.BeginSeeking();
        }

        private void Slider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var vm = (PlaybackControlViewModel)DataContext;
            vm.EndSeeking();
        }
    }
}