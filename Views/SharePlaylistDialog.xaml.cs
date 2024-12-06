using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views
{
    public sealed partial class SharePlaylistDialog : ContentDialog
    {
        public SharePlaylistViewModel SharePlaylistViewModel { get; }

        public SharePlaylistDialog(PlaylistDTO playlist)
        {
            this.InitializeComponent();
            SharePlaylistViewModel = new SharePlaylistViewModel(playlist);
            this.DataContext = SharePlaylistViewModel;
        }
    }
}
