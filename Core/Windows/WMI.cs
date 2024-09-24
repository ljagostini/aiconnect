using System.Management;

namespace Percolore.Core.Windows
{
	public class WMI
    {
        /// <summary>
        /// Id do processador
        /// </summary>
        /// <returns></returns>
        /// 
        public static String GetProcessorId()
        {
            string Id = string.Empty;

            using (ManagementClass mc = new ManagementClass("win32_processor"))
            {
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    Id = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }

            return Id;
        }

        /// <summary>
        /// Serial Number do HD
        /// </summary>
        /// <returns></returns>
        public static String GetHDDSerialNumber()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = "";
            foreach (ManagementObject strt in mcol)
            {
                result += Convert.ToString(strt["VolumeSerialNumber"]);
            }
            return result;
        }

        /// <summary>
        /// System MAC Address.
        /// </summary>
        /// <returns></returns>
        public static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

        /// <summary>
        /// Fabricante da placa mãe.
        /// </summary>
        /// <returns></returns>
        public static string GetBoardMaker()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject wmi in searcher.Get())
            {
                return wmi.GetPropertyValue("Manufacturer").ToString();
            }

            return "Board Maker: Unknown";
        }

        /// <summary>
        /// Id de produto da placa mãe
        /// </summary>
        /// <returns></returns>
        public static string GetBoardProductId()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject wmi in searcher.Get())
            {
                return wmi.GetPropertyValue("Product").ToString();
            }

            return "Product: Unknown";
        }

        /// <summary>
        /// Fabricante da BIOS.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSManufacturer()
        {
            using (ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS"))
            {
                foreach (ManagementObject wmi in searcher.Get())
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }
            }

            return "BIOS Manufacturer: Unknown";
        }

        /// <summary>
        /// Serial Number da BIOS
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSSerialNumber()
        {
            string serialNumber = string.Empty;

            ManagementObjectSearcher MOS =
                new ManagementObjectSearcher(" Select * From Win32_BIOS ");

            foreach (ManagementObject getserial in MOS.Get())
            {
                serialNumber = getserial["SerialNumber"].ToString();
            }

            return serialNumber;

            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
            //foreach (ManagementObject wmi in searcher.Get())
            //{
            //    return wmi.GetPropertyValue("SerialNumber").ToString();
            //}

            //return "BIOS Serial Number: Unknown";

        }

        /// <summary>
        /// Retrieving BIOS Caption.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSCaption()
        {

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                return wmi.GetPropertyValue("Caption").ToString();
            }

            return "BIOS Caption: Unknown";
        }

        /// <summary>
        /// Nome da conta de usuário
        /// </summary>
        /// <returns></returns>
        public static string GetAccountName()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");

            foreach (ManagementObject wmi in searcher.Get())
            {
                return wmi.GetPropertyValue("Name").ToString();
            }

            return "User Account Name: Unknown";

        }

        /// <summary>
        /// Memória física.
        /// </summary>
        /// <returns></returns>
        public static string GetPhysicalMemory()
        {
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long MemSize = 0;
            long mCap = 0;

            // In case more than one Memory sticks are installed
            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }

            MemSize = (MemSize / 1024) / 1024;
            return MemSize.ToString() + "MB";
        }

        /// <summary>
        /// Número de slots de RAm da placa mãe.
        /// </summary>
        public static string GetNoRamSlots()
        {

            int MemSlots = 0;
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

            }

            return MemSlots.ToString();
        }

        /// <summary>
        /// Fabricante da CPU
        /// </summary>
        public static string GetCPUManufacturer()
        {
            string cpuMan = String.Empty;

            //create an instance of the Managemnet class with the
            //Win32_Processor class

            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through

            ManagementObjectCollection objCol = mgmt.GetInstances();

            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuMan == String.Empty)
                {
                    // only return manufacturer from first CPU
                    cpuMan = obj.Properties["Manufacturer"].Value.ToString();
                }
            }

            return cpuMan;
        }

        /// <summary>
        /// CPU Clock Speed
        /// </summary>
        public static int GetCPUCurrentClockSpeed()
        {
            int cpuClockSpeed = 0;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            ManagementObjectCollection objCol = mgmt.GetInstances();
            
            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuClockSpeed == 0)
                {
                    // only return cpuStatus from first CPU
                    cpuClockSpeed = Convert.ToInt32(obj.Properties["CurrentClockSpeed"].Value.ToString());
                }
            }
            
            //return the status
            return cpuClockSpeed;
        }

        /// <summary>
        /// Default IP gateway using WMI
        /// </summary>
        /// <returns>Adapters default IP gateway</returns>
        public static string GetDefaultIPGateway()
        {
            //create out management class object using the
            //Win32_NetworkAdapterConfiguration class to get the attributes
            //of the network adapter
            ManagementClass mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //create our ManagementObjectCollection to get the attributes with
            ManagementObjectCollection objCol = mgmt.GetInstances();
            string gateway = String.Empty;
            
            //loop through all the objects we find
            foreach (ManagementObject obj in objCol)
            {
                if (gateway == String.Empty)  // only return MAC Address from first card
                {
                    //grab the value from the first network adapter we find
                    //you can change the string to an array and get all
                    //network adapters found as well
                    //check to see if the adapter's IPEnabled
                    //equals true
                    if ((bool)obj["IPEnabled"] == true)
                    {
                        gateway = obj["DefaultIPGateway"].ToString();
                    }
                }
                //dispose of our object
                obj.Dispose();
            }

            //replace the ":" with an empty space, this could also
            //be removed if you wish
            gateway = gateway.Replace(":", "");
            //return the mac address
            return gateway;
        }

        /// <summary>
        /// CPU Speed.
        /// </summary>
        /// <returns></returns>
        public static double? GetCpuSpeedInGHz()
        {
            double? GHz = null;
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    GHz = 0.001 * (UInt32)mo.Properties["CurrentClockSpeed"].Value;
                    break;
                }
            }

            return GHz;
        }

        /// <summary>
        /// Linguagem corrente
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguage()
        {

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                return wmi.GetPropertyValue("CurrentLanguage").ToString();
            }

            return "BIOS Maker: Unknown";

        }

        /// <summary>
        /// Informações do SO
        /// </summary>
        /// <returns></returns>
        public static string GetOSInformation()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject wmi in searcher.Get())
            {
                return ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
            }

            return "BIOS Maker: Unknown";
        }

        /// <summary>
        /// Informações do processador.
        /// </summary>
        /// <returns></returns>
        public static string GetProcessorInformation()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            string info = string.Empty;
            foreach (ManagementObject mo in moc)
            {
                string name = (string)mo["Name"];
                name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                info = name + ", " + (string)mo["Caption"] + ", " + (string)mo["SocketDesignation"];
                //mo.Properties["Name"].Value.ToString();
                //break;
            }

            return info;
        }

        /// <summary>
        /// Nome definido para a maáquina
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            string retorno = string.Empty;

            using (ManagementClass mc = new ManagementClass("Win32_ComputerSystem"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    foreach (ManagementObject mo in moc)
                    {
                        retorno = (string)mo["Name"];
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Arquitetura do SO
        /// </summary>
        /// <returns></returns>
        public static OSArchitecture GetOSArchitecture()
        {
            string info = string.Empty;
            string query = "Select OSArchitecture from Win32_OperatingSystem";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject Win32 in searcher.Get())
                {
                    info = Win32["OSArchitecture"].ToString();
                }
            }

            switch (info)
            {
                case "64 bits":
                    {
                        return OSArchitecture.x64;
                    }
                case "32 bits":
                    {
                        return OSArchitecture.x86;
                    }
                default:
                    {
                        return OSArchitecture.Empty;
                    }
            }
        }

        /// <summary>
        /// Objeto Version com informações de compilação do SO
        /// </summary>
        /// <returns></returns>
        public static Version GetOSBuildVersion()
        {
            string info = string.Empty;
            string query = "Select Version from Win32_OperatingSystem";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject Win32 in searcher.Get())
                {
                    info = Win32["Version"].ToString();
                }
            }

            return new Version(info);
        }

        /// <summary>
        /// Descrição da da versão SO
        /// </summary>
        /// <returns></returns>
        public static string GetOSName()
        {
            string retorno = string.Empty;
            string query = "Select Caption from Win32_OperatingSystem";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject Win32 in searcher.Get())
                {
                    retorno = Win32["Caption"].ToString();
                }
            }

            return retorno;
        }
    }

    public enum OSArchitecture { Empty, x64, x86 }
}