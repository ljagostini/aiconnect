using System.Text.RegularExpressions;

namespace Percolore.Core
{
	public class DatPattern04 : IDat
    {
        private int BASE_POSICAO_CIRCUITO;
        private bool UTILIZAR_CORRESPONDENCIA;
        private Dictionary<int, int> LISTA_CORRESPONDENCIA;

        private Regex RGX_UNT = new Regex(@"@UNT\s+[0-9]+(\.[0-9]+)*\s+[0-9]+(\.[0-9]+)*", RegexOptions.Compiled);
        private Regex RGX_UNT_FATOR_FRACAO = new Regex(@"[0-9]+(\.[0-9]+)*", RegexOptions.Compiled);

        private Regex RGX_CAN = new Regex(@"@CAN.*\s{1}[0-9]+(\.[0-9]+)*\s{1}", RegexOptions.Compiled);
        private Regex RGX_CAN_EMB_DISP = new Regex(@"\s{1}[0-9]+(\.[0-9]+)*\s{1}", RegexOptions.Compiled);

        //private Regex RGX_FRM = new Regex(@"@FRM(\s)*""{1}(\d{1,2},{1}[0-9]+\.[0-9]+,?)+""{1}\s*[0-9]+(\.[0-9]+)*", RegexOptions.Compiled);
        private Regex RGX_FRM = new Regex(@"@FRM(\s)*""{1}(\d{1,2},{1}[0-9]+(\.[0-9]+)*,?)+""{1}\s*[0-9]+(\.[0-9]+)*", RegexOptions.Compiled);
        private Regex RGX_FRM_QTDES = new Regex(@"(\d{1,2},{1}[0-9]+(\.[0-9]+)*,?)+", RegexOptions.Compiled);

        private Regex RGX_BAS = new Regex(@"@BAS.*\s", RegexOptions.Compiled);
        private Regex RGX_CLR = new Regex(@"@CLR.*\s", RegexOptions.Compiled);

        private string _linhaUNT;
        private string _linhaCAN;
        private string _linhaFRM;
        private string _linhaBAS;
        private string _linhaCRL;
        private string _codigoCor = string.Empty;
        private string _quantidades = string.Empty;

        public string CodigoCor
        {
            get
            {
                return _codigoCor;
            }
        }

        public string Quantidades
        {
            get
            {
                return _quantidades;
            }
        }

        public DatPattern04(string conteudo, int basePosicaoCircuito,
            bool utilizarCorrespondencia, Dictionary<int, int> listaCorrespondencia)
        {
            BASE_POSICAO_CIRCUITO = basePosicaoCircuito;
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;

            _linhaUNT = RGX_UNT.Match(conteudo).Value;
            _linhaCAN = RGX_CAN.Match(conteudo).Value;
            _linhaFRM = RGX_FRM.Match(conteudo).Value;
            _linhaBAS = RGX_BAS.Match(conteudo).Value;
            _linhaCRL = RGX_CLR.Match(conteudo).Value;
            /*
            string[] vUNT = _linhaUNT.Split(new char[] { ' ', '\"' }, StringSplitOptions.RemoveEmptyEntries);

            double FATOR = 0;
            double.TryParse(vUNT[1], out FATOR);
            double FRACAO = 0;
            double.TryParse(vUNT[2], out FRACAO);
            */
            _codigoCor = _linhaCAN + ";" + _linhaCRL + ";" + _linhaBAS + ";";
        }

        public bool Validar()
        {
            return
                (!string.IsNullOrEmpty(_linhaUNT)
                && !string.IsNullOrEmpty(_linhaCAN)
                && !string.IsNullOrEmpty(_linhaFRM));
        }

        public Dictionary<int, double> GetQuantidades()
        {
            //[Valores decimais referentes ao fator e fração]
            string[] vUNT =
                 _linhaUNT.Split(new char[] { ' ', '\"' }, StringSplitOptions.RemoveEmptyEntries);



            double FATOR = 0;
            double.TryParse(vUNT[1], out FATOR);
            double FRACAO = 0;
            double.TryParse(vUNT[2], out FRACAO);

            //[Quantidade da embalagem de dispensa]
            string[] vCAN =
               _linhaCAN.Split(new char[] { ' ', '\"' }, StringSplitOptions.RemoveEmptyEntries);

            double EMB_DISP = 0;
            double.TryParse(vCAN[vCAN.GetUpperBound(0)], out EMB_DISP);

            //[Vetor com valores da linha FRM]
            string[] vFRM =
                _linhaFRM.Split(new char[] { ' ', '\"' }, StringSplitOptions.RemoveEmptyEntries);

            //[Quantidade da embalagem de referência]
            double EMB_REF = 0;
            double.TryParse(vFRM[2], out EMB_REF);

            //[Posição e quantidade dos colorantes que serão dispensados]
            string[] vShots = vFRM[1].Split(new char[] { ',' });

            Dictionary<int, double > qtdes =
                new Dictionary<int, double>();

            //[Percorre vetor de 2 em 2 (PAR = posição | ÍMPAR = quantidade)]
            for (int index = 0; index <= vShots.GetUpperBound(0); index += 2)
            {
                int CIRCUITO = 0;
                int.TryParse(vShots[index], out CIRCUITO);

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

                double SHOT = 0;
                double.TryParse(vShots[index + 1], out SHOT);

                double QUANTIDADE = ((FATOR / FRACAO) * (EMB_DISP / EMB_REF)) * SHOT;
                qtdes.Add(CIRCUITO, QUANTIDADE);
            }

            return qtdes;
        }
    }
}