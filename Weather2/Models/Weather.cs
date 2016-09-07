using System;
using System.Collections.Generic;
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
    internal class City
    {
        [DataMember]
        public string aqi { get; set; }

        [DataMember]
        public string co { get; set; }

        [DataMember]
        public string no2 { get; set; }

        [DataMember]
        public string o3 { get; set; }

        [DataMember]
        public string pm10 { get; set; }

        [DataMember]
        public string pm25 { get; set; }

        [DataMember]
        public string qlty { get; set; }

        [DataMember]
        public string so2 { get; set; }
    }

    [DataContract]
    internal class Aqi
    {

        [DataMember]
        public City city { get; set; }
    }

    [DataContract]
    internal class Update
    {

        [DataMember]
        public string loc { get; set; }

        [DataMember]
        public string utc { get; set; }
    }

    [DataContract]
    internal class Basic
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
        public Update update { get; set; }
    }

    [DataContract]
    internal class Astro
    {

        [DataMember]
        public string sr { get; set; }

        [DataMember]
        public string ss { get; set; }
    }

    [DataContract]
    internal class Cond
    {

        [DataMember]
        public string code_d { get; set; }

        [DataMember]
        public string code_n { get; set; }

        [DataMember]
        public string txt_d { get; set; }

        [DataMember]
        public string text_n { get; set; }
    }

    [DataContract]
    internal class Tmp
    {

        [DataMember]
        public string max { get; set; }

        [DataMember]
        public string min { get; set; }
    }

    [DataContract]
    internal class Wind
    {

        [DataMember]
        public string deg { get; set; }

        [DataMember]
        public string dir { get; set; }

        [DataMember]
        public string sc { get; set; }

        [DataMember]
        public string spd { get; set; }
    }

    [DataContract]
    internal class DailyForecast
    {

        [DataMember]
        public Astro astro { get; set; }

        [DataMember]
        public Cond cond { get; set; }

        [DataMember]
        public string date { get; set; }

        [DataMember]
        public string hum { get; set; }

        [DataMember]
        public string pcpn { get; set; }

        [DataMember]
        public string pop { get; set; }

        [DataMember]
        public string pres { get; set; }

        [DataMember]
        public Tmp tmp { get; set; }

        [DataMember]
        public string vis { get; set; }

        [DataMember]
        public Wind wind { get; set; }
    }

    [DataContract]
    internal class Wind2
    {

        [DataMember]
        public string deg { get; set; }

        [DataMember]
        public string dir { get; set; }

        [DataMember]
        public string sc { get; set; }

        [DataMember]
        public string spd { get; set; }
    }

    [DataContract]
    internal class HourlyForecast
    {

        [DataMember]
        public string date { get; set; }

        [DataMember]
        public string hum { get; set; }

        [DataMember]
        public string pop { get; set; }

        [DataMember]
        public string pres { get; set; }

        [DataMember]
        public string tmp { get; set; }

        [DataMember]
        public Wind2 wind { get; set; }
    }

    [DataContract]
    internal class Cond2
    {

        [DataMember]
        public string code { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Wind3
    {

        [DataMember]
        public string deg { get; set; }

        [DataMember]
        public string dir { get; set; }

        [DataMember]
        public string sc { get; set; }

        [DataMember]
        public string spd { get; set; }
    }

    [DataContract]
    internal class Now
    {

        [DataMember]
        public Cond2 cond { get; set; }

        [DataMember]
        public string fl { get; set; }

        [DataMember]
        public string hum { get; set; }

        [DataMember]
        public string pcpn { get; set; }

        [DataMember]
        public string pres { get; set; }

        [DataMember]
        public string tmp { get; set; }

        [DataMember]
        public string vis { get; set; }

        [DataMember]
        public Wind3 wind { get; set; }
    }

    [DataContract]
    internal class Comf
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Cw
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Drsg
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Flu
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Sport
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Trav
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Uv
    {

        [DataMember]
        public string brf { get; set; }

        [DataMember]
        public string txt { get; set; }
    }

    [DataContract]
    internal class Suggestion
    {

        [DataMember]
        public Comf comf { get; set; }

        [DataMember]
        public Cw cw { get; set; }

        [DataMember]
        public Drsg drsg { get; set; }

        [DataMember]
        public Flu flu { get; set; }

        [DataMember]
        public Sport sport { get; set; }

        [DataMember]
        public Trav trav { get; set; }

        [DataMember]
        public Uv uv { get; set; }
    }

    [DataContract]
    internal class HeWeathedataService30
    {

        [DataMember]
        public Aqi aqi { get; set; }

        [DataMember]
        public Basic basic { get; set; }

        [DataMember]
        public DailyForecast[] daily_forecast { get; set; }

        [DataMember]
        public HourlyForecast[] hourly_forecast { get; set; }

        [DataMember]
        public Now now { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public Suggestion suggestion { get; set; }
    }

    [DataContract]
    internal class Weather
    {
        [DataMember]
        public HeWeathedataService30[] data { get; set; }
    }

    internal class WeatherProxy
    {
        //这个网址是和风天气的api说明，http://www.heweather.com/documents/api

        //传入cityId，返回Weather类实例
        public static async Task<Weather> GetWeatherByCityIdAsync(string cityId,string heWeatherKey)
        {
            if("0" == heWeatherKey || null == heWeatherKey )
            {
                heWeatherKey = "69ebef9bc6c6480987683f57318298aa";
            }
            string httpUri = String.Format("https://api.heweather.com/x3/weather?cityid={0}&key={1}", cityId, heWeatherKey);
            HttpClient hc = new HttpClient();

            //由于传回的string中命名带有空格，不得已替换之
            string str = await hc.GetStringAsync(new Uri(httpUri));
            string str2 = str.Substring(29);
            string str3 = "{\"data\""+str2;
            var serializer = new DataContractJsonSerializer(typeof(Weather));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str3));
            Weather myWeather = (Weather)serializer.ReadObject(ms);
            return myWeather;
        }
    }
}
