using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spotify.ViewModels;

namespace Spotify.Views;

/// <summary>
/// A page that handles playback control functionality.
/// </summary>
public sealed partial class PlaybackControlPage : Page
{
    /// <summary>
    /// Gets the view model for the playback control page.
    /// </summary>
    public PlaybackControlViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackControlPage"/> class.
    /// </summary>
    public PlaybackControlPage()
    {
        this.InitializeComponent();
        PlaybackControlViewModel.Initialize();
        ViewModel = PlaybackControlViewModel.Instance;
        DataContext = ViewModel;
    }
}
