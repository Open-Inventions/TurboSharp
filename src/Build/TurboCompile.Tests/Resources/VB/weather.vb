'#r "nuget: Newtonsoft.Json, 13.0.2"
'#r "nuget: RestSharp, 108.0.3"

Imports System
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports RestSharp

Module Program
    Sub Main()
        Const url = "https://www.prevision-meteo.ch/services/json/lat=46.259lng=5.235"

        Using client = New RestClient()
            Dim request = New RestRequest(url)
            Dim response = client.Get(request)

            Dim result = JsonConvert.DeserializeObject(Of JObject)(response.Content)
            Dim info = result("forecast_info")
            Dim lat = info("latitude")
            Dim lon = info("longitude")

            Dim cond = result("current_condition")
            Dim vDate = cond("date")
            Dim hour = cond("hour")
            Dim temp = cond("tmp")
            Dim humidity = cond("humidity")

            Console.WriteLine($" [{lat}; {lon}] {vDate} {hour}, {temp} °C, {humidity} %")
        End Using
    End Sub
End Module
