using Spotify.Models.DTOs;
using Spotify.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Spotify.ViewModels
{
    /// <summary>
    /// ViewModel for managing the left sidebar, including playlists.
    /// </summary>
    public class LeftSidebarPageViewModel : INotifyPropertyChanged
    {
        private readonly PlaylistService _playlistService;
        private ObservableCollection<PlaylistDTO> _playlists { get; set; }

        /// <summary>
        /// Gets or sets the collection of playlists.
        /// </summary>
        public ObservableCollection<PlaylistDTO> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        private PlaylistDTO _selectedPlaylist;
        /// <summary>
        /// Gets or sets the selected playlist.
        /// </summary>
        public PlaylistDTO SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftSidebarPageViewModel"/> class.
        /// </summary>
        /// <param name="playlistService">The service for managing playlists.</param>
        public LeftSidebarPageViewModel(PlaylistService playlistService)
        {
            _playlistService = playlistService;
            LoadPlaylists();
        }

        private async void LoadPlaylists()
        {
            var currentUser = (App.Current as App).CurrentUser;

            if (currentUser == null || string.IsNullOrEmpty(currentUser.Id))
            {
                throw new InvalidOperationException("User is not logged in or user ID is invalid.");
            }

            // Ensure the user has a "Liked Songs" playlist
            await _playlistService.EnsureLikedSongsPlaylistAsync(currentUser.Id, currentUser.Username);

            // Get the user's playlists
            var playlists = await _playlistService.GetPlaylistsByUserIdAsync(currentUser.Id);

            // Get playlists shared with the user
            var sharedPlaylists = await _playlistService.GetSharedPlaylistsAsync(currentUser.Id);

            // Merge both lists
            var allPlaylists = playlists.Concat(sharedPlaylists).ToList();

            // Filter duplicate playlists
            allPlaylists = allPlaylists.GroupBy(p => p.Id).Select(g => g.First()).ToList();

            // Assign to ObservableCollection
            Playlists = new ObservableCollection<PlaylistDTO>(allPlaylists);

            // Select the first playlist (if any)
            SelectedPlaylist = Playlists.FirstOrDefault();
        }

        /// <summary>
        /// Adds a new playlist asynchronously.
        /// </summary>
        /// <param name="playlistName">The name of the new playlist.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddPlaylistAsync(string playlistName)
        {
            var newPlaylist = new PlaylistDTO
            {
                Title = playlistName,
                CreatedBy = (App.Current as App).CurrentUser.Username,
                CreatedAt = DateTime.Now,
                IsLikedSong = false,
                IsDeleted = false,
                Avatar = "ms-appx:///Assets/defaultSong.png",
                OwnerId = (App.Current as App).CurrentUser.Id
            };

            await _playlistService.AddPlaylistAsync(newPlaylist);
            Playlists.Add(newPlaylist);
        }

        /// <summary>
        /// Removes a playlist by its ID.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to remove.</param>
        public void RemovePlaylist(string playlistId)
        {
            var playlistToRemove = Playlists.FirstOrDefault(p => p.Id == playlistId);
            if (playlistToRemove != null)
            {
                Playlists.Remove(playlistToRemove);
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
