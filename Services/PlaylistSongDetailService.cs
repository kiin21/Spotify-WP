// PlaylistSongDetailService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

public class PlaylistSongDetailService
{
    private readonly IPlaylistSongDetailDAO _playlistSongDetailDAO;
    private readonly IPlaylistDAO _playlistDAO;

    public PlaylistSongDetailService(IPlaylistSongDetailDAO playlistSongDetailDAO)
    {
        _playlistSongDetailDAO = playlistSongDetailDAO;
    }

    public async Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId)
    {
        return await _playlistSongDetailDAO.GetPlaylistSongDetailAsync(playlistId);
    }

    public async Task RemoveSongFromPlaylistAsync(string playlistId, string songId)
    {
        await _playlistSongDetailDAO.DeleteOneAsync(playlistId, songId);
    }

    public async Task AddSongToPlaylistAsync(string playlistId, PlaylistSongDetailDTO songDetail)
    {
        var playlistSong = new PlaylistSongDTO
        {
            PlaylistId = playlistId,
            SongId = songDetail.SongId,
            AddedAt = DateTime.Now,
            AddedBy = songDetail.AddedBy,
            Avatar = songDetail.Avatar,
        };

        // Thêm bài hát vào database
        await _playlistSongDetailDAO.InsertOneAsync(playlistSong);
    }

}
