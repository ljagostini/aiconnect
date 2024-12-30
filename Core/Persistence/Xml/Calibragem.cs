using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Percolore.Core.Persistence.Xml
{
    public class Calibragem
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Calibragem.xml");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Motor { get; set; }
        public int UltimoPulsoReverso { get; set; }
        public List<ValoresVO> Valores { get; set; }
        public int MinimoFaixas { get; set; } = 5;

        #region Métodos

        public static Calibragem Load(int motor)
        {
            Calibragem c = null;
            int ultimoPRev = 0;
            int minimoFaixas = 5;

            XElement xml = XElement.Load(PathFile);

            //Calibragem
            XElement xeCalibragem =
                xml.Elements()
                .Where(f => f.Attribute("Motor").Value == motor.ToString())
                .First();

            var ultimoPulsoReversoXml = xeCalibragem.Attribute("UltimoPulsoReverso");
            if (ultimoPulsoReversoXml != null)
            {
                int.TryParse(ultimoPulsoReversoXml.Value, out int ultimoPulsoReverso);
                ultimoPRev = ultimoPulsoReverso;
            }

            var minimoFaixasXml = xeCalibragem.Attribute("MinimoFaixas");
            if (minimoFaixasXml != null)
                int.TryParse(minimoFaixasXml.Value, out minimoFaixas);
            if (minimoFaixas < 3)
                minimoFaixas = 3;

            c = new Calibragem() { Motor = motor, Valores = new List<ValoresVO>(), UltimoPulsoReverso = ultimoPRev, MinimoFaixas = minimoFaixas };
            ValoresVO valores = null;

            //Valores
            foreach (XElement v in xeCalibragem.Elements())
            {
                double Volume = 0;
                int PulsoHorario = 0;
                int Velocidade = 0;
                int Delay = 0;
                double MassaMedia = 0;
                double DesvioMedio = 0;
                int PulsoReverso = 0;
                int Aceleracao = 0;

                var atributoXml = v.Attribute("Pulsos");
                if (atributoXml != null)
                {
                    int.TryParse(atributoXml.Value, out int pulsos);
                    PulsoHorario = pulsos;
                }

                atributoXml = v.Attribute("Velocidade");
                if (atributoXml != null)
                {
                    int.TryParse(atributoXml.Value, out int velocidade);
                    Velocidade = velocidade;
                }

                atributoXml = v.Attribute("ReverseDelay");
                if (atributoXml != null)
                {
                    int.TryParse(atributoXml.Value, out int reverseDelay);
                    Delay = reverseDelay;
                }

                atributoXml = v.Attribute("Volume");
                if (atributoXml != null)
                {
                    double.TryParse(atributoXml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double volume);
                    Volume = volume;
                }

                atributoXml = v.Attribute("MassaMedia");
                if (atributoXml != null)
                {
                    double.TryParse(atributoXml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double massaMedia);
                    MassaMedia = massaMedia;
                }

                atributoXml = v.Attribute("DesvioMedio");
                if (atributoXml != null)
                {
                    double.TryParse(atributoXml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double desvioMedio);
                    DesvioMedio = desvioMedio;
                }

                atributoXml = v.Attribute("PulsoReverso");
                if (atributoXml != null)
                {
                    int.TryParse(atributoXml.Value, out int pulsoReverso);
                    PulsoReverso = pulsoReverso;
                }

                atributoXml = v.Attribute("Aceleracao");
                if (atributoXml != null)
                {
                    int.TryParse(atributoXml.Value, out int aceleracao);
                    Aceleracao = aceleracao;
                }

                valores = new ValoresVO();
                valores.Volume = Volume;
                valores.PulsoHorario = PulsoHorario;
                valores.Velocidade = Velocidade;
                valores.Delay = Delay;
                valores.MassaMedia = MassaMedia;
                valores.DesvioMedio = DesvioMedio;
                valores.PulsoReverso = PulsoReverso;
                valores.Aceleracao = Aceleracao;

                c.Valores.Add(valores);
            }

            return c;
        }

        public static bool Validate(Calibragem c, out string outMsg)
        {
            #region Valida parâmetros

            if (c == null)
                throw new ArgumentNullException();

            if (c.Valores == null)
                throw new ArgumentNullException("");

            if (c.Valores.Count == 0)
            {
                outMsg = Properties.UI.Calibragem_NaoContemValor;
                return false;
            }

            #endregion

            StringBuilder validacoes = new StringBuilder();
            StringBuilder validacao = new StringBuilder();

            for (int i = 0; i <= 11; i++)
            {
                validacao.Clear();
                ValoresVO valores = c.Valores[i];

                //Pulsos
                if (valores.PulsoHorario == 0)
                    validacao.AppendLine(Properties.UI.Calibragem_QuantidadePulsosMaiorZero);

                //Velocidade
                if (valores.Velocidade < 0)
                {
                    validacao.AppendLine(Properties.UI.Calibragem_VelocidadeMaiorZero);
                }
                else
                {
                    /*
                    //[A velocidade não pode ser maior que a velocidade para o volume 
                    //na posição imediatamente acima]
                    if (i > 0)
                    {
                        int limite = c.Valores[i - 1].Velocidade;
                        if (valores.Velocidade > limite)
                        {
                            string texto =
                                string.Format(Properties.UI.Calibragem_VelocidadeMenorIgual, limite.ToString());
                            validacao.AppendLine(texto);
                        }
                    }
                    */

                }

                //Delay
                if (valores.Delay < 0)
                {
                    validacao.AppendLine(Properties.UI.Calibragem_DelayMaiorZero);
                }
                else
                {
                    /*
                    //[A velocidade não pode ser maior que a velocidade para o volume 
                    //na posição imediatamente acima]
                    if (i > 0)
                    {
                        int limite = c.Valores[i - 1].Delay;
                        if (valores.Delay > limite)
                        {
                            string texto =
                                string.Format(Properties.UI.Calibragem_DelayMenorIgual, limite.ToString());
                            validacao.AppendLine(texto);
                        }
                    }
                    */
                }

                if (validacao.Length > 0)
                {
                    string MililitroAbrev =
                        Properties.UI.Global_UnidadeMedida_Abreviacao_Mililitro;
                    validacoes.AppendLine("[" + valores.Volume.ToString() + MililitroAbrev);
                    validacoes.Append(validacao.ToString());
                    validacoes.AppendLine();
                }
            }

            outMsg = validacoes.ToString();
            return (validacoes.Length == 0);
        }

        public static void Add(Calibragem c)
        {
            XElement xeCalibragem = new XElement("Calibragem");
            xeCalibragem.Add(new XAttribute("Motor", c.Motor.ToString()));

            foreach (ValoresVO v in c.Valores)
            {
                XElement xeValores = new XElement("Valores");
                xeValores.Add(new XAttribute("Volume", v.Volume));
                xeValores.Add(new XAttribute("Pulsos", v.PulsoHorario));
                xeValores.Add(new XAttribute("Velocidade", v.Velocidade));
                xeValores.Add(new XAttribute("ReverseDelay", v.Delay));
                xeValores.Add(new XAttribute("MassaMedia", v.MassaMedia));
                xeValores.Add(new XAttribute("DesvioMedio", v.DesvioMedio));
                xeValores.Add(new XAttribute("Aceleracao", v.Aceleracao));
                xeValores.Add(new XAttribute("PulsoReverso", v.PulsoReverso));
                xeCalibragem.Add(xeValores);
            }

            XElement xml = XElement.Load(PathFile);
            xml.Add(xeCalibragem);
            xml.Save(PathFile);
        }

        public static void Update(Calibragem c)
        {
            //Xml
            XElement xml = XElement.Load(PathFile);

            //Calibragem
            XElement xeCalibragem =
                xml.Elements()
                .Where(f => f.Attribute("Motor").Value == c.Motor.ToString())
                .First();

            //Valores da calibragem
            foreach (ValoresVO v in c.Valores)
            {
                /* Adicionado em 21/11/2016 
                    * Garante que um double seja convertido em string
                    * exatamente como está na varável independente da cultura
                    * da aplicação.
                    * Isso foi feito para que não ocorram problemas na comparação do valor. */
                string volume = v.Volume.ToString(CultureInfo.InvariantCulture);

                XElement xeValores = xeCalibragem.Elements()
                    .Where(f => f.Attribute("Volume").Value == volume).First();

                xeValores.Attribute("Pulsos").SetValue(v.PulsoHorario);
                xeValores.Attribute("Velocidade").SetValue(v.Velocidade);
                xeValores.Attribute("ReverseDelay").SetValue(v.Delay);
                xeValores.Attribute("MassaMedia").SetValue(v.MassaMedia);
                xeValores.Attribute("DesvioMedio").SetValue(v.DesvioMedio);
                xeValores.Attribute("PulsoReverso").SetValue(v.PulsoReverso);
                xeValores.Attribute("Aceleracao").SetValue(v.Aceleracao);
            }

            xml.Save(PathFile);
        }

        public static void UpdatePulsosRev(int motor, int pulsosRev)
        {
            //Xml
            XElement xml = XElement.Load(PathFile);

            //Calibragem
            XElement xeCalibragem =
                xml.Elements()
                .Where(f => f.Attribute("Motor").Value == motor.ToString())
                .First();

            xeCalibragem.Attribute("UltimoPulsoReverso").SetValue(pulsosRev);


            xml.Save(PathFile);
        }

        public static void UpdateInstall(Calibragem c)
        {
            //Xml
            XElement xml = XElement.Load(PathFile);

            //Calibragem
            XElement xeCalibragem =
                xml.Elements()
                .Where(f => f.Attribute("Motor").Value == c.Motor.ToString())
                .First();

            //Valores da calibragem
            foreach (ValoresVO v in c.Valores)
            {
                /* Adicionado em 21/11/2016 
                 * Garante que um double seja convertido em string
                 * exatamente como está na varável independente da cultura
                 * da aplicação.
                 * Isso foi feito para que não ocorram problemas na comparação do valor. */
                string volume = v.Volume.ToString(CultureInfo.InvariantCulture);

                XElement xeValores = xeCalibragem.Elements()
                    .Where(f => f.Attribute("Volume").Value == volume).First();

                xeValores.Attribute("Pulsos").SetValue(v.PulsoHorario);
                xeValores.Attribute("Velocidade").SetValue(v.Velocidade);
                xeValores.Attribute("ReverseDelay").SetValue(v.Delay);
                xeValores.Attribute("MassaMedia").SetValue(v.MassaMedia);
                xeValores.Attribute("DesvioMedio").SetValue(v.DesvioMedio);
                if (xeValores.Attribute("PulsoReverso") == null)
                {
                    xeValores.Add(new XAttribute("PulsoReverso", v.PulsoReverso));
                }
                else
                {
                    xeValores.Attribute("PulsoReverso").SetValue(v.PulsoReverso);
                }
                if (xeValores.Attribute("Aceleracao") == null)
                {
                    xeValores.Add(new XAttribute("Aceleracao", v.Aceleracao));
                }
                else
                {
                    xeValores.Attribute("Aceleracao").SetValue(v.Aceleracao);
                }
            }

            xml.Save(PathFile);
        }

        public static void UpdateStructInstall(Calibragem c)
        {
            //Xml
            XElement xml = XElement.Load(PathFile);

            //Calibragem
            XElement xeCalibragem =
                xml.Elements()
                .Where(f => f.Attribute("Motor").Value == c.Motor.ToString())
                .First();
            if (xeCalibragem.Attribute("UltimoPulsoReverso") == null)
                xeCalibragem.Add(new XAttribute("UltimoPulsoReverso", 40));

            foreach (ValoresVO v in c.Valores)
            {
                XElement xeValores = new XElement("Valores");
                xeValores.Add(new XAttribute("Volume", v.Volume));
                xeValores.Add(new XAttribute("Pulsos", v.PulsoHorario));
                xeValores.Add(new XAttribute("Velocidade", v.Velocidade));
                xeValores.Add(new XAttribute("ReverseDelay", v.Delay));
                xeValores.Add(new XAttribute("MassaMedia", v.MassaMedia));
                xeValores.Add(new XAttribute("DesvioMedio", v.DesvioMedio));
                xeValores.Add(new XAttribute("Aceleracao", v.Aceleracao));
                xeValores.Add(new XAttribute("PulsoReverso", v.PulsoReverso));
                xeCalibragem.Add(xeValores);
            }

            xml.Save(PathFile);
        }

        #endregion
    }
}