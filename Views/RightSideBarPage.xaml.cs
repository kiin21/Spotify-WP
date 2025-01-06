using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

/// <summary>
/// A page that displays the right sidebar, including the playback queue.
/// </summary>
public sealed partial class RightSideBarPage : Page
{
    /// <summary>
    /// Subscribes to queue updates from ShellWindow.
    /// </summary>
    ShellWindow shellWindow = App.Current.Services.GetRequiredService<ShellWindow>();

    /// <summary>
    /// Gets the view model for playback control.
    /// </summary>
    public PlaybackControlViewModel playbackControlViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RightSideBarPage"/> class.
    /// </summary>
    public RightSideBarPage()
    {
        shellWindow.QueueUpdated += OnQueueUpdated;
        PlaybackControlViewModel.Initialize();
        playbackControlViewModel = PlaybackControlViewModel.Instance;
        this.InitializeComponent();
        DataContext = playbackControlViewModel;
    }

    /// <summary>
    /// Handles the queue updated event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnQueueUpdated(object sender, QueueUpdatedEventArgs e)
    {
        ObservableCollection<SongDTO> updatedQueue = e.UpdatedQueue;

        // Add the song from playlist to the PlaybackList
        bool belongToPlaylist = true;
        playbackControlViewModel.AddToPlaybackList(updatedQueue, belongToPlaylist);
    }
}
