using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs;

public class PlayHistoryWithSongDTO
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("user_id")] 
    public string UserID { get; set; }

    [BsonElement("song_id")] 
    public string SongID { get; set; }

    [BsonElement("played_at")] 
    public DateTime PlayedAt { get; set; }

    [BsonElement("songDetails")]
    public SongDTO SongDetails { get; set; }
}
