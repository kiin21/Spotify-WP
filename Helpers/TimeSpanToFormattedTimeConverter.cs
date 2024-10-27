using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Spotify.Helpers
{
    public class TimeSpanToFormattedTimeConverter : IValueConverter
    {
        // Convert TimeSpan to formatted string (mm:ss)
        public object Convert(object value, Type targetType, object parameter, 
            CultureInfo cultureInfo)
        {
            if (value is TimeSpan timeSpan)
            {
                // Format TimeSpan to "mm:ss" format
                return timeSpan.ToString(@"mm\:ss");
            }
            return "00:00"; // Fallback for invalid input
        }

        // ConvertBack is not needed in this case, so just return the original value.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException(); // Not necessary for OneWay binding
        }

    }

}
