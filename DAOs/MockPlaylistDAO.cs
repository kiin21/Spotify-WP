using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.DAOs;

namespace Spotify.DAO;

public class MockPlaylistDAO : BaseDAO, IPlaylistDAO
{
    private List<PlaylistDTO> _mockPlaylists = new List<PlaylistDTO>
    {
        new PlaylistDTO { Title = "Artist 1", Album = "Album 1", DateAdded = DateTime.Now, Duration = TimeSpan.FromMinutes(3) },
        new PlaylistDTO { Title = "Artist 2", Album = "Album 2", DateAdded = DateTime.Now, Duration = TimeSpan.FromMinutes(4) },
        // Add more mock playlists here
    };

    public Task<List<PlaylistDTO>> GetPlaylistsAsync() =>
        Task.FromResult(_mockPlaylists);
}