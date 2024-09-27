using Percolore.Core;
using System.Globalization;


namespace Percolore.IOConnect.Core
{
	public class DatPattern05 : IDat
    {
        private bool UTILIZAR_CORRESPONDENCIA;
        private List<ListCorrespondencia> LISTA_CORRESPONDENCIA;

        private string _linhaUNT;
        private string _linhaCAN;
        private string _linhaFRM;
        private string _linhaBAS;
        private string _linhaCLR;
        private string _linhaAllFRM = string.Empty;
        private Util.ObjectParametros _parametro = null;
        private List<Util.ObjectBasDat05> listBasDat05 = null;

        private double vFracaoUNT = 0;
        private double vFatorUNT = 0;
        private double vEmbdisp = 0;

        private double vVolumeBas = 0;
        private int vCircuitoBas = -1;

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

        public DatPattern05(string conteudo, int basePosicaoCircuito, bool utilizarCorrespondencia, List<ListCorrespondencia> listaCorrespondencia)
        {
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;
            
            this._parametro = Util.ObjectParametros.Load();
            this.listBasDat05 = Util.ObjectBasDat05.List();
            string trabalho = conteudo.Replace("\n", "");
            string[] linhas = trabalho.Split('\r');
            if (linhas != null && linhas.Length > 0)
            {
                foreach (string strLinha in linhas)
                {
                    if (strLinha.Contains(this._parametro.Dat_05_BAS_Pref))
                    {
                        DesmontaBAS(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains(this._parametro.Dat_05_CAN_Pref))
                    {
                        DesmontaCAN(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains(this._parametro.Dat_05_FRM_Pref))
                    {
                        DesmontarFRM(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains(this._parametro.Dat_05_UNT_Pref))
                    {
                        DesmontarUNT(strLinha.Replace("\n", ""));
                    }
                    else if (strLinha.Contains("CLR"))
                    {
                        this._linhaCLR = strLinha;
                    }
                }
            }

            this._codigoCor = this._linhaCAN + ";" + this._linhaCLR.Replace("|", "/") + ";" + _linhaBAS + ";" + this._linhaAllFRM + ";";
        }

        private void DesmontarFRM(string strFRM)
        {
            string montando = string.Empty;
            string strfrmLimpo = strFRM;
            
            //Removendo os espacos duplos.....
            while (strfrmLimpo.Contains("  "))
            {
                strfrmLimpo = strfrmLimpo.Replace("  ", " ");
            }
            strfrmLimpo = strfrmLimpo.Replace(" ", "");

            if (strfrmLimpo.Contains(_parametro.Dat_05_FRM_SEP))
            {
                string[] controle = strfrmLimpo.Split(_parametro.Dat_05_FRM_SEP[0]);
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    if (controle[0].Equals(this._parametro.Dat_05_FRM_Pref))
                    {
                        for (int i = 1; i < controle.Length; i++)
                        {
                            if (montando == string.Empty)
                            {
                                montando += controle[i];
                            }
                            else
                            {
                                montando += _parametro.Dat_05_FRM_SEP[0] + controle[i];
                            }

                        }
                    }
                }
            }

            this._linhaFRM = montando;
        }

        private void DesmontaBAS(string strBas)
        {
            string montando = string.Empty;
            string strbasLimpo = strBas;
            
            if (this._parametro.Dat_05_BAS_Habilitado == 1)
            {
                //Removendo os espacos duplos.....
                while (strbasLimpo.Contains("  "))
                {
                    strbasLimpo = strbasLimpo.Replace("  ", " ");
                }

                string[] controle = strbasLimpo.Split(' ');
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    if (controle[0].Equals(this._parametro.Dat_05_BAS_Pref))
                    {
                        if (controle[1].Contains("\""))
                        {
                            string str_B = controle[1].Replace("\"", "");
                            foreach (Util.ObjectBasDat05 objD05 in this.listBasDat05)
                            {
                                if (objD05.Name.ToUpper().Equals(str_B))
                                {
                                    this.vVolumeBas = objD05.Volume;
                                    this.vCircuitoBas = objD05.Circuito;
                                    montando = strbasLimpo;
                                }
                            }
                        }

                    }
                }
            }

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
            strcanLimpo = strcanLimpo.Replace(" ", "");
            string[] controle = strcanLimpo.Split('"');
            if (controle != null)
            {
                //Confirmando Prefixo nos parametros
                if (controle[0].Equals(this._parametro.Dat_05_CAN_Pref))
                {
                    bool achou = false;
                    double embdisp = -1;
                    for (int i = 2; !achou && i < controle.Length; i++)
                    {
                        if (controle[i] != null && controle[i].Length > 0)
                        {
                            //Ponto == 1 e vrigula == 0
                            if (this._parametro.Dat_05_CAN_1_IsPonto == 1 && !controle[i].Contains(","))
                            {
                                embdisp = double.Parse(controle[i].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                achou = true;
                            }
                            else if (this._parametro.Dat_05_CAN_1_IsPonto == 0 && !controle[i].Contains("."))
                            {
                                embdisp = double.Parse(controle[i].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
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
                struntLimpo.Replace("  ", " ");
            }
            string[] controle = struntLimpo.Split(' ');
            if (controle != null)
            {
                //Confirmando Prefixo nos parametros
                if (controle[0].Equals(this._parametro.Dat_05_UNT_Pref))
                {
                    //Confirmando Fator
                    double fator = -1;
                    double fracao = -1;
                    //Ponto == 1 e vrigula == 0
                    if (this._parametro.Dat_05_UNT_1_IsPonto == 1 && !controle[1].Contains(","))
                    {
                        fator = double.Parse(controle[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (this._parametro.Dat_05_UNT_1_IsPonto == 0 && !controle[1].Contains("."))
                    {
                        fator = double.Parse(controle[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (fator > -1)
                    {
                        //Ponto == 1 e vrigula == 0
                        if (this._parametro.Dat_05_UNT_2_IsPonto == 1 && !controle[2].Contains(","))
                        {
                            fracao = double.Parse(controle[2].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (this._parametro.Dat_05_UNT_2_IsPonto == 0 && !controle[2].Contains("."))
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
            }

            this._linhaUNT = montando;
        }

        public bool Validar()
        {
            bool retorno =
                (!string.IsNullOrEmpty(_linhaUNT)
                && !string.IsNullOrEmpty(_linhaCAN)
                && !string.IsNullOrEmpty(_linhaFRM));
            if (retorno && this._parametro.Dat_05_BAS_Habilitado == 1)
            {
                retorno = !string.IsNullOrEmpty(_linhaBAS);
            }

            string[] vShots = _linhaFRM.Split(new char[] { this._parametro.Dat_05_FRM_SEP[0] });
            if (vShots == null || vShots.Length < 2)
            {
                retorno = false;
            }

            return retorno;
        }

        public Dictionary<int, double> GetQuantidades()
        {
            string[] vShots = _linhaFRM.Split(new char[] { this._parametro.Dat_05_FRM_SEP[0] });

            Dictionary<int, double> qtdes =
              new Dictionary<int, double>();

            for (int index = 0; index <= vShots.GetUpperBound(0); index += 2)
            {
                int CIRCUITO = 0;
                string _codProd = vShots[index];
                ListCorrespondencia _lCorv = LISTA_CORRESPONDENCIA.Find(o => o.CodigoProduto == _codProd);
                if (_lCorv != null)
                {
                    CIRCUITO = _lCorv.Circuito;
                    double SHOT = double.Parse(vShots[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    
                    double QUANTIDADE = 0.462 * SHOT;
                    qtdes.Add(CIRCUITO, QUANTIDADE);
                }

            }
            if (this._parametro.Dat_05_BAS_Habilitado == 1)
            {
                ListCorrespondencia _lCorvB = LISTA_CORRESPONDENCIA.Find(o => o.Circuito == this.vCircuitoBas);
                if (_lCorvB != null)
                {
                    int CIRCUITO = _lCorvB.Circuito;
                    double QUANTIDADE = this.vEmbdisp * this.vVolumeBas;
                    qtdes.Add(CIRCUITO, QUANTIDADE);
                }
            }

            return qtdes;
        }
    }
}