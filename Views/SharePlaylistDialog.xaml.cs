using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

/// <summary>
/// A dialog for sharing a playlist with other users.
/// </summary>
public sealed partial class SharePlaylistDialog : ContentDialog
{
    /// <summary>
    /// Gets the view model for sharing the playlist.
    /// </summary>
    public SharePlaylistViewModel SharePlaylistViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SharePlaylistDialog"/> class.
    /// </summary>
    /// <param name="playlist">The playlist to be shared.</param>
    public SharePlaylistDialog(PlaylistDTO playlist)
    {
        this.InitializeComponent();
        SharePlaylistViewModel = new SharePlaylistViewModel(playlist);
        this.DataContext = SharePlaylistViewModel;
    }
}
