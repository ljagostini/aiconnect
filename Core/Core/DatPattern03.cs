using System.Globalization;
using System.Text.RegularExpressions;

namespace Percolore.Core
{
	public class DatPattern03 : IDat
    {
        private int BASE_POSICAO_CIRCUITO;
        private bool UTILIZAR_CORRESPONDENCIA;
        private Dictionary<int, int> LISTA_CORRESPONDENCIA;

        private Regex RGX_FRM =
            new Regex(@"#FRM(\s)*""{1}(\d{1,2},{1}[0-9]+(\.[0-9]+)*,?)+""{1}", RegexOptions.Compiled);

        private Regex RGX_FRM_QUANTIDADES =
            new Regex(@"(\d{1,2},{1}[0-9]+(\.[0-9]+)*,?)+", RegexOptions.Compiled);

        private Regex RGX_COR = new Regex(@"#COR.+", RegexOptions.Compiled);
        private Regex RGX_COR_CODIGO = new Regex(@"[\|]{1}[\d]+[-]*[\d]*[\|]{1}", RegexOptions.Compiled);

        private Regex RGX_BAS = new Regex(@"#BAS.*\s", RegexOptions.Compiled);
        private Regex RGX_PRD = new Regex(@"#PRD.*\s", RegexOptions.Compiled);
        private Regex RGX_EMB = new Regex(@"#EMB.*\s", RegexOptions.Compiled);

        private string _conteudo;
        private string _linhaFRM;
        private string _linhaCOR;
        private string _codigoCor;
        private string _quantidades;

        private string _linhaBAS;
        private string _linhaPRD;
        private string _linhaEMB;

        public string CodigoCor
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_codigoCor))
                {
                    _codigoCor =
                        RGX_COR_CODIGO.Match(_linhaCOR).Value.Replace("|", string.Empty);
                }

                return _codigoCor;
            }
        }

        public string Quantidades
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_quantidades))
                    _quantidades = _linhaFRM
                        .Replace("#FRM", string.Empty)
                        .Replace("\"", string.Empty)
                        .Trim();

                return _quantidades;
            }
        }

        public DatPattern03(string conteudo, int basePosicaoCircuito,
            bool utilizarCorrespondencia, Dictionary<int, int> listaCorrespondencia)
        {
            BASE_POSICAO_CIRCUITO = basePosicaoCircuito;
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;

            this._conteudo = conteudo;
            _linhaFRM = RGX_FRM.Match(_conteudo).Value;
            _linhaCOR = RGX_COR.Match(_conteudo).Value;
            _linhaBAS = RGX_BAS.Match(_conteudo).Value;
            _linhaPRD = RGX_PRD.Match(_conteudo).Value;
            _linhaEMB = RGX_EMB.Match(_conteudo).Value;
           
            _codigoCor = _linhaEMB + ";" + _linhaPRD + ";" + _linhaCOR.Replace("|", ",") + ";" + _linhaBAS + ";";
           
        }

        public bool Validar()
        {
            return !string.IsNullOrEmpty(_linhaFRM);
        }

        public Dictionary<int, double> GetQuantidades()
        {
            Dictionary<int, double> qtdes = null;
            Match match = RGX_FRM_QUANTIDADES.Match(_linhaFRM);

            if (match.Success)
            {
                string[] vQtdes =
                    match.Value.Split(new char[] { ',' });
                qtdes = new Dictionary<int, double>();

                //Percorre vetor de 2 em 2 (PAR = posição | ÍMPAR = quantidade)
                for (int index = 0; index <= vQtdes.GetUpperBound(0); index += 2)
                {
                    int CIRCUITO;
                    int.TryParse(vQtdes[index], out CIRCUITO);

                    if (UTILIZAR_CORRESPONDENCIA)
                    {
                        /*Se habilitada, utiliza lista de correspondência 
                         * para encontrar o colorante correto*/
                        CIRCUITO = LISTA_CORRESPONDENCIA[CIRCUITO];
                    }
                    else
                    {
                        /*Se base da posição de ciruito for 0(zero), incrementa. 
                         * O software considera 01 como base da posição do circuito de colorante*/
                        if (BASE_POSICAO_CIRCUITO == 0)
                            CIRCUITO++;
                    }

                    double QUANTIDADE = 0;
                    double.TryParse(vQtdes[index + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out QUANTIDADE);

                    qtdes.Add(CIRCUITO, QUANTIDADE);
                }
            }

            return qtdes;
        }
    }
}