using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Spotify.Views;

/// <summary>
/// A dialog for adding a new playlist.
/// </summary>
public sealed partial class AddPlaylistDialog : ContentDialog
{
    /// <summary>
    /// Gets the name of the playlist entered by the user.
    /// </summary>
    public string PlaylistName => PlaylistNameTextBox.Text;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddPlaylistDialog"/> class.
    /// </summary>
    public AddPlaylistDialog()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Handles the click event of the save button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        var playlistService = (App.Current as App).Services.GetService<PlaylistService>();

        var newPlaylist = new PlaylistDTO
        {
            Title = PlaylistNameTextBox.Text,
            CreatedAt = DateTime.UtcNow,
            ShareWithUsers = new List<string>(),
        };

        await playlistService.AddPlaylistAsync(newPlaylist);
        Hide(); // Close the dialog after saving successfully
    }
}
