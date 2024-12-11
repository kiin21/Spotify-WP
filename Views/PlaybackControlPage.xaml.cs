using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class PlaybackControlPage : Page
{
    public PlaybackControlViewModel ViewModel { get; }

    public PlaybackControlPage()
    {

        this.InitializeComponent();
        //// Initialize the service with the dispatcher from UI thread
        //PlaybackControlService.Initialize(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());

        PlaybackControlViewModel.Initialize();
        ViewModel = PlaybackControlViewModel.Instance;
        DataContext = ViewModel;
    }
}