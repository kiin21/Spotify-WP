using Microsoft.UI.Xaml.Data;
using System;

namespace Spotify.Converters;

/// <summary>
/// Converts an index value to a string representation.
/// </summary>
public class IndexConverter : IValueConverter
{
    /// <summary>
    /// Converts an index value to a string representation.
    /// </summary>
    /// <param name="value">The index value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A string representation of the index value incremented by 1.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int index)
        {
            return (index + 1).ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// Converts a string representation back to an index value.
    /// </summary>
    /// <param name="value">The string representation to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>An index value.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}