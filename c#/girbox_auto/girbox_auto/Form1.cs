using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using System.Linq.Expressions;

namespace girbox_auto
{
   
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            vScrollBar1.Value = 0;
            label8.Text = "0";

            vScrollBar2.Value = 0;
            label9.Text = "0";

            textBox1.Text = "100";
            textBox2.Text = "100";

            angularGauge1.TickStep = .5;
            angularGauge1.ToValue = 30;
           
            angularGauge2.TickStep = 1;
            angularGauge2.ToValue = 50;


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updatePorts();          
            CheckForIllegalCrossThreadCalls = false;
        }

        private void updatePorts()
        {
            // Retrieve the list of all COM ports on your Computer
            cmbPortName.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPortName.Items.Add(port);
            }
        }

        private SerialPort ComPort = new SerialPort();
        private void button1_Click(object sender, EventArgs e)
        {
            if (ComPort.IsOpen)
            {
                disconnect();
            }
            else
            {
                connect();

            }
        }
             private void disconnect()
        {
            ComPort.Close();
            btnConnect.Text = "Connect";
            //btnSend.Enabled = false;
            //groupBox1.Enabled = true;

        
    }

        private void connect()
        {
            bool error = false;
            ComPort.PortName = cmbPortName.Text;
            ComPort.BaudRate = int.Parse("115200");      //convert Text to Integer
            ComPort.Parity = (Parity)Enum.Parse(typeof(Parity), "None"); //convert Text to Parity
            ComPort.DataBits = int.Parse("8");        //convert Text to Integer
            ComPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1");  //convert Text to stop bits

            try  //always try to use this try and catch method to open your port. 
                 //if there is an error your program will not display a message instead of freezing.
            {
                //Open Port
                ComPort.Open();
                ComPort.DataReceived += SerialPortDataReceived;  //Check for received data. When there is data in the receive buffer,
                                                                 //it will raise this event, we need to subscribe to it to know when there is data
            }
            catch (UnauthorizedAccessException) { error = true; }
            catch (System.IO.IOException) { error = true; }
            catch (ArgumentException) { error = true; }

            if (error) MessageBox.Show(this, "Could not open the COM port. Most likely it is already in use, has been removed, or is unavailable.", "COM Port unavailable", MessageBoxButtons.OK, MessageBoxIcon.Stop);


            //if the port is open, Change the Connect button to disconnect, enable the send button.
            //and disable the groupBox to prevent changing configuration of an open port.
            if (ComPort.IsOpen)
            {
                btnConnect.Text = "Disconnect";
               // btnSend.Enabled = true;
            }
        }

        private void sendData()
        {
            try
            {
                ComPort.Write(txtSend.Text);

                // Show in the terminal window 
                rtxtDataArea.ForeColor = Color.Green;    //write sent text data in green colour              
                txtSend.Clear();                       //clear screen after sending data        
            }
            catch
            {
                rtxtDataArea.Text = "error \r\n open the serial";
            }
        }
        float sensor1;
        float sensor2;


        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var serialPort = (SerialPort)sender;
                var data = serialPort.ReadLine();//.ReadExisting();        
                SetText(data);
            }
            catch
            {

            }
        }




        delegate void SetTextCallback(string text);
        Chart chart1;
        private void SetText(string text)
        {
            // text = "sensor1:\r\n";// Bs2,24,19,97Bs3,24,19,97Bs4,24,19,97B\r\n";
            if (text != "" && text != null)
            {
                string s = text;
                string[] twobackslash = s.Split('B');
                label16.Text = s;
                //if (s.Split('C').Count() == 1) // daryaft kamel etelaat
                //{
                    for (int i = 0; i < twobackslash.Count(); i++)
                    {
                        try
                        {
                            label14.Text = twobackslash[0];
                            label15.Text = twobackslash[1];
                        }
                        catch
                        {
                            label18.Text = "error";
                        }
                        string[] sensordata = twobackslash.ElementAt(i).Split(':');
                        for (int j = 0; j < sensordata.Count(); j++)
                        {
                            if (sensordata.ElementAt(0) == "sensor1" && sensordata.Count() > 1)
                            {

                                try
                                {
                                    sensor1 = float.Parse(sensordata.ElementAt(1));  
                                    label2.Text = sensor1.ToString();
                                // chart***************************************
                                DataPoint dp1 = new DataPoint();
                                dp1.SetValueXY(i, sensor1);
                                chart2.Series[0].Color = Color.Red;
                                chart2.Series[0].ChartType = SeriesChartType.Spline;
                                chart2.Series[0].Points.Add(dp1);
                                //********************************************************
                                //********************************************************
                                int blockSize = 100;
                                var series = chart2.Series[0];//.Add("My Series");
                                var chartArea = chart2.ChartAreas[series.ChartArea];
                                chartArea.CursorX.AutoScroll = true;

                                chartArea.AxisX.ScaleView.Zoomable = true;
                                chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
                                int size = blockSize;
                                ////chartArea.AxisX.ScaleView.Zoom(position, size);                               
                                chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                                chartArea.AxisX.ScaleView.SmallScrollSize = blockSize;

                            }
                                catch
                                {
                                    label2.Text = "ERROR";
                                }

                            }

                            if (sensordata.ElementAt(0) == "sensor2" && sensordata.Count() > 1)
                            {

                                try
                                {
                                    sensor2 = float.Parse(sensordata.ElementAt(1));
                                    // if (sensordata.ElementAt(2) != "") sensor2 = float.Parse(sensordata.ElementAt(2));

                                    label13.Text = sensor2.ToString();
                                    //label13.Text = sensor1.ToString();

                                }
                                catch
                                {
                                    label2.Text = "ERROR";
                                }

                            }
                        }
                    //}
                }



                if (this.rtxtDataArea.InvokeRequired)
                {
                    try
                    {
                        rtxtDataArea.ForeColor = Color.Green;    //write text data in Green colour

                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text });
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        this.rtxtDataArea.AppendText(text);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ComPort.IsOpen) ComPort.Close();  //close the port if open when exiting the application.
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // txtSend.Text = "sensor1=on";
                txtSend.Text = "p1";
                sendData();
            }
            else
            {
                //txtSend.Text = "sensor1=off";
                txtSend.Text = "p2";
                sendData();
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtSend.Text = "p1";
                sendData();
            }
            else
            {
                txtSend.Text = "p2";
                sendData();
            }
        }


        double i;
        int first;
        double position = 0;
       
        private void timer1_Tick(object sender, EventArgs e)
        {
          
            i += .5;
            //aGauge1.Value = 6;
            Random random = new Random();
            int randomNumber = random.Next(0, 50);
            int randomNumber2 = random.Next(20, 100);
            // chart***************************************
            DataPoint dp1 = new DataPoint();
            DataPoint dp2 = new DataPoint();
            dp1.SetValueXY(i, randomNumber);
            dp2.SetValueXY(i, randomNumber2);
            chart2.Series[0].Color = Color.Red;
            chart2.Series[1].Color = Color.Blue;
            chart2.Series[0].ChartType = SeriesChartType.Spline;
            chart2.Series[1].ChartType = SeriesChartType.Spline;

            chart2.Series[0].Points.Add(dp1);
            chart2.Series[1].Points.Add(dp2);

            //*********************************************************
            //********************************************************

          
            if (first == 0)
            {
                first = 1;
                

            }

            //********************************************************
            //********************************************************
            //********************************************************
            int blockSize = 100;
            var series = chart2.Series[0];//.Add("My Series");
            var chartArea = chart2.ChartAreas[series.ChartArea];           
            chartArea.CursorX.AutoScroll = true;
            // let's zoom to [0,blockSize] (e.g. [0,100])
            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            
            //if(i > 50) 
            //position = 100;
            int size = blockSize;
            chartArea.AxisX.ScaleView.Zoom(position, size);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;

            // set scrollbar small change to blockSize (e.g. 100)
            chartArea.AxisX.ScaleView.SmallScrollSize = blockSize;
        }
        private void FillChart()
        {
            int blockSize = 100;

            // generates random data (i.e. 30 * blockSize random numbers)
            Random rand = new Random();
            var valuesArray = Enumerable.Range(0, blockSize * 30).Select(x => rand.Next(1, 100)).ToArray();

            // clear the chart
            chart2.Series.Clear();

            // fill the chart
            var series = chart2.Series.Add("My Series");
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Int32;
            for (int i = 0; i < valuesArray.Length; i++)
                series.Points.AddXY(i, valuesArray[i]);
            var chartArea = chart2.ChartAreas[series.ChartArea];

            // set view range to [0,max]
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = valuesArray.Length;

            // enable autoscroll
            chartArea.CursorX.AutoScroll = true;

            // let's zoom to [0,blockSize] (e.g. [0,100])
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            int position = 0;
            int size = blockSize;
            chartArea.AxisX.ScaleView.Zoom(position, size);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change to blockSize (e.g. 100)
            chartArea.AxisX.ScaleView.SmallScrollSize = blockSize;
        }
        private void SplineChartExample()
        {
            
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            txtSend.Text = txtSend.Text;
            sendData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtSend.Text = "p2";
            sendData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtSend.Text = "p1";
            sendData();
        }

        private void rtxtDataArea_TextChanged(object sender, EventArgs e)
        {

        }

        private void aGauge1_ValueInRangeChanged(object sender, AGaugeApp.AGauge.ValueInRangeChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            label8.Text = vScrollBar1.Value.ToString();
           
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            txtSend.Text = "mf:" + textBox1.Text;
            sendData();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            vScrollBar1.Value = 0;
            label8.Text = "0";
            txtSend.Text = "md:" + "0";
            sendData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtSend.Text = "md:" + vScrollBar1.Value.ToString();
            sendData();
        }

        private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            label9.Text = vScrollBar2.Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            vScrollBar2.Value = 0;
            label9.Text = "0";
            txtSend.Text = "td:" + "0";
            sendData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            txtSend.Text = "tf:" + textBox2.Text;
            sendData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            txtSend.Text = "td:" + vScrollBar2.Value.ToString();
            sendData();
        }

        private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            //FillChart();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FillChart();
        }

        Random random2 = new Random();
        int randomNumber3;
        private void button11_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 200; i++)
            {
                
                 randomNumber3 = random2.Next(0, 200);
                DataPoint dp3 = new DataPoint();
                dp3.SetValueXY(i, randomNumber3);

                chart2.Series[2].Color = Color.Green;

                chart2.Series[2].ChartType = SeriesChartType.Spline;

                chart2.Series[2].Points.Add(dp3);

            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            txtSend.Text = "test on";
            sendData();

        }

        private void tempbox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            txtSend.Text = "test off";
            sendData();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            updatePorts();
            CheckForIllegalCrossThreadCalls = false;
        }
    }
}
