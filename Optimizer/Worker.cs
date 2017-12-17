using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    class Worker
    {
        public bool IsActive { get; private set; }
        private const string leanRoot = "C:/Mol/Lean";
        private const string exeRelativePath = "/Launcher/bin/Debug/QuantConnect.Lean.Launcher.exe";


        public void Run()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = leanRoot + exeRelativePath;
            LaunchLean(startInfo);
            //summary = _resultshandler.GetSummary();
        }

        private void LaunchLean(ProcessStartInfo startInfo)
        {
            try
            {
                // Start a Lean process
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
  
        }
    }
}
