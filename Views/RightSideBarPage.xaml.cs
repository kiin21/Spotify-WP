using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class RightSideBarPage : Page
{
    public PlaybackControlViewModel ViewModel = new PlaybackControlViewModel();
    public RightSideBarPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<PlaybackControlViewModel>();
        this.InitializeComponent();
        DataContext = ViewModel;
    }
}
