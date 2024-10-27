using Spotify.Models.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Spotify.ViewModels;

public class SearchResultPageViewModel : INotifyPropertyChanged
{
    private ObservableCollection<SongDTO> _searchResults;

    public ObservableCollection<SongDTO> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged(nameof(SearchResults));
        }
    }

    public SearchResultPageViewModel(ObservableCollection<SongDTO> searchResults)
    {
        SearchResults = searchResults;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
