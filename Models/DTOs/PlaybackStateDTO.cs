using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Playback State.
/// </summary>
public class PlaybackStateDTO
{
    /// <summary>
    /// Gets or sets the ID of the current song.
    /// </summary>
    public string CurrentSongId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the playback is currently playing.
    /// </summary>
    public bool IsPlaying { get; set; }

    /// <summary>
    /// Gets or sets the volume level.
    /// </summary>
    public double Volume { get; set; }

    /// <summary>
    /// Gets or sets the playback speed.
    /// </summary>
    public string PlaybackSpeed { get; set; }

    /// <summary>
    /// Gets or sets the current playback position.
    /// </summary>
    public TimeSpan CurrentPosition { get; set; }

    /// <summary>
    /// Gets or sets the duration of the current song.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether shuffle is enabled.
    /// </summary>
    public bool IsShuffleEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether repeat is enabled.
    /// </summary>
    public bool IsRepeatEnabled { get; set; }
}


