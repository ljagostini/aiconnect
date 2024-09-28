using Percolore.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Core
{
    public class DatPatternSinteplast : IDat
    {
        private bool UTILIZAR_CORRESPONDENCIA;
        private List<ListCorrespondencia> LISTA_CORRESPONDENCIA;
        public int defaultUnit = 0; //UNT default é em milimetro
                                    //UNT default 1 em gramas

        private string _linhaExtKey = string.Empty;
        private string _linhaCheckFormula = string.Empty;
        private string _linhaPRD;
        private string _linhaFRM = string.Empty;
        private string _linhaBAS;
        private string _linhaSV;

        private string _codigoCor = string.Empty;
        private string _quantidades = string.Empty;

        private string _baseCor = string.Empty;

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

        public DatPatternSinteplast(string conteudo, int basePosicaoCircuito, bool utilizarCorrespondencia, List<ListCorrespondencia> listaCorrespondencia)
        {
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;
            try
            {
                string trabalho = conteudo.Replace("\n", "");
                string[] linhas = trabalho.Split('\r');
                if (linhas != null && linhas.Length > 0)
                {
                    int nLinha = 0;
                    foreach (string strLinha in linhas)
                    {
                        if(nLinha ==0)
                        {
                            this._linhaSV = strLinha;
                        }
                        else if(nLinha == 1)
                        {
                            DesmontaBAS(strLinha);
                        }
                        else if (nLinha == 2)
                        {
                            this._linhaPRD = strLinha;
                        }
                        else
                        {
                            DesmontaFRM(strLinha);
                        }
                        nLinha++;
                    }
                }
            }
            catch
            { }
            this._codigoCor = this._linhaPRD + ";" + _linhaBAS + ";@CNX " + this._linhaFRM + ";";
            if (this._linhaSV != String.Empty)
            {
                this._codigoCor += this._linhaSV + ";";
                AssemblyInfo info = new AssemblyInfo(Assembly.GetExecutingAssembly());
                this._codigoCor += "IO " + info.AssemblyComercialVersion + ";";

            }
        }

        private void DesmontaFRM(string strFmr)
        {

            string strfrmLimpo = strFmr;
            try
            {
                //Removendo os espacos duplos.....
                while (strfrmLimpo.Contains("  "))
                {
                    strfrmLimpo = strfrmLimpo.Replace("  ", " ");
                }
                if (strfrmLimpo.Contains("\""))
                {
                    strfrmLimpo = strfrmLimpo.Replace("\"", "");
                }
                if (strfrmLimpo.Contains(";"))
                {
                    string[] controle = strfrmLimpo.Split(';');
                    if (controle != null)
                    {
                        if (this._linhaFRM == string.Empty)
                        {
                            this._linhaFRM = controle[0] + "," + controle[1].Replace(",", ".");
                        }
                        else
                        {
                            this._linhaFRM += "," + controle[0] + "," + controle[1].Replace(",", ".");
                        }
                        
                    }
                }
            }
            catch
            { }
        }
        
        private void DesmontaBAS(string strBas)
        {
            string montando = string.Empty;
            string strbasLimpo = strBas;
            try
            {
                //Removendo os espacos duplos.....
                while (strbasLimpo.Contains("  "))
                {
                    strbasLimpo = strbasLimpo.Replace("  ", " ");
                }
                if (strbasLimpo.Contains("\""))
                {
                    strbasLimpo = strbasLimpo.Replace("\"", "");
                }

                string[] controle = strbasLimpo.Split('-');
                if (controle != null)
                {
                    this._baseCor = controle[1];

                }
                montando = strbasLimpo;
            }
            catch
            { }
            this._linhaBAS = montando;
        }

        public bool Validar()
        {
            bool retorno =
                //(!string.IsNullOrEmpty(_linhaUNT)
                //&& !string.IsNullOrEmpty(_linhaCAN)
                //&& 
                (!string.IsNullOrEmpty(_linhaFRM));


            try
            {
                string[] vMls = _linhaFRM.Split(new char[] { ',' });
                if (vMls == null || vMls.Length < 2)
                {
                    retorno = false;
                }
            }
            catch
            { }

            return retorno;
        }

        public Dictionary<int, double> GetQuantidades()
        {
            string[] vShots = _linhaFRM.Split(new char[] { ',' });

            Dictionary<int, double> qtdes =
              new Dictionary<int, double>();

            for (int index = 0; (index + 1) <= vShots.GetUpperBound(0); index += 2)
            {
                int CIRCUITO = 0;
                string _corr = vShots[index];
                ListCorrespondencia _lCorv = LISTA_CORRESPONDENCIA.Find(o => o.CodigoProduto.Equals(_corr));
                if (_lCorv != null)
                {
                    CIRCUITO = _lCorv.Circuito;
                    double QtdML = double.Parse(vShots[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    //(ml) milimitros
                    qtdes.Add(CIRCUITO, QtdML);
                    
                }
            }

            return qtdes;
        }

    }
}
