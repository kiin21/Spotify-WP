// IArtistDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Artist Data Access Object
/// </summary>
public interface IArtistDAO : IDAO
{
    /// <summary>
    /// Retrieves all artists.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of ArtistDTO.</returns>
    Task<List<ArtistDTO>> GetAllArtistsAsync();

    /// <summary>
    /// Retrieves an artist by their ID.
    /// </summary>
    /// <param name="id">The ID of the artist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an ArtistDTO object.</returns>
    Task<ArtistDTO> GetArtistByIdAsync(string id);

    /// <summary>
    /// Retrieves an artist by their name.
    /// </summary>
    /// <param name="name">The name of the artist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an ArtistDTO object.</returns>
    Task<ArtistDTO> GetArtistByNameAsync(string name);
}
