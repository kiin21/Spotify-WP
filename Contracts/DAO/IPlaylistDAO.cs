using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO
{
    public interface IPlaylistDAO : IDAO
    {
        Task<List<PlaylistDTO>> GetPlaylistsAsync();
        Task<PlaylistDTO> GetPlaylistByIdAsync(string id);
        Task<PlaylistDTO> GetLikedSongsPlaylistAsync(string userId);
        Task AddPlaylistAsync(PlaylistDTO playlist);
        Task UpdatePlaylistStatusAsync(string playlistId, bool isDeleted);
        Task RemovePlaylistAsync(string playlistId);
        Task<List<PlaylistDTO>> GetPlaylistsByUserIdAsync(string userId);
        Task UpdatePlaylistAsync(string playlistId, PlaylistDTO updatedPlaylist);

    }
}
