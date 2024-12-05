using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System.Diagnostics;

namespace Spotify.DAOs;

public class PlayHistoryDAO : BaseDAO, IPlayHistoryDAO
{
    private readonly IMongoCollection<PlayHistoryDTO> _playHistory;

    public PlayHistoryDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playHistory = database.GetCollection<PlayHistoryDTO>("PlayHistory");
    }

    public async Task<List<PlayHistoryWithSongDTO>> GetUserHistoryWithSongAsync(string userID)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("user_id", userID)),
            new BsonDocument("$addFields", new BsonDocument("song_id", new BsonDocument("$toObjectId", "$song_id"))),
            new BsonDocument("$lookup", new BsonDocument{
                                                            { "from", "Songs" },
                                                            { "localField", "song_id" },
                                                            { "foreignField", "_id" },
                                                            { "as", "songDetails" }
                                                        }),
            new BsonDocument("$unwind", new BsonDocument("path", "$songDetails")),
            new BsonDocument("$sort", new BsonDocument("played_at", -1)),
            new BsonDocument("$addFields", new BsonDocument("song_id", new BsonDocument("$toString", "$song_id"))),
        };

        var result = await _playHistory.Aggregate<PlayHistoryWithSongDTO>(pipeline).ToListAsync();
        return result;
    }

        //var another = await _playHistory.Find(new BsonDocument()).ToListAsync();
    public async Task InsertPlayHistoryAsync(PlayHistoryDTO playHistory)
    {
        await _playHistory.InsertOneAsync(playHistory);
    }
}


