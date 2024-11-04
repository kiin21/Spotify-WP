using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Models.DTOs
{
    public class PlaylistSongDTO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("playlist_id")]
        public string PlaylistId { get; set; }

        [BsonElement("position")]
        public int Position { get; set; }

        [BsonElement("added_at")]
        public DateTime AddedAt { get; set; }

        [BsonElement("added_by")]
        public string AddedBy { get; set; }

        [BsonElement("avatar")]
        public string Avatar { get; set; }
    }
}