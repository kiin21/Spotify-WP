using System;
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

    /// <summary>
    /// Occurs when media playback ends.
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
    /// Occurs when media playback fails.
    /// </summary>
    public event EventHandler<Exception> MediaFailed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicEngine"/> class.
    /// </summary>
    public MusicEngine()
    {
        _mediaPlayer = new MediaPlayer();
        InitializeMediaPlayer();
    }

    /// <summary>
    /// Gets a value indicating whether the media is currently playing.
    /// </summary>
    public bool IsPlaying => _mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;

    /// <summary>
    /// Starts media playback.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public void Play()
    {
        ThrowIfDisposed();
        if (_mediaPlayer.Source != null)
        {
            _mediaPlayer.Play();
            PlaybackStateChanged?.Invoke(this, true);
        }
    }

    /// <summary>
    /// Pauses media playback.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public void Pause()
    {
        ThrowIfDisposed();
        _mediaPlayer.Pause();
        PlaybackStateChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Stops media playback and resets the position to the beginning.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
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
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the URI is null.</exception>
    /// <exception cref="Exception">Thrown when setting the media source fails.</exception>
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
    /// Sets the volume to the specified level.
    /// </summary>
    /// <param name="volume">The volume level (0.0 to 1.0).</param>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public void SetVolume(double volume)
    {
        ThrowIfDisposed();

        // Clamp volume between 0 and 1
        volume = Math.Clamp(volume, 0, 1);
        _mediaPlayer.Volume = volume;
        VolumeChanged?.Invoke(this, volume);
    }

    /// <summary>
    /// Sets the playback rate to the specified value.
    /// </summary>
    /// <param name="rate">The playback rate (0.5 to 2.0).</param>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public void SetPlaybackRate(double rate)
    {
        ThrowIfDisposed();

        // Clamp rate between 0.5 and 2.0
        rate = Math.Clamp(rate, 0.5, 2.0);
        _mediaPlayer.PlaybackSession.PlaybackRate = rate;
        PlaybackRateChanged?.Invoke(this, rate);
    }

    /// <summary>
    /// Sets the playback position to the specified time.
    /// </summary>
    /// <param name="position">The playback position.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
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
    /// Gets the current playback position.
    /// </summary>
    /// <returns>The current playback position.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public TimeSpan GetPosition()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.Position;
    }

    /// <summary>
    /// Gets the duration of the media.
    /// </summary>
    /// <returns>The duration of the media.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public TimeSpan GetDuration()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.NaturalDuration;
    }

    /// <summary>
    /// Gets the current volume level.
    /// </summary>
    /// <returns>The current volume level.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public double GetVolume()
    {
        ThrowIfDisposed();
        return _mediaPlayer.Volume;
    }

    /// <summary>
    /// Gets the current playback rate.
    /// </summary>
    /// <returns>The current playback rate.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the object is disposed.</exception>
    public double GetPlaybackRate()
    {
        ThrowIfDisposed();
        return _mediaPlayer.PlaybackSession.PlaybackRate;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="MusicEngine"/>.
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
}
