using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs
{
    public class AdsDTO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string title { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; }

        public string audio_url { get; set; }

        [BsonElement("coverArt_url")]
        public string CoverArtUrl { get; set; }

        [BsonElement("artist_name")]
        public string ArtistName { get; set; }

        [BsonElement("release_date")]
        public string ReleaseDate { get; set; }
    }
}
