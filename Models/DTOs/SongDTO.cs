//SongDTO.cs
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs
{
    public class SongDTO
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Audio_url { get; set; }
        public string CoverArt_url { get; set; } = "ms-appx:///Assets/OMG_NewJeans.jpg";
        public string Artist { get; set; }
        public string Album { get; set; }

        // This property will be used for deserialization
        [BsonElement("Duration")]
        public Decimal128 DurationDecimal { get; set; }

        // This will be calculated from DurationDecimal
        [BsonIgnore]
        public TimeSpan Duration
        {
            get => TimeSpan.FromMinutes((double)DurationDecimal);
        }

        public string FormattedDuration => $"{(int)Duration.TotalMinutes}:{Duration.Seconds:D2}"; // Format as "MM:SS"

        public string Genre { get; set; }

        // Optional fields depending on what your app needs:
        public DateTime ReleaseDate { get; set; }
        public string FilePath { get; set; } // If the song is stored locally
 
    }
}
