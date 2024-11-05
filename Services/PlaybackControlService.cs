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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Spotify.Helpers;
using System.Diagnostics;

namespace Spotify.Services;

public class PlaybackControlService : IPlaybackControlService
{
    private readonly IPlaybackControlDAO _playbackControlDAO;
    //    private readonly MediaPlayer _mediaPlayer;
    private WaveOutEvent _waveOutEvent;
    private MediaFoundationReader _mediaFoundationReader;
    //   private IWavePlayer _wavePlayer;
    private VarispeedSampleProvider _varispeedSampleProvider;
    private PlaybackStateDTO _currentState;
    private SongPlaybackDTO _currentSong = new SongPlaybackDTO(); // Initialize to empty song
    private List<SongPlaybackDTO> _queue;
    private bool _disposed;
    private VolumeSampleProvider _volumeProvider;
    private readonly object _lockObject = new object();
    private bool _isInitializing;


    public event EventHandler<PlaybackStateDTO> PlaybackStateChanged;
    public event EventHandler<SongPlaybackDTO> CurrentSongChanged;

    public PlaybackControlService(IPlaybackControlDAO playbackControlDAO)
    {
        _playbackControlDAO = playbackControlDAO;

        _waveOutEvent = new WaveOutEvent();
        _waveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;

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
                await LoadAndPlaySongAsync(_currentSong);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing playback: {ex.Message}");
            //InitializeDefaultState();
        }
    }

    private void InitializeDefaultState()
    {
        _currentState = new PlaybackStateDTO()
        {
            IsPlaying = false,
            Volume = 50,
            PlaybackSpeed = "0.5x",
            CurrentPosition = TimeSpan.Zero,
            Duration = TimeSpan.Zero,
            IsShuffleEnabled = false,
            IsRepeatEnabled = false
        };

        _waveOutEvent.Volume = (float)(_currentState.Volume / 100.0f);

        //   _mediaPlayer.Volume = _currentState.Volume / 100.0;
        //    _mediaPlayer.PlaybackRate = double.Parse(_currentState.PlaybackSpeed.TrimEnd('x'));
    }

    public Task<int> GetCurrentSongIndex()
    {
        if (_queue == null || _currentSong == null)
        {
            return Task.FromResult(-1);
        }

        int index = _queue.FindIndex(s => s.Id == _currentSong.Id);
        return Task.FromResult(index);
    }

    public PlaybackStateDTO GetCurrentState()
    {
        _currentState.CurrentPosition = _mediaFoundationReader.CurrentTime;
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
            //_mediaPlayer.Play();
            _waveOutEvent.Play();
            _currentState.IsPlaying = true;
            await UpdatePlaybackStateAsync();
        }
    }

    public async Task PauseAsync()
    {
        //_mediaPlayer.Pause();
        _waveOutEvent.Pause();
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
        //   _mediaPlayer.Volume = _currentState.Volume / 100.0;
        _waveOutEvent.Volume = (float)(_currentState.Volume / 100.0f);
        await UpdatePlaybackStateAsync();
    }

    public async Task SetPlaybackSpeedAsync(string speed)
    {
        if (_isInitializing) return;
        lock (_lockObject)
        {
            try
            {
                if (_varispeedSampleProvider == null || _mediaFoundationReader == null)
                    return;

                _currentState.PlaybackSpeed = speed;
                float playbackRate;
                if (!float.TryParse(speed.TrimEnd('x'), out playbackRate))
                {
                    playbackRate = 1.0f;
                }

                if (_varispeedSampleProvider != null)
                {
                    // Store current position before changing speed
                    var currentPosition = _mediaFoundationReader.CurrentTime;
                    bool wasPlaying = _currentState.IsPlaying;


                    // Stop playback temporarily
                    if (wasPlaying)
                    {
                        _waveOutEvent.Stop();
                    }

                    // Update the speed
                    //    _varispeedSampleProvider.PlaybackRate = playbackRate;

                    // Create a new provider chain with updated speed
                    var sampleProvider = _mediaFoundationReader.ToSampleProvider();
                    if (sampleProvider.WaveFormat.Channels > 1)
                    {
                        sampleProvider = sampleProvider.ToMono();
                    }

                    _varispeedSampleProvider = new VarispeedSampleProvider(sampleProvider)
                    {
                        PlaybackRate = playbackRate
                    };

                    _volumeProvider = new VolumeSampleProvider(_varispeedSampleProvider)
                    {
                        Volume = (float)(_currentState.Volume / 100.0f)
                    };

                    // Initialize with new provider
                    _waveOutEvent.Init(_volumeProvider);

                    // Restore position
                    _mediaFoundationReader.CurrentTime = currentPosition;

                    // Resume playback if it was playing
                    if (wasPlaying)
                    {
                        _waveOutEvent.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting playback speed: {ex.Message}");

            }
        }
        await UpdatePlaybackStateAsync();
    }

    public async Task SeekToPositionAsync(TimeSpan position)
    {
        //    _mediaPlayer.Position = position;
        if (_mediaFoundationReader != null)
        {
            _mediaFoundationReader.CurrentTime = position;
            _currentState.CurrentPosition = position;
            await UpdatePlaybackStateAsync();
        }
    }

    public async Task SetRepeatAsync(bool isRepeatEnabled)
    {
        _currentState.IsRepeatEnabled = isRepeatEnabled;
        await _playbackControlDAO.SetRepeatStateAsync(isRepeatEnabled);
        await UpdatePlaybackStateAsync();
    }

    public async Task LoadAndPlaySongAsync(SongPlaybackDTO song)
    {
        if (song == null) return;
        lock (_lockObject)
        {
            if (_isInitializing) return;
            _isInitializing = true;

            try
            {
                _currentSong = song;

                if (_waveOutEvent != null)
                {
                    _waveOutEvent.Stop();
                }

                if (_mediaFoundationReader != null)
                {
                    _mediaFoundationReader.Dispose();
                    _mediaFoundationReader = null;
                }

                _mediaFoundationReader = new MediaFoundationReader(song.AudioUrl);
                var sampleProvider = _mediaFoundationReader.ToSampleProvider();

                if (sampleProvider.WaveFormat.Channels > 1)
                {
                    sampleProvider = sampleProvider.ToMono();
                }

                float currentSpeed;
                if (!float.TryParse(_currentState.PlaybackSpeed.TrimEnd('x'), out currentSpeed))
                {
                    currentSpeed = 1.0f;
                }

                _varispeedSampleProvider = new VarispeedSampleProvider(sampleProvider)
                {
                    PlaybackRate = currentSpeed
                };

                _volumeProvider = new VolumeSampleProvider(_varispeedSampleProvider)
                {
                    Volume = (float)(_currentState.Volume / 100.0f)
                };

                // Use a buffered wave provider to smooth out playback
                var bufferedProvider = new BufferedWaveProvider(_volumeProvider.WaveFormat)
                {
                    BufferLength = _volumeProvider.WaveFormat.AverageBytesPerSecond * 2,
                    DiscardOnBufferOverflow = true
                };

                _waveOutEvent.Init(_volumeProvider);

                _currentState.CurrentPosition = TimeSpan.Zero;
                _currentState.Duration = song.Duration;

                if (_currentState.IsPlaying)
                {
                    _waveOutEvent.Play();
                }

                //   await UpdatePlaybackStateAsync();
                OnCurrentSongChanged(song);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading song: {ex.Message}");
            }
            finally
            {
                _isInitializing = false;
            }
        }
        await UpdatePlaybackStateAsync();
    }

    private async Task UpdatePlaybackStateAsync()
    {
        try
        {
            if (_mediaFoundationReader == null || _currentState == null)
            {
                return;
            }

            // Add a check to prevent accessing CurrentTime if stream is in an invalid state
            if (_mediaFoundationReader.Length > 0)
            {
                _currentState.CurrentPosition = _mediaFoundationReader.CurrentTime;
            }

            PlaybackStateChanged?.Invoke(this, _currentState);

            // Only update if DAO is available and not null
            if (_playbackControlDAO != null)
            {
                await _playbackControlDAO.UpdatePlaybackStateAsync(_currentState);
            }
        }
        catch (System.Runtime.InteropServices.COMException comEx)
        {
            // Log more detailed COM exception information
            System.Diagnostics.Debug.WriteLine($"COM Exception updating playback state: {comEx.Message}");
            System.Diagnostics.Debug.WriteLine($"Error Code: {comEx.HResult}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error updating playback state: {ex.Message}");
        }
    }

    private void OnCurrentSongChanged(SongPlaybackDTO song)
    {
        CurrentSongChanged?.Invoke(this, song);
    }

    private void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
    {
        try
        {
            if (_mediaFoundationReader == null || _isInitializing) return;

            lock (_lockObject)
            {
                var currentPosition = _mediaFoundationReader.CurrentTime;
                var duration = _currentState.Duration;

                // Only consider it ended if we're very close to the end
                bool isAtEnd = (duration - currentPosition).TotalSeconds <= 0.5;

                if (isAtEnd)
                {
                    if (_currentState.IsRepeatEnabled)
                    {
                        _mediaFoundationReader.CurrentTime = TimeSpan.Zero;
                        if (!_isInitializing)
                        {
                            _waveOutEvent.Play();
                        }
                    }
                    else
                    {
                        _ = NextAsync();
                    }
                }
                else if (_currentState.IsPlaying)
                {
                    // If we stopped but weren't at the end and should be playing, resume
                    _waveOutEvent.Play();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in PlaybackStopped: {ex.Message}");
        }
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
                // _mediaPlayer.Dispose();
                _waveOutEvent?.Dispose();
                _mediaFoundationReader?.Dispose();
            }
            _disposed = true;
        }
    }
    public async Task AddToQueueAsync(SongPlaybackDTO song)
    {
        try
        {
            if (_queue == null)
            {
                _queue = new List<SongPlaybackDTO>();
            }

            await _playbackControlDAO.AddToQueueAsync(song);
            _queue = await _playbackControlDAO.GetQueueAsync(); // Refresh queue

            // If no song is currently playing, start playing the added song
            if (_currentSong == null)
            {
                await LoadAndPlaySongAsync(song);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding song to queue: {ex.Message}");
        }
    }

    public async Task AddToHeadOfQueueAsync(SongPlaybackDTO song)
    {
        try
        {
            if (_queue == null)
            {
                _queue = new List<SongPlaybackDTO>();
                await _playbackControlDAO.AddToHeadOfQueueAsync(song);
                _queue = await _playbackControlDAO.GetQueueAsync(); // Refresh queue
            }
            else
            {
                if (song.Id == _queue.First().Id)
                {
                    return;
                }
                else
                {
                    await _playbackControlDAO.AddToHeadOfQueueAsync(song);
                    _queue = await _playbackControlDAO.GetQueueAsync(); // Refresh queue
                }

            }
            // If no song is currently playing, start playing the added song
            if (_currentSong == null)
            {
                _currentSong = song;
                await LoadAndPlaySongAsync(song);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding song to queue: {ex.Message}");
        }
    }
    public async Task AddToNextInQueueAsync(SongPlaybackDTO song)
    {
        try
        {
            if (_queue == null)
            {
                _queue = new List<SongPlaybackDTO>();
                await _playbackControlDAO.AddToHeadOfQueueAsync(song);
                _queue = await _playbackControlDAO.GetQueueAsync(); // Refresh queue
            }
            else
            {
                if (song.Id == _currentSong.Id)
                {
                    return;
                }
                else
                {
                    await _playbackControlDAO.AddToNextInQueueAsync(song);
                    _queue = await _playbackControlDAO.GetQueueAsync(); // Refresh queue
                }

            }
            // If no song is currently playing, start playing the added song
            if (_currentSong == null)
            {
                _currentSong = song;
                await LoadAndPlaySongAsync(song);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding song to queue: {ex.Message}");
        }
    }
    public async Task PlaySongById(string id)
    {
        await PauseAsync();
        var originalIndex = _queue.ToList().FindIndex(song => song.Id == id);
        var currentIndex = GetCurrentSongIndex().Result;
        if (originalIndex >= 0)
        {
            var skipCount = originalIndex - currentIndex;

            if (skipCount > 0)
            {
                for (int i = 0; i < skipCount; i++)
                {
                    await NextAsync();
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(skipCount); i++)
                {
                    await PreviousAsync();
                }
            }

            await SetPlayPauseAsync(true);
        }
    }
}
