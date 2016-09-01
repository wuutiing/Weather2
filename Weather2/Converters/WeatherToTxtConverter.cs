using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Weather2.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Weather2.Converters
{
    public class WeatherToTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            HeWeathedataService30[] w = (HeWeathedataService30[])value;
            string par = parameter.ToString();
            if (par == "tianqi")
            {
                string str = w[0].now.cond.txt;
                return str;

            }
            else if (par == "tupian")
            {
                string str;
                int see = DateTime.Now.Hour;
                if (DateTime.Now.Hour >= 20 || DateTime.Now.Hour < 8)
                {
                    str = String.Format("ms-appx:///Assets/WeatherImages/{0}n.png", w[0].now.cond.code);
                }
                else
                {
                    str = String.Format("ms-appx:///Assets/WeatherImages/{0}.png", w[0].now.cond.code);
                }                   
                BitmapImage bi = new BitmapImage(new Uri(str));

                return bi;

            }
            else if(par == "back")
            {
                string str;
                if (DateTime.Now.Hour >= 20 || DateTime.Now.Hour < 8)
                {
                    str = String.Format("ms-appx:///Assets/WeatherBackImages/{0}n.png", w[0].now.cond.code);
                }
                else
                {
                    str = String.Format("ms-appx:///Assets/WeatherBackImages/{0}.png", w[0].now.cond.code);
                }
                BitmapImage bi = new BitmapImage(new Uri(str));

                return bi; ;
            }
            else if(par == "wendu")
            {
                string str = w[0].now.tmp + "℃";
                return str;

            }
            else if(par == "chengshi")
            {
                string str = w[0].basic.city;
                return str;
            }
            return "失败啦";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
