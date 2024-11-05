using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Spotify.Models.DTOs;
using Spotify.Contracts.Services;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace Spotify.ViewModels;

public partial class QueueViewModel : ObservableObject
{
    private readonly IPlaybackControlService _playbackControlService;

    [ObservableProperty]
    public ObservableCollection<SongPlaybackDTO> _queueSongs;

    [ObservableProperty]
    public SongPlaybackDTO _currentSong;

    [ObservableProperty]
    public bool _isQueueVisible;

    [ObservableProperty]
    public string _title;

    [ObservableProperty]
    public string _artist;

    [ObservableProperty]
    public string _imageSource;

    public QueueViewModel(
        ObservableCollection<SongPlaybackDTO> queueSongs,
        bool isQueueVisible,
        SongPlaybackDTO currentSong,
        string title,
        string artist,
        string imageSource,
        IPlaybackControlService playbackControlService
        )
    {
        _playbackControlService = playbackControlService;

        var currentSongIndex = queueSongs.IndexOf(currentSong);

        if (currentSongIndex >= 0)
        {
            QueueSongs = new ObservableCollection<SongPlaybackDTO>(queueSongs);
        }
        else
        {
            QueueSongs = queueSongs;
        }

        IsQueueVisible = isQueueVisible;
        CurrentSong = currentSong;
        Title = title;
        Artist = artist;
        ImageSource = imageSource;
    }

    [RelayCommand]
    public async Task PlaySelectedSongAsync(SongPlaybackDTO selectedSong)
    {
        if (selectedSong == null) return;

        try
        {
            await _playbackControlService.SetPlayPauseAsync(false);

            var originalIndex = (await _playbackControlService.GetQueueAsync()).ToList().FindIndex(s => s.Id == selectedSong.Id);

            if (originalIndex >= 0)
            {
                var currentIndex = await _playbackControlService.GetCurrentSongIndex();
                var skipCount = originalIndex - currentIndex;

                if (skipCount > 0)
                {
                    for (int i = 0; i < skipCount; i++)
                    {
                        await _playbackControlService.NextAsync();
                    }
                }
                else
                {
                    for (int i = 0; i < Math.Abs(skipCount); i++)
                    {
                        await _playbackControlService.PreviousAsync();
                    }
                }

                await _playbackControlService.SetPlayPauseAsync(true);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error playing selected song: {ex.Message}");
        }
    }
    partial void OnCurrentSongChanged(SongPlaybackDTO oldValue, SongPlaybackDTO newValue)
    {
        foreach (var song in QueueSongs)
        {
            song.IsCurrentSong = song == newValue;
        }
    }
}