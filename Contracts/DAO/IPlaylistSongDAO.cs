using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Contracts.DAO;

/// <summary>
/// Interface for Playlist Song Data Access Object
/// </summary>
public interface IPlaylistSongDAO : IDAO
{
    /// <summary>
    /// Retrieves songs by playlist ID.
    /// </summary>
    /// <param name="id">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistSongDTO.</returns>
    Task<List<PlaylistSongDTO>> GetSongsByPlaylistIdAsync(string id);
}


