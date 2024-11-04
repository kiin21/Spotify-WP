using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.Services;

public interface IPlaybackControlService
{

    event EventHandler<PlaybackStateDTO> PlaybackStateChanged;
    event EventHandler<SongPlaybackDTO> CurrentSongChanged;

    Task<int> GetCurrentSongIndex();
    PlaybackStateDTO GetCurrentState();
    SongPlaybackDTO GetCurrentSong();
    Task<List<SongPlaybackDTO>> GetQueueAsync();
    Task PlayAsync();
    Task PauseAsync();
    Task SetPlayPauseAsync(bool isPlaying);
    Task NextAsync();
    Task PreviousAsync();
    Task ShuffleAsync();
    Task SetRepeatAsync(bool isRepeatEnabled);
    Task SetVolumeAsync(double volume);
    Task SetPlaybackSpeedAsync(string speed);
    Task SeekToPositionAsync(TimeSpan position);

}
