// ArtistService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

public class ArtistService
{
    private readonly IArtistDAO _artistDAO;

    public ArtistService(IArtistDAO artistDAO)
    {
        _artistDAO = artistDAO;
    }

    public async Task<List<ArtistDTO>> GetAllArtistsAsync()
    {
        return await _artistDAO.GetAllArtistsAsync();
    }

    public async Task<ArtistDTO> GetArtistByIdAsync(string id)
    {
        return await _artistDAO.GetArtistByIdAsync(id);
    }

    public async Task<ArtistDTO> GetArtistByNameAsync(string name)
    {
        return await _artistDAO.GetArtistByNameAsync(name);
    }
}// ArtistService.cs
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

public class ArtistService
{
    private readonly IArtistDAO _artistDAO;

    // Cache dữ liệu song_ids của mỗi nghệ sĩ
    private readonly ConcurrentDictionary<string, List<string>> _artistSongCache = new();

    public ArtistService(IArtistDAO artistDAO)
    {
        _artistDAO = artistDAO;
    }

    public async Task<List<ArtistDTO>> GetAllArtistsAsync()
    {
        return await _artistDAO.GetAllArtistsAsync();
    }

    public async Task<ArtistDTO> GetArtistByIdAsync(string id)
    {
        return await _artistDAO.GetArtistByIdAsync(id);
    }

    public async Task<ArtistDTO> GetArtistByNameAsync(string name)
    {
        return await _artistDAO.GetArtistByNameAsync(name);
    }

    public List<string> GetCachedSongs(string artistId)
    {
        return _artistSongCache.TryGetValue(artistId, out var cachedSongs) ? cachedSongs : new List<string>();
    }

    public void UpdateCache(string artistId, List<string> currentSongs)
    {
        _artistSongCache[artistId] = currentSongs;
    }
}