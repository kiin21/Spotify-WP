// BaseDAO.cs
using System;
using MongoDB.Driver;
using Spotify.Contracts.DAO;

namespace Spotify.DAOs;

public class BaseDAO : IDAO
{
    static readonly string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    public MongoClient connection = new MongoClient(connectionString);
}
