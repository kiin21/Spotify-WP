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
    /// <summary>
    /// The media player instance used for playback.
    /// </summary>
    private readonly MediaPlayer _mediaPlayer;

    /// <summary>
    /// Indicates whether the object has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Occurs when the media playback ends.
    /// </summary>
    public event EventHandler MediaEnded;

    /// <summary>
    /// Occurs when the playback position changes.
    /// </summary>
    public event EventHandler<TimeSpan> PositionChanged;

    /// <summary>
    /// Occurs when the volume changes.
    /// </summary>
    public event EventHandler<double> VolumeChanged;

    /// <summary>
    /// Occurs when the playback rate changes.
    /// </summary>
    public event EventHandler<double> PlaybackRateChanged;

    /// <summary>
    /// Occurs when the playback state changes.
    /// </summary>
    public event EventHandler<bool> PlaybackStateChanged;

    /// <summary>
    /// Occurs when there is a media playback failure.
    /// </summary>
    public event EventHandler<Exception> MediaFailed;

    /// <summary>
    /// The total listening time.
    /// </summary>
    private TimeSpan _totalListeningTime = TimeSpan.Zero;

    /// <summary>
    /// The start time of the current playback session.
    /// </summary>
    private DateTime _playbackStartTime;

    /// <summary>
    /// The service for managing play history.
    /// </summary>
    private PlayHistoryService _playHistoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicEngine"/> class.
    /// </summary>
    public MusicEngine()
    {
        _mediaPlayer = new MediaPlayer();
        InitializeMediaPlayer();
    }

    /// <summary>
    /// Gets a value indicating whether the media player is currently playing.
    /// </summary>
    public bool IsPlaying => _mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;

    /// <summary>
    /// Starts or resumes media playback.
    /// </summary>
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

    /// <summary>
    /// Pauses media playback.
    /// </summary>
    public void Pause()
    {
        ThrowIfDisposed();
        _mediaPlayer.Pause();
        _totalListeningTime += DateTime.Now - _playbackStartTime;
        PlaybackStateChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Stops media playback and resets the playback position to the beginning.
    /// </summary>
    public void Stop()
    {
        ThrowIfDisposed();
        _mediaPlayer.Pause();
        SetPosition(TimeSpan.Zero);
        PlaybackStateChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Sets the media source to the specified URI.
    /// </summary>
    /// <param name="uri">The URI of the media source.</param>
    /// <exception cref="ArgumentNullException">Thrown when the URI is null.</exception>
    /// <exception cref="Exception">Thrown when there is an error setting the media source.</exception>
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

    /// <summary>
    /// Sets the volume of the media player.
    /// </summary>
    /// <param name="volume">The volume level to set, between 0 and 1.</param>
    public void SetVolume(double volume)
    {
        ThrowIfDisposed();

        // Clamp volume between 0 and 1
        volume = Math.Clamp(volume, 0, 1);
        _mediaPlayer.Volume = volume;
        VolumeChanged?.Invoke(this, volume);
    }

    /// <summary>
    /// Sets the playback rate of the media player.
    /// </summary>
    /// <param name="rate">The playback rate to set, between 0.5 and 2.0.</param>
    public void SetPlaybackRate(double rate)
    {
        ThrowIfDisposed();

        // Clamp rate between 0.5 and 2.0
        rate = Math.Clamp(rate, 0.5, 2.0);
        _mediaPlayer.PlaybackSession.PlaybackRate = rate;
        PlaybackRateChanged?.Invoke(this, rate);
    }

    /// <summary>
    /// Sets the playback position of the media player.
    /// </summary>
    /// <param name="position">The position to set.</param>
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

    /// <summary>
    /// Gets the current playback position of the media player.
    /// </summary>
    /// <returns>The current playback position.</returns>
    public TimeSpan GetPosition()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.Position;
    }

    /// <summary>
    /// Gets the duration of the media.
    /// </summary>
    /// <returns>The duration of the media.</returns>
    public TimeSpan GetDuration()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.NaturalDuration;
    }

    /// <summary>
    /// Gets the volume of the media player.
    /// </summary>
    /// <returns>The volume level.</returns>
    public double GetVolume()
    {
        ThrowIfDisposed();
        return _mediaPlayer.Volume;
    }

    /// <summary>
    /// Gets the playback rate of the media player.
    /// </summary>
    /// <returns>The playback rate.</returns>
    public double GetPlaybackRate()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.PlaybackRate;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="MusicEngine"/> class.
    /// </summary>
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

    /// <summary>
    /// Initializes the media player and sets up event handlers.
    /// </summary>
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

    /// <summary>
    /// Handles the MediaEnded event of the media player.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The <see cref="object"/> instance containing the event data.</param>
    private void OnMediaEnded(MediaPlayer sender, object args)
    {
        _totalListeningTime = DateTime.Now - _playbackStartTime;
        MediaEnded?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the MediaFailed event of the media player.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The <see cref="MediaPlayerFailedEventArgs"/> instance containing the event data.</param>
    private void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        MediaFailed?.Invoke(this, new Exception(args.ErrorMessage));
    }

    /// <summary>
    /// Handles the PlaybackStateChanged event of the media playback session.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The <see cref="object"/> instance containing the event data.</param>
    private void OnPlaybackStateChanged(MediaPlaybackSession sender, object args)
    {
        PlaybackStateChanged?.Invoke(this, IsPlaying);
    }

    /// <summary>
    /// Handles the VolumeChanged event of the media player.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The <see cref="object"/> instance containing the event data.</param>
    private void OnVolumeChanged(MediaPlayer sender, object args)
    {
        VolumeChanged?.Invoke(this, sender.Volume);
    }

    /// <summary>
    /// Throws an exception if the object has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(MusicEngine));
        }
    }

    /// <summary>
    /// Gets the total listening time.
    /// </summary>
    /// <returns>The total listening time.</returns>
    public TimeSpan GetTotalListeningTime()
    {
        if (IsPlaying)
        {
            _totalListeningTime += DateTime.Now - _playbackStartTime;
            return _totalListeningTime;
        }
        return _totalListeningTime;
    }
}
