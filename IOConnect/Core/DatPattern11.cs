using Percolore.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Percolore.IOConnect.Core
{
    public class DatPattern11 : IDat
    {
        private bool UTILIZAR_CORRESPONDENCIA;
        private List<ListCorrespondencia> LISTA_CORRESPONDENCIA;
        private List<Util.ObjectColorante> lColorantes = null;
        public int defaultUnit = 0; //UNT default é em milimetro
                                    //UNT default 1 em gramas

        private string _linhaExtKey = string.Empty;
        private string _linhaWHG;
        private string _linhaPRD;
        private string _linhaUNT = string.Empty;
        private string _linhaCAN;
        private string _linhaFRM = string.Empty;
        private string _linhaBAS;
        private string _linhaCLR;

        private double vFracaoUNT = 1;
        private double vFatorUNT = 1;
        private double vEmbdisp = 0;


        private string _codigoCor = string.Empty;
        private string _quantidades = string.Empty;

        private string _baseCor = string.Empty;

        private bool _checkFormula = false;
        private bool _extKey = false;

       

        private bool isRunAndEnd
        {
            get
            {
                bool retorno = true;
               
                return retorno;
            }
        }

        public bool ExtKey
        {
            get { return _extKey; }
        }

        public bool CheckFormula
        {
            get { return _checkFormula; }
        }

        public string BaseCor
        {
            get
            {
                return _baseCor;
            }
        }

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

        public DatPattern11(string conteudo, int basePosicaoCircuito, bool utilizarCorrespondencia, List<ListCorrespondencia> listaCorrespondencia)
        {
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;
            try
            {
                string trabalho = conteudo.Replace("\n", "");
                string[] linhas = trabalho.Split('\r');
                if (linhas != null && linhas.Length > 0)
                {
                    bool isFormula = false;
                    bool isIngrediente = false;

                    foreach (string strLinha in linhas)
                    {
                        if(strLinha.Contains("[FORMULA"))
                        {
                            isFormula = true;
                            isIngrediente = false;
                        }
                        else if(strLinha.Contains("[INGREDIENTS"))
                        {
                            isFormula = false;
                            isIngrediente = true;
                        }
                        else if(!strLinha.Contains("="))
                        {
                            isFormula = false;
                            isIngrediente = false;
                        }

                        if (isFormula)
                        {
                            if (strLinha.Contains("CanSize="))
                            {
                                DesmontaCAN(strLinha.Replace("\n", ""));
                            }
                            if (strLinha.Contains("DispenseUnit="))
                            {
                                DesmontarWHG(strLinha.Replace("\n", ""));
                            }
                            else if (strLinha.Contains("Base="))
                            {
                                DesmontaBAS(strLinha.Replace("\n", ""));
                            }
                            else if (strLinha.Contains("MainProduct="))
                            {
                                DesmontarCLR(strLinha.Replace("\n", ""));
                            }
                            else if (strLinha.Contains("ColorName="))
                            {
                                DesmontarPRD(strLinha.Replace("\n", ""));
                            }
                        }
                        else if (isIngrediente)
                        {
                            if (strLinha.Contains("="))
                            {
                                DesmontarFRM(strLinha);
                            }
                        }
                       
                    }
                }
            }
            catch
            { }
            this._codigoCor = this._linhaPRD + ";" + this._linhaCLR.Replace("|", "/") + ";" + this._linhaCAN + ";" + this._linhaUNT + ";" + _linhaBAS + ";@FRM " + this._linhaFRM + ";";
        }

        private void DesmontarFRM(string strFRM)
        {
            string montando = string.Empty;
            if (!string.IsNullOrEmpty(this._linhaFRM))
            {
                montando = this._linhaFRM;
            }
            string strfrmLimpo = strFRM;
            try
            {
                //Removendo os espacos duplos.....
                while (strfrmLimpo.Contains("  "))
                {
                    strfrmLimpo = strfrmLimpo.Replace("  ", " ");
                }
                string[] controle = strfrmLimpo.Split('=');
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    montando = controle[0] + ",";
                    if(controle[1].Contains(","))
                    {
                        montando += controle[1].Replace(",", ".");
                    }
                    else
                    {
                        montando += controle[1];
                    }

                }
            }
            catch
            {
            }
            if (string.IsNullOrEmpty(this._linhaFRM))
            {
                this._linhaFRM = montando;
            }
            else
            {
                this._linhaFRM += "," + montando;
            }
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

                string[] controle = strbasLimpo.Split('=');
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

        private void DesmontaCAN(string strCAN)
        {
            string montando = string.Empty;
            string strcanLimpo = strCAN;
            try
            {
                //Removendo os espacos duplos.....
                while (strcanLimpo.Contains("  "))
                {
                    strcanLimpo = strcanLimpo.Replace("  ", " ");
                }
                strcanLimpo = strcanLimpo.Replace("\"", "");
                string[] controle = strcanLimpo.Split('=');
                if (controle != null)
                {
                    double embdisp = -1;
                    embdisp = double.Parse(controle[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    
                    if (embdisp >= 0)
                    {
                        this.vEmbdisp = embdisp;
                        montando = strCAN;
                    }

                }
            }
            catch
            { }
            this._linhaCAN = montando;
        }

        private void DesmontarWHG(string strWHG)
        {
            string montando = string.Empty;
            string strwghLimpo = strWHG;
            try
            {
                //Removendo os espacos duplos.....
                while (strwghLimpo.Contains("  "))
                {
                    strwghLimpo = strwghLimpo.Replace("  ", " ");
                }
                strwghLimpo = strwghLimpo.Replace("\"", "");
                string[] controle = strwghLimpo.Split('=');
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    if (controle[1] == "ml")
                    {
                        this.defaultUnit = 0;   //(ml)
                    }
                    else
                    {
                        this.defaultUnit = 1;   //(gr)
                    }
                }
            }
            catch
            {

            }
            this._linhaWHG = montando;

        }

        private void DesmontarCLR(string strCLR)
        {
            string strclrLimpo = strCLR;
            try
            {
                //Removendo os espacos duplos.....
                while (strclrLimpo.Contains("  "))
                {
                    strclrLimpo = strclrLimpo.Replace("  ", " ");
                }
                strclrLimpo = strclrLimpo.Replace("\"", "");               
            }
            catch
            {

            }
            this._linhaCLR = strclrLimpo;

        }

        private void DesmontarPRD(string strPRD)
        {
            string montando = string.Empty;
            string strPRDLimpo = strPRD;
            try
            {
                //Removendo os espacos duplos.....
                while (strPRDLimpo.Contains("  "))
                {
                    strPRDLimpo = strPRDLimpo.Replace("  ", " ");
                }
                if (strPRDLimpo.Contains("\""))
                {
                    strPRDLimpo = strPRDLimpo.Replace("\"", "");
                }
                string[] controle = strPRDLimpo.Split('=');
                if (controle != null)
                {
                    this._codigoCor = controle[1];
                    if (controle.Length > 2)
                    {
                        this._codigoCor += " - " + controle[2];
                    }

                }
                montando = strPRDLimpo;
            }
            catch
            {

            }
            this._linhaPRD = montando;

        }

        public bool Validar()
        {
            bool retorno =
                (!string.IsNullOrEmpty(_linhaFRM)
                && this.isRunAndEnd);


            try
            {
                string[] vShots = _linhaFRM.Split(new char[] { ',' });
                if (vShots == null || vShots.Length < 2)
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
                ListCorrespondencia _lCorv = LISTA_CORRESPONDENCIA.Find(o => o.CodigoProduto == _corr);
                if (_lCorv != null)
                {
                    CIRCUITO = _lCorv.Circuito;
                    double SHOT = double.Parse(vShots[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    //double.TryParse(vShots[index + 1], out SHOT);
                    //(ml) milimitros
                    if (this.defaultUnit == 0)
                    {
                        double QUANTIDADE = (this.vFatorUNT / this.vFracaoUNT) * SHOT;
                        //double QUANTIDADE = 0.462 * SHOT;
                        //double QUANTIDADE = SHOT;
                        qtdes.Add(CIRCUITO, QUANTIDADE);
                    }
                    //(gr) gramas
                    else
                    {
                        double QUANTIDADE = (this.vFatorUNT / this.vFracaoUNT) * SHOT;
                        Util.ObjectColorante obj_c = this.lColorantes.Find(o => o.Correspondencia == _lCorv.Circuito);
                        if (obj_c != null)
                        {
                            qtdes.Add(CIRCUITO, (QUANTIDADE / obj_c.MassaEspecifica));
                        }
                        else
                        {
                            qtdes.Add(CIRCUITO, QUANTIDADE);
                        }
                    }
                }
            }

            return qtdes;
        }

        public string getExtKey()
        {
            return this._linhaExtKey;
        }
    }
}
