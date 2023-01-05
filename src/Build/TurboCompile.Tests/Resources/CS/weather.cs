//#r "nuget: Newtonsoft.Json, 13.0.2"
//#r "nuget: RestSharp, 108.0.3"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Resty
{
    internal static class Program
    {
        private static void Main()
        {
            const string url = "https://www.prevision-meteo.ch/services/json/lat=46.259lng=5.235";

            using var client = new RestClient();
            var request = new RestRequest(url);
            var response = client.Get(request);

            var result = JsonConvert.DeserializeObject<JObject>(response.Content!)!;
            var info = result["forecast_info"]!;
            var lat = info["latitude"];
            var lon = info["longitude"];

            var cond = result["current_condition"]!;
            var date = cond["date"];
            var hour = cond["hour"];
            var temp = cond["tmp"];
            var humidity = cond["humidity"];

            Console.WriteLine($" [{lat}; {lon}] {date} {hour}, {temp} °C, {humidity} %");
        }
    }
}
