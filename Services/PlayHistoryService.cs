using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing play history in the Spotify application.
/// </summary>
public class PlayHistoryService
{
    private readonly IPlayHistoryDAO _playHistoryDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayHistoryService"/> class with the specified DAO.
    /// </summary>
    /// <param name="playHistoryDAO">The data access object for play history operations.</param>
    public PlayHistoryService(IPlayHistoryDAO playHistoryDAO)
    {
        _playHistoryDAO = playHistoryDAO ?? throw new ArgumentNullException(nameof(playHistoryDAO));
    }

    /// <summary>
    /// Saves a play history record for a user.
    /// </summary>
    /// <param name="userID">The ID of the user who played the song.</param>
    /// <param name="songID">The ID of the song that was played.</param>
    /// <param name="playedAt">The date and time when the song was played.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SavePlayHistoryAsync(string userID, string songID, DateTime playedAt, TimeSpan totalTime)
    {
        if (string.IsNullOrEmpty(userID))
            throw new ArgumentNullException(nameof(userID), "User ID cannot be null or empty.");
        if (string.IsNullOrEmpty(songID))
            throw new ArgumentNullException(nameof(songID), "Song ID cannot be null or empty.");

        var playHistory = new PlayHistoryDTO
        {
            UserID = userID,
            SongID = songID,
            PlayedAt = playedAt,
            TotalTime = totalTime
        };

        // Call the DAO to save the play history
        await _playHistoryDAO.InsertPlayHistoryAsync(playHistory);
    }

    /// <summary>
    /// Retrieves the play history for a specific user, including song details.
    /// </summary>
    /// <param name="userID">The ID of the user whose play history is to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of play history records with song details.</returns>
    public async Task<List<PlayHistoryWithSongDTO>> GetUserHistoryAsync(string userID)
    {
        if (string.IsNullOrEmpty(userID))
            throw new ArgumentNullException(nameof(userID), "User ID cannot be null or empty.");

        return await _playHistoryDAO.GetUserHistoryWithSongAsync(userID);
    }
}
