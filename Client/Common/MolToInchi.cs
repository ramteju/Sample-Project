using Client.Logging;
using Client.Views;
using Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Common
{
    public class MolToInchi
    {
        public static string Mol2Inchi(string molString)
        {

            try
            {
                ;
                var tempMolFile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".mol";
                var tempTxtFile = tempMolFile + ".txt";
                Debug.WriteLine(tempMolFile);
                File.WriteAllText(tempMolFile, molString);
                Process inchiProcess = new Process();
                inchiProcess.StartInfo.FileName = "inchi/inchi.exe";
                inchiProcess.StartInfo.Arguments = $"{tempMolFile} /option /AuxNone /NoLabels /Key";
                inchiProcess.StartInfo.UseShellExecute = false;
                inchiProcess.StartInfo.CreateNoWindow = true;
                inchiProcess.StartInfo.RedirectStandardOutput = true;
                inchiProcess.Start();
                inchiProcess.WaitForExit();
                inchiProcess.Close();
                var lines = File.ReadLines(tempTxtFile).ToArray();
                ;
                if (lines.Count() > 1 && lines[1].StartsWith("InChIKey", StringComparison.InvariantCultureIgnoreCase))
                    return lines[1].Split('=')[1];
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while generating InChi from Mol : ", ex.ToString());
            }
            return null;
        }
    }
}
