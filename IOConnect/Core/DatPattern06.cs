using Percolore.Core;
using Percolore.Core.Logging;
using System.Globalization;
using System.Reflection;

namespace Percolore.IOConnect.Core
{
	public class DatPattern06 : IDat
    {
        private bool UTILIZAR_CORRESPONDENCIA;
        private List<ListCorrespondencia> LISTA_CORRESPONDENCIA;

        private string _linhaUNT;
        private string _linhaCAN;
        private string _linhaFRM;
        private string _linhaBAS;
        private string _linhaCLR;
        private string _linhaAllFRM;
        private string _linhaSV;
        private Util.ObjectParametros _parametro = null;
        private List<Util.ObjectBasDat06> listBasDat06 = null;

        private double vFracaoUNT = 0;
        private double vFatorUNT = 0;
        private double vEmbdisp = 0;

        private double vVolumeBas = 0;
        private int vCircuitoBas = -1;

        private double vEmbref = -1;

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
        //Gambiarra para solucionar este formato de Dat, o padrão seria implementar lá na Percolore.Core, mas como lá não tem como identificar as configurações necessárias para este dat
        //.....tive que implementar aqui
        public DatPattern06(string conteudo, int basePosicaoCircuito, bool utilizarCorrespondencia, List<ListCorrespondencia> listaCorrespondencia) /*Dictionary<int, int> listaCorrespondencia)*/
        {
            UTILIZAR_CORRESPONDENCIA = utilizarCorrespondencia;
            LISTA_CORRESPONDENCIA = listaCorrespondencia;
            try
            {
                this._parametro  = Util.ObjectParametros.Load();
                this.listBasDat06 = Util.ObjectBasDat06.List();
                string trabalho = conteudo.Replace("\n", "");
                string[] linhas = trabalho.Split('\r');
                if(linhas != null && linhas.Length > 0)
                {
                    foreach (string strLinha in linhas)
                    {
                        if (strLinha.Contains(this._parametro.Dat_06_BAS_Pref))
                        {
                            DesmontaBAS(strLinha.Replace("\n", ""));
                        }
                        else if (strLinha.Contains(this._parametro.Dat_06_CAN_Pref))
                        {
                            DesmontaCAN(strLinha.Replace("\n", ""));
                        }
                        else if (strLinha.Contains(this._parametro.Dat_06_FRM_Pref))
                        {
                            DesmontarFRM(strLinha.Replace("\n", ""));
                        }
                        else if (strLinha.Contains(this._parametro.Dat_06_UNT_Pref))
                        {
                            DesmontarUNT(strLinha.Replace("\n", ""));
                        }
                        else if (strLinha.Contains("@CLR"))
                        {
                            this._linhaCLR = strLinha;
                        }
                        else if (strLinha.Contains("@SV"))
                        {
                            DesmontarSV(strLinha.Replace("\n", ""));
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
			this._codigoCor = this._linhaCAN + ";" + this._linhaCLR.Replace("|", "/") + ";" + _linhaBAS + ";" + this._linhaAllFRM + ";"+ this._linhaUNT + ";";
            if(this._linhaSV != String.Empty)
            {
                this._codigoCor += this._linhaSV + ";";
                AssemblyInfo info = new AssemblyInfo(Assembly.GetExecutingAssembly());
                this._codigoCor += "IO " + info.AssemblyComercialVersion + ";";
            }
        }

        private void DesmontarFRM(string strFRM)
        {
            string montando = string.Empty;
            string strfrmLimpo = strFRM;
            try
            {
                //Removendo os espacos duplos.....
                while (strfrmLimpo.Contains("  "))
                {
                    strfrmLimpo = strfrmLimpo.Replace("  ", " ");
                }
                string[] controle = strfrmLimpo.Split(' ');
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    if (controle[0].Equals(this._parametro.Dat_06_FRM_Pref))
                    {
                        string trabalho = controle[1];
                        if (trabalho.Contains("\""))
                        {
                            trabalho = trabalho.Substring(trabalho.IndexOf("\"") + 1);
                            if(trabalho.Contains("\""))
                            {
                                trabalho = trabalho.Substring(0, trabalho.IndexOf("\""));
                                this.vEmbref = double.Parse(controle[2].Replace(",", "."), CultureInfo.InvariantCulture);
                                montando = trabalho;
                                this._linhaAllFRM = strfrmLimpo;
                            }
                        }

                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			this._linhaFRM = montando;

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
                string[] controle = strbasLimpo.Split(' ');
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    if (controle[0].Equals(this._parametro.Dat_06_BAS_Pref))
                    {   
                       
                        if(controle[1].Contains("\""))
                        {
                            string str_B = controle[1].Replace("\"", "");
                            foreach(Util.ObjectBasDat06  objD06 in  this.listBasDat06)
                            {
                                if(objD06.Name.ToUpper().Equals(str_B))
                                {
                                    this.vVolumeBas = objD06.Volume;
                                    this.vCircuitoBas = objD06.Circuito;
                                    montando = strbasLimpo;
                                }
                            }
                        }
                    }
                }
            }
            catch
            (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
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
                string[] controle = strcanLimpo.Split(' ');
                if (controle != null)
                {
                    //Confirmando Prefixo nos parametros
                    if (controle[0].Equals(this._parametro.Dat_06_CAN_Pref))
                    {
                        bool achou = false;
                        double embdisp = -1;
                        for (int i = 1; !achou && i < controle.Length;i++ )
                        {
                            if (controle[i] != null && controle[i].Length > 0)
                            {
                                //Ponto == 1 e vrigula == 0
                                if (this._parametro.Dat_06_CAN_1_IsPonto == 1 && !controle[i].Contains(","))
                                {
                                    embdisp = double.Parse(controle[i].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                    achou = true;
                                }
                                else if (this._parametro.Dat_06_CAN_1_IsPonto == 0 && !controle[i].Contains("."))
                                {
                                    embdisp = double.Parse(controle[i].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                                    achou = true;
                                }
                            }
                        }
                        if(embdisp >=0)
                        {
                            this.vEmbdisp = embdisp;
                            montando = strCAN;
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
			this._linhaCAN = montando;
        }

        private void DesmontarUNT(string strUNT)
        {
            string montando = string.Empty;
            string struntLimpo = strUNT;
            try
            {
                //Removendo os espacos duplos.....
                while(struntLimpo.Contains("  "))
                {
                    struntLimpo = struntLimpo.Replace("  ", " ");
                }
                string[] controle = struntLimpo.Split(' ');
                if(controle!= null)
                {
                    //Confirmando Prefixo nos parametros
                    if(controle[0].Equals(this._parametro.Dat_06_UNT_Pref))
                    {
                        //Confirmando Fator
                        double fator = -1;
                        double fracao = -1;
                        //Ponto == 1 e vrigula == 0
                        if (this._parametro.Dat_06_UNT_1_IsPonto == 1 && !controle[1].Contains(","))
                        {
                            fator = double.Parse(controle[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if(this._parametro.Dat_06_UNT_1_IsPonto == 0 && !controle[1].Contains("."))
                        {
                            fator = double.Parse(controle[1].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if(fator > -1)
                        {
                            //Ponto == 1 e vrigula == 0
                            if (this._parametro.Dat_06_UNT_2_IsPonto == 1 && !controle[2].Contains(","))
                            {
                                fracao = double.Parse(controle[2].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else if (this._parametro.Dat_06_UNT_2_IsPonto == 0 && !controle[2].Contains("."))
                            {
                                fracao = double.Parse(controle[2].Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        if(fracao > -1)
                        {
                            this.vFracaoUNT = fracao;
                            this.vFatorUNT = fator;
                            montando = "@UNT " + fator.ToString().Replace(",", ".") + "," + fracao.ToString().Replace(",", ".");
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
			this._linhaUNT = montando;
        }

        private void DesmontarSV(string strSV)
        {
            string montando = string.Empty;
            string strsvLimpo = strSV;
            try
            {
                //Removendo os espacos duplos.....
                while (strsvLimpo.Contains("  "))
                {
                    strsvLimpo = strsvLimpo.Replace("  ", " ");
                }
                if(strsvLimpo.Contains("\""))
                {
                    while (strsvLimpo.Contains("\""))
                    {
                        strsvLimpo = strsvLimpo.Replace("\"", "");
                    }
                }
                string[] controle = strsvLimpo.Split(' ');
                if (controle != null)
                {
                    if (controle[0].Equals("@SV"))
                    {
                        for (int i = 1; i < controle.Length; i++)
                        {
                            if (montando == string.Empty)
                            {
                                if (controle[i] == "Rainbow")
                                {
                                    montando = "@SV RB";
                                }
                                else if(controle[i] == "Colour-Smith")
                                {
                                    montando = "@SV CS";
                                }
                                else
                                {
                                    montando = "@SV " + controle[i];
                                }
                            }
                            else
                            {
                                montando += " " + controle[i];
                            }
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
			this._linhaSV = montando;
        }

        public bool Validar()
        {
            bool retorno =
                (!string.IsNullOrEmpty(_linhaUNT)
                && !string.IsNullOrEmpty(_linhaCAN)
                && !string.IsNullOrEmpty(_linhaFRM));
            if(retorno && this._parametro.Dat_06_BAS_Habilitado == 1)
            {
                retorno = !string.IsNullOrEmpty(_linhaBAS);
            }

            try
            {
                string[] vShots = _linhaFRM.Split(new char[] { this._parametro.Dat_06_FRM_SEP[0] });
                if(vShots == null || vShots.Length < 2)
                {
                    retorno = false;
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public Dictionary<int, double> GetQuantidades()
        {
            string[] vShots = _linhaFRM.Split(new char[] { this._parametro.Dat_06_FRM_SEP[0] });

            Dictionary<int, double> qtdes =
              new Dictionary<int, double>();

            for (int index = 0; index <= vShots.GetUpperBound(0); index += 2)
            {
                int CIRCUITO = 0;
                int _codProd = Convert.ToInt32(vShots[index].Replace("\"", ""));
                ListCorrespondencia _lCorv = LISTA_CORRESPONDENCIA.Find(o => o.Correspondencia == _codProd);
                if (_lCorv != null)
                {
                    CIRCUITO = _lCorv.Circuito;
                    double SHOT = double.Parse(vShots[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    
                    double QUANTIDADE = (this.vFatorUNT / this.vFracaoUNT) * (this.vEmbdisp / this.vEmbref) * SHOT ;
                    qtdes.Add(CIRCUITO, QUANTIDADE);
                }
            }
            if (this._parametro.Dat_06_BAS_Habilitado == 1)
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