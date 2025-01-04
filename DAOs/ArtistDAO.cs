// ArtistDAO.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

/// <summary>
/// Data Access Object for Artists
/// </summary>
public class ArtistDAO : BaseDAO, IArtistDAO
{
    private readonly IMongoCollection<ArtistDTO> _artists;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistDAO"/> class.
    /// </summary>
    public ArtistDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _artists = database.GetCollection<ArtistDTO>("Artist");
    }

    /// <summary>
    /// Retrieves all artists.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of ArtistDTO.</returns>
    public async Task<List<ArtistDTO>> GetAllArtistsAsync()
    {
        return await _artists.Find(new BsonDocument()).ToListAsync();
    }

    /// <summary>
    /// Retrieves an artist by their ID.
    /// </summary>
    /// <param name="id">The ID of the artist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an ArtistDTO object.</returns>
    public async Task<ArtistDTO> GetArtistByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        return await _artists.Find(a => a.Id == objectId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves an artist by their name.
    /// </summary>
    /// <param name="name">The name of the artist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an ArtistDTO object.</returns>
    public async Task<ArtistDTO> GetArtistByNameAsync(string name)
    {
        return await _artists.Find(a => a.Name == name).FirstOrDefaultAsync();
    }
}
