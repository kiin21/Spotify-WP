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

}