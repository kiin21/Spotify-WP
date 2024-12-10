using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Service class for managing play history operations.
/// </summary>
public class PlayHistoryService
{
    private readonly IPlayHistoryDAO _playHistoryDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayHistoryService"/> class.
    /// </summary>
    /// <param name="playHistoryDAO">The data access object for play history.</param>
    public PlayHistoryService(IPlayHistoryDAO playHistoryDAO)
    {
        _playHistoryDAO = playHistoryDAO;
    }

    /// <summary>
    /// Saves the play history asynchronously.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="songID">The ID of the song.</param>
    /// <param name="playedAt">The date and time when the song was played.</param>
    public async Task SavePlayHistoryAsync(string userID, string songID, DateTime playedAt)
    {
        var playHistory = new PlayHistoryDTO
        {
            UserID = userID,
            SongID = songID,
            PlayedAt = playedAt
        };

        // Call the DAO to save the play history
        await _playHistoryDAO.InsertPlayHistoryAsync(playHistory);
    }

    /// <summary>
    /// Retrieves the play history for a user asynchronously.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="PlayHistoryWithSongDTO"/>.</returns>
    public async Task<List<PlayHistoryWithSongDTO>> GetUserHistoryAsync(string userID)
    {
        return await _playHistoryDAO.GetUserHistoryWithSongAsync(userID);
    }
}
