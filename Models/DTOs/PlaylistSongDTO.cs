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
/// Data Transfer Object for Playlist Songs.
/// </summary>
public class PlaylistSongDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the unique identifier for the playlist song.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the playlist.
    /// </summary>
    [BsonElement("playlist_id")]
    public string PlaylistId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the song was added to the playlist.
    /// </summary>
    [BsonElement("added_at")]
    public DateTime AddedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who added the song to the playlist.
    /// </summary>
    [BsonElement("added_by")]
    public string AddedBy { get; set; }

    /// <summary>
    /// Gets or sets the URL of the song's avatar.
    /// </summary>
    [BsonElement("avatar")]
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the ID of the song.
    /// </summary>
    [BsonElement("song_id")]
    public string SongId { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}


