//using System;
//using System.Windows;
//using Microsoft.UI.Xaml.Data;

//namespace Spotify.Converters;

//public class SecondsToTimeStringConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, string language)
//    {
//        if (value is double seconds)
//        {
//            int totalSeconds = (int)Math.Floor(seconds);
//            int minutes = totalSeconds / 60;
//            int remainingSeconds = totalSeconds % 60;
//            return $"{minutes:D2}:{remainingSeconds:D2}";
//        }
//        return value?.ToString() ?? string.Empty;
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, string language)
//    {
//        if (value is string timeString)
//        {
//            string[] parts = timeString.Split(':');
//            if (parts.Length == 2 &&
//                int.TryParse(parts[0], out int minutes) &&
//                int.TryParse(parts[1], out int seconds))
//            {
//                return minutes * 60 + seconds;
//            }
//        }
//        return DependencyProperty.UnsetValue;
//    }
//}