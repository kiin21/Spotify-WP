using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spotify.Services
{
    public class PlaylistService
    {
        private readonly IPlaylistDAO _playlistDAO;

        public PlaylistService(IPlaylistDAO playlistDAO)
        {
            _playlistDAO = playlistDAO ?? throw new ArgumentNullException(nameof(playlistDAO));
        }

        public Task<List<PlaylistDTO>> GetPlaylistsAsync() =>
            _playlistDAO.GetPlaylistsAsync();

        public Task<PlaylistDTO> GetPlaylistByIdAsync(string id) =>
            _playlistDAO.GetPlaylistByIdAsync(id);

        public Task<PlaylistDTO> GetLikedSongsPlaylistAsync() =>
            _playlistDAO.GetLikedSongsPlaylistAsync();
    }
}
