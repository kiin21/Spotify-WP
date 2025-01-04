using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Queue Data Access Object
/// </summary>
public interface IQueueDAO : IDAO
{
    /// <summary>
    /// Retrieves a queue by its ID.
    /// </summary>
    /// <param name="id">The ID of the queue.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a QueueDTO object.</returns>
    Task<QueueDTO> GetQueueByIdAsync(string id);

    /// <summary>
    /// Adds a new queue.
    /// </summary>
    /// <param name="queue">The queue to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddQueueAsync(QueueDTO queue);

    /// <summary>
    /// Updates the queue for a specific user.
    /// </summary>
    /// <param name="user_id">The ID of the user.</param>
    /// <param name="song_ids">The list of song IDs to update the queue with.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateQueueAsync(string user_id, List<string> song_ids);

    /// <summary>
    /// Deletes the queue for a specific user.
    /// </summary>
    /// <param name="user_id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteQueueAsync(string user_id);
}


