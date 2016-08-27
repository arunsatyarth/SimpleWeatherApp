using premium.localweather;
using premium.pastweather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherModel;

namespace WeatherController
{
    /// <summary>
    /// Fetches weather info from WorldWeatherOnline.com. this is the first class in the chain of responsibility
    /// </summary>
    public class WeatherRequestWWO : IGetWetherInfo
    {
        private WeatherData txtOutput;
        WeatherInput m_info_historical;
        WeatherInput m_info_future;
        RefreshType m_reftype;
        IGetWetherInfo m_weatherinfo = null;
        public WeatherRequestWWO(WeatherInput future,WeatherInput past=null,RefreshType reftype=RefreshType.FullRefresh )
        {
            m_info_future = future;
            m_info_historical = past;
            m_reftype = reftype;
            m_weatherinfo = new WeatherNullPtr();//This is a NUll PTr Pattern so that there would always be a next actor and not a null pointer
            
        }
        public WeatherData GetInformation()
        {
            WeatherData data=null;
            //call local function to see if we can get weather info
            data = GetInfo();
            //if local function cannot process this, hand it over to next actor in the chain
            if (data == null)
                data = m_weatherinfo.GetInformation();
            return data;
        }
        public  void SetNextChain(IGetWetherInfo next)
        {
            m_weatherinfo = next;
        }
        private WeatherData GetInfo()
        {
            PastWeather pastWeather = null;
            WeatherData data=new WeatherData();
            // set input parameters for the API
            LocalWeatherInput input = new LocalWeatherInput();
            input.query = m_info_future.query;
            input.num_of_days = m_info_future.num_of_days;
            input.date = m_info_future.date;
            input.format = "JSON";

            // call the local weather method with input parameters
            WorldWeatherOnlineAPI api = new WorldWeatherOnlineAPI();
            LocalWeather localWeather = api.GetLocalWeather(input);

            if(m_info_historical!=null)
            {
                //request past data if needed
                PastWeatherInput h_input = new PastWeatherInput();
                h_input.query = m_info_historical.query;
                h_input.date = m_info_historical.date;
                h_input.enddate = m_info_historical.enddate;
                h_input.format = "JSON";
                pastWeather = api.GetPastWeather(h_input);

            }
            //set data in datastore based on which kind of datastore refresh was requested

            if(m_reftype==RefreshType.AddLeft)
                DataStore.Instance().AddLeft( pastWeather);
            else if (m_reftype == RefreshType.AddLeft)
                DataStore.Instance().AddRight(localWeather);
            else
                DataStore.Instance().SetWeather(localWeather, pastWeather);

            return data;
        }


    }
}
