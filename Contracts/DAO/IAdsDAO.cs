using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO
{
    public interface IAdsDAO: IDAO
    {
        Task<List<AdsDTO>> GetAllAds();

        Task<AdsDTO> GetRandomAds();
    }
}
