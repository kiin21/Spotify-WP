using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class CommentDAO : BaseDAO, ICommentDAO
{
    private readonly IMongoCollection<CommentDTO> _comments;

    public CommentDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _comments = database.GetCollection<CommentDTO>("Comment");
    }

    public async Task<List<CommentDTO>> GetCommentsBySongIdAsync(string songId)
    {
        var filter = Builders<CommentDTO>.Filter.Eq(c => c.SongId, songId);
        return await _comments.Find(filter).ToListAsync();
    }

    public async Task AddCommentAsync(CommentDTO comment)
    {
        await _comments.InsertOneAsync(comment);
    }
}
