using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Percolore.Core.Persistence.Xml
{
	public class Colorante
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Colorantes.xml");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Circuito { get; set; }
        public string Nome { get; set; }
        public double MassaEspecifica { get; set; }
        public bool Habilitado { get; set; } = true;
        public double Volume { get; set; } = 0;
        public int Correspondencia { get; set; } = 0;

        public int Dispositivo { get; set; } = 1;

        public double NivelMinimo { get; set; } = 0;
        public double NivelMaximo { get; set; } = 0;

        public bool IsBase { get; set; } = false;

        public int Seguidor { get; set; } = -1;

        public int Step { get; set; } = 0;

        public double VolumePurga { get; set; } = 0;

        public string ColorRGB { get; set; }

        public bool IsBicoIndividual { get; set; } = false;
        public double VolumeBicoIndividual { get; set; } = 0;

        public Colorante() { }

        #region Métodos

        public static Colorante Load(int Circuito)
        {
            Colorante colorante = null;

            XElement xml = XElement.Load(PathFile);
            XElement xElement = xml.Elements()
                .Where(f => f.Attribute("Motor").Value == Circuito.ToString()).SingleOrDefault();

            if (xElement == null)
                return colorante;

            colorante = new Colorante();
            colorante.Circuito = int.Parse(xElement.Attribute("Motor").Value);
            colorante.Nome = xElement.Attribute("Nome").Value;
            colorante.MassaEspecifica = double.Parse(xElement.Attribute("MassaEspecifica").Value);

            XAttribute xAttribute = null;

            xAttribute = xElement.Attribute("Habilitado");
            if (xAttribute != null)
                colorante.Habilitado = XmlConvert.ToBoolean(xElement.Attribute("Habilitado").Value);

            xAttribute = xElement.Attribute("Volume");
            if (xAttribute != null)
                colorante.Volume = XmlConvert.ToDouble(xElement.Attribute("Volume").Value);

            xAttribute = xElement.Attribute("Correspondencia");
            if (xAttribute != null)
                colorante.Correspondencia = XmlConvert.ToInt32(xElement.Attribute("Correspondencia").Value);

            xAttribute = xElement.Attribute("Dispositivo");
            if (xAttribute != null)
                colorante.Dispositivo = XmlConvert.ToInt32(xElement.Attribute("Dispositivo").Value);

            xAttribute = xElement.Attribute("NivelMinimo");
            if (xAttribute != null)
                colorante.NivelMinimo = XmlConvert.ToInt32(xElement.Attribute("NivelMinimo").Value);

            xAttribute = xElement.Attribute("NivelMaximo");
            if (xAttribute != null)
                colorante.NivelMaximo = XmlConvert.ToInt32(xElement.Attribute("NivelMaximo").Value);

            xAttribute = xElement.Attribute("IsBase");
            if (xAttribute != null)
                colorante.IsBase = XmlConvert.ToBoolean(xElement.Attribute("IsBase").Value);

            xAttribute = xElement.Attribute("Seguidor");
            if (xAttribute != null)
                colorante.Seguidor = XmlConvert.ToInt32(xElement.Attribute("Seguidor").Value);

            xAttribute = xElement.Attribute("Step");
            if (xAttribute != null)
                colorante.Step = XmlConvert.ToInt32(xElement.Attribute("Step").Value);

            xAttribute = xElement.Attribute("VolumePurga");
            if (xAttribute != null)
                colorante.VolumePurga = XmlConvert.ToDouble(xElement.Attribute("VolumePurga").Value);

            xAttribute = xElement.Attribute("ColorRGB");
            if (xAttribute != null)
                colorante.ColorRGB = xElement.Attribute("ColorRGB").Value;

            xAttribute = xElement.Attribute("IsBicoIndividual");
            if (xAttribute != null)
                colorante.IsBicoIndividual = XmlConvert.ToBoolean(xElement.Attribute("IsBicoIndividual").Value);

            xAttribute = xElement.Attribute("VolumeBicoIndividual");
            if (xAttribute != null)
                colorante.VolumeBicoIndividual = XmlConvert.ToDouble(xElement.Attribute("VolumeBicoIndividual").Value);

            return colorante;
        }

        public static List<Colorante> List()
        {
            List<Colorante> list = new List<Colorante>();

            IEnumerable<XElement> xml =
                XElement.Load(PathFile).Elements();

            foreach (XElement elemento in xml)
            {
                Colorante c = new Colorante();
                c.Circuito = int.Parse(elemento.Attribute("Motor").Value);
                c.Nome = elemento.Attribute("Nome").Value;
                try
                {
                    c.MassaEspecifica = XmlConvert.ToDouble(elemento.Attribute("MassaEspecifica").Value);
                }
                catch
                {
                    c.MassaEspecifica = 0;
                }

                XAttribute xAttribute = null;

                xAttribute = elemento.Attribute("Habilitado");
                if (xAttribute != null)
                    c.Habilitado = XmlConvert.ToBoolean(elemento.Attribute("Habilitado").Value);

                xAttribute = elemento.Attribute("Volume");
                if (xAttribute != null)
                    c.Volume = XmlConvert.ToDouble(elemento.Attribute("Volume").Value);

                xAttribute = elemento.Attribute("Correspondencia");
                if (xAttribute != null)
                    c.Correspondencia = XmlConvert.ToInt32(elemento.Attribute("Correspondencia").Value);

                xAttribute = elemento.Attribute("Dispositivo");
                if (xAttribute != null)
                    c.Dispositivo = XmlConvert.ToInt32(elemento.Attribute("Dispositivo").Value);

                xAttribute = elemento.Attribute("NivelMinimo");
                if (xAttribute != null)
                    c.NivelMinimo = XmlConvert.ToInt32(elemento.Attribute("NivelMinimo").Value);

                xAttribute = elemento.Attribute("NivelMaximo");
                if (xAttribute != null)
                    c.NivelMaximo = XmlConvert.ToInt32(elemento.Attribute("NivelMaximo").Value);

                xAttribute = elemento.Attribute("IsBase");
                if (xAttribute != null)
                    c.IsBase = XmlConvert.ToBoolean(elemento.Attribute("IsBase").Value);

                xAttribute = elemento.Attribute("Seguidor");
                if (xAttribute != null)
                    c.Seguidor = XmlConvert.ToInt32(elemento.Attribute("Seguidor").Value);

                xAttribute = elemento.Attribute("Step");
                if (xAttribute != null)
                    c.Step = XmlConvert.ToInt32(elemento.Attribute("Step").Value);

                xAttribute = elemento.Attribute("VolumePurga");
                if (xAttribute != null)
                    c.VolumePurga = XmlConvert.ToDouble(elemento.Attribute("VolumePurga").Value);

                xAttribute = elemento.Attribute("ColorRGB");
                if (xAttribute != null)
                    c.ColorRGB = elemento.Attribute("ColorRGB").Value;

                xAttribute = elemento.Attribute("IsBicoIndividual");
                if (xAttribute != null)
                    c.IsBicoIndividual = XmlConvert.ToBoolean(elemento.Attribute("IsBicoIndividual").Value);

                xAttribute = elemento.Attribute("VolumeBicoIndividual");
                if (xAttribute != null)
                    c.VolumeBicoIndividual = XmlConvert.ToDouble(elemento.Attribute("VolumeBicoIndividual").Value);

                list.Add(c);
            }

            return list;
        }

        public static bool Validate(List<Colorante> lista, out string outMsg)
        {
            if (lista == null)
                throw new ArgumentNullException();

            StringBuilder validaItem = new StringBuilder();
            StringBuilder validaLista = new StringBuilder();

            //Seleciona somente habilitados
            lista = lista.Where(w => w.Habilitado == true).ToList();
            foreach (Colorante colorante in lista)
            {
                validaItem.Clear();

                //[Valida apenas colorantes habilitados]
                if (!colorante.Habilitado)
                    continue;

                if (string.IsNullOrEmpty(colorante.Nome))
                    validaItem.AppendLine(Properties.UI.Colorantes_NomeObrigatorio);

                if (colorante.MassaEspecifica == 0)
                    validaItem.AppendLine(Properties.UI.Colorantes_MassaEspecificaObrigatoria);

                int count =
                    lista.Count(s => s.Correspondencia == colorante.Correspondencia);
                if (count > 1)
                    validaItem.AppendLine(Properties.UI.Colorantes_CorrespondenciaRepetida);

                if (validaItem.Length > 0)
                {
                    string texto =
                        string.Format(Properties.UI.Colorantes_Circuito, colorante.Circuito.ToString());
                    validaLista.AppendLine(texto);
                    validaLista.AppendLine(validaItem.ToString());
                }
            }

            outMsg = validaLista.ToString();
            return
                (validaLista.Length == 0);
        }

        public static void Persist(Colorante colorante)
        {
            XElement xml = XElement.Load(PathFile);
            XElement xe = xml.Elements()
                .Where(f => f.Attribute("Motor").Value == colorante.Circuito.ToString()).SingleOrDefault();

            xe.Attribute("Nome").SetValue(colorante.Nome);
            xe.Attribute("MassaEspecifica").SetValue(colorante.MassaEspecifica);

            if (xe.Attribute("Habilitado") == null)
                xe.Add(new XAttribute("Habilitado", colorante.Habilitado));
            else
                xe.Attribute("Habilitado").SetValue(colorante.Habilitado);

            if (xe.Attribute("Volume") == null)
                xe.Add(new XAttribute("Volume", colorante.Volume));
            else
                xe.Attribute("Volume").SetValue(colorante.Volume);

            if (xe.Attribute("Correspondencia") == null)
                xe.Add(new XAttribute("Correspondencia", colorante.Correspondencia));
            else
                xe.Attribute("Correspondencia").SetValue(colorante.Correspondencia);

            if (xe.Attribute("Dispositivo") == null)
                xe.Add(new XAttribute("Dispositivo", colorante.Dispositivo));
            else
                xe.Attribute("Dispositivo").SetValue(colorante.Dispositivo);

            if (xe.Attribute("NivelMinimo") == null)
                xe.Add(new XAttribute("NivelMinimo", colorante.NivelMinimo));
            else
                xe.Attribute("NivelMinimo").SetValue(colorante.NivelMinimo);

            if (xe.Attribute("NivelMaximo") == null)
                xe.Add(new XAttribute("NivelMaximo", colorante.NivelMaximo));
            else
                xe.Attribute("NivelMaximo").SetValue(colorante.NivelMaximo);

            if (xe.Attribute("IsBase") == null)
                xe.Add(new XAttribute("IsBase", colorante.IsBase));
            else
                xe.Attribute("IsBase").SetValue(colorante.IsBase);

            if (xe.Attribute("Seguidor") == null)
                xe.Add(new XAttribute("Seguidor", colorante.Seguidor));
            else
                xe.Attribute("Seguidor").SetValue(colorante.Seguidor);

            if (xe.Attribute("Step") == null)
                xe.Add(new XAttribute("Step", colorante.Step));
            else
                xe.Attribute("Step").SetValue(colorante.Step);

            if (xe.Attribute("VolumePurga") == null)
                xe.Add(new XAttribute("VolumePurga", colorante.VolumePurga));
            else
                xe.Attribute("VolumePurga").SetValue(colorante.VolumePurga);

            if (xe.Attribute("ColorRGB") == null)
                xe.Add(new XAttribute("ColorRGB", colorante.ColorRGB));
            else
                xe.Attribute("ColorRGB").SetValue(colorante.ColorRGB);

            if (xe.Attribute("IsBicoIndividual") == null)
                xe.Add(new XAttribute("IsBicoIndividual", colorante.IsBicoIndividual));
            else
                xe.Attribute("IsBicoIndividual").SetValue(colorante.IsBicoIndividual);

            if (xe.Attribute("VolumeBicoIndividual") == null)
                xe.Add(new XAttribute("VolumeBicoIndividual", colorante.VolumeBicoIndividual));
            else
                xe.Attribute("VolumeBicoIndividual").SetValue(colorante.VolumeBicoIndividual);

            xml.Save(PathFile);
        }

        public static void Persist(List<Colorante> lista)
        {
            XElement xml = XElement.Load(PathFile);

            foreach (Colorante c in lista)
            {
                XElement xeColorante =
                    xml.Elements().Where(f => f.Attribute("Motor").Value == c.Circuito.ToString()).First();

                xeColorante.Attribute("Nome").SetValue(c.Nome);
                xeColorante.Attribute("MassaEspecifica").SetValue(c.MassaEspecifica);

                if (xeColorante.Attribute("Habilitado") == null)
                    xeColorante.Add(new XAttribute("Habilitado", c.Habilitado));
                else
                    xeColorante.Attribute("Habilitado").SetValue(c.Habilitado);

                if (xeColorante.Attribute("Volume") == null)
                    xeColorante.Add(new XAttribute("Volume", c.Volume));
                else
                    xeColorante.Attribute("Volume").SetValue(c.Volume);

                if (xeColorante.Attribute("Correspondencia") == null)
                    xeColorante.Add(new XAttribute("Correspondencia", c.Correspondencia));
                else
                    xeColorante.Attribute("Correspondencia").SetValue(c.Correspondencia);

                if (xeColorante.Attribute("Dispositivo") == null)
                    xeColorante.Add(new XAttribute("Dispositivo", c.Dispositivo));
                else
                    xeColorante.Attribute("Dispositivo").SetValue(c.Dispositivo);

                if (xeColorante.Attribute("NivelMinimo") == null)
                    xeColorante.Add(new XAttribute("NivelMinimo", c.NivelMinimo));
                else
                    xeColorante.Attribute("NivelMinimo").SetValue(c.NivelMinimo);

                if (xeColorante.Attribute("NivelMaximo") == null)
                    xeColorante.Add(new XAttribute("NivelMaximo", c.NivelMaximo));
                else
                    xeColorante.Attribute("NivelMaximo").SetValue(c.NivelMaximo);

                if (xeColorante.Attribute("IsBase") == null)
                    xeColorante.Add(new XAttribute("IsBase", c.IsBase));
                else
                    xeColorante.Attribute("IsBase").SetValue(c.IsBase);

                if (xeColorante.Attribute("Seguidor") == null)
                    xeColorante.Add(new XAttribute("Seguidor", c.Seguidor));
                else
                    xeColorante.Attribute("Seguidor").SetValue(c.Seguidor);

                if (xeColorante.Attribute("Step") == null)
                    xeColorante.Add(new XAttribute("Step", c.Step));
                else
                    xeColorante.Attribute("Step").SetValue(c.Step);

                if (xeColorante.Attribute("VolumePurga") == null)
                    xeColorante.Add(new XAttribute("VolumePurga", c.VolumePurga));
                else
                    xeColorante.Attribute("VolumePurga").SetValue(c.VolumePurga);

                if (xeColorante.Attribute("ColorRGB") == null)
                    xeColorante.Add(new XAttribute("ColorRGB", c.ColorRGB));
                else
                    xeColorante.Attribute("ColorRGB").SetValue(c.ColorRGB);

                if (xeColorante.Attribute("IsBicoIndividual") == null)
                    xeColorante.Add(new XAttribute("IsBicoIndividual", c.IsBicoIndividual));
                else
                    xeColorante.Attribute("IsBicoIndividual").SetValue(c.IsBicoIndividual);

                if (xeColorante.Attribute("VolumeBicoIndividual") == null)
                    xeColorante.Add(new XAttribute("VolumeBicoIndividual", c.VolumeBicoIndividual));
                else
                    xeColorante.Attribute("VolumeBicoIndividual").SetValue(c.VolumeBicoIndividual);
            };

            xml.Save(PathFile);
        }

        #endregion
    }
}