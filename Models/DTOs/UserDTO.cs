// Models/UserDTO.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Users.
/// </summary>
public class UserDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // MongoDB ID, auto-generated

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    [BsonElement("username")]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the avatar URL of the user.
    /// </summary>
    [BsonElement("userAvatar")]
    public string UserAvatar { get; set; } = "../../Assets/defaultAvt.jpg";

    /// <summary>
    /// Gets or sets the hashed password of the user.
    /// </summary>
    [BsonElement("hashedPassword")]
    public string HashedPassword { get; set; }

    /// <summary>
    /// Gets or sets the salt used for hashing the password.
    /// </summary>
    [BsonElement("salt")]
    public string Salt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is a premium user.
    /// </summary>
    [BsonElement("isPremium")]
    public bool IsPremium { get; set; } = false;

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the user was last updated.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of artist IDs that the user follows.
    /// </summary>
    [BsonElement("followArtist")]
    public List<string> FollowArtist { get; set; } = new List<string>();
}



