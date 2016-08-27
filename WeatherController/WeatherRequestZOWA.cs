using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using premium.localweather;
using premium.pastweather;
using WWOLib.OpenWeatherAPI;
namespace WeatherController
{
    /// <summary>
    /// Fetches weather info from OpenWeatherAPI.com. this is the second class in the chain of responsibility
    /// will be called when the first one fails
    /// </summary>
    public class WeatherRequestZOWA : IGetWetherInfo
    {
        private WeatherData txtOutput;
        WeatherInput m_info_historical;
        WeatherInput m_info_future;
        RefreshType m_reftype;
        IGetWetherInfo m_weatherinfo = null;
        public WeatherRequestZOWA(WeatherInput future, WeatherInput past = null, RefreshType reftype = RefreshType.FullRefresh)
        {
               m_info_future = future;
            m_info_historical = past;
            m_reftype = reftype;
            m_weatherinfo = new WeatherNullPtr();
            
        }
        public WeatherData GetInformation()
        {
            WeatherData data = null;
            data = GetInfo();
            if (data == null)
                data = m_weatherinfo.GetInformation();//call next in chain if required
            return data;
        }
        public void SetNextChain(IGetWetherInfo next)
        {
            m_weatherinfo = next;
        }
        private WeatherData GetInfo()
        {
            PastWeather pastWeather = null;
            WeatherData data = new WeatherData();
            // set input parameters for the API
            LocalWeatherInput input = new LocalWeatherInput();
            input.query = m_info_future.query;
            input.num_of_days = m_info_future.num_of_days;
            input.date = m_info_future.date;
            input.format = "JSON";

            // call the local weather method with input parameters
            OpenWeatherAPI api = new OpenWeatherAPI();
            LocalWeather localWeather = api.GetLocalWeather(input);
            

            //set data in datastore based on which kind of datastore refresh was requested
            if (m_reftype == RefreshType.AddLeft)
                DataStore.Instance().AddLeft(pastWeather);
            else if (m_reftype == RefreshType.AddLeft)
                DataStore.Instance().AddRight(localWeather);
            else
                DataStore.Instance().SetWeather(localWeather, pastWeather);
            
            return data;
        }


    }
}
