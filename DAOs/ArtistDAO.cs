// ArtistDAO.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class ArtistDAO : BaseDAO, IArtistDAO
{
    private readonly IMongoCollection<ArtistDTO> _artists;

    public ArtistDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _artists = database.GetCollection<ArtistDTO>("Artist");
    }

    public async Task<List<ArtistDTO>> GetAllArtistsAsync()
    {
        return await _artists.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<ArtistDTO> GetArtistByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        return await _artists.Find(a => a.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<ArtistDTO> GetArtistByNameAsync(string name)
    {
        return await _artists.Find(a => a.Name == name).FirstOrDefaultAsync();
    }
}