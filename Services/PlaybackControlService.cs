using Spotify.Contracts.DAO;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;
using System.Threading.Tasks;
using System.Windows.Documents;
using Windows.Media;
using Windows.Media.Playback;
using System.Collections.Generic;
using System;
using Windows.Media.Core;
using System.Linq;

namespace Spotify.Services;

public class PlaybackControlService : IPlaybackControlService
{
    private readonly IPlaybackControlDAO _playbackControlDAO;
    private readonly MediaPlayer _mediaPlayer;
    private PlaybackStateDTO _currentState;
    private SongPlaybackDTO _currentSong;
    private List<SongPlaybackDTO> _queue;
    private bool _disposed;

    public event EventHandler<PlaybackStateDTO> PlaybackStateChanged;
    public event EventHandler<SongPlaybackDTO> CurrentSongChanged;

    public PlaybackControlService(IPlaybackControlDAO playbackControlDAO)
    {
        _playbackControlDAO = playbackControlDAO;
        _mediaPlayer = new MediaPlayer();
        _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
        _mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;

        InitializeDefaultState();
        InitializePlaybackAsync().Wait();
    }

    private async Task InitializePlaybackAsync()
    {
        try
        {
            _currentState = await _playbackControlDAO.GetPlaybackStateAsync();
            _currentSong = await _playbackControlDAO.GetCurrentSongAsync();
            _queue = await _playbackControlDAO.GetQueueAsync();

            if (_currentSong != null)
            {
                // Set up the media source for the current song
                var mediaSource = MediaSource.CreateFromUri(new Uri(_currentSong.AudioUrl));
                _mediaPlayer.Source = mediaSource;

                // Update state with current song's duration
                _currentState.Duration = _currentSong.Duration;

                // Notify subscribers of the initial song
                OnCurrentSongChanged(_currentSong);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing playback: {ex.Message}");
            InitializeDefaultState();
        }
    }

    private void InitializeDefaultState()
    {
        _currentState = new PlaybackStateDTO()
        {
            IsPlaying = false,
            Volume = 50,
            PlaybackSpeed = "1.0x",
            CurrentPosition = TimeSpan.Zero,
            Duration = TimeSpan.Zero,
            IsShuffleEnabled = false,
            IsRepeatEnabled = false
        };

        _mediaPlayer.Volume = _currentState.Volume / 100.0;
        _mediaPlayer.PlaybackRate = double.Parse(_currentState.PlaybackSpeed.TrimEnd('x'));
    }

    public PlaybackStateDTO GetCurrentState()
    {
        return _currentState;
    }

    public SongPlaybackDTO GetCurrentSong()
    {
        return _currentSong;
    }

    public async Task<List<SongPlaybackDTO>> GetQueueAsync()
    {
        try
        {
            if (_queue == null)
            {
                _queue = new List<SongPlaybackDTO>();
            }

            var queueFromDao = await _playbackControlDAO.GetQueueAsync();
            if (queueFromDao != null)
            {
                _queue = queueFromDao;
            }
            return _queue.ToList(); // Return a new copy of the list
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading queue: {ex.Message}");
            return new List<SongPlaybackDTO>();
        }
    }

    public async Task PlayAsync()
    {
        if (_currentSong != null)
        {
            _mediaPlayer.Play();
            _currentState.IsPlaying = true;
            await UpdatePlaybackStateAsync();
        }
    }

    public async Task PauseAsync()
    {
        _mediaPlayer.Pause();
        _currentState.IsPlaying = false;
        await UpdatePlaybackStateAsync();
    }

    public async Task SetPlayPauseAsync(bool isPlaying)
    {
        if (isPlaying)
        {
            await PlayAsync();
        }
        else
        {
            await PauseAsync();
        }
    }

    public async Task NextAsync()
    {
        try
        {
            var nextSong = await _playbackControlDAO.GetNextSongAsync();

            if (nextSong != null)
            {
                await LoadAndPlaySongAsync(nextSong);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing next track: {ex.Message}");
        }
    }

    public async Task PreviousAsync()
    {
        try
        {
            var previousSong = await _playbackControlDAO.GetPreviousSongAsync();

            if (previousSong != null)
            {
                await LoadAndPlaySongAsync(previousSong);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing previous track: {ex.Message}");
        }
    }

    public async Task ShuffleAsync()
    {
        try
        {
            _currentState.IsShuffleEnabled = !_currentState.IsShuffleEnabled;
            if (_queue == null)
            {
                await GetQueueAsync();
            }

            if (_queue != null && _queue.Count > 0 && _currentState.IsShuffleEnabled)
            {
                await _playbackControlDAO.ShuffleQueueAsync();
                await UpdatePlaybackStateAsync();
                await GetQueueAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($" Error setting shuffle: {ex.Message}");
        }
    }

    public async Task SetVolumeAsync(double volume)
    {
        _currentState.Volume = Math.Clamp(volume, 0, 100);
        _mediaPlayer.Volume = _currentState.Volume / 100.0;
        await UpdatePlaybackStateAsync();
    }

    public async Task SetPlaybackSpeedAsync(string speed)
    {
        _currentState.PlaybackSpeed = speed;
        _mediaPlayer.PlaybackRate = double.Parse(speed.TrimEnd('x'));
        await UpdatePlaybackStateAsync();
    }

    public async Task SeekToPositionAsync(TimeSpan position)
    {
        _mediaPlayer.Position = position;
        _currentState.CurrentPosition = position;
        await UpdatePlaybackStateAsync();
    }

    public async Task SetRepeatAsync(bool isRepeatEnabled)
    {
        _currentState.IsRepeatEnabled = isRepeatEnabled;
        await _playbackControlDAO.SetRepeatStateAsync(isRepeatEnabled);
        await UpdatePlaybackStateAsync();
    }

    private async Task LoadAndPlaySongAsync(SongPlaybackDTO song)
    {
        try { 
            _currentSong = song;
            var mediaSource = MediaSource.CreateFromUri(new Uri(song.AudioUrl));
            _mediaPlayer.Source = mediaSource;

            _currentState.CurrentPosition = TimeSpan.Zero;
            _currentState.Duration = song.Duration;

            if(_currentState.IsPlaying)
            {
                _mediaPlayer.Play();
            }

            await UpdatePlaybackStateAsync();
            OnCurrentSongChanged(song);
        }
        catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Error loading song: {ex.Message}");
        }
    }

    private async Task UpdatePlaybackStateAsync()
    {
        try
        {
           _currentState.CurrentPosition = _mediaPlayer.Position;
            PlaybackStateChanged?.Invoke(this, _currentState);
            await _playbackControlDAO.UpdatePlaybackStateAsync(_currentState);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating playback state: {ex.Message}");
        }
    }

    private void OnCurrentSongChanged(SongPlaybackDTO song)
    {
        CurrentSongChanged?.Invoke(this, song);
    }

    private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        if (_currentState.IsRepeatEnabled)
        {
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
        }
        else
        {
            _ = NextAsync();
        }
    }

    private void MediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args) {
        System.Diagnostics.Debug.WriteLine($"Media playback failed: {args.Error} - {args.ErrorMessage}");
    }

    private void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
    {
        _currentState.Duration = _mediaPlayer.NaturalDuration;
        _ = UpdatePlaybackStateAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _mediaPlayer.Dispose();
            }
            _disposed = true;
        }
    }
}
