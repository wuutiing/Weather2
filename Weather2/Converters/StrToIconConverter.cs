using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Weather2.Converters
{
    public class StrToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            
            if (localSettings.Values["FavoriteCities"] == null)
            {
                return 0;
            }
            else
            {
                string[] strs = (string[])localSettings.Values["FavoriteCities"];
                string stee = (string)value;
                bool sss = strs.Contains((string)value);
                if (strs.Contains((string)value))
                {
                    return 24;
                    
                }
                else
                {
                    return 0;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
