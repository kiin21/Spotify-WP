//PlaylistSongDAO.cs
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
/// Data Access Object for Playlist Songs
/// </summary>
public class PlaylistSongDAO : BaseDAO, IPlaylistSongDAO
{
    private readonly IMongoCollection<PlaylistSongDTO> _playlistSongs;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSongDAO"/> class.
    /// </summary>
    public PlaylistSongDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playlistSongs = database.GetCollection<PlaylistSongDTO>("PlaylistSong");
    }

    /// <summary>
    /// Retrieves songs by playlist ID.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistSongDTO.</returns>
    public async Task<List<PlaylistSongDTO>> GetSongsByPlaylistIdAsync(string playlistId)
    {
        return await _playlistSongs.Find(song => song.PlaylistId == playlistId).ToListAsync();
    }
}
