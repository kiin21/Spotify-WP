//MockSongDAO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;

namespace Spotify.DAO
{
    public class MockSongDAO : ISongDAO
    {
        private List<SongDTO> _mockSongs = new List<SongDTO>
        {
            new SongDTO { Title = "Shape of You", Artist = "Ed Sheeran", Album = "Divide", Duration = TimeSpan.FromMinutes(3.5),  },
            new SongDTO { Title = "Blinding Lights", Artist = "The Weeknd", Album = "After Hours", Duration = TimeSpan.FromMinutes(4) },
            new SongDTO { Title = "Rolling in the Deep", Artist = "Adele", Album = "21", Duration = TimeSpan.FromMinutes(3.5) },
            new SongDTO { Title = "Bohemian Rhapsody", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromMinutes(6) },
            new SongDTO { Title = "Stairway to Heaven", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromMinutes(8) },
            new SongDTO { Title = "Billie Jean", Artist = "Michael Jackson", Album = "Thriller", Duration = TimeSpan.FromMinutes(4.5) },
            new SongDTO { Title = "Smells Like Teen Spirit", Artist = "Nirvana", Album = "Nevermind", Duration = TimeSpan.FromMinutes(5) },
            new SongDTO { Title = "Imagine", Artist = "John Lennon", Album = "Imagine", Duration = TimeSpan.FromMinutes(3) },
            new SongDTO { Title = "Hotel California", Artist = "Eagles", Album = "Hotel California", Duration = TimeSpan.FromMinutes(6.5) },
            new SongDTO { Title = "Lose Yourself", Artist = "Eminem", Album = "8 Mile", Duration = TimeSpan.FromMinutes(5) },
            new SongDTO { Title = "Rolling Stone", Artist = "Bob Dylan", Album = "Highway 61 Revisited", Duration = TimeSpan.FromMinutes(6) },
            new SongDTO { Title = "Take On Me", Artist = "A-ha", Album = "Hunting High and Low", Duration = TimeSpan.FromMinutes(3.5) },
            new SongDTO { Title = "Sweet Child O' Mine", Artist = "Guns N' Roses", Album = "Appetite for Destruction", Duration = TimeSpan.FromMinutes(6) },
            new SongDTO { Title = "Shake It Off", Artist = "Taylor Swift", Album = "1989", Duration = TimeSpan.FromMinutes(3.9) },
            new SongDTO { Title = "Wonderwall", Artist = "Oasis", Album = "(What's the Story) Morning Glory?", Duration = TimeSpan.FromMinutes(4.2) },
            new SongDTO { Title = "Uptown Funk", Artist = "Mark Ronson ft. Bruno Mars", Album = "Uptown Special", Duration = TimeSpan.FromMinutes(4.5) },
            new SongDTO { Title = "Let It Be", Artist = "The Beatles", Album = "Let It Be", Duration = TimeSpan.FromMinutes(4) },
            new SongDTO { Title = "Hey Jude", Artist = "The Beatles", Album = "Single", Duration = TimeSpan.FromMinutes(7) },
            new SongDTO { Title = "Purple Rain", Artist = "Prince", Album = "Purple Rain", Duration = TimeSpan.FromMinutes(8) },
            new SongDTO { Title = "Old Town Road", Artist = "Lil Nas X", Album = "7 EP", Duration = TimeSpan.FromMinutes(2.7) },
            new SongDTO { Title = "Bad Guy", Artist = "Billie Eilish", Album = "When We All Fall Asleep, Where Do We Go?", Duration = TimeSpan.FromMinutes(3) }
        };


        public Task<List<SongDTO>> SearchSongs(string query) =>
            Task.FromResult(_mockSongs.Where(s => s.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList());

        // Return all songs directly without using generics
        public Task<List<SongDTO>> GetAllSongs() =>
            Task.FromResult(_mockSongs.ToList());
    }
}
