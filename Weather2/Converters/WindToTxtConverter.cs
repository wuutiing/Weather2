using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Weather2.Models;

namespace Weather2.Converters
{
    public class WindToTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Wind2 w2 = value as Wind2;
            string str = w2.dir + "  " + w2.sc;
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;

        }
    }
}
