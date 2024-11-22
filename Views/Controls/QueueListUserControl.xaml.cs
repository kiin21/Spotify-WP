using System;
using System.Diagnostics;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views.Controls;

public sealed partial class QueueListUserControl : UserControl
{
    public PlaybackControlViewModel ViewModel { get; }
    public QueueListUserControl()
    {
        this.InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<PlaybackControlViewModel>();
        DataContext = ViewModel;
    }

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
