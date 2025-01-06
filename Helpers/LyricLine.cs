using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Helpers;

/// <summary>
/// Represents a line of lyrics with text and highlight state.
/// </summary>
public class LyricLine : INotifyPropertyChanged
{
    private string _text;
    private bool _isHighlighted = false;

    /// <summary>
    /// Gets or sets the text of the lyric line.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the lyric line is highlighted.
    /// </summary>
    public bool IsHighlighted
    {
        get => _isHighlighted;
        set
        {
            _isHighlighted = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricLine"/> class with the specified text and highlight state.
    /// </summary>
    /// <param name="text">The text of the lyric line.</param>
    /// <param name="isHighlighted">A value indicating whether the lyric line is highlighted.</param>
    public LyricLine(string text, bool isHighlighted)
    {
        Text = text;
        IsHighlighted = isHighlighted;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
