using Spotify.Models.DTOs;
using Spotify.Services;
using Spotify;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

public partial class QueueViewModel : INotifyPropertyChanged
{
    private static QueueViewModel _instance;
    private static readonly object _lock = new object(); // Lock object for thread safety

    public event PropertyChangedEventHandler PropertyChanged;
    private readonly QueueService _queueService;
    private ObservableCollection<SongDTO> _queue;
    private SongDTO _selectedSong;

    public ObservableCollection<SongDTO> Queue
    {
        get => _queue;
        set
        {
            _queue = value;
            OnPropertyChanged();
        }
    }

    public SongDTO SelectedSong
    {
        get => _selectedSong;
        set
        {
            _selectedSong = value;
            OnPropertyChanged();
        }
    }

    // Private constructor to prevent instantiation from outside
    private QueueViewModel(QueueService queueService)
    {
        _queueService = queueService;
    }

    // Public property to get the single instance of QueueViewModel
    public static QueueViewModel Instance
    {
        get
        {
            lock (_lock)
            {
                // Only create the instance if it doesn't exist yet
                if (_instance == null)
                {
                    // Use dependency injection to pass the necessary service
                    var queueService = App.Current.Services.GetRequiredService<QueueService>();
                    _instance = new QueueViewModel(queueService);
                }
                return _instance;
            }
        }
    }

    // Method to load the queue asynchronously
    public async Task LoadQueueAsync()
    {
        var queue = await _queueService.GetQueue();
        Queue = new ObservableCollection<SongDTO>(queue);
    }

    // Notify changes in properties
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
