using Microsoft.UI.Xaml.Data;
using System;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class InvertConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            Double i = (Double)value;
            return -i;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed partial class FontSizeToHeightConverter : IValueConverter
    {
        private static Double COEFF = 0.715;

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            return (Double)value * COEFF;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed partial class FontSizeToLineHeightConverter : IValueConverter
    {
        private static Double COEFF = 0.875;

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            return Double.Parse(value.ToString()) * COEFF;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
