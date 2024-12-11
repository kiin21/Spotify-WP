using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing songs in the Spotify application.
/// </summary>
public class SongService
{
    private readonly ISongDAO _songDAO;
    private readonly ConcurrentDictionary<string, List<string>> _songCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SongService"/> class with the specified DAO.
    /// </summary>
    /// <param name="songDAO">The data access object for song operations.</param>
    public SongService(ISongDAO songDAO)
    {
        _songDAO = songDAO ?? throw new ArgumentNullException(nameof(songDAO));
    }

    /// <summary>
    /// Searches for songs that match the given query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of matching songs.</returns>
    public Task<List<SongDTO>> SearchSongs(string query) =>
        _songDAO.SearchSongs(query);

    /// <summary>
    /// Retrieves all songs in the application.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of all songs.</returns>
    public Task<List<SongDTO>> GetAllSongs() =>
        _songDAO.GetAllSongs();

    /// <summary>
    /// Retrieves a song by its ID asynchronously.
    /// </summary>
    /// <param name="songId">The ID of the song to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the song details.</returns>
    public async Task<SongDTO> GetSongByIdAsync(string songId) =>
        await _songDAO.GetSongByIdAsync(songId);

    /// <summary>
    /// Retrieves cached songs for a specific artist by their ID.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <returns>A list of cached song IDs for the artist.</returns>
    public List<string> GetCachedSongs(string artistId)
    {
        if (!_songCache.TryGetValue(artistId, out var cachedSongs))
        {
            // Create a new cache entry if not found
            cachedSongs = new List<string>();
            _songCache.TryAdd(artistId, cachedSongs);
        }

        return cachedSongs;
    }

    /// <summary>
    /// Updates the song cache for a specific artist.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="currentSongs">The list of song IDs to update in the cache.</param>
    public void UpdateCache(string artistId, List<string> currentSongs)
    {
        if (string.IsNullOrEmpty(artistId))
            throw new ArgumentNullException(nameof(artistId), "Artist ID cannot be null or empty.");
        if (currentSongs == null)
            throw new ArgumentNullException(nameof(currentSongs), "Current songs cannot be null.");

        _songCache[artistId] = currentSongs;
    }
}
