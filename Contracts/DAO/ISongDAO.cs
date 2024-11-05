// ISongDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

public interface ISongDAO : IDAO
{
    Task<List<SongDTO>> SearchSongs(string query);
    Task<List<SongDTO>> GetAllSongs();

    Task<SongDTO> GetSongByIdAsync(string songId);
}
