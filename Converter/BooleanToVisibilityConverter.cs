﻿using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;
using System.Windows;


namespace Spotify.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility visibility) && (visibility == Visibility.Visible);
        }
    }
}
