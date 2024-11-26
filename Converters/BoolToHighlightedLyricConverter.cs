// BoolToHighlightedLyricConverter.cs
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;

namespace Spotify.Converters;

public class BoolToHighlightedLyricConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isHighlighted)
        {
            return isHighlighted ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
