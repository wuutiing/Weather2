using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Weather2.Models;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections;
using Windows.UI.Popups;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Weather2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WeatherPage : Page
    {
        List<DailyForecast> dailyForecast = new List<DailyForecast>();
        List<HourlyForecast> hourlyForecast = new List<HourlyForecast>();
        Hashtable ht = CityProxy.GetPopularCityBackground();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public WeatherPage()
        {
            this.InitializeComponent();
            if(localSettings.Values["NowDisplayCity"] != null)
            {
                if (ht.ContainsKey(localSettings.Values["NowDisplayCity"].ToString()))
                {
                    ImageBrush ib = new ImageBrush();
                    string tise = ht[localSettings.Values["NowDisplayCity"].ToString()].ToString();
                    ib.ImageSource = new BitmapImage(new Uri(String.Format("ms-appx:///" + tise), UriKind.Absolute));
                    ib.Stretch = Stretch.UniformToFill;
                    myGrid2.Background = ib;

                }
                else
                {
                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/CityImages/other.jpg", UriKind.Absolute));
                    ib.Stretch = Stretch.UniformToFill;
                    myGrid2.Background = ib;

                }               
            }
            
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //if ( e.Parameter != null )
            //{
            //    Weather myWeather = (Weather)e.Parameter;                   
            //    PaintPage(myWeather);

            //}
            //else
            {
                if(localSettings.Values["NowDisplayCity"] != null)
                {
                    Weather myWeather = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[localSettings.Values["NowDisplayCity"]]);
                    PaintPage(myWeather);

                }
                else
                {
                    var messageDialog = new MessageDialog(String.Format("大气探测装置出现存储故障，请重启。"))
                    {
                        Title = "告诉你一个坏消息"
                    };
                    messageDialog.Commands.Add(new UICommand("关闭", cmd => { }, commandId: 0));
                    await messageDialog.ShowAsync();

                }
            }
            
            
        }
        
        private void leftButton_Click(object sender, RoutedEventArgs e)
        {
            rightButton.IsEnabled = true;
            ScrollViewer sv = FindVisualChildByName<ScrollViewer>(myItemsControl, "scrollViewer2");
            double d = sv.HorizontalOffset;
            double q = sv.ActualWidth;
            sv.ChangeView(d + 100, null, null);
            if ((700 - sv.HorizontalOffset - q) <= 100)
            {
                leftButton.IsEnabled = false;
            }
            
        }

        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            leftButton.IsEnabled = true;
            ScrollViewer sv = FindVisualChildByName<ScrollViewer>(myItemsControl, "scrollViewer2");
            double d = sv.HorizontalOffset;
            sv.ChangeView(d - 100, null, null);
            if (sv.HorizontalOffset <= 100)
            {
                rightButton.IsEnabled = false;                
            }
        }

        private void PaintPage(Weather myWeather)
        {
            
            PaintNowWeather(myWeather);
            PaintDailyForecast(myWeather);
            PainHourlyForecast(myWeather);
            
        }

        private void PaintNowWeather(Weather myWeather)
        {            
            if (DateTime.Now.Hour >= 20 || DateTime.Now.Hour < 8)
            {
                WeatherIcon.Source = new BitmapImage(new Uri(String.Format("ms-appx:///Assets/WeatherImages/{0}n.png", myWeather.data[0].now.cond.code)));

            }
            else
            {
                WeatherIcon.Source = new BitmapImage(new Uri(String.Format("ms-appx:///Assets/WeatherImages/{0}.png", myWeather.data[0].now.cond.code)));

            }
            Temperature.Text = myWeather.data[0].now.tmp + "℃";

        }

        private void PaintDailyForecast(Weather myWeather)
        {
            int i = myWeather.data[0].daily_forecast.Count();
            for(int j =0; j < i; j++)
            {   
                if(j == 0)
                {
                    myWeather.data[0].daily_forecast[0].pres = "special";
                }             
                dailyForecast.Add(myWeather.data[0].daily_forecast[j]);
            }
            //foreach (DailyForecast df in myWeather.data[0].daily_forecast)
            //{
            //    dailyForecast.Add(df);

            //}
            myItemsControl.ItemsSource = dailyForecast;

        }

        private void PainHourlyForecast(Weather myWeather)
        {
            int i = myWeather.data[0].hourly_forecast.Count();
            for (int j = 0; j < i; j++) 
            {
                if (j == 0)
                {
                    myWeather.data[0].hourly_forecast[0].pres = "special";
                }

                hourlyForecast.Add(myWeather.data[0].hourly_forecast[j]);                   

                               
            }            
            myItemsControl2.ItemsSource = hourlyForecast;

        }
        
        //重要，遍历visualtree，按名查找指定对象
        private static ScrollViewer FindVisualChildByName<ScrollViewer>(DependencyObject parent, string name) where ScrollViewer : DependencyObject
        {
            try
            {
                int j = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if ((string.IsNullOrEmpty(name) || controlName == name) && child is ScrollViewer)
                    {
                        return child as ScrollViewer;

                    }
                    else
                    {
                        ScrollViewer result = FindVisualChildByName<ScrollViewer>(child, name);
                        if (result != null)
                            return result;

                    }
                }
                return null;

            }
            catch
            {
                return null;

            }
        }

        private static RelativePanel FindRelativePanelByName<RelativePanel>(DependencyObject parent, string name) where RelativePanel : DependencyObject
        {
            try
            {
                int j = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if ((string.IsNullOrEmpty(name) || controlName == name) && child is RelativePanel)
                    {
                        return child as RelativePanel;
                    }
                    else
                    {
                        RelativePanel result = FindVisualChildByName<RelativePanel>(child, name);
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard2.Begin();            
        }
    }
}
