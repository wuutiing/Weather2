using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Weather2.Converters
{
    public class StrToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str = (string)parameter;
            if (str == "hourlyF")
            {
                if ((string)value == "special")
                {
                    return 100;
                }
                else return 0;
            }
            else if( str == "dailyF")
            {
                if((string)value == "special")
                {
                    Windows.UI.Xaml.Media.SolidColorBrush scb = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(159, 255, 255, 255));
                    return scb;
                }
                else
                {
                    Windows.UI.Xaml.Media.SolidColorBrush scb = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(159, 0, 255, 255));
                    return scb;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
