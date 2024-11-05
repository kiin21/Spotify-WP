// IPlaylistSongDetailDAO.cs
using Spotify.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO
{
    public interface IPlaylistSongDetailDAO : IDAO
    {
        Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId);
    }
}
