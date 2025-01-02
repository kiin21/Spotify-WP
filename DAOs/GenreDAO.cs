
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class GenreDAO : BaseDAO, IGenreDAO
{
    private readonly IMongoCollection<GenreDAO> _songs;

    public GenreDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _songs = database.GetCollection<GenreDAO>("Genres");
    }
}