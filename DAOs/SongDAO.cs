using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class SongDAO : BaseDAO, ISongDAO
{
    private readonly IMongoCollection<SongDTO> _songs;

    public SongDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _songs = database.GetCollection<SongDTO>("Songs");
    }

    public async Task<List<SongDTO>> SearchSongs(string query)
    {
        var filter = Builders<SongDTO>.Filter.Regex("title", new BsonRegularExpression(query, "i"));
        var songs = await _songs.Find(filter).ToListAsync();
        return songs;
    }

    public async Task<List<SongDTO>> GetAllSongs()
    {
        var songs = await _songs.Find(new BsonDocument()).ToListAsync();
        return songs; // Duration will be automatically converted via the property
    }


    public async Task<SongDTO> GetSongByIdAsync(string songId)
    {
        var objectId = ObjectId.Parse(songId);
        var song = await _songs.Find(s => s.Id == objectId).FirstOrDefaultAsync();
        return song;
    }

}