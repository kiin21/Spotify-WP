using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

public class QueueService
{
    private readonly IQueueDAO _queueDAO;
    private readonly ISongDAO _songDAO;
    private readonly UserDTO _user;

    private static QueueService _instance;
    private static readonly object _lock = new object();

    private QueueService(IQueueDAO queueDAO, ISongDAO songDAO, UserDTO user)
    {
        _queueDAO = queueDAO;
        _songDAO = songDAO;
        _user = user;
    }

    // Public method to get the single instance of QueueService
    public static QueueService GetInstance(IQueueDAO queueDAO, ISongDAO songDAO, UserDTO user)
    {
        // Double-checked locking for thread safety
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new QueueService(queueDAO, songDAO, user);
                }
            }
        }
        return _instance;
    }

    public async Task<ObservableCollection<SongDTO>> GetQueue()
    {
        QueueDTO queue = await _queueDAO.GetQueueByIdAsync(_user.Id);
        
        if(queue == null)
        {
            return new ObservableCollection<SongDTO>();
        }

        ObservableCollection<SongDTO> songs = new ObservableCollection<SongDTO>();

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
