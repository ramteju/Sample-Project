using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
//using System.Windows.Forms;
using System.Timers;
namespace R_CADViewXService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer1=null;
        public bool InProgress = false;


        public Service1()
        {
            
            InitializeComponent();
        }

       

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval =Convert.ToInt32(ConfigurationManager.AppSettings["Timer"].ToString()); //15 mins refresh
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
        }

   

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {

            if (!InProgress)
            {
                InProgress = true;
                CadViewxProcess.RunCadViewx().ContinueWith(t=> {
                    InProgress = false;
                });
            }
           
        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    sb.Append("timer1 method triggerd strated");
        //    file.WriteLine(sb.ToString());
        //    file.Flush();

        //    CadViewxProcess.RunCadViewx();
        //}

        protected override void OnStop()
        {
            
        }
    }
}
