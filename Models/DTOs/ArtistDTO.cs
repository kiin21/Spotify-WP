// ArtistDTO.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Spotify.Models.DTOs
{
    public class ArtistDTO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("biography")]
        public string Biography { get; set; }

        [BsonElement("avatar_url")]
        public string Avatar { get; set; }

        [BsonElement("country")]
        public string Country { get; set; }

        [BsonElement("debut_date")]
        public DateTime DebutDate { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("song_ids")]
        public List<string> SongIds { get; set; } = new List<string>();
    }
}
