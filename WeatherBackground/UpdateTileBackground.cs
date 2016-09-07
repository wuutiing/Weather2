
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace WeatherBackground
{
    public sealed class UpdateTileBackground : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            UpdateTile();

            _deferral.Complete();
        }

        private const string myTileTemplate = @"<tile><visual><binding template='TileSquarePeekImageAndText02'><image id='1' src='{0}' alt='alt text'/><text id='1'>{1}</text><text id='2'>{2}</text></binding></visual></tile>";
        private void UpdateTile()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueue(true);
            updater.Clear();

            var doc = new XmlDocument();
            var xml = String.Format(myTileTemplate, "Assets/cloudy2.png", "查看所在城市天气", "很久没有更新啦，快来看看吧");
            doc.LoadXml(WebUtility.HtmlDecode(xml));

            updater.Update(new TileNotification(doc));
        }
    }
}
