using Spotify.Models.DTOs;
using Spotify.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class ArtistViewModel : INotifyPropertyChanged
{
    private ArtistDTO _artist;
    public ArtistDTO Artist
    {
        get { return _artist; }
        set
        {
            _artist = value;
            OnPropertyChanged(); // Thông báo thay đổi thuộc tính
        }
    }

    private ObservableCollection<SongDTO> _songs;
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

    public ArtistViewModel(ArtistService artistService, SongService songService)
    {
        _artistService = artistService;
        _songService = songService;
    }

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
                    song.Index = counter++; // Gán chỉ mục tăng dần
                    songList.Add(song);
                }
            }

            Songs = songList; // Gán lại để UI cập nhật
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
