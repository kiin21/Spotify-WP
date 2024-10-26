using Microsoft.UI.Xaml.Data;
using System;

namespace Spotify.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Try to cast parameter as index (if bound correctly)
            if (parameter is int index)
            {
                // Increment index by 1 for display purposes
                return (index + 1).ToString();
            }
            return "1"; // Default fallback if no index provided
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
