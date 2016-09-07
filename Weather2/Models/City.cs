using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Weather2.Models
{
    [DataContract]
    internal class CityInfo
    {
        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string cnty { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string lat { get; set; }

        [DataMember]
        public string lon { get; set; }

        [DataMember]
        public string prov { get; set; }

        public CityInfo(string cityName)
        {
            city = cityName;
            cnty = "中国";
            id = (string)App.cityHash[cityName];
            lat = "";
            lon = "";
            prov = String.Format("ms-appx:///Assets/CityImages/{0}.png",cityName);
        }
    }

    [DataContract]
    internal class CityList
    {
        [DataMember]
        public CityInfo[] city_info { get; set; }

        [DataMember]
        public string status { get; set; }
    }

    internal class FavoriteCityList : INotifyPropertyChanged
    {
        private string city_now;
        public string cityNow
        {
            get { return city_now; }
            set
            {
                city_now = value;
                OnPropertyChanged("cityNow");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs (name));
            }
        }

    }

    internal class CityProxy
    {
        //获得城市和id的对应hashtable
        public static async Task<Hashtable> GetCityHashtableAsync(string heWeatherKey)
        {
            CityList myCityList = await GetCityListAsync(heWeatherKey);
            Hashtable cityHash = new Hashtable();
            foreach (CityInfo ci in myCityList.city_info)
            {
                if (!cityHash.ContainsKey(ci.city))
                {
                    cityHash.Add(ci.city, ci.id);
                }
            }
            //由于市县的名字有重复，所以将重复的几个地点手动添加
            cityHash.Add("九龙(四川)", "CN101271805");
            cityHash.Add("通州(江苏)", "CN101190509");
            cityHash.Add("河南(青海)", "CN101150304");
            cityHash.Add("河口(云南)", "CN101290313");
            cityHash.Add("东兴(四川)", "CN101271202");
            cityHash.Add("东乡(甘肃)", "CN101161106");
            cityHash.Add("朝阳(辽宁)", "CN101071201");

            return cityHash;
        }

        public static async Task<CityList> GetCityListAsync(string heWeatherKey)
        {
            if("0" == heWeatherKey)
            {
                heWeatherKey = "69ebef9bc6c6480987683f57318298aa";
            }
            string httpUri = String.Format("https://api.heweather.com/x3/citylist?search={0}&key={1}", "allchina", heWeatherKey);
            HttpClient hc = new HttpClient();
            var serializer = new DataContractJsonSerializer(typeof(CityList));
            string str = await hc.GetStringAsync(new Uri(httpUri));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str));
            CityList myCityList = (CityList)serializer.ReadObject(ms);

            return myCityList;
        }

        public static List<CityInfo> GetPopularCities()
        {
            List<CityInfo> popularCities = new List<CityInfo>();
            popularCities.Add(new CityInfo("杭州"));
            popularCities.Add(new CityInfo("上海"));
            popularCities.Add(new CityInfo("北京"));
            popularCities.Add(new CityInfo("天津"));
            popularCities.Add(new CityInfo("深圳"));
            popularCities.Add(new CityInfo("广州"));
            popularCities.Add(new CityInfo("南京"));
            popularCities.Add(new CityInfo("西安"));
            popularCities.Add(new CityInfo("重庆"));
            popularCities.Add(new CityInfo("乌鲁木齐"));
            popularCities.Add(new CityInfo("成都"));
            popularCities.Add(new CityInfo("苏州"));

            return popularCities;
        }

        public static Hashtable GetPopularCityBackground()
        {
            Hashtable ht = new Hashtable();
            ht.Add("杭州", "Assets/CityImages/hangzhou.jpg");
            ht.Add("北京", "Assets/CityImages/beijing.jpg");
            ht.Add("成都", "Assets/CityImages/chengdu.jpg");
            ht.Add("重庆", "Assets/CityImages/chongqing.jpg");
            ht.Add("广州", "Assets/CityImages/guangzhou.jpg");
            ht.Add("南京", "Assets/CityImages/nanjing.jpg");
            ht.Add("上海", "Assets/CityImages/shanghai.jpg");
            ht.Add("深圳", "Assets/CityImages/shenzhen.jpg");
            ht.Add("苏州", "Assets/CityImages/suzhou.jpg");
            ht.Add("天津", "Assets/CityImages/tianjin.jpg");
            ht.Add("乌鲁木齐", "Assets/CityImages/wulumuqi.jpg");
            ht.Add("西安", "Assets/CityImages/xian.jpg");
            
            //尚未完工
            return ht;
        }

        public static async Task<OneLocation> GetCityByLocationAsync(double lat,double lon,string TencentKey)
        {
            if(TencentKey == "0")
            {
                TencentKey = "BMCBZ-LANA3-7KD3I-36N2E-ZYBOF-QZFD4";
            }
            string uri = string.Format("http://apis.map.qq.com/ws/geocoder/v1/?location={0},{1}&key={2}&get_poi=1", lat,lon, TencentKey);
            HttpClient hc = new HttpClient();
            string back = await hc.GetStringAsync(uri);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OneLocation));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(back));
            OneLocation ol = (OneLocation)serializer.ReadObject(ms);
            return ol;
        }

        public static string GetCityName(OneLocation ol)
        {
            if("query ok" != ol.message)
            {
                return "bad TencentKey";
            }
            else
            {
                string cns;
                string dis = ol.result.address_component.district;
                string cn = ol.result.address_component.city;
                if (dis.Contains("县"))
                {
                    cns = dis.Remove(cn.LastIndexOf("县"));
                }
                else if (cn.Contains("市"))
                {
                    cns = cn.Remove(cn.LastIndexOf("市"));

                }
                else
                {
                    cns = cn;
                }
                return cns;
            }
            
        }

        
    }

    public class GeoProxy
    {
        public static async Task<bool> isGeoallowedAsync()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus != GeolocationAccessStatus.Allowed)
            {
                
                return false;
            }
            else
                return true;
        }

        public static async Task<Geoposition> GetGeoLocationAsync()
        {
            var geoLocator = new Geolocator { DesiredAccuracyInMeters = 1000 };
            var position = await geoLocator.GetGeopositionAsync();
            return position;
        }
    }
    
    [DataContract]
    public class Location
    {

        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class FormattedAddresses
    {

        [DataMember]
        public string recommend { get; set; }

        [DataMember]
        public string rough { get; set; }
    }

    [DataContract]
    public class AddressComponent
    {

        [DataMember]
        public string nation { get; set; }

        [DataMember]
        public string province { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string district { get; set; }

        [DataMember]
        public string street { get; set; }

        [DataMember]
        public string street_number { get; set; }
    }

    [DataContract]
    public class AdInfo
    {

        [DataMember]
        public string adcode { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public string nation { get; set; }

        [DataMember]
        public string province { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string district { get; set; }
    }

    [DataContract]
    public class FamousArea
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public int _distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class Crossroad
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public double distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class Village
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public double _distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class Town
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public int distance { get; set; }

        [DataMember]
        public string dirDesc { get; set; }
    }

    [DataContract]
    public class StreetNumber
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public double _distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class Street
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public double _distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class LandmarkL2
    {

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public int _distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class AddressReference
    {

        [DataMember]
        public FamousArea famous_area { get; set; }

        [DataMember]
        public Crossroad crossroad { get; set; }

        [DataMember]
        public Village village { get; set; }

        [DataMember]
        public Town town { get; set; }

        [DataMember]
        public StreetNumber street_number { get; set; }

        [DataMember]
        public Street street { get; set; }

        [DataMember]
        public LandmarkL2 landmark_l2 { get; set; }
    }
    
    [DataContract]
    public class AdInfo2
    {

        [DataMember]
        public string adcode { get; set; }

        [DataMember]
        public string province { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string district { get; set; }
    }

    [DataContract]
    public class Pois
    {

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string category { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public AdInfo2 ad_info { get; set; }

        [DataMember]
        public double _distance { get; set; }

        [DataMember]
        public string _dir_desc { get; set; }
    }

    [DataContract]
    public class Result
    {

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public FormattedAddresses formatted_addresses { get; set; }

        [DataMember]
        public AddressComponent address_component { get; set; }

        [DataMember]
        public AdInfo ad_info { get; set; }

        [DataMember]
        public AddressReference address_reference { get; set; }

        [DataMember]
        public int poi_count { get; set; }

        [DataMember]
        public Pois[] pois { get; set; }
    }

    [DataContract]
    public class OneLocation
    {

        [DataMember]
        public int status { get; set; }

        [DataMember]
        public string message { get; set; }

        [DataMember]
        public string request_id { get; set; }

        [DataMember]
        public Result result { get; set; }
    }


    
}
