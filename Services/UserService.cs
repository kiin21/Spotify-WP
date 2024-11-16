//UserService.cs

using System;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using Spotify.DAOs;
using System.Collections.Generic;
using Spotify.Helpers;

namespace Spotify.Services;

public class UserService
{
    private readonly IUserDAO _userDAO;
    public event EventHandler<string> ArtistFollowStatusChanged;

    public UserService(IUserDAO userDAO)
    {
        _userDAO = userDAO ?? throw new ArgumentNullException(nameof(userDAO));
    }

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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userDAO.AddUserAsync(user);
    }


    // Method to get a user by ID
    public async Task<UserDTO> GetUserByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _userDAO.GetUserByIdAsync(id);
    }

    // Method to update user information
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

    // Method to delete a user by ID
    public async Task DeleteUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("ID cannot be null or empty", nameof(id));

        await _userDAO.DeleteUserAsync(id);
    }

    // Method to authenticate a user by checking their username and password
    public async Task<bool> AuthenticateUserAsync(string username, string plainPassword)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plainPassword))
            throw new ArgumentException("Username and password cannot be empty");

        var user = await _userDAO.GetUserByUsernameAsync(username);
        if (user == null) return false;

        var hashedInputPassword = HashPassword(plainPassword, user.Salt);
        return user.HashedPassword == hashedInputPassword;
    }

    // Helper method to hash a password with a salt
    private string HashPassword(string password, string salt)
    {
        // Here you can use a hashing algorithm like SHA256 or bcrypt
        // This is a placeholder function; replace with a secure hash
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + salt));
    }

    // Helper method to generate a random salt
    private string GenerateSalt()
    {
        // Generate a random salt for hashing; replace with a secure RNG implementation
        return Guid.NewGuid().ToString();
    }

    public Task<List<UserDTO>> GetUsersAsync()
    {
        return _userDAO.GetUsersAsync();
    }

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
            isFollowed = false; // Đã bỏ theo dõi
        }
        else
        {
            currentUser.FollowArtist.Add(artistId);
            isFollowed = true; // Đã theo dõi
        }

        // Cập nhật thông tin người dùng
        await _userDAO.UpdateUserAsync(currentUser.Id, currentUser);

        // Kích hoạt sự kiện thông báo thay đổi
        ArtistFollowStatusChanged?.Invoke(this, artistId);

        return isFollowed;
    }

}
