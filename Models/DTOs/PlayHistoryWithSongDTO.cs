using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs;

public class PlayHistoryWithSongDTO
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("user_id")] // Matches the MongoDB field name
    public string UserID { get; set; }

    [BsonElement("song_id")] // Matches the MongoDB field name
    public string SongID { get; set; }

    [BsonElement("played_at")] // Matches the MongoDB field name
    public DateTime PlayedAt { get; set; }

    [BsonElement("songDetails")] // Matches the MongoDB field name for the joined data
    public SongDTO SongDetails { get; set; }
}
