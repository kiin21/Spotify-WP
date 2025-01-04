using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Playlist Data Access Object
/// </summary>
public interface IPlaylistDAO : IDAO
{
    /// <summary>
    /// Retrieves all playlists.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistDTO.</returns>
    Task<List<PlaylistDTO>> GetPlaylistsAsync();

    /// <summary>
    /// Retrieves a playlist by its ID.
    /// </summary>
    /// <param name="id">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a PlaylistDTO object.</returns>
    Task<PlaylistDTO> GetPlaylistByIdAsync(string id);

    /// <summary>
    /// Retrieves the liked songs playlist for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a PlaylistDTO object.</returns>
    Task<PlaylistDTO> GetLikedSongsPlaylistAsync(string userId);

    /// <summary>
    /// Adds a new playlist.
    /// </summary>
    /// <param name="playlist">The playlist to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddPlaylistAsync(PlaylistDTO playlist);

    /// <summary>
    /// Updates the status of a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="isDeleted">The new status of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdatePlaylistStatusAsync(string playlistId, bool isDeleted);

    /// <summary>
    /// Removes a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemovePlaylistAsync(string playlistId);

    /// <summary>
    /// Retrieves playlists by user ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistDTO.</returns>
    Task<List<PlaylistDTO>> GetPlaylistsByUserIdAsync(string userId);

    /// <summary>
    /// Updates an existing playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="updatedPlaylist">The updated playlist data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdatePlaylistAsync(string playlistId, PlaylistDTO updatedPlaylist);
}

