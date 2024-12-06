// IPlayHistoryDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

public interface IPlayHistoryDAO : IDAO
{
    Task<List<PlayHistoryWithSongDTO>> GetUserHistoryWithSongAsync(string userID); 
    Task InsertPlayHistoryAsync(PlayHistoryDTO playHistory);
}


