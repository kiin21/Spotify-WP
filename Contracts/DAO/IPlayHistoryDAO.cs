// IPlayHistoryDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

public interface IPlayHistoryDAO: IDAO
{
    Task<List<PlayHistoryDTO>> GetUserHistoryAsync(string userID);
}


