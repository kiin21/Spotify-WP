using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing the playback queue.
/// </summary>
public class QueueService
{
    private readonly IQueueDAO _queueDAO;
    private readonly ISongDAO _songDAO;
    private readonly UserDTO _user;

    private static QueueService _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Occurs when the queue changes.
    /// </summary>
    public event Action QueueChanged;

    private QueueService(IQueueDAO queueDAO, ISongDAO songDAO, UserDTO user)
    {
        _queueDAO = queueDAO;
        _songDAO = songDAO;
        _user = user;
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="QueueService"/> class.
    /// </summary>
    /// <param name="queueDAO">The data access object for queues.</param>
    /// <param name="songDAO">The data access object for songs.</param>
    /// <param name="user">The user.</param>
    /// <returns>The singleton instance of the <see cref="QueueService"/> class.</returns>
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

    /// <summary>
    /// Gets the queue for the current user asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the queue of songs.</returns>
    public async Task<ObservableCollection<SongDTO>> GetQueue()
    {
        QueueDTO queue = await _queueDAO.GetQueueByIdAsync(_user.Id);

        if (queue == null)
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

    /// <summary>
    /// Adds a new queue asynchronously.
    /// </summary>
    /// <param name="queue">The queue to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the queue is null.</exception>
    public async Task AddQueueAsync(QueueDTO queue)
    {
        if (queue == null)
            throw new ArgumentNullException(nameof(queue));

        await _queueDAO.AddQueueAsync(queue);
        NotifyQueueChanged();
    }

    /// <summary>
    /// Updates the queue for the specified user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="songIds">The list of song IDs to update the queue with.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the userId or songIds are null or empty.</exception>
    public async Task UpdateQueueAsync(string userId, List<string> songIds)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));
        if (songIds == null || songIds.Count == 0)
            throw new ArgumentNullException(nameof(songIds));

        await _queueDAO.UpdateQueueAsync(userId, songIds);
        NotifyQueueChanged();
    }

    /// <summary>
    /// Deletes the queue for the specified user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the userId is null or empty.</exception>
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

    /// HOT_FIX
    public async Task AddQueueForNewUser(string userId)
    {
        QueueDTO queue = new QueueDTO
        {
            UserId = userId,
            SongIds = new List<string>()
        };
        await AddQueueAsync(queue);
    }
    /// End HOT_FIX
}
