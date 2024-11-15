// IArtistDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO
{
    public interface IArtistDAO : IDAO
    {
        Task<List<ArtistDTO>> GetAllArtistsAsync();
        Task<ArtistDTO> GetArtistByIdAsync(string id);

        Task<ArtistDTO> GetArtistByNameAsync(string name);
    }
}
