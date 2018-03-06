using Client.XML;
using MDL.Draw.Renderer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Mol2Image
{
    public partial class Form1 : Form
    {
        Mode mode;
        private static Renderer renderer = new Renderer();

        public Form1()
        {
            InitializeComponent();
            statusLabel.Text = String.Empty;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (File.Exists(inPath.Text) && Directory.Exists(outPath.Text))
            {
                Dictionary<String, String> moles = new Dictionary<String, String>();
                if (mode == Mode.s8500)
                {
                    System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(Series8500));
                    using (FileStream stream = new FileStream(inPath.Text, FileMode.Open, FileAccess.Read))
                    {
                        Series8500 series8500 = (Series8500)serializer.Deserialize(stream);
                        foreach (var mole8500 in series8500.Table1)
                            if (!String.IsNullOrEmpty(mole8500.MOL_FILE) && !moles.ContainsKey(mole8500.REG_NO.ToString()))
                                moles[mole8500.REG_NO.ToString()] = mole8500.MOL_FILE;
                    }
                }
                else if (mode == Mode.s9000)
                {
                    System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(Series9000));
                    using (FileStream stream = new FileStream(inPath.Text, FileMode.Open, FileAccess.Read))
                    {
                        Series9000 series9000 = (Series9000)serializer.Deserialize(stream);
                        foreach (var mole9000 in series9000.Table1)
                            if (!String.IsNullOrEmpty(mole9000.MOL_FILE) && !moles.ContainsKey(mole9000.REG_NO))
                                moles[mole9000.REG_NO] = mole9000.MOL_FILE;
                    }
                }

                int count = moles.Count;
                int index = 1;
                foreach (var mole in moles)
                {
                    backgroundWorker.ReportProgress(0, index + " / " + count);
                    renderer.MolfileString = mole.Value;
                    renderer.Image.Save(Path.Combine(outPath.Text, mole.Key + ".gif"), ImageFormat.Gif);
                    index++;
                }
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusLabel.Text = e.UserState.ToString();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusLabel.Text = "Done";
        }

        private void s8500Btn_Click(object sender, EventArgs e)
        {
            mode = Mode.s8500;
            if (!backgroundWorker.IsBusy)
                backgroundWorker.RunWorkerAsync();
        }

        private void s9000Btn_Click(object sender, EventArgs e)
        {
            mode = Mode.s9000;
            if (!backgroundWorker.IsBusy)
                backgroundWorker.RunWorkerAsync();
        }
    }

    enum Mode
    {
        s8500,
        s9000
    }
}
