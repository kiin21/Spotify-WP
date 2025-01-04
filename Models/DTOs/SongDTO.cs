//SongDTO.cs
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Songs.
/// </summary>
public class SongDTO : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the unique identifier for the song.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the song.
    /// </summary>
    public string title { get; set; }

    /// <summary>
    /// Gets or sets the duration of the song in seconds.
    /// </summary>
    [BsonElement("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// Gets the formatted duration of the song in "minutes:seconds" format.
    /// </summary>
    [BsonIgnore]
    public string FormattedDuration
    {
        get
        {
            int minutes = Duration / 60;
            int seconds = Duration % 60;
            return $"{minutes}:{seconds:D2}";
        }
    }

    /// <summary>
    /// Gets or sets the URL of the song's audio.
    /// </summary>
    public string audio_url { get; set; }

    /// <summary>
    /// Gets or sets the URL of the song's cover art.
    /// </summary>
    [BsonElement("coverArt_url")]
    public string CoverArtUrl { get; set; }

    /// <summary>
    /// Gets or sets the synced lyrics of the song.
    /// </summary>
    public string syncedLyrics { get; set; }

    /// <summary>
    /// Gets or sets the plain lyrics of the song.
    /// </summary>
    public string plainLyrics { get; set; }

    /// <summary>
    /// Gets or sets the name of the artist of the song.
    /// </summary>
    [BsonElement("artist_name")]
    public string ArtistName { get; set; }

    /// <summary>
    /// Gets or sets the release date of the song.
    /// </summary>
    [BsonElement("release_date")]
    public string ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the genre ID of the song.
    /// </summary>
    [BsonElement("genreId")]
    public string GenreId { get; set; }

    /// <summary>
    /// Gets the parsed synced lyrics of the song.
    /// </summary>
    [BsonIgnore]
    public List<(TimeSpan Timestamp, string Text)> ParsedSyncedLyrics
    {
        get
        {
            if (string.IsNullOrEmpty(syncedLyrics))
                return new List<(TimeSpan, string)>();

            var result = new List<(TimeSpan, string)>();
            var lines = syncedLyrics.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("[") && line.Contains("]"))
                {
                    var timeStr = line.Substring(1, line.IndexOf("]") - 1);
                    var text = line.Substring(line.IndexOf("]") + 1).Trim();

                    if (TimeSpan.TryParseExact(timeStr, @"mm\:ss\.ff", null, out TimeSpan timestamp))
                    {
                        result.Add((timestamp, text));
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Gets or sets the index of the song in a playlist.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the song is the current song.
    /// </summary>
    public bool IsCurrentSong { get; set; } = false;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}

