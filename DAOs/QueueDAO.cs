using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.DAOs;

/// <summary>
/// Data Access Object for Queue
/// </summary>
public class QueueDAO : BaseDAO, IQueueDAO
{
    private readonly IMongoCollection<QueueDTO> _queues;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueDAO"/> class.
    /// </summary>
    public QueueDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _queues = database.GetCollection<QueueDTO>("Queue");
    }

    /// <summary>
    /// Retrieves a queue by its ID.
    /// </summary>
    /// <param name="id">The ID of the queue.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a QueueDTO object.</returns>
    public async Task<QueueDTO> GetQueueByIdAsync(string id)
    {
        var res = await _queues.Find(q => q.UserId.ToString() == id).FirstOrDefaultAsync();
        return res;
    }

    /// <summary>
    /// Adds a new queue.
    /// </summary>
    /// <param name="queue">The queue to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddQueueAsync(QueueDTO queue)
    {
        if (queue == null) throw new ArgumentNullException(nameof(queue));
        await _queues.InsertOneAsync(queue);
    }

    /// <summary>
    /// Updates the queue for a specific user.
    /// </summary>
    /// <param name="user_id">The ID of the user.</param>
    /// <param name="song_ids">The list of song IDs to update the queue with.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateQueueAsync(string user_id, List<string> song_ids)
    {
        if (string.IsNullOrWhiteSpace(user_id))
            throw new ArgumentNullException(nameof(user_id));
        if (song_ids == null || song_ids.Count == 0)
            throw new ArgumentNullException(nameof(song_ids));

        var filter = Builders<QueueDTO>.Filter.Eq(q => q.UserId, user_id);

        // Use UpdateDefinition to replace the `song_ids` field
        var update = Builders<QueueDTO>.Update.Set(q => q.SongIds, song_ids);

        var updateResult = await _queues.UpdateOneAsync(filter, update);

        // Check if any document matched the filter
        if (updateResult.MatchedCount == 0)
        {
            throw new InvalidOperationException("No queue found for the given user_id.");
        }

        // Check if the document was modified
        if (updateResult.ModifiedCount == 0)
        {
            throw new InvalidOperationException("No changes were made to the queue. The provided data might be the same as the existing data.");
        }
    }

    /// <summary>
    /// Deletes the queue for a specific user.
    /// </summary>
    /// <param name="user_id">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteQueueAsync(string user_id)
    {
        //TODO : Implement this method
        throw new NotImplementedException();
    }
}