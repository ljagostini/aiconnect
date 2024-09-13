using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Percolore.Core.Security.License
{
	public class MACLicenseKey : ALicenseKey
    {
        private string keycode;
        private string generatedLicense;

        public MACLicenseKey()
        {
            try
            {
                List<NetworkInterface> nics = new List<NetworkInterface>();
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface n in interfaces)
                {
                    if (n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        nics.Add(n);
                }

                if (nics.Count == 0)
                {
                    //Se não houver placa de rede gera código a partir de chave pré-definida
                    createLicensePair("tacosaregood");
                }
                else
                {
                    if (nics[0].GetPhysicalAddress().ToString().Equals(""))
                        createLicensePair("tacosaregood2");
                    else
                        createLicensePair(nics[0].GetPhysicalAddress().ToString());
                }
            }
            catch
            {
                createLicensePair("somethingwentverywrong");
            }
        }
        public MACLicenseKey(string key)
        {
            keycode = key;
            createLicenseKey(keycode);
        }
        public MACLicenseKey(string mac, bool ismac)
        {
            createLicensePair(mac);
        }

        private string getMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        private string getCode(string i)
        {
            return getMd5Hash(i).Substring(0, 12);
        }
        private void createLicensePair(string mac)
        {
            keycode = getCode(mac);
            createLicenseKey(keycode);
        }
        private void createLicenseKey(string key)
        {
            generatedLicense = getCode(keycode);
        }

        public override string KeyCode
        {
            get { return keycode; }
        }
        public override string License
        {
            get { return generatedLicense; }
        }
        public override bool CheckLicense(string license)
        {
            if (string.IsNullOrWhiteSpace(license))
                return false;

            return
                (license.Equals(generatedLicense));
        }
    }

}