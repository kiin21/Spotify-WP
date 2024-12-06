using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System.Diagnostics;
using System.Linq;

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

    public async Task InsertPlayHistoryAsync(PlayHistoryDTO playHistory)
    {
        // Define the aggregation pipeline to check if the song has been played today
        var pipeline = new[]
        {
                new BsonDocument("$match", new BsonDocument
                {
                    { "user_id", playHistory.UserID },
                    { "song_id", playHistory.SongID },
                    { "played_at", new BsonDocument
                        {
                            { "$gte", playHistory.PlayedAt.Date },
                            { "$lt", playHistory.PlayedAt.Date.AddDays(1) }
                        }
                    }
                })
            };

        var existingPlayHistory = await _playHistory.Aggregate<PlayHistoryDTO>(pipeline).ToListAsync();

        if (!existingPlayHistory.Any())
        {
            // Insert the new play history record
            await _playHistory.InsertOneAsync(playHistory);
        }
    }
}


