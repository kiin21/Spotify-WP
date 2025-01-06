using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Playlist Song Details.
/// </summary>
public class PlaylistSongDetailDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the unique identifier for the playlist song.
    /// </summary>
    public string PlaylistSongId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the song.
    /// </summary>
    public string SongId { get; set; }

    /// <summary>
    /// Gets or sets the title of the song.
    /// </summary>
    public string SongTitle { get; set; }

    /// <summary>
    /// Gets or sets the URL of the song's avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the artist of the song.
    /// </summary>
    public string Artist { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the song was added to the playlist.
    /// </summary>
    public DateTime AddedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who added the song to the playlist.
    /// </summary>
    public string AddedBy { get; set; }

    /// <summary>
    /// Gets or sets the duration of the song in seconds.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Gets or sets the index of the song in the playlist.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the song is in the "Liked Songs" playlist.
    /// </summary>
    public bool IsInLikedPlaylist { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}


