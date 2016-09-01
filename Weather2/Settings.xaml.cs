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
            if (localSettings.Values["isAlwaysGotoDefault"] == null)
                localSettings.Values["isAlwaysGotoDefault"] = true;
            myToggleSwitch.IsOn = (bool)localSettings.Values["isAlwaysGotoDefault"];
            if(localSettings.Values["DefaultCity"] != null)
                defaultCity.Text = localSettings.Values["DefaultCity"].ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values.Remove("isFirstLaunched");
            localSettings.Values.Remove("DefaultCity");
            localSettings.Values.Remove("NowDisplayCity");
            localSettings.Values.Remove("isAlwaysGotoDefault");
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(FirstLaunchPage));

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            localSettings.Values.Remove("DefaultCity");
            defaultCity.Text = "";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(FirstLaunchPage));
        }

        private void myToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            localSettings.Values["isAlwaysGotoDefault"] = ts.IsOn;
        }
    }
}
