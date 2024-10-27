using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

 

    //public static readonly DependencyProperty ViewModelProperty =
    //    DependencyProperty.Register(
    //        nameof(ViewModel),
    //        typeof(PlaybackControlViewModel),
    //        typeof(PlaybackControlPage),
    //        new PropertyMetadata(null)
    //    );

    //public PlaybackControlViewModel ViewModel
    //{
    //    get => (PlaybackControlViewModel)GetValue(ViewModelProperty);
    //    set => SetValue(ViewModelProperty, value);
    //}

    //public PlaybackControlPage()
    //{
    //    try
    //    {
    //        this.InitializeComponent();

    //        // Safer type checking and casting
    //        if (App.Current is not App currentApp)
    //        {
    //            throw new InvalidOperationException("Application instance is not of the expected type 'App'");
    //        }

    //        var services = currentApp.Services ??
    //            throw new InvalidOperationException("Service provider is not initialized");

    //        // Get the ViewModel from DI
    //       ViewModel = services.GetRequiredService<PlaybackControlViewModel>() ?? throw new InvalidOperationException("Failed to resolve PlaybackControlViewModel");

    //        DataContext = ViewModel;

    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the error appropriately
    //        System.Diagnostics.Debug.WriteLine($"Error initializing PlaybackControlPage: {ex}");
    //        throw; // Rethrow to maintain the error state
    //    }
    //}
}
