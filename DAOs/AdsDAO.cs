using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spotify.DAOs;

/// <summary>
/// Data Access Object for Ads
/// </summary>
public class AdsDAO : BaseDAO, IAdsDAO
{
    private readonly IMongoCollection<AdsDTO> _ads;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdsDAO"/> class.
    /// </summary>
    public AdsDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _ads = database.GetCollection<AdsDTO>("Ads");
    }

    /// <summary>
    /// Retrieves all ads.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of AdsDTO.</returns>
    public async Task<List<AdsDTO>> GetAllAds()
    {
        var ads = await _ads.Find(new BsonDocument()).ToListAsync();
        return ads;
    }

    /// <summary>
    /// Retrieves a random ad.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an AdsDTO object.</returns>
    public async Task<AdsDTO> GetRandomAds()
    {
        var ads = await GetAllAds();

        if (ads == null || ads.Count == 0)
        {
            return null;
        }

        var random = new Random();
        int index = random.Next(ads.Count);

        return ads[index];
    }
}