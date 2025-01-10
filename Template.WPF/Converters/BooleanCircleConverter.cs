using System.Globalization;
using System.Windows.Data;

namespace Template.WPF.Converters
{
    public class BooleanCircleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "○";
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "○")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
