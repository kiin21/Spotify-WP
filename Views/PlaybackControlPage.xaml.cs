using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Catel.IoC;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Contracts.Services;
using Spotify.Services;
using Spotify.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlaybackControlPage : Page
{ 
    public PlaybackControlViewModel ViewModel { get; }

    public PlaybackControlPage()
    {
        this.InitializeComponent();
        // Resolve the SongService from the service provider (DI container)
        
        var playbackControlService = (App.Current as App).Services.GetRequiredService<PlaybackControlService>();

        // Pass it to the ViewModel
        ViewModel = new PlaybackControlViewModel(playbackControlService);
        DataContext = ViewModel;
    }

    private void Slider_DragStarted(object sender, DragStartedEventArgs e)
    {
        var vm = (PlaybackControlViewModel)DataContext;
        vm.BeginSeeking();
    }

    private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        var vm = (PlaybackControlViewModel)DataContext;
        vm.EndSeeking();
    }

}
