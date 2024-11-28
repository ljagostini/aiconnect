using System.Globalization;
using System.Text.RegularExpressions;

namespace Percolore.Core
{
	public class DatPattern02 : IDat
    {
        private int BASE_POSICAO_CIRCUITO;
        private bool UTILIZAR_CORRESPONDENCIA;
        private Dictionary<int, int> LISTA_CORRESPONDENCIA;

        private Regex RGX_FRM =
            new Regex(@"@FRM(\s)*""{1}(\d{1,2},{1}[0-9]+(\.[0-9]+)*,?)+""{1}", RegexOptions.Compiled);

        private Regex RGX_FRM_QTDES =
            new Regex(@"(\d{1,2},{1}[0-9]+(\.[0-9]+)*,?)+", RegexOptions.Compiled);

        private string _conteudo;
        private string _linhaFRM;
        private string _codigoCor = string.Empty;
        private string _quantidades = string.Empty;


        public string CodigoCor
        {
            get { return _codigoCor; }
        }

        public string Quantidades
        {
            get { return _quantidades; }
        }

        public DatPattern02(string conteudo, int basePosicaoCircuito,
            bool utilizarCorrespondencia, Dictionary<int, int> listaCorrespondencia)
        {
            BASE_POSICAO_CIRCUITO = basePosicaoCircuito;
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;

            this._conteudo = conteudo;
            _linhaFRM = RGX_FRM.Match(conteudo).Value;
        }

        public bool Validar()
        {
            return !string.IsNullOrEmpty(_linhaFRM);
        }

        public Dictionary<int, double> GetQuantidades()
        {
            Dictionary<int, double> qtdes = null;
            Match match = RGX_FRM_QTDES.Match(_linhaFRM);

            if (match.Success)
            {
                string[] vQtde =
                    match.Value.Split(new char[] { ',' });
                qtdes = new Dictionary<int, double>();

                //Percorre vetor de 2 em 2 (PAR = posição | ÍMPAR = quantidade)
                for (int index = 0; index <= vQtde.GetUpperBound(0); index += 2)
                {
                    int CIRCUITO;
                    int.TryParse(vQtde[index], out CIRCUITO);

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
                    double.TryParse(vQtde[index + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out QUANTIDADE);

                    qtdes.Add(CIRCUITO, QUANTIDADE);
                }
            }

            return qtdes;
        }
    }
}