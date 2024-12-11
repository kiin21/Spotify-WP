using Microsoft.Extensions.DependencyInjection;
using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.ViewModels
{
    /// <summary>
    /// ViewModel for managing the sharing of playlists.
    /// </summary>
    public class SharePlaylistViewModel
    {
        /// <summary>
        /// Gets or sets the collection of users available for sharing the playlist.
        /// </summary>
        public ObservableCollection<UserShareDTO> Users { get; set; }
        private readonly PlaylistDTO _playlist;
        private readonly UserService _userService;
        private readonly PlaylistService _playlistService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePlaylistViewModel"/> class.
        /// </summary>
        /// <param name="playlist">The playlist to be shared.</param>
        public SharePlaylistViewModel(PlaylistDTO playlist)
        {
            _playlist = playlist;

            // Get UserService and PlaylistService from App container
            _userService = (App.Current as App).Services.GetRequiredService<UserService>();
            _playlistService = (App.Current as App).Services.GetRequiredService<PlaylistService>();

            Users = new ObservableCollection<UserShareDTO>();
            _ = LoadUsersAsync();
        }

        /// <summary>
        /// Loads the users asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetUsersAsync();

            // Exclude the current user from the list
            var currentUserId = (App.Current as App).CurrentUser.Id;
            users = users.Where(u => u.Id != currentUserId).ToList();

            foreach (var user in users)
            {
                Users.Add(new UserShareDTO
                {
                    UserId = user.Id,
                    Username = user.Username,
                    IsSelected = false
                });
            }
        }

        /// <summary>
        /// Shares the playlist with the selected users asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SharePlaylistAsync()
        {
            var selectedUsers = Users.Where(u => u.IsSelected).ToList();
            foreach (var user in selectedUsers)
            {
                await _playlistService.SharePlaylistAsync(_playlist.Id, user.UserId);
            }
        }
    }

    /// <summary>
    /// Data transfer object for sharing a user.
    /// </summary>
    public class UserShareDTO
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the user is selected for sharing.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
