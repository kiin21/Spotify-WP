using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Playlists.
/// </summary>
public class PlaylistDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistDTO"/> class.
    /// </summary>
    public PlaylistDTO() { }

    private PlaylistDTO likedSongs;

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistDTO"/> class with the specified liked songs playlist.
    /// </summary>
    /// <param name="likedSongs">The liked songs playlist.</param>
    public PlaylistDTO(PlaylistDTO likedSongs)
    {
        this.likedSongs = likedSongs;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the playlist.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // Dùng để chuyển ObjectId thành string tự động
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the avatar URL of the playlist.
    /// </summary>
    [BsonElement("avatar")]
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the title of the playlist.
    /// </summary>
    [BsonElement("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the playlist.
    /// </summary>
    [BsonElement("description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the playlist is public.
    /// </summary>
    [BsonElement("is_public")]
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the playlist was created.
    /// </summary>
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who created the playlist.
    /// </summary>
    [BsonElement("created_by")]
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the playlist is a liked songs playlist.
    /// </summary>
    [BsonElement("is_liked_song")]
    public bool IsLikedSong { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the playlist is deleted.
    /// </summary>
    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the owner ID of the playlist.
    /// </summary>
    [BsonElement("owner_id")]
    public string OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the list of users with whom the playlist is shared.
    /// </summary>
    [BsonElement("shareWithUsers")]
    public List<string> ShareWithUsers { get; set; } = new List<string>();
}
