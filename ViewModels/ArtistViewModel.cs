using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// ViewModel for managing artist data and their songs.
/// </summary>
public class ArtistViewModel : INotifyPropertyChanged
{
    private ArtistDTO _artist;
    /// <summary>
    /// Gets or sets the artist data.
    /// </summary>
    public ArtistDTO Artist
    {
        get { return _artist; }
        set
        {
            _artist = value;
            OnPropertyChanged(); // Notify property change
        }
    }

    private ObservableCollection<SongDTO> _songs;
    /// <summary>
    /// Gets or sets the collection of songs by the artist.
    /// </summary>
    public ObservableCollection<SongDTO> Songs
    {
        get { return _songs; }
        set
        {
            _songs = value;
            OnPropertyChanged();
        }
    }

    private readonly ArtistService _artistService;
    private readonly SongService _songService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
    /// </summary>
    /// <param name="artistService">The service for managing artist data.</param>
    /// <param name="songService">The service for managing song data.</param>
    public ArtistViewModel(ArtistService artistService, SongService songService)
    {
        _artistService = artistService;
        _songService = songService;
    }

    /// <summary>
    /// Initializes the ViewModel with artist data and their songs.
    /// </summary>
    /// <param name="artistId">The ID of the artist to load.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InitializeAsync(string artistId)
    {
        Artist = await _artistService.GetArtistByIdAsync(artistId);

        if (Artist != null)
        {
            var songList = new ObservableCollection<SongDTO>();
            int counter = 1;

            foreach (var songId in Artist.SongIds)
            {
                var song = await _songService.GetSongByIdAsync(songId);
                if (song != null)
                {
                    song.Index = counter++; // Assign incremental index
                    songList.Add(song);
                }
            }

            Songs = songList; // Assign to update UI
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
