using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Percolore.IOConnect.Core
{
    public class RunRemoteAccessHelper
    {
        private string PROCESS_NAME;
        public RunRemoteAccessHelper(string nameApp)
        {
            PROCESS_NAME = nameApp;
        }

        private Process GetProcess()
        {
            return
                Process.GetProcesses().FirstOrDefault(p => p.ProcessName == PROCESS_NAME);
        }

        public bool isRunRemoteAccess()
        {
            bool retorno = false;
            try
            {
                if (GetProcess() != null)
                {
                    retorno = true;
                }
            }
            catch
            { }
            return retorno;
        }
    }
}
