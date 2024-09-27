using System.Diagnostics;

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
            return (GetProcess() != null);
        }
    }
}