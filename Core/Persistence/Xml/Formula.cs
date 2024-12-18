using System.Xml.Linq;

namespace Percolore.Core.Persistence.Xml
{
	public class Formula
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Formulas.xml");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public Guid Id { get; internal set; }
        public string Nome { get; set; }
        public List<FormulaItem> Itens { get; set; }

        #region Contrutores

        public Formula()
        {
            Itens = new List<FormulaItem>();
        }

        public Formula(XElement xelement)
        {
            Id = Guid.Parse(xelement.Attribute("Id").Value);
            Nome = xelement.Attribute("Nome").Value;
            Itens = new List<FormulaItem>();

            //Itens      
            foreach (XElement xeItem in xelement.Elements())
            {
                int _IdCorante = int.Parse(xeItem.Attribute("IdCorante").Value);
                FormulaItem item = new FormulaItem();
                item.Mililitros = double.Parse(xeItem.Attribute("Volume").Value, System.Globalization.CultureInfo.InvariantCulture);
                item.IdColorante = _IdCorante;

                Itens.Add(item);
                item = null;
            };
        }

        #endregion

        #region Métodos públicos   

        public static void Persist(Formula formula)
        {
            if (formula.Id == Guid.Empty)
            {
                formula.Id = Guid.NewGuid();
                Add(formula);
            }
            else
            {
                Update(formula);
            }
        }

        public static void Delete(Guid id)
        {
            XElement xml = XElement.Load(PathFile);
            XElement xeFormula =
                xml.Elements()
                .Where(xe => xe.Attribute("Id").Value == id.ToString())
                .First();

            xeFormula.Remove();
            xml.Save(PathFile);
        }

        public static Formula Load(Guid id)
        {
            Formula formula = null;

            XElement xml = XElement.Load(PathFile);
            XElement xeFormula =
                xml.Elements()
                .Where(xe => xe.Attribute("Id").Value == id.ToString())
                .First();

            formula = new Formula(xeFormula);
            return formula;
        }

        public static List<Formula> List()
        {
            List<Formula> list = new List<Formula>();

            IEnumerable<XElement> xml =
                XElement.Load(PathFile).Elements();

            foreach (XElement xeFormula in xml)
            {
                Formula formula = new Formula(xeFormula);
                list.Add(formula);
            }

            return list;
        }

        #endregion

        #region Métodos privados

        static void Add(Formula formula)
        {
            XElement xeFormula = new XElement("Formula");
            xeFormula.Add(new XAttribute("Id", formula.Id));
            xeFormula.Add(new XAttribute("Nome", formula.Nome));

            foreach (FormulaItem item in formula.Itens)
            {
                XElement xeItem = new XElement("Item");
                xeItem.Add(new XAttribute("IdCorante", item.IdColorante));
                xeItem.Add(new XAttribute("Volume", item.Mililitros));
                xeFormula.Add(xeItem);
            }

            XElement xml = XElement.Load(PathFile);
            xml.Add(xeFormula);
            xml.Save(PathFile);
        }

        static void Update(Formula formula)
        {
            XElement xml = XElement.Load(PathFile);
            XElement xeFormula =
                xml.Elements()
                .Where(xe => xe.Attribute("Id").Value == formula.Id.ToString())
                .First();

            xeFormula.Attribute("Nome").SetValue(formula.Nome);
            xeFormula.Elements().Remove();
            foreach (FormulaItem item in formula.Itens)
            {
                XElement xeItem = new XElement("Item");
                xeItem.Add(new XAttribute("IdCorante", item.IdColorante));
                xeItem.Add(new XAttribute("Volume", item.Mililitros));
                xeFormula.Add(xeItem);
            }

            xml.Save(PathFile);
        }

        #endregion
    }
}