using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO
{
    public interface ILikedSongDAO : IDAO
    {
        Task<List<LikedSongDTO>> GetLikedSongAsync();
    }
}
