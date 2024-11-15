using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.Models.DTOs;

namespace Spotify.ViewModels;

public class LyricViewModel
{
    public SongDTO _song { get; set; } = new SongDTO();
    LyricViewModel(SongDTO song)
    {
        _song = song;
    }
}
