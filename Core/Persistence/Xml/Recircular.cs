using System.Globalization;
using System.Xml.Linq;

namespace Percolore.Core.Persistence.Xml
{
    public class Recircular
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Recircular.xml");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public List<Recircular> listaRecircular = new List<Recircular>();

        public int Circuito { get; set; }
        public bool Habilitado { get; set; }
        public double VolumeDin { get; set; }
        public int Dias { get; set; }
        public double VolumeRecircular { get; set; }
        public double VolumeDosado { get; set; }
        public DateTime DtInicio { get; set; }
        public bool isValve { get; set; }
        public bool isAuto { get; set; }

        #region Métodos
        public static List<Recircular> List()
        {
            List<Recircular> listaRecircular = new List<Recircular>();

            if (File.Exists(PathFile))
            {
                XElement xRoot = XElement.Load(PathFile);
                foreach (var xRecircular in xRoot.Elements("Recircular"))
                {
                    Recircular recircular = new Recircular();
                    foreach (var prop in recircular.GetType().GetProperties())
                    {
                        if (prop.Name.Contains("Volume"))
                        {
                            prop.SetValue(recircular, double.TryParse(xRecircular.Element(prop.Name).Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0);
                        }
                        else
                        {
                            prop.SetValue(recircular, Convert.ChangeType(xRecircular.Element(prop.Name).Value, prop.PropertyType));
                        }
                    }
                    listaRecircular.Add(recircular);
                }
            }

            if (listaRecircular.Count < 32)
            {
                for (int i = listaRecircular.Count; i < 32; i++)
                {
                    Recircular recircular = new Recircular();
                    recircular.Circuito = i + 1;
                    recircular.Habilitado = false;
                    recircular.VolumeDin = 100;
                    recircular.Dias = 10;
                    recircular.VolumeRecircular = 100;
                    recircular.VolumeDosado = 0;
                    recircular.DtInicio = DateTime.Now;
                    recircular.isValve = false;
                    recircular.isAuto = false;
                    listaRecircular.Add(recircular);
                }
            }

            return listaRecircular;
        }

        public static void Add(Recircular obj)
        {
            XElement xRecircular = new XElement("Recircular");
            foreach (var prop in obj.GetType().GetProperties())
            {
                xRecircular.Add(new XElement(prop.Name, prop.GetValue(obj)));
            }

            XElement xRoot = XElement.Load(PathFile);
            xRoot.Add(xRecircular);
            xRoot.Save(PathFile);

        }
        #endregion
    }
}
