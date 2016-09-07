using System;
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
using Windows.UI.Xaml.Navigation;
using Weather2.Models;
using Windows.ApplicationModel.DataTransfer;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Weather2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Settings : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        
        public Settings()
        {
            this.InitializeComponent();     
            if(null == localSettings.Values["isAlwaysGotoDefault"])
            {
                localSettings.Values["isAlwaysGotoDefault"] = 2;
            }
            int i = (int)localSettings.Values["isAlwaysGotoDefault"];
            if(1 == i)
            {
                myToggleSwitch.IsOn = true;
                myToggleSwitch2.IsOn = false;
            }
            else if(2 == i)
            {
                myToggleSwitch.IsOn = false;
                myToggleSwitch2.IsOn = true;
            }
            else
            {
                myToggleSwitch.IsOn = false;
                myToggleSwitch2.IsOn = false;
            }
            if(null != localSettings.Values["Subscription"])
            {
                int[] ints = (int[])localSettings.Values["Subscription"];
                myToggleSwitch3.IsOn = (ints[7] == 1);
                subscriptionList.Visibility = ints[7] == 1 ? Visibility.Visible : Visibility.Collapsed;
                cb0.IsChecked = (ints[0] == 1);
                cb1.IsChecked = (ints[1] == 1);
                cb2.IsChecked = (ints[2] == 1);
                cb3.IsChecked = (ints[3] == 1);
                cb4.IsChecked = (ints[4] == 1);
                cb5.IsChecked = (ints[5] == 1);
                cb6.IsChecked = (ints[6] == 1);
                
            }
            if(null != localSettings.Values["DefaultCity"])
                defaultCity.Text = localSettings.Values["DefaultCity"].ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values.Clear();
            
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(FirstLaunchPage));

        }
                
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(FirstLaunchPage));
        }

        private async void myToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts.IsOn)
            {
                if(null != localSettings.Values["DefaultCity"])
                {
                    localSettings.Values["isAlwaysGotoDefault"] = 1;
                    myToggleSwitch2.IsOn = false;
                }
                else
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "不要冲动",
                        Content = "大气探测装置未设置默认城市，请先冷静一下再来设置",
                        SecondaryButtonText = "关闭",
                        FullSizeDesired = false,

                    };
                    dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                    await dialog.ShowAsync();
                    ts.IsOn = false;
                }
                
            }
            else
            {
                if(1 == (int)localSettings.Values["isAlwaysGotoDefault"])
                {
                    localSettings.Values["isAlwaysGotoDefault"] = 0;
                }
            }
        }

        private async void myToggleSwitch2_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts.IsOn)
            {
                bool isGeoEnabled = true;                
                try
                {
                    var geo = await GeoProxy.GetGeoLocationAsync();
                }
                catch
                {
                    isGeoEnabled = false;
                }
                if(isGeoEnabled == false)
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "不要冲动",
                        Content = "检测到您的设备没有定位功能，请先冷静一下再来设置",
                        SecondaryButtonText = "关闭",
                        FullSizeDesired = false,

                    };
                    dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                    await dialog.ShowAsync();
                    ts.IsOn = false;
                    return;
                }

                else
                {
                    localSettings.Values["isAlwaysGotoDefault"] = 2;
                    myToggleSwitch.IsOn = false;
                }             
                
                
            }
            else
            {
                if (2 == (int)localSettings.Values["isAlwaysGotoDefault"])
                {
                    localSettings.Values["isAlwaysGotoDefault"] = 0;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            string cbc = cb.Content.ToString();
            int[] strs = (int[])localSettings.Values["Subscription"];
            switch (cbc)
            {                
                case "舒适度":
                    {
                        List<int> l = strs.ToList();
                        l[0] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }                
                case "体感温度":
                    {
                        List<int> l = strs.ToList();
                        l[1] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }              
                case "运动":
                    {
                        List<int> l = strs.ToList();
                        l[2] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                
                case "旅游":
                    {
                        List<int> l = strs.ToList();
                        l[3] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "洗车":
                    {
                        List<int> l = strs.ToList();
                        l[4] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "紫外线":
                    {
                        List<int> l = strs.ToList();
                        l[5] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "流感":
                    {
                        List<int> l = strs.ToList();
                        l[6] = 1;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                default: break;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            string cbc = cb.Content.ToString();
            int[] strs = (int[])localSettings.Values["Subscription"];
            switch (cbc)
            {
                case "舒适度":
                    {
                        List<int> l = strs.ToList();
                        l[0] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "体感温度":
                    {
                        List<int> l = strs.ToList();
                        l[1] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "运动":
                    {
                        List<int> l = strs.ToList();
                        l[2] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }

                case "旅游":
                    {
                        List<int> l = strs.ToList();
                        l[3] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "洗车":
                    {
                        List<int> l = strs.ToList();
                        l[4] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "紫外线":
                    {
                        List<int> l = strs.ToList();
                        l[5] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                case "流感":
                    {
                        List<int> l = strs.ToList();
                        l[6] = 0;
                        localSettings.Values["Subscription"] = l.ToArray();
                        break;
                    }
                default: break;
            }
            int[] strss = (int[])localSettings.Values["Subscription"];
            int isNoneChosen = strss[0] + strss[1] + strss[2] + strss[3] + strss[4] + strss[5] + strss[6];
            if(0 == isNoneChosen)
            {
                myToggleSwitch3.IsOn = false;
            }
        }

        private void myToggleSwitch3_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            int[] ints = (int[])localSettings.Values["Subscription"];
            List<int> l = ints.ToList();

            if (ts.IsOn)
            {
                //if(subscriptionList != null)
                    subscriptionList.Visibility = Visibility.Visible;
                l[7] = 1;
                localSettings.Values["Subscription"] = l.ToArray();
            }
            else
            {
                subscriptionList.Visibility = Visibility.Collapsed;
                l[7] = 0;
                localSettings.Values["Subscription"] = l.ToArray();
            }
        }

        private async void TencentKeyB_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Content.ToString())
            {
                case "设置TencentKey":
                    {
                        if("" != TencentKeyTX.Text)
                        {
                            OneLocation ol = await CityProxy.GetCityByLocationAsync(120, 30, TencentKeyTX.Text);
                            if("query ok" != ol.message)
                            {
                                var dialog = new ContentDialog()
                                {
                                    Title = "警告",
                                    Content = "您设置的TencentKey可能失效，请检查！",
                                    SecondaryButtonText = "关闭",
                                    FullSizeDesired = false,
                                };
                                dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                                await dialog.ShowAsync();
                            }
                            else
                            {
                                localSettings.Values["TencentKey"] = TencentKeyTX.Text;
                                TencentKeyTX.IsEnabled = false;
                                btn.Content = "清除自定义的TencentKey";
                            }
                            
                        }
                        else
                        {
                            var dialog = new ContentDialog()
                            {
                                Title = "哈哈傻了吧",
                                Content = "您填入的内容为空，设置未更改",
                                SecondaryButtonText = "关闭",
                                FullSizeDesired = false,
                            };
                            dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                            await dialog.ShowAsync();
                        }
                        break;
                    }
                case "清除自定义的TencentKey":
                    {
                        TencentKeyTX.IsEnabled = true;
                        TencentKeyTX.Text = "";
                        localSettings.Values["TencentKey"] = "0";
                        btn.Content = "设置TencentKey";
                        break;
                    }
                default:break;
            }

        }

        private async void HeWeatherKeyB_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Content.ToString())
            {
                case "设置HeWeatherKey":
                    {
                        if ("" != HeWeatherKeyTX.Text)
                        {
                           
                            Weather w = await WeatherProxy.GetWeatherByCityIdAsync("CN101010100", HeWeatherKeyTX.Text);
                            if("invalid key" == w.data[0].status)
                            {
                                var dialog = new ContentDialog()
                                {
                                    Title = "警告",
                                    Content = "您设置的HeWeatherKey可能失效，请检查！",
                                    SecondaryButtonText = "关闭",
                                    FullSizeDesired = false,
                                };
                                dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                                await dialog.ShowAsync();
                                
                            }
                            else
                            {
                                localSettings.Values["HeWeatherKeyKey"] = HeWeatherKeyTX.Text;
                                HeWeatherKeyTX.IsEnabled = false;
                                btn.Content = "清除自定义的HeWeatherKey";
                            }
                        }
                        else
                        {
                            var dialog = new ContentDialog()
                            {
                                Title = "哈哈傻了吧",
                                Content = "您填入的内容为空，设置未更改",
                                SecondaryButtonText = "关闭",
                                FullSizeDesired = false,
                            };
                            dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                            await dialog.ShowAsync();
                        }
                        break;
                    }
                case "清除自定义的HeWeatherKey":
                    {
                        HeWeatherKeyTX.IsEnabled = true;
                        HeWeatherKeyTX.Text = "";
                        localSettings.Values["HeWeatherKey"] = "0";
                        btn.Content = "设置HeWeatherKey";

                        break;
                    }
                default: break;
            }
        }

        

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri($"mailto:{"wuutiing@outlook.com"}?subject={""}&body={""}");
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if ("+1s" == answerTextBox.Text)
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["isMagician"] = "1";
                var dialog = new ContentDialog()
                {
                    Title = "善意的微笑",
                    Content = "年轻人，欢迎加入魔法协会，凭会员身份您可以收藏不限数目城市。",
                    SecondaryButtonText = "关闭",
                    FullSizeDesired = false,
                };
                dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                await dialog.ShowAsync();
                answerTextBox.Text = "";
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "善意的微笑",
                    Content = "年轻人，你还是Too Young。",
                    SecondaryButtonText = "关闭",
                    FullSizeDesired = false,
                };
                dialog.SecondaryButtonClick += (s, ee) => { dialog.Hide(); };
                await dialog.ShowAsync();
                answerTextBox.Text = "";
            }
            answerTextBox.IsEnabled = false;
            bt.IsEnabled = false;
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText( AliPayText.Text);
            Clipboard.SetContent(dp);
            ((HyperlinkButton)sender).Opacity = 0.6;          
        }
    }
}
