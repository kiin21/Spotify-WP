using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Genres.
/// </summary>
public class GenreDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the genre.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the genre.
    /// </summary>
    [BsonElement("name")]
    public string name { get; set; }
}

