//PlaylistDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class PlaylistDAO : BaseDAO, IPlaylistDAO
{
    private readonly IMongoCollection<PlaylistDTO> _playlists;

    public PlaylistDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playlists = database.GetCollection<PlaylistDTO>("Playlist");
    }

    public async Task<List<PlaylistDTO>> GetPlaylistsAsync()
    {
        return await _playlists.Find(_ => true).ToListAsync();
    }

    public async Task<PlaylistDTO> GetPlaylistByIdAsync(string playlistId)
    {
        // Gọi MongoDB để lấy playlist với ID cụ thể
        return await _playlists.Find(p => p.Id.ToString() == playlistId).FirstOrDefaultAsync();
    }

    public async Task<PlaylistDTO> GetLikedSongsPlaylistAsync()
    {
        // Tìm kiếm playlist có thuộc tính IsLikedSongs là true
        var likedSongs = await _playlists.Find(p => p.IsLikedSong).FirstOrDefaultAsync();

        // Trả về PlaylistDTO nếu tìm thấy, ngược lại trả về null
        return likedSongs != null ? new PlaylistDTO(likedSongs) : null;
    }

}