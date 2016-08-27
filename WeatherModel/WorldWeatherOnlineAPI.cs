using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Script.Serialization;
using premium.localweather;
using premium.pastweather;

namespace WeatherModel
{
    public class WorldWeatherOnlineAPI
    {
        public string ApiBaseURL = ConfigurationManager.AppSettings["WWOurl"];
        public string PremiumAPIKey = ConfigurationManager.AppSettings["WWOKey"];
        public WorldWeatherOnlineAPI()
        {
            //if app.config was not found then use these defaults
            if (ApiBaseURL == null || ApiBaseURL == "")
                ApiBaseURL = WeatherModel.Resource1.wwourl;
            if (PremiumAPIKey == null || PremiumAPIKey == "")
                PremiumAPIKey = WeatherModel.Resource1.wwokey;
        }
        public LocalWeather GetLocalWeather(LocalWeatherInput input)
        {

            // create URL based on input paramters
            string apiURL = ApiBaseURL + "weather.ashx?q=" + input.query + "&format=" + input.format + "&extra=" + input.extra + "&num_of_days=" + input.num_of_days + "&date=" + input.date + "&fx=" + input.fx + "&cc=" + input.cc + "&includelocation=" + input.includelocation + "&show_comments=" + input.show_comments + "&callback=" + input.callback + "&key=" + PremiumAPIKey;
            // get the web response
            string result = new RequestHandler(apiURL).Process();

            // deserialize the json output into the c#classes created earlier
            LocalWeather lWeather = null;
            if(result!=null)
                result = result.Replace("No moonset", "11:47 AM");
            try
            {
                lWeather = (LocalWeather)new JavaScriptSerializer().Deserialize(result, typeof(LocalWeather));

            }
            catch (FormatException ex)
            {

            }
            //LocalWeather lWeather = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalWeather>(result);
            return lWeather;
        }




        public PastWeather GetPastWeather(PastWeatherInput input)
        {
            // create URL based on input paramters
            string apiURL = ApiBaseURL + "past-weather.ashx?q=" + input.query + "&format=" + input.format + "&extra=" + input.extra + "&enddate=" + input.enddate + "&date=" + input.date + "&includelocation=" + input.includelocation + "&callback=" + input.callback + "&key=" + PremiumAPIKey;

            // get the web response
            string result = new RequestHandler(apiURL).Process();

            // deserialize the json output into the c#classes created earlier
            PastWeather pWeather = null;
            if (result != null)
                result = result.Replace("No moonset", "11:47 AM");
            try
            {
                pWeather = (PastWeather)new JavaScriptSerializer().Deserialize(result, typeof(PastWeather));

            }
            catch (FormatException ex)
            {

            }

            return pWeather;
        }

    }
}