using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Services
{
    /// <summary>
    /// Provides services for managing advertisements.
    /// </summary>
    public class AdsService
    {
        private readonly IAdsDAO _adsDAO;
        private static AdsService _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdsService"/> class.
        /// </summary>
        /// <param name="adsDAO">The data access object for advertisements.</param>
        private AdsService(IAdsDAO adsDAO)
        {
            _adsDAO = adsDAO;
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="AdsService"/> class.
        /// </summary>
        /// <param name="adsDAO">The data access object for advertisements.</param>
        /// <returns>The singleton instance of the <see cref="AdsService"/> class.</returns>
        public static AdsService GetInstance(IAdsDAO adsDAO)
        {
            _instance = new AdsService(adsDAO);
            return _instance;
        }

        /// <summary>
        /// Gets a random advertisement.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the random advertisement.</returns>
        public async Task<AdsDTO> GetRandomAds() => await _adsDAO.GetRandomAds();
    }
}
