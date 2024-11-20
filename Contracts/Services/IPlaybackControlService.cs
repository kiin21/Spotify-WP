using System;
using System.Collections.Generic;

namespace Spotify.Contracts.Services;

/// <summary>
/// Interface for controlling music playback, including playlist management and playback settings
/// </summary>
public interface IPlaybackControlService : IDisposable
{
    // Properties
    bool IsPlaying { get; }

    // Events
    event EventHandler<bool> PlaybackStateChanged;
    event EventHandler<TimeSpan> PositionChanged;
    event EventHandler MediaEnded;

    // Methods
    void Play(Uri audioUrl);
    void Resume();
    void Pause();
    void SetVolume(double volume);
    void SetPlaybackRate(double rate);
    void Seek(TimeSpan position);
    TimeSpan GetCurrentPosition();
    TimeSpan GetDuration();
}
