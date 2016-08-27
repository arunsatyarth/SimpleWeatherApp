
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherController;
using NUnit.Framework;
namespace Test
{
    [NUnit.Framework.TestFixture]
    public class DataStoretest
    {
        public DataStoretest()
        {

        }
        [TestCase]
        public void TodaysWeather()
        {
            WeatherInput input = new WeatherInput();
            input.query = "Bangalore";
            input.num_of_days = "1";

            IGetWetherInfo weather = new WeatherRequestWWO(input);
            WeatherData data = weather.GetInformation();
            if (DataStore.Instance().m_start != null && DataStore.Instance().m_start.IsValid)
                NUnit.Framework.Assert.True(true);
            else
                NUnit.Framework.Assert.True(false);

        }
        [TestCase]
        public void PastWeather()
        {
            WeatherInput input = new WeatherInput();
            input.query = "Bangalore";
            input.num_of_days = "7";


            WeatherInput past = null;
            past = new WeatherInput();
            past.query = input.query;

            DateTime startdate = DateTime.Now;
            startdate = startdate.AddDays(-5);
            string date_s = startdate.ToString("yyyy-MM-dd");
            past.date = date_s;

            DateTime enddate = DateTime.Now;

            enddate = enddate.AddDays(-1);
            string enddate_s = enddate.ToString("yyyy-MM-dd");
            past.enddate = enddate_s;


            IGetWetherInfo weather = new WeatherRequestWWO(input, past);
            WeatherData data = weather.GetInformation();
            bool success = false;

            if (DataStore.Instance().m_start != null && DataStore.Instance().m_start.IsValid)
            {
                string firstdate=DataStore.Instance().m_start.datetime.ToString("yyyy-MM-dd");
                if (firstdate == date_s)
                    success = true;
            }
            NUnit.Framework.Assert.True(success);

        }
        [TestCase]
        public void AddLeft()
        {
            WeatherInput input = new WeatherInput();
            input.query = "Bangalore";
            input.num_of_days = "7";
            IGetWetherInfo weather = new WeatherRequestWWO(input);
            WeatherData data = weather.GetInformation();
            Node current=DataStore.Instance().m_start;
            WeatherInput past = null;
            past = new WeatherInput();
            past.query = input.query;

            DateTime startdate = DateTime.Now;
            startdate = startdate.AddDays(-5);
            string date_s = startdate.ToString("yyyy-MM-dd");
            past.date = date_s;

            DateTime enddate = DateTime.Now;

            enddate = enddate.AddDays(-1);
            string enddate_s = enddate.ToString("yyyy-MM-dd");
            past.enddate = enddate_s;


            IGetWetherInfo weather2 = new WeatherRequestWWO(input, past,RefreshType.AddLeft);
            WeatherData data2 = weather2.GetInformation();
            bool success = false;

            if (DataStore.Instance().m_start != null && DataStore.Instance().m_start.IsValid)
            {
                Node temp = DataStore.Instance().m_start;
                int count = 1;
                bool added5 = false;
                while (temp != null)
                {
                    temp = temp.next;
                    if (count == 5 && temp == current)
                        added5 = true;
                    count++;

                }
                if (count == 13&& added5)
                    success = true;
            }
            NUnit.Framework.Assert.True(success);

        }
        [TestCase]
        public void FalseInput_Negative()
        {
            WeatherInput input = new WeatherInput();
            input.query = "...";
            input.num_of_days = "-1";

            IGetWetherInfo weather = new WeatherRequestWWO(input);
            WeatherData data = weather.GetInformation();
            if (DataStore.Instance().m_start != null)
                NUnit.Framework.Assert.True(true);
            else
                NUnit.Framework.Assert.True(false);

        }
    }
}
