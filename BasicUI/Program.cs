using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeatherController;

namespace BasicUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //IGetWetherInfo info = new WeatherRequestWWO();
            //info.GetTemparature();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
