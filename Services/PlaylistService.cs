using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify.Services
{
    /// <summary>
    /// Provides services for managing playlists.
    /// </summary>
    public class PlaylistService
    {
        private readonly IPlaylistDAO _playlistDAO;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistService"/> class.
        /// </summary>
        /// <param name="playlistDAO">The data access object for playlists.</param>
        /// <exception cref="ArgumentNullException">Thrown when the playlistDAO is null.</exception>
        public PlaylistService(IPlaylistDAO playlistDAO)
        {
            _playlistDAO = playlistDAO ?? throw new ArgumentNullException(nameof(playlistDAO));
        }

        /// <summary>
        /// Gets all playlists asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of playlists.</returns>
        public Task<List<PlaylistDTO>> GetPlaylistsAsync() =>
            _playlistDAO.GetPlaylistsAsync();

        /// <summary>
        /// Gets a playlist by ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the playlist.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the playlist with the specified ID.</returns>
        public Task<PlaylistDTO> GetPlaylistByIdAsync(string id) =>
            _playlistDAO.GetPlaylistByIdAsync(id);

        /// <summary>
        /// Gets the liked songs playlist for a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the liked songs playlist for the user.</returns>
        public Task<PlaylistDTO> GetLikedSongsPlaylistAsync(string userId) =>
            _playlistDAO.GetLikedSongsPlaylistAsync(userId);

        /// <summary>
        /// Adds a new playlist asynchronously.
        /// </summary>
        /// <param name="playlist">The playlist to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the playlist is null.</exception>
        public Task AddPlaylistAsync(PlaylistDTO playlist)
        {
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            return _playlistDAO.AddPlaylistAsync(playlist);
        }

        /// <summary>
        /// Updates the status of a playlist asynchronously.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <param name="isDeleted">The new status of the playlist.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdatePlaylistStatusAsync(string playlistId, bool isDeleted)
        {
            await _playlistDAO.UpdatePlaylistStatusAsync(playlistId, isDeleted);
        }

        /// <summary>
        /// Removes a playlist asynchronously.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to remove.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemovePlaylistAsync(string playlistId)
        {
            await _playlistDAO.RemovePlaylistAsync(playlistId);
        }

        /// <summary>
        /// Gets playlists by user ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of playlists for the user.</returns>
        public async Task<List<PlaylistDTO>> GetPlaylistsByUserIdAsync(string userId)
        {
            return await _playlistDAO.GetPlaylistsByUserIdAsync(userId);
        }

        /// <summary>
        /// Ensures that the liked songs playlist exists for a user, creating it if necessary.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the liked songs playlist for the user.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the userId or username is null or empty.</exception>
        public async Task<PlaylistDTO> EnsureLikedSongsPlaylistAsync(string userId, string username)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
                throw new ArgumentNullException("UserId or Username is null.");

            var likedSongsPlaylist = await _playlistDAO.GetLikedSongsPlaylistAsync(userId);

            if (likedSongsPlaylist == null)
            {
                // Create new "Liked Songs" playlist
                likedSongsPlaylist = new PlaylistDTO
                {
                    Title = "Liked Songs",
                    CreatedBy = username,
                    CreatedAt = DateTime.Now,
                    OwnerId = userId,
                    IsLikedSong = true,
                    IsDeleted = false,
                    Avatar = "https://i1.sndcdn.com/artworks-4Lu85Xrs7UjJ4wVq-vuI2zg-t500x500.jpg"
                };

                await _playlistDAO.AddPlaylistAsync(likedSongsPlaylist);
            }

            return likedSongsPlaylist;
        }

        /// <summary>
        /// Shares a playlist with a user asynchronously.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to share.</param>
        /// <param name="userId">The ID of the user to share the playlist with.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SharePlaylistAsync(string playlistId, string userId)
        {
            var playlist = await _playlistDAO.GetPlaylistByIdAsync(playlistId);

            if (playlist.ShareWithUsers == null)
                playlist.ShareWithUsers = new List<string>();

            if (!playlist.ShareWithUsers.Contains(userId))
            {
                playlist.ShareWithUsers.Add(userId);
                await _playlistDAO.UpdatePlaylistAsync(playlistId, playlist);
            }
        }

        /// <summary>
        /// Gets the playlists shared with a user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of playlists shared with the user.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the userId is null or empty.</exception>
        public async Task<List<PlaylistDTO>> GetSharedPlaylistsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            // Get all playlists
            var allPlaylists = await _playlistDAO.GetPlaylistsAsync();

            // Filter playlists shared with the user
            var sharedPlaylists = allPlaylists.Where(p => p.ShareWithUsers != null && p.ShareWithUsers.Contains(userId)).ToList();

            return sharedPlaylists;
        }
    }
}
