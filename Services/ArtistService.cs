// ArtistService.cs
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

/// <summary>
/// Provides services for managing artists.
/// </summary>
public class ArtistService
{
    private readonly IArtistDAO _artistDAO;

    // Cache for storing song IDs of each artist
    private readonly ConcurrentDictionary<string, List<string>> _artistSongCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistService"/> class.
    /// </summary>
    /// <param name="artistDAO">The data access object for artists.</param>
    public ArtistService(IArtistDAO artistDAO)
    {
        _artistDAO = artistDAO;
    }

    /// <summary>
    /// Gets all artists asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of all artists.</returns>
    public async Task<List<ArtistDTO>> GetAllArtistsAsync()
    {
        return await _artistDAO.GetAllArtistsAsync();
    }

    /// <summary>
    /// Gets an artist by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the artist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the artist with the specified ID.</returns>
    public async Task<ArtistDTO> GetArtistByIdAsync(string id)
    {
        return await _artistDAO.GetArtistByIdAsync(id);
    }

    /// <summary>
    /// Gets an artist by name asynchronously.
    /// </summary>
    /// <param name="name">The name of the artist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the artist with the specified name.</returns>
    public async Task<ArtistDTO> GetArtistByNameAsync(string name)
    {
        return await _artistDAO.GetArtistByNameAsync(name);
    }

    /// <summary>
    /// Gets the cached songs for the specified artist.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <returns>The list of cached song IDs for the specified artist.</returns>
    public List<string> GetCachedSongs(string artistId)
    {
        return _artistSongCache.TryGetValue(artistId, out var cachedSongs) ? cachedSongs : new List<string>();
    }

    /// <summary>
    /// Updates the cache with the current songs for the specified artist.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="currentSongs">The list of current song IDs for the artist.</param>
    public void UpdateCache(string artistId, List<string> currentSongs)
    {
        _artistSongCache[artistId] = currentSongs;
    }
}
