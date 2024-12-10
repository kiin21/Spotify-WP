using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

public class PlayHistoryService
{
    private readonly IPlayHistoryDAO _playHistoryDAO;

    public PlayHistoryService(IPlayHistoryDAO playHistoryDAO)
    {
        _playHistoryDAO = playHistoryDAO;
    }

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

    public async Task<List<PlayHistoryWithSongDTO>> GetUserHistoryAsync(string userID)
    {
        return await _playHistoryDAO.GetUserHistoryWithSongAsync(userID);
    }
}