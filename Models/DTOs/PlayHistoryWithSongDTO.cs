using System;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Play History with Song Details.
/// </summary>
public class PlayHistoryWithSongDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the unique identifier for the play history record.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who played the song.
    /// </summary>
    [BsonElement("user_id")]
    public string UserID { get; set; }

    /// <summary>
    /// Gets or sets the ID of the song that was played.
    /// </summary>
    [BsonElement("song_id")]
    public string SongID { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the song was played.
    /// </summary>
    [BsonElement("played_at")]
    public DateTime PlayedAt { get; set; }

    /// <summary>
    /// Gets or sets the total time the song was played.
    /// </summary>
    [BsonElement("total_time")]
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// Gets or sets the details of the song that was played.
    /// </summary>
    [BsonElement("songDetails")]
    public SongDTO SongDetails { get; set; }

    /// <summary>
    /// Gets or sets the details of the genre of the song that was played.
    /// </summary>
    [BsonElement("genreDetails")]
    public GenreDTO Genre { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}


