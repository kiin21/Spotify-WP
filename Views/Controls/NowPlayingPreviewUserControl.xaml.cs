using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views.Controls;

public sealed partial class NowPlayingPreviewUserControl : UserControl
{
    public PlaybackControlViewModel ViewModel { get; }

    public NowPlayingPreviewUserControl()
    {
        ViewModel = App.Current.Services.GetRequiredService<PlaybackControlViewModel>();
        this.InitializeComponent();
        DataContext = ViewModel;
    }
}