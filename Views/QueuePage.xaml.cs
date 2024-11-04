using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Spotify.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class QueuePage : Page
{

    public QueueViewModel ViewModel { get; private set; }
    public QueuePage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Tuple<ObservableCollection<SongPlaybackDTO>, bool, SongPlaybackDTO, string, string, string, IPlaybackControlService> parameters)
        {
            var queueSongs = parameters.Item1;
            var isQueueVisible = parameters.Item2;
            var currentSong = parameters.Item3;
            var title = parameters.Item4;
            var artist = parameters.Item5;
            var imageSource = parameters.Item6;
            var playbackControlService = parameters.Item7;

            ViewModel = new QueueViewModel(queueSongs, isQueueVisible, currentSong, title, artist, imageSource, playbackControlService);
            this.DataContext = ViewModel;
        }
        base.OnNavigatedTo(e);
    }
    private async void QueueList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongPlaybackDTO selectedSong)
        {
            await ViewModel.PlaySelectedSongAsync(selectedSong);
        }
    }
}
