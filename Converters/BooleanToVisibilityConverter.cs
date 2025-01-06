﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Spotify.Converters;

/// <summary>
/// Converts a boolean value to a Visibility value.
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a Visibility value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter to invert the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>Visibility.Visible if the boolean value is true, otherwise Visibility.Collapsed.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            bool invert = parameter?.ToString() == "Invert";
            return (boolValue != invert) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    /// <summary>
    /// Converts a Visibility value back to a boolean value.
    /// </summary>
    /// <param name="value">The Visibility value to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter for the conversion.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>True if the Visibility value is Visibility.Visible, otherwise false.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return (value is Visibility visibility) && (visibility == Visibility.Visible);
    }
}