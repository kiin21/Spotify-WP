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
    private static string FOLDER_URL = "D:\\Download\\Music Audio";

    private List<SongDTO> _mockSongs = new List<SongDTO>
    {
        // new SongDTO { Id = 1, Title = "Song 1", Artist = "Artist 1", Album = "Album 1", Duration = TimeSpan.FromMinutes(3) },
        // new SongDTO { Id = 2, Title = "Song 2", Artist = "Artist 2", Album = "Album 2", Duration = TimeSpan.FromMinutes(4) },
        new SongDTO { Id = 3, Title = "Want you", Artist = "Oxlade", ImageUrl = "../Assets/want_you_img.png", AudioUrl = $"{FOLDER_URL}\\Want_You.mp3", Duration = TimeSpan.FromMinutes(2.33) },
        new SongDTO { Id = 4, Title = "ThienLyOi", Artist = "J97", ImageUrl = "../Assets/ThienLyOi_img.png", AudioUrl = $"{FOLDER_URL}\\ThienLyOi.mp3", Duration = TimeSpan.FromMinutes(1.30) },
    };
   
    public Task<List<SongDTO>> SearchSongsAsync(string query) =>
        Task.FromResult(_mockSongs.Where(s => s.Title.Contains(query)).ToList());
};
