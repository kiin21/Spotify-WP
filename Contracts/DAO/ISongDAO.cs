// ISongDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

public interface ISongDAO : IDAO
{
    Task<List<SongDTO>> SearchSongsAsync(string query);
}
