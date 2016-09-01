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
        static string myKey = "69ebef9bc6c6480987683f57318298aa";
        //获得城市和id的对应hashtable
        public static async Task<Hashtable> GetCityHashtableAsync()
        {
            CityList myCityList = await GetCityListAsync();
            Hashtable cityHash = new Hashtable();
            foreach(CityInfo ci in myCityList.city_info)
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

        public static async Task<CityList> GetCityListAsync()
        {
            string httpUri = String.Format("https://api.heweather.com/x3/citylist?search={0}&key={1}", "allchina", myKey);
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
            popularCities.Add(new CityInfo("衢州"));            
            popularCities.Add(new CityInfo("开化"));

            return popularCities;
        }

        public static Hashtable GetPopularCityBackground()
        {
            Hashtable ht = new Hashtable();
            ht.Add("杭州", "Assets/CityImages/hangzhou.jpg");
            //尚未完工
            return ht;
        }
    }
}
