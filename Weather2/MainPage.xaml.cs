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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Weather2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        FavoriteCityList fcl = new FavoriteCityList();
        
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = fcl;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                searchCityBoxGrid.Visibility = Visibility.Collapsed;
            }
        }

        //private async void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Weather myWeather = await WeatherProxy.GetWeatherByCityIdAsync("CN101210101");
        //    string code = myWeather.data[0].now.cond.code;
        //    image1.Source = new BitmapImage(new Uri(String.Format("http://files.heweather.com/cond_icon/{0}.png", code)));
        //    textBlock1.Text = myWeather.data[0].now.cond.txt;
        //}
        protected  override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            //处理从firstlaunchpage页/搜索导航到mainpage页进一步加载weatherpage页
            if ((string)e.Parameter != "" && e.Parameter != null && App.cityHash.ContainsKey((string)e.Parameter))
            {   
                string info = (string)e.Parameter;
                //Weather myWeather = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[info]);
                //mainFrame.Navigate(typeof(WeatherPage),myWeather);
                localSettings.Values["NowDisplayCity"] = info;
                fcl.cityNow = (string)localSettings.Values["NowDisplayCity"];
                myListView.SelectedIndex = 0;                
            }
            //处理从Cortana导航到mainpage页进一步加载favorites页
            else if("Favorite" == (string)e.Parameter)
            {
                myListView.SelectedIndex = 1;
            }
            //处理正常打开APP的流程
            else
            {
                if (1 == (int)localSettings.Values["isAlwaysGotoDefault"])
                {
                    if(null != localSettings.Values["DefaultCity"])
                    {
                        localSettings.Values["NowDisplayCity"] = localSettings.Values["DefaultCity"];
                        myListView.SelectedIndex = 0;
                    }
                    else
                    {
                        mainFrame.Navigate(typeof(FirstLaunchPage));
                        storyboard3.Begin();

                    }

                }
                else if (2 == (int)localSettings.Values["isAlwaysGotoDefault"])
                {
                    bool isGeoAllowed = await GeoProxy.isGeoallowedAsync();
                    if (!isGeoAllowed)
                    {
                        mainFrame.Navigate(typeof(FirstLaunchPage), "1");
                        storyboard3.Begin();

                    }
                    else
                    {
                        var geo = await GeoProxy.GetGeoLocationAsync();

                        OneLocation ol = await CityProxy.GetCityByLocationAsync(geo.Coordinate.Point.Position.Latitude,
                            geo.Coordinate.Point.Position.Longitude,
                            (string)localSettings.Values["TencentKey"]);
                        string str = CityProxy.GetCityName(ol);
                        if (App.cityHash.Contains(str))
                        {
                            localSettings.Values["NowDisplayCity"] = str;
                            myListView.SelectedIndex = 0;
                        }
                        else
                        {
                            mainFrame.Navigate(typeof(FirstLaunchPage), "2");
                        }
                        
                    }
                }
                else
                {
                    mainFrame.Navigate(typeof(FirstLaunchPage));
                }
                fcl.cityNow = (string)localSettings.Values["NowDisplayCity"];

            }
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
            
        }

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(myListView.SelectedIndex != -1)
            {
                var listViewItem = sender as ListViewItem;
                listViewItem.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 25, 25, 112));
                myListView.SelectedIndex = -1;
                HeadingText.Text = "设置";
                searchCityBox.Visibility = Visibility.Collapsed;
                addToFavorite.Visibility = Visibility.Collapsed;
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    searchButton.Visibility = Visibility.Collapsed;
                    searchCityBoxGrid.Visibility = Visibility.Collapsed;
                }
                mainFrame.Navigate(typeof(Settings));
                mySplitView.IsPaneOpen = false;
                storyboard3.Begin();
            }
            
        }

        private async void myListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mySplitView.IsPaneOpen = false;
            var listView = sender as ListView;
            if (listView.SelectedIndex != -1)
            {
                searchCityBox.Visibility = Visibility.Visible;
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    searchButton.Visibility = Visibility.Visible;
                    searchCityBoxGrid.Visibility = Visibility.Collapsed;
                }
                myListViewItem.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 255, 255));
            }
            if (listView.SelectedIndex == 0)
            {
                HeadingText.Text = localSettings.Values["NowDisplayCity"].ToString();
                mainFrame.Navigate(typeof(WeatherPage));
                addToFavorite.Visibility = Visibility.Visible;
                storyboard3.Begin();
            }
            if (listView.SelectedIndex == 1)
            {
                HeadingText.Text = "我的收藏";
                object fcw = await getFavoriteWeathersAsync();
                mainFrame.Navigate(typeof(Favorites), fcw);
                addToFavorite.Visibility = Visibility.Collapsed;
                storyboard3.Begin();
            }
        }

        private async void searchCityBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            if (asb.Text != "")
            {
                List<CityInfo> all = new List<CityInfo>();
                var cityList = await CityProxy.GetCityListAsync((string)localSettings.Values["HeWeatherKey"]);
                foreach (CityInfo o in cityList.city_info)
                {
                    all.Add(o);
                }
                var filtered = all.Where(p => p.city.StartsWith(asb.Text)).ToArray();             
                asb.ItemsSource = filtered;
            }
            else
            {
                asb.ItemsSource = null;
            }
            
        }

        private void searchCityBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            var cityInfo = args.SelectedItem as CityInfo;
            asb.Text = cityInfo.city;
            
        }

        private async void searchCityBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            if (!App.cityHash.ContainsKey(asb.Text))
            {
                var dialog = new ContentDialog()
                {
                    Title = "大事不好啦",
                    Content = String.Format("大气探测装置无法检测到\"{0}\"的存在。", asb.Text),
                    SecondaryButtonText = "关闭",
                    FullSizeDesired = false,
                };
                dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                await dialog.ShowAsync();
                
            }
            else
            {                
                localSettings.Values["NowDisplayCity"] = asb.Text;
                fcl.cityNow = localSettings.Values["NowDisplayCity"].ToString();
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    searchButton.Visibility = Visibility.Visible;
                    searchCityBoxGrid.Visibility = Visibility.Collapsed;
                }
                if (myListView.SelectedIndex == 0)
                {
                    mainFrame.Navigate(typeof(WeatherPage));
                    HeadingText.Text = (string)localSettings.Values["NowDisplayCity"];
                    storyboard3.Begin();
                }
                else myListView.SelectedIndex = 0;
                asb.Text = "";
                asb.ItemsSource = null;
            }            
        }

        private async Task<Object> getFavoriteWeathersAsync()
        {
            System.Collections.ObjectModel.ObservableCollection<Weather> favoriteCityWeathers = 
                new System.Collections.ObjectModel.ObservableCollection<Weather>();
            if (localSettings.Values["FavoriteCities"] != null)
            {
                string[] strs = (string[])localSettings.Values["FavoriteCities"];
                for (int i = 0; i < strs.Count(); i++)
                {
                    string iefael = strs[i];
                    Weather w = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[strs[i]],(string)localSettings.Values["HeWeatherKey"]);
                    favoriteCityWeathers.Add(w);
                }
            }            
            return favoriteCityWeathers;
        }

        private async void addToFavorite_Click(object sender, RoutedEventArgs e)
        {
            var teer = localSettings.Values["FavoriteCities"];
            if (localSettings.Values["FavoriteCities"] == null)
            {
                string[] strss = new string[] { (string)localSettings.Values["NowDisplayCity"] };
                localSettings.Values["FavoriteCities"] = strss;
            }
            else
            {
                string[] strs = (string[])localSettings.Values["FavoriteCities"];
                if(strs.Count() == 1)
                {
                    if (strs.Contains((string)localSettings.Values["NowDisplayCity"]))
                    {
                        localSettings.Values.Remove("FavoriteCities");
                    }
                    else
                    {
                        string[] strss = { (string)localSettings.Values["NowDisplayCity"], strs[0] };
                        localSettings.Values["FavoriteCities"] = strss;
                    }
                }
                else if(strs.Count() == 5)
                {
                    if(null == localSettings.Values["isMagician"])
                    {
                        if (strs.Contains((string)localSettings.Values["NowDisplayCity"]))
                        {
                            List<string> l = strs.ToList();
                            int i = l.IndexOf((string)localSettings.Values["NowDisplayCity"]);
                            l.RemoveAt(i);
                            string[] strss = l.ToArray();
                            localSettings.Values["FavoriteCities"] = strss;
                        }
                        else
                        {
                            string[] strss = { (string)localSettings.Values["NowDisplayCity"], strs[0], strs[1], strs[2], strs[3] };
                            localSettings.Values["FavoriteCities"] = strss;
                            var dialog = new ContentDialog()
                            {
                                Title = "您还不是会员",
                                Content = String.Format("您的存储达到上限，城市\"{0}\"已被挤掉，请到设置中获取会员以享受无限容量。", strs[4]),
                                SecondaryButtonText = "关闭",
                                FullSizeDesired = false,
                            };
                            dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                            await dialog.ShowAsync();

                        }
                    }
                    else
                    {
                        if (strs.Contains((string)localSettings.Values["NowDisplayCity"]))
                        {
                            List<string> l = strs.ToList();
                            int i = l.IndexOf((string)localSettings.Values["NowDisplayCity"]);
                            l.RemoveAt(i);
                            string[] strss = l.ToArray();
                            localSettings.Values["FavoriteCities"] = strss;
                        }
                        else
                        {
                            List<string> l = strs.ToList();
                            l.Insert(0, (string)localSettings.Values["NowDisplayCity"]);
                            string[] strss = l.ToArray();
                            localSettings.Values["FavoriteCities"] = strss;
                        }
                    }
                    
                }
                else
                {
                    if (strs.Contains((string)localSettings.Values["NowDisplayCity"]))
                    {
                        List<string> l = strs.ToList();
                        int i = l.IndexOf((string)localSettings.Values["NowDisplayCity"]);
                        l.RemoveAt(i);
                        string[] strss = l.ToArray();
                        localSettings.Values["FavoriteCities"] = strss;
                    }
                    else
                    {
                        List<string> l = strs.ToList();
                        l.Insert(0, (string)localSettings.Values["NowDisplayCity"]);
                        string[] strss = l.ToArray();
                        localSettings.Values["FavoriteCities"] = strss;
                    }
                }
                
            }
            fcl.cityNow = localSettings.Values["NowDisplayCity"].ToString();
        }

        private void Border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            flagText.Opacity = 1;
            mainFrame.Navigate(typeof(WeatherPage));
            storyboard.Begin();            
            storyboard2.Begin();            
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchButton.Visibility = Visibility.Collapsed;
            searchCityBoxGrid.Visibility = Visibility.Visible;
        }
    }
}
