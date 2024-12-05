using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.DAOs;

public class PlayHistoryDAO : BaseDAO, IPlayHistoryDAO
{
    private readonly IMongoCollection<PlayHistoryDTO> _playHistory;
    public PlayHistoryDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playHistory = database.GetCollection<PlayHistoryDTO>("PlayHistory");
    }
    public async Task<List<PlayHistoryDTO>> GetUserHistoryAsync(string userID)
    {
        // Find documents where UserID matches and sort by PlayedAt in descending order
        return await _playHistory
            .Find(playHistory => playHistory.UserID == userID)
            .SortByDescending(playHistory => playHistory.PlayedAt)
            .ToListAsync();
    }
}

