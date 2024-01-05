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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Linq.Expressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

using Microsoft.Office.Core;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Controls;
using System.Diagnostics.PerformanceData;


namespace girbox_auto
{

    public partial class Form1 : Form
    {

        public Form1()
        {



            InitializeComponent();

            vScrollBar1.Value = 30;
            label8.Text = "0";

            vScrollBar2.Value = 0;
            label9.Text = "0";

            textBox1.Text = "300";
            textBox2.Text = "100";

            angularGauge1.TickStep = .5;
            angularGauge1.ToValue = 15;

            angularGauge2.TickStep = 1;
            angularGauge2.ToValue = 20;


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
            try
            {
                ComPort.Close();
                btnConnect.Text = "Connect";
            }
            catch
            {

            }
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
                ComPort.Write(txtSend.Text + "\n");

                // Show in the terminal window 
                rtxtDataArea.ForeColor = Color.Green;    //write sent text data in green colour              
                //txtSend.Clear();                       //clear screen after sending data        
            }
            catch
            {
                rtxtDataArea.Text = "error \r\n open the serial";
            }
        }
        float sensor1;
        float sensor2;
        float[] input1 = new float[1000000];
        float[] input2 = new float[1000000];
        int count_input1;
        int count_input2;
        int start_program;
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
                                angularGauge2.Value = sensor1;
                                if (sensor1 >= 0 && start_program == 1)
                                {
                                    input1[count_input1] = float.Parse(sensordata.ElementAt(1));
                                    ++count_input1;
                                }


                                //*******************************
                            }
                            catch
                            {
                                //label2.Text = "ERROR";
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
                                angularGauge1.Value = sensor2;
                                if (sensor2 >= 0 && start_program == 1)
                                {
                                    input2[count_input2] = float.Parse(sensordata.ElementAt(1)); ;
                                    ++count_input2;
                                }
                            }
                            catch
                            {
                                // label2.Text = "ERROR";
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
        int count_data = 2;
        //*****************timer*****************

        private void timer1_Tick(object sender, EventArgs e)
        {
            ++time_start_program;
            label19.Text = time_start_program.ToString();

            String ss1 = time_start_program.ToString();
            string ss2 = count_input_cell.ToString();

            try
            {
                if (time1[time_start_program] != "") //Int16.Parse(time1[time_start_program])
                {

                    txtSend.Text = "td:" + in1[time_start_program] + "\n";
                    sendData();
                    label25.Text = in1[time_start_program];
                }

                if (ss1 == ss2)
                {

                    txtSend.Text = "td:0"  + "\n";
                    sendData();
                    timer1.Enabled = false;
                    txtSend.Text = "stop_program";
                    start_program = 0;
                    sendData();
                    time_start_program = 0;
                    label19.Text = "0";
                    count_data = 0;
                    MessageBox.Show("***End file***");
                }
            }
            catch
            {
                label25.Text = "no data";
            }
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
            txtSend.Text = txtSend.Text + "\n";
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

            // txtSend.Text = "md:" + vScrollBar1.Value.ToString();
            // sendData();
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
            txtSend.Text = "md:" + vScrollBar1.Value.ToString() + "\r\n";
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
            txtSend.Text = "td:" + vScrollBar2.Value.ToString() + "\r\n";
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

        private void angularGauge1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            label25.Text = "";
            label19.Text = "0";
            count_data = 0;
            txtSend.Text = "start_program";
            sendData();
           

            time_start_program = 0;
            label19.Text = "0";
            for (int i = 0; i < count_input1; i++)
            {
                input1[i] = 0;
            }

            for (int i = 0; i < count_input2; i++)
            {
                input2[i] = 0;
            }
            count_input2 = 0;
            count_input1 = 0;

            start_program = 1;
            Thread.Sleep(500); txtSend.Text = "md:" + main_presure + "\n";
            sendData();
            Thread.Sleep(500);
            timer1.Enabled = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            txtSend.Text = "stop_program";
            start_program = 0;
            count_data = 0;
            sendData();
            time_start_program = 0;
            label19.Text = "0";
            timer1.Enabled = false;
            count = count_input2;
        }
        int time_start_program;
        private void timer2_Tick(object sender, EventArgs e)
        {
            ++time_start_program;
            label19.Text = time_start_program.ToString();



            if (start_program == 1)
            {

            }

        }

        //************clear current data**********
        private void button19_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < count_input1; i++)
            {
                input1[i] = 0;
            }

            for (int i = 0; i < count_input2; i++)
            {
                input2[i] = 0;
            }
            count_input2 = 0;
            count_input1 = 0;

        }

        //*****show current result*****
        private void button17_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= count_input1; i++)
            {
                DataPoint dp1 = new DataPoint();

                dp1.SetValueXY(i, input1[i]);
                chart2.Series[0].Color = Color.Red;
                chart2.Series[0].ChartType = SeriesChartType.StepLine;
                chart2.Series[0].Points.Add(dp1);
            }

            for (int i = 0; i <= count_input2; i++)
            {
                DataPoint dp2 = new DataPoint();

                dp2.SetValueXY(i, input2[i]);
                chart2.Series[1].Color = Color.Blue;
                chart2.Series[1].ChartType = SeriesChartType.Spline;
                chart2.Series[1].Points.Add(dp2);


            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            
        }

        private void button22_Click(object sender, EventArgs e)
        {
            chart2.Series.Clear();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }


        Random random = new Random();
        int count;
        private void button20_Click(object sender, EventArgs e) // count_input1
        {

            if (textBox3.Text != "")
            {
                //***creat excel file***
                Excel.Application xlApp1 = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp1 == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlWorkBook = xlApp1.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Cells[1, 1] = "";
                xlWorkSheet.Cells[1, 2] = "";
                xlWorkSheet.Cells[2, 1] = "";
                xlWorkSheet.Cells[2, 2] = "";
                xlWorkSheet.Cells[3, 1] = "";
                xlWorkSheet.Cells[3, 2] = "";
                xlWorkBook.SaveAs("D:\\files\\" + textBox3.Text + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp1.Quit();
                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp1);
                //MessageBox.Show("Excel file created");


                //*****save to exel*****
                string filePath = "";
                FileInfo fi = new FileInfo(@"D:\\files\\" + textBox3.Text + ".xls");
                filePath = fi.FullName;
                label1.Text = filePath;

                Excel.Application xlApp = new Excel.Application();
                //open workbook
                Excel.Workbook workbook = xlApp.Workbooks.Open(filePath);
                //open sheet
                Excel._Worksheet sheet = workbook.Sheets[1];
                Excel.Range excelFile = sheet.UsedRange;

                //int row = int.Parse(txtRow.Text), column = int.Parse(txtColumn.Text);
                //string value = txtValue.Text;
                //excelFile.Cells[row, column].Value = value;
                excelFile.Cells[1, 1].Value = "Main valve";
                excelFile.Cells[1, 2].Value = "Test valve";

                for (int i = 2; i <= count; i++)
                {
                    excelFile.Cells[i, 1].Value = input1[i].ToString();
                }

                for (int i = 2; i <= count; i++)
                {
                    excelFile.Cells[i, 2].Value = input2[i].ToString();
                }


                //save changed data in second excel file
                workbook.Save();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(excelFile);
                //close and release
                workbook.Close();
                Marshal.ReleaseComObject(workbook);
                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
                MessageBox.Show("data saved");
            }
            else
            {
                MessageBox.Show("INSERT THE FILE NAME");
            }
        }







        //*************************************************excel  save test***************************************
        private void button23_Click(object sender, EventArgs e)
        {

            //***inpu maqadir***
            for (int i = 2; i <= 100; i++)
            {


                int randomNumber = random.Next(0, 100);
                int randomNumber2 = random.Next(0, 100);


                input1[i] = randomNumber;
                input2[i] = randomNumber2;
                ++count;
                textBox1.Text = input1[i].ToString() + "\r\n";
            }
            label22.Text = "new count insert";


            //***creat excel file***
            Excel.Application xlApp1 = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp1 == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp1.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "";
            xlWorkSheet.Cells[1, 2] = "";
            xlWorkSheet.Cells[2, 1] = "";
            xlWorkSheet.Cells[2, 2] = "";
            xlWorkSheet.Cells[3, 1] = "";
            xlWorkSheet.Cells[3, 2] = "";
            xlWorkBook.SaveAs("D:\\files\\" + textBox3.Text + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp1.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp1);
            MessageBox.Show("Excel file created");


            //*****save to exel*****
            string filePath = "";
            FileInfo fi = new FileInfo(@"D:\\files\\" + textBox3.Text + ".xls");
            filePath = fi.FullName;
            label1.Text = filePath;

            Excel.Application xlApp = new Excel.Application();
            //open workbook
            Excel.Workbook workbook = xlApp.Workbooks.Open(filePath);
            //open sheet
            Excel._Worksheet sheet = workbook.Sheets[1];
            Excel.Range excelFile = sheet.UsedRange;

            //int row = int.Parse(txtRow.Text), column = int.Parse(txtColumn.Text);
            //string value = txtValue.Text;
            //excelFile.Cells[row, column].Value = value;
            excelFile.Cells[1, 1].Value = "Main valve";
            excelFile.Cells[1, 2].Value = "Test valve";

            for (int i = 2; i <= count; i++)
            {
                excelFile.Cells[i, 1].Value = input1[i].ToString();
            }

            for (int i = 2; i <= count; i++)
            {
                excelFile.Cells[i, 2].Value = input2[i].ToString();
            }


            //save changed data in second excel file
            workbook.Save();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(excelFile);
            //close and release
            workbook.Close();
            Marshal.ReleaseComObject(workbook);
            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            label3.Text = "***saved***";

        }

        private void button24_Click(object sender, EventArgs e)
        {

        }

        //***********************show from open graph*******************
        string filePath = "";
        private void button21_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "File 1";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //save selected file name
                filePath = openFileDialog1.FileName;
                label23.Text = filePath;
            }
        }


        String[] input_1 = new string[20000];
        String[] input_2 = new string[20000];
        float[] input11 = new float[1000000];
        float[] input22 = new float[1000000];

        private void button25_Click(object sender, EventArgs e)
        {
            //open excel
            Excel.Application xlApp = new Excel.Application();
            //open workbook
            Excel.Workbook workbook = xlApp.Workbooks.Open(filePath);
            //open sheet
            Excel._Worksheet sheet = workbook.Sheets[1];
            Excel.Range excelFile = sheet.UsedRange;

            for (int i = 2; i < excelFile.Rows.Count; i++)
            {

                input_1[i] = excelFile.Cells[i, 1].Value2.ToString();


                input11[i] = float.Parse(input_1[i]);

            }

            for (int i = 2; i < excelFile.Rows.Count; i++)
            {

                input_2[i] = excelFile.Cells[i, 2].Value2.ToString();
                input22[i] = float.Parse(input_2[i]);

            }

            int count_cll = excelFile.Rows.Count;


            GC.Collect();
            GC.WaitForPendingFinalizers();
            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(excelFile);
            //close and release
            workbook.Close();
            Marshal.ReleaseComObject(workbook);
            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);


            for (int i = 2; i <= count_cll; i++)
            {
                DataPoint dp1 = new DataPoint();

                dp1.SetValueXY(i, input11[i]);
                chart2.Series[3].Color = Color.Orange;
                chart2.Series[3].ChartType = SeriesChartType.StepLine;
                chart2.Series[3].Points.Add(dp1);

            }

            for (int i = 2; i <= count_cll; i++)
            {
                DataPoint dp1 = new DataPoint();

                dp1.SetValueXY(i, input22[i]);
                chart2.Series[4].Color = Color.Blue;
                chart2.Series[4].ChartType = SeriesChartType.StepLine;
                chart2.Series[4].Points.Add(dp1);

            }

            MessageBox.Show("done");
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }



        //********open file********
        String[] time1 = new String[1000];
        String[] in1 = new String[1000];
        int count_input_cell;
        String main_presure;
        String main_freq;
        String test_freq;
        private void button26_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open File";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //save selected file name
                filePath = openFileDialog1.FileName;
                label24.Text = filePath;
            }

            //open excel
            Excel.Application xlApp = new Excel.Application();
            //open workbook
            Excel.Workbook workbook = xlApp.Workbooks.Open(filePath);
            //open sheet
            Excel._Worksheet sheet = workbook.Sheets[1];
            Excel.Range excelFile = sheet.UsedRange;

            count_input_cell = excelFile.Rows.Count; //tedad satrha

            main_presure = excelFile.Cells[2, 2].Value2.ToString();
            main_freq = excelFile.Cells[2, 4].Value2.ToString();
            test_freq = excelFile.Cells[2, 5].Value2.ToString();

            //*******get time********
            for (int i = 2; i <= excelFile.Rows.Count; i++)
            {
                time1[i] = excelFile.Cells[i, 1].Value2.ToString();
            }

            //*******get input data********
            for (int i = 2; i <= excelFile.Rows.Count; i++)
            {
                in1[i] = excelFile.Cells[i, 3].Value2.ToString();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(excelFile);
            //close and release
            workbook.Close();
            Marshal.ReleaseComObject(workbook);
            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);


            txtSend.Text = "mf:" + main_freq + "\n";
            sendData();
            Thread.Sleep(500);
            txtSend.Text = "tf:" + test_freq + "\n";
            sendData();
            Thread.Sleep(500);
            txtSend.Text = "md:" + "0" + "\n";
            sendData();
            Thread.Sleep(500);
            txtSend.Text = "td:0" + "\n";
            sendData();
            Thread.Sleep(500);
            textBox1.Text = main_freq;
            textBox2.Text = test_freq;
            vScrollBar1.Value = int.Parse(main_presure);
            label8.Text = main_presure;
            MessageBox.Show("file ready");

            // label25.Text = time1[2];




        }



        private void button10_Click(object sender, EventArgs e)
        {
        }



        //*******************END************************
    }
}
