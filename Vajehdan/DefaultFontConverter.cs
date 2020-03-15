using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Vajehdan
{
    public class DefaultFontConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value.ToString().Contains("فونت پیش‌فرض") || string.IsNullOrEmpty(value.ToString()) ? parameter : value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}