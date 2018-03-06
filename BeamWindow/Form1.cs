using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamWindow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
              InitTimer();
          
        }

        private Timer timer1;
        bool blnRunningStatus = false;

        public void InitTimer()
        {
            try
            {
                timer1 = new Timer();
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = 4000; // in miliseconds
                timer1.Start();
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }
        public void timer1_Tick(object sender, EventArgs e)
        {

            BeamProcess.RunModel();
        }

       
    }
}
