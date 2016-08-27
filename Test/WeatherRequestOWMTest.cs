
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherController;
using NUnit.Framework;
namespace Test
{
    [NUnit.Framework.TestFixture]
    public class WeatherRequestOWATest
    {
        public WeatherRequestOWATest()
        {

        }
        [TestCase]
        public void TodaysWeather()
        {
            WeatherInput input = new WeatherInput();
            input.query = "Bangalore";
            input.num_of_days = "1";

            IGetWetherInfo weather = new WeatherRequestZOWA(input);
            WeatherData data = weather.GetInformation();
            if (DataStore.Instance().m_start != null)
                NUnit.Framework.Assert.True(true);
            else
                NUnit.Framework.Assert.True(false);

        }
       
        [TestCase]
        public void FalseInput_Negative()
        {
            WeatherInput input = new WeatherInput();
            input.query = "...";
            input.num_of_days = "-1";

            IGetWetherInfo weather = new WeatherRequestZOWA(input);
            WeatherData data = weather.GetInformation();
            if (DataStore.Instance().m_start != null)
                NUnit.Framework.Assert.True(true);
            else
                NUnit.Framework.Assert.True(false);

        }
    }
}
