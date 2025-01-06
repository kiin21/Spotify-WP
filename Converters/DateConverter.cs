using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace Spotify.Converters;

/// <summary>
/// Converts a DateTime value to a formatted string.
/// </summary>
public class DateConverter : IValueConverter
{
    /// <summary>
    /// Converts a DateTime value to a formatted string.
    /// </summary>
    /// <param name="value">The DateTime value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A string representation of the DateTime value in "dd/MM/yyyy" format.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime date)
        {
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        return string.Empty;
    }

    /// <summary>
    /// Converts a string value back to a DateTime value.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A DateTime value.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}