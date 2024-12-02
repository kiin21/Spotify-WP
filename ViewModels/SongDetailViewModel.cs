using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spotify.Models.DTOs;
using Spotify.ViewModels;
using System.ComponentModel;
using Windows.Services.Maps;

namespace Spotify.ViewModels
{
    public partial class SongDetailViewModel : ObservableObject
    {
        private readonly PlaybackControlViewModel _playbackViewModel;

        [ObservableProperty]
        private SongDTO _song;

        [ObservableProperty]
        private string _playPauseGlyph;
        public IRelayCommand PlayPauseCommand { get; }
        public SongDetailViewModel(SongDTO song)
        {
            PlayPauseCommand = new RelayCommand(TogglePlayPause);
            _playbackViewModel = PlaybackControlViewModel.Instance;
            
            //_playbackViewModel.CurrentSong = song; //////////////////////////////
            
            // Initialize with passed song details
            Song = song;
            Song.plainLyrics = song.plainLyrics ?? "No lyrics available";

            // Subscribe to playback view model events
            _playbackViewModel.PropertyChanged += PlaybackViewModel_PropertyChanged;

            //_playbackViewModel.IsPlaying = false; //////////////////////////////

            // Set the initial play/pause glyph
            //PlayPauseGlyph = _playbackViewModel.IsPlaying ? "\uE769" : "\uE768"; // Play or Pause //////////////////////////////
            PlayPauseGlyph = "\uE768"; // Play or Pause
        }

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
            else if(e.PropertyName == nameof(PlaybackControlViewModel.PlayPauseIcon))
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

        private void TogglePlayPause()
        {
            if (Song == _playbackViewModel.CurrentSong)
            {
                _playbackViewModel.PlayPauseCommand.Execute(null);
            }
            else
            {
                _playbackViewModel.Play(Song);
            }
            PlayPauseGlyph = _playbackViewModel.IsPlaying ? "\uE769" : "\uE768"; // Play or Pause
        }
    }
}