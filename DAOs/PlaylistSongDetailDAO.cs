// PlaylistSongDetailDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

public class PlaylistSongDetailDAO : BaseDAO, IPlaylistSongDetailDAO
{
    private readonly IMongoCollection<PlaylistSongDTO> _playlistSongs;
    private readonly IMongoCollection<SongDTO> _songs;

    public PlaylistSongDetailDAO()
    {
        var database = connection.GetDatabase("SpotifineDB");
        _playlistSongs = database.GetCollection<PlaylistSongDTO>("PlaylistSong");
        _songs = database.GetCollection<SongDTO>("Songs"); // Collection chứa dữ liệu bài hát
    }

    public async Task<List<PlaylistSongDetailDTO>> GetPlaylistSongDetailAsync(string playlistId)
    {
        // Lấy tất cả PlaylistSong dựa trên playlistId
        var playlistSongs = await _playlistSongs
            .Find(song => song.PlaylistId == playlistId)
            .ToListAsync();

        // Lấy danh sách các SongId từ PlaylistSong để dùng trong join
        var songIds = playlistSongs.Select(ps => ps.SongId).ToList();

        // Lấy dữ liệu từ collection Song với các SongId lấy từ PlaylistSong
        var songs = await _songs
            .Find(song => songIds.Contains(song.Id.ToString()))
            .ToListAsync();

        // Join dữ liệu PlaylistSong với Song dựa trên SongId
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
}
