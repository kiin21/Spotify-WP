//UserService.cs

using System;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using Spotify.DAOs;
using System.Collections.Generic;
using Spotify.Helpers;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing users.
/// </summary>
public class UserService
{
    private readonly IUserDAO _userDAO;
    public event EventHandler<string> ArtistFollowStatusChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="userDAO">The data access object for users.</param>
    /// <exception cref="ArgumentNullException">Thrown when the userDAO is null.</exception>
    public UserService(IUserDAO userDAO)
    {
        _userDAO = userDAO ?? throw new ArgumentNullException(nameof(userDAO));
    }

    /// <summary>
    /// Adds a new user asynchronously.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="password">The password of the new user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the username or password is null or empty.</exception>
    public async Task AddUserAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username), "Username cannot be null or empty");

        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");

        // Hash the password and generate the salt
        var (hashedPassword, salt) = PasswordHasher.HashPassword(password);

        var user = new UserDTO
        {
            Username = username,
            HashedPassword = hashedPassword,
            Salt = salt,
            IsPremium = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userDAO.AddUserAsync(user);
    }

    /// <summary>
    /// Gets a user by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user with the specified ID.</returns>
    /// <exception cref="ArgumentException">Thrown when the ID is null or empty.</exception>
    public async Task<UserDTO> GetUserByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _userDAO.GetUserByIdAsync(id);
    }

    /// <summary>
    /// Updates user information asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUser">The updated user information.</param>
    /// <param name="newPlainPassword">The new plain text password, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the ID is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the updatedUser is null.</exception>
    public async Task UpdateUserAsync(string id, UserDTO updatedUser, string newPlainPassword = null)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));
        if (updatedUser == null) throw new ArgumentNullException(nameof(updatedUser));

        // If a new password is provided, hash it
        if (!string.IsNullOrEmpty(newPlainPassword))
        {
            var salt = GenerateSalt();
            updatedUser.Salt = salt;
            updatedUser.HashedPassword = HashPassword(newPlainPassword, salt);
        }

        await _userDAO.UpdateUserAsync(id, updatedUser);
    }

    /// <summary>
    /// Deletes a user by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the ID is null or empty.</exception>
    public async Task DeleteUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));

        await _userDAO.DeleteUserAsync(id);
    }

    /// <summary>
    /// Authenticates a user by checking their username and password asynchronously.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="plainPassword">The plain text password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the authentication was successful.</returns>
    /// <exception cref="ArgumentException">Thrown when the username or password is null or empty.</exception>
    public async Task<bool> AuthenticateUserAsync(string username, string plainPassword)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plainPassword))
            throw new ArgumentException("Username and password cannot be empty");

        var user = await _userDAO.GetUserByUsernameAsync(username);
        if (user == null) return false;

        var hashedInputPassword = HashPassword(plainPassword, user.Salt);
        return user.HashedPassword == hashedInputPassword;
    }

    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of users.</returns>
    public Task<List<UserDTO>> GetUsersAsync()
    {
        return _userDAO.GetUsersAsync();
    }

    /// <summary>
    /// Toggles the follow status of an artist for the current user asynchronously.
    /// </summary>
    /// <param name="artistId">The ID of the artist to follow or unfollow.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the artist is now followed.</returns>
    /// <exception cref="ArgumentException">Thrown when the artist ID is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the current user is not found.</exception>
    public async Task<bool> ToggleFollowArtistAsync(string artistId)
    {
        if (string.IsNullOrEmpty(artistId))
            throw new ArgumentException("Artist ID cannot be null or empty", nameof(artistId));

        var currentUser = (App.Current as App).CurrentUser;

        if (currentUser == null)
            throw new InvalidOperationException("User not found");

        bool isFollowed;
        if (currentUser.FollowArtist.Contains(artistId))
        {
            currentUser.FollowArtist.Remove(artistId);
            isFollowed = false; // Unfollowed
        }
        else
        {
            currentUser.FollowArtist.Add(artistId);
            isFollowed = true; // Followed
        }

        // Update user information
        await _userDAO.UpdateUserAsync(currentUser.Id, currentUser);

        // Trigger the event to notify the change
        ArtistFollowStatusChanged?.Invoke(this, artistId);

        return isFollowed;
    }

    /// <summary>
    /// Hashes a password with a salt.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>The hashed password.</returns>
    private string HashPassword(string password, string salt)
    {
        // Here you can use a hashing algorithm like SHA256 or bcrypt
        // This is a placeholder function; replace with a secure hash
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + salt));
    }

    /// <summary>
    /// Generates a random salt.
    /// </summary>
    /// <returns>The generated salt.</returns>
    private string GenerateSalt()
    {
        // Generate a random salt for hashing; replace with a secure RNG implementation
        return Guid.NewGuid().ToString();
    }

    public async Task<UserDTO> getUserByUsernameAsync(string username)
    {
        return await _userDAO.GetUserByUsernameAsync(username);
    }
}
