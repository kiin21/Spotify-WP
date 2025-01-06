//BoolToColorConverter.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Spotify.Models.DTOs;
using System;

namespace Spotify.Converters;

/// <summary>
/// Converts a boolean value to a SolidColorBrush.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a SolidColorBrush.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>A SolidColorBrush with the color Green if the boolean value is true, otherwise Gray.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (bool)value ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Gray);
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