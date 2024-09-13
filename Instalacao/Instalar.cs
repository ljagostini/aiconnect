using Percolore.Core;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Persistence.Xml;

namespace Percolore.Instalacao
{
	public class Instalador
    {
        private string defaultDirName;
        private string pathDatabase;
        public bool createXmlFromAccess = false;

        #region Métodos    

        public Instalador(string defaultDirName)
        {
            this.defaultDirName = defaultDirName;
            this.pathDatabase = string.Empty;
            

            string[] listDB =
            {
                "Percolore_1.6.accdb",
                "Percolore_1.5.accdb",
                "Percolore_1.4.accdb",
                "Percolore_1.3.accdb",
                "Percolore.accdb"
            };

            // Recupera arquivos com extensão .accdb do diretório do cliente
            FileInfo[] filesExistentes = new DirectoryInfo(
                this.defaultDirName).GetFiles("*.accdb", SearchOption.TopDirectoryOnly);

            foreach (string dbFile in listDB)
            {
                FileInfo file = filesExistentes.FirstOrDefault(f => f.Name == dbFile);
                if (file != null)
                {
                    this.pathDatabase = Path.Combine(this.defaultDirName, dbFile);
                    break;
                }
            }
            
        }

        public void AccessToXml()
        {
            this.createXmlFromAccess = true;
            if (!File.Exists(this.pathDatabase))
            {
                VerifyCalibragem();
                this.createXmlFromAccess = false;
                return;
                //throw new FileNotFoundException("Não foi encontrada nenhuma base de dados válida.");
            }

            List<Colorante> colorantes = new List<Colorante>();

            #region xml: define colorantes

            Colorante xmlColorante = new Colorante();
            xmlColorante.Circuito = 1;
            xmlColorante.Nome = "G";
            xmlColorante.MassaEspecifica = 1.2325;
            xmlColorante.Correspondencia = 1;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 2;
            xmlColorante.Nome = "B";
            xmlColorante.MassaEspecifica = 1.3252;
            xmlColorante.Correspondencia = 2;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 3;
            xmlColorante.Nome = "C";
            xmlColorante.MassaEspecifica = 1.7722;
            xmlColorante.Correspondencia = 3;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 4;
            xmlColorante.Nome = "D";
            xmlColorante.MassaEspecifica = 1.3454;
            xmlColorante.Correspondencia = 4;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 5;
            xmlColorante.Nome = "E";
            xmlColorante.MassaEspecifica = 1.3846;
            xmlColorante.Correspondencia = 5;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 6;
            xmlColorante.Nome = "F";
            xmlColorante.MassaEspecifica = 2.1056;
            xmlColorante.Correspondencia = 6;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 7;
            xmlColorante.Nome = "J";
            xmlColorante.MassaEspecifica = 1.365;
            xmlColorante.Correspondencia = 7;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 8;
            xmlColorante.Nome = "W";
            xmlColorante.MassaEspecifica = 1.9871;
            xmlColorante.Correspondencia = 8;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 9;
            xmlColorante.Nome = "L";
            xmlColorante.MassaEspecifica = 1.3249;
            xmlColorante.Correspondencia = 9;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 10;
            xmlColorante.Nome = "M";
            xmlColorante.MassaEspecifica = 1.2996;
            xmlColorante.Correspondencia = 10;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 11;
            xmlColorante.Nome = "Z";
            xmlColorante.MassaEspecifica = 1.26;
            xmlColorante.Correspondencia = 11;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 12;
            xmlColorante.Nome = "V";
            xmlColorante.MassaEspecifica = 1.2507;
            xmlColorante.Correspondencia = 12;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 13;
            xmlColorante.Nome = "13X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 13;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 14;
            xmlColorante.Nome = "14X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 14;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 15;
            xmlColorante.Nome = "15X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 15;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 16;
            xmlColorante.Nome = "16X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 16;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            #endregion

            //xml: Persiste colorantes
            Colorante.Persist(colorantes);

          
            //Access: Obdjeto de consultas à base de dados
            Queries queries = new Queries(pathDatabase);

            //access: Recupera parametros
            DbParametro dbParametro = queries.GetParametro();

            //access: Recupera pigmentos
            List<DbPigmento> pigmentos = queries.GetPigmentos();
            
            //xml: Atualiza estrutura do xml e recupera parâmetros
            Parametros.UpdateStructure();
            Parametros xmlParametro = Parametros.Load();

            #region xml: define parâmetros

            //Pigmento da posição 01 selecionado aleatoriamente para valores globais
            DbPigmento pigmento01 =
                pigmentos.FirstOrDefault(p => p.Posicao == 1);

            //Global

            xmlParametro.PortaModbus = dbParametro.ComPort;
            xmlParametro.SomarPulsoReverso = true;
            xmlParametro.Velocidade = pigmento01.Velocidade;
            xmlParametro.Aceleracao = pigmento01.Velocidade;
            xmlParametro.DelayReverso = pigmento01.ReverseDelay;
            xmlParametro.PulsoReverso = pigmento01.ReverseSteps;

            //Funcionamento software
            xmlParametro.HabilitarTecladoVirtual = true;
            xmlParametro.HabilitarDispensaSequencial = false;
            xmlParametro.HabilitarFormulaPersonalizada = false;

            //Unidade medida
            xmlParametro.HabilitarMililitro = true;
            xmlParametro.HabilitarOnca = true;
            xmlParametro.HabilitarShot = false;
            xmlParametro.ValorShot = 0.154;
            xmlParametro.HabilitarGrama = false;

            //Dat
            xmlParametro.PathMonitoramentoDAT =
                Path.Combine(this.defaultDirName, "Formula", "Formula.dat");
            xmlParametro.PathRepositorioDAT =
                 Path.Combine(Parametros.PathDiretorioSistema, "dat_old");
            xmlParametro.PadraoConteudoDAT = (int)DatPattern.Padrao03;
            xmlParametro.BasePosicaoCircuitoDAT = 1;
            xmlParametro.UtilizarCorrespondenciaDAT = false;

            //Purga
            xmlParametro.ControlarExecucaoPurga = true;
            xmlParametro.PrazoExecucaoPurga = new TimeSpan(12, 0, 0);
            xmlParametro.VelocidadePurga = xmlParametro.Velocidade / 2;
            xmlParametro.AceleracaoPurga = xmlParametro.VelocidadePurga;
            xmlParametro.VolumePurga = 1;
            xmlParametro.DelayPurga = 500;

            //Nível de colorante
            xmlParametro.ControlarNivel = false;
            xmlParametro.VolumeMinimo = 200;
            xmlParametro.VolumeMaximo = 2300;

            //Inicialização de circuitos
            xmlParametro.IniPulsoInicial = 1;
            xmlParametro.IniPulsoLimite = 50;
            xmlParametro.IniVariacaoPulso = 1;
            xmlParametro.IniStepVariacao = 0.5;
            xmlParametro.IniVelocidade = xmlParametro.Velocidade / 2;
            xmlParametro.IniAceleracao = xmlParametro.IniVelocidade;
            xmlParametro.IniMovimentoReverso = true;
            xmlParametro.InicializarCircuitosPurga = false;

            //Monit Circuitos
            xmlParametro.MonitVelocidade = 833;
            xmlParametro.MonitAceleracao = 500;
            xmlParametro.MonitDelay = 500;
            xmlParametro.MonitTimerDelay = 60;
            xmlParametro.MonitTimerDelayIni = 60;
            xmlParametro.MonitMovimentoReverso = false;
            xmlParametro.QtdeMonitCircuitoGrupo = 4;
            xmlParametro.DesabilitarInterfaceMonitCircuito = true;
            xmlParametro.DesabilitarProcessoMonitCircuito = true;
            xmlParametro.MonitPulsos = 833;



            //Log
            xmlParametro.PathLogProcessoDispensa =
                Path.Combine(this.defaultDirName, "log", "AD-D8.log");
            xmlParametro.PathLogControleDispensa =
                Path.Combine(Parametros.PathDiretorioSistema, "ctrldsp.log");

            //Placa 1.1
            xmlParametro.IdDispositivo = (int)Dispositivo.Placa_1;
            xmlParametro.HabilitarTesteRecipiente = false;
            xmlParametro.HabilitarDispensaSequencial = true;
            xmlParametro.InicializarCircuitosPurga = false;

            #endregion

            //xml: Persiste parâmetros
            Parametros.Persist(xmlParametro);

            #region xml: define calibragem

            foreach (DbPigmento pigmento in pigmentos)
            {
                Calibragem calibragem = new Calibragem();
                calibragem.Motor = pigmento.Posicao;
                calibragem.UltimoPulsoReverso = xmlParametro.PulsoReverso;
                calibragem.Valores = new List<ValoresVO>();

                ValoresVO v100 = new ValoresVO();
                v100.Volume = 100;
                v100.Velocidade = pigmento.Velocidade;
                v100.Delay = pigmento01.ReverseDelay;
                v100.PulsoHorario = 100 * pigmento.FatorEscalaCorrente;
                v100.PulsoReverso = 50;                
                
                calibragem.Valores.Add(v100);

                ValoresVO v10 = new ValoresVO();
                v10.Volume = 10;
                v10.Velocidade = v100.Velocidade;
                v10.Delay = v100.Delay;
                v10.PulsoHorario = v100.PulsoHorario / 10;
                v10.PulsoReverso = 50;
                calibragem.Valores.Add(v10);

                ValoresVO v1 = new ValoresVO();
                v1.Volume = 1;
                v1.Velocidade = v100.Velocidade;
                v1.Delay = v100.Delay;
                v1.PulsoHorario = v100.PulsoHorario / 100;
                v1.PulsoReverso = 50;
                calibragem.Valores.Add(v1);

                ValoresVO v0616 = new ValoresVO();
                v0616.Volume = 0.616;
                v0616.Velocidade = (int)(0.65 * (double)v100.Velocidade);
                v0616.Delay = v100.Delay;
                v0616.PulsoHorario = (int)(v100.PulsoHorario / 162.33766);
                v0616.PulsoReverso = 50;
                calibragem.Valores.Add(v0616);

                ValoresVO v0308 = new ValoresVO();
                v0308.Volume = 0.308;
                v0308.Velocidade = (int)(0.5 * (double)v100.Velocidade);
                v0308.Delay = v100.Delay;
                v0308.PulsoHorario = (int)(v100.PulsoHorario / 324.67532);
                v0308.PulsoReverso = 50;
                calibragem.Valores.Add(v0308);

                ValoresVO v0154 = new ValoresVO();
                v0154.Volume = 0.154;
                v0154.Velocidade = (int)(0.4 * (double)v100.Velocidade);
                v0154.Delay = v100.Delay;
                v0154.PulsoHorario = (int)(v100.PulsoHorario / 649.35064);
                v0154.PulsoReverso = 50;
                calibragem.Valores.Add(v0154);

                ValoresVO v0077 = new ValoresVO();
                v0077.Volume = 0.077;
                v0077.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v0077.Delay = v100.Delay / 2;
                v0077.PulsoHorario = (int)(v100.PulsoHorario / 1298.7012);
                v0077.PulsoReverso = 50;

                calibragem.Valores.Add(v0077);

                ValoresVO v00385 = new ValoresVO();
                v00385.Volume = 0.0385;
                v00385.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v00385.Delay = v100.Delay / 2;
                v00385.PulsoHorario = (int)(v100.PulsoHorario / 2597.4025);
                v00385.PulsoReverso = 50;
                calibragem.Valores.Add(v00385);

                ValoresVO v01925 = new ValoresVO();
                v01925.Volume = 0.1925;
                v01925.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v01925.Delay = v100.Delay / 2;
                v01925.PulsoHorario = (int)(v100.PulsoHorario / 2597.4025);
                v01925.PulsoReverso = 50;
                calibragem.Valores.Add(v01925);

                ValoresVO v009625 = new ValoresVO();
                v009625.Volume = 0.009625;
                v009625.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v009625.Delay = v100.Delay / 2;
                v009625.PulsoHorario = (int)(v100.PulsoHorario / 2597.4025);
                v009625.PulsoReverso = 50;
                calibragem.Valores.Add(v009625);

                ValoresVO v0048125 = new ValoresVO();
                v0048125.Volume = 0.0048125;
                v0048125.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v0048125.Delay = v100.Delay / 2;
                v0048125.PulsoHorario = (int)(v100.PulsoHorario / 2597.4025);
                v0048125.PulsoReverso = 50;
                calibragem.Valores.Add(v0048125);

                ValoresVO v00240625 = new ValoresVO();
                v00240625.Volume = 0.00240625;
                v00240625.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v00240625.Delay = v100.Delay / 2;
                v00240625.PulsoHorario = (int)(v100.PulsoHorario / 2597.4025);
                v00240625.PulsoReverso = 50;
                calibragem.Valores.Add(v00240625);

                ValoresVO v001203125 = new ValoresVO();
                v001203125.Volume = 0.001203125;
                v001203125.Velocidade = (int)(0.2 * (double)v100.Velocidade);
                v001203125.Delay = v100.Delay / 2;
                v001203125.PulsoHorario = (int)(v100.PulsoHorario / 2597.4025);
                v001203125.PulsoReverso = 50;
                calibragem.Valores.Add(v001203125);


                Calibragem.Update(calibragem);
            }

            #endregion
        }

        public void AtualizarRegistro()
        {
            string pathManutencao  = Path.Combine(this.defaultDirName, "manutencao.txt");
            if (File.Exists(pathManutencao))
            {
                string[] linhas = File.ReadAllLines(
                    Path.Combine(this.defaultDirName, "manutencao.txt"));

                string machineName = Environment.MachineName;
                foreach (string linha in linhas)
                {
                    if (linha.Contains(machineName))
                    {
                        string[] split = linha.Split(new Char[] { ';' });
                        DateTimeOffset instalacao = DateTimeOffset.Parse(split[0]);
                        DateTimeOffset manutencao = DateTimeOffset.Parse(split[2]);

                        using (IOConnectRegistry icnt = new IOConnectRegistry())
                        {
                            icnt.SetDataInstalacao(instalacao.ToUnixTimeSeconds());
                        }

                        using (PercoloreRegistry perc = new PercoloreRegistry())
                        {
                            perc.SetValidadeManutencao(manutencao.ToUnixTimeSeconds());
                        }
                    }
                }
            }
            else
            {
                DateTimeOffset instalacao = DateTimeOffset.Now;
                DateTimeOffset manutencao = DateTimeOffset.Now;

                using (IOConnectRegistry icnt = new IOConnectRegistry())
                {
                    icnt.SetDataInstalacao(instalacao.ToUnixTimeSeconds());
                }

                using (PercoloreRegistry perc = new PercoloreRegistry())
                {
                    perc.SetValidadeManutencao(manutencao.ToUnixTimeSeconds());
                }
            }
        }

        public void VerifyCalibragem()
        {
            try
            {
                Calibragem[] ca = new Calibragem[16];
                int ultimopulsRev = 0;
                for (int i = 1; i <= 16; i++)
                {
                    try
                    {
                        Calibragem c = Calibragem.Load(i);
                        ca[i - 1] = c;
                        ultimopulsRev = c.UltimoPulsoReverso;
                    }
                    catch
                    { }
                }
                List<Calibragem> lCal = new List<Calibragem>();

                for (int i = 1; i <= 16; i++)
                {
                    string[] volumeT = { "0.01925", "0.009625", "0.0048125", "0.00240625", "0.001203125" };
                    for (int j = 0; j < volumeT.Length; j++)
                    {
                        string volume = volumeT[j];
                        if (ca[i - 1] != null)
                        {
                            bool achou = false;
                            foreach (ValoresVO vVo in ca[i - 1].Valores)
                            {
                                string strVol = vVo.Volume.ToString();
                                if (strVol.Replace(",", ".") == volume)
                                {
                                    achou = true;
                                }
                            }
                            if (!achou)
                            {

                                Calibragem ccB = lCal.Find(o => o.Motor == i);
                                if (ccB == null)
                                {
                                    ccB = new Calibragem();
                                    ccB.Motor = i;
                                    ccB.UltimoPulsoReverso = ultimopulsRev;
                                    ValoresVO vOLoc = Parser(Convert.ToDouble(volume.Replace(".", ",")), ca[i - 1].Valores);
                                    ccB.Valores = new List<ValoresVO>();
                                    ccB.Valores.Add(vOLoc);
                                    lCal.Add(ccB);
                                }
                                else if (ccB != null)
                                {

                                    ValoresVO vOLoc = Parser(Convert.ToDouble(volume.Replace(".", ",")), ca[i - 1].Valores);
                                    ccB.Valores.Add(vOLoc);

                                }

                            }
                        }

                    }
                }


                foreach (Calibragem cb in lCal)
                {
                    Calibragem.UpdateStructInstall(cb);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Deleta arquivos criados manualmente e os arquivos que não poderiam ser deletados 
        /// pelo instalador pois seriam utilizados
        /// </summary>
        public void DeletarArquivos()
        {
            if (!Directory.Exists(this.defaultDirName))
            {
                return;
            }


            //Arquivos que devem ser deletados
            string[] deleteList = {
                ".accdb",
                "Dosadora",
                "DYMO.Common.dll",
                "DYMO.Label.Framework.dll",
                "DYMO.DLS.Runtime.dll",
                "Altera Barra Rolagem",
                "Log_Dispensa",
                ".label",
                "ModbusMixerFrontEnd-Portuguese",
                ".rtf",
                "MBFEConfig.bin"
            };

            //Recupera todos os arquivos do diretório de instalação
            FileInfo[] existingfiles = new DirectoryInfo(
                this.defaultDirName).GetFiles("*", SearchOption.TopDirectoryOnly);

            //Percolore lista de arquivos do diretório de instalação
            foreach (FileInfo file in existingfiles)
            {
                //Percore lista de arquivos com entradas da lista de exclusão
                foreach (string nameDelete in deleteList)
                {
                    /* Verifica se o arquivo do diretório de instalação possui,
                    em alguma parte do nome, a dexcrição especificada na lista de exclusão */
                    if (file.Name.Contains(nameDelete))
                    {
                        string path = Path.Combine(this.defaultDirName, file.Name);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
            }

        }

        public void UpgradeXML()
        {
           
            Calibragem[] ca = new Calibragem[16];
            int ultimopulsRev = 0;
            for (int i = 1; i <= 16; i++)
            {
                try
                {
                    Calibragem c = Calibragem.Load(i);
                    ca[i - 1] = c;
                    ultimopulsRev = c.UltimoPulsoReverso;
                }
                catch
                { }
            }
            for (int i = 1; i <= 16; i++)
            {
                try
                {
                    Calibragem.UpdateInstall(ca[i - 1]);
                 
                }
                catch
                { }
            }

            List<Calibragem> lCal = new List<Calibragem>();

           
            for (int i = 1; i <= 16; i++)
            {
                string[] volumeT = { "0.01925", "0.009625", "0.0048125", "0.00240625", "0.001203125" };
                for (int j = 0; j < volumeT.Length; j++)
                {
                    string volume = volumeT[j];
                    if (ca[i - 1] != null)
                    {
                        bool achou = false;
                        foreach (ValoresVO vVo in ca[i - 1].Valores)
                        {
                            string strVol = vVo.Volume.ToString();
                            if (strVol.Replace(",", ".") == volume)
                            {
                                achou = true;
                            }
                        }
                        if (!achou)
                        {
                            
                            Calibragem ccB = lCal.Find(o => o.Motor == i);
                            if(ccB == null)
                            {
                                ccB = new Calibragem();
                                ccB.Motor = i;
                                ccB.UltimoPulsoReverso = ultimopulsRev;
                                ValoresVO vOLoc = Parser(Convert.ToDouble(volume.Replace(".", ",")), ca[i - 1].Valores);
                                ccB.Valores = new List<ValoresVO>();
                                ccB.Valores.Add(vOLoc);
                                lCal.Add(ccB);
                            }
                            else if (ccB != null)
                            {
                                
                                ValoresVO vOLoc = Parser(Convert.ToDouble(volume.Replace(".", ",")), ca[i - 1].Valores);
                                ccB.Valores.Add(vOLoc);

                            }
                            
                        }
                    }

                }
            }


            foreach (Calibragem cb in lCal)
            {
                Calibragem.UpdateStructInstall(cb);
            }

            Parametros parametros = Parametros.Load();
           

            for (int i = 1; i <= 16; i++)
            {
                Calibragem cb = Calibragem.Load(i);
                foreach(ValoresVO vO in cb.Valores)
                {
                    double AG = parametros.Aceleracao;
                    double VG = parametros.Velocidade;
                    double V = vO.Velocidade;

                    double FATOR = (VG / V) * (VG / V);
                    double AC = (1 / FATOR) * VG;
                    int newAceleracao = int.Parse(Math.Round(AC).ToString());
                    vO.Aceleracao = newAceleracao;
                    vO.PulsoReverso = parametros.PulsoReverso;
                }
                Calibragem.Update(cb);
            }
        }

        public void UpgradeVersao19()
        {
            List<Colorante> colorantes = new List<Colorante>();

            #region xml: define colorantes

            Colorante xmlColorante = new Colorante();
            xmlColorante.Circuito = 17;
            xmlColorante.Nome = "17X";
            xmlColorante.MassaEspecifica = 1.2325;
            xmlColorante.Correspondencia = 17;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 18;
            xmlColorante.Nome = "18X";
            xmlColorante.MassaEspecifica = 1.3252;
            xmlColorante.Correspondencia = 18;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 19;
            xmlColorante.Nome = "19X";
            xmlColorante.MassaEspecifica = 1.7722;
            xmlColorante.Correspondencia = 19;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 20;
            xmlColorante.Nome = "20X";
            xmlColorante.MassaEspecifica = 1.3454;
            xmlColorante.Correspondencia = 20;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 21;
            xmlColorante.Nome = "21X";
            xmlColorante.MassaEspecifica = 1.3846;
            xmlColorante.Correspondencia = 21;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 22;
            xmlColorante.Nome = "22X";
            xmlColorante.MassaEspecifica = 2.1056;
            xmlColorante.Correspondencia = 22;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 23;
            xmlColorante.Nome = "23X";
            xmlColorante.MassaEspecifica = 1.365;
            xmlColorante.Correspondencia = 23;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 24;
            xmlColorante.Nome = "24X";
            xmlColorante.MassaEspecifica = 1.9871;
            xmlColorante.Correspondencia = 24;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 25;
            xmlColorante.Nome = "25X";
            xmlColorante.MassaEspecifica = 1.3249;
            xmlColorante.Correspondencia = 25;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 26;
            xmlColorante.Nome = "26X";
            xmlColorante.MassaEspecifica = 1.2996;
            xmlColorante.Correspondencia = 26;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 27;
            xmlColorante.Nome = "27X";
            xmlColorante.MassaEspecifica = 1.26;
            xmlColorante.Correspondencia = 27;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 28;
            xmlColorante.Nome = "28X";
            xmlColorante.MassaEspecifica = 1.2507;
            xmlColorante.Correspondencia = 28;
            xmlColorante.Habilitado = true;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 29;
            xmlColorante.Nome = "29X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 29;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 30;
            xmlColorante.Nome = "30X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 30;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 31;
            xmlColorante.Nome = "31X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 31;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            xmlColorante = new Colorante();
            xmlColorante.Circuito = 32;
            xmlColorante.Nome = "32X";
            xmlColorante.MassaEspecifica = 0;
            xmlColorante.Correspondencia = 32;
            xmlColorante.Habilitado = false;
            xmlColorante.Volume = 0;
            colorantes.Add(xmlColorante);

            #endregion

            //xml: Persiste colorantes
            Colorante.Persist(colorantes);
        }

        public void UpdatePar()
        {
            Parametros.UpdateStructureInstall();
        }


        private ValoresVO Parser(double volume, List<ValoresVO> listaValores)
        {
            ValoresVO superior = null;
            ValoresVO inferior = null;
            ValoresVO valores = null;

            int indexLimite = listaValores.Count - 1;
            for (int index = 0; index <= indexLimite; index++)
            {
                #region Encontra a faixa de volumes

                if (volume == listaValores[index].Volume)
                {
                    valores = listaValores[index];
                    break;
                }

                if (volume > listaValores[index].Volume)
                {
                    superior = (index == 0) ? null : listaValores[index - 1];
                    inferior = listaValores[index];
                    break;
                }

                //Se não encontrou limite inferior da faixa então o volume 
                //é menor que a última posição da lista
                if (inferior == null)
                    superior = listaValores[index];

                #endregion
            }

            if (valores == null)
            {
                valores = new ValoresVO();

                if (superior == null)
                {
                    #region [Valores acima do maior volume]

                    double P = volume * (inferior.PulsoHorario / inferior.Volume);
                    double PR = volume * (inferior.PulsoReverso / inferior.Volume);
                    valores.PulsoHorario = int.Parse(Math.Round(P).ToString());
                    // valores.PulsoReverso = int.Parse(Math.Round(PR).ToString());
                    valores.PulsoReverso = inferior.PulsoReverso;
                    valores.Velocidade = inferior.Velocidade;
                    valores.Delay = inferior.Delay;
                    valores.Aceleracao = inferior.Aceleracao;

                    #endregion
                }
                else if (inferior == null)
                {
                    #region [Valores abaixo do menor volume]

                    double P = volume * (superior.PulsoHorario / superior.Volume);
                    double PR = volume * (superior.PulsoReverso / superior.Volume);
                    valores.PulsoHorario = int.Parse(Math.Round(P).ToString());
                    //valores.PulsoReverso = int.Parse(Math.Round(PR).ToString());

                    valores.PulsoReverso = superior.PulsoReverso;
                    valores.Velocidade = superior.Velocidade;
                    valores.Delay = superior.Delay;
                    valores.Aceleracao = superior.Aceleracao;

                    #endregion
                }
                else
                {
                    #region[Valores entre maior e menor volume]

                    double VOL = volume;
                    double VOLS = superior.Volume;
                    double VOLI = inferior.Volume;
                    double PULS = superior.PulsoHorario;
                    double PULI = inferior.PulsoHorario;

                    double PULRS = superior.PulsoReverso;
                    double PULRI = inferior.PulsoReverso;

                    double VELS = superior.Velocidade;
                    double VELI = inferior.Velocidade;

                    double DELS = superior.Delay;
                    double DELI = inferior.Delay;

                    double ACCS = superior.Aceleracao;
                    double ACCI = inferior.Aceleracao;


                    //Taxa auxiliar    
                    double TX = 0;
                    if (VOL > 1)
                        TX = (VOL + VOLI) / (VOLS + VOLI);
                    else
                        TX = (VOL - VOLI) / (VOLS - VOLI);

                    //Velocidade
                    double V = VELI + (TX * (VELS - VELI));

                    //Delay
                    double D = DELI + (TX * (DELS - DELI));

                    TX = (VOL - VOLI) / (VOLS - VOLI);
                    if (TX < 0)
                    {
                        TX *= -1;
                    }

                    //Pulso Reverso
                    double PR = PULRI + (TX * (PULRS - PULRI));


                    //Pulsos
                    double P = PULI + (TX * (PULS - PULI));

                    double ac_ = ACCI + (TX * (ACCS - ACCI));


                    valores.PulsoHorario = int.Parse(Math.Round(P).ToString());
                    valores.PulsoReverso = int.Parse(Math.Round(PR).ToString());
                    valores.Velocidade = int.Parse(Math.Round(V).ToString());
                    valores.Delay = int.Parse(Math.Round(D).ToString());
                    valores.Aceleracao = int.Parse(Math.Round(ac_).ToString());

                    #endregion
                }
            }
           

            valores.Volume = volume;
            return valores;
        }


        #region Void ExecuteProcess [Comentado]

        //public static void Finalizar()
        //{
        //    // Deleta diretório com arquivos de instalação
        //    if (Directory.Exists(PathArquivos))
        //        Directory.Delete(PathArquivos, true);

        //    //Auto-deleta .exe do instalador
        //    ProcessStartInfo Info = new ProcessStartInfo();
        //    Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + Assembly.GetExecutingAssembly().Location;
        //    Info.WindowStyle = ProcessWindowStyle.Hidden;
        //    Info.CreateNoWindow = true;
        //    Info.FileName = "cmd.exe";
        //    Process.Start(Info);
        //}

        #endregion

        #endregion
    }
}