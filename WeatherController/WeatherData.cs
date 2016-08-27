using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherController
{
    /// <summary>
    /// Stores the final fetched date from weather service
    /// </summary>
    public class WeatherData
    {
        public WeatherInput m_weatherInput { get; set; }
        public string m_temperature { get; set; }
        public int m_temperature_int { get; set; }
        public string m_temperature_min { get; set; }
        public string m_temperature_max { get; set; }
        public string m_humidity { get; set; }
        public int m_rainchance { get; set; }
        public DateTime m_date { get; set; }
        public string m_location { get; set; }
        public int m_cloudcover { get; set; }
        public string m_precipitationInches { get; set; }
        public int m_visibility { get; set; }
        public string m_windspeed { get; set; }
        public string m_weatherdescription { get; set; }
        public bool m_error { get; set; }

        
        public string m_text = "";
        public void AssignDummy()
        {
            m_temperature = "-275";
            m_temperature_int = -275;
            m_temperature_min = "-275";
            m_temperature_max = "-275";
            m_humidity = "0";
            m_rainchance = 0;
            m_date = DateTime.Now;
            m_location = "Bangalore";
            m_cloudcover = 0;
            m_visibility = 0;

            m_windspeed = "-275";
            m_weatherdescription = "Sunny";
            m_error = false;

        }
    }
}
