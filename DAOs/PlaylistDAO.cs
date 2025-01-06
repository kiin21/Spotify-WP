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

namespace Spotify.DAOs;

/// <summary>
/// Data Access Object for Playlists
/// </summary>
public class PlaylistDAO : BaseDAO, IPlaylistDAO
{
    private readonly IMongoCollection<PlaylistDTO> _playlists;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistDAO"/> class.
    /// </summary>
    public PlaylistDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playlists = database.GetCollection<PlaylistDTO>("Playlist");
    }

    /// <summary>
    /// Retrieves all playlists.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistDTO.</returns>
    public async Task<List<PlaylistDTO>> GetPlaylistsAsync()
    {
        return await _playlists.Find(p => !p.IsDeleted).ToListAsync();
    }

    /// <summary>
    /// Retrieves a playlist by its ID.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a PlaylistDTO object.</returns>
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

    /// <summary>
    /// Retrieves the liked songs playlist for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a PlaylistDTO object.</returns>
    public async Task<PlaylistDTO> GetLikedSongsPlaylistAsync(string userId)
    {
        var filter = Builders<PlaylistDTO>.Filter.And(
            Builders<PlaylistDTO>.Filter.Eq(p => p.OwnerId, userId),
            Builders<PlaylistDTO>.Filter.Eq(p => p.IsLikedSong, true),
            Builders<PlaylistDTO>.Filter.Eq(p => p.IsDeleted, false)
        );

        return await _playlists.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds a new playlist.
    /// </summary>
    /// <param name="playlist">The playlist to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddPlaylistAsync(PlaylistDTO playlist)
    {
        if (playlist == null) throw new ArgumentNullException(nameof(playlist));
        await _playlists.InsertOneAsync(playlist);
    }

    /// <summary>
    /// Updates the status of a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="isDeleted">The new status of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdatePlaylistStatusAsync(string playlistId, bool isDeleted)
    {
        var filter = Builders<PlaylistDTO>.Filter.Eq(p => p.Id, playlistId);
        var update = Builders<PlaylistDTO>.Update.Set(p => p.IsDeleted, isDeleted);
        await _playlists.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// Removes a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RemovePlaylistAsync(string playlistId)
    {
        await UpdatePlaylistStatusAsync(playlistId, true);
    }

    /// <summary>
    /// Retrieves playlists by user ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistDTO.</returns>
    public async Task<List<PlaylistDTO>> GetPlaylistsByUserIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

        var filter = Builders<PlaylistDTO>.Filter.And(
            Builders<PlaylistDTO>.Filter.Eq(p => p.OwnerId, userId),
            Builders<PlaylistDTO>.Filter.Eq(p => p.IsDeleted, false)
        );

        return await _playlists.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Updates an existing playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="updatedPlaylist">The updated playlist data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdatePlaylistAsync(string playlistId, PlaylistDTO updatedPlaylist)
    {
        var filter = Builders<PlaylistDTO>.Filter.Eq(p => p.Id, playlistId);
        var update = Builders<PlaylistDTO>.Update
            .Set(p => p.Title, updatedPlaylist.Title)
            .Set(p => p.Avatar, updatedPlaylist.Avatar)
            .Set(p => p.ShareWithUsers, updatedPlaylist.ShareWithUsers)
            .Set(p => p.IsDeleted, updatedPlaylist.IsDeleted);

        await _playlists.UpdateOneAsync(filter, update);
    }
}