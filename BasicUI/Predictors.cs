using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherController;

namespace BasicUI
{
    /// <summary>
    /// Interface to bind actors who can predict any type of behaviour
    /// We use DECORATOR pattern to bring together many classes which work together to predict the weather
    /// </summary>
    interface IPredictors
    {
        Prediction Predict();
        WeatherData getData();
    }
    /// <summary>
    /// 
    /// Take the input from other predictors and add your own logic to predict the rain
    /// 
    /// </summary>
    class RainPredictor:IPredictors
    {
        IPredictors m_nextpredictor;//next predictor in the decorator 
        WeatherData m_data;

        public RainPredictor(IPredictors p)
        {
            m_nextpredictor = p;
            m_data = m_nextpredictor.getData();
        }
        public WeatherData getData()
        {
            return m_data;
        }
        public Prediction Predict()
        {
            Prediction p=null;
            if(m_nextpredictor!=null)
                p= m_nextpredictor.Predict();
            else
                p=new Prediction();
            if (m_nextpredictor.getData().m_rainchance > 30 && p.cloud > CloudLevel.Cloudy && p.smiley<Smiley.ReallySad)
            {
                p.cloud = CloudLevel.MildRain;
                p.rain = 1;
                p.advise="Rain is Certain. Please Take an Umbrella";
                p.smiley = Smiley.Sad;
            }
            else if (m_nextpredictor.getData().m_rainchance > 50 && p.cloud > CloudLevel.Cloudy && p.smiley < Smiley.Angry)
            {
                p.cloud = CloudLevel.ThunderStorm;
                p.rain = 2;
                p.advise = "Thunderstorms predicted. Please stay indoors";
                p.smiley = Smiley.ReallySad;

            }
            return p;

        }

    }
    /// <summary>
    /// This takes input from temparature predictor and predicts the layout of clouds
    /// </summary>
    class CloudPredictor:IPredictors
    {
        IPredictors m_nextpredictor;//next predictor in the decorator 
        WeatherData m_data;
        public WeatherData getData()
        {
            return m_data;
        }
        public CloudPredictor(IPredictors p)
        {
            m_nextpredictor = p;
            m_data = m_nextpredictor.getData();

        }
        public Prediction Predict()
        {
            Prediction p=null;
            if(m_nextpredictor!=null)
                p= m_nextpredictor.Predict();
            else
                p=new Prediction();
            if (m_data.m_cloudcover > 95 && p.smiley<Smiley.Sad )
            {
                p.cloud = CloudLevel.OverCast;
                if (p.smiley < Smiley.Sad)
                {
                    p.advise = "Weather cloudy. Might rain";
                    p.smiley = Smiley.OK;
                }
            }
            else if (m_data.m_cloudcover > 70 )
            {
                    p.cloud = CloudLevel.Cloudy;
                    if (p.smiley < Smiley.Sad)
                    {
                        p.advise = "Weather looks cloudy and sweet. Might not rain";
                        p.smiley = Smiley.OK;
                    }
            }
            else if (m_data.m_cloudcover > 45  )
            {

                p.cloud = CloudLevel.PartlyCloudy;
                if(p.smiley < Smiley.OK){
                    p.advise = "Cloudy with a chance of rainbows";
                    p.smiley = Smiley.Happy;
                }
            }
            else
            {
                p.smiley = Smiley.Happy;
                p.cloud = CloudLevel.Sunny;

            }
            return p;

        }
    }
    /// <summary>
    /// This is the lowest in the decorator which just looks at the temparature and sets the mood
    /// But It cannot predict weather on temparature alone
    /// </summary>
    class TemperaturePredictor:IPredictors
    {
        IPredictors m_nextpredictor;
        WeatherData m_data = new WeatherData();
        public WeatherData getData()
        {
            return m_data;
        }
        public TemperaturePredictor(IPredictors p,WeatherData data=null)
        {
            if (p != null)
                m_nextpredictor = p;
            if (data == null)
                throw new UnstableAdapter("Input not given");
            m_data = data;
        }
        public Prediction Predict()
        {
            Prediction p = new Prediction();
            if (m_data.m_temperature_int >45)
            {
                p.advise = "End of the world";
                p.smiley = Smiley.Angry;

            }
            else if (m_data.m_temperature_int < 45 && m_data.m_temperature_int >= 30)
            {
                p.advise = "Too sunny";
                p.smiley = Smiley.ReallySad;
            }
            else if (m_data.m_temperature_int < 30 && m_data.m_temperature_int >= 10)
            {
                p.advise = "Today’s weather is quite cool and sunny";
                p.smiley = Smiley.Happy;

            }
            else if ( m_data.m_temperature_int < 10)
            {
                p.advise = "Today’s weather is really freezing";
                p.smiley = Smiley.Angry;

            }
            return p;

        }
    }
    /// <summary>
    /// Exception to be thrown if we decorate  inproperly
    /// </summary>
    class UnstableAdapter:Exception
    {
        string m_error;
        public UnstableAdapter(string str)
        {
            m_error=str;
        }
    }
    //The final prediction to be made is stored here
    class Prediction
    {
        public CloudLevel cloud { get; set; }
        public int rain { get; set; }
        public System.Drawing.Image picture { get; set; }
        public string advise { get; set; }
        public Smiley smiley { get; set; }


    }
    enum CloudLevel
    {
        Sunny,
        PartlyCloudy,
        Cloudy,
        OverCast,
        MildRain,
        ThunderStorm
    }
    enum Smiley
    {
        Happy,
        OK,
        Sad,
        ReallySad,
        Angry
        
    }
}
