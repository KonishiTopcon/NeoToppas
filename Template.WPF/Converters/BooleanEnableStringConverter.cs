using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Template.WPF.Converters
{
    public class BooleanEnableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool))
            {
                return string.Empty;
            }
            if ((bool)value)
            {
                return "有効";
            }
            else
            {
                return "無効";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "有効")
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
