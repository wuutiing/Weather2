using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Weather2.Models;

namespace Weather2.Converters
{
    public class tempToTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Tmp tmp = value as Tmp;
            string str = tmp.max + "/" + tmp.min + "℃";
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
