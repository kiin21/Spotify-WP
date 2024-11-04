using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;
using Windows.UI;

namespace Spotify.Helpers
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked)
            {
                if (isChecked)
                {
                    // Red color
                    return new SolidColorBrush(Color.FromArgb(255, 29, 185, 84));
                }
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}