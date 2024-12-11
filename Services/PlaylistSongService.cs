using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Services
{
    /// <summary>
    /// Provides services for managing songs in playlists.
    /// </summary>
    public class PlaylistSongService
    {
        private readonly IPlaylistSongDAO _playlistSongDAO;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistSongService"/> class.
        /// </summary>
        /// <param name="playlistSongDAO">The data access object for playlist songs.</param>
        public PlaylistSongService(IPlaylistSongDAO playlistSongDAO)
        {
            _playlistSongDAO = playlistSongDAO;
        }

        /// <summary>
        /// Gets the songs for a specified playlist asynchronously.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of songs in the playlist.</returns>
        public async Task<List<PlaylistSongDTO>> GetSongsForPlaylistAsync(string playlistId)
        {
            return await _playlistSongDAO.GetSongsByPlaylistIdAsync(playlistId);
        }
    }
}
