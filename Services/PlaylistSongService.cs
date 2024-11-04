using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Services
{
    public class PlaylistSongService
    {
        private readonly IPlaylistSongDAO _playlistSongDAO;

        public PlaylistSongService(IPlaylistSongDAO playlistSongDAO)
        {
            _playlistSongDAO = playlistSongDAO;
        }

        public async Task<List<PlaylistSongDTO>> GetSongsForPlaylistAsync(string playlistId)
        {
            return await _playlistSongDAO.GetSongsByPlaylistIdAsync(playlistId);
        }
    }
}
