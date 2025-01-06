// Models/CommentDTO.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Comments.
/// </summary>
public class CommentDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the unique identifier for the comment.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the song associated with the comment.
    /// </summary>
    [BsonElement("song_id")]
    public string SongId { get; set; } // ID của bài hát

    /// <summary>
    /// Gets or sets the date and time when the comment was added.
    /// </summary>
    [BsonElement("added_at")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow; // Thời gian thêm bình luận

    /// <summary>
    /// Gets or sets the user who added the comment.
    /// </summary>
    [BsonElement("added_by")]
    public string AddedBy { get; set; }   // Người thêm bình luận

    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    [BsonElement("content")]
    public string Content { get; set; } // Nội dung bình luận

    public event PropertyChangedEventHandler PropertyChanged;
}
