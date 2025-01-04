using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.DAOs;

/// <summary>
/// Data Access Object for Comments
/// </summary>
public class CommentDAO : BaseDAO, ICommentDAO
{
    private readonly IMongoCollection<CommentDTO> _comments;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDAO"/> class.
    /// </summary>
    public CommentDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _comments = database.GetCollection<CommentDTO>("Comment");
    }

    /// <summary>
    /// Retrieves comments by song ID.
    /// </summary>
    /// <param name="songId">The ID of the song.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of CommentDTO.</returns>
    public async Task<List<CommentDTO>> GetCommentsBySongIdAsync(string songId)
    {
        var filter = Builders<CommentDTO>.Filter.Eq(c => c.SongId, songId);
        return await _comments.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Adds a new comment.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddCommentAsync(CommentDTO comment)
    {
        await _comments.InsertOneAsync(comment);
    }
}