// PlaylistSongDetailService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

public class PlaylistSongDetailService
{
    private readonly IPlaylistSongDetailDAO _playlistSongDetailDAO;

    public PlaylistSongDetailService(IPlaylistSongDetailDAO playlistSongDetailDAO)
    {
        _playlistSongDetailDAO = playlistSongDetailDAO;
    }

    public async Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId)
    {
        return await _playlistSongDetailDAO.GetPlaylistSongDetailAsync(playlistId);
    }


}
