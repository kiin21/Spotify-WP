using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class RightSideBarPage : Page
{
    public PlaybackControlViewModel ViewModel;
    public RightSideBarPage()
    {
        PlaybackControlViewModel.Initialize();
        ViewModel = PlaybackControlViewModel.Instance;
        this.InitializeComponent();
        DataContext = ViewModel;
    }
}
