using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string csvPath = @"C:\Users\Administrator\Documents\test.csv";
            string fileOutput = textBox1.Text;
            System.IO.File.WriteAllText(csvPath, fileOutput.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = System.IO.File.ReadAllText(@"C:\Users\Administrator\Documents\test.csv");
        }
    }
}
