using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Write_File
{
    public partial class Form1 : Form
    {
        string filePath = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "File 1";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //save selected file name
                filePath = openFileDialog1.FileName;
                lblFilePath.Text = filePath;
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            //open excel
            Excel.Application xlApp = new Excel.Application();
            //open workbook
            Excel.Workbook workbook = xlApp.Workbooks.Open(filePath);
            //open sheet
            Excel._Worksheet sheet = workbook.Sheets[1];
            Excel.Range excelFile = sheet.UsedRange;
            int row = int.Parse(txtRow.Text), column = int.Parse(txtColumn.Text);
            string value = txtValue.Text;
            excelFile.Cells[row, column].Value = value;

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
        }
    }
}
