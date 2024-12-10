using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class RightSideBarPage : Page
{
    // Subscribe to queue updates from ShellWindow
    ShellWindow shellWindow = App.Current.Services.GetRequiredService<ShellWindow>();

    public PlaybackControlViewModel playbackControlViewModel;
    public RightSideBarPage()
    {
        shellWindow.QueueUpdated += OnQueueUpdated;
        PlaybackControlViewModel.Initialize();
        playbackControlViewModel = PlaybackControlViewModel.Instance;
        this.InitializeComponent();
        DataContext = playbackControlViewModel;
    }


    private void OnQueueUpdated(object sender, QueueUpdatedEventArgs e)
    {
        ObservableCollection<SongDTO> updatedQueue = e.UpdatedQueue;

        // Add the song from playlist to the PlaybackList
        bool belongToPlaylist = true;
        playbackControlViewModel.AddToPlaybackList(updatedQueue, belongToPlaylist);

    }

}
