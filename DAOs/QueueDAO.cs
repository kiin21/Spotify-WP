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

    public async Task UpdateQueueAsync(string id, QueueDTO updatedQueue)
    {
        //TODO: Implement this method
        throw new NotImplementedException();
        //var filter = Builders<QueueDTO>.Filter.Eq(q => q.Id, id);
        //var options = new FindOneAndReplaceOptions<QueueDTO> { ReturnDocument = ReturnDocument.After };
        //await _queues.FindOneAndReplaceAsync(filter, updatedQueue, options);
    }

    public async Task DeleteQueueAsync(string id)
    {
        //TODO : Implement this method
        throw new NotImplementedException();
        //var filter = Builders<QueueDTO>.Filter.Eq(q => q.Id, id);
        //await _queues.DeleteOneAsync(filter);
    }
}