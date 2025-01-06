using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for User Data Access Object
/// </summary>
public interface IUserDAO : IDAO
{
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of UserDTO.</returns>
    Task<List<UserDTO>> GetUsersAsync();

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UserDTO object.</returns>
    Task<UserDTO> GetUserByIdAsync(string id);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UserDTO object.</returns>
    Task<UserDTO> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddUserAsync(UserDTO user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="updatedUser">The updated user data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateUserAsync(string id, UserDTO updatedUser);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteUserAsync(string id);
}


