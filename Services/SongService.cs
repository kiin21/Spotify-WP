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

    // Returns a list of songs that match the search query
    public Task<List<SongDTO>> SearchSongs(string query) =>
        _songDAO.SearchSongs(query);

    // Returns all songs as a List<SongDTO>
    public Task<List<SongDTO>> GetAllSongs() =>
        _songDAO.GetAllSongs();

    // Retrieves a song by its ID asynchronously
    public async Task<SongDTO> GetSongByIdAsync(string songId) =>
        await _songDAO.GetSongByIdAsync(songId);
}
