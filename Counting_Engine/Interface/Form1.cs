using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPY_Simulator;
using SPY_Data_Processor;
using System.Threading;

namespace Interface
{
    public partial class Form1 : Form
    {
        private SPY_Simulation Simulation;
        private string dir, file;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button2.Visible = true;
            this.button1.Visible = false;

            this.progressBar1.Maximum

            try
            {
                this.dir = this.richTextBox1.Text;
                this.file = DirectoryValidator.DirectoryValidator.getfilename(this.dir, ".progress");

                StartOldSim();
            }
            catch(Exception ex)
            {
                DisplayLog(ex.Message);
                DisplayLog("Failed to find progress file in directory " + this.dir + ", starting by looking for SPY json.");

                try
                {
                    this.file = DirectoryValidator.DirectoryValidator.getfilename(this.dir, ".json");


                    StartNewSim();
                }
                catch
                {
                    DisplayLog(ex.Message);
                    DisplayLog("Failed to find SPY json file in directory " + this.dir + ", check directory.");
                }
            }
        }

        private void StartNewSim()
        {
            this.Simulation = new SPY_Simulation(11, 8, 6, 50, 10, SPY_History.DeserializeFromJSONFile(this.file));

            this.Simulation.ProgressReported += this.ProcessProgressReport;

            this.Simulation.Run();
        }

        private void StartOldSim()
        {
            this.Simulation = SPY_Simulation.DeserializeFromJSONFile(this.file);

            this.Simulation.ProgressReported += this.ProcessProgressReport;

            this.Simulation.Run();
        }

        private void ProcessProgressReport(SimulationReport SR, EventArgs e)
        {
            this.CheckCurrentProgress(SR.CurrentProgress);

        }

        private bool CheckCurrentProgress(long Progress)
        {

        }

        private void DisplayLog(string log)
        {
            this.richTextBox2.Text = this.richTextBox2.Text + "\n\n" + log;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Visible = false;
            this.button1.Visible = true;

            this.Simulation.Stop();

            this.Simulation.ProgressReported -= this.ProcessProgressReport;

            int ii = 0;

            while (this.Simulation.done == false)
            {
                Thread.Sleep(1000);

                ii++;

                if (ii == 20)
                {
                    DisplayLog("Simulation failed to stop within 20 seconds.");
                    return;
                }
            }

            SPY_History.SerializeClassToFile<SPY_Simulation>(this.dir, this.Simulation);
        }
    }
}
