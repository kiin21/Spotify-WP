//MockSongDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.DAO
{
    public class MockSongDAO : BaseDAO, ISongDAO
    {
        private readonly IMongoCollection<SongDTO> _songsCollection;

        public MockSongDAO()
        {
            var database = connection.GetDatabase("SpotifineDB");
            _songsCollection = database.GetCollection<SongDTO>("Songs");
        }

        public async Task<List<SongDTO>> SearchSongs(string query)
        {
            var filter = Builders<SongDTO>.Filter.Regex("Title", new BsonRegularExpression(query, "i"));
            var songs = await _songsCollection.Find(filter).ToListAsync();
            return songs;
        }

        public async Task<List<SongDTO>> GetAllSongs()
        {
            var songs = await _songsCollection.Find(new BsonDocument()).ToListAsync();
            return songs; // Duration will be automatically converted via the property
        }
    }
}
