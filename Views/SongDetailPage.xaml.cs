// SongDetailPage.xaml.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

    public PlaybackControlService playbackService = PlaybackControlService.Instance;
    public SongDetailPage()
    {
        ViewModel = new SongDetailViewModel();
        this.InitializeComponent();


        // Đăng ký lắng nghe sự kiện CurrentSongChanged
        //playbackService.CurrentSongChanged += PlaybackService_CurrentSongChanged;
    }
    //private void PlaybackService_CurrentSongChanged(object sender, SongPlaybackDTO e)
    //{
    //    if (e.Id == ViewModel.Id)
    //    {
    //        isPlaying = true;
    //        PlayButtonIcon.Symbol = isPlaying ? Symbol.Pause : Symbol.Play;
    //    }
    //    else
    //    {
    //        isPlaying = false;
    //        PlayButtonIcon.Symbol = Symbol.Play;
    //    }
    //}

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
        //var playbackService = (App.Current as App).Services.GetRequiredService<PlaybackControlService>();
        //isPlaying = !isPlaying; // Toggle the playing state
        //// Update the button icon based on state
        //PlayButtonIcon.Symbol = isPlaying ? Symbol.Pause : Symbol.Play;

        //// If the current song is the same as the song being played, resume playing
        //SongPlaybackDTO currentSong = playbackService.GetCurrentSong();
        //if (currentSong.Id == ViewModel.Id)
        //{
        //    await playbackService.SetPlayPauseAsync(isPlaying);
        //}
        //else
        //{
        //    var newSong = new SongPlaybackDTO
        //    {
        //        Id = ViewModel.Id,
        //        Title = ViewModel.Title,
        //        Artist = ViewModel.ArtistInfo,
        //        AudioUrl = ViewModel.AudioUrl,
        //        ImageUrl = ViewModel.ImageUrl,
        //        Duration = TimeSpan.FromSeconds(ViewModel.Duration),
        //    };
        //    var queueSongs = await playbackService.GetQueueAsync();

        //    // Kiểm tra xem bài hát đã có trong queue chưa
        //    if (queueSongs.Any(song => song.Id == ViewModel.Id))
        //    {
        //        await playbackService.PlaySongById(ViewModel.Id);
        //    }
        //    else
        //    {
        //        // Nếu chưa có, thêm vào queue


        //        try
        //        {
        //            // Thêm bài hát vào queue và chuyển đến nó
        //            await playbackService.AddToNextInQueueAsync(newSong);
        //            await playbackService.SetPlayPauseAsync(true);
        //            await playbackService.NextAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Error adding song to queue: {ex.Message}");
        //        }
        //    }
        //}
    }

    private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Frame.GoBack();
    }
}