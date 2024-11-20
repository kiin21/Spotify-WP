using System;
using System.Collections.Generic;

namespace Spotify.Contracts.Services;

/// <summary>
/// Interface for controlling music playback, including playlist management and playback settings
/// </summary>
public interface IPlaybackControlService : IDisposable
{
    #region Properties

    /// <summary>
    /// Gets the URI of the currently playing song
    /// </summary>
    Uri CurrentSongUri { get; }

    /// <summary>
    /// Gets whether media is currently playing
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Gets whether shuffle mode is enabled
    /// </summary>
    bool IsShuffleEnabled { get; }

    /// <summary>
    /// Gets the current repeat mode
    /// </summary>
    RepeatMode RepeatMode { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the current song changes
    /// </summary>
    event EventHandler CurrentSongChanged;

    /// <summary>
    /// Occurs when the playback state (playing/paused) changes
    /// </summary>
    event EventHandler<bool> PlaybackStateChanged;

    /// <summary>
    /// Occurs when the playback position changes
    /// </summary>
    event EventHandler<TimeSpan> PositionChanged;

    #endregion

    #region Playlist Management

    /// <summary>
    /// Sets the current playlist
    /// </summary>
    /// <param name="songs">Collection of song URIs to set as the playlist</param>
    void SetPlaylist(IEnumerable<Uri> songs);

    #endregion

    #region Playback Controls

    /// <summary>
    /// Starts playback of a specific song
    /// </summary>
    /// <param name="songUri">URI of the song to play</param>
    void Play(Uri songUri);

    /// <summary>
    /// Resumes playback of the current song
    /// </summary>
    void Resume();

    /// <summary>
    /// Pauses playback of the current song
    /// </summary>
    void Pause();

    /// <summary>
    /// Skips to the next song in the playlist
    /// </summary>
    void Next();

    /// <summary>
    /// Returns to the previous song in the playlist
    /// </summary>
    void Previous();

    #endregion

    #region Playback Settings

    /// <summary>
    /// Sets the playback volume
    /// </summary>
    /// <param name="volume">Volume level (0-100)</param>
    void SetVolume(double volume);

    /// <summary>
    /// Sets the playback rate
    /// </summary>
    /// <param name="rate">Playback rate (0.5-2.0)</param>
    void SetPlaybackRate(double rate);

    /// <summary>
    /// Enables or disables shuffle mode
    /// </summary>
    /// <param name="enable">True to enable shuffle, false to disable</param>
    void SetShuffle(bool enable);

    /// <summary>
    /// Sets the repeat mode
    /// </summary>
    /// <param name="mode">The repeat mode to set</param>
    void SetRepeatMode(RepeatMode mode);

    #endregion

    #region Position Control

    /// <summary>
    /// Seeks to a specific position in the current song
    /// </summary>
    /// <param name="position">Position to seek to</param>
    void Seek(TimeSpan position);

    /// <summary>
    /// Gets the current playback position
    /// </summary>
    /// <returns>Current position in the song</returns>
    TimeSpan GetCurrentPosition();

    /// <summary>
    /// Gets the total duration of the current song
    /// </summary>
    /// <returns>Total duration of the current song</returns>
    TimeSpan GetDuration();

    #endregion
}

/// <summary>
/// Defines the repeat modes for playback
/// </summary>
public enum RepeatMode
{
    /// <summary>
    /// No repeat
    /// </summary>
    None,

    /// <summary>
    /// Repeat all songs in the playlist
    /// </summary>
    All,

    /// <summary>
    /// Repeat current song
    /// </summary>
    One
}