using premium.localweather;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
namespace WWOLib.OpenWeatherAPI
{
    public class OpenWeatherAPI
    {
        public string m_error = "";

        public LocalWeather GetLocalWeather(LocalWeatherInput input)
        {

            string url = ConfigurationManager.AppSettings["OWAurl"];
            string key = ConfigurationManager.AppSettings["OWAKey"];
            if (url == null || url == "")
                url = WeatherModel.Resource1.owapiurl;
            if (key == null || key == "")
                key = WeatherModel.Resource1.owakey;
            // create URL based on input paramters
            string apiURL = url + "&q="+ input.query +"&appid=" + key;

            // get the web response
            string result = new RequestHandler(apiURL).Process();


            // deserialize the json output into the c#classes created earlier
            RootObject lWeather = null;
            try
            {
                //lWeather = (LocalWeather)new JavaScriptSerializer().Deserialize(result, typeof(LocalWeather));
                lWeather = (RootObject)new JavaScriptSerializer().Deserialize(result, typeof(RootObject));

            }
            catch (ArgumentNullException ex)
            {
                m_error = ex.Message;
            }
            catch (FormatException ex)
            {
                m_error = ex.Message;

            }
            //LocalWeather lWeather = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalWeather>(result);
            return Adapt(lWeather);
        }
        public LocalWeather Adapt(RootObject obj)
        {
            LocalWeather lobj=null;
            if (obj == null)
                return null;
            try
            {
                lobj = new LocalWeather();
                lobj.data = new Data();
                lobj.data.current_Condition = new List<Current_Condition>();
                lobj.data.current_Condition.Add(new Current_Condition());
                lobj.data.weather = new List<premium.localweather.Weather>();
                lobj.data.weather.Add(new premium.localweather.Weather());
                lobj.data.weather[0].hourly = new List<Hourly>();
                lobj.data.weather[0].hourly.Add(new premium.localweather.Hourly());
                lobj.data.weather[0].hourly[0].weatherDesc = new List<WeatherDesc>();
                lobj.data.weather[0].hourly[0].weatherDesc.Add(new premium.localweather.WeatherDesc());

                lobj.data.request = new List<Request>();
                lobj.data.request.Add(new Request());
                lobj.data.request[0].query = obj.name;

                lobj.data.weather[0].hourly[0].tempC = (int)obj.main.temp;
                lobj.data.weather[0].maxtempC = (int)obj.main.temp_max;
                lobj.data.weather[0].mintempC = (int)obj.main.temp_min;

                lobj.data.weather[0].hourly[0].weatherDesc[0].value = obj.weather[0].main;

            }
            catch (NullReferenceException e)
            {
                lobj = null;
            }
            catch (Exception e)
            {
                lobj = null;

            }

            return lobj;
        }
    }
}
