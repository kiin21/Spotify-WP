using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.DAOs
{
    public class MockPlaybackControlDAO : IPlaybackControlDAO
    {
        private PlaybackStateDTO _currentState;
        private List<SongPlaybackDTO> _queue;
        private int _currentIndex;
        public MockPlaybackControlDAO() {

            _currentState = new PlaybackStateDTO
            {
                CurrentSongId = "1",
                IsPlaying = false,
                Volume = 50,
                PlaybackSpeed = "1.0x",
                CurrentPosition = TimeSpan.Zero,
                Duration = TimeSpan.FromMinutes(3.5),
                IsShuffleEnabled = false,
                IsRepeatEnabled = false
            };

            _queue = new List<SongPlaybackDTO>
            {
                new SongPlaybackDTO
                {
                    Id = "1",
                    Title = "Thien Ly Oi",
                    Artist = "J97",
                    ImageUrl = "../Assets/ThienLyOi_img.png",
                    Duration = TimeSpan.FromMinutes(3.5),
                    AudioUrl = "D:\\Download\\Music Audio\\ThienLyOi.mp3"
                },
                new SongPlaybackDTO
                {
                    Id = "2",
                    Title = "Want You",
                    Artist = "Anonymous",
                    ImageUrl = "../Assets/want_you_img.png",
                    Duration = TimeSpan.FromMinutes(3.5),
                    AudioUrl = "D:\\Download\\Music Audio\\Want_You.mp3"
                },
                // Add more mock songs here
            };

            _currentIndex = 0;
        }

        public async Task<PlaybackStateDTO> GetPlaybackStateAsync()
        {
            return await Task.FromResult(_currentState);
        }

        public async Task UpdatePlaybackStateAsync(PlaybackStateDTO state)
        {
            _currentState = state;
            await Task.CompletedTask;
        }

        public async Task<SongPlaybackDTO> GetCurrentSongAsync()
        {
            return await Task.FromResult(_queue[_currentIndex]);
        }

        public async Task<SongPlaybackDTO> GetNextSongAsync()
        {
            if (_currentIndex < _queue.Count - 1)
            {
                _currentIndex++;
            }
            else if (_currentState.IsRepeatEnabled)
            {
                _currentIndex = 0;
            }
            return await Task.FromResult(_queue[_currentIndex]);
        }

        public async Task<SongPlaybackDTO> GetPreviousSongAsync()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
            }
            else if (_currentState.IsRepeatEnabled)
            {
                _currentIndex = _queue.Count - 1;
            }
            return await Task.FromResult(_queue[_currentIndex]);
        }

        public async Task UpdateCurrentPositionAsync(TimeSpan position)
        {
            _currentState.CurrentPosition = position;
            await Task.CompletedTask;
        }

        public async Task<List<SongPlaybackDTO>> GetQueueAsync()
        {
            return await Task.FromResult(_queue.ToList());
        }

        public async Task ShuffleQueueAsync()
        {
            var currentSong = _queue[_currentIndex];
            var remainingSongs = _queue.Skip(_currentIndex + 1).ToList();

            // Fisher-Yates shuffle algorithm
            Random rng = new Random();
            int n = remainingSongs.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = remainingSongs[k];
                remainingSongs[k] = remainingSongs[n];
                remainingSongs[n] = value;
            }

            _queue = new List<SongPlaybackDTO> { currentSong };
            _queue.AddRange(remainingSongs);
            _currentIndex = 0;

            await Task.CompletedTask;
        }

        public async Task SetRepeatStateAsync(bool isEnabled)
    {
        await Task.Delay(50); // Simulate network delay
        _currentState.IsRepeatEnabled = isEnabled;
    }
    }
}
