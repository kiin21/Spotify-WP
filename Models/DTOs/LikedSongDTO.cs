﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class LikedSongDTO
    {
        public string Title { get; set; }
        public string Album { get; set; }
        public DateTime DateAdded { get; set; }
        public TimeSpan Duration { get; set; }
        public string Image { get; set; } // Thêm thuộc tính Image
    }
}