using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO
{
    public interface IQueueDAO : IDAO
    {
        //Task<List<QueueDTO>> GetQueueAsync();
        Task<QueueDTO> GetQueueByIdAsync(string id);
        Task AddQueueAsync(QueueDTO queue);
        Task UpdateQueueAsync(string user_id, List<string> song_ids);
        Task DeleteQueueAsync(string user_id);
    }
}