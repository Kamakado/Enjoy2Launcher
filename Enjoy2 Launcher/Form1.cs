using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Enjoy2_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Читаем файл test.txt
            System.IO.FileStream stream1 = System.IO.File.OpenRead("crashhandler.dll");
            label1.Text = Convert.ToString(CRCTools.CalculateCRC(stream1));
            //Console.WriteLine(string.Format("{0:X}", CalculateCRC(stream1)));
            //Console.ReadLine();
        }
    }
}
