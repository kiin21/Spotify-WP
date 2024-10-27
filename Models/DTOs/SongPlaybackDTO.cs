using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class SongPlaybackDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public string AudioUrl { get; set; }
    }
}
