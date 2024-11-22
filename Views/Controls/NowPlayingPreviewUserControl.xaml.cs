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
        this.InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<PlaybackControlViewModel>();
        DataContext = ViewModel;
    }
}