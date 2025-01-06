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
/// Data Access Object for Genres
/// </summary>
public class GenreDAO : BaseDAO, IGenreDAO
{
    private readonly IMongoCollection<GenreDAO> _songs;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreDAO"/> class.
    /// </summary>
    public GenreDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _songs = database.GetCollection<GenreDAO>("Genres");
    }
}