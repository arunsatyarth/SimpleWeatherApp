using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeatherController;

namespace BasicUI
{
    /// <summary>
    /// This class takes in some weather inputs and some UI controls
    /// It then predicts the weather and sets proper values in the UI controls
    /// </summary>
    class WeatherForecaster
    {

        PictureBox m_picturebox;
        PictureBox m_smiley;
        Label m_label_advise;
        Label m_label_desc;
        WeatherData m_data;


        public WeatherForecaster(PictureBox picturebox,PictureBox smiley, Label label,WeatherData data,Label desc)
        {

             m_picturebox=picturebox;
             m_label_advise = label;
             m_data=data;
             m_label_desc = desc;
             m_smiley = smiley;

        }
        public void SetForecast()
        {
            try
            {
                //Building an Decorator Pattern where upper layer predictor takes input from lower and predicts the weather
                IPredictors predictor = new RainPredictor(new CloudPredictor(new TemperaturePredictor(null,m_data)));
                Prediction prediction=predictor.Predict();
                //Now look at the final prediction and set proper values in the UI controls
                if (m_picturebox != null)
                {
                    if (prediction.cloud == CloudLevel.Sunny)
                        m_picturebox.Image = (System.Drawing.Image)BasicUI.Properties.Resources.sunny;
                    else if (prediction.cloud == CloudLevel.PartlyCloudy)
                        m_picturebox.Image = (System.Drawing.Image)BasicUI.Properties.Resources.partly_cloudy;
                    if (prediction.cloud == CloudLevel.Cloudy)
                        m_picturebox.Image = (System.Drawing.Image)BasicUI.Properties.Resources.cloudy;
                    else if (prediction.cloud == CloudLevel.MildRain)
                        m_picturebox.Image = (System.Drawing.Image)BasicUI.Properties.Resources.rain_s_cloudy;
                    else if (prediction.cloud == CloudLevel.OverCast)
                        m_picturebox.Image = (System.Drawing.Image)BasicUI.Properties.Resources.rain_s_cloudy;
                    else if (prediction.cloud == CloudLevel.ThunderStorm)
                        m_picturebox.Image = (System.Drawing.Image)BasicUI.Properties.Resources.thunderstorms;

                    m_picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                if(m_smiley!=null){
                    if (prediction.smiley == Smiley.Happy)
                        m_smiley.Image = (System.Drawing.Image)BasicUI.Properties.Resources.happy;
                    else if (prediction.smiley == Smiley.Sad)
                        m_smiley.Image = (System.Drawing.Image)BasicUI.Properties.Resources.sad;
                    else if (prediction.smiley == Smiley.ReallySad)
                        m_smiley.Image = (System.Drawing.Image)BasicUI.Properties.Resources.reallysad;
                    else if (prediction.smiley == Smiley.Angry)
                        m_smiley.Image = (System.Drawing.Image)BasicUI.Properties.Resources.angry;
                    else if (prediction.smiley == Smiley.OK)
                        m_smiley.Image = (System.Drawing.Image)BasicUI.Properties.Resources.ok;
                    m_smiley.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                if(m_label_advise!=null)
                {
                    m_label_advise.Text = prediction.advise;
                    if(m_smiley!=null)
                    {
                        //m_smiley.Left = m_label_advise.PreferredWidth + 5 + m_label_advise.Location.X;
                    }
                }
                if (m_label_desc != null)
                {
                    m_label_desc.Text = prediction.cloud.ToString();
                }

            }
            catch (UnstableAdapter ex)
            {

            }
        }
    }
}
