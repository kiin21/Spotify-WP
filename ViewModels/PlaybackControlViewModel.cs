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
using System.Diagnostics;

namespace Spotify.ViewModels;


public partial class PlaybackControlViewModel : ObservableObject, IDisposable
{
    
    private readonly IPlaybackControlService _playbackControlService;
    private readonly DispatcherTimer _playbackTimer;
    private bool _disposed;

    [ObservableProperty]
    public bool _isPlaying;

    [ObservableProperty]
    public double _volume;

    [ObservableProperty]
    public string _selectedSpeed = "1.0x";

    [ObservableProperty]
    public TimeSpan _currentPosition;

    [ObservableProperty]
    public TimeSpan _songDuration;

    [ObservableProperty]
    public bool _isReplayEnabled;

    [ObservableProperty]
    public bool _isLyricsVisible;

    [ObservableProperty]
    public bool _isQueueVisible;

    [ObservableProperty]
    public string _imageSource;

    [ObservableProperty]
    public string _title;

    [ObservableProperty]
    public string _artist;

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

    [ObservableProperty]
    public string _playPauseGlyph;



    public PlaybackControlViewModel(PlaybackControlService playbackControlService)
    {
        try
        {
            _playbackControlService = playbackControlService;

            PlayPauseGlyph = "\uE768";

            _playbackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
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
            SelectedSpeed = state.PlaybackSpeed;
            CurrentPosition = state.CurrentPosition;
            SongDuration = state.Duration;
            IsReplayEnabled = state.IsRepeatEnabled;

            ImageSource = currentSong.ImageUrl;
            Title = currentSong.Title;
            Artist = currentSong.Artist;

           _ = LoadQueueAsync();
            Console.WriteLine("Initialize successfully!!!");
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
                PlayPauseGlyph = "\uE769";
            }
            else
            {
                _playbackTimer.Stop();
                PlayPauseGlyph = "\uE768";
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
        try
        {
            // Remove the 'x' suffix if present
            string speedValue = value.TrimEnd('x');
            _ = _playbackControlService.SetPlaybackSpeedAsync(speedValue);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error changing playback speed: {ex.Message}");
        }
    }

    [SuppressPropertyChangedWarnings]
    partial void OnCurrentPositionChanged(TimeSpan value)
    {
        try
        {
            _ = _playbackControlService.SeekToPositionAsync(value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeking position: {ex.Message}");
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
        if (IsPlaying)
        {
            CurrentPosition = FormatTimeSpan(CurrentPosition + TimeSpan.FromSeconds(1));
            if (CurrentPosition >= SongDuration)
            {
                if (IsReplayEnabled)
                {
                    CurrentPosition = TimeSpan.Zero;
                }
                else
                {
                    _ = NextCommand.ExecuteAsync(null);
                }
            }
        }
    }

    [SuppressPropertyChangedWarnings]
    private void OnPlaybackStateChanged(object sender, PlaybackStateDTO state)
    {
        try
        {
            IsPlaying = state.IsPlaying;
            Volume = state.Volume;
            SelectedSpeed = state.PlaybackSpeed;
            CurrentPosition = FormatTimeSpan(state.CurrentPosition);
            SongDuration = FormatTimeSpan(state.Duration);
            IsReplayEnabled = state.IsRepeatEnabled;
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
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling current song change: {ex.Message}");
        }
    }

    private async Task LoadQueueAsync()
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
