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
        return await _playlists.Find(p => !p.IsDeleted).ToListAsync();
    }

    public async Task<PlaylistDTO> GetPlaylistByIdAsync(string playlistId)
    {
        if (string.IsNullOrEmpty(playlistId))
            return null;

        try
        {
            var filter = Builders<PlaylistDTO>.Filter.And(
                Builders<PlaylistDTO>.Filter.Eq(p => p.Id, playlistId),
                Builders<PlaylistDTO>.Filter.Eq(p => p.IsDeleted, false)
            );

            var playlist = await _playlists.Find(filter)
                .FirstOrDefaultAsync();

            return playlist;
        }
        catch (Exception ex)
        {
            // Log exception if you have logging system
            Console.WriteLine($"Error getting playlist by ID: {ex.Message}");
            throw; // Re-throw to handle it at higher level
        }
    }

    public async Task<PlaylistDTO> GetLikedSongsPlaylistAsync()
    {
        // Tìm kiếm playlist có thuộc tính IsLikedSongs là true
        var likedSongs = await _playlists.Find(p => p.IsLikedSong).FirstOrDefaultAsync();

        // Trả về PlaylistDTO nếu tìm thấy, ngược lại trả về null
        return likedSongs != null ? new PlaylistDTO(likedSongs) : null;
    }

    public async Task AddPlaylistAsync(PlaylistDTO playlist)
    {
        if (playlist == null) throw new ArgumentNullException(nameof(playlist));
        await _playlists.InsertOneAsync(playlist);
    }

    public async Task UpdatePlaylistStatusAsync(string playlistId, bool isDeleted)
    {
        var filter = Builders<PlaylistDTO>.Filter.Eq(p => p.Id, playlistId);
        var update = Builders<PlaylistDTO>.Update.Set(p => p.IsDeleted, isDeleted);
        await _playlists.UpdateOneAsync(filter, update);
    }

    public async Task RemovePlaylistAsync(string playlistId)
    {
        await UpdatePlaylistStatusAsync(playlistId, true);
    }

}