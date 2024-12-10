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
using System.Collections.ObjectModel;
using Catel.Collections;
using Microsoft.UI.Xaml.Controls;
using Spotify.Views;

namespace Spotify.ViewModels;

public partial class PlaybackControlViewModel : ObservableObject, IDisposable
{

    public static PlaybackControlViewModel Instance { get; private set; }

    public static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new PlaybackControlViewModel();
        }
    }

    private static UserDTO CurrentUser => App.Current.CurrentUser;
    private readonly AdsService _adsService;
    private readonly PlaybackControlService _playbackService;
    private ObservableCollection<SongDTO> _playbacklist = new();
    private readonly ObservableCollection<SongDTO> _shuffledPlaylist = new();
    private int _currentIndex;
    private bool _isQueueVisible = true;
    private int _songsPlayedSinceLastAd = 0;
    private bool _isAdPlaying = false;
    private readonly bool _isPremium = CurrentUser.IsPremium;
    private bool _hasReachedHalfway = false;
    private AdsDTO _ads = new();

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
    private bool _isShowingLyricPage = false;


    // Constants
    private readonly string[] _speedOptions = new[] { "0.5x", "0.75x", "1.0x", "1.25x", "1.5x", "2.0x" };

    #region Commands

    public IRelayCommand PlayPauseCommand { get; }
    public IRelayCommand NextCommand { get; }
    public IRelayCommand PreviousCommand { get; }
    public IRelayCommand ShuffleCommand { get; }
    public IRelayCommand RepeatCommand { get; }
    public IRelayCommand ToggleQueueCommand { get; }
    public IRelayCommand ShowLyricCommand { get; }

    #endregion


    private PlaybackControlViewModel()
    {
        _playbackService = PlaybackControlService.Instance;
        _adsService = AdsService.GetInstance(
                App.Current.Services.GetRequiredService<IAdsDAO>()
            );

        // Initialize commands
        PlayPauseCommand = new RelayCommand(TogglePlayPause);
        NextCommand = new RelayCommand(Next);
        PreviousCommand = new RelayCommand(Previous);
        ShuffleCommand = new RelayCommand(ToggleShuffle);
        RepeatCommand = new RelayCommand(ToggleRepeat);
        ToggleQueueCommand = new RelayCommand(ToggleQueue);
        ShowLyricCommand = new RelayCommand(ShowLyricControl);

        // Initialize the playlist 
        try
        {

            // Get the queue from the app and assign it to the playbacklist on initialization
            _playbacklist = App.Current.ShellWindow.Queue;

            if (_playbacklist?.Count > 0)
            {
                CurrentSong = _playbacklist[0];
                UpdateShuffledPlaylist();

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

    #region Properties

    public ObservableCollection<SongDTO> PlaybackList
    {
        get => _playbacklist;
        set
        {
            if (SetProperty(ref _playbacklist, value))
            {
                OnPropertyChanged(nameof(PlaybackList));
            }
        }
    }
    // Current Song Properties
    public SongDTO CurrentSong
    {
        get => _currentSong;
        set
        {
            if (SetProperty(ref _currentSong, value))
            {
                // Update total duration
                _totalDuration = TimeSpan.FromSeconds(CurrentSongDurationInSeconds);
                _playbackService.AddCurrentSong(value);
                foreach (var song in _playbacklist)
                {
                    if (song == value)
                    {
                        song.IsCurrentSong = true;
                    }
                    else
                    {
                        song.IsCurrentSong = false;
                    }
                }

                OnPropertyChanged(nameof(CurrentSongTitle));
                OnPropertyChanged(nameof(CurrentArtistName));
                OnPropertyChanged(nameof(CurrentCoverArtUrl));
                OnPropertyChanged(nameof(TotalDurationDisplay));
                OnPropertyChanged(nameof(TotalDurationSeconds));
            }
        }
    }
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (SetProperty(ref _isPlaying, value))
            {
                OnPropertyChanged(nameof(PlayPauseIcon));
            }
        }
    }
    public string CurrentSongTitle => CurrentSong?.title ?? "No song playing";
    public string CurrentArtistName => CurrentSong?.ArtistName ?? "Unknown artist";
    public string CurrentCoverArtUrl => CurrentSong?.CoverArtUrl ?? "default_cover.jpg";
    public int CurrentSongDurationInSeconds => CurrentSong?.Duration ?? 0;

    public bool IsAdPlaying => _isAdPlaying;

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
            OnPropertyChanged(nameof(VolumeIcon)); // Update icon when volume changes
        }
    }
    public string VolumeIcon
    {
        get
        {
            if (Volume == 0)
            {
                return "\uE74F"; // Muted icon
            }
            else if (Volume < 25)
            {
                return "\uE993"; // Low volume icon
            }
            else if (Volume < 50)
            {
                return "\uE994"; // Low volume icon
            }
            else
            {
                return "\uE995"; // High volume icon
            }
        }
    }

    public bool IsQueueVisible
    {
        get => _isQueueVisible;
        set
        {
            SetProperty(ref _isQueueVisible, value);
        }
    }
    public bool IsShowingLyricPage
    {
        get => _isShowingLyricPage;
        set
        {
            SetProperty(ref _isShowingLyricPage, value);
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
    public string PlayPauseIcon => _isPlaying ? "\uE769" : "\uE768"; // Play : Pause 

    public string RepeatButtonColor => _repeatMode != RepeatMode.None ? "#1DB954" : "White";

    public string ShuffleButtonColor => _isShuffleEnabled ? "#1DB954" : "White";

    #endregion

    #region Command Implementations

    private void TogglePlayPause()
    {
        if (_isAdPlaying) return;

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

    public void Play(SongDTO song)
    {
        if (_isAdPlaying) return;

        if (_playbacklist.Contains(song))
        {
            //Do nothing
        }
        else
        {
            _playbacklist.Insert(_currentIndex, song);
        }
        CurrentSong = song;
        _playbackService.Play(new Uri(song.audio_url));

        _hasReachedHalfway = false; // Reset halfway marker for the new song
        CheckForAd();
    }

    private void Next()
    {
        if (_isAdPlaying) return; // Block action if ad is playing

        var playlist = new ObservableCollection<SongDTO>();
        if (_isShuffleEnabled)
        {
            playlist = _shuffledPlaylist;
            UpdateShuffledPlaylist();
        }
        else
        {
            playlist = _playbacklist;
        }
        if (playlist.Count == 0) return;

        if (_isShuffleEnabled)
        {
            while (CurrentSong == playlist[_currentIndex])
            {
                _currentIndex = Random.Shared.Next(playlist.Count);
            }
        }
        else
        {
            _currentIndex = (_currentIndex + 1) % playlist.Count;
        }
        Play(playlist[_currentIndex]);
    }

    private void Previous()
    {

        if (_isAdPlaying) return; // Block action if ad is playing

        var playlist = new ObservableCollection<SongDTO>();
        if (_isShuffleEnabled)
        {
            playlist = _shuffledPlaylist;
            UpdateShuffledPlaylist();
        }
        else
        {
            playlist = _playbacklist;
        }
        if (playlist.Count == 0) return;

        if (_isShuffleEnabled)
        {
            while (CurrentSong == playlist[_currentIndex])
            {
                _currentIndex = Random.Shared.Next(playlist.Count);
            }
        }
        else
        {
            _currentIndex = (_currentIndex - 1 + playlist.Count) % playlist.Count;
        }

        Play(playlist[_currentIndex]);
    }

    private void ToggleShuffle()
    {
        if (_isAdPlaying) return; // Block action if ad is playing

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

        if (_isAdPlaying) return; // Block action if ad is playing

        // Turn off shuffle
        _isShuffleEnabled = false;
        OnPropertyChanged(nameof(ShuffleButtonColor));

        // Business logic
        if (_repeatMode == RepeatMode.None)
        {
            _repeatMode = RepeatMode.One;
        }
        else
        {
            _repeatMode = RepeatMode.None;
        }
        OnPropertyChanged(nameof(RepeatButtonColor));
    }
    private void ToggleQueue()
    {
        if (_isAdPlaying) return; // Block action if ad is playing

        IsQueueVisible = !IsQueueVisible;
        Debug.WriteLine("Toggle queue");
    }
    private void ShowLyricControl()
    {
        if (_isAdPlaying) return; // Block action if ad is playing

        IsShowingLyricPage = !IsShowingLyricPage;
        var shellWindow = App.Current.ShellWindow;
        Frame mainFrame = shellWindow.getMainFrame();

        
        if (IsShowingLyricPage) // If we aren't showing the lyric page, navigate to it
        {
            mainFrame.Navigate(typeof(LyricPage), CurrentSong);
        }
        else // Otherwise, go back to the previous page
        {
            if(mainFrame.CanGoBack)
                mainFrame.GoBack();
        }
    }

    #endregion

    #region Private Helper Methods
    private void UpdateShuffledPlaylist()
    {
        _shuffledPlaylist.Clear();

        foreach (var song in _playbacklist)
        {
            _shuffledPlaylist.Add(song);
        }

        if (_isShuffleEnabled)
        {
            for (int i = _shuffledPlaylist.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (_shuffledPlaylist[i], _shuffledPlaylist[j]) = (_shuffledPlaylist[j], _shuffledPlaylist[i]);
            }
        }
    }

    private async Task CheckForAd()
    {
        if (_isPremium || _songsPlayedSinceLastAd < 3)
        {
            return;
        }

        // Play an ad
        _isAdPlaying = true;
        var ad = await _adsService.GetRandomAds();

        var adSong = new SongDTO
        {
            title = ad.title,
            ArtistName = ad.ArtistName,
            CoverArtUrl = ad.CoverArtUrl,
            audio_url = ad.audio_url,
            Duration = ad.Duration
        };

        CurrentSong = adSong;
        _playbackService.Play(new Uri(ad.audio_url));
        _songsPlayedSinceLastAd = 0;

        OnPropertyChanged(nameof(PlayPauseIcon));
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

            // Check if the current song has reached half its duration
            if (!_hasReachedHalfway && _currentSong != null && _currentPosition.TotalSeconds >= _totalDuration.TotalSeconds / 2)
            {
                _hasReachedHalfway = true; // Mark as having reached halfway
                _songsPlayedSinceLastAd++;  // Increment the counter
                Debug.WriteLine("Halfway reached, incrementing song count for ads.");
            }


            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(CurrentPositionSeconds));
            OnPropertyChanged(nameof(CurrentPositionDisplay));
        }


    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        if (_isAdPlaying)
        {
            _isAdPlaying = false;
            OnPropertyChanged(nameof(PlayPauseIcon));

            // Resume the next song automatically
            if (_playbacklist.Count > _currentIndex)
            {
                Play(_playbacklist[_currentIndex]);
            }
            else
            {
                Play(_playbacklist[0]);
            }

            return;
        }

        switch (this._repeatMode)
        {
            case RepeatMode.One:
                
                if (!_isPremium && _songsPlayedSinceLastAd > 2)
                {
                    CheckForAd();
                    break;
                }
                _songsPlayedSinceLastAd++;
                _playbackService.Seek(TimeSpan.Zero);
                _playbackService.Resume();
                break;

            case RepeatMode.None:
                Next();

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
        //if (_isDraggingSlider)
        //{
        //    _isDraggingSlider = false;
        //    _playbackService.Seek(_currentPosition);
        //}
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