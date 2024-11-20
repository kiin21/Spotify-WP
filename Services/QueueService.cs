using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services
{
    public class QueueService
    {
        private readonly IQueueDAO _queueDAO;
        private readonly ISongDAO _songDAO;

        public QueueService(IQueueDAO queueDAO, ISongDAO songDAO)
        {
            _queueDAO = queueDAO;
            _songDAO = songDAO;
        }

        public async Task<List<SongDTO>> GetQueueById(string id)
        {
            QueueDTO queue = await _queueDAO.GetQueueByIdAsync(id);
            List<SongDTO> songs = new List<SongDTO>();

            foreach (string songId in queue.SongIds)
            {
                SongDTO song = await _songDAO.GetSongByIdAsync(songId);
                songs.Add(song);
            }

            return songs;
        }

        public Task AddQueue(QueueDTO queue) => _queueDAO.AddQueueAsync(queue);

        public Task UpdateQueue(string id, QueueDTO updatedQueue) => _queueDAO.UpdateQueueAsync(id, updatedQueue);

        public Task DeleteQueue(string id) => _queueDAO.DeleteQueueAsync(id);
    }
}
