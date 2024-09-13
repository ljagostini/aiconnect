using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public class CheckInternet
    {
        public static bool TestInternet(string myHost, int timeout)
        {
            bool retorno = false;
            try
            {
                Ping myPing = new Ping();
                String host = myHost;
                //byte[] buffer = new byte[32];
                //int timeout = 10000;
                //PingOptions pingOptions = new PingOptions();
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                PingOptions pingOptions = new PingOptions(64, true);
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                retorno = (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {

            }
            return retorno;
        }
    }
}
