using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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

            Frame rootFrame = Window.Current.Content as Frame;
            cityHash = await CityProxy.GetCityHashtableAsync();

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            //用于检查localsetting的值
            //var i = localSettings.Values["isFirstLaunched"];
            //var j = localSettings.Values["DefaultCity"];
            //var k = localSettings.Values["NowDisplayCity"];
            //var l = localSettings.Values["isAlwaysGotoDefault"];
            //var m = localSettings.Values["FavoriteCities"];
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

            //初始化设置值
            if (localSettings.Values["isAlwaysGotoDefault"] == null)
            {
                localSettings.Values["isAlwaysGotoDefault"] = true;
            }

            Frame root = Window.Current.Content as Frame;
            if (localSettings.Values["DefaultCity"] != null)
            {
                if ((bool)localSettings.Values["isAlwaysGotoDefault"] == true)
                {
                    localSettings.Values["NowDisplayCity"] = localSettings.Values["DefaultCity"];
                    root.Navigate(typeof(MainPage), localSettings.Values["DefaultCity"]);
                }
                else
                {
                    root.Navigate(typeof(FirstLaunchPage));
                }
            }
            else
            {
                root.Navigate(typeof(FirstLaunchPage));
                
            }
                       
            if (e.PrelaunchActivated == false)
            {                
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
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

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

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
                            Frame frame = Window.Current.Content as Frame;
                            frame.Navigate(typeof(MainPage), spokenCity);
                            localSettings.Values["NowDisplayCity"] = spokenCity;
                        }                        
                        break;
                    default:break;                        
                }

            }
        }
    }
}
