// SongDetailPage.xaml.cs
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class SongDetailPage : Page
{
    private bool isPlaying = false;
    public SongDetailViewModel ViewModel { get; }

    public SongDetailPage()
    {
        ViewModel = new SongDetailViewModel();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is SongDTO song)
        {
            ViewModel.Initialize(song);
        }
    }

    private async void PlayButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var playbackService = (App.Current as App).Services.GetRequiredService<PlaybackControlService>();
        isPlaying = !isPlaying; // Toggle the playing state
        // Update the button icon based on state
        PlayButtonIcon.Symbol = isPlaying ? Symbol.Pause : Symbol.Play;

        // If the current song is the same as the song being played, resume playing
        SongPlaybackDTO currentSong = playbackService.GetCurrentSong();
        if (currentSong.Id == ViewModel.Id)
        {
            await playbackService.SetPlayPauseAsync(isPlaying);
        }
        // Otherwise, load and play the new song
        else
        {
            var newSong = new SongPlaybackDTO
            {
                Id = ViewModel.Id,
                Title = ViewModel.Title,
                Artist = ViewModel.ArtistInfo,
                AudioUrl = ViewModel.AudioUrl,
                ImageUrl = ViewModel.ImageUrl,
                Duration = TimeSpan.FromSeconds(ViewModel.Duration),
            };
            await playbackService.AddToHeadOfQueueAsync(newSong);
            await playbackService.SetPlayPauseAsync(isPlaying);
        }
    }

    private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Frame.GoBack();
    }
}