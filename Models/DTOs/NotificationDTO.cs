using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class NotificationDTO
    {
        public string Message { get; set; }
        public string ArtistId { get; set; }
        public string ArtistName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
