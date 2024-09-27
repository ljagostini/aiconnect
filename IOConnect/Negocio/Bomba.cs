using Percolore.Core.Persistence.Xml;
using System.Text;
using System.Xml.Linq;

namespace Percolore.IOConnect
{
	public class Bomba2
    {
        public static string path { get; } = @".\Bombas.xml";

        public int Id { get; internal set; }
        public bool Habilitado { get; set; }
        public DateTime DataPurga { get; set; }
        public bool PurgaPendente { get; set; }
        public Colorante Colorante { get; set; }

        public Bomba2() { }

        #region Métodos públicos

        public static Bomba2 Get(int id)
        {
            XElement xml = XElement.Load(path);
            XElement xe = xml.Elements()
                .Where(f => f.Attribute("Id").Value == id.ToString()).First();

            Bomba2 b = new Bomba2();
            b.Id = int.Parse(xe.Attribute("Id").Value);
            b.Habilitado = bool.Parse(xe.Attribute("Habilitado").Value);
            b.DataPurga = DateTime.Parse(xe.Attribute("DataPurga").Value);
            b.PurgaPendente = bool.Parse(xe.Attribute("PurgaPendente").Value);
            b.Colorante = Colorante.Load(id);

            return b;
        }

        public static List<Bomba2> GetList()
        {
            List<Bomba2> list = new List<Bomba2>();

            IEnumerable<XElement> xml = XElement.Load(path).Elements();
            foreach (XElement xe in xml)
            {
                Bomba2 b = new Bomba2();
                b.Id = int.Parse(xe.Attribute("Id").Value);
                b.Habilitado = bool.Parse(xe.Attribute("Habilitado").Value);
                b.DataPurga = DateTime.Parse(xe.Attribute("DataPurga").Value);
                b.PurgaPendente = bool.Parse(xe.Attribute("PurgaPendente").Value);
                b.Colorante = Colorante.Load(b.Id);

                list.Add(b);
            }

            return list;
        }

        public static bool Validar(List<Colorante> lista, out string outMsg)
        {
            if (lista == null)
                throw new ArgumentNullException();

            StringBuilder validacao = new StringBuilder();
            StringBuilder validacoes = new StringBuilder();

            foreach (Colorante colorante in lista)
            {
                validacao.Clear();

                if (string.IsNullOrEmpty(colorante.Nome))
                    validacao.AppendLine("O nome do corante é um dado obrigatório.");

                if (colorante.MassaEspecifica == 0)
                    validacao.AppendLine("A massa específica deve ser maior que zero.");

                if (validacao.Length > 0)
                {
                    validacoes.AppendLine("[Colorante do motor " + colorante.Circuito.ToString() + "]");
                    validacoes.AppendLine(validacao.ToString());
                }
            }

            outMsg = validacoes.ToString();
            return
                (validacoes.Length == 0);
        }

        public static void Alterar(Colorante m)
        {
            XElement xml = XElement.Load(path);
            XElement xe = xml.Elements()
                .Where(f => f.Attribute("Motor").Value == m.Circuito.ToString()).First();

            xe.Attribute("Nome").SetValue(m.Nome);
            xe.Attribute("MassaEspecifica").SetValue(m.MassaEspecifica);
            xml.Save(path);
        }

        public static void Alterar(List<Colorante> lista)
        {
            XElement xml = XElement.Load(path);
            IEnumerable<XElement> eMassas = xml.Elements();

            foreach (Colorante m in lista)
            {
                XElement xe =
                    eMassas.Where(f => f.Attribute("Motor").Value == m.Circuito.ToString()).First();

                xe.Attribute("Nome").SetValue(m.Nome);
                xe.Attribute("MassaEspecifica").SetValue(m.MassaEspecifica);
            };

            xml.Save(path);
        }    

        #endregion
    }
}