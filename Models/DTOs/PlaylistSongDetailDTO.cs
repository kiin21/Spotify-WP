using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class PlaylistSongDetailDTO
    {
        public int Position { get; set; }
        public string PlaylistSongId { get; set; }
        public string SongId { get; set; }
        public string SongTitle { get; set; }
        public string Avatar { get; set; }
        public string Artist { get; set; }
        public DateTime AddedAt { get; set; }
        public string AddedBy { get; set; }
        public string Duration { get; set; }
    }
}
