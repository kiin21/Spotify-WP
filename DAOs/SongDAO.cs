using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.DAOs;
/// <summary>
/// Data Access Object for Songs
/// </summary>
public class SongDAO : BaseDAO, ISongDAO
{
    private readonly IMongoCollection<SongDTO> _songs;

    /// <summary>
    /// Initializes a new instance of the <see cref="SongDAO"/> class.
    /// </summary>
    public SongDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _songs = database.GetCollection<SongDTO>("Songs");
    }

    /// <summary>
    /// Searches for songs based on a query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of SongDTO.</returns>
    public async Task<List<SongDTO>> SearchSongs(string query)
    {
        var filter = Builders<SongDTO>.Filter.Regex("title", new BsonRegularExpression(query, "i"));
        var songs = await _songs.Find(filter).ToListAsync();
        return songs;
    }

    /// <summary>
    /// Retrieves all songs.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of SongDTO.</returns>
    public async Task<List<SongDTO>> GetAllSongs()
    {
        var songs = await _songs.Find(new BsonDocument()).ToListAsync();
        return songs; // Duration will be automatically converted via the property
    }

    /// <summary>
    /// Retrieves a song by its ID.
    /// </summary>
    /// <param name="songId">The ID of the song.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a SongDTO object.</returns>
    public async Task<SongDTO> GetSongByIdAsync(string songId)
    {
        var objectId = ObjectId.Parse(songId);
        var song = await _songs.Find(s => s.Id == objectId).FirstOrDefaultAsync();
        return song;
    }
}

