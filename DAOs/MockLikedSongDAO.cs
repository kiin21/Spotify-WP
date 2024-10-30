using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.DAO;

public class MockLikedSongDAO : ILikedSongDAO
{
    private List<LikedSongDTO> _mockLikedSongs = new List<LikedSongDTO>
    {
        new LikedSongDTO {
            Title = "Artist 1",
            Album = "Album 1",
            DateAdded = DateTime.Now,
            Duration = TimeSpan.FromMinutes(3),
            Image = "/Assets/Image1.jpg"
        },
        new LikedSongDTO {
            Title = "Artist 2",
            Album = "Album 2",
            DateAdded = DateTime.Now,
            Duration = TimeSpan.FromMinutes(4),
            Image = "/Assets/Image2.jpg"
        },
    };

    public Task<List<LikedSongDTO>> GetLikedSongAsync() =>
        Task.FromResult(_mockLikedSongs);
}