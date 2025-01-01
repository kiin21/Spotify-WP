using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.Contracts.DAO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Spotify.Views;
namespace Spotify.ViewModels;

public partial class PlaybackControlViewModel : ObservableObject, IDisposable
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="PlaybackControlViewModel"/> class.
    /// </summary>
    public static PlaybackControlViewModel Instance { get; private set; }

    /// <summary>
    /// Initializes the singleton instance of the <see cref="PlaybackControlViewModel"/> class.
    /// </summary>
    public static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new PlaybackControlViewModel();
        }
    }

    /// <summary>
    /// Gets the current user.
    /// </summary>
    private static UserDTO CurrentUser => App.Current.CurrentUser;

    private readonly AdsService _adsService;
    private readonly PlaybackControlService _playbackService;
    private readonly PlayHistoryService _playHistoryService;

    private ObservableCollection<SongDTO> _playbacklist = new();
    private readonly ObservableCollection<SongDTO> _shuffledPlaylist = new();
    private int _currentIndex;
    private bool _isQueueVisible = true;
    private int _songsPlayedSinceLastAd = 0;
    private bool _isAdPlaying = false;
    private readonly bool _isPremium = false;
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

    /// <summary>
    /// Gets the command to toggle play/pause state.
    /// </summary>
    public IRelayCommand PlayPauseCommand { get; }

    /// <summary>
    /// Gets the command to play the next song.
    /// </summary>
    public IRelayCommand NextCommand { get; }

    /// <summary>
    /// Gets the command to play the previous song.
    /// </summary>
    public IRelayCommand PreviousCommand { get; }

    /// <summary>
    /// Gets the command to toggle shuffle mode.
    /// </summary>
    public IRelayCommand ShuffleCommand { get; }

    /// <summary>
    /// Gets the command to toggle repeat mode.
    /// </summary>
    public IRelayCommand RepeatCommand { get; }

    /// <summary>
    /// Gets the command to toggle the visibility of the queue.
    /// </summary>
    public IRelayCommand ToggleQueueCommand { get; }

    /// <summary>
    /// Gets the command to show the lyric control.
    /// </summary>
    public IRelayCommand ShowLyricCommand { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackControlViewModel"/> class.
    /// </summary>
    private PlaybackControlViewModel()
    {
        
        _playbackService = PlaybackControlService.Instance;
        _adsService = AdsService.GetInstance(
                App.Current.Services.GetRequiredService<IAdsDAO>()
            );
        _playHistoryService = new PlayHistoryService(App.Current.Services.GetService<IPlayHistoryDAO>());

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

    /// <summary>
    /// Gets or sets the playback list.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the current song.
    /// </summary>
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
                    song.IsCurrentSong = song == value;
                }

                // Save play history asynchronously
                // SavePlayHistory(value);

                // Notify UI
                OnPropertyChanged(nameof(CurrentSongTitle));
                OnPropertyChanged(nameof(CurrentArtistName));
                OnPropertyChanged(nameof(CurrentCoverArtUrl));
                OnPropertyChanged(nameof(TotalDurationDisplay));
                OnPropertyChanged(nameof(TotalDurationSeconds));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the media is currently playing.
    /// </summary>
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

    /// <summary>
    /// Gets the title of the current song.
    /// </summary>
    public string CurrentSongTitle => CurrentSong?.title ?? "No song playing";

    /// <summary>
    /// Gets the name of the current artist.
    /// </summary>
    public string CurrentArtistName => CurrentSong?.ArtistName ?? "Unknown artist";

    /// <summary>
    /// Gets the cover art URL of the current song.
    /// </summary>
    public string CurrentCoverArtUrl => CurrentSong?.CoverArtUrl ?? "default_cover.jpg";

    /// <summary>
    /// Gets the duration of the current song in seconds.
    /// </summary>
    public int CurrentSongDurationInSeconds => CurrentSong?.Duration ?? 0;

    /// <summary>
    /// Gets a value indicating whether an advertisement is currently playing.
    /// </summary>
    public bool IsAdPlaying => _isAdPlaying;

    // Volume Control

    /// <summary>
    /// Gets or sets the volume level.
    /// </summary>
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

    /// <summary>
    /// Gets the icon representing the current volume level.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the queue is visible.
    /// </summary>
    public bool IsQueueVisible
    {
        get => _isQueueVisible;
        set
        {
            SetProperty(ref _isQueueVisible, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the lyric page is showing.
    /// </summary>
    public bool IsShowingLyricPage
    {
        get => _isShowingLyricPage;
        set
        {
            SetProperty(ref _isShowingLyricPage, value);
        }
    }

    // Playback Speed

    /// <summary>
    /// Gets the available playback speed options.
    /// </summary>
    public string[] SpeedOptions => _speedOptions;

    /// <summary>
    /// Gets or sets the selected playback speed.
    /// </summary>
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

    /// <summary>
    /// <summary>
    /// Gets or sets the current playback position in seconds.
    /// </summary>
    public double CurrentPositionSeconds
    {
        get => _currentPosition.TotalSeconds;
        set
        {
            if (!IsAdPlaying || CurrentUser.IsPremium)
            {
                CurrentPosition = TimeSpan.FromSeconds(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the current playback position.
    /// </summary>
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

    /// <summary>
    /// Gets the display string for the current playback position.
    /// </summary>
    public string CurrentPositionDisplay => _currentPosition.ToString(@"mm\:ss");

    /// <summary>
    /// Gets the total duration of the current song in seconds.
    /// </summary>
    public double TotalDurationSeconds => _totalDuration.TotalSeconds;

    /// <summary>
    /// Gets the display string for the total duration of the current song.
    /// </summary>
    public string TotalDurationDisplay => _totalDuration.ToString(@"mm\:ss");

    // Playback State

    /// <summary>
    /// Gets the icon representing the play/pause state.
    /// </summary>
    public string PlayPauseIcon => _isPlaying ? "\uE769" : "\uE768"; // Play : Pause 

    /// <summary>
    /// Gets the color of the repeat button.
    /// </summary>
    public string RepeatButtonColor => _repeatMode != RepeatMode.None ? "#1DB954" : "White";

    /// <summary>
    /// Gets the color of the shuffle button.
    /// </summary>
    public string ShuffleButtonColor => _isShuffleEnabled ? "#1DB954" : "White";

    #endregion


    #region Command Implementations

    /// <summary>
    /// Toggles the play/pause state of the current song.
    /// </summary>
    public void TogglePlayPause()
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

    /// <summary>
    /// Plays the specified song.
    /// </summary>
    /// <param name="song">The song to play.</param>
    /// <param name="belongToPlaylist">Indicates whether the song belongs to a playlist.</param>
    public async void Play(SongDTO song, bool belongToPlaylist = false)
    {
        if (_isAdPlaying) return;

        if (_playbacklist.Contains(song))
        {
            // Do nothing
        }
        else
        {
            SavePlayHistory(_currentSong);
            if (!belongToPlaylist) { _playbacklist.Clear(); }
            // Save in playlist temporarily, not save in database
            _playbacklist.Insert(0, song);
            CurrentSong = song;
            _playbackService.Play(new Uri(song.audio_url));
        }
        CurrentSong = song;
        _playbackService.Play(new Uri(song.audio_url));

        _hasReachedHalfway = false; // Reset halfway marker for the new song
        await CheckForAd();
    }

    /// <summary>
    /// Plays the next song in the playlist.
    /// </summary>
    public void Next()
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

    /// <summary>
    /// Plays the previous song in the playlist.
    /// </summary>
    public void Previous()
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

    /// <summary>
    /// Toggles shuffle mode.
    /// </summary>
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

    /// <summary>
    /// Toggles repeat mode.
    /// </summary>
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

    /// <summary>
    /// Toggles the visibility of the queue.
    /// </summary>
    private void ToggleQueue()
    {
        if (_isAdPlaying) return; // Block action if ad is playing

        IsQueueVisible = !IsQueueVisible;
        Debug.WriteLine("Toggle queue");
    }

    /// <summary>
    /// Shows the lyric control.
    /// </summary>
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
            if (mainFrame.CanGoBack)
                mainFrame.GoBack();
        }
    }

    #endregion


    #region Private Helper Methods

    /// <summary>
    /// Saves the play history for the specified song asynchronously.
    /// </summary>
    /// <param name="song">The song to save in the play history.</param>
    private async void SavePlayHistory(SongDTO song)
    {
        try
        {
            var userID = App.Current.CurrentUser.Id;
            var songID = song.Id.ToString();
            var currentTime = DateTime.Now;
            TimeSpan totalTime = _playbackService.GetTotalListeningTime();
            await _playHistoryService.SavePlayHistoryAsync(userID, songID, currentTime, totalTime);
        }
        catch (Exception ex)
        {
            // Handle or log the exception
            Console.WriteLine($"Error saving play history: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds the specified songs to the playback list.
    /// </summary>
    /// <param name="songs">The collection of songs to add.</param>
    /// <param name="belongToPlaylist">Indicates whether the songs belong to a playlist.</param>
    public void AddToPlaybackList(ObservableCollection<SongDTO> songs, bool belongToPlaylist = false)
    {
        if (belongToPlaylist)
        {
            _playbacklist.Clear();
            foreach (var song in songs)
            {
                _playbacklist.Add(song);
            }
            CurrentSong = _playbacklist[0];
            _playbackService.Resume();
        }
        else
        {
            // FIX_LATTER
            foreach (var song in songs)
            {
                _playbacklist.Add(song);
            }
        }
    }

    /// <summary>
    /// Updates the shuffled playlist based on the current playback list.
    /// </summary>
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

    /// <summary>
    /// Checks if an advertisement should be played and plays it if necessary.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task CheckForAd()
    {
        if (CurrentUser.IsPremium || _songsPlayedSinceLastAd < 3)
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

    private async void OnMediaEnded(object sender, EventArgs e)
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
        SavePlayHistory(_currentSong);
        switch (this._repeatMode)
        {
            case RepeatMode.One:

                if (!CurrentUser.IsPremium && _songsPlayedSinceLastAd > 2)
                {
                    await CheckForAd();
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