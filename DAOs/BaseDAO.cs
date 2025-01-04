// BaseDAO.cs
using System;
using MongoDB.Driver;
using Spotify.Contracts.DAO;

namespace Spotify.DAOs;

/// <summary>
/// Base Data Access Object class providing MongoDB connection.
/// </summary>
public class BaseDAO : IDAO
{
    /// <summary>
    /// The connection string for MongoDB.
    /// </summary>
    static readonly string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");

    /// <summary>
    /// The MongoDB client connection.
    /// </summary>
    public MongoClient connection = new MongoClient(connectionString);
}


