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
using Windows.UI.Popups;
using Windows.Storage;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Weather2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FirstLaunchPage : Page
    {
        List<CityInfo> popularCities = new List<CityInfo>();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public FirstLaunchPage()
        {
            this.InitializeComponent();
            popularCities = CityProxy.GetPopularCities();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(null != e.Parameter)
            {
                string sddsss = e.Parameter.ToString();
                if ("1" == e.Parameter.ToString())
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "大气探测装置受阻",
                        Content = "定位未打开或没有定位权限，请转到设置",
                        PrimaryButtonText = "确定",
                        SecondaryButtonText = "取消",
                        FullSizeDesired = false,

                    };
                    dialog.PrimaryButtonClick += async (s, ee) => { await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location")); };
                    dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                    await dialog.ShowAsync();
                }
                else if ("2" == e.Parameter.ToString())
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "大气探测装置出现错误",
                        Content = "定位失败，可能的原因是不支持的地点，或者是自定义TencentKey无效，请尝试搜索/更改自定义Key",

                        SecondaryButtonText = "关闭",
                        FullSizeDesired = false,
                    };
                    dialog.SecondaryButtonClick += (s, ee) => { };
                    await dialog.ShowAsync();
                }
            }
            
            
        }

        private void myGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = myGridView.SelectedItem as CityInfo;
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), item.city);
            localSettings.Values["DefaultCity"] = item.city;
            localSettings.Values["NowDisplayCity"] = item.city;
            if (localSettings.Values["isFirstLaunched"] == null)
                localSettings.Values["isFirstLaunched"] = 0;
        }

        private void myAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            var cityInfo = args.SelectedItem as CityInfo;
            asb.Text = cityInfo.city;
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), asb.Text);
            localSettings.Values["DefaultCity"] = asb.Text;
            localSettings.Values["NowDisplayCity"] = asb.Text;
            if (localSettings.Values["isFirstLaunched"] == null)
                localSettings.Values["isFirstLaunched"] = 0;
        }

        private async void myAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //List<string> all = new List<string>();
            //foreach(object o in App.cityHash.Keys)
            //{
            //    all.Add((string)o);
            //}
            //var asb = (AutoSuggestBox)sender;
            //var filtered = all.Where(p => p.StartsWith(asb.Text)).ToArray();
            //asb.ItemsSource = filtered;
            var asb = (AutoSuggestBox)sender;
            if(asb.Text != "")
            {
                List<CityInfo> all = new List<CityInfo>();
                var cityList = await CityProxy.GetCityListAsync((string)localSettings.Values["HeWeatherKey"]);
                foreach (CityInfo o in cityList.city_info)
                {
                    all.Add(o);
                }
                var filtered = all.Where(p => p.city.StartsWith(asb.Text)).ToArray();
                if (filtered.Count() > 8)
                {
                    var frrr = filtered.Take(8);
                    asb.ItemsSource = frrr;
                }
                else
                {
                    asb.ItemsSource = filtered;
                }
            }
            else
            {
                asb.ItemsSource = null;
            }

        }

        private async void myAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var asb = (AutoSuggestBox)sender;
            if (!App.cityHash.ContainsKey(asb.Text))
            {
                var messageDialog = new MessageDialog(String.Format("未搜索到城市\"{0}\"。", asb.Text))
                {
                    Title = "出现错误"
                };
                messageDialog.Commands.Add(new UICommand("关闭", cmd => { }, commandId: 0));
                await messageDialog.ShowAsync();
            }
            else
            {
                Frame thisFrame = Window.Current.Content as Frame;
                thisFrame.Navigate(typeof(MainPage), asb.Text );
                localSettings.Values["DefaultCity"] = asb.Text;
                localSettings.Values["NowDisplayCity"] = asb.Text;
                if (localSettings.Values["isFirstLaunched"] == null)
                    localSettings.Values["isFirstLaunched"] = 0;
            }
        }

       
    }
}
