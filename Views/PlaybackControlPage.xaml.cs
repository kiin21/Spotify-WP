using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spotify.Services;
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

    private void Slider_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (ViewModel.CurrentSong == null || ViewModel.TotalDurationSeconds <= 0)
        {
            Debug.WriteLine("Cannot seek: No song playing or invalid duration");
            return;
        }
        ViewModel.OnSliderDragStarted();
    }

    private void Slider_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (ViewModel.CurrentSong == null || ViewModel.TotalDurationSeconds <= 0)
        {
            Debug.WriteLine("Cannot seek: No song playing or invalid duration");
            return;
        }

        var slider = sender as Slider;
        if (slider != null)
        {
            ViewModel.CurrentPosition = TimeSpan.FromSeconds(slider.Value);
            ViewModel.OnSliderDragCompleted();
        }
    }

    private void Slider_Tapped(object sender, TappedRoutedEventArgs e)
    {

    }

    private void Slider_Tapped_1(object sender, TappedRoutedEventArgs e)
    {

    }
}