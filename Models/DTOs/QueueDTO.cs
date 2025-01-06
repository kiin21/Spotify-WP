using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Queue.
/// </summary>
public class QueueDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the queue.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user associated with the queue.
    /// </summary>
    [BsonElement("user_id")]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the list of song IDs in the queue.
    /// </summary>
    [BsonElement("song_ids")]
    public List<string> SongIds { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the queue was created.
    /// </summary>
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}