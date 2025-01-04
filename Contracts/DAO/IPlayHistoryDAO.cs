// IPlayHistoryDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Play History Data Access Object
/// </summary>
public interface IPlayHistoryDAO : IDAO
{
    /// <summary>
    /// Retrieves the play history with song details for a specific user.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlayHistoryWithSongDTO.</returns>
    Task<List<PlayHistoryWithSongDTO>> GetUserHistoryWithSongAsync(string userID);

    /// <summary>
    /// Inserts a new play history record.
    /// </summary>
    /// <param name="playHistory">The play history record to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertPlayHistoryAsync(PlayHistoryDTO playHistory);
}

