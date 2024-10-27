using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO
{
    public interface IPlaybackControlDAO
    {
        Task<PlaybackStateDTO> GetPlaybackStateAsync();
        Task UpdatePlaybackStateAsync(PlaybackStateDTO state);
        Task<SongPlaybackDTO> GetCurrentSongAsync();
        Task<SongPlaybackDTO> GetNextSongAsync();
        Task<SongPlaybackDTO> GetPreviousSongAsync();
        Task UpdateCurrentPositionAsync(TimeSpan position);
        Task<List<SongPlaybackDTO>> GetQueueAsync();
        Task ShuffleQueueAsync();
        Task SetRepeatStateAsync(bool isRepeatEnabled);
    }
}
