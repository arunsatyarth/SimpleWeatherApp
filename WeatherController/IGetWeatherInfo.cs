using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherController
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY interface
    /// </summary>
    public interface IGetWetherInfo
    {
        WeatherData GetInformation();
        void SetNextChain(IGetWetherInfo next);
    }
    class WeatherNullPtr:IGetWetherInfo
    {
        IGetWetherInfo m_weatherinfo = null;

        public WeatherData GetInformation()
        {
            return null;
        }
        public void SetNextChain(IGetWetherInfo next)
        {
            m_weatherinfo = new WeatherNullPtr();
        }

    }
}
