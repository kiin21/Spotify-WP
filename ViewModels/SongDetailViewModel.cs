using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using System.ComponentModel;
using Windows.Services.Maps;

namespace Spotify.ViewModels
{
    /// <summary>
    /// ViewModel for managing song details and playback control.
    /// </summary>
    public partial class SongDetailViewModel : ObservableObject
    {
        private readonly PlaybackControlViewModel _playbackViewModel;

        [ObservableProperty]
        private SongDTO _song;

        [ObservableProperty]
        private string _playPauseGlyph;

        /// <summary>
        /// Command to toggle play/pause state.
        /// </summary>
        public IRelayCommand PlayPauseCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongDetailViewModel"/> class.
        /// </summary>
        /// <param name="song">The song details.</param>
        public SongDetailViewModel(SongDTO song)
        {
            PlayPauseCommand = new RelayCommand(TogglePlayPause);
            _playbackViewModel = PlaybackControlViewModel.Instance;

            // Initialize with passed song details
            Song = song;
            Song.plainLyrics = song.plainLyrics ?? "No lyrics available";

            // Subscribe to playback view model events
            _playbackViewModel.PropertyChanged += PlaybackViewModel_PropertyChanged;

            // Set the initial play/pause glyph
            PlayPauseGlyph = "\uE768"; // Play or Pause
        }

        /// <summary>
        /// Handles property changes in the playback view model.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlaybackViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check for changes in CurrentSong or PlaybackState
            if (e.PropertyName == nameof(PlaybackControlViewModel.CurrentSong))
            {
                // Update details if the current song changes
                if (Song != _playbackViewModel.CurrentSong)
                {
                    PlayPauseGlyph = "\uE768"; // Pause
                }
            }
            else if (e.PropertyName == nameof(PlaybackControlViewModel.PlayPauseIcon))
            {
                // Update details if the current song changes
                if (Song != _playbackViewModel.CurrentSong)
                {
                    PlayPauseGlyph = "\uE768"; // Pause
                }
                else
                {
                    PlayPauseGlyph = _playbackViewModel.IsPlaying ? "\uE769" : "\uE768"; // Play or Pause
                }
            }
        }

        /// <summary>
        /// Toggles the play/pause state of the playback.
        /// </summary>
        private void TogglePlayPause()
        {
            if (Song == _playbackViewModel.CurrentSong)
            {
                _playbackViewModel.PlayPauseCommand.Execute(null);
            }
            else
            {
                bool belongToPlaylist = false;
                _playbackViewModel.Play(Song, belongToPlaylist);
            }
            PlayPauseGlyph = _playbackViewModel.IsPlaying ? "\uE769" : "\uE768"; // Play or Pause
        }
    }
}