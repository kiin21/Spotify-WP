using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Spotify.Helpers;
using Spotify.Services;
using Spotify.Models.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Spotify.Contracts.Services;
using PropertyChanged;
using Microsoft.UI.Dispatching;
using System.Diagnostics;
using Spotify.Views;
using System.DirectoryServices;
using Microsoft.Extensions.DependencyInjection;


namespace Spotify.ViewModels;


public partial class PlaybackControlViewModel : ObservableObject, IDisposable
{

    private readonly IPlaybackControlService _playbackControlService;
    private readonly DispatcherTimer _playbackTimer;
    private bool _disposed;
    private bool _isFirstPlayClicked = false;
    //    private bool _isUserSeeking; // Add this field to track user-initiated seeking


    [ObservableProperty]
    public bool _isPlaying;

    [ObservableProperty]
    public double _volume;

    [ObservableProperty]
    public TimeSpan _currentPosition;

    [ObservableProperty]
    public TimeSpan _songDuration;

    [ObservableProperty]
    public bool _isReplayEnabled;

    [ObservableProperty]
    public bool _isLyricsVisible;

    [ObservableProperty]
    public bool _isQueueVisible = true;

    [ObservableProperty]
    public string _imageSource;

    [ObservableProperty]
    public string _title;

    [ObservableProperty]
    public string _artist;

    [ObservableProperty]
    private string _selectedSpeed;

    private bool _isProcessingSpeedChange;

    // Change to use simple string collection for speeds
    public ObservableCollection<string> SpeedOptions { get; } = new()
    {
        "1.0x",
        "1.25x",
        "1.5x",
        "1.75x",
        "2.0x",
        "2.5x",
    };

    private TimeSpan FormatTimeSpan(TimeSpan time)
    {
        // Round to nearest second and ensure proper format
        int totalSeconds = (int)Math.Round(time.TotalSeconds);
        return TimeSpan.FromSeconds(totalSeconds);
    }

    public double CurrentPositionSeconds
    {
        get => Math.Round(CurrentPosition.TotalSeconds);
        set => CurrentPosition = FormatTimeSpan(TimeSpan.FromSeconds(value));
    }

    public double SongDurationSeconds => Math.Round(SongDuration.TotalSeconds);

    [ObservableProperty]
    public ObservableCollection<SongPlaybackDTO> _queueSongs = new();

    private string _playPauseGlyph;
    public string PlayPauseGlyph
    {
        get => _playPauseGlyph;
        private set => SetProperty(ref _playPauseGlyph, value);
    }


    public PlaybackControlViewModel(IPlaybackControlService playbackControlService)
    {
        try
        {
            _playbackControlService = playbackControlService;

            _selectedSpeed = SpeedOptions.First();

            PlayPauseGlyph = "\uF5B0";

            _playbackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _playbackTimer.Tick += PlaybackTimer_Tick;

            _playbackControlService.PlaybackStateChanged += OnPlaybackStateChanged;

            _playbackControlService.CurrentSongChanged += OnCurrentSongChanged;
            InitializeFromCurrentState();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in ViewModel constructor: {ex}");
            throw; // or handle appropriately
        }
    }

    private void InitializeFromCurrentState()
    {
        try
        {
            var state = _playbackControlService.GetCurrentState();
            var currentSong = _playbackControlService.GetCurrentSong();

            IsPlaying = state.IsPlaying;
            Volume = state.Volume;
            UpdateSpeedFromService(state.PlaybackSpeed);
            CurrentPosition = state.CurrentPosition;
            SongDuration = state.Duration;
            IsReplayEnabled = state.IsRepeatEnabled;

            ImageSource = currentSong.ImageUrl;
            Title = currentSong.Title;
            Artist = currentSong.Artist;

            _ = LoadQueueAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            System.Diagnostics.Debug.WriteLine($"Error initializing state: {ex.Message}");
        }
    }

    [SuppressPropertyChangedWarnings]
    partial void OnIsPlayingChanged(bool value)
    {
        try
        {
            _ = _playbackControlService.SetPlayPauseAsync(value);
            if (value)
            {
                _playbackTimer.Start();
                PlayPauseGlyph = "\uF8AE";
            }
            else
            {
                _playbackTimer.Stop();
                PlayPauseGlyph = "\uF5B0";
            }
            OnPropertyChanged(nameof(PlayPauseGlyph));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error changing playback state: {ex.Message}");
        }
    }

    [SuppressPropertyChangedWarnings]
    partial void OnVolumeChanged(double value)
    {
        try
        {
            _ = _playbackControlService.SetVolumeAsync(value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error changing volume: {ex.Message}");
        }
    }


    [SuppressPropertyChangedWarnings]
    partial void OnSelectedSpeedChanged(string value)
    {
        if (_isProcessingSpeedChange) return;

        try
        {
            _isProcessingSpeedChange = true;

            Task.Run(async () =>
            {
                try
                {
                    // Remove 'x' before sending to service
                    string speedValue = value.TrimEnd('x');
                    await _playbackControlService.SetPlaybackSpeedAsync(speedValue);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in async speed change: {ex.Message}");
                }
                finally
                {
                    _isProcessingSpeedChange = false;
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error changing playback speed: {ex.Message}");
            _isProcessingSpeedChange = false;
        }
    }

    private void UpdateSpeedFromService(string newSpeed)
    {
        if (_isProcessingSpeedChange) return;
        try
        {
            _isProcessingSpeedChange = true;

            // Find matching speed option (with 'x' suffix)
            var matchingSpeed = SpeedOptions.FirstOrDefault(x => x.StartsWith(newSpeed))
                ?? SpeedOptions.First();

            // Get the dispatcher from the current window
            var dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            if (dispatcher != null)
            {
                dispatcher.TryEnqueue(() =>
                {
                    SelectedSpeed = matchingSpeed;
                });
            }
            else
            {
                // Fallback if we're already on the UI thread
                SelectedSpeed = matchingSpeed;
            }
        }
        finally
        {
            _isProcessingSpeedChange = false;
        }
    }

    [SuppressPropertyChangedWarnings]
    partial void OnCurrentPositionChanged(TimeSpan value)
    {
        try
        {

            Task.Run(async () =>
            {
                await _playbackControlService.SeekToPositionAsync(value);
            }).Wait(100); // Small timeout to prevent flooding

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeking position: {ex.Message}");
        }
    }

    // Add method to handle user-initiated seeking
    public void BeginSeeking()
    {
        _playbackTimer.Stop();
    }

    public void EndSeeking()
    {
        if (IsPlaying)
        {
            _playbackTimer.Start(); // Restart timer if playing
        }
    }

    [SuppressPropertyChangedWarnings]
    partial void OnIsReplayEnabledChanged(bool value)
    {
        try
        {
            _ = _playbackControlService.SetRepeatAsync(value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error changing replay state: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task PlayPause()
    {
        IsPlaying = !IsPlaying;
        if (!_isFirstPlayClicked)
        {
            _ = LoadQueueAsync();
            _isFirstPlayClicked = true;
            await Console.Out.WriteLineAsync("First play clicked");
        }
    }

    [RelayCommand]
    private async Task Next()
    {
        try
        {
            await _playbackControlService.NextAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing next track: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Previous()
    {
        try
        {
            await _playbackControlService.PreviousAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing previous track: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ToggleLyrics()
    {
        IsLyricsVisible = !IsLyricsVisible;
    }

    [RelayCommand]
    private void ToggleQueue()
    {
        IsQueueVisible = !IsQueueVisible;
        _ = LoadQueueAsync();
    }

    [RelayCommand]
    private void ToggleReplay()
    {
        IsReplayEnabled = !IsReplayEnabled;
    }

    [RelayCommand]
    private async Task Shuffle()
    {
        try
        {
            await _playbackControlService.ShuffleAsync();
            await LoadQueueAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error shuffling queue: {ex.Message}");
        }
    }

    private void PlaybackTimer_Tick(object sender, object e)
    {
        try
        {
            if (IsPlaying)
            {
                var state = _playbackControlService.GetCurrentState();

                // Get the current position from the service
                var servicePosition = (state.CurrentPosition);

                // Smooth update threshold
                double thresholdInSeconds = 0.9999;

                // Update only if the difference is beyond the threshold
                if (Math.Abs((servicePosition - CurrentPosition).TotalSeconds) > thresholdInSeconds)
                {
                    CurrentPosition = servicePosition;
                    //    OnPropertyChanged(nameof(CurrentPositionSeconds));
                }

                // Check if we've reached the end of the song  
                if (CurrentPosition >= SongDuration)
                {
                    if (IsReplayEnabled)
                    {
                        // Reset to beginning if replay is enabled
                        CurrentPosition = TimeSpan.Zero;
                    }
                    else
                    {
                        _ = NextCommand.ExecuteAsync(null);
                    }
                }

                // Update other UI bindings here if necessary
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in playback timer tick: {ex.Message}");
        }
    }


    [SuppressPropertyChangedWarnings]
    private void OnPlaybackStateChanged(object sender, PlaybackStateDTO state)
    {
        try
        {
            IsPlaying = state.IsPlaying;
            Volume = state.Volume;

            UpdateSpeedFromService(state.PlaybackSpeed);

            CurrentPosition = FormatTimeSpan(state.CurrentPosition);

            SongDuration = FormatTimeSpan(state.Duration);
            IsReplayEnabled = state.IsRepeatEnabled;

            // Ensure timer is running/stopped based on playback state
            if (IsPlaying)
            {
                _playbackTimer.Start();
            }
            else
            {
                _playbackTimer.Stop();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling playback state change: {ex.Message}");
        }
    }

    // 5. Update OnCurrentSongChanged method
    [SuppressPropertyChangedWarnings]
    private void OnCurrentSongChanged(object sender, SongPlaybackDTO song)
    {
        try
        {
            ImageSource = song.ImageUrl;
            Title = song.Title;
            Artist = song.Artist;

            // Parse duration with proper format
            if (TimeSpan.TryParseExact(song.Duration.ToString(@"hh\:mm\:ss"), @"hh\:mm\:ss", null, out TimeSpan parsedDuration))
            {
                SongDuration = parsedDuration;
            }
            else
            {
                // Fallback to formatting the existing TimeSpan
                SongDuration = FormatTimeSpan(song.Duration);
            }

            // Set playback speed to 1.0x in the ViewModel
            SelectedSpeed = "1.0x";

            // Set playback speed to 1.0 in the backend
            _ = _playbackControlService.SetPlaybackSpeedAsync("1.0");

            UpdateSpeedFromService("1.0");
            _ = LoadQueueAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling current song change: {ex.Message}");
        }
    }

    public async Task LoadQueueAsync()
    {
        try
        {
            var queueList = await _playbackControlService.GetQueueAsync();
            if (queueList != null)
            {
                // Create a new ObservableCollection with explicit type checking
                QueueSongs = new ObservableCollection<SongPlaybackDTO>(
                    queueList.Select(song =>
                    {
                        // Ensure each item is actually a SongPlaybackDTO
                        if (song == null)
                        {
                            throw new InvalidCastException("Null song in queue");
                        }
                        return song;
                    })
                );
                OnPropertyChanged(nameof(QueueSongs));
            }
            else
            {
                QueueSongs = new ObservableCollection<SongPlaybackDTO>();
            }

            var currentSong = _playbackControlService.GetCurrentSong();
            var navigationParams = 
                    new Tuple<
                                ObservableCollection<SongPlaybackDTO>, 
                                bool, SongPlaybackDTO, 
                                string, 
                                string, 
                                string, 
                                IPlaybackControlService>(QueueSongs, IsQueueVisible, 
                                                            currentSong, Title, Artist, ImageSource, _playbackControlService);


            var shellWindow = (App.Current as App).ShellWindow;
            if (shellWindow != null)
            {
                var rightSidebarFrame = shellWindow.getRightSidebarFrame();
                shellWindow.GetNavigationService().Navigate(typeof(QueuePage), rightSidebarFrame, navigationParams);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ShellWindow is not initialized.");
            }


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading queue: {ex.Message}");
            // Initialize empty collection on error
            QueueSongs = new ObservableCollection<SongPlaybackDTO>();
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
                _playbackTimer.Stop();
                _playbackTimer.Tick -= PlaybackTimer_Tick;
                _playbackControlService.PlaybackStateChanged -= OnPlaybackStateChanged;
                _playbackControlService.CurrentSongChanged -= OnCurrentSongChanged;
            }
            _disposed = true;
        }
    }

}
