using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services;

/// <summary>
/// Provides services for managing comments in the Spotify application.
/// </summary>
public class CommentService
{
    private readonly ICommentDAO _commentDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentService"/> class with the specified DAO.
    /// </summary>
    /// <param name="commentDAO">The data access object for comment operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when the commentDAO is null.</exception>
    public CommentService(ICommentDAO commentDAO)
    {
        _commentDAO = commentDAO ?? throw new ArgumentNullException(nameof(commentDAO));
    }

    /// <summary>
    /// Retrieves comments for a specific song.
    /// </summary>
    /// <param name="songId">The ID of the song.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of comments.</returns>
    public Task<List<CommentDTO>> GetCommentsBySongIdAsync(string songId) =>
        _commentDAO.GetCommentsBySongIdAsync(songId);

    /// <summary>
    /// Adds a new comment to a song.
    /// </summary>
    /// <param name="songId">The ID of the song.</param>
    /// <param name="content">The content of the comment.</param>
    /// <param name="userName">The name of the user adding the comment.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddCommentAsync(string songId, string content, string userName)
    {
        var comment = new CommentDTO
        {
            SongId = songId,
            Content = content,
            AddedBy = userName
        };
        return _commentDAO.AddCommentAsync(comment);
    }
}
