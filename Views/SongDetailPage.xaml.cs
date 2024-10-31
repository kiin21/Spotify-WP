// SongDetailPage.xaml.cs
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;
using Spotify.ViewModels;

namespace Spotify.Views;

public sealed partial class SongDetailPage : Page
{
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
        var playbackService = (App.Current as App).Services.GetRequiredService<IPlaybackControlService>();

        // Convert SongDTO to SongPlaybackDTO
        var songPlayback = new SongPlaybackDTO
        {
            Id = "NULL",
            Title = ViewModel.Title,
            Artist = ViewModel.ArtistInfo,
            AudioUrl = ViewModel.AudioUrl,
            ImageUrl = ViewModel.ImageUrl,
            Duration = TimeSpan.FromSeconds(ViewModel.Duration), 
        };

        await playbackService.AddToQueueAsync(songPlayback);
    }
}