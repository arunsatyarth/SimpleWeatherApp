using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherController
{
    public delegate void UIUpdaterDelegate(WeatherData data);

    /// <summary>
    /// All info required for fetching weather info 
    /// viz location, days etc
    /// </summary>
    public class WeatherInput
    {
        public string query { get; set; }
        public string format { get; set; }
        public string extra { get; set; }
        public string num_of_days { get; set; }
        public string date { get; set; }
        public string enddate { get; set; }
        public string fx { get; set; }
        public string cc { get; set; }
        public string includelocation { get; set; }
        public string show_comments { get; set; }
        public string tp { get; set; }
        public string callback { get; set; }
        public string ToString(WeatherInput key)
        {
            string h_key = key.query + key.num_of_days.ToString();
            return h_key;
        }
    }


    /// <summary>
    /// We Follow a COMMAND pattern
    /// Everything that the UI wants to do is done asynchronously by wrapping it in a command and pushing to the queue
    ///     /// </summary>
    public class Command
    {
        WeatherController m_controller;
        UIUpdaterDelegate m_callback;
        WeatherInput m_info_historical;
        WeatherInput m_info_future;

        RefreshType m_refresh;
        public Command()
        {

        }
        public void SetInput(WeatherInput future,WeatherInput past=null,RefreshType reftype=RefreshType.FullRefresh)
        {
            m_info_future = future;
            m_info_historical = past;
            m_refresh = reftype;
        }
        public void SetCallback(UIUpdaterDelegate callback)
        {
            m_callback = callback;
        }
        /// <summary>
        /// When this command is pushed to commandqueue, the commandexecutor will wakeup and retrieve this command, 
        /// It wil then call the Execute method which will do what this coimmand is suppose dto dio
        /// </summary>
        public void Execute()
        {
            m_controller = new WeatherController(m_info_future, m_info_historical,m_refresh);
            WeatherData data = null;
            try
            {
                data = m_controller.FetchWeather();

            }
            catch (ApiKeyException e)
            {
                data = new WeatherData();
                data.m_error = true;
                m_callback(data);
                
            }
            m_callback(data);
        }

    }
    public enum RefreshType//The weather you want to add new data or only add to left or right in datastore
    {
        FullRefresh,
        AddLeft,
        AddRight
    }
}
