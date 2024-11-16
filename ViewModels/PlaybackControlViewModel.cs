using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Spotify.Contracts.Services;
using Spotify.Models.DTOs;
using Spotify.Engine;
using System.Windows.Threading;
using PropertyChanged;
using Spotify.Views;
using System.Collections.Generic;

namespace Spotify.ViewModels
{
    public partial class PlaybackControlViewModel : ObservableObject, IDisposable
    {
        private readonly IPlaybackControlService _playbackControlService;
        private readonly DispatcherTimer _playbackTimer;
        private readonly MusicEngine _musicEngine;
        private bool _disposed;
        private bool _isFirstPlayClicked = false;

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

        public ObservableCollection<string> SpeedOptions { get; } = new()
        {
            "1.0x",
            "1.25x",
            "1.5x",
            "1.75x",
            "2.0x",
            "2.5x",
        };

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
            get => IsPlaying ? "\uE103" : "\uE102"; // Play: E102, Pause: E103
            private set => SetProperty(ref _playPauseGlyph, value);
        }

        public PlaybackControlViewModel(IPlaybackControlService playbackControlService)
        {
            _playbackControlService = playbackControlService;
            _musicEngine = new MusicEngine();
            _musicEngine.MediaEnded += MediaPlayer_MediaEnded;

            _selectedSpeed = SpeedOptions.First();
            PlayPauseGlyph = "\uE102"; // Default to play glyph

            _playbackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _playbackTimer.Tick += PlaybackTimer_Tick;

            _playbackControlService.PlaybackStateChanged += OnPlaybackStateChanged;
            _playbackControlService.CurrentSongChanged += OnCurrentSongChanged;
            InitializeFromCurrentState();
        }

        private void InitializeFromCurrentState()
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

            _musicEngine.SetSource(new Uri(currentSong.AudioUrl));
            _musicEngine.SetVolume(Volume);
            _musicEngine.SetPosition(CurrentPosition);

            _ = LoadQueueAsync();
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (IsReplayEnabled)
            {
                _musicEngine.SetPosition(TimeSpan.Zero);
                _musicEngine.Play();
            }
            else
            {
                _ = NextCommand.ExecuteAsync(null);
            }
        }

        [SuppressPropertyChangedWarnings]
        partial void OnIsPlayingChanged(bool value)
        {
            if (value)
            {
                _musicEngine.Play();
                _playbackTimer.Start();
                PlayPauseGlyph = "\uE103"; // Pause glyph
            }
            else
            {
                _musicEngine.Pause();
                _playbackTimer.Stop();
                PlayPauseGlyph = "\uE102"; // Play glyph
            }
            OnPropertyChanged(nameof(PlayPauseGlyph));
        }

        [SuppressPropertyChangedWarnings]
        partial void OnVolumeChanged(double value)
        {
            _musicEngine.SetVolume(value);
        }

        [SuppressPropertyChangedWarnings]
        partial void OnSelectedSpeedChanged(string value)
        {
            if (_isProcessingSpeedChange) return;

            _isProcessingSpeedChange = true;
            var speedValue = double.Parse(value.TrimEnd('x'));
            _musicEngine.SetPlaybackRate(speedValue);
            _isProcessingSpeedChange = false;
        }

        private void UpdateSpeedFromService(string newSpeed)
        {
            if (_isProcessingSpeedChange) return;

            _isProcessingSpeedChange = true;
            var matchingSpeed = SpeedOptions.FirstOrDefault(x => x.StartsWith(newSpeed)) ?? SpeedOptions.First();
            var dispatcher = DispatcherQueue.GetForCurrentThread();
            dispatcher.TryEnqueue(() => SelectedSpeed = matchingSpeed);
            _isProcessingSpeedChange = false;
        }

        [SuppressPropertyChangedWarnings]
        partial void OnCurrentPositionChanged(TimeSpan value)
        {
            _musicEngine.SetPosition(value);
        }

        public void BeginSeeking()
        {
            _playbackTimer.Stop();
        }

        public void EndSeeking()
        {
            if (IsPlaying)
            {
                _playbackTimer.Start();
            }
        }

        [SuppressPropertyChangedWarnings]
        partial void OnIsReplayEnabledChanged(bool value)
        {
            // No need to handle this for MediaPlayer directly
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
            await _playbackControlService.NextAsync();
        }

        [RelayCommand]
        private async Task Previous()
        {
            await _playbackControlService.PreviousAsync();
        }

        [RelayCommand]
        private void ToggleLyrics()
        {
            IsLyricsVisible = !IsLyricsVisible;
            var currentSong = _playbackControlService.GetCurrentSong();
            var shellWindow = (App.Current as App).ShellWindow;
            if (shellWindow != null)
            {
                var mainFrame = shellWindow.getMainFrame();
                shellWindow.GetNavigationService().Navigate(typeof(LyricPage), mainFrame, currentSong);
            }
            else
            {
                Debug.WriteLine("ShellWindow is not initialized.");
            }
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
            await _playbackControlService.ShuffleAsync();
            await LoadQueueAsync();
        }

        private void PlaybackTimer_Tick(object sender, object e)
        {
            if (IsPlaying)
            {
                CurrentPosition = _musicEngine.GetPosition();
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
            IsPlaying = state.IsPlaying;
            Volume = state.Volume;
            UpdateSpeedFromService(state.PlaybackSpeed);
            CurrentPosition = FormatTimeSpan(state.CurrentPosition);
            SongDuration = FormatTimeSpan(state.Duration);
            IsReplayEnabled = state.IsRepeatEnabled;

            if (IsPlaying)
            {
                _playbackTimer.Start();
            }
            else
            {
                _playbackTimer.Stop();
            }
        }

        [SuppressPropertyChangedWarnings]
        private void OnCurrentSongChanged(object sender, SongPlaybackDTO song)
        {
            ImageSource = song.ImageUrl;
            Title = song.Title;
            Artist = song.Artist;
            SongDuration = FormatTimeSpan(song.Duration);
            SelectedSpeed = "1.0x";
            _musicEngine.SetSource(new Uri(song.AudioUrl));
            _ = LoadQueueAsync();
        }

        public async Task LoadQueueAsync()
        {
            var queueList = await _playbackControlService.GetQueueAsync();
            QueueSongs = new ObservableCollection<SongPlaybackDTO>(queueList ?? new List<SongPlaybackDTO>());
            OnPropertyChanged(nameof(QueueSongs));

            var currentSong = _playbackControlService.GetCurrentSong();
            var navigationParams = new Tuple<ObservableCollection<SongPlaybackDTO>, bool, SongPlaybackDTO, string, string, string, IPlaybackControlService>(
                QueueSongs, IsQueueVisible, currentSong, Title, Artist, ImageSource, _playbackControlService);

            var shellWindow = (App.Current as App).ShellWindow;
            if (shellWindow != null)
            {
                var rightSidebarFrame = shellWindow.getRightSidebarFrame();
                shellWindow.GetNavigationService().Navigate(typeof(QueuePage), rightSidebarFrame, navigationParams);
            }
            else
            {
                Debug.WriteLine("ShellWindow is not initialized.");
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
                    _musicEngine.Dispose();
                }
                _disposed = true;
            }
        }

        private TimeSpan FormatTimeSpan(TimeSpan time)
        {
            int totalSeconds = (int)Math.Round(time.TotalSeconds);
            return TimeSpan.FromSeconds(totalSeconds);
        }
    }
}