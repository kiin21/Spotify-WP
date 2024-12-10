using System;
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

    // Event to notify when the Queue changes
    public event Action QueueChanged;
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
    public async Task AddQueueAsync(QueueDTO queue)
    {
        if (queue == null)
            throw new ArgumentNullException(nameof(queue));

        await _queueDAO.AddQueueAsync(queue);
        NotifyQueueChanged();
    }

    public async Task UpdateQueueAsync(string userId, List<string> songIds)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));
        if (songIds == null || songIds.Count == 0)
            throw new ArgumentNullException(nameof(songIds));

        await _queueDAO.UpdateQueueAsync(userId, songIds);
        NotifyQueueChanged();
    }

    public async Task DeleteQueueAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        await _queueDAO.DeleteQueueAsync(userId);
        NotifyQueueChanged();
    }

    private void NotifyQueueChanged()
    {
        QueueChanged?.Invoke();
    }
}
