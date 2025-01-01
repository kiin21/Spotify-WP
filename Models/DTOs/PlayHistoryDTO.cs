using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Spotify.Models.DTOs;

public class PlayHistoryDTO
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string Id { get; set; }

    [BsonElement("user_id")]
    public string UserID { get; set; }

    [BsonElement("song_id")]
    public string SongID { get; set; }

    [BsonElement("played_at")]
    public DateTime PlayedAt { get; set; }

    [BsonElement("total_time")]
    public TimeSpan TotalTime { get; set; }
}

