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

        public async Task<PlaylistDTO> GetLikedSongsPlaylistAsync() =>
            await _playlistDAO.GetLikedSongsPlaylistAsync();

        public Task AddPlaylistAsync(PlaylistDTO playlist)
        {
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            return _playlistDAO.AddPlaylistAsync(playlist);
        }

        public async Task RemovePlaylistAsync(string playlistId)
        {
            await _playlistDAO.RemovePlaylistAsync(playlistId);
            // Không cần trả về gì vì chúng ta sẽ lấy lại danh sách trong ViewModel
        }
    }
}
