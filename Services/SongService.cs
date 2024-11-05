// SongService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

public class SongService
{
    private readonly ISongDAO _songDAO;

    public SongService(ISongDAO songDAO)
    {
        _songDAO = songDAO;
    }

    public Task<List<SongDTO>> SearchSongs(string query) =>
        _songDAO.SearchSongs(query);

        // Directly return List<SongDTO> instead of using generics
        public Task<List<SongDTO>> GetAllSongs() => _songDAO.GetAllSongs();

        public async Task<SongDTO> GetSongByIdAsync(string songId) =>
            await _songDAO.GetSongByIdAsync(songId);
    }
}
