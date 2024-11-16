using System;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Spotify.Engine;

public class MusicEngine : IDisposable
{
    private readonly MediaPlayer _mediaPlayer;

    public event EventHandler MediaEnded;

    public MusicEngine()
    {
        _mediaPlayer = new MediaPlayer();
        _mediaPlayer.MediaEnded += OnMediaEnded;
    }

    public MediaPlayer MediaPlayer => _mediaPlayer;

    public void Play()
    {
        _mediaPlayer.Play();
    }

    public void Pause()
    {
        _mediaPlayer.Pause();
    }

    public void SetSource(Uri uri)
    {
        _mediaPlayer.Source = MediaSource.CreateFromUri(uri);
    }

    public void SetVolume(double volume)
    {
        _mediaPlayer.Volume = volume / 100;
    }

    public void SetPlaybackRate(double rate)
    {
        _mediaPlayer.PlaybackSession.PlaybackRate = rate;
    }

    public void SetPosition(TimeSpan position)
    {
        _mediaPlayer.PlaybackSession.Position = position;
    }

    public TimeSpan GetPosition()
    {
        return _mediaPlayer.PlaybackSession.Position;
    }

    public TimeSpan GetDuration()
    {
        return _mediaPlayer.PlaybackSession.NaturalDuration;
    }

    private void OnMediaEnded(MediaPlayer sender, object args)
    {
        MediaEnded?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _mediaPlayer.MediaEnded -= OnMediaEnded;
        _mediaPlayer.Dispose();
    }
}