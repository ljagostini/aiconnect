using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Modbus
{
    public class Constantes
    {
        public static bool bSerialLog = false;
        public static int qtdLogSerial = 100;
        public static List<Util.LogSerial> listLogSerial = new List<Util.LogSerial>();

        public static int qtdLog_TXT = 50;
        public static List<string> listLog_TXT = new List<string>();
    }
}
