using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Spotify.DAOs
{
    public class AdsDAO : BaseDAO, IAdsDAO
    {

        private readonly IMongoCollection<AdsDTO> _ads;

        public AdsDAO()
        {
            var database = connection.GetDatabase("SpotifineDB");
            _ads = database.GetCollection<AdsDTO>("Ads");
        }

        public async Task<List<AdsDTO>> GetAllAds()
        {

            var ads = await _ads.Find(new BsonDocument()).ToListAsync();
            return ads;
        }

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
}
