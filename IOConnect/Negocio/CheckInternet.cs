using System.Net.NetworkInformation;
using System.Text;

namespace Percolore.IOConnect.Negocio
{
	public class CheckInternet
    {
        public static bool TestInternet(string myHost, int timeout)
        {
            bool retorno;
            try
            {
                Ping myPing = new Ping();
                String host = myHost;
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                PingOptions pingOptions = new PingOptions(64, true);
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                retorno = (reply.Status == IPStatus.Success);
            }
            catch
            {
                retorno = false;
            }

            return retorno;
        }
    }
}