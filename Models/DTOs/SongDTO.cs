//SongDTO.cs
using System;

namespace Spotify.Models.DTOs;
public class SongDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Audio_url { get; set; }
    public string CoverArt_url { get; set; } = "ms-appx:///Assets/OMG_NewJeans.jpg";
    public string Artist { get; set; }
    public string Album { get; set; }
    public TimeSpan Duration { get; set; }
    public string Genre { get; set; }

    // Optional fields depending on what your app needs:
    public DateTime ReleaseDate { get; set; }
    public string FilePath { get; set; } // If the song is stored locally
}
