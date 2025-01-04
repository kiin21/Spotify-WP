using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.Services
{
    public class CommentService
    {
        private readonly ICommentDAO _commentDAO;

        public CommentService(ICommentDAO commentDAO)
        {
            _commentDAO = commentDAO ?? throw new ArgumentNullException(nameof(commentDAO));
        }

        public Task<List<CommentDTO>> GetCommentsBySongIdAsync(string songId) =>
            _commentDAO.GetCommentsBySongIdAsync(songId);

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
}
