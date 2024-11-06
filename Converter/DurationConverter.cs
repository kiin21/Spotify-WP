using Microsoft.UI.Xaml.Data;
using System;

namespace Spotify.Converters
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int seconds)
            {
                // Chuyển đổi giây thành TimeSpan
                TimeSpan duration = TimeSpan.FromSeconds(seconds);
                // Trả về chuỗi định dạng phút:giây
                return duration.ToString(@"m\:ss");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
