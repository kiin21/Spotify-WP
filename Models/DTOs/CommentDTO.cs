// Models/CommentDTO.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Spotify.Models.DTOs
{
    public class CommentDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 

        [BsonElement("song_id")]
        public string SongId { get; set; } // ID của bài hát

        [BsonElement("added_at")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; // Thời gian thêm bình luận

        [BsonElement("added_by")]
        public string AddedBy { get; set; }   // Người thêm bình luận

        [BsonElement("content")]
        public string Content { get; set; } // Nội dung bình luận
    }
}
