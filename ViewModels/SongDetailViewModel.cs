using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace Spotify.ViewModels
{
    /// <summary>
    /// ViewModel for managing song details and playback control.
    /// </summary>
    public partial class SongDetailViewModel : ObservableObject
    {
        private readonly PlaybackControlViewModel _playbackViewModel;

        private readonly CommentService _commentService;

        [ObservableProperty]
        private SongDTO _song;

        [ObservableProperty]
        private string _playPauseGlyph;

        [ObservableProperty]
        private ObservableCollection<CommentDTO> _comments = new();

        [ObservableProperty]
        private string _newCommentContent;

        [ObservableProperty]
        private string _currentUserName;
        public IRelayCommand AddCommentCommand { get; }

        /// <summary>
        /// Command to toggle play/pause state.
        /// </summary>
        public IRelayCommand PlayPauseCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SongDetailViewModel"/> class.
        /// </summary>
        /// <param name="song">The song details.</param>
        public SongDetailViewModel(SongDTO song, CommentService commentService)
        {
            _playbackViewModel = PlaybackControlViewModel.Instance;
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));

            // Initialize commands
            PlayPauseCommand = new RelayCommand(TogglePlayPause);
            AddCommentCommand = new RelayCommand(async () => await AddComment());

            // Initialize song details
            Song = song;
            Song.plainLyrics = song.plainLyrics ?? "No lyrics available";

            // Subscribe to playback view model events
            _playbackViewModel.PropertyChanged += PlaybackViewModel_PropertyChanged;

            // Set the initial play/pause glyph
            PlayPauseGlyph = "\uE768"; // Play icon

            // Get Current User
            var currentUser = App.Current.CurrentUser;
            _currentUserName = currentUser.Username;

            // Load comments asynchronously
            _ = LoadComments();
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

        /// <summary>
        /// Loads comments for the current song asynchronously.
        /// </summary>
        private async Task LoadComments()
        {
            try
            {
                var comments = await _commentService.GetCommentsBySongIdAsync(Song.Id.ToString());
                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log them or display a message to the user)
                Console.WriteLine($"Error loading comments: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new comment for the current song.
        /// </summary>
        private async Task AddComment()
        {
            if (string.IsNullOrWhiteSpace(NewCommentContent))
                return;

            try
            {
                var currentUser = App.Current.CurrentUser; // Get the current user
                var newComment = new CommentDTO
                {
                    SongId = Song.Id.ToString(),
                    Content = NewCommentContent,
                    AddedBy = currentUser.Username,
                    AddedAt = DateTime.UtcNow
                };

                await _commentService.AddCommentAsync(newComment.SongId, newComment.Content, currentUser.Username);

                // Add the new comment to the UI
                Comments.Add(newComment);

                // Clear the input field
                NewCommentContent = string.Empty;
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log them or display a message to the user)
                Console.WriteLine($"Error adding comment: {ex.Message}");
            }
        }
    }
}