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
            if (localSettings.Values["NowDisplayCity"] != null)
            {
                fcl.cityNow = (string)localSettings.Values["NowDisplayCity"];
            }
            
            
        }

        //private async void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Weather myWeather = await WeatherProxy.GetWeatherByCityIdAsync("CN101210101");
        //    string code = myWeather.data[0].now.cond.code;
        //    image1.Source = new BitmapImage(new Uri(String.Format("http://files.heweather.com/cond_icon/{0}.png", code)));
        //    textBlock1.Text = myWeather.data[0].now.cond.txt;
        //}
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if ((string)e.Parameter != "" && e.Parameter != null && App.cityHash.ContainsKey((string)e.Parameter))
            {
                //只有传入了有效的城市名才可以进行的获取城市天气
                string info = (string)e.Parameter;
                //Weather myWeather = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[info]);
                //mainFrame.Navigate(typeof(WeatherPage),myWeather);
                localSettings.Values["NowDisplayCity"] = info;
                fcl.cityNow = localSettings.Values["NowDisplayCity"].ToString();
                HeadingText.Text = info;                
                myListView.SelectedIndex = 0;                
            }
            else
            {
                //mainFrame.Navigate(typeof(WeatherPage));
                Frame myFrame = Window.Current.Content as Frame;
                myFrame.Navigate(typeof(FirstLaunchPage));
            }
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
            
        }

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var listViewItem = sender as ListViewItem;
            listViewItem.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,25,25,112));
            myListView.SelectedIndex = -1;
            HeadingText.Text = "设置";
            searchCityBox.Visibility = Visibility.Collapsed;
            addToFavorite.Visibility = Visibility.Collapsed;
            mainFrame.Navigate(typeof(Settings));
            mySplitView.IsPaneOpen = false;
        }

        private async void myListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mySplitView.IsPaneOpen = false;
            var listView = sender as ListView;
            if(listView.SelectedIndex != -1)
            {
                searchCityBox.Visibility = Visibility.Visible;
                myListViewItem.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 255, 255, 255));
            }
            if(listView.SelectedIndex == 0)
            {
                HeadingText.Text = localSettings.Values["NowDisplayCity"].ToString();
                mainFrame.Navigate(typeof(WeatherPage));
                addToFavorite.Visibility = Visibility.Visible;
            }
            if(listView.SelectedIndex == 1)
            {
                HeadingText.Text = "我的收藏";
                object fcw = await getFavoriteWeathersAsync();
                mainFrame.Navigate(typeof(Favorites),fcw);
                addToFavorite.Visibility = Visibility.Collapsed;
            }
        }

        private async void searchCityBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            if (asb.Text != "")
            {
                List<CityInfo> all = new List<CityInfo>();
                var cityList = await CityProxy.GetCityListAsync();
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
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), asb.Text);
            localSettings.Values["NowDisplayCity"] = asb.Text;
            fcl.cityNow = localSettings.Values["NowDisplayCity"].ToString();
            //if (localSettings.Values["isFirstLaunched"] == null)
            //    localSettings.Values["isFirstLaunched"] = 0;
        }

        private async void searchCityBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            if (!App.cityHash.ContainsKey(asb.Text))
            {
                var messageDialog = new MessageDialog(String.Format("大气探测装置无法检测到\"{0}\"的存在。", asb.Text))
                {
                    Title = "告诉你一个坏消息"
                };
                messageDialog.Commands.Add(new UICommand("关闭", cmd => { }, commandId: 0));
                await messageDialog.ShowAsync();
            }
            else
            {
                Frame thisFrame = Window.Current.Content as Frame;
                thisFrame.Navigate(typeof(MainPage), asb.Text);
                localSettings.Values["NowDisplayCity"] = asb.Text;
                fcl.cityNow = localSettings.Values["NowDisplayCity"].ToString();
                //if (localSettings.Values["isFirstLaunched"] == null)
                //    localSettings.Values["isFirstLaunched"] = 0;
            }
        }
        private async Task<Object> getFavoriteWeathersAsync()
        {
            System.Collections.ObjectModel.ObservableCollection<Weather> favoriteCityWeathers = new System.Collections.ObjectModel.ObservableCollection<Weather>();
            if (localSettings.Values["FavoriteCities"] != null)
            {
                string[] strs = (string[])localSettings.Values["FavoriteCities"];
                for (int i = 0; i < strs.Count(); i++)
                {
                    string iefael = strs[i];
                    Weather w = await WeatherProxy.GetWeatherByCityIdAsync((string)App.cityHash[strs[i]]);
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
                        var messageDialog = new MessageDialog(String.Format("您的存储达到上限，城市\"{0}\"已被挤掉，请续费会员以获得无限容量。", strs[4]))
                        {
                            Title = "会员到期啦"
                        };
                        messageDialog.Commands.Add(new UICommand("关闭", cmd => { }, commandId: 0));
                        await messageDialog.ShowAsync();
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

        
    }
}
