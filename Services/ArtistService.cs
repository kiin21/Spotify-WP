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
}