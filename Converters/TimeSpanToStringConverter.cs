using Microsoft.UI.Xaml.Data;
using System;

namespace Spotify.Converters;

/// <summary>
/// Converts a TimeSpan value to a formatted string.
/// </summary>
public class TimeSpanToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts a TimeSpan value to a formatted string.
    /// </summary>
    /// <param name="value">The TimeSpan value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A string representation of the TimeSpan value in "mm:ss" format.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }
        return "00:00";
    }

    /// <summary>
    /// Converts a string representation back to a TimeSpan value.
    /// </summary>
    /// <param name="value">The string representation to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A TimeSpan value.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}