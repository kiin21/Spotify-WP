using System;
using System.Collections.Generic;
using System.Windows;
using Catel.IoC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.Contracts.DAO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Spotify.ViewModels;

public partial class PlaybackControlViewModel : ObservableObject, IDisposable
{
    private readonly PlaybackControlService _playbackService;
    private List<SongDTO> _playlist = new();
    private readonly List<SongDTO> _shuffledPlaylist = new();
    private int _currentIndex;
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    // State fields
    private SongDTO _currentSong;
    private double _volume = 50;
    private TimeSpan _currentPosition = TimeSpan.Zero;
    private TimeSpan _totalDuration = TimeSpan.Zero;
    private bool _isPlaying;
    private bool _isShuffleEnabled;
    private RepeatMode _repeatMode = RepeatMode.None;
    private string _selectedSpeed = "1.0x";
    private bool _isDraggingSlider;

    // Constants
    private readonly string[] _speedOptions = new[] { "0.5x", "0.75x", "1.0x", "1.25x", "1.5x", "2.0x" };

    #region Commands

    public IRelayCommand PlayPauseCommand { get; }
    public IRelayCommand NextCommand { get; }
    public IRelayCommand PreviousCommand { get; }
    public IRelayCommand ShuffleCommand { get; }
    public IRelayCommand RepeatCommand { get; }

    #endregion

    // Queue service
    private readonly QueueService _queueService;

    public PlaybackControlViewModel()
    {
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        _queueService = new QueueService(
            App.Current.Services.GetRequiredService<IQueueDAO>(),
            App.Current.Services.GetRequiredService<ISongDAO>());
        _playbackService = PlaybackControlService.Instance;

        // Initialize commands
        PlayPauseCommand = new RelayCommand(TogglePlayPause);
        NextCommand = new RelayCommand(Next);
        PreviousCommand = new RelayCommand(Previous);
        ShuffleCommand = new RelayCommand(ToggleShuffle);
        RepeatCommand = new RelayCommand(ToggleRepeat);

        // Initialize the playlist 
        try
        {
            _playlist = App.Current.Queue;

            if (_playlist?.Count > 0)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    CurrentSong = _playlist[0];
                    UpdateShuffledPlaylist();
                });
            }

            // Subscribe to service events
            _playbackService.PlaybackStateChanged += OnPlaybackStateChanged;
            _playbackService.PositionChanged += OnPositionChanged;
            _playbackService.MediaEnded += OnMediaEnded;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing playlist: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            // Handle error (e.g., show a message to the user)
        }

    }

    private async Task<List<SongDTO>> InitializeQueue()
    {
        List<SongDTO> queue = await _queueService.GetQueueById("1234567");
        foreach (SongDTO song in queue)
        {
            Debug.WriteLine(song);
        }
        return queue;
    }

    #region Properties

    // Current Song Properties
    public SongDTO CurrentSong
    {
        get => _currentSong;
        private set
        {
            if (SetProperty(ref _currentSong, value))
            {
                // Update total duration
                _totalDuration = TimeSpan.FromSeconds(CurrentSongDurationInSeconds);

                OnPropertyChanged(nameof(CurrentSongTitle));
                OnPropertyChanged(nameof(CurrentArtistName));
                OnPropertyChanged(nameof(CurrentCoverArtUrl));
                OnPropertyChanged(nameof(TotalDurationDisplay));
            }
        }
    }

    public string CurrentSongTitle => CurrentSong?.title ?? "No song playing";
    public string CurrentArtistName => CurrentSong?.ArtistName ?? "Unknown artist";
    public string CurrentCoverArtUrl => CurrentSong?.CoverArtUrl ?? "default_cover.jpg";
    public int CurrentSongDurationInSeconds => CurrentSong?.Duration ?? 0;

    // Volume Control
    public double Volume
    {
        get => _volume;
        set
        {
            if (SetProperty(ref _volume, value))
            {
                _playbackService.SetVolume(value);
            }
        }
    }

    // Playback Speed
    public string[] SpeedOptions => _speedOptions;

    public string SelectedSpeed
    {
        get => _selectedSpeed;
        set
        {
            if (SetProperty(ref _selectedSpeed, value))
            {
                var speed = double.Parse(value.TrimEnd('x'));
                _playbackService.SetPlaybackRate(speed);
            }
        }
    }

    // Position and Duration
    public double CurrentPositionSeconds
    {
        get => _currentPosition.TotalSeconds;
        set
        {
            if (!_isDraggingSlider)
            {
                CurrentPosition = TimeSpan.FromSeconds(value);
            }
        }
    }

    public TimeSpan CurrentPosition
    {
        get => _currentPosition;
        set
        {
            if (SetProperty(ref _currentPosition, value))
            {
                _playbackService.Seek(value);
                OnPropertyChanged(nameof(CurrentPositionDisplay));
                OnPropertyChanged(nameof(CurrentPositionSeconds));
            }
        }
    }

    public string CurrentPositionDisplay => _currentPosition.ToString(@"mm\:ss");

    public double TotalDurationSeconds => _totalDuration.TotalSeconds;

    public string TotalDurationDisplay => _totalDuration.ToString(@"mm\:ss");

    // Playback State
    public string PlayPauseIcon => _isPlaying ? "\uE769" : "\uE768"; // Pause : Play

    public string RepeatButtonColor => _repeatMode != RepeatMode.None ? "#1DB954" : "White";

    public string ShuffleButtonColor => _isShuffleEnabled ? "#1DB954" : "White";

    #endregion

    #region Command Implementations

    private void TogglePlayPause()
    {
        if (_isPlaying)
        {
            _playbackService.Pause();
        }
        else if (_currentSong != null)
        {
            if (CurrentPosition == TimeSpan.Zero)
            {
                _playbackService.Play(new Uri(_currentSong.audio_url));
            }
            else
            {
                _playbackService.Resume();
            }
        }
    }

    private void Next()
    {
        var playlist = _isShuffleEnabled ? _shuffledPlaylist : _playlist;
        if (playlist.Count == 0) return;

        _currentIndex = (_currentIndex + 1) % playlist.Count;
        CurrentSong = playlist[_currentIndex];

        _playbackService.Play(new Uri(CurrentSong.audio_url));
    }

    private void Previous()
    {
        var playlist = _isShuffleEnabled ? _shuffledPlaylist : _playlist;
        if (playlist.Count == 0) return;

        _currentIndex = (_currentIndex - 1 + playlist.Count) % playlist.Count;
        CurrentSong = playlist[_currentIndex];
        _playbackService.Play(new Uri(CurrentSong.audio_url));
    }

    private void ToggleShuffle()
    {
        // Business logic
        _isShuffleEnabled = !_isShuffleEnabled;
        UpdateShuffledPlaylist();
        OnPropertyChanged(nameof(ShuffleButtonColor));
        
        // Turn off repeat mode
        _repeatMode = RepeatMode.None;
        OnPropertyChanged(nameof(RepeatButtonColor));
    }

    private void ToggleRepeat()
    {
        // Turn off shuffle
        _isShuffleEnabled = false;
        OnPropertyChanged(nameof(ShuffleButtonColor));

        // Business logic
        _repeatMode = _repeatMode switch
        {
            RepeatMode.None => RepeatMode.One,
            RepeatMode.One => RepeatMode.None,
            // All other cases
            _ => RepeatMode.None
        };
        OnPropertyChanged(nameof(RepeatButtonColor));
    }

    #endregion

    #region Private Helper Methods

    private void UpdateShuffledPlaylist()
    {
        _shuffledPlaylist.Clear();
        _shuffledPlaylist.AddRange(_playlist);

        if (_isShuffleEnabled)
        {
            for (int i = _shuffledPlaylist.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (_shuffledPlaylist[i], _shuffledPlaylist[j]) = (_shuffledPlaylist[j], _shuffledPlaylist[i]);
            }
        }
    }

    #endregion

    #region Event Handlers

    private void OnPlaybackStateChanged(object sender, bool isPlaying)
    {
        _isPlaying = isPlaying;
        OnPropertyChanged(nameof(PlayPauseIcon));
    }

    private void OnPositionChanged(object sender, TimeSpan position)
    {
        // Check if we should update the position
        if (!_isDraggingSlider && Math.Abs(_currentPosition.TotalSeconds - position.TotalSeconds) > 0.5)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                _currentPosition = position;
                OnPropertyChanged(nameof(CurrentPosition));
                OnPropertyChanged(nameof(CurrentPositionSeconds));
                OnPropertyChanged(nameof(CurrentPositionDisplay));
            });
        }
    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        switch (_repeatMode)
        {
            case RepeatMode.One:
                _playbackService.Seek(TimeSpan.Zero);
                _playbackService.Resume();
                break;

            //case RepeatMode.All:
            //    Next();
            //    break;

            case RepeatMode.None:
                var playlist = _isShuffleEnabled ? _shuffledPlaylist : _playlist;
                if (_currentIndex < playlist.Count - 1)
                {
                    Next();
                }
                else
                {
                    _isPlaying = false;
                    OnPropertyChanged(nameof(PlayPauseIcon));
                }
                break;

            default:
                break;
        }
    }

    #endregion

    #region Slider Interaction Methods

    public void OnSliderDragStarted()
    {
        _isDraggingSlider = true;
    }

    public void OnSliderDragCompleted()
    {
        if (_isDraggingSlider)
        {
            _isDraggingSlider = false;
            _playbackService.Seek(_currentPosition);
        }
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        _playbackService.PlaybackStateChanged -= OnPlaybackStateChanged;
        _playbackService.PositionChanged -= OnPositionChanged;
        _playbackService.MediaEnded -= OnMediaEnded;
    }

    #endregion
}
