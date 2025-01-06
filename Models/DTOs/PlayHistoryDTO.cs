using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Play History.
/// </summary>
public class PlayHistoryDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the play history record.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who played the song.
    /// </summary>
    [BsonElement("user_id")]
    public string UserID { get; set; }

    /// <summary>
    /// Gets or sets the ID of the song that was played.
    /// </summary>
    [BsonElement("song_id")]
    public string SongID { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the song was played.
    /// </summary>
    [BsonElement("played_at")]
    public DateTime PlayedAt { get; set; }

    /// <summary>
    /// Gets or sets the total time the song was played.
    /// </summary>
    [BsonElement("total_time")]
    public TimeSpan TotalTime { get; set; }
}


