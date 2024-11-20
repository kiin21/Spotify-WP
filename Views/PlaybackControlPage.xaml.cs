using System;
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

        // Initialize the service with the dispatcher from UI thread
        PlaybackControlService.Initialize(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());

        ViewModel = new PlaybackControlViewModel();
        DataContext = ViewModel;
    }

    private void Slider_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // Chuyển vào trạng thái đang tìm kiếm (seeking)
        ViewModel.OnSliderDragStarted();
    }

    private void Slider_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var slider = (Slider)sender;
        ViewModel.CurrentPosition = TimeSpan.FromSeconds(slider.Value);
        ViewModel.OnSliderDragCompleted();
    }
}