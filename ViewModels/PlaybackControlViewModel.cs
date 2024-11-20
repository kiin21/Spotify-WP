using System;
using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spotify.Models.DTOs;
using Spotify.Services;

namespace Spotify.ViewModels;

public partial class PlaybackControlViewModel : ObservableObject, IDisposable
{
    private readonly PlaybackControlService _playbackService;
    private readonly List<SongDTO> _playlist;
    private readonly List<SongDTO> _shuffledPlaylist = new();
    private int _currentIndex;

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

    public PlaybackControlViewModel()
    {
        _playbackService = PlaybackControlService.Instance;

        // Initialize mock playlist
        _playlist = new List<SongDTO>
        {
            new SongDTO
            {
                title = "Memories",
                ArtistName = "Maroon 5",
                CoverArtUrl = "https://i.scdn.co/image/ab67616d0000b2735dbaecd8dfa2c325da65245c",
                Duration = 195,
                audio_url = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FMaroon%205%20-%20Memories%20(Official%20Video).mp3?alt=media&token=cf9ba411-5edf-4e6c-bc5c-a170f71e2c4e",
            },
            new SongDTO
            {
                title = "On My Way",
                ArtistName = "Alan Walker",
                CoverArtUrl = "https://i.scdn.co/image/ab67616d0000b273d2aaf635815c265aa1ecdecc",
                Duration = 194,
                audio_url = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FAlan%20Walker%2C%20Sabrina%20Carpenter%20%26%20Farruko%20-%20On%20My%20Way.mp3?alt=media&token=adb6f047-4aae-4a32-9829-52e7c72a310d"
            }
            // Add more mock songs as needed
        };

        // Play the first song on initialization
        CurrentSong = _playlist[0];
        UpdateShuffledPlaylist();

        // Subscribe to service events
        _playbackService.PlaybackStateChanged += OnPlaybackStateChanged;
        _playbackService.PositionChanged += OnPositionChanged;
        _playbackService.MediaEnded += OnMediaEnded;

        // Initialize commands
        PlayPauseCommand = new RelayCommand(TogglePlayPause);
        NextCommand = new RelayCommand(Next);
        PreviousCommand = new RelayCommand(Previous);
        ShuffleCommand = new RelayCommand(ToggleShuffle);
        RepeatCommand = new RelayCommand(ToggleRepeat);
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

    #region Commands

    public IRelayCommand PlayPauseCommand { get; }
    public IRelayCommand NextCommand { get; }
    public IRelayCommand PreviousCommand { get; }
    public IRelayCommand ShuffleCommand { get; }
    public IRelayCommand RepeatCommand { get; }

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
        _isShuffleEnabled = !_isShuffleEnabled;
        //_playbackService.SetShuffle(_isShuffleEnabled);
        UpdateShuffledPlaylist();
        OnPropertyChanged(nameof(ShuffleButtonColor));
    }

    private void ToggleRepeat()
    {
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
            _currentPosition = position;
            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(CurrentPositionSeconds));
            OnPropertyChanged(nameof(CurrentPositionDisplay));
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
