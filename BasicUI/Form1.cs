using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WeatherController;

namespace BasicUI
{
    public partial class Form1 : Form
    {
        DateTime today = DateTime.Now;
        WeatherInput input = new WeatherInput();
        WeatherData[] m_weatherdata=new WeatherData[7];
        System.Threading.Timer timer = null;
        private string m_currentloc;
        private string  getdate(DateTime date)
        {
            string str = date.DayOfWeek.ToString();
            string str2 = date.ToString("MMM");

            return str+" "+str2+" "+date.Day;
        }
        //takes a lot of input and sets values into te ui controls
        void set_info(Label date,Label temper, Label tempmax, Label tempmin, Label humidity, Label desc, PictureBox pic, Label lbladvise,PictureBox smiley, Node temp)
        {

            date.Text = getdate(temp.datetime);
            temper.Text = temp.data.m_temperature;
            tempmax.Text = temp.data.m_temperature_max;
            tempmin.Text = temp.data.m_temperature_min;
            humidity.Text = temp.data.m_humidity;
            desc.Text = temp.data.m_weatherdescription;
            WeatherForecaster forecaster2 = new WeatherForecaster(pic, smiley, lbladvise, temp.data, desc);
            forecaster2.SetForecast();

        }
        /// <summary>
        /// Callback function called after a command is executed to update the UI with newly fetched results
        /// Asynchronously updates UI
        /// </summary>
        /// <param name="something"></param>
        void updateui(WeatherData data)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UIUpdaterDelegate(updateui),
                                          new object[] { data });
                return;
            }
            else
            {
                EnableButtons();
                if (data.m_error)
                {
                    lblHeading.Text = "Could not connect. Have you entered an API key?";
                    return;
                }
                //check for validity of data
                if (!DataStore.Instance().m_today.IsValid)
                    return;
                //set data in currentdata 
                lblHeading.Text=DataStore.Instance().m_today.data.m_location;
                lblTodayDate.Text = getdate(DataStore.Instance().m_today.data.m_date);
                lblTodayTemp.Text = DataStore.Instance().m_today.data.m_temperature.ToString();
                //set forecast info for today
                WeatherForecaster forecaster = new WeatherForecaster(picToday,pic_similey, lblFinal, DataStore.Instance().m_today.data,null);
                forecaster.SetForecast();
                
                //Now walk the doubly link list and updats data for 7 days
                Node temp = DataStore.Instance().m_left;
                set_info(lblData1, lblTemp1, lblTempMax1, lblTempMin1, lblHumidity1, lblDesc1, pic1, lbladvise1, picsmiley1, temp);

                temp = temp.next;
                set_info(lblData2, lblTemp2, lblTempMax2, lblTempMin2, lblHumidity2, lblDesc2, pic2, lbladvise2, picsmiley2, temp);


                temp = temp.next;
                set_info(lblData3, lblTemp3, lblTempMax3, lblTempMin3, lblHumidity3, lblDesc3, pic3, lbladvise3, picsmiley3, temp);


                temp = temp.next;
                set_info(lblData4, lblTemp4, lblTempMax4, lblTempMin4, lblHumidity4, lblDesc4, pic4, lbladvise4, picsmiley4, temp);


                temp = temp.next;
                set_info(lblData5, lblTemp5, lblTempMax5, lblTempMin5, lblHumidity5, lblDesc5, pic5, lbladvise5, picsmiley5, temp);


                temp = temp.next;
                set_info(lblData6, lblTemp6, lblTempMax6, lblTempMin6, lblHumidity6, lblDesc6, pic6, lbladvise6, picsmiley6, temp);


                temp = temp.next;
                set_info(lblData7, lblTemp7, lblTempMax7, lblTempMin7, lblHumidity7, lblDesc7, pic7, lbladvise7, picsmiley7, temp);


                if (DataStore.Instance().m_right.next == null)
                    button3.Enabled = false;
                SetButtonNames();
                DrawChart();//draw the temperatur chart as well

                pic_similey.Left = lblFinal.PreferredWidth + 10 + lblFinal.Location.X;
            }
        }
        /// <summary>
        /// Pull data using timer every 20 mins
        /// </summary>
        /// <param name="obj"></param>
        void timercallback(object obj)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UIUpdaterDelegate(timercallback),
                                          new object[] { obj });
                return;
            }
            else
            {
                if (comboBox1.Text.Length == 0)
                    return;
                if (DataStore.Instance().m_end == DataStore.Instance().m_right)// do not issue refresh if we are viewing past data
                {
                    input.query = m_currentloc;
                    input.num_of_days = numericUpDown2.Value.ToString();
                    //create a command and push to queue
                    Command cmd = new Command();
                    cmd.SetInput(input);
                    cmd.SetCallback(updateui);
                    CommandQueue.Instance().add(cmd);
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
      

        }
        private void init_combo()//set some def value in combo
        {
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;

            DataTable t = new DataTable();
            t.Columns.Add("ID", typeof(int));
            t.Columns.Add("Display", typeof(string));
            string[] cities = { "Bangalore", "Moscow","Canberra","Oslo","New York" };
            for (int i = 0; i < cities.Length; i++)
            {
                t.Rows.Add(i, cities[i]);
            }

            comboBox1.DataSource = t;
            comboBox1.ValueMember = "ID";
            comboBox1.DisplayMember = "Display";
            m_currentloc= comboBox1.Text;
        }
        void DrawChart()
        {
            chart1.Series.Clear();
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Line
            };

            Node first = DataStore.Instance().m_left;

            int min = 1000;
            int max = -275;
            for (int i = 0; i < 7; i++)
            {
                if(first.data.m_temperature_int<min)
                    min=first.data.m_temperature_int;
                if (first.data.m_temperature_int > max)
                    max = first.data.m_temperature_int;

                first = first.next;
            }
            Axis yaxis = chart1.ChartAreas[0].AxisY;
            yaxis.IntervalType = DateTimeIntervalType.Number;
            yaxis.Interval = (int)((max-min)/3);
            yaxis.Minimum = min-1;
            yaxis.Maximum = max+1;
            this.chart1.Series.Add(series1);
            //chart1.BorderlineDashStyle = ChartDashStyle.NotSet;
            chart1.BackColor = Color.Transparent;

            first = DataStore.Instance().m_left;

            for (int i = 0; i < 7; i++)
            {
                series1.Points.AddXY(getdate(first.datetime), first.data.m_temperature_int);
                first = first.next;
            }
            chart1.Invalidate();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            pic_similey.Image = (Image)BasicUI.Properties.Resources.reallysad;
            pic_similey.SizeMode = PictureBoxSizeMode.StretchImage;

            init_combo();
            DisableButtons();
            input.query = comboBox1.Text;
            input.num_of_days = "7";
            Command cmd = new Command();
            cmd.SetInput(input);
            cmd.SetCallback(updateui);
            CommandQueue.Instance().add(cmd);
            this.BackgroundImage = (Image)BasicUI.Properties.Resources.sky3;
            foreach (Control item in this.Controls)
            {
                try
                {
                    item.BackColor = Color.Transparent;

                }
                catch (Exception)
                {
                }
            }
            Resize += Form1_Resize;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            toolStripMenuItem1.Click+=toolStripMenuItem1_Click;
            FormClosed += Form1_FormClosed;
            timer = new System.Threading.Timer(timercallback, null, 1000 * 60 * 60, 1000 * 60 * 30);
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CommandExecutor.Instance().Close();
            Environment.Exit(0);
        }

        void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            int num1 = (int)numericUpDown1.Value;
            int num2 = (int)numericUpDown2.Value;
            if (num1 + num2 > 7)
            {
                numericUpDown1.Value = num1 - 1;
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int num1 = (int)numericUpDown1.Value;
            int num2 = (int)numericUpDown2.Value; 
            if(num1+num2>7)
            {
                numericUpDown2.Value = num2 - 1;
            }
        }

        void DisableButtons()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }
        void EnableButtons()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length == 0)
                return;
            m_currentloc = comboBox1.Text;
            today = DateTime.Now;
            DisableButtons();



            input.query = m_currentloc;
            input.num_of_days = numericUpDown2.Value.ToString();

            WeatherInput past =null;
            int days = (int)numericUpDown1.Value;
            if(days>0)
            {
                past = new WeatherInput();
                past.query = m_currentloc;

                DateTime startdate = DateTime.Now;
                startdate = startdate.AddDays(-1 * days);
                string date_s = startdate.ToString("yyyy-MM-dd");
                past.date = date_s;

                DateTime enddate = DateTime.Now;

                enddate = enddate.AddDays(-1 );
                string enddate_s = enddate.ToString("yyyy-MM-dd");
                past.enddate = enddate_s;


            }

            //create a command and push to queue
            
            Command cmd = new Command();
            cmd.SetInput(input,past);
            cmd.SetCallback(updateui);
            CommandQueue.Instance().add(cmd);
            SetButtonNames();

        }
        void SetButtonNames()
        {
            DateTime yesterday = DataStore.Instance().m_left.datetime.AddDays(-1);
            DateTime tomorrow = DataStore.Instance().m_right.datetime.AddDays(1);
            button2.Text = getdate(yesterday);
            button3.Text = getdate(tomorrow);
        }
        private void button2_Click(object sender, EventArgs e)
        {

            m_currentloc = comboBox1.Text;
            today = today.AddDays(-1);
            DisableButtons();
            if (DataStore.Instance().m_start == DataStore.Instance().m_left)
            {
                //fetch new data
                input.query = m_currentloc;
                input.num_of_days = "1";

                WeatherInput past = null;
                past = new WeatherInput();
                past.query = m_currentloc;

                DateTime startdate = DataStore.Instance().m_start.datetime;
                startdate = startdate.AddDays(-5);
                string date_s = startdate.ToString("yyyy-MM-dd");
                past.date = date_s;

                DateTime enddate = DataStore.Instance().m_start.datetime;

                enddate = enddate.AddDays(-1);
                string enddate_s = enddate.ToString("yyyy-MM-dd");
                past.enddate = enddate_s;




                Command cmd = new Command();
                cmd.SetInput(input, past, RefreshType.AddLeft);
                cmd.SetCallback(updateui);
                CommandQueue.Instance().add(cmd);
            }
            else
            {
                DataStore.Instance().moveleft();
                //Uncomment following line if you want to change todays weather when you click left button
                //DataStore.Instance().m_today = DataStore.Instance().m_today.prev;
                updateui(null);
            }
            SetButtonNames();

 

        }

        private void button3_Click(object sender, EventArgs e)
        {

            today = today.AddDays(1);
            SetButtonNames();
            DisableButtons();
            //while going forward we dont fetch any data as data shud be present in our store
            DataStore.Instance().moveright();
            //Uncomment following line if you want to change todays weather when you click left button           
            //DataStore.Instance().m_today = DataStore.Instance().m_today.next;
            updateui(null);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void lblData1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
