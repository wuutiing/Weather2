using System;
using System.Collections;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Weather2.Models;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Windows.ApplicationModel.VoiceCommands;

namespace Weather2
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        internal static Hashtable cityHash = new Hashtable();
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
                        
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Frame rootFrame = Window.Current.Content as Frame;
            //初始化一些设置值
            Initialize();
            cityHash = await CityProxy.GetCityHashtableAsync((string)localSettings.Values["HeWeatherKey"]);
            //用于检查localsetting的值
            //指示是否是初次启动，null/0
            //var i = localSettings.Values["isFirstLaunched"];
            ////存储默认城市，null/string
            //var j = localSettings.Values["DefaultCity"];
            ////存储程序现在展示的城市，null/string
            //var k = localSettings.Values["NowDisplayCity"];
            ////存储用户设置，是否进入默认城市1/定位城市2/推荐页0
            //var l = localSettings.Values["isAlwaysGotoDefault"];
            ////存储用户收藏的城市，最多五个，null/string[]
            //var m = localSettings.Values["FavoriteCities"];
            ////存储用户订阅的详细信息,null/int[7]
            //var n = localSettings.Values["Subscription"];
            ////存储用户自定义的TencentKey，null/string
            //var ii = localSettings.Values["TencentKey"];
            ////存储用户自定义的HeWeatherKey，null/string
            //var jj = localSettings.Values["HeWeatherKey"];
            ////存储用户是不是魔法师
            //var kk = localSettings.Values["isMagician"];
            //localSettings.Values.Remove("");
            //localSettings.Values.Clear();
            //注册语音命令
            try
            {
                StorageFile vcdStorageFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri("ms-appx:///WeatherQueryXmlFile.xml"));

                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.
                    InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("安装语音命令失败" + ex.ToString());
            }
            //动态修改语音命令的监听对象
            if (null != localSettings.Values["FavoriteCities"])
            {
                VoiceCommandDefinition commandSetZhCn;
                if (Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("WeatherQueryCommandSet_zh-cn",
                    out commandSetZhCn))
                {
                    string[] newListListened = (string[])localSettings.Values["FavoriteCities"];
                    await commandSetZhCn.SetPhraseListAsync("cityName", newListListened);
                }
            }

            //注册后台任务
            //await RegisterLiveTileTask();
            

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态

            if (rootFrame == null)
            {

                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }         
            
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage));
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        protected async override void OnActivated(IActivatedEventArgs args )
        {
            base.OnActivated(args);

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            Initialize();
            cityHash = await CityProxy.GetCityHashtableAsync((string)localSettings.Values["HeWeatherKey"]);
            
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }
            
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                var commandArgs = args as VoiceCommandActivatedEventArgs;
                Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;
                switch (voiceCommandName)
                {
                    case "WeatherQueryCommand":
                        {
                            string spokenCity = speechRecognitionResult.SemanticInterpretation.Properties["cityName"][0];
                            localSettings.Values["NowDisplayCity"] = spokenCity;
                            rootFrame.Navigate(typeof(MainPage), spokenCity);
                           
                        }
                        break;
                    case "enterSet":
                        {
                            if(null == localSettings.Values["DefaultCity"])
                            {
                                rootFrame.Navigate(typeof(FirstLaunchPage));
                            }
                            else
                                rootFrame.Navigate(typeof(MainPage), localSettings.Values["DefaultyCity"]);
                        }
                        break;
                    case "CheckFavoriteCommand":
                        {
                            if(null == localSettings.Values["NowDisplayCity"])
                            {
                                
                                bool isGeoAllowed = await GeoProxy.isGeoallowedAsync();
                                if (!isGeoAllowed)
                                {
                                    rootFrame.Navigate(typeof(FirstLaunchPage), "1");
                                }
                                else
                                {
                                    var geo = await GeoProxy.GetGeoLocationAsync();

                                    OneLocation ol = await CityProxy.GetCityByLocationAsync(geo.Coordinate.Point.Position.Latitude,
                                        geo.Coordinate.Point.Position.Longitude,
                                        (string)localSettings.Values["TencentKey"]);
                                    string str = CityProxy.GetCityName(ol);
                                    localSettings.Values["kkkkk"] = str;
                                    if (cityHash.Contains(str))
                                    {
                                        localSettings.Values["NowDisplayCity"] = str;
                                        rootFrame.Navigate(typeof(MainPage), "Favorite");
                                    }
                                    else
                                    {
                                        rootFrame.Navigate(typeof(FirstLaunchPage), "2");
                                    }
                                }
                            }
                            else
                                rootFrame.Navigate(typeof(MainPage),"Favorite");
                        }
                        break;
                    default: break;
                }
                Window.Current.Activate();

            }

        }

        private async Task RegisterLiveTileTask()
        {
            bool isTaskRegistered = false;
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if(status == BackgroundAccessStatus.Denied || status == BackgroundAccessStatus.Unspecified)
            {
                return;

            }
            foreach(var task in BackgroundTaskRegistration.AllTasks)
            {
                if(task.Value.Name == "UpdateTileBackground")
                {
                    isTaskRegistered = true;
                }
            }
            if (!isTaskRegistered)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = "大气探测装置";
                builder.TaskEntryPoint = "WeatherBackground.UpdateTileBackground";
                builder.SetTrigger(new TimeTrigger(60, false));
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                BackgroundTaskRegistration task = builder.Register();
            }
            
        }

        private void Initialize()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (null == localSettings.Values["HeWeatherKey"])
            {
                localSettings.Values["HeWeatherKey"] = "0";
            }
            if (null == localSettings.Values["TencnetKey"])
            {
                localSettings.Values["TencentKey"] = "0";
            }
            if (null == localSettings.Values["Subscription"])
                localSettings.Values["Subscription"] = new int[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
            if (localSettings.Values["isAlwaysGotoDefault"] == null)
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                    localSettings.Values["isAlwaysGotoDefault"] = 2;
                else localSettings.Values["isAlwaysGotoDefault"] = 1;
            }
            //是不是手机，如是的话注册后退事件
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, eee) =>
                {
                    Frame frame = Window.Current.Content as Frame;
                    if (null == frame)
                        return;
                    if (frame.CanGoBack)
                    {
                        if (null != localSettings.Values["isFirstLaunched"])
                        {
                            frame.GoBack();
                            eee.Handled = true;
                        }
                        else
                            return;

                    }

                };

            }
        }



    }
}
