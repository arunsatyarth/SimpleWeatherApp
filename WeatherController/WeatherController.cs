//using PersistenceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherController
{
    /// <summary>
    /// Handles request from the Command patterns for fetching the info
    /// It builds a chain of responsilbility of other classes to which it delegates the responsibility to fetch the info
    /// </summary>
    class WeatherController
    {
        IGetWetherInfo m_weatherinfo = null;
        WeatherInput m_info_historical;
        WeatherInput m_info_future;
        RefreshType m_reftype;
        public WeatherData FetchWeather()
        {
            WeatherData data = null;
            
            data = m_weatherinfo.GetInformation();//fetches and returns the data

            return data;
        }
        public WeatherController(WeatherInput future, WeatherInput past = null, RefreshType reftype = RefreshType.FullRefresh)
        {
            m_info_future = future;
            m_info_historical = past;
            m_reftype = reftype;
            BuildResponsibilityChain();
        }
        void BuildResponsibilityChain()
        {
            //Chain of responsibility pattern where 2 classes are given responsibility to collect Weather data
            IGetWetherInfo first = new WeatherRequestWWO(m_info_future,m_info_historical,m_reftype);
            IGetWetherInfo second = new WeatherRequestZOWA(m_info_future, m_info_historical, m_reftype);
            first.SetNextChain(second);
            m_weatherinfo = first;
        }
        
    }
}
