using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO
{
    public interface IUserDAO : IDAO
    {
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<UserDTO> GetUserByUsernameAsync(string username);
        Task AddUserAsync(UserDTO user);
        Task UpdateUserAsync(string id, UserDTO updatedUser);
        Task DeleteUserAsync(string id);
    }
}
