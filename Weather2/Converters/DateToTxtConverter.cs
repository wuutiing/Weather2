using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Weather2.Converters
{
    public class DateToTxtConverter : IValueConverter
    {
        string[] week = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(parameter.ToString() == "daily")
            {
                string dateStr = value.ToString() + " 12:00:00";
                DateTime dt = System.Convert.ToDateTime(dateStr);
                string str = week[(int)dt.DayOfWeek];
                return str;
                
            }
            else if(parameter.ToString() == "time")
            {
                string str = value.ToString();
                string str2 = str.Substring(11);
                return str2;
            }
            
            return "失败啦";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
