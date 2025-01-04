using System;
using System.Diagnostics;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views.Controls;

/// <summary>
/// A user control that displays the playback queue list.
/// </summary>
public sealed partial class QueueListUserControl : UserControl
{
    /// <summary>
    /// Gets the view model for playback control.
    /// </summary>
    public PlaybackControlViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueListUserControl"/> class.
    /// </summary>
    public QueueListUserControl()
    {
        this.InitializeComponent();
        PlaybackControlViewModel.Initialize();
        ViewModel = PlaybackControlViewModel.Instance;
        DataContext = ViewModel;
    }

    /// <summary>
    /// Handles the click event for a song in the playback list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void PlaybackListSong_OnClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            // Get the clicked song
            if (e.ClickedItem is SongDTO clickedSong)
            {
                Debug.WriteLine($"Song clicked: {clickedSong.title}");

                ViewModel.Play(clickedSong);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in PlaybackListSong_OnClick: {ex}");
        }
    }
}

