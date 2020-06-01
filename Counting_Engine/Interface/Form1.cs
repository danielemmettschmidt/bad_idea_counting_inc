using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPY_Simulator;

namespace Interface
{
    public partial class Form1 : Form
    {
        private SPY_Simulation Simulation;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button2.Visible = true;
            this.button1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.button2.Visible = false;
            this.button1.Visible = true;
        }
    }
}
