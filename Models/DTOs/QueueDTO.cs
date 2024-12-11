using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Spotify.Models.DTOs;

public class QueueDTO
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; }

    [BsonElement("song_ids")]
    public List<string> SongIds { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}