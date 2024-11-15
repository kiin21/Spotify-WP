// LyricPage.xaml.cs
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Spotify.Views;

public sealed partial class LyricPage : Page
{
    public LyricViewModel ViewModel { get; private set; }
    private double _playbackRate = 2.0;
    public LyricPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is SongDTO song)
        {
            ViewModel = new LyricViewModel(song);
            ViewModel.LoadLyrics();
            LoadMediaPlayer(song);
        }
    }

    private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private void LoadMediaPlayer(SongDTO song)
    {
        // Create a Uri from the URL string
        var mediaUri = new Uri("https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FShape%20of%20You.mp3?alt=media&token=ea5b128a-eef8-4f65-8003-73c16b991fd9");

        // Set the Source property of the mediaPlayerElement
        mediaPlayerElement.Source = MediaSource.CreateFromUri(mediaUri);

        // Set the playback rate
        mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackRate = _playbackRate;

        // Optionally, start playing the media
        mediaPlayerElement.MediaPlayer.Play();
    }

    private void MediaPlayerElement_Loaded(object sender, RoutedEventArgs e)
    {
        mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackRateChanged += PlaybackSession_PlaybackRateChanged;
    }

    private void PlaybackSession_PlaybackRateChanged(MediaPlaybackSession sender, object args)
    {
        _playbackRate = sender.PlaybackRate;
        // You can update UI elements here to reflect the current playback rate
    }
}