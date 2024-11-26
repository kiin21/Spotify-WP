// PlaybackControlService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Spotify.Engine;
using Spotify;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;

namespace Spotify.Services;

public class PlaybackControlService : IDisposable, IPlaybackControlService
{
    private readonly MusicEngine _musicEngine;
    private readonly System.Timers.Timer _positionUpdateTimer;
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
    private double _volume = 50;
    private double _playbackRate = 1.0;
    private bool _isPlaying;

    public static PlaybackControlService Instance { get; private set; }

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

    public bool IsPlaying => _isPlaying;

    public event EventHandler<bool> PlaybackStateChanged;
    public event EventHandler<TimeSpan> PositionChanged;
    public event EventHandler MediaEnded;

    public void AddCurrentSong(SongDTO song)
    {
        _musicEngine.SetSource(new Uri(song.audio_url));
        _musicEngine.SetVolume(_volume / 100.0);
        _musicEngine.SetPlaybackRate(_playbackRate);
    }
    public void Play(Uri audioUrl)
    {
        _musicEngine.SetSource(audioUrl);
        _musicEngine.SetVolume(_volume / 100.0);  
        _musicEngine.SetPlaybackRate(_playbackRate);
        Resume();
    }

    public void Resume()
    {
        _musicEngine.Play();
        _isPlaying = true;
        PlaybackStateChanged?.Invoke(this, true);
    }

    public void Pause()
    {
        _musicEngine.Pause();
        _isPlaying = false;
        PlaybackStateChanged?.Invoke(this, false);
    }

    public void SetVolume(double volume)
    {
        _volume = Math.Clamp(volume, 0, 100);
        _musicEngine.SetVolume(_volume / 100.0);
    }

    public void SetPlaybackRate(double rate)
    {
        _playbackRate = Math.Clamp(rate, 0.5, 2.0);
        _musicEngine.SetPlaybackRate(_playbackRate);
    }


    public void Seek(TimeSpan position)
    {
        _musicEngine.SetPosition(position);
        PositionChanged?.Invoke(this, position);
    }

    public TimeSpan GetCurrentPosition() => _musicEngine.GetPosition();

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

    public void Dispose()
    {
        _positionUpdateTimer.Stop();
        _positionUpdateTimer.Dispose();
        _musicEngine.MediaEnded -= OnMediaEnded;
        _musicEngine.Stop();
    }
}