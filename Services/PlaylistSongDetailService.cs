using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing playlist song details in the Spotify application.
/// </summary>
public class PlaylistSongDetailService
{
    private readonly IPlaylistSongDetailDAO _playlistSongDetailDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSongDetailService"/> class with the specified DAO.
    /// </summary>
    /// <param name="playlistSongDetailDAO">The data access object for playlist song details operations.</param>
    public PlaylistSongDetailService(IPlaylistSongDetailDAO playlistSongDetailDAO)
    {
        _playlistSongDetailDAO = playlistSongDetailDAO ?? throw new ArgumentNullException(nameof(playlistSongDetailDAO));
    }

    /// <summary>
    /// Retrieves the details of songs in a specific playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of song details in the playlist.</returns>
    public async Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId)
    {
        if (string.IsNullOrEmpty(playlistId))
            throw new ArgumentNullException(nameof(playlistId), "Playlist ID cannot be null or empty.");

        return await _playlistSongDetailDAO.GetPlaylistSongDetailAsync(playlistId);
    }

    /// <summary>
    /// Removes a song from a specific playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="songId">The ID of the song to be removed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RemoveSongFromPlaylistAsync(string playlistId, string songId)
    {
        if (string.IsNullOrEmpty(playlistId))
            throw new ArgumentNullException(nameof(playlistId), "Playlist ID cannot be null or empty.");
        if (string.IsNullOrEmpty(songId))
            throw new ArgumentNullException(nameof(songId), "Song ID cannot be null or empty.");

        await _playlistSongDetailDAO.DeleteOneAsync(playlistId, songId);
    }

    /// <summary>
    /// Adds a song to a specific playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="songDetail">The details of the song to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddSongToPlaylistAsync(string playlistId, PlaylistSongDetailDTO songDetail)
    {
        if (string.IsNullOrEmpty(playlistId))
            throw new ArgumentNullException(nameof(playlistId), "Playlist ID cannot be null or empty.");
        if (songDetail == null)
            throw new ArgumentNullException(nameof(songDetail), "Song detail cannot be null.");

        var playlistSong = new PlaylistSongDTO
        {
            PlaylistId = playlistId,
            SongId = songDetail.SongId,
            AddedAt = DateTime.Now,
            AddedBy = songDetail.AddedBy,
            Avatar = songDetail.Avatar,
        };

        // Add the song to the database
        await _playlistSongDetailDAO.InsertOneAsync(playlistSong);
    }
}
