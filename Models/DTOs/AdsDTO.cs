using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Spotify.Models.DTOs;

/// <summary>
/// Data Transfer Object for Ads.
/// </summary>
public class AdsDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the ad.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the ad.
    /// </summary>
    public string title { get; set; }

    /// <summary>
    /// Gets or sets the duration of the ad in seconds.
    /// </summary>
    [BsonElement("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// Gets or sets the URL of the ad's audio.
    /// </summary>
    public string audio_url { get; set; }

    /// <summary>
    /// Gets or sets the URL of the ad's cover art.
    /// </summary>
    [BsonElement("coverArt_url")]
    public string CoverArtUrl { get; set; }

    /// <summary>
    /// Gets or sets the name of the artist featured in the ad.
    /// </summary>
    [BsonElement("artist_name")]
    public string ArtistName { get; set; }

    /// <summary>
    /// Gets or sets the release date of the ad.
    /// </summary>
    [BsonElement("release_date")]
    public string ReleaseDate { get; set; }
}
