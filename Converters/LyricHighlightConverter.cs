//using System;
//using Microsoft.UI;
//using Microsoft.UI.Xaml.Data;
//using Microsoft.UI.Xaml.Media;
//using Windows.UI;

//namespace Spotify.Converters;

//public class LyricHighlightConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, string language)
//    {
//        if (value is bool isCurrentLyric && isCurrentLyric)
//        {
//            // Highlighted color (bright white)
//            return new SolidColorBrush(Colors.White);
//        }

//        // Default color (light gray)
//        return new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, string language)
//    {
//        throw new NotImplementedException();
//    }
//}