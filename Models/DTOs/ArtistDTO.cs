// ArtistDTO.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Artists.
/// </summary>
public class ArtistDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the artist.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the artist.
    /// </summary>
    [BsonElement("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the biography of the artist.
    /// </summary>
    [BsonElement("biography")]
    public string Biography { get; set; }

    /// <summary>
    /// Gets or sets the URL of the artist's avatar.
    /// </summary>
    [BsonElement("avatar_url")]
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the country of the artist.
    /// </summary>
    [BsonElement("country")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the debut date of the artist.
    /// </summary>
    [BsonElement("debut_date")]
    public DateTime DebutDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the artist was created.
    /// </summary>
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of song IDs associated with the artist.
    /// </summary>
    [BsonElement("song_ids")]
    public List<string> SongIds { get; set; } = new List<string>();
}