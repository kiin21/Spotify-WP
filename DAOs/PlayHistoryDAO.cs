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

/// <summary>
/// Data Access Object for Play History
/// </summary>
public class PlayHistoryDAO : BaseDAO, IPlayHistoryDAO
{
    private readonly IMongoCollection<PlayHistoryDTO> _playHistory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayHistoryDAO"/> class.
    /// </summary>
    public PlayHistoryDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playHistory = database.GetCollection<PlayHistoryDTO>("PlayHistory");
    }

    /// <summary>
    /// Retrieves the play history with song details for a specific user.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlayHistoryWithSongDTO.</returns>
    public async Task<List<PlayHistoryWithSongDTO>> GetUserHistoryWithSongAsync(string userID)
    {
        var pipeline = new[]
        {
            // Match play history records for the specific user
            new BsonDocument("$match", new BsonDocument("user_id", userID)),
        
            // Convert string song_id to ObjectId for lookup
            new BsonDocument("$addFields", new BsonDocument("song_id",
                new BsonDocument("$toObjectId", "$song_id"))),
        
            // Lookup song details
            new BsonDocument("$lookup", new BsonDocument{
                { "from", "Songs" },
                { "localField", "song_id" },
                { "foreignField", "_id" },
                { "as", "songDetails" }
            }),
        
            // Unwind the song details array
            new BsonDocument("$unwind", new BsonDocument("path", "$songDetails")),
        
            // Add field to convert genre ID to ObjectId for next lookup
            new BsonDocument("$addFields", new BsonDocument(
                "genreObjectId", new BsonDocument("$toObjectId", "$songDetails.genreId"))),
        
            // Lookup genre details
            new BsonDocument("$lookup", new BsonDocument{
                { "from", "Genres" },
                { "localField", "genreObjectId" },
                { "foreignField", "_id" },
                { "as", "genreDetails" }
            }),
        
            // Unwind the genre details array
            new BsonDocument("$unwind", new BsonDocument("path", "$genreDetails")),
        
            // Sort by played_at in descending order
            new BsonDocument("$sort", new BsonDocument("played_at", -1)),
        
            // Convert ObjectId back to string for the response
            new BsonDocument("$addFields", new BsonDocument("song_id",
                new BsonDocument("$toString", "$song_id"))),
        
            // Project the final shape of the documents
            new BsonDocument("$project", new BsonDocument{
                { "_id", 1 },
                { "user_id", 1 },
                { "song_id", 1 },
                { "played_at", 1 },
                { "total_time", 1 },
                { "songDetails", 1 },
                { "genreDetails", 1 }
            })
        };

        var result = await _playHistory.Aggregate<PlayHistoryWithSongDTO>(pipeline).ToListAsync();
        return result;
    }

    /// <summary>
    /// Inserts a new play history record.
    /// </summary>
    /// <param name="playHistory">The play history record to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
            if (playHistory.TotalTime > TimeSpan.Zero)
            {
                await _playHistory.InsertOneAsync(playHistory);
            }
        }
        else
        {
            existingPlayHistory[0].TotalTime += playHistory.TotalTime;
            await _playHistory.UpdateOneAsync(
                Builders<PlayHistoryDTO>.Filter.Eq("user_id", playHistory.UserID) &
                Builders<PlayHistoryDTO>.Filter.Eq("song_id", playHistory.SongID) &
                Builders<PlayHistoryDTO>.Filter.Eq("played_at", playHistory.PlayedAt.Date),
                Builders<PlayHistoryDTO>.Update.Set("total_time", existingPlayHistory[0].TotalTime)
            );
        }
    }
}
