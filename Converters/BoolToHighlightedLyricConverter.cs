// BoolToHighlightedLyricConverter.cs
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;

namespace Spotify.Converters;

/// <summary>
/// Converts a boolean value to a SolidColorBrush for highlighting lyrics.
/// </summary>
public class BoolToHighlightedLyricConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a SolidColorBrush.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A SolidColorBrush with the color Black if the boolean value is true, otherwise White.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isHighlighted)
        {
            return isHighlighted ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }
        return new SolidColorBrush(Colors.White);
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