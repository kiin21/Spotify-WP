using Spotify.Contracts.DAO;
using Spotify.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.DAOs;

public class MockPlaybackControlDAO : IPlaybackControlDAO
{
    private PlaybackStateDTO _currentState;
    private List<SongPlaybackDTO> _queue;
    private int _currentIndex;
    private List<int> _shuffleOrder;

    public MockPlaybackControlDAO()
    {

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
                    ImageUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/image%2FThienLyOi_image.png?alt=media&token=40086374-0835-44b4-8a46-2a614c53d6fe",
                    Duration = TimeSpan.FromSeconds(89),
                    AudioUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FThienLyOi.mp3?alt=media&token=d4ac1dc1-089d-4a41-9830-8e8ac96186e5"
                },
                new SongPlaybackDTO
                {
                    Id = "2",
                    Title = "Want You",
                    Artist = "Anonymous",
                    ImageUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/image%2Fwant_you_img.png?alt=media&token=60af2195-fe8e-4a5d-b60a-20601b63048a",
                    Duration = TimeSpan.FromSeconds(139),
                    AudioUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FWant_You.mp3?alt=media&token=dfd5dc70-4cc0-4d7d-84fa-4fb58ed033de"
                },
                new SongPlaybackDTO
                {
                    Id = "3",
                    Title = "Dong Kiem Em",
                    Artist = "Vu",
                    ImageUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/image%2FDongKiemEm.png?alt=media&token=5df2dd39-c3c5-48d0-a2f3-dac038402015",
                    Duration = TimeSpan.FromSeconds(246),
                    AudioUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FDongKiemEm.mp3?alt=media&token=50fad1ac-d1fb-401c-a103-7b25fa97e84f"
                },
                new SongPlaybackDTO
                {
                    Id = "4",
                    Title = "Cat Bui",
                    Artist = "Khanh Ly",
                    ImageUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/image%2FCatBui.png?alt=media&token=2dccb349-62b4-41ad-adb1-bce3055becc0",
                    Duration = TimeSpan.FromSeconds(209),
                    AudioUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FCatBui.mp3?alt=media&token=6fffef62-1e57-46a6-bcf4-8f2a0f33dcd5"
                },
                new SongPlaybackDTO
                {
                    Id = "5",
                    Title = "Phoi Pha",
                    Artist = "Tuan Ngoc",
                    ImageUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/image%2FPhoiPha.png?alt=media&token=a06f42bf-88c9-4149-8438-fc5fe3bb9ea0",
                    Duration = TimeSpan.FromSeconds(196),
                    AudioUrl = "https://firebasestorage.googleapis.com/v0/b/my-firebase-e3f67.appspot.com/o/audio%2FPhoiPha.mp3?alt=media&token=4e4de9fe-903e-4e19-9561-23a8df47eb60"
                },
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
                //stay unchanged
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

    public async Task SetRepeatStateAsync(bool isEnabled)
    {
        await Task.Delay(50); // Simulate network delay
        _currentState.IsRepeatEnabled = isEnabled;
    }

    public async Task AddToQueueAsync(SongPlaybackDTO song)
    {
        _queue.Add(song);
        await Task.CompletedTask;
    }

    public async Task AddToHeadOfQueueAsync(SongPlaybackDTO song)
    {
        _queue.Insert(0, song);
        await Task.CompletedTask;
    }
    public async Task AddToNextInQueueAsync(SongPlaybackDTO song)
    {
        _queue.Insert(_currentIndex + 1, song);
        await Task.CompletedTask;
    }
}
