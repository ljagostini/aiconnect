using Percolore.Core;
using System.Globalization;

namespace Percolore.IOConnect.Core
{
	public class DatPattern07 : IDat
    {
        private bool UTILIZAR_CORRESPONDENCIA;
        private List<ListCorrespondencia> LISTA_CORRESPONDENCIA;
        private List<Util.ObjectColorante> lColorantes = null;
        public int defaultUnit = 0; //UNT default é em shot
                                    

        private string _linhaExtKey = string.Empty;
        private string _linhaCheckFormula = string.Empty;
        private string _linhaPRD;
        private string _linhaUNT;
        private string _linhaCAN;
        private string _linhaFRM = string.Empty;
        private string _linhaBAS;

        private double vFracaoUNT = 1;
        private double vFatorUNT = 1;
        private double vEmbdisp = 0;

        private string _codigoCor = string.Empty;
        private string _quantidades = string.Empty;

        private string _baseCor = string.Empty;

        private bool _checkFormula = false;
        private bool _extKey = false;

        private bool _Run = false;
        private bool _End = false;

        public bool isRunAndEnd
        {
            get
            {
                bool retorno = false;
                if (_Run && _End)
                {
                    retorno = true;
                }
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
        public DatPattern07(string conteudo, int basePosicaoCircuito, bool utilizarCorrespondencia, List<ListCorrespondencia> listaCorrespondencia)
        {
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;
            
            string trabalho = conteudo.Replace("\n", "");
            string[] linhas = trabalho.Split('\r');
            if (linhas != null && linhas.Length > 0)
            {
                foreach (string strLinha in linhas)
                {                        
                    if (strLinha.Contains("@BAS"))
                    {
                        DesmontaBAS(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("@CAN"))
                    {
                        DesmontaCAN(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("@CNX"))
                    {
                        DesmontarFRM(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("@CNT"))
                    {
                        if (lColorantes == null)
                        {
                            lColorantes = Util.ObjectColorante.List().FindAll(o => o.Seguidor == -1 && o.Habilitado).ToList();
                        }
                        DesmontarFRMCodProd(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("@UNT"))
                    {
                        DesmontarUNT(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("@PRD"))
                    {
                        DesmontarPRD(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("@RUN"))
                    {
                        _Run = true;
                    }
                    else if (strLinha.Contains("@END"))
                    {
                        _End = true;
                    }
                }
            }

            this._codigoCor = this._linhaPRD + ";" + this._linhaCAN + ";" + this._linhaUNT + ";" + _linhaBAS + ";@CNX " + this._linhaFRM + ";";
        }

        private void DesmontarFRM(string strFRM)
        {
            string montando = string.Empty;
            if (!string.IsNullOrEmpty(this._linhaFRM))
            {
                montando = this._linhaFRM;
            }
            string strfrmLimpo = strFRM;
            
            //Removendo os espacos duplos.....
            while (strfrmLimpo.Contains("  "))
            {
                strfrmLimpo = strfrmLimpo.Replace("  ", " ");
            }
            string[] controle = strfrmLimpo.Split(' ');
            if (controle != null)
            {
                //Confirmando Prefixo nos parametros
                if (controle[0].Equals("@CNX"))
                {
                    for (int i = 1; i < controle.Length; i++)
                    {
                        if (montando == string.Empty)
                        {
                            montando += controle[i];
                        }
                        else
                        {
                            montando += "," + controle[i];
                        }
                    }
                }
            }

            this._linhaFRM += montando;
        }

        private void DesmontarFRMCodProd(string strFRM)
        {
            string montando = string.Empty;
            if (!string.IsNullOrEmpty(this._linhaFRM))
            {
                montando = this._linhaFRM;
            }
            string strfrmLimpo = strFRM;
            
            //Removendo os espacos duplos.....
            while (strfrmLimpo.Contains("  "))
            {
                strfrmLimpo = strfrmLimpo.Replace("  ", " ");
            }
            string[] controle = strfrmLimpo.Split(' ');
            if (controle != null)
            {
                //Confirmando Prefixo nos parametros
                if (controle[0].Equals("@CNT"))
                {

                    bool achouProd = false;
                    for (int i = 1; i < controle.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            if (achouProd)
                            {
                                if (montando == string.Empty)
                                {
                                    montando += controle[i];
                                }
                                else
                                {
                                    montando += "," + controle[i];
                                }
                            }
                            achouProd = false;
                        }
                        else
                        {
                            string mPr = controle[i];
                            if (mPr.Contains("\""))
                            {
                                mPr = mPr.Replace("\"", "");

                            }
                            Util.ObjectColorante nCol = lColorantes.Find(o => o.Nome == mPr);
                            if (nCol != null)
                            {
                                achouProd = true;
                                if (montando == string.Empty)
                                {
                                    montando += nCol.Correspondencia;
                                }
                                else
                                {
                                    montando += "," + nCol.Correspondencia;
                                }
                            }
                        }
                    }
                }
            }

            this._linhaFRM += montando;
        }

        private void DesmontaBAS(string strBas)
        {
            string montando = string.Empty;
            string strbasLimpo = strBas;
            
            //Removendo os espacos duplos.....
            while (strbasLimpo.Contains("  "))
            {
                strbasLimpo = strbasLimpo.Replace("  ", " ");
            }
            if (strbasLimpo.Contains("\""))
            {
                strbasLimpo = strbasLimpo.Replace("\"", "");
            }

            string[] controle = strbasLimpo.Split(' ');
            if (controle != null)
            {
                this._baseCor = controle[1];

            }
            montando = strbasLimpo;

            this._linhaBAS = montando;
        }

        private void DesmontaCAN(string strCAN)
        {
            string montando = string.Empty;
            string strcanLimpo = strCAN;
            
            //Removendo os espacos duplos.....
            while (strcanLimpo.Contains("  "))
            {
                strcanLimpo = strcanLimpo.Replace("  ", " ");
            }
            strcanLimpo = strcanLimpo.Replace("\"", "");
            string[] controle = strcanLimpo.Split(' ');
            if (controle != null && controle.Length > 0)
            {
                //Confirmando Prefixo nos parametros

                bool achou = false;
                double embdisp = -1;

                for (int i = controle.Length - 1; !achou && i < controle.Length; i++)
                {
                    if (controle[i] != null && controle[i].Length > 0)
                    {
                        //Ponto == 1 e vrigula == 0
                        if (controle[i].Contains(","))
                        {
                            embdisp = double.Parse(controle[i].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            achou = true;
                        }
                        else
                        {
                            embdisp = double.Parse(controle[i], System.Globalization.CultureInfo.InvariantCulture);
                            achou = true;
                        }
                    }
                }

                if (embdisp >= 0)
                {
                    this.vEmbdisp = embdisp;
                    montando = strCAN;
                }
            }

            this._linhaCAN = montando;
        }

        private void DesmontarUNT(string strUNT)
        {
            string montando = string.Empty;
            string struntLimpo = strUNT;
            
            //Removendo os espacos duplos.....
            while (struntLimpo.Contains("  "))
            {
                struntLimpo = struntLimpo.Replace("  ", " ");
            }
            string[] controle = struntLimpo.Split(' ');
            if (controle != null)
            {
                //Confirmando Fator
                double fator = -1;
                double fracao = -1;
                //Ponto == 1 e vrigula == 0
                if (controle[1].Contains(","))
                {
                    fator = double.Parse(controle[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    fator = double.Parse(controle[1], System.Globalization.CultureInfo.InvariantCulture);
                }
                if (fator > -1 && controle.Length > 1)
                {

                    if (controle[2].Contains(","))
                    {
                        fracao = double.Parse(controle[2].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        fracao = double.Parse(controle[2].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (fracao > -1)
                {
                    this.vFracaoUNT = fracao;
                    this.vFatorUNT = fator;
                    montando = "@UNT " + fator.ToString().Replace(",", ".") + ";" + fracao.ToString().Replace(",", ".");
                }
            }

            this._linhaUNT = montando;
        }

        private void DesmontarPRD(string strPRD)
        {
            string montando = string.Empty;
            string strPRDLimpo = strPRD;
            
            //Removendo os espacos duplos.....
            while (strPRDLimpo.Contains("  "))
            {
                strPRDLimpo = strPRDLimpo.Replace("  ", " ");
            }
            if (strPRDLimpo.Contains("\""))
            {
                strPRDLimpo = strPRDLimpo.Replace("\"", "");
            }
            string[] controle = strPRDLimpo.Split(' ');
            if (controle != null)
            {
                _codigoCor = controle[1];
            }
            montando = strPRDLimpo;

            this._linhaPRD = montando;
        }

        public bool Validar()
        {
            bool retorno =
                (!string.IsNullOrEmpty(_linhaUNT)
                && !string.IsNullOrEmpty(_linhaFRM));

            string[] vShots = _linhaFRM.Split(new char[] { ',' });
            if (vShots == null || vShots.Length < 2)
            {
                retorno = false;
            }

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
                ListCorrespondencia _lCorv = LISTA_CORRESPONDENCIA.Find(o => o.Correspondencia == Convert.ToInt32(_corr));
                if (_lCorv != null)
                {
                    CIRCUITO = _lCorv.Circuito;
                    double SHOT = double.Parse(vShots[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    
                    if (this.defaultUnit == 0)
                    {
                        double QUANTIDADE = (this.vFatorUNT / this.vFracaoUNT) * SHOT;
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

        public Dictionary<int, double> GetCheckFormula()
        {
            string[] vShots = _linhaCheckFormula.Split(new char[] { ',' });

            Dictionary<int, double> qtdes =
              new Dictionary<int, double>();

            for (int index = 0; index <= vShots.GetUpperBound(0); index += 2)
            {
                int CIRCUITO = 0;
                string _codProd = vShots[index];
                ListCorrespondencia _lCorv = LISTA_CORRESPONDENCIA.Find(o => o.Correspondencia == Convert.ToInt32(_codProd));
                if (_lCorv != null)
                {
                    CIRCUITO = _lCorv.Circuito;
                    double SHOT = double.Parse(vShots[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    double QUANTIDADE = 0.462 * SHOT;
                    qtdes.Add(CIRCUITO, QUANTIDADE);
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