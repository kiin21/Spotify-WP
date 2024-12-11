// PlaybackControlService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Spotify.Engine;
using Spotify;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for controlling music playback.
/// </summary>
public class PlaybackControlService : IDisposable, IPlaybackControlService
{
    private readonly MusicEngine _musicEngine;
    private readonly System.Timers.Timer _positionUpdateTimer;
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
    private double _volume = 50;
    private double _playbackRate = 1.0;
    private bool _isPlaying;

    /// <summary>
    /// Gets the singleton instance of the <see cref="PlaybackControlService"/> class.
    /// </summary>
    public static PlaybackControlService Instance { get; private set; }

    /// <summary>
    /// Initializes the singleton instance of the <see cref="PlaybackControlService"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher queue for UI thread operations.</param>
    public static void Initialize(Microsoft.UI.Dispatching.DispatcherQueue dispatcher)
    {
        if (Instance == null)
        {
            Instance = new PlaybackControlService(dispatcher);
        }
    }

    private PlaybackControlService(Microsoft.UI.Dispatching.DispatcherQueue dispatcher)
    {
        _dispatcherQueue = dispatcher;
        _musicEngine = new MusicEngine();
        _musicEngine.MediaEnded += OnMediaEnded;
        _positionUpdateTimer = new System.Timers.Timer(500);
        _positionUpdateTimer.Elapsed += (s, e) =>
        {
            if (_isPlaying)
            {
                _dispatcherQueue.TryEnqueue(
                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                    () =>
                    {
                        PositionChanged?.Invoke(this, GetCurrentPosition());
                    }
                );
            }
        };
        _positionUpdateTimer.Start();
    }

    /// <summary>
    /// Gets a value indicating whether the media is currently playing.
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// Occurs when the playback state changes.
    /// </summary>
    public event EventHandler<bool> PlaybackStateChanged;

    /// <summary>
    /// Occurs when the playback position changes.
    /// </summary>
    public event EventHandler<TimeSpan> PositionChanged;

    /// <summary>
    /// Occurs when media playback ends.
    /// </summary>
    public event EventHandler MediaEnded;

    /// <summary>
    /// Adds the current song to the playback queue.
    /// </summary>
    /// <param name="song">The song to add.</param>
    public void AddCurrentSong(SongDTO song)
    {
        _musicEngine.SetSource(new Uri(song.audio_url));
        _musicEngine.SetVolume(_volume / 100.0);
        _musicEngine.SetPlaybackRate(_playbackRate);
    }

    /// <summary>
    /// Plays the media from the specified URL.
    /// </summary>
    /// <param name="audioUrl">The URL of the audio to play.</param>
    public void Play(Uri audioUrl)
    {
        _musicEngine.SetSource(audioUrl);
        _musicEngine.SetVolume(_volume / 100.0);
        _musicEngine.SetPlaybackRate(_playbackRate);
        Resume();
    }

    /// <summary>
    /// Resumes media playback.
    /// </summary>
    public void Resume()
    {
        _musicEngine.Play();
        _isPlaying = true;
        PlaybackStateChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Pauses media playback.
    /// </summary>
    public void Pause()
    {
        _musicEngine.Pause();
        _isPlaying = false;
        PlaybackStateChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Sets the volume to the specified level.
    /// </summary>
    /// <param name="volume">The volume level (0 to 100).</param>
    public void SetVolume(double volume)
    {
        _volume = Math.Clamp(volume, 0, 100);
        _musicEngine.SetVolume(_volume / 100.0);
    }

    /// <summary>
    /// Sets the playback rate to the specified value.
    /// </summary>
    /// <param name="rate">The playback rate (0.5 to 2.0).</param>
    public void SetPlaybackRate(double rate)
    {
        _playbackRate = Math.Clamp(rate, 0.5, 2.0);
        _musicEngine.SetPlaybackRate(_playbackRate);
    }

    /// <summary>
    /// Seeks to the specified position in the media.
    /// </summary>
    /// <param name="position">The position to seek to.</param>
    public void Seek(TimeSpan position)
    {
        _musicEngine.SetPosition(position);
        PositionChanged?.Invoke(this, position);
    }

    /// <summary>
    /// Gets the current playback position.
    /// </summary>
    /// <returns>The current playback position.</returns>
    public TimeSpan GetCurrentPosition() => _musicEngine.GetPosition();

    /// <summary>
    /// Gets the duration of the media.
    /// </summary>
    /// <returns>The duration of the media.</returns>
    public TimeSpan GetDuration() => _musicEngine.GetDuration();

    private void OnMediaEnded(object sender, EventArgs e)
    {
        _dispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            () =>
            {
                MediaEnded?.Invoke(this, EventArgs.Empty);
            }
        );
    }

    /// <summary>
    /// Releases all resources used by the <see cref="PlaybackControlService"/>.
    /// </summary>
    public void Dispose()
    {
        _positionUpdateTimer.Stop();
        _positionUpdateTimer.Dispose();
        _musicEngine.MediaEnded -= OnMediaEnded;
        _musicEngine.Stop();
    }
}
