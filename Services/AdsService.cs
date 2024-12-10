using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Services
{
    public class AdsService
    {
        private readonly IAdsDAO _adsDAO;
        private static AdsService _instance;

        private AdsService(IAdsDAO adsDAO)
        {
            _adsDAO = adsDAO;
        }

        public static AdsService GetInstance(IAdsDAO adsDAO)
        {
            _instance = new AdsService(adsDAO);
            return _instance;
        }

        public async Task<AdsDTO> GetRandomAds() => await _adsDAO.GetRandomAds();
    }
}
