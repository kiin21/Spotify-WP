//LocalStorageService.cs
using Windows.Storage;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Linq;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing local storage.
/// </summary>
public class LocalStorageService
{
    private readonly ApplicationDataContainer _localSettings;
    private const string USERS_KEY = "users";

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStorageService"/> class.
    /// </summary>
    public LocalStorageService()
    {
        _localSettings = ApplicationData.Current.LocalSettings;
    }

    /// <summary>
    /// Saves a user asynchronously.
    /// </summary>
    /// <param name="user">The user to save.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveUserAsync(UserDTO user)
    {
        try
        {
            var users = await GetUsersAsync();

            // Check if user already exists
            if (!users.Any(u => u.Username == user.Username))
            {
                users.Add(user);
                string jsonUsers = JsonSerializer.Serialize(users);
                _localSettings.Values[USERS_KEY] = jsonUsers;
            }
        }
        catch (Exception ex)
        {
            // Handle or log error
            Debug.WriteLine($"Error saving user: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of users.</returns>
    public Task<List<UserDTO>> GetUsersAsync()
    {
        try
        {
            if (_localSettings.Values.TryGetValue(USERS_KEY, out object jsonUsers))
            {
                var users = JsonSerializer.Deserialize<List<UserDTO>>(jsonUsers.ToString());
                return Task.FromResult(users);
            }
            return Task.FromResult(new List<UserDTO>());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting users: {ex.Message}");
            return Task.FromResult(new List<UserDTO>());
        }
    }
}
