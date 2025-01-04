// PlaylistSongDetailDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify.DAOs;

/// <summary>
/// Data Access Object for Playlist Song Details
/// </summary>
public class PlaylistSongDetailDAO : BaseDAO, IPlaylistSongDetailDAO
{
    private readonly IMongoCollection<PlaylistSongDTO> _playlistSongs;
    private readonly IMongoCollection<SongDTO> _songs;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistSongDetailDAO"/> class.
    /// </summary>
    public PlaylistSongDetailDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playlistSongs = database.GetCollection<PlaylistSongDTO>("PlaylistSong");
        _songs = database.GetCollection<SongDTO>("Songs"); // Collection containing song data
    }

    /// <summary>
    /// Retrieves playlist song details by playlist ID.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of PlaylistSongDetailDTO.</returns>
    public async Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId)
    {
        // Get all PlaylistSong based on playlistId
        var playlistSongs = await _playlistSongs
            .Find(song => song.PlaylistId == playlistId)
            .ToListAsync();

        // Get the list of SongIds from PlaylistSong for join
        var songIds = playlistSongs.Select(ps => ps.SongId).ToList();

        // Get data from the Song collection with SongIds from PlaylistSong
        var songs = await _songs
            .Find(song => songIds.Contains(song.Id.ToString()))
            .ToListAsync();

        // Join PlaylistSong data with Song based on SongId
        var joinedData = (from ps in playlistSongs
                          join s in songs on ps.SongId equals s.Id.ToString()
                          select new PlaylistSongDetailDTO
                          {
                              PlaylistSongId = ps.Id.ToString(),
                              SongId = s.Id.ToString(),
                              SongTitle = s.title,
                              Avatar = s.CoverArtUrl,
                              Artist = s.ArtistName,
                              AddedAt = ps.AddedAt,
                              AddedBy = ps.AddedBy,
                              Duration = s.Duration,
                          }).ToList();

        return joinedData;
    }

    /// <summary>
    /// Deletes a song from a playlist.
    /// </summary>
    /// <param name="playlistId">The ID of the playlist.</param>
    /// <param name="songId">The ID of the song.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteOneAsync(string playlistId, string songId)
    {
        var filter = Builders<PlaylistSongDTO>.Filter.And(
            Builders<PlaylistSongDTO>.Filter.Eq(ps => ps.PlaylistId, playlistId),
            Builders<PlaylistSongDTO>.Filter.Eq(ps => ps.SongId, songId)
        );

        await _playlistSongs.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Inserts a new song into a playlist.
    /// </summary>
    /// <param name="playlistSong">The playlist song to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InsertOneAsync(PlaylistSongDTO playlistSong)
    {
        await _playlistSongs.InsertOneAsync(playlistSong);
    }
}
