//PlaylistSongDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class PlaylistSongDAO : BaseDAO, IPlaylistSongDAO
{
    private readonly IMongoCollection<PlaylistSongDTO> _playlistSongs;

    public PlaylistSongDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playlistSongs = database.GetCollection<PlaylistSongDTO>("PlaylistSong");
    }

    public async Task<List<PlaylistSongDTO>> GetSongsByPlaylistIdAsync(string playlistId)
    {
        return await _playlistSongs.Find(song => song.PlaylistId == playlistId).ToListAsync();
    }

}