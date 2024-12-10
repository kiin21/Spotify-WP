// UserDAO.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.DAOs
{
    public class UserDAO : BaseDAO, IUserDAO
    {
        private readonly IMongoCollection<UserDTO> _userCollection;

        public UserDAO()
        {
            // Initialize the MongoDB collection
            var database = connection.GetDatabase("SpotifineDB"); 
            _userCollection = database.GetCollection<UserDTO>("User");
        }

        // Method to add a new user
        public async Task AddUserAsync(UserDTO user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            await _userCollection.InsertOneAsync(user);
        }

        // Method to get a user by ID
        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty", nameof(id));

            var filter = Builders<UserDTO>.Filter.Eq(u => u.Id, id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Method to update user information by ID
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

        // Method to delete a user by ID
        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty", nameof(id));

            var filter = Builders<UserDTO>.Filter.Eq(u => u.Id, id);
            await _userCollection.DeleteOneAsync(filter);
        }

        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            var filter = Builders<UserDTO>.Filter.Eq(u => u.Username, username);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<List<UserDTO>> GetUsersAsync()
        {
            return _userCollection.Find(_ => true).ToListAsync();
        }
    }
}
