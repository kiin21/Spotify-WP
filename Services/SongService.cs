// SongService.cs
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Service for handling song-related operations.
/// </summary>
public class SongService
{
    private readonly ISongDAO _songDAO;
    private readonly ConcurrentDictionary<string, List<string>> _songCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SongService"/> class.
    /// </summary>
    /// <param name="songDAO">The song DAO.</param>
    public SongService(ISongDAO songDAO)
    {
        _songDAO = songDAO;
    }

    /// <summary>
    /// Searches for songs that match the given query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="SongDTO"/>.</returns>
    public Task<List<SongDTO>> SearchSongs(string query) =>
        _songDAO.SearchSongs(query);

    /// <summary>
    /// Gets all songs.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="SongDTO"/>.</returns>
    public Task<List<SongDTO>> GetAllSongs() =>
        _songDAO.GetAllSongs();

    /// <summary>
    /// Retrieves a song by its ID asynchronously.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="SongDTO"/>.</returns>
    public async Task<SongDTO> GetSongByIdAsync(string songId) =>
        await _songDAO.GetSongByIdAsync(songId);

    /// <summary>
    /// Gets the cached songs for a specific artist.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    /// <returns>A list of cached song identifiers.</returns>
    public List<string> GetCachedSongs(string artistId)
    {
        if (!_songCache.TryGetValue(artistId, out var cachedSongs))
        {
            // Create a new cache if it does not exist
            cachedSongs = new List<string>();
            _songCache.TryAdd(artistId, cachedSongs);
        }

        return cachedSongs;
    }

    /// <summary>
    /// Updates the cache with the current songs for a specific artist.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    /// <param name="currentSongs">The list of current song identifiers.</param>
    public void UpdateCache(string artistId, List<string> currentSongs)
    {
        _songCache[artistId] = currentSongs;
    }
}
