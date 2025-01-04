using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.Contracts.DAO
{
    public interface ICommentDAO : IDAO
    {
        Task<List<CommentDTO>> GetCommentsBySongIdAsync(string songId);
        Task AddCommentAsync(CommentDTO comment);
    }
}
