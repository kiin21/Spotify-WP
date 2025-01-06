using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Spotify.Converters;

/// <summary>
/// Converts a boolean value to a column span value.
/// </summary>
public class BoolToColumnSpanConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a column span value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>1 if the boolean value is true, otherwise 2.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (bool)value ? 1 : 2; // Return 1 when sidebar is visible, 2 when hidden
    }

    /// <summary>
    /// Converts a column span value back to a boolean value.
    /// </summary>
    /// <param name="value">The column span value to convert.</param>
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