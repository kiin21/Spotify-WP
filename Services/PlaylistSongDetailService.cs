// PlaylistSongDetailService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

/// <summary>
/// Service for handling playlist song details.
/// </summary>
public class PlaylistSongDetailService
{
    private readonly IPlaylistSongDetailDAO _playlistSongDetailDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSongDetailService"/> class.
    /// </summary>
    /// <param name="playlistSongDetailDAO">The playlist song detail DAO.</param>
    public PlaylistSongDetailService(IPlaylistSongDetailDAO playlistSongDetailDAO)
    {
        _playlistSongDetailDAO = playlistSongDetailDAO;
    }

    /// <summary>
    /// Gets the details of songs in a playlist asynchronously.
    /// </summary>
    /// <param name="playlistId">The playlist identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="PlaylistSongDetailDTO"/>.</returns>
    public async Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId)
    {
        return await _playlistSongDetailDAO.GetPlaylistSongDetailAsync(playlistId);
    }
}
