using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Weather2.Models;

namespace Weather2.Converters
{
    public class CondToTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Cond cond = value as Cond;
            if (parameter.ToString() == "dayImg")
            {
                string str = String.Format("ms-appx:///Assets/WeatherImages/{0}.png", cond.code_n);
                return str;
            }
            else if (parameter.ToString() == "nightImg")
            {
                string str = String.Format("ms-appx:///Assets/WeatherImages/{0}n.png", cond.code_n);
                //string str = String.Format("http://files.heweather.com/cond_icon/{0}.png", cond.code_n);
                return str;
            }
            else if (parameter.ToString() == "dayTxt")
            {
                return cond.txt_d;
            }            
            else
                return "失败";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
