// IPlaylistSongDetailDAO.cs
using Spotify.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Playlist Song Detail Data Access Object
/// </summary>
public interface IPlaylistSongDetailDAO : IDAO
{
    /// <summary>
    /// Retrieves playlist song details by playlist ID.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistSongDetailDTO.</returns>
    Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId);

    /// <summary>
    /// Deletes a song from a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="songId">The ID of the song.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteOneAsync(string playlistId, string songId);

    /// <summary>
    /// Inserts a new song into a playlist.
    /// </summary>
    /// <param name="playlistSong">The playlist song to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertOneAsync(PlaylistSongDTO playlistSong);
}


