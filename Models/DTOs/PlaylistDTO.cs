using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class PlaylistDTO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("avatar")]
        public string Avatar { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("is_public")]
        public bool IsPublic { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("created_by")]
        public string CreatedBy { get; set; }
    }
}