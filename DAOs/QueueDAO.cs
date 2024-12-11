using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class QueueDAO : BaseDAO, IQueueDAO
{
    private readonly IMongoCollection<QueueDTO> _queues;

    public QueueDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _queues = database.GetCollection<QueueDTO>("Queue");
    }

    //public async Task<List<QueueDTO>> GetQueueAsync()
    //{
    //    return await _queues.Find(q => true).ToListAsync();
    //}

    public async Task<QueueDTO> GetQueueByIdAsync(string id)
    {
        var res = await _queues.Find(q => q.UserId.ToString() == id).FirstOrDefaultAsync();
        return res;
    }

    public async Task AddQueueAsync(QueueDTO queue)
    {
        if (queue == null) throw new ArgumentNullException(nameof(queue));
        await _queues.InsertOneAsync(queue);
    }

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



    public async Task DeleteQueueAsync(string user_id)
    {
        //TODO : Implement this method
        throw new NotImplementedException();
        //var filter = Builders<QueueDTO>.Filter.Eq(q => q.Id, id);
        //await _queues.DeleteOneAsync(filter);
    }
}