// ISongDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Song Data Access Object
/// </summary>
public interface ISongDAO : IDAO
{
    /// <summary>
    /// Searches for songs based on a query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of SongDTO.</returns>
    Task<List<SongDTO>> SearchSongs(string query);

    /// <summary>
    /// Retrieves all songs.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of SongDTO.</returns>
    Task<List<SongDTO>> GetAllSongs();

    /// <summary>
    /// Retrieves a song by its ID.
    /// </summary>
    /// <param name="songId">The ID of the song.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a SongDTO object.</returns>
    Task<SongDTO> GetSongByIdAsync(string songId);
}


