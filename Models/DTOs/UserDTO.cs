// Models/UserDTO.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Spotify.Models.DTOs
{
    public class UserDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB ID, auto-generated

        [BsonElement("username")]
        public string Username { get; set; }       
        [BsonElement("userAvatar")]
        public string UserAvatar { get; set; }

        [BsonElement("hashedPassword")]
        public string HashedPassword { get; set; }

        [BsonElement("salt")]
        public string Salt { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
