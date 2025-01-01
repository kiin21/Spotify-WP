using System;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Contracts.DAO;
using Spotify.Services;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Spotify.Engine;

/// <summary>
/// Provides functionality to manage music playback.
/// </summary>
public class MusicEngine : IDisposable
{
    private readonly MediaPlayer _mediaPlayer;
    private bool _isDisposed;

    public event EventHandler MediaEnded;

    public event EventHandler<TimeSpan> PositionChanged;

    public event EventHandler<double> VolumeChanged;

    public event EventHandler<double> PlaybackRateChanged;

    public event EventHandler<bool> PlaybackStateChanged;

    public event EventHandler<Exception> MediaFailed;

    private TimeSpan _totalListeningTime = TimeSpan.Zero;

    private DateTime _playbackStartTime;

    private PlayHistoryService _playHistoryService;
    public MusicEngine()
    {
        _mediaPlayer = new MediaPlayer();
        InitializeMediaPlayer();
    }

    public bool IsPlaying => _mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;

    public void Play()
    {
        ThrowIfDisposed();
        if (_mediaPlayer.Source != null)
        {
            _mediaPlayer.Play();
            _playbackStartTime = DateTime.Now;
            PlaybackStateChanged?.Invoke(this, true);
        }
    }

    public void Pause()
    {
        ThrowIfDisposed();
        _mediaPlayer.Pause();
        _totalListeningTime += DateTime.Now - _playbackStartTime;
        PlaybackStateChanged?.Invoke(this, false);
    }

    public void Stop()
    {
        ThrowIfDisposed();
        _mediaPlayer.Pause();
        SetPosition(TimeSpan.Zero);
        PlaybackStateChanged?.Invoke(this, false);
    }

    public void SetSource(Uri uri)
    {
        ThrowIfDisposed();

        try
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            // Stop current playback
            Stop();

            // Create and set new media source
            var mediaSource = MediaSource.CreateFromUri(uri);
            _mediaPlayer.Source = mediaSource;
        }
        catch (Exception ex)
        {
            MediaFailed?.Invoke(this, ex);
            throw;
        }
    }

    public void SetVolume(double volume)
    {
        ThrowIfDisposed();

        // Clamp volume between 0 and 1
        volume = Math.Clamp(volume, 0, 1);
        _mediaPlayer.Volume = volume;
        VolumeChanged?.Invoke(this, volume);
    }

    public void SetPlaybackRate(double rate)
    {
        ThrowIfDisposed();

        // Clamp rate between 0.5 and 2.0
        rate = Math.Clamp(rate, 0.5, 2.0);
        _mediaPlayer.PlaybackSession.PlaybackRate = rate;
        PlaybackRateChanged?.Invoke(this, rate);
    }

    public void SetPosition(TimeSpan position)
    {
        ThrowIfDisposed();

        var duration = GetDuration();
        if (duration != TimeSpan.Zero)
        {
            // Ensure position is within valid range
            position = TimeSpan.FromMilliseconds(
                Math.Clamp(position.TotalMilliseconds, 0, duration.TotalMilliseconds)
            );
            _mediaPlayer.PlaybackSession.Position = position;
            PositionChanged?.Invoke(this, position);
        }
    }

    public TimeSpan GetPosition()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.Position;
    }

    public TimeSpan GetDuration()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.NaturalDuration;
    }

    public double GetVolume()
    {
        ThrowIfDisposed();
        return _mediaPlayer.Volume;
    }

    public double GetPlaybackRate()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.PlaybackRate;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            // Unsubscribe from events
            _mediaPlayer.MediaEnded -= OnMediaEnded;
            _mediaPlayer.MediaFailed -= OnMediaFailed;
            _mediaPlayer.PlaybackSession.PlaybackStateChanged -= OnPlaybackStateChanged;
            _mediaPlayer.VolumeChanged -= OnVolumeChanged;

            // Stop playback and dispose
            Stop();
            _mediaPlayer.Dispose();

            _isDisposed = true;
        }
    }

    private void InitializeMediaPlayer()
    {
        // Set up event handlers
        _mediaPlayer.MediaEnded += OnMediaEnded;
        _mediaPlayer.MediaFailed += OnMediaFailed;
        _mediaPlayer.PlaybackSession.PlaybackStateChanged += OnPlaybackStateChanged;
        _mediaPlayer.VolumeChanged += OnVolumeChanged;

        // Set default properties
        _mediaPlayer.AutoPlay = false;
        _mediaPlayer.Volume = 0.5; // 50% default volume
    }

    private void OnMediaEnded(MediaPlayer sender, object args)
    {
        _totalListeningTime = DateTime.Now - _playbackStartTime;
        MediaEnded?.Invoke(this, EventArgs.Empty);
    }


    private void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        MediaFailed?.Invoke(this, new Exception(args.ErrorMessage));
    }

    private void OnPlaybackStateChanged(MediaPlaybackSession sender, object args)
    {
        PlaybackStateChanged?.Invoke(this, IsPlaying);
    }

    private void OnVolumeChanged(MediaPlayer sender, object args)
    {
        VolumeChanged?.Invoke(this, sender.Volume);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(MusicEngine));
        }
    }

    public TimeSpan GetTotalListeningTime()
    {
        if(IsPlaying)
        {
            _totalListeningTime += DateTime.Now - _playbackStartTime;
            return _totalListeningTime;
        }
        return _totalListeningTime;
    }
}
