using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Services;

/// <summary>
/// Service for handling playlist songs.
/// </summary>
public class PlaylistSongService
{
    private readonly IPlaylistSongDAO _playlistSongDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSongService"/> class.
    /// </summary>
    /// <param name="playlistSongDAO">The playlist song DAO.</param>
    public PlaylistSongService(IPlaylistSongDAO playlistSongDAO)
    {
        _playlistSongDAO = playlistSongDAO;
    }

    /// <summary>
    /// Gets the songs for a playlist asynchronously.
    /// </summary>
    /// <param name="playlistId">The playlist identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="PlaylistSongDTO"/>.</returns>
    public async Task<List<PlaylistSongDTO>> GetSongsForPlaylistAsync(string playlistId)
    {
        return await _playlistSongDAO.GetSongsByPlaylistIdAsync(playlistId);
    }
}
