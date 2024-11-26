using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Helpers;

// LyricLine.cs
public class LyricLine : INotifyPropertyChanged
{
    private string _text;
    private bool _isHighlighted = false;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set
        {
            _isHighlighted = value;
            OnPropertyChanged();
        }
    }

    public LyricLine(string text, bool isHighlighted)
    {
        Text = text;
        IsHighlighted = isHighlighted;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}