using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Spotify.Converters;

/// <summary>
/// Converts a boolean value to a SolidColorBrush for highlighting the current song.
/// </summary>
public class CurrentSongHighlightConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a SolidColorBrush.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A SolidColorBrush with the color LightBlue if the boolean value is true, otherwise Transparent.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (bool)value ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Transparent);
    }

    /// <summary>
    /// Converts a SolidColorBrush back to a boolean value.
    /// </summary>
    /// <param name="value">The SolidColorBrush value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A boolean value.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}



