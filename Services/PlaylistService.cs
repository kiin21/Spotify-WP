﻿using Spotify.Contracts.DAO;
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

        public Task<PlaylistDTO> GetLikedSongsPlaylistAsync(string userId) =>
            _playlistDAO.GetLikedSongsPlaylistAsync(userId);

        public Task AddPlaylistAsync(PlaylistDTO playlist)
        {
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            return _playlistDAO.AddPlaylistAsync(playlist);
        }

        public async Task UpdatePlaylistStatusAsync(string playlistId, bool isDeleted)
        {
            await _playlistDAO.UpdatePlaylistStatusAsync(playlistId, isDeleted);
        }

        public async Task RemovePlaylistAsync(string playlistId)
        {
            await _playlistDAO.RemovePlaylistAsync(playlistId);
        }

        public async Task<List<PlaylistDTO>> GetPlaylistsByUserIdAsync(string userId)
        {
            return await _playlistDAO.GetPlaylistsByUserIdAsync(userId);
        }

        public async Task<PlaylistDTO> EnsureLikedSongsPlaylistAsync(string userId, string username)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
                throw new ArgumentNullException("UserId or Username is null.");

            var likedSongsPlaylist = await _playlistDAO.GetLikedSongsPlaylistAsync(userId);

            if (likedSongsPlaylist == null)
            {
                // Tạo mới playlist "Liked Songs"
                likedSongsPlaylist = new PlaylistDTO
                {
                    Title = "Liked Songs",
                    CreatedBy = username,
                    CreatedAt = DateTime.Now,
                    OwnerId = userId,
                    IsLikedSong = true,
                    IsDeleted = false,
                    Avatar = "https://i1.sndcdn.com/artworks-4Lu85Xrs7UjJ4wVq-vuI2zg-t500x500.jpg"
                };

                await _playlistDAO.AddPlaylistAsync(likedSongsPlaylist);
            }

            return likedSongsPlaylist;
        }
    }
}
