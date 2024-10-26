using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spotify.Services
{
    public class PlaylistService
    {
        private readonly IPlaylistDAO _playlistDAO;

        public PlaylistService(IPlaylistDAO playlistDAO)
        {
            _playlistDAO = playlistDAO;
        }

        public Task<List<PlaylistDTO>> GetPlaylistsAsync() =>
            _playlistDAO.GetPlaylistsAsync();
    }
}