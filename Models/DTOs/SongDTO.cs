//SongDTO.cs
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs
{
    public class SongDTO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string title { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; }

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

        public string audio_url { get; set; }

        [BsonElement("coverArt_url")]
        public string CoverArtUrl { get; set; }

        public string syncedLyrics { get; set; }  // Changed to string

        public string plainLyrics { get; set; }

        [BsonElement("artist_name")]
        public string ArtistName { get; set; }

        [BsonElement("release_date")]
        public string ReleaseDate { get; set; }

        // Optional: Add helper method to parse synced lyrics if needed
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
    }
}