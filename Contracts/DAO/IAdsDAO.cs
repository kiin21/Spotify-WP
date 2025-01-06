using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Ads Data Access Object
/// </summary>
public interface IAdsDAO : IDAO
{
    /// <summary>
    /// Retrieves all ads.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of AdsDTO.</returns>
    Task<List<AdsDTO>> GetAllAds();

    /// <summary>
    /// Retrieves a random ad.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an AdsDTO object.</returns>
    Task<AdsDTO> GetRandomAds();
}