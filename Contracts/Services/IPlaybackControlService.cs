using System;
using System.Collections.Generic;

namespace Spotify.Contracts.Services;

/// <summary>
/// Interface for controlling music playback, including playlist management and playback settings
/// </summary>
public interface IPlaybackControlService : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether music is currently playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Occurs when the playback state changes.
    /// </summary>
    event EventHandler<bool> PlaybackStateChanged;

    /// <summary>
    /// Occurs when the playback position changes.
    /// </summary>
    event EventHandler<TimeSpan> PositionChanged;

    /// <summary>
    /// Occurs when the media playback ends.
    /// </summary>
    event EventHandler MediaEnded;

    /// <summary>
    /// Plays the specified audio URL.
    /// </summary>
    /// <param name="audioUrl">The URL of the audio to play.</param>
    void Play(Uri audioUrl);

    /// <summary>
    /// Resumes the playback.
    /// </summary>
    void Resume();

    /// <summary>
    /// Pauses the playback.
    /// </summary>
    void Pause();

    /// <summary>
    /// Sets the volume.
    /// </summary>
    /// <param name="volume">The volume level to set.</param>
    void SetVolume(double volume);

    /// <summary>
    /// Sets the playback rate.
    /// </summary>
    /// <param name="rate">The playback rate to set.</param>
    void SetPlaybackRate(double rate);

    /// <summary>
    /// Seeks to the specified position.
    /// </summary>
    /// <param name="position">The position to seek to.</param>
    void Seek(TimeSpan position);

    /// <summary>
    /// Gets the current playback position.
    /// </summary>
    /// <returns>The current playback position.</returns>
    TimeSpan GetCurrentPosition();

    /// <summary>
    /// Gets the duration of the media.
    /// </summary>
    /// <returns>The duration of the media.</returns>
    TimeSpan GetDuration();
}