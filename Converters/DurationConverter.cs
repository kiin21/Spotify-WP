using Microsoft.UI.Xaml.Data;
using System;

namespace Spotify.Converters;

/// <summary>
/// Converts a duration in seconds to a formatted string.
/// </summary>
public class DurationConverter : IValueConverter
{
    /// <summary>
    /// Converts a duration in seconds to a formatted string.
    /// </summary>
    /// <param name="value">The duration in seconds to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A string representation of the duration in "m:ss" format.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int seconds)
        {
            // Convert seconds to TimeSpan
            TimeSpan duration = TimeSpan.FromSeconds(seconds);
            // Return formatted string in minutes:seconds
            return duration.ToString(@"m\:ss");
        }
        return string.Empty;
    }

    /// <summary>
    /// Converts a formatted string back to a duration in seconds.
    /// </summary>
    /// <param name="value">The formatted string to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A duration in seconds.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}