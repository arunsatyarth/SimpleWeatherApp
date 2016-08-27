using premium.localweather;
using premium.pastweather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherController
{
    /// <summary>
    /// This stores the weather information that we have fetched as a DOUBLY LINKED LIST
    /// is singleton 
    /// We can traverse the doubly linked list back or forward to show data of any day that we have collected
    /// 
    /// </summary>
    public class DataStore
    {
        public Node m_today { get; set; }
        public Node m_start { get; set; }
        public Node m_end { get; set; }
        public Node m_left { get; set; }
        public Node m_right { get; set; }
        private static object singletonsynch = new object();
        private static object accesssynch = new object();
        static DataStore m_singleobj;
        private DataStore()
        {
            Clear();
        }
        private void HandleError()
        {
            if(m_start==null)
            {
                Node dummy=new Node();
                m_start = dummy;
                m_end =   dummy;
                m_left =  dummy;
                m_today = dummy;
                m_right = dummy;
                dummy.AssignDummy();
            }
        }
        /// <summary>
        /// Creates a weather data object from the output of past API call. we might get tons of data from api but we only need to use few
        /// </summary>
        /// <param name="weather"></param>
        /// <returns></returns>
        WeatherData fillDataPast( premium.pastweather.Weather weather)
        {
            WeatherData data = new WeatherData();
            data.m_date = weather.date;
            data.m_humidity = weather.hourly[0].humidity.ToString();
            data.m_cloudcover = weather.hourly[0].cloudcover;
            data.m_precipitationInches = weather.hourly[0].precipInches.ToString();
            data.m_temperature_int = weather.hourly[0].tempC;

            data.m_temperature = string.Format("{0}°C", weather.hourly[0].tempC);
            data.m_temperature_max = string.Format("{0}°C", weather.maxtempC);
            data.m_temperature_min = string.Format("{0}°C", weather.mintempC);
            data.m_visibility = weather.hourly[0].visibility;
            data.m_weatherdescription = weather.hourly[0].weatherDesc[0].value;
            return data;
        }
        /// <summary>
        /// Creates a weather data object from the output of futire API call. we might get tons of data from api but we only need to use few
        /// </summary>
        /// <param name="weather"></param>
        /// <returns></returns>
        WeatherData fillDataFuture(premium.localweather.Weather weather)
        {
            WeatherData data = new WeatherData();
            data.m_date = weather.date;
            data.m_humidity = weather.hourly[0].humidity.ToString();
            data.m_cloudcover = weather.hourly[0].cloudcover;
            data.m_precipitationInches = weather.hourly[0].precipInches.ToString();
            data.m_temperature_int = weather.hourly[0].tempC;

            data.m_temperature = string.Format("{0}°C", weather.hourly[0].tempC);
            data.m_temperature_max = string.Format("{0}°C", weather.maxtempC);
            data.m_temperature_min = string.Format("{0}°C", weather.mintempC);
            data.m_visibility = weather.hourly[0].visibility;
            data.m_weatherdescription = weather.hourly[0].weatherDesc[0].value;
            data.m_rainchance = weather.hourly[0].chanceofrain;
            return data;
        }
        /// <summary>
        /// Main interface to set weather information to be used later.
        /// If input is valid, we clear all current data and set new data
        /// </summary>
        /// <param name="future"></param>
        /// <param name="past"></param>
        public void SetWeather(LocalWeather future, PastWeather past = null)
        {
            lock (accesssynch)
            {
                if (future == null || future.data == null || future.data.weather == null)
                {
                    HandleError();
                    return;
                }
                Clear();
                if (past != null)//for historical data
                {
                    List<premium.pastweather.Weather> pastweathers = past.data.weather;
                    foreach (premium.pastweather.Weather weather in pastweathers)
                    {
                        //create input to usable and storable weatherdata object
                        WeatherData data = fillDataPast(weather);
                        data.m_location = past.data.request[0].query;


                        data.m_rainchance = 0;
                        //Create a doubly Linked list chain to store weather of each date
                        Node node = new Node();
                        node.data = data;
                        node.datetime = data.m_date;
                        if (m_start == null)
                        {
                            m_start = node;
                            m_left = node;
                        }
                        else
                        {
                            m_end.next = node;
                            node.prev = m_end;
                        }
                        m_right = node;
                        m_end = node;


                    }
                }
                if (future.data.weather != null)//for future data
                {
                    List<premium.localweather.Weather> localweathers = future.data.weather;
                    foreach (premium.localweather.Weather weather in localweathers)
                    {
                        WeatherData data = fillDataFuture(weather);
                        data.m_location = future.data.request[0].query;
                        //Create a doubly Linked list chain to store weather of each date
                        Node node = new Node();
                        node.data = data;
                        node.datetime = data.m_date;
                        if (m_today == null)
                        {
                            m_today = node;
                        }
                        if (m_start == null)
                        {
                            m_start = node;
                            m_left = node;
                        }
                        else
                        {
                            m_end.next = node;
                            node.prev = m_end;
                        }
                        m_right = node;
                        m_end = node;

                    }
                }
            }


        }
        /// <summary>
        /// Adds information to the right of Linked list. ie if we want to fetch data far into the future
        /// </summary>
        /// <param name="future"></param>
        public void AddRight(LocalWeather future)
        {
            lock (accesssynch)
            {
                if (future.data.weather == null)
                {
                    HandleError();
                    return;
                }
                if (m_end == null)
                {
                    SetWeather(future, null);
                    return;
                }
                List<premium.localweather.Weather> localweathers = future.data.weather;
                foreach (premium.localweather.Weather weather in localweathers)
                {
                    WeatherData data = fillDataFuture(weather);
                    data.m_location = future.data.request[0].query;

                    Node node = new Node();
                    node.data = data;
                    node.datetime = data.m_date;

                    m_end.next = node;
                    node.prev = m_end;
                    m_end = node;
                }
                moveright();
            }
        }
        /// <summary>
        /// Adds information to the left of Linked list. ie if we want to fetch historical data far into the past
        /// </summary>
        /// <param name="future"></param>
        public void AddLeft(PastWeather past)
        {
            lock (accesssynch)
            {
                if (m_start == null || past.data.weather == null)//we cant recover from this situation
                {
                    HandleError();
                    return;
                }
                List<premium.pastweather.Weather> pastweathers = past.data.weather;
                Node first = null;
                Node last = null;
                foreach (premium.pastweather.Weather weather in pastweathers)
                {
                    WeatherData data = fillDataPast(weather);
                    data.m_location = past.data.request[0].query;

                    //Todo: determine this value
                    data.m_rainchance = 0;
                    Node node = new Node();
                    node.data = data;
                    node.datetime = data.m_date;
                    if (first == null)
                    {
                        first = node;
                        last = first;
                    }
                    else
                    {
                        last.next = node;
                        node.prev = last;
                        last = node;
                    }



                }

                last.next = m_start;
                m_start.prev = last;
                m_start = first;
                moveleft();
            }
        }


        //Singleton with thread synch
        public static DataStore Instance()
        {
            if (m_singleobj != null)
                return m_singleobj;
            else
            {
                lock (singletonsynch)//Locking singleton
                {
                    if (m_singleobj == null)
                        m_singleobj = new DataStore();
                    return m_singleobj;
                }

            }
        }
        private void Clear()
        {
            m_start = null;
            m_end = null;
            m_left = null;
            m_today = null;
            m_right = null;
        }
        public void moveleft()
        {
            lock (accesssynch)
            {
                m_right = m_right.prev;
                m_left = m_left.prev;
            }
        }
        public void moveright()
        {
            lock (accesssynch)
            {
                m_right = m_right.next;
                m_left = m_left.next;
            }
        }

    }
    /// <summary>
    /// Node for the doubly linked list. One node representd 1 day
    /// </summary>
    public class Node
    {

        public WeatherData data { get; set; }//weather for that day
        public DateTime datetime { get; set; }//date for that day
        public Node next { get; set; }
        public Node prev { get; set; }
        public bool IsValid { //sometimes we can have dummy nodes to protect from NullrefExceptions. chck this to ensure validity
            get{
                return m_isvalid;
            } 
        }
        bool m_isvalid = true;
        public void AssignDummy()//NullPtr pattern where we assign dummy valueless node with validity set as false
        {
            m_isvalid = false;
            data = new WeatherData();
            data.AssignDummy();
        }
    }
}
