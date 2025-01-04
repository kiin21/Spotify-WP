// UserDAO.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.DAOs;

/// <summary>
/// Data Access Object for Users
/// </summary>
public class UserDAO : BaseDAO, IUserDAO
{
    private readonly IMongoCollection<UserDTO> _userCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDAO"/> class.
    /// </summary>
    public UserDAO()
    {
        // Initialize the MongoDB collection
        var database = connection.GetDatabase("SpotifineDB");
        _userCollection = database.GetCollection<UserDTO>("User");
    }

    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddUserAsync(UserDTO user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        await _userCollection.InsertOneAsync(user);
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UserDTO object.</returns>
    public async Task<UserDTO> GetUserByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        var filter = Builders<UserDTO>.Filter.Eq(u => u.Id, id);
        return await _userCollection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="updatedUser">The updated user data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateUserAsync(string id, UserDTO updatedUser)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        if (updatedUser == null)
            throw new ArgumentNullException(nameof(updatedUser), "Updated user data cannot be null");

        var filter = Builders<UserDTO>.Filter.Eq(u => u.Id, id);
        var update = Builders<UserDTO>.Update
            .Set(u => u.Username, updatedUser.Username)
            .Set(u => u.HashedPassword, updatedUser.HashedPassword)
            .Set(u => u.Salt, updatedUser.Salt)
            .Set(u => u.IsPremium, updatedUser.IsPremium)
            .Set(u => u.FollowArtist, updatedUser.FollowArtist)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        await _userCollection.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        var filter = Builders<UserDTO>.Filter.Eq(u => u.Id, id);
        await _userCollection.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UserDTO object.</returns>
    public async Task<UserDTO> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        var filter = Builders<UserDTO>.Filter.Eq(u => u.Username, username);
        return await _userCollection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of UserDTO.</returns>
    public Task<List<UserDTO>> GetUsersAsync()
    {
        return _userCollection.Find(_ => true).ToListAsync();
    }
}


