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
        private List<int> _shuffleOrder;

        public MockPlaybackControlDAO() {

            _currentState = new PlaybackStateDTO
            {
                CurrentSongId = "1",
                IsPlaying = false,
                Volume = 50,
                PlaybackSpeed = "1.0x",
                CurrentPosition = TimeSpan.Zero,
                Duration = TimeSpan.FromSeconds(60),
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
                    Duration = TimeSpan.FromSeconds(89),
                    AudioUrl = "D:\\Download\\Music Audio\\ThienLyOi.mp3"
                },
                new SongPlaybackDTO
                {
                    Id = "2",
                    Title = "Want You",
                    Artist = "Anonymous",
                    ImageUrl = "../Assets/want_you_img.png",
                    Duration = TimeSpan.FromSeconds(139),
                    AudioUrl = "D:\\Download\\Music Audio\\Want_You.mp3"
                },
                new SongPlaybackDTO
                {
                    Id = "3",
                    Title = "Dong Kiem Em",
                    Artist = "Vu",
                    ImageUrl = "../Assets/DongKiemEm.png",
                    Duration = TimeSpan.FromSeconds(246),
                    AudioUrl = "D:\\Download\\Music Audio\\DongKiemEm.mp3"
                },
                new SongPlaybackDTO
                {
                    Id = "4",
                    Title = "Cat Bui",
                    Artist = "Khanh Ly",
                    ImageUrl = "../Assets/CatBui.png",
                    Duration = TimeSpan.FromSeconds(209),
                    AudioUrl = "D:\\Download\\Music Audio\\CatBui.mp3"
                },
                new SongPlaybackDTO
                {
                    Id = "5",
                    Title = "Phoi Pha",
                    Artist = "Tuan Ngoc",
                    ImageUrl = "../Assets/PhoiPha.png",
                    Duration = TimeSpan.FromSeconds(196),
                    AudioUrl = "D:\\Download\\Music Audio\\PhoiPha.mp3"
                }
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

        //public async Task<SongPlaybackDTO> GetNextSongAsync()
        //{
        //    if (_currentIndex < _queue.Count - 1)
        //    {
        //        _currentIndex++;
        //    }
        //    else if (_currentState.IsRepeatEnabled)
        //    {
        //        _currentIndex = 0;
        //    }
        //    return await Task.FromResult(_queue[_currentIndex]);
        //}

        //public async Task<SongPlaybackDTO> GetPreviousSongAsync()
        //{
        //    if (_currentIndex > 0)
        //    {
        //        _currentIndex--;
        //    }
        //    else if (_currentState.IsRepeatEnabled)
        //    {
        //        _currentIndex = _queue.Count - 1;
        //    }
        //    return await Task.FromResult(_queue[_currentIndex]);
        //}

    public async Task<SongPlaybackDTO> GetNextSongAsync()
    {
        if (_queue == null || _queue.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        if (_currentState.IsShuffleEnabled)
        {
            // Initialize shuffle order if needed
            if (_shuffleOrder == null || _shuffleOrder.Count == 0)
            {
                await ShuffleQueueAsync();
            }

            int currentShuffleIndex = _shuffleOrder.IndexOf(_currentIndex);
            if (currentShuffleIndex < _shuffleOrder.Count - 1)
            {
                _currentIndex = _shuffleOrder[currentShuffleIndex + 1];
            }
            else if (_currentState.IsRepeatEnabled)
            {
                // Reshuffle and start from beginning when both shuffle and repeat are enabled
                await ShuffleQueueAsync();
                _currentIndex = _shuffleOrder[0];
            }
        }
        else
        {
            if (_currentIndex < _queue.Count - 1)
            {
                _currentIndex++;
            }
            else if (_currentState.IsRepeatEnabled)
            {
                _currentIndex = 0;
            }
            // If neither condition is met, stay on current song
        }

        _currentState.CurrentSongId = _queue[_currentIndex].Id;
        _currentState.CurrentPosition = TimeSpan.Zero;
        return await Task.FromResult(_queue[_currentIndex]);
    }

    public async Task<SongPlaybackDTO> GetPreviousSongAsync()
    {
        if (_queue == null || _queue.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        if (_currentState.IsShuffleEnabled)
        {
            // Initialize shuffle order if needed
            if (_shuffleOrder == null || _shuffleOrder.Count == 0)
            {
                await ShuffleQueueAsync();
            }

            int currentShuffleIndex = _shuffleOrder.IndexOf(_currentIndex);
            if (currentShuffleIndex > 0)
            {
                _currentIndex = _shuffleOrder[currentShuffleIndex - 1];
            }
            else if (_currentState.IsRepeatEnabled)
            {
                _currentIndex = _shuffleOrder[_shuffleOrder.Count - 1];
            }
        }
        else
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
            }
            else if (_currentState.IsRepeatEnabled)
            {
                _currentIndex = _queue.Count - 1;
            }
            // If neither condition is met, stay on current song
        }

        _currentState.CurrentSongId = _queue[_currentIndex].Id;
        _currentState.CurrentPosition = TimeSpan.Zero;
        return await Task.FromResult(_queue[_currentIndex]);
    }

    public async Task ShuffleQueueAsync()
    {
        if (_queue == null || _queue.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        _shuffleOrder = Enumerable.Range(0, _queue.Count).ToList();

        // Fisher-Yates shuffle algorithm (excluding current song)
        Random rng = new Random();
        int currentSongIndex = _currentIndex;

        // Move current song to start of shuffle order
        int currentOrderIndex = _shuffleOrder.IndexOf(currentSongIndex);
        if (currentOrderIndex > 0)
        {
            _shuffleOrder[currentOrderIndex] = _shuffleOrder[0];
            _shuffleOrder[0] = currentSongIndex;
        }

        // Shuffle remaining indices
        for (int i = _queue.Count - 1; i > 1; i--)
        {
            int k = rng.Next(1, i + 1); // Start from 1 to preserve current song
            int temp = _shuffleOrder[k];
            _shuffleOrder[k] = _shuffleOrder[i];
            _shuffleOrder[i] = temp;
        }

        _currentIndex = currentSongIndex;
        await Task.CompletedTask;
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

        //public async Task ShuffleQueueAsync()
        //{
        //    var currentSong = _queue[_currentIndex];
        //    var remainingSongs = _queue.Skip(_currentIndex + 1).ToList();

        //    // Fisher-Yates shuffle algorithm
        //    Random rng = new Random();
        //    int n = remainingSongs.Count;
        //    while (n > 1)
        //    {
        //        n--;
        //        int k = rng.Next(n + 1);
        //        var value = remainingSongs[k];
        //        remainingSongs[k] = remainingSongs[n];
        //        remainingSongs[n] = value;
        //    }

        //    _queue = new List<SongPlaybackDTO> { currentSong };
        //    _queue.AddRange(remainingSongs);
        //    _currentIndex = 0;

        //    await Task.CompletedTask;
        //}

        public async Task SetRepeatStateAsync(bool isEnabled)
    {
        await Task.Delay(50); // Simulate network delay
        _currentState.IsRepeatEnabled = isEnabled;
    }
    }
}
