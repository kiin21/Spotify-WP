//MockSongDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.DAO;

public class MockSongDAO : ISongDAO
{
    private List<SongDTO> _mockSongs = new List<SongDTO>
    {
        new SongDTO { Title = "Song 1", Artist = "Artist 1", Album = "Album 1", Duration = TimeSpan.FromMinutes(3) },
        new SongDTO { Title = "Song 2", Artist = "Artist 2", Album = "Album 2", Duration = TimeSpan.FromMinutes(4) },
        // Add more mock songs here
    };

    public Task<List<SongDTO>> SearchSongsAsync(string query) =>
        Task.FromResult(_mockSongs.Where(s => s.Title.Contains(query)).ToList());
}
