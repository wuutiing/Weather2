using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Weather2.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Weather2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Favorites : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        Hashtable ht = CityProxy.GetPopularCityBackground();
        ObservableCollection<Weather> favoriteCityWeather = new ObservableCollection<Weather>();

        public Favorites()
        {
            this.InitializeComponent();
            Style morenButtonStyle = (Style)morenButton.GetValue(Button.StyleProperty);
            if (localSettings.Values["NowDisplayCity"] != null)
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

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["DefaultCity"] != null)
            {
                Grid ng = new Grid();
                ng.Height = 150;
                ng.Width = 300;
                string str = localSettings.Values["DefaultCity"].ToString();
                Weather w = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[str],(string)localSettings.Values["HeWeatherKey"]);

                ImageBrush ibback = new ImageBrush();
                Image iweather = new Image();
                string code = w.data[0].now.cond.code;
                if(DateTime.Now.Hour >= 20 ||DateTime.Now.Hour < 8)
                {
                    ibback.ImageSource = new BitmapImage(new Uri(String.Format("ms-appx:///Assets/WeatherBackImages/{0}n.png", code)));
                    iweather.Source = new BitmapImage(new Uri(String.Format("ms-appx:///Assets/WeatherImages/{0}n.png", code)));
                }
                else
                {
                    ibback.ImageSource = new BitmapImage(new Uri(String.Format("ms-appx:///Assets/WeatherBackImages/{0}.png", code)));
                    iweather.Source = new BitmapImage(new Uri(String.Format("ms-appx:///Assets/WeatherImages/{0}n.png", code)));

                }
                iweather.Margin = new Thickness(10, 30, 0, 0);
                iweather.Height = 100;
                iweather.Width = 100;
                ng.Children.Add(iweather);
                iweather.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                TextBlock tianqiText = new TextBlock();
                tianqiText.Text = w.data[0].now.cond.txt;
                tianqiText.FontSize = 12;
                tianqiText.Margin = new Thickness(120, 100, 0, 0);
                ng.Children.Add(tianqiText);
                TextBlock wenduText = new TextBlock();
                wenduText.Text = w.data[0].now.tmp + "℃";
                wenduText.FontSize = 36;
                wenduText.Margin = new Thickness(120, 40, 0, 0);
                ng.Children.Add(wenduText);
                TextBlock chengshiText = new TextBlock();
                chengshiText.Text = w.data[0].basic.city;
                chengshiText.FontSize = 15;
                chengshiText.Margin = new Thickness(10, 10, 0, 0);
                ng.Children.Add(chengshiText);
                morenButton.Background = ibback;
                morenButton.Content = ng;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (null != e.Parameter)
            {
                favoriteCityWeather = e.Parameter as ObservableCollection<Weather>;

            }
            
        }

        private void morenButton_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (null == localSettings.Values["DefaultCity"])
            {
                frame.Navigate(typeof(FirstLaunchPage));
            }
            else
            {
                frame.Navigate(typeof(MainPage), localSettings.Values["DefaultCity"]);
            }           
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var mygridview = sender as GridView;
            //Weather weather = mygridview.SelectedItem as Weather; 
            //Frame frame = Window.Current.Content as Frame;
            //frame.Navigate(typeof(MainPage), weather.data[0].basic.city);
            
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mygridview = sender as GridView;
            if (mygridview.SelectedIndex != -1)
            {
                Weather weather = mygridview.SelectedItem as Weather;
                localSettings.Values["NowDisplayCity"] = weather.data[0].basic.city;
                mygridview.SelectedIndex = -1;
                Frame frame = Window.Current.Content as Frame;
                frame.Navigate(typeof(MainPage), weather.data[0].basic.city);
            }
        }

        private void FindVisualChildByNameAndOrder<Grid>(DependencyObject parent, string name, List<Grid> gridList) where Grid : DependencyObject
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if ((string.IsNullOrEmpty(name) || controlName == name) && child is Grid)
                    {
                        gridList.Add(child as Grid);
                    }
                    else
                    {
                        FindVisualChildByNameAndOrder(child, name, gridList);
                    }
                }

            }
            catch
            {

            }
        }

        private void RootGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Grid g = sender as Grid;
            FlyoutBase.ShowAttachedFlyout(g);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem mfi = sender as MenuFlyoutItem;
            string[] fc = (string[])localSettings.Values["FavoriteCities"];
            if (fc.Contains(mfi.Name))
            {
                List<string> ls = fc.ToList();
                int i = ls.IndexOf(mfi.Name);
                ls.Remove(mfi.Name);
                favoriteCityWeather.RemoveAt(i);
                string[] strss = ls.ToArray();
                if(strss.Count() == 0)
                {
                    localSettings.Values["FavoriteCities"] = null;
                }
                else
                    localSettings.Values["FavoriteCities"] = strss;
            }
            //var tb = VisualTreeHelper.GetChild(g,0) as TextBlock;
            //if(tb != null)
            //    ceshite.Text = ceshite.Text + tb.Text;
            
        }

        

    }
}
