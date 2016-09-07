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
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

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
                if (ht.ContainsKey((string)localSettings.Values["NowDisplayCity"]))
                {
                    ImageBrush ib = new ImageBrush();
                    string tise = ht[localSettings.Values["NowDisplayCity"].ToString()].ToString();
                    ib.ImageSource = new BitmapImage(new Uri(String.Format("ms-appx:///" + tise), UriKind.Absolute));
                    ib.Stretch = Stretch.UniformToFill;
                    myScrollViewer3.Background = ib;

                }
                else
                {
                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/CityImages/other.jpg", UriKind.Absolute));
                    ib.Stretch = Stretch.UniformToFill;
                    myScrollViewer3.Background = ib;

                }               
            }
            if(null == localSettings.Values["Subscription"])
            {
                int[] intss = new int[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
                localSettings.Values["Subscription"] = intss;
            }
            object o = localSettings.Values["Subscription"];
            int[] ints = (int[])localSettings.Values["Subscription"];
            
            int iiii = ints[7] == 1 ? 1 : 0;
            detailButton.Visibility = ints[7] == 1 ? Visibility.Visible : Visibility.Collapsed;
            
            
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (localSettings.Values["NowDisplayCity"] != null)
            {
                Weather myWeather = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[localSettings.Values["NowDisplayCity"]],
                    (string)localSettings.Values["HeWeatherKey"]);
                PaintPage(myWeather);
                UpdateTile(myWeather);
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "告诉你一个坏消息",
                    Content = "大气探测装置出现存储故障，请重启。",
                    SecondaryButtonText = "关闭",
                    FullSizeDesired = false,

                };
                dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                await dialog.ShowAsync();

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
            PaintDetail(myWeather);
            storyboard2.Begin();

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
        private void PaintDetail(Weather myWeather)
        {
            int[] ints = (int[])localSettings.Values["Subscription"];
            comfortS.Visibility = ints[0] == 1 ? Visibility.Visible : Visibility.Collapsed;
            feelTemS.Visibility = ints[1] == 1 ? Visibility.Visible : Visibility.Collapsed;
            sportS.Visibility = ints[2] == 1 ? Visibility.Visible : Visibility.Collapsed;
            travelS.Visibility = ints[3] == 1 ? Visibility.Visible : Visibility.Collapsed;
            washCarS.Visibility = ints[4] == 1 ? Visibility.Visible : Visibility.Collapsed;
            uvS.Visibility = ints[5] == 1 ? Visibility.Visible : Visibility.Collapsed;
            fluS.Visibility = ints[6] == 1 ? Visibility.Visible : Visibility.Collapsed;
            comfortB.Text = myWeather.data[0].suggestion.comf.brf;
            comfortD.Text = myWeather.data[0].suggestion.comf.txt;
            feelTemB.Text = myWeather.data[0].suggestion.drsg.brf;
            feelTemD.Text = myWeather.data[0].suggestion.drsg.txt;
            sportB.Text = myWeather.data[0].suggestion.sport.brf;
            sportD.Text = myWeather.data[0].suggestion.sport.txt;
            travelB.Text = myWeather.data[0].suggestion.trav.brf;
            travelD.Text = myWeather.data[0].suggestion.trav.txt;
            washCarB.Text = myWeather.data[0].suggestion.cw.brf;
            washCarD.Text = myWeather.data[0].suggestion.cw.txt;
            uvB.Text = myWeather.data[0].suggestion.uv.brf;
            uvD.Text = myWeather.data[0].suggestion.uv.txt;
            
            fluB.Text = myWeather.data[0].suggestion.flu.brf;
            fluD.Text = myWeather.data[0].suggestion.flu.txt;
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

        //private static RelativePanel FindRelativePanelByName<RelativePanel>(DependencyObject parent, string name) where RelativePanel : DependencyObject
        //{
        //    try
        //    {
        //        int j = VisualTreeHelper.GetChildrenCount(parent);
        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //        {
        //            var child = VisualTreeHelper.GetChild(parent, i);
        //            string controlName = child.GetValue(Control.NameProperty) as string;
        //            if ((string.IsNullOrEmpty(name) || controlName == name) && child is RelativePanel)
        //            {
        //                return child as RelativePanel;
        //            }
        //            else
        //            {
        //                RelativePanel result = FindVisualChildByName<RelativePanel>(child, name);
        //                if (result != null)
        //                    return result;
        //            }
        //        }
        //        return null;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            detailButton.Visibility = Visibility.Collapsed;
            closeDetailButton.Visibility = Visibility.Visible;
            myScrollViewer1.Visibility = Visibility.Visible;
            storyboard3.Begin();
        }

        private void closeDetailButton_Click(object sender, RoutedEventArgs e)
        {
            detailButton.Visibility = Visibility.Visible;
            closeDetailButton.Visibility = Visibility.Collapsed;
            myScrollViewer1.Visibility = Visibility.Collapsed;
        }

        private void UpdateTile(Weather w)
        {
            string myTileTemplateMedium = "<tile><visual><binding template='TileMedium'><image id='1' src='ms-appx:///Assets/WeatherImages/{0}.png' placement='peek'/><text hint-align='center' hint-style='subheader'>{1}℃</text><text hint-align='center'>{2}</text><text hint-align='center'>{3}</text></binding>";
            string myTileMedium = String.Format(myTileTemplateMedium, w.data[0].now.cond.code, w.data[0].now.tmp, (string)localSettings.Values["NowDisplayCity"], w.data[0].now.cond.txt);
            string dahoutian = w.data[0].daily_forecast[3].date.Substring(8) + "日";
            string dadahoutian = w.data[0].daily_forecast[4].date.Substring(8) + "日";

            string myTileTemplateWide = "<binding template='TileWide' displayName='大气探测装置:{9}' branding='name'><image src='ms-appx:///Assets/WeatherBackImages/{0}.png' placement='background'/><image src='ms-appx:///Assets/WeatherImages/{8}.png' hint-removeMargin='false' placement='peek'/><group><subgroup hint-weight='1'><text hint-align='center'>今天</text><image src='ms-appx:///Assets/WeatherImages/{1}.png'/></subgroup><subgroup hint-weight='1'><text hint-align='center'>明天</text><image src='ms-appx:///Assets/WeatherImages/{2}.png'/></subgroup><subgroup hint-weight='1'><text hint-align='center'>后天</text><image src='ms-appx:///Assets/WeatherImages/{3}.png'/></subgroup><subgroup hint-weight='1'><text hint-align='center'>{4}</text><image src='ms-appx:///Assets/WeatherImages/{5}.png'/></subgroup><subgroup hint-weight='1'><text hint-align='center'>{6}</text><image src='ms-appx:///Assets/WeatherImages/{7}.png'/></subgroup></group></binding></visual></tile>";
            string myTileWide = String.Format(myTileTemplateWide,w.data[0].now.cond.code,w.data[0].daily_forecast[0].cond.code_d, w.data[0].daily_forecast[1].cond.code_d, w.data[0].daily_forecast[2].cond.code_d,dahoutian, w.data[0].daily_forecast[3].cond.code_d,dadahoutian, w.data[0].daily_forecast[4].cond.code_d,w.data[0].now.cond.code,w.data[0].basic.city);

            string str = myTileMedium + myTileWide;
            
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueue(true);
            updater.Clear();

            var doc = new XmlDocument();
            
            doc.LoadXml(str);
            updater.Update(new TileNotification(doc));

        }
    }
}
