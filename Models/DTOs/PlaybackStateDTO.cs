using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class PlaybackStateDTO
    {
        public string CurrentSongId { get; set; }
        public bool IsPlaying { get; set; }
        public double Volume { get; set; }
        public string PlaybackSpeed { get; set; }
        public TimeSpan CurrentPosition { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsShuffleEnabled { get; set; }
        public bool IsRepeatEnabled { get; set; }
    }
}
