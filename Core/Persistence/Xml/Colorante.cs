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
                c.MassaEspecifica = XmlConvert.ToDouble(elemento.Attribute("MassaEspecifica").Value);

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

                //
                xAttribute = elemento.Attribute("Dispositivo");
                if (xAttribute != null)
                    c.Dispositivo = XmlConvert.ToInt32(elemento.Attribute("Dispositivo").Value);

                xAttribute = elemento.Attribute("NivelMinimo");
                if (xAttribute != null)
                    c.NivelMinimo = XmlConvert.ToInt32(elemento.Attribute("NivelMinimo").Value);

                xAttribute = elemento.Attribute("NivelMaximo");
                if (xAttribute != null)
                    c.NivelMaximo = XmlConvert.ToInt32(elemento.Attribute("NivelMaximo").Value);

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

            //
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
            };

            xml.Save(PathFile);
        }

        #endregion
    }
}