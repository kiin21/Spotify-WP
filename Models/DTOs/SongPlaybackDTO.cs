// SongPlaybackDTO.cs
using System;

namespace Spotify.Models.DTOs
{
    public class SongPlaybackDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "ms-appx:///Assets/Error.svg";
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string AudioUrl { get; set; } = string.Empty;

    }
}