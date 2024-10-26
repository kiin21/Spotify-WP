using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spotify.Services
{
    public class LikedSongService
    {
        private readonly ILikedSongDAO _likedSongDAO;

        public LikedSongService(ILikedSongDAO likedSongDAO)
        {
            _likedSongDAO = likedSongDAO;
        }

        public Task<List<LikedSongDTO>> GetLikedSongsAsync() =>
            _likedSongDAO.GetLikedSongAsync();
    }
}