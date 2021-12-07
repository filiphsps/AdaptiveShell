using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class InvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var i = (double)value;
            return -i;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class FontSizeToHeightConverter : IValueConverter
    {
        private static double COEFF = 0.715;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value * COEFF;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class FontSizeToLineHeightConverter : IValueConverter
    {
        private static double COEFF = 0.875;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return double.Parse(value.ToString()) * COEFF;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
