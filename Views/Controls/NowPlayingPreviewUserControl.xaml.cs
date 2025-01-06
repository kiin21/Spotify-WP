using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views.Controls;

/// <summary>
/// A user control that displays a preview of the currently playing song.
/// </summary>
public sealed partial class NowPlayingPreviewUserControl : UserControl
{
    /// <summary>
    /// Gets the view model for playback control.
    /// </summary>
    public PlaybackControlViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NowPlayingPreviewUserControl"/> class.
    /// </summary>
    public NowPlayingPreviewUserControl()
    {
        PlaybackControlViewModel.Initialize();
        ViewModel = PlaybackControlViewModel.Instance;
        this.InitializeComponent();
        DataContext = ViewModel;
    }
}
