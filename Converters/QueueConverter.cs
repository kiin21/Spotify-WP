//using Microsoft.UI.Xaml;
//using Microsoft.UI.Xaml.Data;
//using System;

//namespace Spotify.Converter
//{
//    public class HeightConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            if (value == null)
//                return "1*"; // Return as string instead of GridLength

//            if (value is bool isQueueVisible)
//            {
//                return isQueueVisible ? "Auto" : "1*";
//            }

//            return "1*"; // Default fallback
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class AlignmentConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            if (value == null)
//                return "Center"; // Return as string instead of VerticalAlignment

//            if (value is bool isQueueVisible)
//            {
//                return isQueueVisible ? "Stretch" : "Center";
//            }

//            return "Center"; // Default fallback
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class ImageSizeConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            if (value == null)
//                return 120; // Numbers can be returned directly as they're primitive types

//            if (value is bool isQueueVisible)
//            {
//                return isQueueVisible ? 120 : 300;
//            }

//            return 120; // Default fallback
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class BoolNegationConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            if (value is bool boolValue)
//            {
//                return !boolValue;
//            }
//            return false;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}