using Newtonsoft.Json;
using Percolore.Core;
using Percolore.Core.AccessControl;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Util;
using Percolore.IOConnect.Negocio;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace Percolore.IOConnect
{
	public partial class fPainelControle : Form
    {
        NotifyIcon _notifyIcon;
        ContextMenuStrip _contextMenu;
        Util.ObjectParametros _parametros = null;
        List<Util.ObjectColorante> _colorantes = null;

        private bool isMonitCircuitos = false;

        private bool isTempoIniMonit = false;
        private int timerDelayMonit = 0;
        private int timerIniDelayMonit = 0;
        private bool isThread = false;
        private bool menu = false;
        private DateTime dtMonitoramentoCircuitos = DateTime.Now;
        private DateTime dtMonitoramentoFilaDat = DateTime.Now;
        private DateTime dtMonitoramentoFilaBaseDados = DateTime.Now;

        private int timerMonitFilaDat = 5;

        private bool isHDProducao = false;
        private bool isProgProducao = false;
        private bool isProgProducaoOnLine = false;
        private bool transmitiuProgProducao = false;
        private TimeSpan tsProgProducao = new TimeSpan(22, 00, 00);        
        private DateTime dtMonitProducao = DateTime.Now;
        private int timerDelayMonitProducao = 0;
        private bool isMonitProducao = false;

        private Negocio.Producao Prod = null;

        /*
         private Modbus.USBClass usb = new Modbus.USBClass();
         private System.Collections.Generic.List<Modbus.USBClass.DeviceProperties> ListOfUSBDeviceProperties;
         */

        // public MethodInvoker mtinvoke =  new MethodInvoker(MonitoramentoEvent);
        // this.Invoke(new MethodInvoker(function));
        //private int countTimerDelayMonit = 0;

        string CLICK_PURGA = Path.Combine(Environment.CurrentDirectory, "click.purgar");

        string CLICK_PURGA_INDIVIDUAL = Path.Combine(Environment.CurrentDirectory, "click.purgarindividual");

        string CLICK_GERENCIAR_VOLUME = Path.Combine(Environment.CurrentDirectory, "click.gerenciarnivel");

        string CLICK_SINC_FORMULA = Path.Combine(Environment.CurrentDirectory, "click.SincFormula");

        string CLICK_Notificacao = Path.Combine(Environment.CurrentDirectory, "click.Notificacao");

        string CLICK_UPLOAD_FORMULA = Path.Combine(Environment.CurrentDirectory, "UpFormula.json");

        string CLICK_PROGRESS = Path.Combine(Environment.CurrentDirectory, "click.PROGRESS");

        string CLICK_Precisao = Path.Combine(Environment.CurrentDirectory, "accurracy.json");

        string CLICK_Precisao_Up = Path.Combine(Environment.CurrentDirectory, "accurracyUp.json");

        string CLICK_CLOSED = Path.Combine(Environment.CurrentDirectory, "click.closed");

        string CLICK_LOGBD = Path.Combine(Environment.CurrentDirectory, "click.logBD");

        //TcpClient tcpcliente = new TcpClient();

        private List<Util.ObjectRecircular> _listaRecircular = new List<Util.ObjectRecircular>();
        DateTime dtRecircular = DateTime.Now;
        DateTime dtRecircularPurga = DateTime.Now.AddDays(-1);

        private List<Util.ObjectRecircular> _listaRecircularAuto = new List<Util.ObjectRecircular>();
        DateTime dtRecircularAuto = DateTime.Now;
        private int qtdTentativasRecirculaAUto = 0;

        private bool isControlePlacaMovAlerta = false;
        private bool isControleEsponja = false;
        private DateTime dtControleEsponja = DateTime.Now;

        private DateTime dtControleMaquinaLigada = DateTime.Now;
        private bool machine_turned_on = false;
        private DateTime? dtControlePlacaMovAlerta = null;

        private string VersaoSoftware = "";
        private fLogComunication fLog = null;
        private string pathUDCP = "";

        private bool isHabLimpBicos = false;
        private DateTime dtControleLimpBicos = DateTime.Now;
        private DateTime dtControleLimpBicosPurga = DateTime.Now.AddDays(-1);


        private DateTime dtMonitEventos = DateTime.Now;
        private int timerDelayMonitEventos = 0;
        private bool isMonitEventos = false;

        private DateTime dtMonitBkpCalibragem = DateTime.Now.AddMinutes(-59);
        private int timerDelayMonitBkpCalibragem = 0;
        private bool isMonitBkpCalibragem = false;

        private bool isHDEventos = false;
        private bool isProgEventos = false;
        private bool isProgEventosOnLine = false;
        private TimeSpan tsProgEventos = new TimeSpan(22, 00, 00);
        private bool transmitiuProgEventos = false;
        private int condSendTCP_Prod_Evt = 0;
        private bool isNotifyMouseUp = false;


        private DateTime dtControleMaquinaOnLine = DateTime.Now;
        private bool machine_OnLine = false;

        private DateTime dtFailEvt = DateTime.Now.AddMinutes(-20);


        private DateTime dtExecutadoLimpBicos = DateTime.Now;
        private List<Util.ObjectLimpBicos> listLimpBicosConfig = new List<Util.ObjectLimpBicos>();
        public fPainelControle()
        {  
            //Parâmetros que serão utilizados no monitoramento
            _parametros = Util.ObjectParametros.Load();
            this.isControlePlacaMovAlerta = getControlePlacaMov();
            this.isControleEsponja = _parametros.HabilitarIdentificacaoCopo;
            this.isHabLimpBicos = _parametros.HabLimpBicos;
            this.dtControlePlacaMovAlerta = DateTime.Now;
            _colorantes = Util.ObjectColorante.List();

            //Cria ícone para acesso ao menú de opções na bandeja do relógio
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _notifyIcon.MouseUp += _notifyIcon_MouseUp;

            //_notifyIcon.Click += new System.EventHandler(Notify_Click);
            AssemblyInfo info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            VersaoSoftware = info.AssemblyComercialVersion;
            /*
           if(VersaoSoftware.Contains("."))
           {
               string[] arrVersion = VersaoSoftware.Split('.');
               if (arrVersion.Length > 1)
               {
                   VersaoSoftware = arrVersion[0] + ".";
                   for (int i = 1; i < arrVersion.Length -1; i++)
                   {
                       if (arrVersion[i].Length <= 1)
                       {
                           VersaoSoftware += "0";
                       }                        
                       VersaoSoftware += arrVersion[i] + ".";
                   }
                   if(arrVersion[arrVersion.Length - 1].Length<=1)
                   {
                       VersaoSoftware += "0";
                   }
                   VersaoSoftware += arrVersion[arrVersion.Length - 1];
               }
           }
           */
            #region Limpeza Bicos
            this.dtExecutadoLimpBicos = Util.ObjectLimpBicos.LoadExecutado();
            this.listLimpBicosConfig = Util.ObjectLimpBicos.List();

            #endregion
            CreateMenuDinamicoIdioma();

            #region adicionar novo controle de monitoramento dos circuitos

            //transformar de minutos para segundos
            this.timerDelayMonit = _parametros.MonitTimerDelay * 60;
            this.timerIniDelayMonit = _parametros.MonitTimerDelayIni * 60;
            this.isMonitCircuitos = !_parametros.DesabilitarProcessoMonitCircuito;
            Constants.countTimerDelayMonit = 0;
            #endregion

            #region estrutura para producao
            this.Prod = new Negocio.Producao();
            /*
                OnLine
                1HS
                2HS
                6HS
                12HS
                18HS
                Prog 22HS
                HD
            */

            this.transmitiuProgProducao = false;
            this.isHDProducao = false;
            this.dtMonitProducao = DateTime.Now;
            if (_parametros.TipoProducao.Contains("Prog"))
            {
                this.isProgProducao = true;
                string tpProd = _parametros.TipoProducao.Replace("Prog", "").Replace(" ", "").Replace("HS", "");
                int hour = Convert.ToInt32(tpProd);
                this.tsProgProducao = new TimeSpan(hour, 0, 0);
            }
            else
            {
                if (_parametros.TipoProducao.Contains("OnLine"))
                {
                    this.isProgProducaoOnLine = true;
                    this.isProgProducao = false;
                    this.timerDelayMonitProducao = 0;
                }
                else if (_parametros.TipoProducao.Contains("HD"))
                {
                    this.isHDProducao = true;
                    this.timerDelayMonitProducao = 1;
                }
                else if (_parametros.TipoProducao.Contains("HS"))
                {
                    string tpProd = _parametros.TipoProducao.Replace("HS", "");
                    this.timerDelayMonitProducao = Convert.ToInt32(tpProd) * 60;
                }
            }


            this.isMonitProducao = !_parametros.DesabilitaMonitProcessoProducao;
            
            this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
            this._listaRecircularAuto = Util.ObjectRecircular.List().Where(o => o.Habilitado && o.isAuto).ToList();


            this.isMonitEventos = !_parametros.DesabilitaMonitSyncToken;
            this.timerDelayMonitEventos = 10;

            this.isMonitBkpCalibragem = !_parametros.DesabilitaMonitSyncBkpCalibragem;
            this.timerDelayMonitBkpCalibragem = 60;

            this.isHDEventos = false;
            this.dtMonitEventos = DateTime.Now;
            if (_parametros.TipoEventos.Contains("Prog"))
            {
                this.isProgEventos = true;
                string tpProd = _parametros.TipoEventos.Replace("Prog", "").Replace(" ", "").Replace("HS", "");
                int hour = Convert.ToInt32(tpProd);
                this.tsProgEventos = new TimeSpan(hour, 0, 0);
            }
            else
            {
                if (_parametros.TipoEventos.Contains("OnLine"))
                {
                    this.isProgEventosOnLine = true;
                    this.isProgEventos = false;
                    this.timerDelayMonitEventos = 0;
                }
                else if (_parametros.TipoEventos.Contains("HD"))
                {
                    this.isHDEventos = true;
                    this.timerDelayMonitEventos = 1;
                }
                else if (_parametros.TipoEventos.Contains("HS"))
                {
                    string tpProd = _parametros.TipoEventos.Replace("HS", "");
                    this.timerDelayMonitEventos = Convert.ToInt32(tpProd) * 60;
                }
            }
            #endregion

            #region Recirculacao
            if (_parametros.HabilitarRecirculacao)
            {
                this.dtRecircular = DateTime.Now.Subtract(new TimeSpan(_parametros.DelayMonitRecirculacao - 1, 59, 0));
            }
            #endregion

            #region identify click
            if(this._parametros.PathMonitoramentoDAT.Contains("DosadoraPercolore_zhm69scv2n72e") || this._parametros.PathMonitoramentoDAT.Contains("ColourSmith_njjhsg65ezkby"))
            {
                string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                string strPath = "";
                for(int i = 0; i < arrayStr.Length - 1; i++)
                {
                    strPath += arrayStr[i] + Path.DirectorySeparatorChar;
                }
                //CLICK_PURGA_INDIVIDUAL = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Local\\Packages\\DosadoraPercolore_zhm69scv2n72e\\LocalState\\click.purgarindividual");
                CLICK_PURGA = strPath + "click.purgar";               
                CLICK_PURGA_INDIVIDUAL = strPath + "click.purgarindividual";
                CLICK_GERENCIAR_VOLUME = strPath + "click.gerenciarnivel";
                CLICK_SINC_FORMULA = strPath + "click.SincFormula";
                CLICK_Notificacao = strPath + "click.Notificacao";
                CLICK_UPLOAD_FORMULA = strPath + "UpFormula.json";
                CLICK_PROGRESS = strPath + "click.PROGRESS";
                CLICK_Precisao = strPath + "accurracy.json";
                CLICK_Precisao_Up = strPath + "accurracyUp.json";
                CLICK_CLOSED = strPath + "click.closed";
                CLICK_LOGBD = strPath + "click.logBD";
            }
            if (File.Exists(this.CLICK_CLOSED))
            {
                File.Delete(this.CLICK_CLOSED);
            }             
            #endregion

        }

        private void initVerifApp()
        {
            try
            {
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Placa_2:
                        {
                            this.dtControleMaquinaLigada = DateTime.Now.AddMinutes(-6);
                            this.dtControleMaquinaOnLine = DateTime.Now.AddMinutes(-56);
                            getMaquinaLigada();
                            break;
                        }
                    case Dispositivo.Simulador:
                        {
                            this.dtControleMaquinaLigada = DateTime.Now.AddMinutes(-11);
                            this.dtControleMaquinaOnLine = DateTime.Now.AddMinutes(-59);
                            getMaquinaLigada();
                            break;
                        }
                    default:
                        {
                            this.machine_turned_on = true;
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void CreateMenuDinamicoIdioma(bool createEvt = true)
        {
            #region Cria menu de contexto para acesso as funcionalidades


            _notifyIcon.Visible = false;
            _contextMenu = new ContextMenuStrip();
            ToolStripMenuItem item;

            //Imagem padrão para possibilitar defiição de largura do item de menu
            Image image = Imagem.Base64ToImage(
                "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgAQMAAABJtOi3AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuOWwzfk4AAAADUExURf///6fEG8gAAAABdFJOUwBA5thmAAAAC0lEQVQIHWMY5AAAAKAAAZearVIAAAAASUVORK5CYII=");

            string mConfig = Negocio.IdiomaResxExtensao.PainelControle_Menu_Configuracoes;
            string mPurgInd = Negocio.IdiomaResxExtensao.PainelControle_Menu_PurgaIndividual;
            string mPurg = Negocio.IdiomaResxExtensao.PainelControle_Menu_Purga;
            string mformPers = Negocio.IdiomaResxExtensao.PainelControle_Menu_FormulasPersonalizadas;
            string mNivCol = Negocio.IdiomaResxExtensao.PainelControle_Menu_NivelColorante;
            string mTrein = Negocio.IdiomaResxExtensao.PainelControle_Menu_Treinamento;
            string msobre = string.Format( Negocio.IdiomaResxExtensao.PainelControle_Menu_Sobre, VersaoSoftware);
            string mSair = Negocio.IdiomaResxExtensao.PainelControle_Menu_Sair;
            string mnotificacao = Negocio.IdiomaResxExtensao.PainelControele_Notificacao;
            string mAutomaticDisp = Negocio.IdiomaResxExtensao.PainelControle_AutoTesterProtocol;
            string mRecircularProdutos = Negocio.IdiomaResxExtensao.PainelControle_Menu_RecircularProdutos;
            string mManutencaoPlacaMov = Negocio.IdiomaResxExtensao.PainelControle_Menu_PlacaMov;
            string mLimpezaBico = Negocio.IdiomaResxExtensao.PainelControle_Menu_LimpezaBico;
            string mSerialLog = "Log Serial";
            string mConectPlaca = Negocio.IdiomaResxExtensao.PainelControle_Menu_ConnectPlaca + (this.machine_turned_on ? Negocio.IdiomaResxExtensao.PainelControle_Menu_ConnectPlaca_On : Negocio.IdiomaResxExtensao.PainelControle_Menu_ConnectPlaca_Off);

            try
            {
                if (!_parametros.DesabilitarMonitoramentoFilaDAT)
                {
                    string pathFile = _parametros.PathMonitoramentoFilaDAT;
                    if (pathFile.Contains(@"\"))
                    {
                        string[] arrPath = pathFile.Split((char)92);
                        if (arrPath.Length > 0)
                        {
                            string file = arrPath[arrPath.Length - 1];
                            if (file.Contains("."))
                            {
                                string[] arrFile = file.Split('.');
                                int indexof = pathFile.IndexOf(file);
                                string path = pathFile.Substring(0, indexof);
                                string[] fileEntries = Directory.GetFiles(path, "*." + arrFile[arrFile.Length - 1]);
                                if (fileEntries != null && fileEntries.Length > 0)
                                {
                                    item = new ToolStripMenuItem(
                                        fileEntries.Length + " " + mnotificacao,
                                        Imagem.Get_Notificacao(),
                                       MenuNotificacao,
                                       OpcaoMenu.GerenciadorFilaDat.ToString());
                                    item.ImageScaling = ToolStripItemImageScaling.None;
                                    _contextMenu.Items.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
            }

            bool isUDCP = false;
            if ((DatPattern)_parametros.PadraoConteudoDAT == DatPattern.PadraoUDCP)
            {
                isUDCP = true;
            }

            //Sobre
            item = new ToolStripMenuItem(
                //Properties.IOConnect.PainelControle_Menu_Sobre,
                msobre,
                Imagem.Get_Sobre_32x32(),
                MenuSobre,
                OpcaoMenu.Sobre.ToString());
            item.ImageScaling = ToolStripItemImageScaling.None;
            _contextMenu.Items.Add(item);

            //Connect Placa
            item = new ToolStripMenuItem(
                //Properties.IOConnect.PainelControle_Menu_Sobre,
                mConectPlaca,
                Imagem.Get_Sobre_32x32(),
                MenuConnectPlaca,
                OpcaoMenu.ConnectPlaca.ToString());
            item.ImageScaling = ToolStripItemImageScaling.None;
            _contextMenu.Items.Add(item);


            if (!isUDCP)
            {
                //Treinamento
                item = new ToolStripMenuItem(
                    //Properties.IOConnect.PainelControle_Menu_Treinamento,
                    mTrein,
                    Imagem.Get_Treinamento_32x32(),
                    MenuTreinamento,
                    OpcaoMenu.Treinamento.ToString());
                item.ImageScaling = ToolStripItemImageScaling.None;
                _contextMenu.Items.Add(item);

                //Separador
                _contextMenu.Items.Add(new ToolStripSeparator());

                if (_parametros.ControlarNivel)
                {
                    //Nível de colorante
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_NivelColorante,
                        mNivCol,
                        Imagem.GetGerenciarNivel_32x32(),
                        MenuNivelColorante,
                        OpcaoMenu.GerenciarNivel.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }

                //Purga
                item = new ToolStripMenuItem(
                    //Properties.IOConnect.PainelControle_Menu_Purga,
                    mPurg,
                    Imagem.GetPurgar_32x32(),
                    MenuPurga,
                    OpcaoMenu.Purgar.ToString());
                item.ImageScaling = ToolStripItemImageScaling.None;
                _contextMenu.Items.Add(item);

                if (_parametros.HabilitarPurgaIndividual)
                {
                    //PurgaIndividual
                    item = new ToolStripMenuItem(
                    //Properties.IOConnect.PainelControle_Menu_PurgaIndividual,
                    mPurgInd,
                    Imagem.GetPurgar_32x32(),
                    MenuPurgaIndividual,
                    OpcaoMenu.PurgarIndividual.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }
                if (_parametros.HabilitarLogAutomateTesterProt)
                {
                    //Dosagem Automática
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_Configurações,
                        mAutomaticDisp,
                        Imagem.GetPurgar_32x32(),
                        MenuAutomaticDispensa,
                        OpcaoMenu.MenuAutomaticDispensa.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }

                if (_parametros.HabilitarRecirculacao)
                {
                    //Recirculação
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_Configurações,
                        mRecircularProdutos,
                        Imagem.GetPurgar_32x32(),
                        MenuRecircularProdutos,
                        OpcaoMenu.Recircular.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }

                if (_parametros.HabilitarFormulaPersonalizada)
                {
                    //Fórmulas personalizadas
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_FormulasPersonalizadas,
                        mformPers,
                        Imagem.GetFormulaPersonalizada_32x32(),
                        MenuFormulaPersonalizada,
                        OpcaoMenu.FormulaPersonalizada.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }

                //Manutenção Placa Mov
                //if (_parametros.IdDispositivo == 4)
                if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_3)
                {
                    item = new ToolStripMenuItem(
                        mManutencaoPlacaMov,
                        Imagem.GetConfigurar_32x32(),
                        MenuPlacaMov,
                        OpcaoMenu.PlacaMov.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);

                    item = new ToolStripMenuItem(
                       mLimpezaBico,
                       Imagem.GetPurgar_32x32(),
                       MenuLimpezaBico,
                       OpcaoMenu.PlacaMov.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);


                }

                if (Modbus.Constantes.bSerialLog)
                {
                    //Configurações
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_Configurações,
                        mSerialLog,
                        Imagem.GetConfigurar_32x32(),
                        MenuLogSerial,
                        OpcaoMenu.LogSerial.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);

                }
            }
            else
            {
                if (_parametros.HabilitarLogAutomateTesterProt)
                {
                    //Dosagem Automática
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_Configurações,
                        mAutomaticDisp,
                        Imagem.GetPurgar_32x32(),
                        MenuAutomaticDispensa,
                        OpcaoMenu.MenuAutomaticDispensa.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }

                if (_parametros.HabilitarRecirculacao)
                {
                    //Recirculação
                    item = new ToolStripMenuItem(
                        //Properties.IOConnect.PainelControle_Menu_Configurações,
                        mRecircularProdutos,
                        Imagem.GetPurgar_32x32(),
                        MenuRecircularProdutos,
                        OpcaoMenu.Recircular.ToString());
                    item.ImageScaling = ToolStripItemImageScaling.None;
                    _contextMenu.Items.Add(item);
                }
            }

            //Configurações
            item = new ToolStripMenuItem(
                //Properties.IOConnect.PainelControle_Menu_Configurações,
                mConfig,
                Imagem.GetConfigurar_32x32(),
                MenuConfig,
                OpcaoMenu.Configuracoes.ToString());
            item.ImageScaling = ToolStripItemImageScaling.None;
            _contextMenu.Items.Add(item);


            //Separador
            _contextMenu.Items.Add(new ToolStripSeparator());


            //Sair
            item = new ToolStripMenuItem(
            //Properties.IOConnect.PainelControle_Menu_Sair,
                mSair,
                Imagem.GetSair_32x32_02(),
               MenuSair,
               OpcaoMenu.Sair.ToString());
            item.ImageScaling = ToolStripItemImageScaling.None;
            _contextMenu.Items.Add(item);

            //Adiciona menu de contexto ao ícone da bandeja
            _notifyIcon.ContextMenuStrip = _contextMenu;

            _notifyIcon.ContextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(ToolStripDropDown1_Closed);


            if (createEvt)
            {

                _notifyIcon.BalloonTipClosed += new System.EventHandler(Notify_CloseTipClicked);
                _notifyIcon.BalloonTipClicked += new System.EventHandler(Notify_TipClicked);
            }
            _notifyIcon.Visible = true;

            #endregion
        }

        private void ToolStripDropDown1_Closed(Object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange)
            {
                this.dtMonitoramentoFilaDat = DateTime.Now.Subtract(new TimeSpan(0, 0, this.timerMonitFilaDat));
                this.isNotifyMouseUp = false;
            }

        }

        private void Notify_Click(object sender, EventArgs e)
        {
            //CreateMenuDinamicoIdioma(false);
        }

        private void Notify_CloseTipClicked(object sender, EventArgs e)
        {
            this.dtMonitoramentoFilaDat = DateTime.Now;
            this.isNotifyMouseUp = false;
        }

        private void Notify_TipClicked(object sender, EventArgs e)
        {
            ShowMenu(OpcaoMenu.GerenciadorFilaDat);
        }
               
        #region USB
        /*
        /// <summary>
        /// Try to connect to the device.
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        private bool USBTryMyDeviceConnection()
        {
            Nullable<UInt32> MI = null;

            if (this.usb.GetUSBDevice(uint.Parse("0403", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("6015", System.Globalization.NumberStyles.AllowHexSpecifier), ref ListOfUSBDeviceProperties, true, MI))
            {
                //My Device is attached
                //NumberOfFoundDevicesLabel.Text = "Number of found devices: " + ListOfUSBDeviceProperties.Count.ToString();
                //FoundDevicesComboBox.Items.Clear();
                //for (int i = 0; i < ListOfUSBDeviceProperties.Count; i++)
                //{
                //    FoundDevicesComboBox.Items.Add("Device " + i.ToString());
               /// }
                //FoundDevicesComboBox.Enabled = (ListOfUSBDeviceProperties.Count > 1);
                //FoundDevicesComboBox.SelectedIndex = 0;
                this.usb.InitializeFileConnection(ListOfUSBDeviceProperties[0].DevicePhysicalObjectName.ToLower());

                Modbus.USBConstant.connectUsb = true;
                return true;
            }
            else
            {
                
                return false;
            }
        }

        private void USBPort_USBDeviceAttached(object sender, Modbus.USBClass.USBDeviceEventArgs e)
        {
            if (!Modbus.USBConstant.connectUsb)
            {
                if (USBTryMyDeviceConnection())
                {
                    Modbus.USBConstant.connectUsb = true;
                }
            }
        }

        private void USBPort_USBDeviceRemoved(object sender, Modbus.USBClass.USBDeviceEventArgs e)
        {
            if (Modbus.USBConstant.connectUsb)
            {
                if (!usb.GetUSBDevice(uint.Parse("0403", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("6015", System.Globalization.NumberStyles.AllowHexSpecifier), ref ListOfUSBDeviceProperties, false))
                {
                    //My Device is removed
                    Modbus.USBConstant.connectUsb = false;

                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            bool IsHandled = false;

            this.usb.ProcessWindowsMessage(m.Msg, m.WParam, m.LParam, ref IsHandled);

            base.WndProc(ref m);
        }
        */

        #endregion
            
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Visible = false;
            ShowInTaskbar = false;

            //[Aplica controle de acesso aos menus]
            ControlarAcessoMenu();

                

            //[Executa monitoramento]
            ExecutarMonitoramento();

            /*
            #region connect USB
            ListOfUSBDeviceProperties = new List<Modbus.USBClass.DeviceProperties>();
            this.usb.USBDeviceAttached += new Modbus.USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            this.usb.USBDeviceRemoved += new Modbus.USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            this.usb.RegisterForDeviceChange(true, this.Handle);
            USBTryMyDeviceConnection();
            #endregion
            */
            try
            {
                this.Invoke(new MethodInvoker(initVerifApp));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        //Função para identificar o reinicio do Monitoramento dos circuitos
        void RessetarTempoMonitoramento()
        {
            Constants.countTimerDelayMonit = 0;
            this.dtMonitoramentoCircuitos = DateTime.Now;
        }

        //Função para identificar o reinicio do Monitoramento dos circuitos
        void IncrementarTempoMonitoramento()
        {
            Constants.countTimerDelayMonit++;
        }
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
        private int ConvertDoubletoInt(double valor)
        {
            int retorno = 0;
            string _str = valor.ToString().Replace(",", "");

            if (_str.Contains("."))
            {
                string[] strArr = _str.Split('.');
                _str = strArr[0];
            }
            int.TryParse(_str, out retorno);
            return retorno;
        }        
        private void MonitoramentoEvent()
        {
            try
            {
                bool isGerenciadorFilaData = false;
                int qtdFilaDat = 0;
                bool isProtocolUDCP = false;
                #region Processa arquivo click gerado pelo software dosadora [Compatibilidade]

                if (File.Exists(this.CLICK_PURGA))
                {
                    //PausarMonitoramento();
                    File.Delete(this.CLICK_PURGA);
                    if (!File.Exists(this.CLICK_PROGRESS))
                    {
                        File.Create(this.CLICK_PROGRESS).Close();
                    }
                    ShowMenu(OpcaoMenu.Purgar);
                    RessetarTempoMonitoramento();
                    this.menu = false;
                    return;
                }

                if (File.Exists(this.CLICK_PURGA_INDIVIDUAL))
                {
                    File.Delete(this.CLICK_PURGA_INDIVIDUAL);
                    if (!File.Exists(this.CLICK_PROGRESS))
                    {
                        File.Create(this.CLICK_PROGRESS).Close();
                    }
                    ShowMenu(OpcaoMenu.PurgarIndividual);
                    RessetarTempoMonitoramento();
                    this.menu = false;
                    return;
                }

                if (File.Exists(this.CLICK_Notificacao))
                {
                    File.Delete(this.CLICK_Notificacao);
                    ShowMenu(OpcaoMenu.GerenciadorFilaDat);
                    RessetarTempoMonitoramento();
                    this.menu = false;
                    return;
                }

                if (File.Exists(this.CLICK_GERENCIAR_VOLUME))
                {
                    //PausarMonitoramento();
                    File.Delete(this.CLICK_GERENCIAR_VOLUME);
                    if (!File.Exists(this.CLICK_PROGRESS))
                    {
                        File.Create(this.CLICK_PROGRESS).Close();
                    }
                    ShowMenu(OpcaoMenu.GerenciarNivel);
                    return;
                }

                if (File.Exists(this.CLICK_Precisao))
                {
                    ShowMenu(OpcaoMenu.Precisao);
                    this.menu = false;
                    return;
                }

                if (File.Exists(this.CLICK_CLOSED))
                {
                    File.Delete(this.CLICK_CLOSED);
                    ShowMenu(OpcaoMenu.SairClick);
                    this.menu = false;
                    return;
                }

                if (File.Exists(this.CLICK_LOGBD))
                {
                    ShowMenu(OpcaoMenu.LogBD);
                    File.Delete(this.CLICK_LOGBD);
                    this.menu = false;
                    return;
                }


                if (!_parametros.DesabilitaMonitSincFormula && File.Exists(this.CLICK_SINC_FORMULA))
                {
                    File.Delete(this.CLICK_SINC_FORMULA);
                    ShowMenu(OpcaoMenu.SincronismoFormula);
                    return;
                }

                if (!_parametros.DesabilitaMonitSincFormula && File.Exists(this.CLICK_UPLOAD_FORMULA))
                {

                    ShowMenu(OpcaoMenu.UploadFormula);
                    return;
                }

                if (!_parametros.DesabilitarMonitoramentoFilaDAT)
                {
                    try
                    {

                        string pathFile = _parametros.PathMonitoramentoFilaDAT;
                        if (pathFile.Contains(@"\"))
                        {
                            string[] arrPath = pathFile.Split((char)92);
                            if (arrPath.Length > 0)
                            {
                                //bool existFile = false;
                                string file = arrPath[arrPath.Length - 1];
                                if (file.Contains("."))
                                {
                                    string[] arrFile = file.Split('.');
                                    int indexof = pathFile.IndexOf(file);
                                    string path = pathFile.Substring(0, indexof);
                                    string[] fileEntries = Directory.GetFiles(path, "*." + arrFile[arrFile.Length - 1]);

                                    if (fileEntries != null && fileEntries.Length > 0)
                                    {
                                        qtdFilaDat = fileEntries.Length;
                                        TimeSpan? tDiffFilaDat = DateTime.Now.Subtract(this.dtMonitoramentoFilaDat);
                                        if (tDiffFilaDat != null && tDiffFilaDat.HasValue && tDiffFilaDat.Value.TotalSeconds > this.timerMonitFilaDat)
                                        {
                                            if (this.timerMonitFilaDat == 5)
                                            {
                                                this.timerMonitFilaDat = _parametros.DelayMonitoramentoFilaDAT * 60;
                                                if (this.timerMonitFilaDat < 60)
                                                {
                                                    this.timerMonitFilaDat = 60;
                                                }
                                            }
                                            isGerenciadorFilaData = true;
                                        }
                                        tDiffFilaDat = null;
                                    }
                                    if (_parametros.TipoBaseDados == "1")
                                    {
                                        TimeSpan? tDiffFilaBaseDados = DateTime.Now.Subtract(this.dtMonitoramentoFilaBaseDados);
                                        if (tDiffFilaBaseDados != null && tDiffFilaBaseDados.HasValue && tDiffFilaBaseDados.Value.TotalSeconds > 5)
                                        {
                                            VerificarBaseDados(fileEntries, path);
                                        }
                                        tDiffFilaBaseDados = null;
                                    }

                                }

                            }
                        }
                    }
					catch (Exception ex)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
					}
				}

                switch ((DatPattern)_parametros.PadraoConteudoDAT)
                {
                    case DatPattern.PadraoUDCP:
                        {
                            string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                            string _srt_udcp_busy = "";
                            for (int i = 0; i < arrayStr.Length - 1; i++)
                            {
                                _srt_udcp_busy += arrayStr[i] + Path.DirectorySeparatorChar;
                            }
                            if (File.Exists(Path.Combine(_srt_udcp_busy, "busy.flg")))
                            {
                                File.Delete(Path.Combine(_srt_udcp_busy, "busy.flg"));
                            }
                            break;
                        }
                }

                #endregion

                string conteudoDAT = string.Empty;


                if (!File.Exists(_parametros.PathMonitoramentoDAT))
                {
                    if (getRecirculacao())
                    {
                        return;
                    }
                  

                    if(getRecirculacaoAuto())
                    {
                        return;
                    }
                    

                    #region Placa Movimentacao Alerta
                    if (isControlePlacaMovAlerta && this.dtControlePlacaMovAlerta != null)
                    {
                        TimeSpan? tsPMAlerta = DateTime.Now.Subtract(this.dtControlePlacaMovAlerta.Value);
                        if (tsPMAlerta != null && tsPMAlerta.HasValue && ConvertDoubletoInt(tsPMAlerta.Value.TotalMinutes) > _parametros.DelayAlertaPlacaMov)
                        {
                            this.dtControlePlacaMovAlerta = DateTime.Now;
                            getMessagePlacaMov();
                            this.dtControlePlacaMovAlerta = DateTime.Now;
                        }
                        tsPMAlerta = null;
                    }
                    else if(this.isControleEsponja)
                    {
                        getMessageEsponja();
                    }
                    #endregion

                    #region Maquina Ligada
                    switch ((Dispositivo)_parametros.IdDispositivo)
                    {
                        case Dispositivo.Placa_2:
                            {
                                getMaquinaLigada();
                                break;
                            }
                        case Dispositivo.Simulador:
                            {
                                getMaquinaLigada();
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    #endregion

                    #region Limpeza Bicos
                    if(this.isHabLimpBicos)
                    {
                        getLimpezaBico();
                    }
                    #endregion

                    
                }
                //bool find_file_data = false;
                if (!File.Exists(_parametros.PathMonitoramentoDAT) && !isGerenciadorFilaData)
                {
                    bool isMoniCircLocal = false;
                    #region Monitoramento de circuitos
                    try
                    {
                        if (this.isMonitCircuitos)
                        {
                            //Aguarde o tempo inicial
                            if (!this.isTempoIniMonit)
                            {
                                if (Constants.countTimerDelayMonit > this.timerIniDelayMonit)
                                {
                                    this.isTempoIniMonit = true;
                                    RessetarTempoMonitoramento();
                                    this.dtMonitoramentoCircuitos = DateTime.Now;
                                }
                                IncrementarTempoMonitoramento();
                            }
                            else
                            {
                                int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitoramentoCircuitos).TotalSeconds;
                                if (tempoDelay > this.timerDelayMonit)
                                //if (Constants.countTimerDelayMonit > this.timerDelayMonit)
                                {
                                    isMoniCircLocal = true;
                                    // PausarMonitoramento();
                                    ShowMenu(OpcaoMenu.MonitCircuitos);
                                    //ExecutarMonitoramento();
                                }

                            }

                        }
                    }
					catch (Exception ex)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
					    Log.Logar(
                        TipoLog.Processo,
                        _parametros.PathLogProcessoDispensa,
                        Negocio.IdiomaResxExtensao.Log_Cod_29 + Negocio.IdiomaResxExtensao.LogProcesso_FalhaLeituraConteudoDat);

                        /*Se ocorrer algum erro de leitura, abandona a rotina para que 
                        * monitoramento seja reiniciado, a existência do dat seja novamente 
                        * verificada e uma nova tentativa de leitura seja efetuada */
                        //ExecutarMonitoramento();
                        this.menu = false;
                        return;
                    }
                    #endregion

                    if (!isMoniCircLocal)
                    {
                        #region Monitoramento de Produção e Eventos
                        switch(this.condSendTCP_Prod_Evt)
                        {
                            case 0:
                                {
                                    this.condSendTCP_Prod_Evt++;
                                    this.Invoke(new MethodInvoker(MonitProd));                                    
                                    break;
                                }
                            case 1:
                                {
                                    this.condSendTCP_Prod_Evt++;
                                    this.Invoke(new MethodInvoker(MonitEventos));
                                    break;
                                }
                            case 2:
                                {
                                    this.condSendTCP_Prod_Evt++;
                                    this.Invoke(new MethodInvoker(MonitBkpCalibragem));
                                    break;
                                }
                            case 3:
                                {
                                    this.condSendTCP_Prod_Evt++;
                                    this.Invoke(new MethodInvoker(GenerateEventoOnLine));
                                    break;
                                }
                            default:
                                {
                                    this.condSendTCP_Prod_Evt = 0;
                                    break;
                                }
                        }
                        
                        #endregion
                    }

                    this.menu = false;
                    //Se arquivo dat não for encontrado, abandona método para reinicar monitoramento
                    return;
                }
                else if (File.Exists(_parametros.PathMonitoramentoDAT))
                {
                    #region Arquivo dat encontrado
                    //RessetarTempoMonitoramento();
                    //Pausa monitoramento para que conteúdo seja processado
                    // PausarMonitoramento();


                    try
                    {
                        //Efetua tentativa de leitura do conteúdo do arquivo
                        conteudoDAT = File.ReadAllText(_parametros.PathMonitoramentoDAT, Encoding.GetEncoding("iso-8859-1"));
                        Log.Logar(
                            TipoLog.Processo,
                            _parametros.PathLogProcessoDispensa,
                            Negocio.IdiomaResxExtensao.LogProcesso_SucessoLeituraConteudoDat);
                    }
					catch (Exception ex)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
					    Log.Logar(
                            TipoLog.Processo,
                            _parametros.PathLogProcessoDispensa,
                            Negocio.IdiomaResxExtensao.Log_Cod_29 + Negocio.IdiomaResxExtensao.LogProcesso_FalhaLeituraConteudoDat);

                        /*Se ocorrer algum erro de leitura, abandona a rotina para que 
                            * monitoramento seja reiniciado, a existência do dat seja novamente 
                            * verificada e uma nova tentativa de leitura seja efetuada */
                        //ExecutarMonitoramento();
                        this.menu = false;
                        return;
                    }

                    #endregion
                }
                else if (isGerenciadorFilaData)
                {
                    this.dtMonitoramentoFilaDat = DateTime.Now;
                    this.menu = false;
                    //ShowMenu(OpcaoMenu.GerenciadorFilaDat);
                    //_notifyIcon.BalloonTipText =  "Você possui " + qtdFilaDat.ToString() + " fórmula para Dosar";
                    //_notifyIcon.BalloonTipTitle = "Fórmula para Dosar";

                    _notifyIcon.BalloonTipText = string.Format(Negocio.IdiomaResxExtensao.fNotificacaoFilaDat_lblDesc, qtdFilaDat.ToString());
                    _notifyIcon.BalloonTipTitle = Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_lblTitulo;
                    _notifyIcon.ShowBalloonTip(30);

                    return;
                }
                else
                {
                    this.menu = false;
                    return;

                }

                IDispenser dispenser = null;
                IDispenser dispenser2 = null;
                ModBusDispenser_P2 modBusDispenser_P2 = null;
                ModBusDispenserMover_P3 modBusDispenser_P3 = null;
                
                try
                {
                    bool insertBDSql = false;
                    string Dat_usado_tcp = "@dat 01";

                    //Parâmetros referentes ao arquivo dat
                    int PARAMETRO_BASE_POSICAO_CIRCUITO = _parametros.BasePosicaoCircuitoDAT;
                    bool UTILIZAR_CORRESPONDENCIA = _parametros.UtilizarCorrespondenciaDAT;

                    //Colorantes habilitados
                    List<Util.ObjectColorante> colorantesHabilitados = _colorantes.Where(w => w.Habilitado == true && w.Seguidor == -1).ToList();

                    Dictionary<int, int> LISTA_CORRESPONDENCIA = colorantesHabilitados.ToDictionary(t => t.Correspondencia, t => t.Circuito);
                    List<ListCorrespondencia> lCorr = new List<ListCorrespondencia>();
                    foreach (Util.ObjectColorante _col in colorantesHabilitados)
                    {
                        ListCorrespondencia _l = new ListCorrespondencia();
                        _l.Circuito = _col.Circuito;
                        _l.Correspondencia = _col.Correspondencia;
                        _l.CodigoProduto = _col.Nome;
                        lCorr.Add(_l);

                    }
                   

                    IDat DAT = null;
                    switch ((DatPattern)_parametros.PadraoConteudoDAT)
                    {
                        #region Padrão de dat configurado

                        case DatPattern.Padrao01:
                            {
                                Dat_usado_tcp = "@dat 01";
                                DAT = new DatPattern01(
                                    conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, LISTA_CORRESPONDENCIA);
                                //insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao02:
                            {
                                Dat_usado_tcp = "@dat 02";
                                DAT = new DatPattern02(
                                    conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, LISTA_CORRESPONDENCIA);
                                //insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao03:
                            {
                                Dat_usado_tcp = "@dat 03";
                                DAT = new DatPattern03(
                                    conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, LISTA_CORRESPONDENCIA);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao04:
                            {
                                Dat_usado_tcp = "@dat 04";
                                DAT = new DatPattern04(
                                    conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, LISTA_CORRESPONDENCIA);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao05:
                            {
                                Dat_usado_tcp = "@dat 05";
                                DAT = new Core.DatPattern05(
                                  conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao06:
                            {
                                Dat_usado_tcp = "@dat 06";
                                DAT = new Core.DatPattern06(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.PadraoUDCP:
                            {
                                Dat_usado_tcp = "@dat UDCP";
                                string npath_udcp = _parametros.PathMonitoramentoDAT;
                                string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                                this.pathUDCP = "";
                                for (int i = 0; i < arrayStr.Length - 1; i++)
                                {
                                    this.pathUDCP += arrayStr[i] + Path.DirectorySeparatorChar;
                                }
                                npath_udcp = npath_udcp.Replace(".sig", ".dat");
                                if (File.Exists(npath_udcp))
                                {
                                    if(File.Exists(_parametros.PathMonitoramentoDAT))
                                    {
                                        File.Delete(_parametros.PathMonitoramentoDAT);                                        
                                    }
                                    conteudoDAT = File.ReadAllText(npath_udcp, Encoding.GetEncoding("iso-8859-1"));

                                    if(this._parametros.CreateFileTmpUDCP == 1)
                                    {
                                        using(StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "cmd_" + this._parametros.ExtFileTmpUDCP +".dat")))
                                        {
                                            sw.Write(conteudoDAT);
                                            sw.Close();
                                        }
                                        if(File.Exists(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                        {
                                            File.Delete(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat"));
                                        }
                                    }
                                    Core.DatPatternUDCP dpUDCP = new Core.DatPatternUDCP(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                    File.Delete(npath_udcp);
                                    if (dpUDCP.isRunAndEnd)
                                    {
                                        if (dpUDCP.CheckFormula)
                                        {
                                            ExecuteCheckFormula(dpUDCP.GetCheckFormula());
                                            return;
                                        }
                                        else if (dpUDCP.ExtKey)
                                        {
                                            ExecuteExtKey(dpUDCP.getExtKey());
                                            return;
                                        }
                                        else
                                        {
                                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "busy.flg")))
                                            {
                                                sw.Write("0");
                                                sw.Close();
                                            }
                                            //using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "msg.dat")))
                                            //{
                                            //    sw.WriteLine("[Msg]");
                                            //    sw.WriteLine("Code = 1");
                                            //    sw.WriteLine("DlgType = 1");
                                            //    sw.WriteLine("Buttons = 4");
                                            //    sw.WriteLine("Msg = Click Ok to dispense");
                                            //    sw.Close();
                                            //}
                                            //using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "msg.sig")))
                                            //{
                                            //    sw.WriteLine("[Msg]");
                                            //    sw.WriteLine("Code = 1");
                                            //    sw.WriteLine("DlgType = 1");
                                            //    sw.WriteLine("Buttons = 4");
                                            //    sw.WriteLine("Msg = Click Ok to dispense");
                                            //    sw.Close();
                                            //}
                                            bool existReply = true;

                                            //for(int i = 0; !existReply && i < 30; i++)
                                            //{
                                            //    if (File.Exists(Path.Combine(this.pathUDCP, "reply.sig")))
                                            //    {
                                            //        existReply = true;
                                            //    }
                                            //    Thread.Sleep(1000);
                                            //}
                                            
                                            if (existReply)
                                            {                                              
                                                //File.Delete(Path.Combine(this.pathUDCP, "reply.sig"));
                                                //if (File.Exists(Path.Combine(this.pathUDCP, "reply.dat")))
                                                //{
                                                //    File.Delete(Path.Combine(this.pathUDCP, "reply.dat"));
                                                //}

                                                //Precisa implementar a lógica do retorno do Mix2Win reply.dat

                                                DAT = (IDat)dpUDCP;
                                                insertBDSql = true;
                                                isProtocolUDCP = true;
                                            }
                                            else
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {   
                                        return;
                                    }
                                }
                                else
                                {
                                    File.Delete(_parametros.PathMonitoramentoDAT);
                                    Thread.Sleep(100);
                                    return;
                                }
                                
                                break;
                            }
                        case DatPattern.Padrao07:
                            {
                                Dat_usado_tcp = "@dat 07";
                                DAT = new Core.DatPattern07(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.PadraoSinteplast:
                            {
                                Dat_usado_tcp = "@dat Sinteplast";
                                DAT = new Core.DatPatternSinteplast(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = false;
                                break;
                            }
                        case DatPattern.Padrao10:
                            {
                                Dat_usado_tcp = "@dat 10";
                                DAT = new Core.DatPattern10(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao11:
                            {
                                Dat_usado_tcp = "@dat 11";
                                DAT = new Core.DatPattern11(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = true;
                                break;
                            }
                        case DatPattern.Padrao12:
                            {
                                Dat_usado_tcp = "@dat 12";
                                DAT = new Core.DatPattern12(conteudoDAT, PARAMETRO_BASE_POSICAO_CIRCUITO, UTILIZAR_CORRESPONDENCIA, lCorr);
                                insertBDSql = true;
                                break;
                            }
                            #endregion
                    }


                    if (_parametros.ControlarExecucaoPurga
                        && Operar.TemPurgaPendente())
                    {
                        if (isProtocolUDCP)
                        {
                            if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                            {
                                File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_10 + Negocio.IdiomaResxExtensao.Global_PurgaPendente3);
                                sw.Close();
                            }
                            if (this._parametros.CreateFileTmpUDCP == 1)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_10 + Negocio.IdiomaResxExtensao.Global_PurgaPendente3);

                                    sw.Close();
                                }
                            }

                            if (this._parametros.DelayUDCP > 0)
                            {
                                Thread.Sleep(this._parametros.DelayUDCP * 1000);
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }
                        #region Controle de execução de purga

                        EncerrarOperacao(
                            descricaoDAT: Negocio.IdiomaResxExtensao.Log_Cod_10 + Negocio.IdiomaResxExtensao.Global_PurgaPendente3,
                            mensagemUsuario: Negocio.IdiomaResxExtensao.Global_PurgaPendente
                                            + Environment.NewLine +
                                            Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                        this.menu = false;

                        return;

                        #endregion
                    }

                    if (!DAT.Validar())
                    {
                        if(isProtocolUDCP)
                        {
                            if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                            {
                                File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_11 + Negocio.IdiomaResxExtensao.Global_FormatoInvalido);
                                sw.Close();
                            }
                            if (this._parametros.CreateFileTmpUDCP == 1)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_11 + Negocio.IdiomaResxExtensao.Global_FormatoInvalido);

                                    sw.Close();
                                }
                            }

                            if (this._parametros.DelayUDCP > 0)
                            {
                                Thread.Sleep(this._parametros.DelayUDCP * 1000);
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }
                        #region Valida conteúdo do arquivo

                        EncerrarOperacao(
                            descricaoDAT: Negocio.IdiomaResxExtensao.Global_FormatoInvalido,
                            log: new string[] {
                            Negocio.IdiomaResxExtensao.Log_Cod_11 + Negocio.IdiomaResxExtensao.Global_FormatoInvalido,
                            Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado},
                            mensagemUsuario:
                                Negocio.IdiomaResxExtensao.Global_FormatoInvalido
                                + Environment.NewLine
                                + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                        this.menu = false;

                        return;

                        #endregion
                    }

                    //[Recupera demanda com volume para cada circuito]
                    Dictionary<int, double> demanda = DAT.GetQuantidades();

                    #region Valida circuitos recuperados

                    foreach (KeyValuePair<int, double> item in demanda)
                    {
                        int circuito = item.Key;
                        Util.ObjectColorante colorante = _colorantes.SingleOrDefault(c => c.Circuito == circuito);

                        //Verifica se colorante existe e está habilitado
                        if ((colorante == null) || colorante.Habilitado == false)
                        {
                            EncerrarOperacao(
                                descricaoDAT: Negocio.IdiomaResxExtensao.Global_CircuitoColoranteInexistente,
                                log:
                                    Negocio.IdiomaResxExtensao.Global_CircuitoColoranteInexistente
                                    + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado,
                                mensagemUsuario:
                                    Negocio.IdiomaResxExtensao.Global_CircuitoColoranteInexistente
                                    + Environment.NewLine
                                    + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                            this.menu = false;

                            return;
                        }
                    }

                    #endregion

                    if (_parametros.ControlarNivel)
                    {
                        #region Verifica volume de colorante        

                        List<Util.ObjectColorante> excedentes = null;
                        if (!Operar.TemColoranteSuficiente(demanda, out excedentes))
                        {
                            #region Texto da pergunta

                            string texto = Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo + " ";

                            bool primeiro = true;
                            foreach (Util.ObjectColorante colorante in excedentes)
                            {
                                texto += (primeiro ? "" : ", ") + colorante.Nome;
                                primeiro = false;
                            }

                            texto += Environment.NewLine;
                            texto += Environment.NewLine;
                            texto += Negocio.IdiomaResxExtensao.Global_Confirmar_AbastecerDosadora;

                            #endregion

                            bool abastecer = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                            {
                                abastecer = m.ShowDialog(texto, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            }

                            if (abastecer)
                            {
                                using (fGerenciarNivel vcg = new fGerenciarNivel())
                                {
                                    vcg.ShowDialog();
                                }
                            }
                            else
                            {
                                string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Informacao_NivelColoranteInsuficiente;
                                string log =
                                    Negocio.IdiomaResxExtensao.Log_Cod_13 + Negocio.IdiomaResxExtensao.Global_Informacao_NivelColoranteInsuficiente
                                    + Negocio.IdiomaResxExtensao.Global_Informacao_ProcessoCancelado
                                    + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;
                                string mensagemUsuario =
                                    Negocio.IdiomaResxExtensao.Global_Informacao_ProcessoCancelado
                                    + Environment.NewLine
                                    + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;

                                EncerrarOperacao(descricaoDAT, log, mensagemUsuario);
                            }
                            this.menu = false;
                            return;
                        }

                        if(isProtocolUDCP)
                        {
                            List<Util.ObjectColorante> mycolorantesCN = _colorantes.Where(w => w.Habilitado == true && w.Seguidor == -1).ToList();
                            string log_nivel_colorante = "";
                            foreach (Util.ObjectColorante c in mycolorantesCN)
                                log_nivel_colorante += $"{c.Circuito },{Math.Round(c.Volume, 3)},";

                            log_nivel_colorante =
                                log_nivel_colorante.Remove(log_nivel_colorante.Length - 1);

                            Log.Logar(
                                TipoLog.Processo, _parametros.PathLogProcessoDispensa, log_nivel_colorante);
                        }
                        #endregion
                    }

                    DateTime dtInicioDosagem = DateTime.Now;

                    ValoresVO[] vValores = new ValoresVO[16];
                    ValoresVO[] vValores2 = new ValoresVO[16];

                    ValoresVO[] vValores_b = new ValoresVO[16];
                    ValoresVO[] vValores2_b = new ValoresVO[16];
                                        
                    List<ListColoranteSeguidor> lColorSeguidor_P1 = null;
                    List<ListColoranteSeguidor> lColorSeguidor_P2 = null;

                    bool isDosarBase_P1 = false;
                    bool isDosarBase_P2 = false;

                    bool _exist_Seg_P1 = false;
                    bool _exist_Seg_P2 = false;

                    //Define o modo de execução do dispenser
                    switch ((Dispositivo)_parametros.IdDispositivo)
                    {
                        case Dispositivo.Simulador:
                            {
                                //insertBDSql = false;
                                vValores = new ValoresVO[16];
                                vValores_b = new ValoresVO[16];

                                dispenser = new ModBusSimulador();
                                break;
                            }
                        case Dispositivo.Placa_1:
                            {
                                dispenser = new ModBusDispenser_P1();
                                break;
                            }
                        case Dispositivo.Placa_2:
                            {
                                modBusDispenser_P2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                dispenser = modBusDispenser_P2;
                                break;
                            }
                        case Dispositivo.Placa_3:
                            {
                                modBusDispenser_P3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                                break;
                            }
                        //case Dispositivo.Placa_4:
                        //    {
                        //        vValores = new ValoresVO[24];
                        //        vValores_b = new ValoresVO[24];
                        //        modBusDispenser_P4 = new ModBusDispenser_P4(_parametros.NomeDispositivo);
                        //        dispenser = modBusDispenser_P4;
                        //        break;
                        //    }
                        default:
                            {
                                dispenser = new ModBusSimulador();
                                break;
                            }
                    }
                    //Identificar segunda placa
                    switch ((Dispositivo2)_parametros.IdDispositivo2)
                    {
                        case Dispositivo2.Nenhum:
                            {
                                dispenser2 = null;
                                break;
                            }
                        case Dispositivo2.Simulador:
                            {
                                vValores2 = new ValoresVO[16];
                                vValores2_b = new ValoresVO[16];


                                dispenser2 = new ModBusSimulador();
                                break;
                            }
                        case Dispositivo2.Placa_2:
                            {
                                dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                                break;
                            }
                        //case Dispositivo2.Placa_4:
                        //    {
                        //        vValores2 = new ValoresVO[24];
                        //        vValores2_b = new ValoresVO[24];
                        //        dispenser2 = new ModBusDispenser_P4(_parametros.NomeDispositivo2);
                        //        break;
                        //    }
                        default:
                            {
                                dispenser2 = null;
                                break;
                            }
                    }

                    List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Seguidor > 0).ToList();


                    foreach (KeyValuePair<int, double> item in demanda)
                    {
                        #region Verifica dados de dispensa

                        int POSICAO_CIRCUITO = item.Key;
                        int dispositivo = 1;// colorantesHabilitados.Where(c => c.Correspondencia == POSICAO_CIRCUITO).Select(c => c.Dispositivo);
                        
                        foreach (Util.ObjectColorante col in colorantesHabilitados)
                        {
                            if (col.Circuito == POSICAO_CIRCUITO)
                            {
                                dispositivo = col.Dispositivo;
                        
                            }
                        }
                        
                        List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == POSICAO_CIRCUITO).ToList();
                        
                        if (dispositivo == 1)
                        {
                            if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                            {
                                _exist_Seg_P1 = true;
                            }
                            
                        }
                        else if (dispositivo == 2)
                        {
                            if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                            {
                                _exist_Seg_P2 = true;
                            }
                            
                        }

                        #endregion
                    }

                 

                    foreach (KeyValuePair<int, double> item in demanda)
                    {
                        #region Gera dados de dispensa

                        int POSICAO_CIRCUITO = item.Key;
                        Util.ObjectColorante _colorante = Util.ObjectColorante.Load(POSICAO_CIRCUITO);

                        int dispositivo = 1;// colorantesHabilitados.Where(c => c.Correspondencia == POSICAO_CIRCUITO).Select(c => c.Dispositivo);
                        bool isBase = false;
                        foreach (Util.ObjectColorante col in colorantesHabilitados)
                        {
                            if (col.Circuito == POSICAO_CIRCUITO)
                            {
                                dispositivo = col.Dispositivo;
                                isBase = col.IsBase;
                            }
                        }
                        double VOLUME = item.Value;
                        if(isBase && !_parametros.HabilitarDispensaSequencial && _parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                        {
                            VOLUME = item.Value / 2;
                        }

                        //[Recupera calibragem do circuito
                        Util.ObjectCalibragem c = Util.ObjectCalibragem.Load(POSICAO_CIRCUITO);
                        List<ValoresVO> calibragem = c.Valores;

                        //List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == POSICAO_CIRCUITO).ToList();
                        List<Util.ObjectColorante> ncol = new List<Util.ObjectColorante>();

                        if (isBase && !_parametros.HabilitarDispensaSequencial)
                        {
                            ncol = lCol.FindAll(o => o.Seguidor == POSICAO_CIRCUITO).ToList();
                        }
                        else
                        {
                            if (_colorante.IsBicoIndividual && VOLUME > _colorante.VolumeBicoIndividual)
                            {
                                ncol = lCol.FindAll(o => o.Seguidor == _colorante.Circuito).ToList();
                                if (ncol.Count > 0)
                                {
                                    VOLUME = VOLUME / (ncol.Count + 1);
                                }
                            }
                            else
                            {
                                if (!_colorante.IsBicoIndividual)
                                {
                                    ncol = lCol.FindAll(o => o.Seguidor == _colorante.Circuito).ToList();
                                    if (ncol.Count > 0)
                                    {
                                        VOLUME = VOLUME / ncol.Count;
                                    }
                                }
                            }
                        }


                        //[Define valores para dispensa]

                        ValoresVO valoresRetornados = new ValoresVO();
                        if (Dat_usado_tcp == "@dat 12")
                        {
                            valoresRetornados.PulsoHorario = int.Parse(Math.Round(VOLUME * 1000).ToString());
                            valoresRetornados.Volume = VOLUME;
                            valoresRetornados.MassaIdeal = 0;
                            valoresRetornados.MassaMedia = 0;
                            valoresRetornados.Aceleracao = 0;
                            valoresRetornados.Delay = 0;
                            valoresRetornados.DesvioMedio = 0;
                            valoresRetornados.PulsoReverso = 0;
                            valoresRetornados.Velocidade = 0;


                        }
                        else
                        {
                            valoresRetornados = Operar.Parser(VOLUME, calibragem, c.UltimoPulsoReverso);
                        }
                            
                        if (dispositivo == 1)
                        {          
                            if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                            {
                                int[] n_motor = new int[ncol.Count];
                                
                                for (int i = 0; i < ncol.Count; i++)
                                {
                                    n_motor[i] = (ncol[i].Circuito);                                    
                                }
                                if(lColorSeguidor_P1 == null)
                                {
                                    lColorSeguidor_P1 = new List<ListColoranteSeguidor>();
                                }
                                ListColoranteSeguidor m_col = new ListColoranteSeguidor();
                                m_col.Circuito = POSICAO_CIRCUITO;
                                m_col.isbase = isBase;
                                m_col.lCircuitoSeguidores = n_motor.ToList();
                                m_col.Qtd_Circuito = m_col.lCircuitoSeguidores.Count + 1;

                                m_col.myValoresPrincipal = valoresRetornados;
                                for (int i = 0; i < n_motor.Length; i++)
                                {
                                    Util.ObjectCalibragem c_col = Util.ObjectCalibragem.Load(n_motor[i]);
                                    List<ValoresVO> calibragem_col = c_col.Valores;
                                    ValoresVO valoresRetornados_col = new ValoresVO();
                                    if (Dat_usado_tcp == "@dat 12")
                                    {
                                        valoresRetornados_col.PulsoHorario = int.Parse(Math.Round(VOLUME * 1000).ToString());
                                        valoresRetornados_col.Volume = VOLUME;
                                        valoresRetornados_col.MassaIdeal = 0;
                                        valoresRetornados_col.MassaMedia = 0;
                                        valoresRetornados_col.Aceleracao = 0;
                                        valoresRetornados_col.Delay = 0;
                                        valoresRetornados_col.DesvioMedio = 0;
                                        valoresRetornados_col.PulsoReverso = 0;
                                        valoresRetornados_col.Velocidade = 0;
                                    }
                                    else
                                    {
                                        valoresRetornados_col = Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                    }
                                    
                                    Negocio.ListValoresVOCircuitos lvO = new Negocio.ListValoresVOCircuitos();
                                    lvO.circuito = n_motor[i];
                                    lvO.myValores = valoresRetornados_col;
                                    //m_col.myValores = valoresRetornados;
                                    m_col.myValores.Add(lvO);

                                }

                                //m_col.myValores = valoresRetornados;

                                lColorSeguidor_P1.Add(m_col);
                                if(isBase && (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases ||
                                    _parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes))
                                {
                                    isDosarBase_P1 = true;
                                }
                            }
                            else
                            {
                                if (_exist_Seg_P1 && !isBase)
                                {
                                    vValores[POSICAO_CIRCUITO - 1] = valoresRetornados;
                                }
                                else if (isBase && !_parametros.HabilitarDispensaSequencial &&
                                    (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases ||
                                    _parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes))
                                {
                                    if (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                    {
                                        vValores[POSICAO_CIRCUITO - 1] = valoresRetornados;
                                    }
                                    vValores_b[POSICAO_CIRCUITO - 1] = valoresRetornados;
                                    isDosarBase_P1 = true;
                                }
                                //else if(!_exist_Seg_P1)
                                //{
                                //    vValores[POSICAO_CIRCUITO - 1] = valoresRetornados;
                                //}
                                
                                else
                                {
                                    vValores[POSICAO_CIRCUITO - 1] = valoresRetornados;
                                }
                            }
                        }
                        else if (dispositivo == 2)
                        {
                            int index_v2_count = 16;                           

                            if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                            {
                                int[] n_motor = new int[ncol.Count];                               
                                for (int i = 0; i < ncol.Count; i++)
                                {
                                    n_motor[i] = (ncol[i].Circuito - index_v2_count);                                    
                                }
                                if (lColorSeguidor_P2 == null)
                                {
                                    lColorSeguidor_P2 = new List<ListColoranteSeguidor>();
                                }
                                ListColoranteSeguidor m_col = new ListColoranteSeguidor();
                                m_col.Circuito = POSICAO_CIRCUITO - index_v2_count;
                                m_col.isbase = isBase;
                                m_col.lCircuitoSeguidores = n_motor.ToList();
                                m_col.Qtd_Circuito = m_col.lCircuitoSeguidores.Count + 1;

                                m_col.myValoresPrincipal = valoresRetornados;
                                for (int i = 0; i < n_motor.Length; i++)
                                {
                                    Util.ObjectCalibragem c_col = Util.ObjectCalibragem.Load(n_motor[i]+16);
                                    List<ValoresVO> calibragem_col = c_col.Valores;
                                    ValoresVO valoresRetornados_col = new ValoresVO();
                                    if (Dat_usado_tcp == "@dat 12")
                                    {
                                        valoresRetornados_col.PulsoHorario = int.Parse(Math.Round(VOLUME * 1000).ToString());
                                        valoresRetornados_col.Volume = VOLUME;
                                        valoresRetornados_col.MassaIdeal = 0;
                                        valoresRetornados_col.MassaMedia = 0;
                                        valoresRetornados_col.Aceleracao = 0;
                                        valoresRetornados_col.Delay = 0;
                                        valoresRetornados_col.DesvioMedio = 0;
                                        valoresRetornados_col.PulsoReverso = 0;
                                        valoresRetornados_col.Velocidade = 0;
                                    }
                                    else
                                    {
                                        valoresRetornados_col = Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                    }
                                    //Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                    Negocio.ListValoresVOCircuitos lvO = new Negocio.ListValoresVOCircuitos();
                                    lvO.circuito = n_motor[i];
                                    lvO.myValores = valoresRetornados_col;
                                    //m_col.myValores = valoresRetornados;
                                    m_col.myValores.Add(lvO);

                                }

                                //m_col.myValores = valoresRetornados;

                                lColorSeguidor_P2.Add(m_col);
                                if (isBase && (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases ||
                                    _parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes))
                                {
                                    isDosarBase_P2 = true;
                                }
                            }
                            else
                            {
                                if (_exist_Seg_P2 && !isBase)
                                {
                                    vValores2[(POSICAO_CIRCUITO - index_v2_count) - 1] = valoresRetornados;
                                }
                                else if (isBase && !_parametros.HabilitarDispensaSequencial && (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases ||
                                    _parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes))
                                {
                                    vValores2_b[(POSICAO_CIRCUITO - index_v2_count) - 1] = valoresRetornados;
                                    if(_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                    {
                                        vValores2[(POSICAO_CIRCUITO - index_v2_count) - 1] = valoresRetornados;
                                    }
                                    isDosarBase_P2 = true;
                                }
                                //else if (!_exist_Seg_P2)
                                //{
                                //    vValores2[(POSICAO_CIRCUITO - 16) - 1] = valoresRetornados;
                                //}
                                
                                else
                                {
                                    vValores2[(POSICAO_CIRCUITO - index_v2_count) - 1] = valoresRetornados;
                                }
                            }
                        }

                        #endregion
                    }

                   

                    if (modBusDispenser_P3 != null && !Operar.ConectarP3(ref modBusDispenser_P3))
                    {
                        #region Não conectou e usuário cancelou

                        string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                        string[] log = { Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao, Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado };
                        string mensagemUsuario =
                             Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao
                            + Environment.NewLine
                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;

                        EncerrarOperacao(descricaoDAT, log, mensagemUsuario);
                        if (this.machine_turned_on)
                        {
                            gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                            this.machine_turned_on = false;
                        }
                        this.menu = false;
                        return;
                        #endregion
                    }

                    if (modBusDispenser_P3 == null && !Operar.Conectar(ref dispenser))
                    {
                        #region Não conectou e usuário cancelou

                        string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                        string[] log = { Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao, Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado };
                        string mensagemUsuario =
                             Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao
                            + Environment.NewLine
                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;

                        EncerrarOperacao(descricaoDAT, log, mensagemUsuario);
                        //this.machine_turned_on = false;
                        if (this.machine_turned_on)
                        {
                            gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                            this.machine_turned_on = false;
                        }
                        this.menu = false;

                        if (isProtocolUDCP)
                        {
                            if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                            {
                                File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                            }

                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao);
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                            if (this._parametros.CreateFileTmpUDCP == 1)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao);
                                    WriteCanisterContents(sw, null, null);
                                    sw.Close();
                                }
                            }

                            if (this._parametros.DelayUDCP > 0)
                            {
                                Thread.Sleep(this._parametros.DelayUDCP * 1000);
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }
                        return;

                        #endregion
                    }

                    if (modBusDispenser_P3 == null && _parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        #region Não conectou e usuário cancelou

                        string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                        string[] log = { Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao, Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado };
                        string mensagemUsuario =
                            Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao
                            + Environment.NewLine
                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;

                        EncerrarOperacao(descricaoDAT, log, mensagemUsuario);

                        //this.machine_turned_on = false;
                        //if (this.machine_turned_on)
                        //{
                        //    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                        //    this.machine_turned_on = false;
                        //}
                        this.menu = false;
                        if (isProtocolUDCP)
                        {
                            if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                            {
                                File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                            }

                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao);
                                sw.Close();
                            }
                            if (this._parametros.CreateFileTmpUDCP == 1)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_99 + Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao);
                                    sw.Close();
                                }
                            }

                            if (this._parametros.DelayUDCP > 0)
                            {
                                Thread.Sleep(this._parametros.DelayUDCP * 1000);
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }
                        return;

                        #endregion
                    }

                    if (!this.machine_turned_on)
                    {
                        this.machine_turned_on = true;
                        gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                    }
                    if (modBusDispenser_P3 == null && !Operar.TemRecipiente(dispenser))
                    {
                        #region Não há recipiente e usuário cancelou

                        string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2;
                        string log = Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;
                        string mensagemUsuario =
                            Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2
                            + Environment.NewLine
                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;

                        EncerrarOperacao(descricaoDAT, log, mensagemUsuario);
                        this.menu = false;
                        if (isProtocolUDCP)
                        {
                            if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                            {
                                File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                            }

                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2);
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                            if (this._parametros.CreateFileTmpUDCP == 1)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2);
                                    sw.Close();
                                }
                            }

                            if (this._parametros.DelayUDCP > 0)
                            {
                                Thread.Sleep(this._parametros.DelayUDCP * 1000);
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }
                        return;

                        #endregion
                    }


                    bool confirmaVolumeMinimoDat = true;
                    if (!_parametros.DesabilitarVolumeMinimoDat)
                    {
                        string str_l = "";
                        foreach (KeyValuePair<int, double> item in demanda)
                        {
                            if (Dat_usado_tcp == "@dat 12")
                            {
                                Util.ObjectColorante colorante = this._colorantes.Find(o => o.Circuito == item.Key);
                                double valor = UnidadeMedidaHelper.GramaToMililitro(item.Value, colorante.MassaEspecifica);
                                if (valor < _parametros.VolumeMinimoDat)
                                {
                                    str_l += item.Key + ";" + item.Value;
                                }

                            }
                            else
                            {
                                if (item.Value < _parametros.VolumeMinimoDat)
                                {
                                    str_l += item.Key + ";" + item.Value;
                                }
                            }
                        }
                        if (str_l.Length > 0)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                            {
                                //Confirmar dispensar com valores inferiores ao mínimo permitido de 0,0385 ml?
                                //string msg = string.Format("Confirmar dispensar com valores inferiores ao mínimo permitido de {0} ml?", _parametros.VolumeMinimoDat.ToString());
                                string msg = string.Format(Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolume, _parametros.VolumeMinimoDat.ToString());
                                confirmaVolumeMinimoDat = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            }
                            if (confirmaVolumeMinimoDat)
                            {

                                //string msg = string.Format("Dispensar com Valores Inferior ao Mínimo de Volume permitido: {0}", str_l);
                                string msg = string.Format(Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolumeLog, str_l);
                                Log.Logar(
                                      TipoLog.Processo,
                                      _parametros.PathLogProcessoDispensa,
                                      msg);
                            }
                        }
                    }

                    if (confirmaVolumeMinimoDat)
                    {
                        if (!File.Exists(this.CLICK_PROGRESS))
                        {
                            File.Create(this.CLICK_PROGRESS).Close();
                        }
                        DialogResult result = DialogResult.None;
                        if (_parametros.HabilitarRecirculacaoAuto)
                        {
                            if (modBusDispenser_P2 != null)
                            {
                                modBusDispenser_P2.AcionaValvulas(false);
                            }
                        }

                        if (_parametros.HabilitarDispensaSequencial)
                        {
                            #region Executa processo de dipensa sequencial

                            //bool is24MotoresP1 = false;
                            //bool is24MotoresP2 = false;
                            //if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                            //{
                            //    is24MotoresP1 = true; 
                            //}
                            //if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador /*|| (Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4*/)
                            //{
                            //    is24MotoresP2 = true;
                            //}
                            DispensaSequencialVO dispSeqVO = new DispensaSequencialVO();
                            dispSeqVO.Dispenser = new List<IDispenser>();
                            if (modBusDispenser_P3 == null)
                            {
                                dispSeqVO.Dispenser.Add(dispenser);
                                if (_parametros.IdDispositivo2 != 0)
                                {
                                    dispSeqVO.Dispenser.Add(dispenser2);
                                }
                            }
                            else
                            {
                                dispSeqVO.modBusDispenser_P3 = modBusDispenser_P3;
                            }
                            dispSeqVO.Demanda = demanda;
                            dispSeqVO.DescricaoCor = DAT.CodigoCor;
                            dispSeqVO.CodigoCor = "";
                            if (Dat_usado_tcp == "@dat 03")
                            {
                                if (dispSeqVO.DescricaoCor != null && dispSeqVO.DescricaoCor.Contains(";"))
                                {
                                    string[] ndesc = dispSeqVO.DescricaoCor.Split(';');
                                    if (ndesc != null && ndesc.Length > 2)
                                    {
                                        if (ndesc[2] != null && ndesc[2].Contains(","))
                                        {
                                            string[] nCod = ndesc[2].Split(',');
                                            if (nCod != null && nCod.Length > 1)
                                            {
                                                dispSeqVO.CodigoCor = nCod[1];
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Dat_usado_tcp == "@dat 04")
                            {
                                if (dispSeqVO.DescricaoCor != null && dispSeqVO.DescricaoCor.Contains(";"))
                                {
                                    string[] ndesc = dispSeqVO.DescricaoCor.Split(';');
                                    if (ndesc != null && ndesc.Length > 2)
                                    {
                                        if (ndesc[2] != null && ndesc[2].Length > 0 && ndesc[2].Contains(" "))
                                        {
                                            string[] nCod = ndesc[2].Substring(5).Replace("\r", "").Replace("\n", "").Split(' ');

                                            if (nCod != null && nCod.Length > 1)
                                            {
                                                dispSeqVO.CodigoCor = nCod[0].Replace("\"", "") + " " + nCod[1].Replace("\"", "");
                                            }

                                        }
                                    }
                                }
                            }
                            else if (Dat_usado_tcp == "@dat 06")
                            {
                                if (DAT.CodigoCor != null && DAT.CodigoCor.Contains(";"))
                                {
                                    string[] ndesc = DAT.CodigoCor.Split(';');
                                    if (ndesc != null && ndesc.Length > 2)
                                    {
                                        for (int i = 0; i < ndesc.Length - 2; i++)
                                        {
                                            if (dispSeqVO.CodigoCor.Length == 0)
                                            {
                                                dispSeqVO.CodigoCor += ndesc[i];
                                            }
                                            else
                                            {
                                                dispSeqVO.CodigoCor += ";" + ndesc[i];
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (KeyValuePair<int, double> item in demanda)
                            {
                                dispSeqVO.Colorantes.Add(Util.ObjectColorante.Load(item.Key));
                            }
                            int countP2 = 0;
                            foreach (Util.ObjectColorante colorante in dispSeqVO.Colorantes)
                            {
                                int index = colorante.Circuito - 1;
                                if (colorante.Dispositivo == 1)
                                {
                                    dispSeqVO.PulsoHorario[index] = vValores[index].PulsoHorario;
                                    dispSeqVO.Velocidade[index] = vValores[index].Velocidade;
                                    dispSeqVO.Aceleracao[index] = vValores[index].Aceleracao;
                                    dispSeqVO.Delay[index] = vValores[index].Delay;
                                    dispSeqVO.PulsoReverso[index] = vValores[index].PulsoReverso;
                                    dispSeqVO.Volume[index] = vValores[index].Volume;
                                }
                                else if (colorante.Dispositivo == 2 && modBusDispenser_P3 == null)
                                {                                    
                                    if (countP2 < 16)
                                    {
                                        //if (is24MotoresP1)
                                        //{
                                        //    dispSeqVO.PulsoHorario2[index - 24] = vValores2[index - 16].PulsoHorario;
                                        //    dispSeqVO.Velocidade2[index - 24] = vValores2[index - 16].Velocidade;
                                        //    dispSeqVO.Aceleracao2[index - 24] = vValores2[index - 16].Aceleracao;
                                        //    dispSeqVO.Delay2[index - 24] = vValores2[index - 16].Delay;
                                        //    dispSeqVO.PulsoReverso2[index - 24] = vValores2[index - 16].PulsoReverso;
                                        //    dispSeqVO.Volume2[index - 24] = vValores2[index - 16].Volume;
                                        //}
                                        //else
                                        {
                                            dispSeqVO.PulsoHorario2[index - 16] = vValores2[index - 16].PulsoHorario;
                                            dispSeqVO.Velocidade2[index - 16] = vValores2[index - 16].Velocidade;
                                            dispSeqVO.Aceleracao2[index - 16] = vValores2[index - 16].Aceleracao;
                                            dispSeqVO.Delay2[index - 16] = vValores2[index - 16].Delay;
                                            dispSeqVO.PulsoReverso2[index - 16] = vValores2[index - 16].PulsoReverso;
                                            dispSeqVO.Volume2[index - 16] = vValores2[index - 16].Volume;
                                        }
                                    }
                                    countP2++;
                                }
                            }
                            

                            using (Form f = new fDispensaSequencial(
                                dispSeqVO, _parametros.HabilitarDispensaSequencialP1, _parametros.HabilitarDispensaSequencialP2, dispSeqVO.Demanda,
                                _parametros.DesabilitarInterfaceDispensaSequencial))
                            {
                                result = f.ShowDialog();
                            }

                            #endregion

                            #region gerar Evento Dispensa Produtos
                            if (result == DialogResult.OK)
                            {
                                string delathes = "";
                                try
                                {
                                    foreach (KeyValuePair<int, double> item in dispSeqVO.Demanda)
                                    {
                                        Util.ObjectColorante itemColorante = this._colorantes.Find(o => o.Circuito == item.Key);
                                        if (itemColorante != null)
                                        {
                                            if (delathes == "")
                                            {
                                                delathes += item.Key.ToString() + "," + itemColorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                            }
                                            else
                                            {
                                                delathes += "," + item.Key.ToString() + "," + itemColorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                            }
                                        }
                                    }
                                }
								catch (Exception ex)
								{
									LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
								}

								gerarEventoDispensa(0, delathes);
                            }
                            else if (result == DialogResult.Cancel)
                            {
                                gerarEventoDispensa(1);
                            }
                            else if (result == DialogResult.Abort)
                            {
                                gerarEventoDispensa(2);
                            }
                            else
                            {
                                gerarEventoDispensa(3);
                            }
                            #endregion
                        }
                        else
                        {
                            #region Executa processo de dispensa simultânea   
                            string _codCor = "";
                            if (Dat_usado_tcp == "@dat 03")
                            {
                                if (DAT.CodigoCor != null && DAT.CodigoCor.Contains(";"))
                                {
                                    string[] ndesc = DAT.CodigoCor.Split(';');
                                    if (ndesc != null && ndesc.Length > 2)
                                    {
                                        if (ndesc[2] != null && ndesc[2].Contains(","))
                                        {
                                            string[] nCod = ndesc[2].Split(',');
                                            if (nCod != null && nCod.Length > 1)
                                            {
                                                _codCor = nCod[1];
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Dat_usado_tcp == "@dat 04")
                            {
                                if (DAT.CodigoCor != null && DAT.CodigoCor.Contains(";"))
                                {
                                    string[] ndesc = DAT.CodigoCor.Split(';');
                                    if (ndesc != null && ndesc.Length > 2)
                                    {
                                        if (ndesc[2] != null && ndesc[2].Length > 0 && ndesc[2].Contains(" "))
                                        {
                                            string[] nCod = ndesc[2].Substring(5).Replace("\r", "").Replace("\n", "").Split(' ');

                                            if (nCod != null && nCod.Length > 1)
                                            {
                                                _codCor = nCod[0].Replace("\"", "") + " " + nCod[1].Replace("\"", "");
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Dat_usado_tcp == "@dat 06")
                            {
                                if (DAT.CodigoCor != null && DAT.CodigoCor.Contains(";"))
                                {
                                    string[] ndesc = DAT.CodigoCor.Split(';');
                                    if (ndesc != null && ndesc.Length > 2)
                                    {
                                        for(int i = 0; i< ndesc.Length-1; i++)
                                        {
                                            if (_codCor.Length == 0)
                                            {
                                                _codCor += ndesc[i];
                                            }
                                            else
                                            {
                                                _codCor += ";" +ndesc[i];
                                            }
                                        }
                                    }
                                }
                            }

                            List<IDispenser> lDisp = new List<IDispenser>();
                            if (modBusDispenser_P3 == null)
                            {
                                lDisp.Add(dispenser);
                                if (_parametros.IdDispositivo2 != 0)
                                {
                                    lDisp.Add(dispenser2);
                                }
                            }

                            List<Util.ObjectColorante> colSimultanea = new List<Util.ObjectColorante>();
                            foreach (KeyValuePair<int, double> item in demanda)
                            {
                                colSimultanea.Add(Util.ObjectColorante.Load(item.Key));
                            }

                            List<ValoresVO[]> v_Val_DosP1_B = new List<ValoresVO[]>();
                            List<ValoresVO[]> v_Val_DosP2_B = new List<ValoresVO[]>();
                            if (lColorSeguidor_P1 != null)
                            {
                                List<ValoresVO[]> v_Val_Dos_B = new List<ValoresVO[]>();

                                lColorSeguidor_P1 = lColorSeguidor_P1.OrderBy(o => o.Qtd_Circuito).ToList();
                                int index_vB = 16;
                                if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                                {
                                    index_vB = 16;
                                }
                                if (isDosarBase_P1)
                                {
                                    ValoresVO[] v_variavel_B = vValores_b;
                                    List<Negocio.ListColoranteSeguidor> col_SegBase = new List<Negocio.ListColoranteSeguidor>();
                                    for (int _i_mSeg = 0; _i_mSeg < lColorSeguidor_P1.Count; _i_mSeg++)
                                    {
                                        Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P1[_i_mSeg];
                                        if (_cSeguidor.isbase)
                                        {
                                            col_SegBase.Add(_cSeguidor);
                                            lColorSeguidor_P1.RemoveAt(_i_mSeg--);
                                        }
                                    }

                                    int qtd_Max = 0;
                                    
                                    for (int _m_i = 0; _m_i < index_vB; _m_i++)
                                    {
                                        if (v_variavel_B[_m_i] != null)
                                        {
                                            qtd_Max++;
                                        }
                                    }
                                    if (qtd_Max > 5 || col_SegBase.Count == 0)
                                    {
                                        v_Val_Dos_B.Add(v_variavel_B);
                                        v_variavel_B = new ValoresVO[index_vB];
                                        qtd_Max = 0;
                                    }
                                    for (int _i_segbase = 0; _i_segbase < col_SegBase.Count; _i_segbase++)
                                    {
                                        Negocio.ListColoranteSeguidor _cSeguidor = col_SegBase[_i_segbase];
                                        if ((qtd_Max + _cSeguidor.Qtd_Circuito) > 5)
                                        {
                                            v_Val_Dos_B.Add(v_variavel_B);
                                            qtd_Max = 0;
                                            v_variavel_B = new ValoresVO[index_vB];
                                        }
                                        v_variavel_B[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                        qtd_Max++;
                                        foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                        {
                                            Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                            if (lvo_local != null)
                                            {
                                                v_variavel_B[_c_list - 1] = lvo_local.myValores;
                                            }
                                            else
                                            {
                                                v_variavel_B[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                            }
                                            //v_variavel_B[_c_list - 1] = _cSeguidor.myValores;
                                            qtd_Max++;
                                        }
                                        if ((_i_segbase + 1) >= col_SegBase.Count)
                                        {
                                            v_Val_Dos_B.Add(v_variavel_B);
                                        }
                                    }
                                   
                                }
                                if (isDosarBase_P1 && _parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                {
                                    for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                    {
                                        v_Val_DosP1_B.Add(v_Val_Dos_B[_i]);
                                    }
                                }

                                ValoresVO[] vSeg = new ValoresVO[index_vB];
                                bool ainda_tem_ckt = true;
                                int qtd_nCirc = 0;
                                //foreach (Negocio.ListColoranteSeguidor _cSeguidor in lColorSeguidor_P1)
                                for (int _v_lCol = 0; _v_lCol < lColorSeguidor_P1.Count; _v_lCol++)
                                {
                                    Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P1[_v_lCol];
                                    if (qtd_nCirc + _cSeguidor.Qtd_Circuito > 5)
                                    {
                                        v_Val_DosP1_B.Add(vSeg);
                                        vSeg = new ValoresVO[index_vB];
                                        qtd_nCirc = 0;
                                    }
                                    qtd_nCirc = _cSeguidor.Qtd_Circuito;
                                    vSeg[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                    int _i_seg = _cSeguidor.Qtd_Circuito;
                                    foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                    {
                                        Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                        if (lvo_local != null)
                                        {
                                            vSeg[_c_list - 1] = lvo_local.myValores;
                                        }
                                        else
                                        {
                                            vSeg[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                        }
                                        //vSeg[_c_list - 1] = _cSeguidor.myValores;
                                    }
                                    bool aghou_5 = false;
                                    if (ainda_tem_ckt)
                                    {
                                        ainda_tem_ckt = false;
                                        for (int _t_seg = _i_seg; !aghou_5 && _t_seg < 5; _t_seg++)
                                        {
                                            for (int _v_VO = 0; !aghou_5 && _v_VO < vValores.Length; _v_VO++)
                                            {
                                                if (vValores[_v_VO] != null)
                                                {
                                                    ainda_tem_ckt = true;
                                                    //vSeg[_v_VO] = vValores[_v_VO];
                                                    vSeg[_v_VO] = new ValoresVO();
                                                    vSeg[_v_VO].Aceleracao = vValores[_v_VO].Aceleracao;
                                                    vSeg[_v_VO].Delay = vValores[_v_VO].Delay;
                                                    vSeg[_v_VO].DesvioMedio = vValores[_v_VO].DesvioMedio;
                                                    vSeg[_v_VO].MassaIdeal = vValores[_v_VO].MassaIdeal;
                                                    vSeg[_v_VO].MassaMedia = vValores[_v_VO].MassaMedia;
                                                    vSeg[_v_VO].PulsoHorario = vValores[_v_VO].PulsoHorario;
                                                    vSeg[_v_VO].PulsoReverso = vValores[_v_VO].PulsoReverso;
                                                    vSeg[_v_VO].Velocidade = vValores[_v_VO].Velocidade;
                                                    vSeg[_v_VO].Volume = vValores[_v_VO].Volume;
                                                    vValores[_v_VO] = null;
                                                    _i_seg++;
                                                    qtd_nCirc++;
                                                    if (_i_seg > 4)
                                                    {
                                                        aghou_5 = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (qtd_nCirc >= 5)
                                    {
                                        v_Val_DosP1_B.Add(vSeg);
                                        vSeg = new ValoresVO[index_vB];
                                        qtd_nCirc = 0;
                                    }
                                    else if ((_v_lCol + 1) >= lColorSeguidor_P1.Count)
                                    {
                                        v_Val_DosP1_B.Add(vSeg);
                                    }
                                }
                                bool sobrou_algum_ckt_seg = false;
                                for (int _v_VO = 0; !sobrou_algum_ckt_seg && _v_VO < vValores.Length; _v_VO++)
                                {
                                    if (vValores[_v_VO] != null)
                                    {
                                        sobrou_algum_ckt_seg = true;
                                    }
                                }
                                
                                if (sobrou_algum_ckt_seg)
                                {
                                    v_Val_DosP1_B.Add(vValores);
                                }
                                if (isDosarBase_P1 && _parametros.TipoDosagemExec != (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                {
                                    for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                    {
                                        v_Val_DosP1_B.Add(v_Val_Dos_B[_i]);
                                    }
                                }
                               

                            }
                            else
                            {
                                
                                if (isDosarBase_P1)
                                {
                                    if (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                    {
                                        v_Val_DosP1_B.Add(vValores_b);
                                        v_Val_DosP1_B.Add(vValores);
                                    }
                                    else if (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes)
                                    {
                                        v_Val_DosP1_B.Add(vValores);
                                        v_Val_DosP1_B.Add(vValores_b);
                                        
                                    }
                                }
                                else
                                {
                                    v_Val_DosP1_B.Add(vValores);
                                }
                                
                            }
                            if (lColorSeguidor_P2 != null)
                            {
                                int index_vB2 = 16;
                                if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador/* || (Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4*/)
                                {
                                    index_vB2 = 16;
                                }
                                List<ValoresVO[]> v_Val_Dos_B = new List<ValoresVO[]>();
                                lColorSeguidor_P2 = lColorSeguidor_P2.OrderBy(o => o.Qtd_Circuito).ToList();
                                if (isDosarBase_P2)
                                {
                                    ValoresVO[] v_variavel_B = vValores2_b;
                                    List<Negocio.ListColoranteSeguidor> col_SegBase_2 = new List<Negocio.ListColoranteSeguidor>();
                                    for (int _i_mSeg = 0; _i_mSeg < lColorSeguidor_P2.Count; _i_mSeg++)
                                    {
                                        Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P2[_i_mSeg];
                                        if (_cSeguidor.isbase)
                                        {
                                            col_SegBase_2.Add(_cSeguidor);
                                            lColorSeguidor_P2.RemoveAt(_i_mSeg--);
                                        }
                                    }
                                    int qtd_Max = 0;
                                    for (int _m_i = 0; _m_i < index_vB2; _m_i++)
                                    {
                                        if (v_variavel_B[_m_i] != null)
                                        {
                                            qtd_Max++;
                                        }
                                    }
                                    if (qtd_Max > 5 || col_SegBase_2.Count == 0)
                                    {
                                        v_Val_Dos_B.Add(v_variavel_B);
                                        v_variavel_B = new ValoresVO[index_vB2];
                                        qtd_Max = 0;
                                    }
                                    for (int _i_segbase = 0; _i_segbase < col_SegBase_2.Count; _i_segbase++)
                                    {
                                        Negocio.ListColoranteSeguidor _cSeguidor = col_SegBase_2[_i_segbase];
                                        if ((qtd_Max + _cSeguidor.Qtd_Circuito) > 5)
                                        {
                                            v_Val_Dos_B.Add(v_variavel_B);
                                            qtd_Max = 0;
                                            v_variavel_B = new ValoresVO[index_vB2];
                                        }
                                        v_variavel_B[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                        qtd_Max++;
                                        foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                        {
                                            Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                            if (lvo_local != null)
                                            {
                                                v_variavel_B[_c_list - 1] = lvo_local.myValores;
                                            }
                                            else
                                            {
                                                v_variavel_B[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                            }
                                            //v_variavel_B[_c_list - 1] = _cSeguidor.myValores;
                                            qtd_Max++;
                                        }
                                        if ((_i_segbase + 1) >= col_SegBase_2.Count)
                                        {
                                            v_Val_Dos_B.Add(v_variavel_B);
                                        }
                                    }
                                    

                                }
                                if (isDosarBase_P2 &&_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                {
                                    for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                    {
                                        v_Val_DosP2_B.Add(v_Val_Dos_B[_i]);
                                    }
                                }

                                ValoresVO[] vSeg = new ValoresVO[index_vB2];
                                bool ainda_tem_ckt = true;
                                int qtd_nCirc = 0;
                                //foreach (Negocio.ListColoranteSeguidor _cSeguidor in lColorSeguidor_P2)
                                for (int _v_lCol = 0; _v_lCol < lColorSeguidor_P2.Count; _v_lCol++)
                                {
                                    //ValoresVO[] vSeg = new ValoresVO[16];
                                    Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P2[_v_lCol];
                                    if (qtd_nCirc + _cSeguidor.Qtd_Circuito > 5)
                                    {
                                        v_Val_DosP2_B.Add(vSeg);
                                        vSeg = new ValoresVO[index_vB2];
                                        qtd_nCirc = 0;
                                    }
                                    qtd_nCirc = _cSeguidor.Qtd_Circuito;
                                    vSeg[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                    int _i_seg = _cSeguidor.Qtd_Circuito;
                                    foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                    {
                                        Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                        if (lvo_local != null)
                                        {
                                            vSeg[_c_list - 1] = lvo_local.myValores;
                                        }
                                        else
                                        {
                                            vSeg[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                        }
                                        //if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_1 || 
                                        //    (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_2)
                                        //{
                                        //    vSeg[(_c_list - 16) - 1] = _cSeguidor.myValores;
                                        //}
                                        //else
                                        //{
                                        //    vSeg[(_c_list - 24) - 1] = _cSeguidor.myValores;
                                        //}
                                    }
                                    bool aghou_5 = false;
                                    if (ainda_tem_ckt)
                                    {
                                        ainda_tem_ckt = false;
                                        for (int _t_seg = _i_seg; !aghou_5 && _t_seg < 5; _t_seg++)
                                        {
                                            for (int _v_VO = 0; !aghou_5 && _v_VO < vValores2.Length; _v_VO++)
                                            {
                                                if (vValores2[_v_VO] != null)
                                                {
                                                    //vSeg[_v_VO] = vValores2[_v_VO];
                                                    vSeg[_v_VO] = new ValoresVO();
                                                    vSeg[_v_VO].Aceleracao = vValores2[_v_VO].Aceleracao;
                                                    vSeg[_v_VO].Delay = vValores2[_v_VO].Delay;
                                                    vSeg[_v_VO].DesvioMedio = vValores2[_v_VO].DesvioMedio;
                                                    vSeg[_v_VO].MassaIdeal = vValores2[_v_VO].MassaIdeal;
                                                    vSeg[_v_VO].MassaMedia = vValores2[_v_VO].MassaMedia;
                                                    vSeg[_v_VO].PulsoHorario = vValores2[_v_VO].PulsoHorario;
                                                    vSeg[_v_VO].PulsoReverso = vValores2[_v_VO].PulsoReverso;
                                                    vSeg[_v_VO].Velocidade = vValores2[_v_VO].Velocidade;
                                                    vSeg[_v_VO].Volume = vValores2[_v_VO].Volume;
                                                    vValores2[_v_VO] = null;
                                                    ainda_tem_ckt = true;
                                                    _i_seg++;
                                                    qtd_nCirc++;
                                                    if (_i_seg > 4)
                                                    {
                                                        aghou_5 = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //v_Val_DosP2_B.Add(vSeg);
                                    if (qtd_nCirc >= 5)
                                    {
                                        v_Val_DosP2_B.Add(vSeg);
                                        vSeg = new ValoresVO[index_vB2];
                                        qtd_nCirc = 0;
                                    }
                                    else if ((_v_lCol + 1) >= lColorSeguidor_P2.Count)
                                    {
                                        v_Val_DosP2_B.Add(vSeg);
                                    }
                                }
                                bool sobrou_algum_ckt_seg = false;
                                for (int _v_VO = 0; !sobrou_algum_ckt_seg && _v_VO < vValores2.Length; _v_VO++)
                                {
                                    if (vValores2[_v_VO] != null)
                                    {
                                        sobrou_algum_ckt_seg = true;
                                    }
                                }
                                if (sobrou_algum_ckt_seg)                                
                                {
                                    v_Val_DosP2_B.Add(vValores2);
                                }

                                if (isDosarBase_P2 && _parametros.TipoDosagemExec != (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                {
                                    for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                    {
                                        v_Val_DosP2_B.Add(v_Val_Dos_B[_i]);
                                    }
                                }
                            }
                            else
                            {
                                bool exist_prod = false;
                                for (int _i_p_l = 0; !exist_prod && _i_p_l < vValores2.Length; _i_p_l++)
                                {
                                    if (vValores2[_i_p_l] != null)
                                    {
                                        exist_prod = true;
                                    }
                                }
                                if (isDosarBase_P2)
                                {
                                    if (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases)
                                    {
                                        v_Val_DosP2_B.Add(vValores2_b);
                                        if (exist_prod)
                                        {
                                            v_Val_DosP2_B.Add(vValores2);
                                        }
                                    }
                                    else if (_parametros.TipoDosagemExec == (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes)
                                    {
                                        if (exist_prod)
                                        {
                                            v_Val_DosP2_B.Add(vValores2);
                                        }
                                        v_Val_DosP2_B.Add(vValores2_b);
                                        
                                    }
                                }
                                else
                                {
                                    if (exist_prod)
                                    {
                                        v_Val_DosP2_B.Add(vValores2);
                                    }
                                }
                                
                                
                            }

                            using (Form f = new fDispensaSimultanea(
                                lDisp, v_Val_DosP1_B, v_Val_DosP2_B, demanda, colSimultanea, DAT.CodigoCor,
                                _parametros.DesabilitarInterfaceDispensaSimultanea, _codCor, modBusDispenser_P3, isDosarBase_P1, isDosarBase_P2))
                            {
                                result = f.ShowDialog();
                            }

                            #endregion
                            #region gerar Evento Dispensa Produtos
                            if (result == DialogResult.OK)
                            {
                                string delathes = "";
                                try
                                {
                                    foreach (KeyValuePair<int, double> item in demanda)
                                    {
                                        Util.ObjectColorante itemColorante = this._colorantes.Find(o => o.Circuito == item.Key);
                                        if (itemColorante != null)
                                        {
                                            if (delathes == "")
                                            {
                                                delathes += item.Key.ToString() + "," + itemColorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                            }
                                            else
                                            {
                                                delathes += "," + item.Key.ToString() + "," + itemColorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                            }
                                        }
                                    }
                                }
								catch (Exception ex)
								{
									LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
								}

								gerarEventoDispensa(0, delathes);
                            }
                            else if (result == DialogResult.Cancel)
                            {
                                gerarEventoDispensa(1);
                            }
                            else if (result == DialogResult.Abort)
                            {
                                gerarEventoDispensa(2);
                            }
                            else
                            {
                                gerarEventoDispensa(3);
                            }
                            #endregion
                        }

                        
                        if(isProtocolUDCP && _parametros.ProcRemoveLataUDCP)
                        {
                            switch ((Dispositivo)_parametros.IdDispositivo)
                            {
                                case Dispositivo.Simulador:
                                    {
                                        break;
                                    }
                                case Dispositivo.Placa_1:
                                    {
                                        break;
                                    }
                                case Dispositivo.Placa_2:
                                    {
                                        if (this._parametros.HabilitarTesteRecipiente)
                                        {
                                            RemoverLataUDCP(dispenser);
                                        }
                                        
                                        break;
                                    }
                                case Dispositivo.Placa_3:
                                    {
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }

                        switch (result)
                        {
                            #region Trata o arquivo dat de acordo com o retorno da dispensa

                            case DialogResult.Cancel:
                                {
                                    if (isProtocolUDCP)
                                    {
                                        if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                                        {
                                            File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 1");
                                            sw.WriteLine("Output = 0");
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_07 + Negocio.IdiomaResxExtensao.Global_DosagemCancelada);
                                            WriteCanisterContents(sw, null, null);
                                            sw.Close();
                                        }
                                        if (this._parametros.CreateFileTmpUDCP == 1)
                                        {
                                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                            {
                                                sw.WriteLine("[General]");
                                                sw.WriteLine("Result = 1");
                                                sw.WriteLine("Output = 0");
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_07 + Negocio.IdiomaResxExtensao.Global_DosagemCancelada);
                                                WriteCanisterContents(sw, null, null);
                                                sw.Close();
                                            }
                                        }

                                        if (this._parametros.DelayUDCP > 0)
                                        {
                                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                        {
                                            sw.WriteLine("0");
                                            sw.Close();
                                        }
                                    }
                                    EncerrarOperacao(
                                        descricaoDAT: Negocio.IdiomaResxExtensao.Global_Informacao_DispensaCancelada,
                                        log: Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado,
                                        mensagemUsuario:
                                            Negocio.IdiomaResxExtensao.Global_Informacao_DispensaCancelada
                                            + Environment.NewLine
                                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                                    break;
                                }
                            case DialogResult.Abort:
                                {
                                    if (isProtocolUDCP)
                                    {
                                        if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                                        {
                                            File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 1");
                                            sw.WriteLine("Output = 0");
                                            sw.WriteLine("MsgResult = "+ Negocio.IdiomaResxExtensao.Log_Cod_08 + Negocio.IdiomaResxExtensao.Global_DosagemAbortada);
                                            WriteCanisterContents(sw, null, null);
                                            sw.Close();
                                        }
                                        if (this._parametros.CreateFileTmpUDCP == 1)
                                        {
                                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                            {
                                                sw.WriteLine("[General]");
                                                sw.WriteLine("Result = 1");
                                                sw.WriteLine("Output = 0");
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_08 + Negocio.IdiomaResxExtensao.Global_DosagemAbortada);
                                                WriteCanisterContents(sw, null, null);
                                                sw.Close();
                                            }
                                        }

                                        if (this._parametros.DelayUDCP > 0)
                                        {
                                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                        {
                                            sw.WriteLine("0");
                                            sw.Close();
                                        }
                                    }
                                    EncerrarOperacao(
                                        descricaoDAT: Negocio.IdiomaResxExtensao.Global_Informacao_DispensaAbortada,
                                        log: Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado,
                                        mensagemUsuario:
                                            Negocio.IdiomaResxExtensao.Global_Informacao_DispensaAbortada
                                            + Environment.NewLine
                                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                                    break;
                                }
                            case DialogResult.No:
                                {
                                    if (isProtocolUDCP)
                                    {
                                        if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                                        {
                                            File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 1");
                                            sw.WriteLine("Output = 0");
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_06 + Negocio.IdiomaResxExtensao.Global_FalhaDosagem);
                                            WriteCanisterContents(sw, null, null);
                                            sw.Close();
                                        }
                                        if (this._parametros.CreateFileTmpUDCP == 1)
                                        {
                                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                            {
                                                sw.WriteLine("[General]");
                                                sw.WriteLine("Result = 1");
                                                sw.WriteLine("Output = 0");
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_06 + Negocio.IdiomaResxExtensao.Global_FalhaDosagem);
                                                WriteCanisterContents(sw, null, null);
                                                sw.Close();
                                            }
                                        }

                                        if (this._parametros.DelayUDCP > 0)
                                        {
                                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                        {
                                            sw.WriteLine("0");
                                            sw.Close();
                                        }
                                    }
                                    EncerrarOperacao(
                                        descricaoDAT: Negocio.IdiomaResxExtensao.Global_Falha_Dispensar,
                                        log: Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado,
                                        mensagemUsuario:
                                            Negocio.IdiomaResxExtensao.Global_Falha_Dispensar
                                            + Environment.NewLine
                                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                                    break;
                                }
                            case DialogResult.OK:
                                {
                                    #region Sucesso
                                    if (File.Exists(_parametros.PathMonitoramentoDAT))
                                    {
                                        FileHelper.Delete(_parametros.PathMonitoramentoDAT);
                                    }
                                    Log.Logar(
                                        TipoLog.Processo,
                                        _parametros.PathLogProcessoDispensa,
                                        Negocio.IdiomaResxExtensao.Global_Informacao_ArquivoDatDeletado);
                                    if(isProtocolUDCP)
                                    {  
                                        this._colorantes = Util.ObjectColorante.List();
                                        lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();

                                        if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                                        {
                                            File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 0");
                                            sw.WriteLine("Output = 0");
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_04 + Negocio.IdiomaResxExtensao.Global_DosagemConcluida);
                                            WriteCanisterContents(sw, demanda, null);
                                           
                                            sw.Close();
                                        }
                                        if (this._parametros.CreateFileTmpUDCP == 1)
                                        {
                                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                            {
                                                sw.WriteLine("[General]");
                                                sw.WriteLine("Result = 0");
                                                sw.WriteLine("Output = 0");
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_04 + Negocio.IdiomaResxExtensao.Global_DosagemConcluida);
                                                WriteCanisterContents(sw, demanda, null);
                                                
                                                sw.Close();
                                            }
                                        }

                                        if (this._parametros.DelayUDCP > 0)
                                        {
                                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                        }
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                        {
                                            sw.WriteLine("0");
                                            sw.Close();
                                        }
                                    }

                                    setCondicaoMaquinaLigada(true);

                                    #endregion

                                    //Armazenar a Produção 
                                    if (insertBDSql && !_parametros.DesabilitaMonitProcessoProducao)
                                    {
                                        string _serial = string.Empty;
                                        //Popula campos
                                        using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                                        {
                                            _serial = percRegistry.GetSerialNumber();
                                        }
                                        string dataHora = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
                                        string msgProduao = _serial + "|" + DAT.CodigoCor + "|";
                                        foreach (KeyValuePair<int, double> item in demanda)
                                        {
                                            #region Gera dados de dispensa
                                            int POSICAO_CIRCUITO = item.Key;
                                            double VOLUME = item.Value;


                                            msgProduao += POSICAO_CIRCUITO.ToString() + ";" + VOLUME.ToString() + ";";
                                            #endregion
                                            //
                                        }
                                        msgProduao = msgProduao.Replace('\r', ' ').Replace('\n', ' ').Replace('\"', ' ').Replace("  ;", ";").Replace(" ;", ";");
                                        if (Dat_usado_tcp == "@dat 03")
                                        {
                                            msgProduao = msgProduao.Replace("  ", " ");
                                        }
                                        msgProduao += "|" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", dtInicioDosagem) + "|" + Dat_usado_tcp;
                                        if (this.Prod.insertProducao(msgProduao, dataHora) <= 0)
                                        {
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Falha do inserir dosagem no BD Producao: " + msgProduao);
                                        }
                                    }
                                    break;

                                }

                                #endregion
                        }

                      

                        this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();

                        if (_parametros.HabilitarIdentificacaoCopo)
                        {
                            using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                                bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 20);
                            }
                        }
                    }
                    else
                    {
                        #region usuário cancelou
                        if (isProtocolUDCP)
                        {
                            if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                            {
                                File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                            }

                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_24 + Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat + Environment.NewLine + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                            if (this._parametros.CreateFileTmpUDCP == 1)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_24 + Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat + Environment.NewLine + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);
                                    WriteCanisterContents(sw, null, null); 
                                    sw.Close();
                                }
                            }

                            if (this._parametros.DelayUDCP > 0)
                            {
                                Thread.Sleep(this._parametros.DelayUDCP * 1000);
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }

                        string descricaoDAT = Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat;
                        string log = Negocio.IdiomaResxExtensao.Log_Cod_24 + Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat + Environment.NewLine + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;
                        string mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat + Environment.NewLine + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado;

                        EncerrarOperacao(descricaoDAT, log, mensagemUsuario);
                        this.menu = false;
                        #endregion
                        return;
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				
                    #region Catch


				    if (isProtocolUDCP)
                    {
                        if (File.Exists(Path.Combine(this.pathUDCP, "busy.flg")))
                        {
                            File.Delete(Path.Combine(this.pathUDCP, "busy.flg"));
                        }
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 1");
                            sw.WriteLine("Output = 0");
                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_29 + Negocio.IdiomaResxExtensao.LogProcesso_FalhaLeituraConteudoDat);
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                        if (this._parametros.CreateFileTmpUDCP == 1)
                        {
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_29 + Negocio.IdiomaResxExtensao.LogProcesso_FalhaLeituraConteudoDat);
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                        }

                        if (this._parametros.DelayUDCP > 0)
                        {
                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                        }
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                        {
                            sw.WriteLine("0");
                            sw.Close();
                        }
                    }

                    EncerrarOperacao(
                        descricaoDAT: Negocio.IdiomaResxExtensao.Global_Falha_Monitoramento,
                        log:
                            Negocio.IdiomaResxExtensao.Global_Falha_Monitoramento
                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado,
                        mensagemUsuario:
                            Negocio.IdiomaResxExtensao.Global_Falha_Monitoramento
                            + Environment.NewLine
                            + Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado);

                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog(ex.Message);
                    }

                    #endregion
                }
                finally
                {
                    #region Finally
                    this.menu = false;
                    if (dispenser != null)
                    {
                        dispenser.Disconnect();
                        //dispenser = null;
                    }
                    if (dispenser2 != null)
                    {
                        dispenser2.Disconnect();
                    }
                    if (modBusDispenser_P3 != null)
                    {
                        modBusDispenser_P3.Disconnect();
                        modBusDispenser_P3.Disconnect_Mover();
                    }

                    //ExecutarMonitoramento();

                    #endregion
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    this.menu = false;
            }
        }

        void RemoverLataUDCP(IDispenser disp)
        {
            //DateTime dtTempoRetirarLata = DateTime.Now;
            bool isLoop = true;
            try
            {
               
                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "msg.dat")))
                {
                    sw.WriteLine("[Msg]");
                    sw.WriteLine("Code = 86");
                    sw.WriteLine("DlgType = 4");
                    sw.WriteLine("Buttons = 0");
                    sw.WriteLine("Msg =  Remove the base can please...");
                    sw.Close();
                }
                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "msg.sig")))
                {
                    sw.WriteLine("[Msg]");
                    sw.WriteLine("Code = 86");
                    sw.WriteLine("DlgType = 4");
                    sw.WriteLine("Buttons = 0");
                    sw.WriteLine("Msg =  Remove the base can please...");
                    sw.Close();
                }
            
                bool existLastMSG = false;
                for (int i = 0; !existLastMSG && i < 5; i++)
                {
                    if (!File.Exists(Path.Combine(this.pathUDCP, "msg.sig")))
                    {
                        existLastMSG = true;
                    }
                    Thread.Sleep(1000);
                }
                if (existLastMSG)
                {
                    while (isLoop)
                    {
                        //TimeSpan? ts = DateTime.Now.Subtract(dtTempoRetirarLata);
                        //double tempoMin = ts.Value.TotalSeconds;
                        //ts = null;
                        //if (tempoMin > 30)
                        //{
                        //    break;
                        //}
                        if(!this.isThread)
                        {
                            break;
                        }
                        isLoop = Operar.TemRecipiente(disp, false);
                        Thread.Sleep(2000);
                    }
                }

                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "msg.dat")))
                {
                    sw.WriteLine("[Msg]");
                    sw.WriteLine("Code = 86");
                    sw.WriteLine("DlgType = -1");
                    sw.WriteLine("Buttons = 0");
                    sw.WriteLine("Msg =  Remove the base can please...");
                    sw.Close();
                }
                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "msg.sig")))
                {
                    sw.WriteLine("[Msg]");
                    sw.WriteLine("Code = 86");
                    sw.WriteLine("DlgType = -1");
                    sw.WriteLine("Buttons = 0");
                    sw.WriteLine("Msg =  Remove the base can please...");
                    sw.Close();
                }

                existLastMSG = false;
                for(int i = 0; !existLastMSG && i < 5; i++)
                {
                    if(!File.Exists(Path.Combine(this.pathUDCP, "msg.sig")))
                    {
                        existLastMSG = true;
                    }
                    Thread.Sleep(1000);
                }
                
                if (File.Exists(Path.Combine(this.pathUDCP, "reply.sig")))
                {
                    File.Delete(Path.Combine(this.pathUDCP, "reply.sig"));
                }
                if (File.Exists(Path.Combine(this.pathUDCP, "reply.dat")))
                {
                    File.Delete(Path.Combine(this.pathUDCP, "reply.dat"));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    Thread.Sleep(1000);
            }

            try
            {
                if(!isLoop)
                {
                    if (File.Exists(Path.Combine(this.pathUDCP, "reply.sig")))
                    {
                        File.Delete(Path.Combine(this.pathUDCP, "reply.sig"));
                    }
                    if (File.Exists(Path.Combine(this.pathUDCP, "reply.dat")))
                    {
                        File.Delete(Path.Combine(this.pathUDCP, "reply.dat"));
                    }
			    }
            
                if (File.Exists(Path.Combine(this.pathUDCP, "msg.dat")))
                {
                    File.Delete(Path.Combine(this.pathUDCP, "msg.dat"));
                }
                if (File.Exists(Path.Combine(this.pathUDCP, "msg.sig")))
                {
                    File.Delete(Path.Combine(this.pathUDCP, "msg.sig"));
                }
                
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        int gerarEventoDispensa(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Disspensa Produtos
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.DispensaProdutos;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private void ExecuteCheckFormula(Dictionary<int, double> demanda)
        {
            try
            {
                List<Util.ObjectColorante> excedentes = null;
                bool temCol = false;
                if (Operar.TemColoranteSuficiente(demanda, out excedentes))
                {
                    temCol = true;
                }
                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                {
                    sw.WriteLine("[General]");
                    string texto = "";
                    if (temCol)
                    {
                        sw.WriteLine("Result = 0");
                        texto = "Ok";
                    }
                    else
                    {
                        sw.WriteLine("Result = 1");
                        texto = Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;
                        bool primeiro = true;
                        foreach (Util.ObjectColorante colorante in excedentes)
                        {
                            texto += (primeiro ? "" : ", ") + colorante.Nome;
                            primeiro = false;
                        }
                    }
                    sw.WriteLine("Output = 0");
                    sw.WriteLine("MsgResult = " + texto);
                    WriteCanisterContents(sw, null, null);
                    sw.Close();
                }

                if (this._parametros.CreateFileTmpUDCP == 1)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                    {
                        sw.WriteLine("[General]");
                        string texto = "";
                        if (temCol)
                        {
                            sw.WriteLine("Result = 0");
                            texto = "Ok";
                        }
                        else
                        {
                            sw.WriteLine("Result = 1");
                            texto = Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;
                            bool primeiro = true;
                            foreach (Util.ObjectColorante colorante in excedentes)
                            {
                                texto += (primeiro ? "" : ", ") + colorante.Nome;
                                primeiro = false;
                            }
                        }
                        sw.WriteLine("Output = 0");
                        sw.WriteLine("MsgResult = " + texto);
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                }

                if (this._parametros.DelayUDCP > 0)
                {
                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                }
                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                {
                    sw.WriteLine("0");
                    sw.Close();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ExecuteExtKey(string _strExtKey)
        {
            try
            {
                if (_strExtKey.Contains("VERSION"))
                {
                    string nVersion = string.Empty;
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = " + VersaoSoftware.Replace(".", ""));
                        sw.WriteLine("[Version]");
                        sw.WriteLine("Text1 = Software Version: " + VersaoSoftware);
                       
                        try
                        {
                            switch ((Dispositivo)_parametros.IdDispositivo)
                            {
                                case Dispositivo.Placa_2:
                                    {
                                        ModBusDispenser_P2 mdp_2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                        nVersion = Operar.GetVersion(mdp_2);
                                        sw.WriteLine("Text2 = Firmware Version B1: " + nVersion);
                                        break;
                                    }
                            }

                            switch ((Dispositivo)_parametros.IdDispositivo2)
                            {
                                case Dispositivo.Placa_2:
                                    {
                                        ModBusDispenser_P2 mdp_2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                                        nVersion = Operar.GetVersion(mdp_2);
                                        sw.WriteLine("Text2 = Firmware Version B2: " + nVersion);
                                        break;
                                    }
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						}

						WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }

                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = " + VersaoSoftware.Replace(".", ""));
                            sw.WriteLine("[Version]");
                            sw.WriteLine("Text1 = Software Version " + VersaoSoftware);
                            if (!string.IsNullOrEmpty(nVersion))
                            {
                                sw.WriteLine("Text2 = Firmware Version " + nVersion);
                            }
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }
                    if(this._parametros.DelayUDCP> 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }

                }
                else if (_strExtKey.Contains("RESET#HARD"))
                {
                    //List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                    bool isDelay = false;
                    try
                    {
                        
                        switch ((Dispositivo)_parametros.IdDispositivo)
                        {
                            case Dispositivo.Placa_2:
                                {
                                    ModBusDispenser_P2 mdp_2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                    Operar.RessetHard(mdp_2, 1);
                                    isDelay = true;
                                    break;
                                }
                        }

                        switch ((Dispositivo)_parametros.IdDispositivo2)
                        {
                            case Dispositivo.Placa_2:
                                {
                                    ModBusDispenser_P2 mdp_2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                                    Operar.RessetHard(mdp_2, 2);
                                    isDelay = true;
                                    break;
                                }
                        }
                    }
					catch (Exception ex)
					{
						LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
					}

					if (isDelay)
                    {
                        Thread.Sleep(4000);
                    }
                }
                else if (_strExtKey.Contains("RESET"))
                {
                    //List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);                            
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }

                }
                else if (_strExtKey.Contains("SHUTDOWN"))
                {
                    //List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);                            
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                   
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                    this.isThread = false;
                    PausarMonitoramento();
                    //this.Close();
                }
                else if (_strExtKey.Contains("RESUME_FORMULA"))
                {
                    //List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);                            
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                else if(_strExtKey.Contains("SHOWSERVICEWINDOW"))
                {
                    //MenuConfig(null, null);
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }               
                else if (_strExtKey.Contains("MACHINE_INFO#FEATURES"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Output = 12345678");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("[Features]");

                        List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                        switch ((Dispositivo)_parametros.IdDispositivo)
                        {
                            case Dispositivo.Simulador:
                                {
                                    sw.WriteLine("Text1 = Type: Simulated dispenser");
                                    break;
                                }
                            case Dispositivo.Placa_1:
                                {
                                    sw.WriteLine("Text1 = Type: Mode dispenser");
                                    break;
                                }
                            case Dispositivo.Placa_2:
                                {
                                    sw.WriteLine("Text1 = Type: Mode dispenser");
                                    break;
                                }
                            case Dispositivo.Placa_3:
                                {
                                    sw.WriteLine("Text1 = Type: Mode dispenser");
                                    break;
                                }
                        }

                        sw.WriteLine("Text2 = Contains " + lCol.Count.ToString() + " Canisters");
                        sw.WriteLine("[ErrorLog]");
                        for (int i = 0; i < Modbus.Constantes.listLog_TXT.Count; i++)
                        {
                            sw.WriteLine("Error" + (i + 1).ToString() + " = " + Modbus.Constantes.listLog_TXT[i]);
                        }
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }

                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Output = 12345678");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("[Features]");

                            List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                            switch ((Dispositivo)_parametros.IdDispositivo)
                            {
                                case Dispositivo.Simulador:
                                    {
                                        sw.WriteLine("Text1 = Type: Simulated dispenser");
                                        break;
                                    }
                                case Dispositivo.Placa_1:
                                    {
                                        sw.WriteLine("Text1 = Type: Mode dispenser");
                                        break;
                                    }
                                case Dispositivo.Placa_2:
                                    {
                                        sw.WriteLine("Text1 = Type: Mode dispenser");
                                        break;
                                    }
                                case Dispositivo.Placa_3:
                                    {
                                        sw.WriteLine("Text1 = Type: Mode dispenser");
                                        break;
                                    }
                            }

                            sw.WriteLine("Text2 = Contains " + lCol.Count.ToString() + " Canisters");
                            sw.WriteLine("[ErrorLog]");
                            for (int i = 0; i < Modbus.Constantes.listLog_TXT.Count; i++)
                            {
                                sw.WriteLine("Error" + (i + 1).ToString() + " = " + Modbus.Constantes.listLog_TXT[i]);
                            }
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                else if (_strExtKey.Contains("MACHINE_INFO#CANISTERSIZE"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).OrderBy(o => o.Correspondencia).ToList();
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = " + lCol.Count.ToString());
                        WriteCanisterContents(sw, null, null);                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).OrderBy(o => o.Correspondencia).ToList();
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = " + lCol.Count.ToString());
                            WriteCanisterContents(sw, null, null);
                           
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                else if (_strExtKey.Contains("MACHINE_INFO#PUMPCOUNT"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = " + lCol.Count.ToString());
                        WriteCanisterContents(sw, null, null);
                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = " + lCol.Count.ToString());
                            WriteCanisterContents(sw, null, null);
                            
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }               
                else if (_strExtKey.Contains("MACHINE_INFO#CANISTERCOUNT"))
                {
                    List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");                       
                        sw.WriteLine("Result = 0");                        
                        sw.WriteLine("Output = " + lCol.Count.ToString());
                        WriteCanisterContents(sw, null, null);
                        
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = " + lCol.Count.ToString());
                            WriteCanisterContents(sw, null, null);
                            
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }

                    //Thread.Sleep(2000);
                }
                else if (_strExtKey.Contains("MACHINE_INFO#INTERRUPTEDFORMULA"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Output = 0");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("[InterruptedFormulaContents]");

                        List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                        foreach (Util.ObjectColorante _col in lCol)
                        {
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ml = " + _col.Volume.ToString());
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_size_ml = " + _col.NivelMaximo.ToString());
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_warn_ml = " + (((int)_col.NivelMinimo) + ((int)(_col.NivelMinimo * 0.1))).ToString());
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_min_ml = " + _col.NivelMinimo.ToString());
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ml = 0");
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_remaining_ml_ml = 0");
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ColorantName = " + _col.Nome);
                        }

                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Output = 0");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("[InterruptedFormulaContents]");

                            List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                            foreach (Util.ObjectColorante _col in lCol)
                            {
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ml = " + _col.Volume.ToString());
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_size_ml = " + _col.NivelMaximo.ToString());
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_warn_ml = " + (((int)_col.NivelMinimo) + ((int)(_col.NivelMinimo * 0.1))).ToString());
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_min_ml = " + _col.NivelMinimo.ToString());
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ml = 0");
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_remaining_ml_ml = 0");
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ColorantName = " + _col.Nome);
                            }

                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                else if (_strExtKey.Contains("PURGE#"))
                {
                    string[] array = _strExtKey.Split('#');
                
                    if (array.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(array[1]))
                        {
                            List<int> purge_cir = new List<int>();
                            for (int i = 1; i < array.Length; i++)
                            {
                                purge_cir.Add(Convert.ToInt32(array[i]));
                            }

                            int exec = ExecutarPurgaUDCP(purge_cir);
                            this._colorantes = Util.ObjectColorante.List();
                            if (exec == 0)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 0");
                                    sw.WriteLine("Output = 0" );
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                    WriteCanisterContents(sw, null, purge_cir);
                                    sw.Close();
                                }
                                if (this._parametros.CreateFileTmpUDCP == 1)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result=0");
                                        sw.WriteLine("Output=0");
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                        WriteCanisterContents(sw, null, purge_cir);
                                        sw.Close();
                                    }
                                }

                                if (this._parametros.DelayUDCP > 0)
                                {
                                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                }
                                Thread.Sleep(100);
                                Util.ObjectParametros.InitLoad();                                
                            }
                            else
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");                                    
                                    if(exec == 1)
                                    {
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                    }
                                    else if (exec == 2)
                                    {
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                    }
                                    else
                                    {
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                    }
                                    WriteCanisterContents(sw, null, null);
                                    sw.Close();
                                }

                                if (this._parametros.CreateFileTmpUDCP == 1)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 1");
                                        sw.WriteLine("Output = 0");
                                        if (exec == 1)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                        }
                                        else if (exec == 2)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                        }
                                        else
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                        }
                                        WriteCanisterContents(sw, null, null);                                  
                                        sw.Close();
                                    }
                                }

                                if (this._parametros.DelayUDCP > 0)
                                {
                                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                }
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }

                        }
                    }
                }
                else if (_strExtKey.Contains("PURGECOL#"))
                {
                    string[] array = _strExtKey.Split('#');
                    if (array.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(array[1]))
                        {
                            List<int> purge_cir = new List<int>();
                            for (int i = 1; i < array.Length; i++)
                            {
                                Util.ObjectColorante _col = this._colorantes.Find(o => o.Nome == array[i] && o.Habilitado && o.Seguidor == -1);
                                if (_col != null)
                                {
                                    purge_cir.Add(_col.Circuito);
                                }

                            }
                            if (purge_cir.Count > 0)
                            {
                                int exec = ExecutarPurgaUDCP(purge_cir);
                                List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                                this._colorantes = Util.ObjectColorante.List();
                                if (exec == 0)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 0");
                                        sw.WriteLine("Output = 0");
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                        WriteCanisterContents(sw, null, purge_cir);
                                        sw.Close();
                                    }
                                    if (this._parametros.CreateFileTmpUDCP == 1)
                                    {
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 0");
                                            sw.WriteLine("Output = 0");
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                            WriteCanisterContents(sw, null, purge_cir);
                                            sw.Close();
                                        }
                                    }

                                    if (this._parametros.DelayUDCP > 0)
                                    {
                                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 1");
                                        sw.WriteLine("Output = 0");
                                        if (exec == 1)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                        }
                                        else if (exec == 2)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                        }
                                        else
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                        }
                                        WriteCanisterContents(sw, null, null);
                                        sw.Close();
                                    }
                                    if (this._parametros.CreateFileTmpUDCP == 1)
                                    {
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 1");
                                            sw.WriteLine("Output = 0");
                                            if (exec == 1)
                                            {
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                            }
                                            else if (exec == 2)
                                            {
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                            }
                                            else
                                            {
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                            }
                                            WriteCanisterContents(sw, null, null);
                                            sw.Close();
                                        }
                                    }

                                    if (this._parametros.DelayUDCP > 0)
                                    {
                                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                    }
                                }
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                {
                                    sw.WriteLine("0");
                                    sw.Close();
                                }
                            }
                            else
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 0");
                                    sw.WriteLine("Output = 1");
                                    WriteCanisterContents(sw, null, null);
                                    sw.Close();
                                }
                                if (this._parametros.CreateFileTmpUDCP == 1)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 0");
                                        sw.WriteLine("Output = 1");
                                        WriteCanisterContents(sw, null, null);
                                        sw.Close();
                                    }
                                }

                                if (this._parametros.DelayUDCP > 0)
                                {
                                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                }
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                {
                                    sw.WriteLine("0");
                                    sw.Close();
                                }
                            }
                        }
                    }
                }
                else if (_strExtKey.Contains("PURGEVOL#"))
                {
                    string[] volumes = _strExtKey.Split('#');
                    if (volumes.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(volumes[1]))
                        {
                            Dictionary<int, double> qtdes = new Dictionary<int, double>();
                            for (int index = 1; index <= volumes.GetUpperBound(0); index += 2)
                            {
                                int circuito = Convert.ToInt32(volumes[index]);
                                double vol_d = double.Parse(volumes[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                                qtdes.Add(circuito, vol_d);
                            }
                            int exec = ExecutarPurgaUDCP(qtdes);
                            this._colorantes = Util.ObjectColorante.List();
                            List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                            if (exec == 0)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 0");
                                    sw.WriteLine("Output = 0" );
                                    sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                    WriteCanisterContents(sw, qtdes, null);
                                    sw.Close();
                                }
                                if (this._parametros.CreateFileTmpUDCP == 1)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 0");
                                        sw.WriteLine("Output = 0");
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                        WriteCanisterContents(sw, qtdes, null);
                                        sw.Close();
                                    }
                                }

                                if (this._parametros.DelayUDCP > 0)
                                {
                                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                }
                            }
                            else
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0" );
                                    if (exec == 1)
                                    {
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                    }
                                    else if (exec == 2)
                                    {
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                    }
                                    else
                                    {
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                    }
                                    WriteCanisterContents(sw, null, null);
                                    sw.Close();
                                }

                                if (this._parametros.CreateFileTmpUDCP == 1)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 1");
                                        sw.WriteLine("Output = 0");
                                        if (exec == 1)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                        }
                                        else if (exec == 2)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                        }
                                        else
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                        }
                                        WriteCanisterContents(sw, null, null);
                                        sw.Close();
                                    }
                                }

                                if (this._parametros.DelayUDCP > 0)
                                {
                                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                }
                            }
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                            {
                                sw.WriteLine("0");
                                sw.Close();
                            }
                        }
                    }
                }
                else if (_strExtKey.Contains("PURGECOLVOL#"))
                {
                    string[] volumes = _strExtKey.Split('#');
                    if (volumes.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(volumes[1]))
                        {
                            Dictionary<int, double> qtdes = new Dictionary<int, double>();
                            List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                            for (int index = 1; index <= volumes.GetUpperBound(0); index += 2)
                            {
                                Util.ObjectColorante _col = lCol.Find(o => o.Nome == volumes[index]);
                                if (_col != null)
                                {
                                    int circuito = _col.Circuito;
                                    double vol_d = double.Parse(volumes[index + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                                    qtdes.Add(circuito, vol_d);
                                }
                            }
                            if (qtdes.Count > 0)
                            {
                                int exec = ExecutarPurgaUDCP(qtdes);
                                this._colorantes = Util.ObjectColorante.List();
                                lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                                if (exec == 0)
                                {                                    
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 0");
                                        sw.WriteLine("Output = 0");
                                        sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                        WriteCanisterContents(sw, qtdes, null);
                                        sw.Close();

                                    }

                                    if (this._parametros.CreateFileTmpUDCP == 1)
                                    {
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 0");
                                            sw.WriteLine("Output = 0");
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_15 + Negocio.IdiomaResxExtensao.Global_PurgaConcluida);
                                            WriteCanisterContents(sw, qtdes, null);
                                            sw.Close();
                                        }
                                    }

                                    if (this._parametros.DelayUDCP > 0)
                                    {
                                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 1");
                                        sw.WriteLine("Output = 0" );
                                        if (exec == 1)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                        }
                                        else if (exec == 2)
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                        }
                                        else
                                        {
                                            sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                        }
                                        WriteCanisterContents(sw, null, null);
                                        sw.Close();
                                    }

                                    if (this._parametros.CreateFileTmpUDCP == 1)
                                    {
                                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                        {
                                            sw.WriteLine("[General]");
                                            sw.WriteLine("Result = 1");
                                            sw.WriteLine("Output = 0");
                                            if (exec == 1)
                                            {
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_16 + Negocio.IdiomaResxExtensao.Global_PurgaCancelada);
                                            }
                                            else if (exec == 2)
                                            {
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_17 + Negocio.IdiomaResxExtensao.Global_PurgaAbortada);
                                            }
                                            else
                                            {
                                                sw.WriteLine("MsgResult = " + Negocio.IdiomaResxExtensao.Log_Cod_18 + Negocio.IdiomaResxExtensao.Global_FalhaPurgar);
                                            }
                                            WriteCanisterContents(sw, null, null);
                                            sw.Close();
                                        }
                                    }

                                    if (this._parametros.DelayUDCP > 0)
                                    {
                                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                    }
                                }
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                {
                                    sw.WriteLine("0");
                                    sw.Close();
                                }
                            }
                            else
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                                {
                                    sw.WriteLine("[General]");
                                    sw.WriteLine("Result = 1");
                                    sw.WriteLine("Output = 0");
                                    WriteCanisterContents(sw, null, null);
                                    sw.Close();
                                }

                                if (this._parametros.CreateFileTmpUDCP == 1)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                                    {
                                        sw.WriteLine("[General]");
                                        sw.WriteLine("Result = 1");
                                        sw.WriteLine("Output = 0");
                                        WriteCanisterContents(sw, null, null);
                                        sw.Close();
                                    }
                                }

                                if (this._parametros.DelayUDCP > 0)
                                {
                                    Thread.Sleep(this._parametros.DelayUDCP * 1000);
                                }

                                using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                                {
                                    sw.WriteLine("0");
                                    sw.Close();
                                }
                            }
                        }
                    }
                }
                else if (_strExtKey.Contains("FILLCANISTER#"))
                {
                    List<string> listStrExtKey = new List<string>();
                    string trabalho = _strExtKey;
                    while (trabalho.Contains("FILLCANISTER#"))
                    {
                        int index_ini = 0;
                        int index_fim = trabalho.IndexOf("FILLCANISTER#");
                        listStrExtKey.Add(trabalho.Substring(index_ini, index_fim));
                        trabalho = trabalho.Substring(index_fim + 13);
                    }
                    if (trabalho.Length > 0)
                    {
                        listStrExtKey.Add(trabalho);
                    }

                    bool executarCmd = true;
                    string msgerror = "";
                    foreach (string _str in listStrExtKey)
                    {
                        if (_str != null && _str.Contains("#"))
                        {
                            string[] array = _str.Split('#');
                            if (array[0] == "ADD")
                            {
                                int circuito = Convert.ToInt32(array[1]);
                                double vol_d = double.Parse(array[2].Replace(",", "."), CultureInfo.InvariantCulture);
                                Util.ObjectColorante _col = this._colorantes.Find(o => o.Circuito == circuito && o.Habilitado && o.Seguidor == -1);
                                if (_col != null)
                                {
                                    vol_d += _col.Volume;
                                    if (vol_d > _col.NivelMaximo)
                                    {
                                        executarCmd = false;

                                        msgerror += "#Canister " + circuito + " " + Negocio.IdiomaResxExtensao.Nivel_Informacao_ExcedeNivelMaximo;
                                    }
                                }
                                else
                                {
                                    executarCmd = false;
                                    msgerror += "#Canister " + circuito + " invalid";
                                }
                            }
                            else if (array[0] == "SET")
                            {
                                int circuito = Convert.ToInt32(array[1]);
                                double vol_d = double.Parse(array[2].Replace(",", "."), CultureInfo.InvariantCulture);
                                Util.ObjectColorante _col = this._colorantes.Find(o => o.Circuito == circuito && o.Habilitado && o.Seguidor == -1);
                                if (_col != null)
                                {
                                    if (vol_d > _col.NivelMaximo)
                                    {
                                        executarCmd = false;
                                        msgerror += "#Canister " + circuito + " " + Negocio.IdiomaResxExtensao.Nivel_Informacao_ExcedeNivelMaximo;
                                    }
                                }
                                else
                                {
                                    executarCmd = false;
                                    msgerror += "#Canister " + circuito + " invalid";
                                }
                            }

                        }
                    }

                    if (executarCmd)
                    {
                        foreach (string _str in listStrExtKey)
                        {
                            if (_str != null && _str.Contains("#"))
                            {
                                string[] array = _str.Split('#');
                                if (array[0] == "ADD")
                                {
                                    int circuito = Convert.ToInt32(array[1]);
                                    double vol_d = double.Parse(array[2].Replace(",", "."), CultureInfo.InvariantCulture);
                                    Util.ObjectColorante _col = this._colorantes.Find(o => o.Circuito == circuito);
                                    if (_col != null)
                                    {
                                        vol_d += _col.Volume;
                                        if (vol_d <= _col.NivelMaximo)
                                        {
                                            _col.Volume = vol_d;
                                            Util.ObjectColorante.Persist(_col);
                                        }
                                    }
                                }
                                else if (array[0] == "SET")
                                {
                                    int circuito = Convert.ToInt32(array[1]);
                                    double vol_d = double.Parse(array[2].Replace(",", "."), CultureInfo.InvariantCulture);
                                    Util.ObjectColorante _col = this._colorantes.Find(o => o.Circuito == circuito);
                                    if (_col != null)
                                    {
                                        if (vol_d <= _col.NivelMaximo)
                                        {
                                            _col.Volume = vol_d;
                                            Util.ObjectColorante.Persist(_col);
                                        }
                                    }
                                }

                            }
                        }


                        this._colorantes = Util.ObjectColorante.List();
                        List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();

                        string detalhesCol = "";
                        foreach (Util.ObjectColorante _col in lCol)
                        {
                            if (detalhesCol == "")
                            {
                                detalhesCol += _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                            }
                            else
                            {
                                detalhesCol += "," + _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                            }
                        }
                        gerarEventoAbastecimento(0, detalhesCol);

                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                        if (this._parametros.CreateFileTmpUDCP == 1)
                        {
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 0");
                                sw.WriteLine("Output = 0");
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                        }

                        if (this._parametros.DelayUDCP > 0)
                        {
                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                        }
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                        {
                            sw.WriteLine("0");
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 1");
                            sw.WriteLine("Output = 0");
                            sw.WriteLine("MsgResult = " + msgerror);
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                        if (this._parametros.CreateFileTmpUDCP == 1)
                        {
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                sw.WriteLine("MsgResult = " + msgerror);
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                        }

                        if (this._parametros.DelayUDCP > 0)
                        {
                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                        }

                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                        {
                            sw.WriteLine("0");
                            sw.Close();
                        }
                    }
                }
                else if (_strExtKey.Contains("SETCANCOLNAME#"))
                {
                    List<string> listStrExtKey = new List<string>();
                    string trabalho = _strExtKey;
                    while (trabalho.Contains("SETCANCOLNAME#"))
                    {
                        int index_ini = 0;
                        int index_fim = trabalho.IndexOf("SETCANCOLNAME#");
                        listStrExtKey.Add(trabalho.Substring(index_ini, index_fim));
                        trabalho = trabalho.Substring(index_fim + 14);
                    }
                    if (trabalho.Length > 0)
                    {
                        listStrExtKey.Add(trabalho);
                    }
                    bool att = false;
                    foreach (string _str in listStrExtKey)
                    {
                        if (_str != null && _str.Contains("#"))
                        {
                            string[] array = _str.Split('#');
                            int circuit = Convert.ToInt32(array[0]);
                            Util.ObjectColorante _col = this._colorantes.Find(o => o.Circuito == circuit);
                            if(_col!= null)
                            {
                                att = true;
                                _col.Nome = array[1];
                                Util.ObjectColorante.Persist(_col);
                            }
                        }
                    }
                    if(att)
                    {
                        this._colorantes = Util.ObjectColorante.List();
                        List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();

                        string detalhesCol = "";
                        foreach (Util.ObjectColorante _col in lCol)
                        {
                            if (detalhesCol == "")
                            {
                                detalhesCol += _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.MassaEspecifica, 3).ToString();
                            }
                            else
                            {
                                detalhesCol += "," + _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.MassaEspecifica, 3).ToString();
                            }
                        }
                        gerarEventoAlteradoProdutos(0, detalhesCol);

                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                        if (this._parametros.CreateFileTmpUDCP == 1)
                        {
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 0");
                                sw.WriteLine("Output = 0");
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                        }

                        if (this._parametros.DelayUDCP > 0)
                        {
                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                        }
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                        {
                            sw.WriteLine("0");
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 1");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                        if (this._parametros.CreateFileTmpUDCP == 1)
                        {
                            using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                            {
                                sw.WriteLine("[General]");
                                sw.WriteLine("Result = 1");
                                sw.WriteLine("Output = 0");
                                WriteCanisterContents(sw, null, null);
                                sw.Close();
                            }
                        }

                        if (this._parametros.DelayUDCP > 0)
                        {
                            Thread.Sleep(this._parametros.DelayUDCP * 1000);
                        }
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                        {
                            sw.WriteLine("0");
                            sw.Close();
                        }
                    }
                }
                /*Agitador*/
                else if (_strExtKey.Contains("STIR#") || _strExtKey.Contains("STIR"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                /*Agitador*/
                else if (_strExtKey.Contains("STIRCOL#") || _strExtKey.Contains("STIRCOL"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }                
                /*Clean*/
                else if (_strExtKey.Contains("CLEAN#") || _strExtKey.Contains("CLEAN"))
                {                       
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                /*Clean*/
                else if (_strExtKey.Contains("CLEANCOL#") || _strExtKey.Contains("CLEANCOL"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                /*PNC*/
                else if (_strExtKey.Contains("PNC#") || _strExtKey.Contains("PNC"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                /*SHL*/
                else if (_strExtKey.Contains("SHL#") || _strExtKey.Contains("SHL"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                /*IDONOTEXIST*/
                else if (_strExtKey.Contains("IDONOTEXIST#") || _strExtKey.Contains("IDONOTEXIST"))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.dat")))
                    {
                        sw.WriteLine("[General]");
                        sw.WriteLine("Result = 0");
                        sw.WriteLine("Output = 0");
                        WriteCanisterContents(sw, null, null);
                        sw.Close();
                    }
                    if (this._parametros.CreateFileTmpUDCP == 1)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result_" + this._parametros.ExtFileTmpUDCP + ".dat")))
                        {
                            sw.WriteLine("[General]");
                            sw.WriteLine("Result = 0");
                            sw.WriteLine("Output = 0");
                            WriteCanisterContents(sw, null, null);
                            sw.Close();
                        }
                    }

                    if (this._parametros.DelayUDCP > 0)
                    {
                        Thread.Sleep(this._parametros.DelayUDCP * 1000);
                    }
                    using (StreamWriter sw = new StreamWriter(Path.Combine(this.pathUDCP, "result.sig")))
                    {
                        sw.WriteLine("0");
                        sw.Close();
                    }
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        int gerarEventoAbastecimento(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Abastecimento
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.Abastecimento;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        int gerarEventoAlteradoProdutos(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Alterado Produtos
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.AlteradoProdutos;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private void WriteCanisterContents(StreamWriter sw, Dictionary<int, double> _demanda, List<int> purge_cir)
        {
            try
            {
                List<Util.ObjectColorante> lCol = this._colorantes.FindAll(o => o.Habilitado && o.Seguidor == -1).ToList();
                sw.WriteLine("[CanisterContents]");
                foreach (Util.ObjectColorante _col in lCol)
                {
                    if (_col.Habilitado && _col.Seguidor == -1)
                    {
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ml = " + ((int)_col.Volume).ToString());                        
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_size_ml = " + ((int)_col.NivelMaximo).ToString());
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_min_ml = " + ((int)_col.NivelMinimo).ToString());
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_warn_ml = " + ((int)(_col.NivelMaximo / 3)).ToString());
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ColorantName = " + _col.Nome);
                        if (purge_cir != null && purge_cir.Count > 0)
                        {
                            bool achou_can = false;
                            foreach (int _c_purga in purge_cir)
                            {
                                if (_c_purga == _col.Correspondencia || _c_purga == 0)
                                {
                                    achou_can = true;
                                    break;
                                }
                            }
                            if (!achou_can)
                            {
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ul = 0");
                            }
                            else
                            {
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ul = 500");
                            }
                        }
                        else if (_demanda != null && _demanda.Count > 0)
                        {
                            bool achou_can = false;
                            double vol_dosed = 0;
                            foreach (KeyValuePair<int, double> item in _demanda)
                            {
                                int circuito = item.Key;
                                if (circuito == _col.Correspondencia)
                                {
                                    achou_can = true;
                                    vol_dosed = item.Value;
                                    break;
                                }
                            }
                            if (!achou_can)
                            {
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ul = 0");
                            }
                            else
                            {
                                sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ul = " + ((int)(vol_dosed * 1000)).ToString());
                            }
                        }
                        else
                        {
                            sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ul = 0");
                        }
                    }
                    else
                    {
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ml = -1");
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_size_ml = -1");
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_warn_ml = -1");
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_min_ml = -1");
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_ColorantName = " + _col.Nome);
                        sw.WriteLine("Canister_" + _col.Correspondencia.ToString() + "_dosed_ul = -1");
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void VerificarBaseDados(string[] filesDat, string pathMonitFD)
        {
            try
            {
                List<DatIOConnectTO> lDatDB = new List<DatIOConnectTO>();
				using HttpClient client = new HttpClient();

				try
                {
                    string text = "";
                    MyModel adloc = new MyModel();
                    MyModel adlocReturn = new MyModel();
                    adloc.Output = "0";
                    adloc.User = "adm";
                    adloc.Pass = "adm2019";
					
					string strParameter = JsonConvert.SerializeObject(adloc);
					string urlReq = _parametros.PathBasesDados + "DatIOConnect/DatIOConnect_GetIOConnect/";

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					HttpContent content = new StringContent(strParameter, Encoding.UTF8, "application/json");
					HttpResponseMessage response = client.PostAsync(urlReq, content).Result;

					if (response.IsSuccessStatusCode)
					{
						text = response.Content.ReadAsStringAsync().Result;
					}

					if (text != null && text.Length > 1)
                    {
                        adlocReturn = JsonConvert.DeserializeObject<MyModel>(text);
                        var serializedResult = JsonConvert.SerializeObject(adlocReturn.ListObjetoSaida);
                        lDatDB = JsonConvert.DeserializeObject<List<DatIOConnectTO>>(serializedResult);
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}

				if (lDatDB != null && lDatDB.Count > 0)
                {
                    foreach (DatIOConnectTO _datIoCon in lDatDB)
                    {
                        bool achou_dat = false;
                        if (filesDat != null && filesDat.Length > 0)
                        {
                            foreach (string fDat in filesDat)
                            {
                                string _strCmp = _datIoCon.NomeDat + "-" + _datIoCon.Id;
                                if (fDat.Contains(_strCmp))
                                {
                                    achou_dat = true;
                                    break;
                                }
                            }
                        }
                        if (!achou_dat)
                        {
                            string pathNomeFile = pathMonitFD + _datIoCon.NomeDat + "-" + _datIoCon.Id + ".dat";
                            using (StreamWriter sW = new StreamWriter(pathNomeFile, true, Encoding.GetEncoding("ISO-8859-1")))
                            {
                                sW.WriteLine(_datIoCon.ValoresDat.Replace(";", ","));
                                sW.Close();
                            }
                        }

                        string text = "";
                        MyModel adloc = new MyModel();
                        MyModel adlocReturn = new MyModel();
                        adloc.Output = "0";
                        adloc.User = "adm";
                        adloc.Pass = "adm2019";
                        _datIoCon.Executado = "1";

                        adloc.ObjetoEntrada = _datIoCon;

						string strParameter = JsonConvert.SerializeObject(adloc);
						string urlReq = _parametros.PathBasesDados + "DatIOConnect/DatIOConnect_UpDate/";

						client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						HttpContent content = new StringContent(strParameter, Encoding.UTF8, "application/json");
						HttpResponseMessage response = client.PostAsync(urlReq, content).Result;

						if (response.IsSuccessStatusCode)
						{
							text = response.Content.ReadAsStringAsync().Result;
						}

                        if (text != null && text.Length > 1)
                        {
                            adlocReturn = JsonConvert.DeserializeObject<MyModel>(text);
                            if (adlocReturn.Output == "1;OK;")
                            {
                                var serializedResult = JsonConvert.SerializeObject(adlocReturn.ListObjetoSaida);
                                string retorno = JsonConvert.DeserializeObject<List<string>>(serializedResult)[0];
                                if (retorno == "1")
                                {

                                }
                            }
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			this.dtMonitoramentoFilaBaseDados = DateTime.Now;
        }      

        #region Menu

        enum OpcaoMenu
        {
            Configuracoes = 1,
            FormulaPersonalizada = 2,
            Purgar = 3,
            GerenciarNivel = 4,
            Sair = 5,
            Sobre = 6,
            Treinamento = 7,
            MonitCircuitos = 8,
            PurgarIndividual = 9,
            SincronismoFormula = 10,
            UploadFormula = 11,
            GerenciadorFilaDat = 12,
            Precisao = 13,
            MenuAutomaticDispensa = 14,
            Recircular = 15,
            PlacaMov = 16,
            RecircularAutomatica = 17,
            LimpezaBico = 18,
            LogSerial = 19,
            SairClick = 20,
            LogBD = 21,
            ConnectPlaca = 22
        }

        private void _notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            this.isNotifyMouseUp = true;
            //[Implementa clique com o botão esquerdo/apenas um clique no touch]
            CreateMenuDinamicoIdioma(false);

            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            }
        }

        void ShowMenu(OpcaoMenu opcao)
        {
            //Ao acessar interface através do menu o monitoramente deve ser pausado
            //PausarMonitoramento();

            try
            {
                switch (opcao)
                {
                    #region Executa funcionalidades

                    case OpcaoMenu.Configuracoes:
                        {
                            #region Configurações
                            this.menu = true;

                            Authentication authentication = null;
                            Util.ObjectUser _usuario = null;

                            using (fAutenticacao f = new fAutenticacao())
                            {
                                f.ShowDialog();
                                authentication = f.Authentication;
                                _usuario = f.Usuario;
                            }

                            if (authentication != null && _usuario != null)
                            {
                                using (Form f = new fConfiguracoes(authentication, _usuario))
                                {
                                    f.ShowDialog();
                                }

                                Util.ObjectParametros.InitLoad();

                                //[Atualiza parâmetros e colorantes utilizados no monitoramento]
                                _parametros = Util.ObjectParametros.Load();
                                this.isControlePlacaMovAlerta = getControlePlacaMov();
                                this.isControleEsponja = _parametros.HabilitarIdentificacaoCopo;                                
                                this.dtControleEsponja = DateTime.Now;

                                this.isHabLimpBicos = _parametros.HabLimpBicos;
                                this.dtControleLimpBicos = DateTime.Now;

                                _colorantes = Util.ObjectColorante.List();
                                this.listLimpBicosConfig = Util.ObjectLimpBicos.List();
                                //[Aplica  controle de acesso ao menus]
                                ControlarAcessoMenu();


                                #region adicionar novo controle de monitoramento dos circuitos
                                _parametros = Util.ObjectParametros.Load();
                                //transformar de minutos para segundos
                                this.timerDelayMonit = _parametros.MonitTimerDelay * 60;
                                this.timerIniDelayMonit = _parametros.MonitTimerDelayIni * 60;
                                this.isMonitCircuitos = !_parametros.DesabilitarProcessoMonitCircuito;
                                Constants.countTimerDelayMonit = 0;
                                #endregion

                                #region parametros Producao

                                this.transmitiuProgProducao = false;
                                this.isHDProducao = false;
                                this.dtMonitProducao = DateTime.Now;
                                if (_parametros.TipoProducao.Contains("Prog"))
                                {
                                    string tpProd = _parametros.TipoProducao.Replace("Prog", "").Replace(" ", "").Replace("HS", "");
                                    int hour = Convert.ToInt32(tpProd);
                                    this.tsProgProducao = new TimeSpan(hour, 0, 0);
                                }
                                else
                                {
                                    if (_parametros.TipoProducao.Contains("OnLine"))
                                    {
                                        this.isProgProducao = false;
                                        this.timerDelayMonitProducao = 0;
                                    }
                                    else if (_parametros.TipoProducao.Contains("HD"))
                                    {
                                        this.isHDProducao = true;
                                        this.timerDelayMonitProducao = 1;

                                    }
                                    else if (_parametros.TipoProducao.Contains("HS"))
                                    {
                                        string tpProd = _parametros.TipoProducao.Replace("HS", "");
                                        this.timerDelayMonitProducao = Convert.ToInt32(tpProd) * 60;

                                    }

                                }
                                this.timerMonitFilaDat = _parametros.DelayMonitoramentoFilaDAT * 60;
                                if (this.timerMonitFilaDat < 60)
                                {
                                    this.timerMonitFilaDat = 60;
                                }

                                this.isMonitProducao = !_parametros.DesabilitaMonitProcessoProducao;
                                #endregion
                                this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
                                this._listaRecircularAuto = Util.ObjectRecircular.List().Where(o => o.Habilitado && o.isAuto).ToList();

                                #region parametros Eventos

                                this.transmitiuProgEventos= false;
                                this.isHDEventos = false;
                                this.dtMonitEventos = DateTime.Now;
                                if (_parametros.TipoEventos.Contains("Prog"))
                                {
                                    string tpProd = _parametros.TipoEventos.Replace("Prog", "").Replace(" ", "").Replace("HS", "");
                                    int hour = Convert.ToInt32(tpProd);
                                    this.tsProgEventos = new TimeSpan(hour, 0, 0);
                                }
                                else
                                {
                                    if (_parametros.TipoEventos.Contains("OnLine"))
                                    {
                                        this.isProgEventos = false;
                                        this.timerDelayMonitEventos = 0;
                                    }
                                    else if (_parametros.TipoEventos.Contains("HD"))
                                    {
                                        this.isHDEventos = true;
                                        this.timerDelayMonitEventos = 1;

                                    }
                                    else if (_parametros.TipoEventos.Contains("HS"))
                                    {
                                        string tpProd = _parametros.TipoEventos.Replace("HS", "");
                                        this.timerDelayMonitEventos = Convert.ToInt32(tpProd) * 60;

                                    }

                                }

                                this.isMonitEventos = !_parametros.DesabilitaMonitSyncToken;
                                //this.timerDelayMonitEventos = 10;

                                #endregion

                                #region Bkp Calibragem
                                this.isMonitBkpCalibragem = !_parametros.DesabilitaMonitSyncBkpCalibragem;
                                this.timerDelayMonitBkpCalibragem = 60;
                                #endregion
                            }
                            #endregion


                            break;
                        }
                    case OpcaoMenu.FormulaPersonalizada:
                        {
                            this.menu = true;
                            using (Form f = new fGerenciarFormula())
                                f.ShowDialog();


                            break;
                        }
                    case OpcaoMenu.Purgar:
                        {
                            this.menu = true;
                            Log.Logar(
                                TipoLog.Processo,
                                _parametros.PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Global_SelecionadoMenuPurga);


                            bool execPurga = ExecutarPurga();
                            if(execPurga)
                            {
                                setCondicaoMaquinaLigada(true);                               
                                getRecirculacao(true);
                                getRecirculacaoAuto(true);
                                getLimpezaBico(true);
                            }
                            Thread.Sleep(100);
                            Util.ObjectParametros.InitLoad();
                            this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
                            RessetarTempoMonitoramento();
                            

                            break;
                        }
                    case OpcaoMenu.PurgarIndividual:
                        {
                            this.menu = true;
                            Log.Logar(
                                TipoLog.Processo,
                                _parametros.PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Global_SelecionadoMenuPurgaIndividual);

                            ExecutarPurgaIndividual();
                            RessetarTempoMonitoramento();


                            break;
                        }
                    case OpcaoMenu.GerenciarNivel:
                        {
                            this.menu = true;
                            using (Form f = new fGerenciarNivel())
                                f.ShowDialog();

                            break;
                        }
                    case OpcaoMenu.Treinamento:
                        {
                            this.menu = true;
                            if (!TreinamentoHelper.RunTreinamento())
                            {
                                #region Valida execução do treinamento

                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    m.ShowDialog(
                                        Negocio.IdiomaResxExtensao.Global_Treinamento_NaofoiPossivelIniciar);
                                }

                                #endregion
                            }

                            break;
                        }
                    case OpcaoMenu.Sobre:
                        {
                            this.menu = true;
                            using (Form f = new fSobre())
                            {
                                f.ShowDialog();
                                _parametros = Util.ObjectParametros.Load();
                                this.isControlePlacaMovAlerta = getControlePlacaMov();
                                CreateMenuDinamicoIdioma();
                                #region identify click
                                if (this._parametros.PathMonitoramentoDAT.Contains("DosadoraPercolore_zhm69scv2n72e"))
                                {
                                    string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                                    string strPath = "";
                                    for (int i = 0; i < arrayStr.Length - 1; i++)
                                    {
                                        strPath += arrayStr[i] + Path.DirectorySeparatorChar;
                                    }
                                    //CLICK_PURGA_INDIVIDUAL = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Local\\Packages\\DosadoraPercolore_zhm69scv2n72e\\LocalState\\click.purgarindividual");
                                    CLICK_PURGA = strPath + "click.purgar";
                                    CLICK_PURGA_INDIVIDUAL = strPath + "click.purgarindividual";
                                    CLICK_GERENCIAR_VOLUME = strPath + "click.gerenciarnivel";
                                    CLICK_SINC_FORMULA = strPath + "click.SincFormula";
                                    CLICK_Notificacao = strPath + "click.Notificacao";
                                    CLICK_UPLOAD_FORMULA = strPath + "UpFormula.json";
                                    CLICK_PROGRESS = strPath + "click.PROGRESS";
                                    CLICK_Precisao = strPath + "accurracy.json";
                                    CLICK_Precisao_Up = strPath + "accurracyUp.json";
                                }
                                #endregion
                            }
                            break;
                        }
                    case OpcaoMenu.Sair:
                        {
                            bool confirma = false;
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                            {
                                confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Sair_Solucao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                            }
                            if (confirma)
                            {
                                this.isThread = false;
                                PausarMonitoramento();
                                if(this.fLog != null)
                                {
                                    this.fLog.Close();
                                }
                                
                                #region gravar Evento Fechar
                                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                                objEvt.DATAHORA = DateTime.Now;
                                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.FecharSistema;
                                objEvt.INTEGRADO = false;
                                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                                {
                                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                                }
                                Util.ObjectEventos.InsertEvento(objEvt);
                                #endregion

                                this.Close();
                            }

                            break;
                        }
                    case OpcaoMenu.MonitCircuitos:
                        {
                            this.menu = true;
                            Log.Logar(
                                    TipoLog.Processo,
                                    _parametros.PathLogProcessoDispensa,
                                    Negocio.IdiomaResxExtensao.Global_Inic_Monitoramento_Circuitos);
                            bool execMonit = ExecutarMonitoramentoCircuitos();
                            if (execMonit)
                            {
                                setCondicaoMaquinaLigada(true);
                            }
                            RessetarTempoMonitoramento();
                            break;
                        }
                    case OpcaoMenu.SincronismoFormula:
                        {
                            this.menu = true;
                            ExecutarSincronismoFormula(1);
                            break;
                        }
                    case OpcaoMenu.UploadFormula:
                        {
                            this.menu = true;
                            ExecutarSincronismoFormula(2);
                            break;
                        }

                    case OpcaoMenu.GerenciadorFilaDat:
                        {
                            this.menu = true;
                            if (!ExecutarGerenciadorFilaDat())
                            {
                                this.dtMonitoramentoFilaDat = DateTime.Now;
                            }
                            else
                            {
                                this.dtMonitoramentoFilaDat = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, this.timerMonitFilaDat, 0));
                            }
                        }
                        break;
                    case OpcaoMenu.Precisao:
                        {
                            this.menu = true;
                            ExecutarPrecisao();
                        }
                        break;
                    case OpcaoMenu.MenuAutomaticDispensa:
                        {
                            this.menu = true;
                            ExecutarMenuAutomaticDispensa();
                        }
                        break;
                    case OpcaoMenu.Recircular:
                        {
                            this.menu = true;
                            ExecutarRecircular();
                            this.dtRecircular = DateTime.Now;
                            this.dtRecircularPurga = DateTime.Now;
                        }
                        break;
                    case OpcaoMenu.PlacaMov:
                        {
                            this.menu = true;
                            ExecutarPlacaMov();
                            break;
                        }
                    case OpcaoMenu.RecircularAutomatica:
                        {
                            this.menu = true;
                            ExecutarRecircularAutomatica();
                            Util.ObjectParametros.InitLoad();
                            this.dtRecircularAuto = DateTime.Now;
                            this.qtdTentativasRecirculaAUto = 0;
                            this._listaRecircularAuto = Util.ObjectRecircular.List().Where(o => o.Habilitado && o.isAuto).ToList();
                            break;
                        }
                    case OpcaoMenu.LimpezaBico:
                        {
                            this.menu = true;
                            ExecutarLimpezaBico();
                            RessetarTempoMonitoramento();
                            break;
                        }
                    case OpcaoMenu.LogSerial:
                        {
                            this.menu = true;
                            ExecutarLogSerial();
                            break;
                        }
                    case OpcaoMenu.SairClick:
                        {   
                            this.isThread = false;
                            PausarMonitoramento();
                            if (this.fLog != null)
                            {
                                this.fLog.Close();
                            }
                            
                            #region gravar Evento Fechar
                            Util.ObjectEventos objEvt = new Util.ObjectEventos();
                            objEvt.DATAHORA = DateTime.Now;
                            objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.FecharSistema;
                            objEvt.INTEGRADO = false;
                            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                            {
                                objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                            }
                            Util.ObjectEventos.InsertEvento(objEvt);
                            #endregion

                            this.Close();
                            
                            break;
                        }
                    case OpcaoMenu.LogBD:
                        {
                            bool logDB = false;                                
                            DateTime? dtFimValLog = null;
                            using (StreamReader reader = new StreamReader(this.CLICK_LOGBD))
                            {
                                string line = "";
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line.Contains("="))
                                    {
                                        string[] array = line.Split('=');
                                        if(array[0] == "LogBD")
                                        {
                                            if(array[1] =="1" || array[1].ToUpper() == "TRUE")
                                            {
                                                logDB = true;
                                            }
                                        }
                                        else if(array[0] == "dataFimValLog")
                                        {
                                            dtFimValLog = Convert.ToDateTime(array[1]);
                                        }
                                    }
                                }
                                     
                                reader.Close();
                            }
                            if(dtFimValLog!= null)
                            {
                                if(dtFimValLog.Value.CompareTo( DateTime.Now) > 0)
                                {
                                    _parametros.LogBD = logDB;
                                    Util.ObjectParametros.Persist(_parametros);
                                    Util.ObjectParametros.InitLoad();
                                    _parametros = Util.ObjectParametros.Load();
                                    if(logDB)
                                    {
                                        using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                        {
                                            message.ShowDialog("Log by BD", Negocio.IdiomaResxExtensao.Global_Sim, null, true, 10);
                                        }
                                    }
                                    else
                                    {
                                        using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                        {
                                            message.ShowDialog("Log by TXT", Negocio.IdiomaResxExtensao.Global_Sim, null, true, 10);
                                        }
                                    }
                                }
                            }
                            dtFimValLog = null;

                            break;
                        }
                    default:
                        {
                            break;
                        }
                        #endregion
                }
            }
            finally
            {
                this.menu = false;
                this.dtMonitoramentoFilaDat = DateTime.Now.Subtract(new TimeSpan(0, 0, this.timerMonitFilaDat));
                //Reinicia monitoramento
                //ExecutarMonitoramento();
            }

            this.isNotifyMouseUp = false;
        }
        void MenuConfig(object sender, EventArgs e) { ShowMenu(OpcaoMenu.Configuracoes); }
        void MenuPurga(object sender, EventArgs e) { ShowMenu(OpcaoMenu.Purgar); }
        void MenuPurgaIndividual(object sender, EventArgs e) { ShowMenu(OpcaoMenu.PurgarIndividual); }
        void MenuFormulaPersonalizada(object sender, EventArgs e) { ShowMenu(OpcaoMenu.FormulaPersonalizada); }
        void MenuNivelColorante(object sender, EventArgs e) { ShowMenu(OpcaoMenu.GerenciarNivel); }
        void MenuSair(object sender, EventArgs e) { ShowMenu(OpcaoMenu.Sair); }
        void MenuTreinamento(object sender, EventArgs e) { ShowMenu(OpcaoMenu.Treinamento); }
        void MenuSobre(object sender, EventArgs e) { ShowMenu(OpcaoMenu.Sobre); }

        void MenuConnectPlaca(object sender, EventArgs e) { ShowMenu(OpcaoMenu.ConnectPlaca); }
        void MenuNotificacao(object sender, EventArgs e) { ShowMenu(OpcaoMenu.GerenciadorFilaDat); }
        void MenuAutomaticDispensa(object sender, EventArgs e) { ShowMenu(OpcaoMenu.MenuAutomaticDispensa); }
        void MenuRecircularProdutos(object sender, EventArgs e) { ShowMenu(OpcaoMenu.Recircular); }
        void MenuPlacaMov(object sender, EventArgs e) { ShowMenu(OpcaoMenu.PlacaMov); }
        void MenuLimpezaBico(object sender, EventArgs e) { ShowMenu(OpcaoMenu.LimpezaBico); }
        void MenuLogSerial(object sender, EventArgs e) { ShowMenu(OpcaoMenu.LogSerial); }        
        void ControlarAcessoMenu()
        {
            try
            {
                if ((DatPattern)_parametros.PadraoConteudoDAT != DatPattern.PadraoUDCP)
                {
                    if (_parametros.ControlarNivel)
                    {
                        var item = _notifyIcon.ContextMenuStrip.Items[OpcaoMenu.GerenciarNivel.ToString()];

                        if (item != null)
						    item.Visible = _parametros.ControlarNivel;
                    }

                    if (_parametros.HabilitarFormulaPersonalizada)
                    {
                        var item = _notifyIcon.ContextMenuStrip.Items[OpcaoMenu.FormulaPersonalizada.ToString()];
                        
                        if (item != null)
                            item.Visible = _parametros.HabilitarFormulaPersonalizada;
                    }

                    if (_parametros.HabilitarPurgaIndividual)
                    {
                        var item = _notifyIcon.ContextMenuStrip.Items[OpcaoMenu.PurgarIndividual.ToString()];
                        
                        if (item != null)
                            item.Visible = _parametros.HabilitarPurgaIndividual;
                    }

                    if (_parametros.HabilitarLogAutomateTesterProt)
                    {
                        var item = _notifyIcon.ContextMenuStrip.Items[OpcaoMenu.MenuAutomaticDispensa.ToString()];
                        
                        if (item != null)
                            item.Visible = _parametros.HabilitarLogAutomateTesterProt;
                    }
                    if (_parametros.HabilitarRecirculacao)
                    {
                        var item = _notifyIcon.ContextMenuStrip.Items[OpcaoMenu.Recircular.ToString()];
                        
                        if (item != null)
                            item.Visible = _parametros.HabilitarRecirculacao;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        #endregion

        #region Métodos

        void ExecutarLogSerial()
        {
            try
            {
                if(this.fLog == null)
                {
                    this.fLog = new fLogComunication();
                    this.fLog.OnClosedEvent += new Util.CloseWindows(ClosedLogComunication);
                    this.fLog.Show();
                }
                else
                {
                    this.fLog.Focus();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ClosedLogComunication()
        {
            this.fLog = null;
        }

        bool ExecutarLimpezaBico()
        {
            //Objetivo: Unificar iniciallização de circuito e purga           
            ModBusDispenserMover_P3 dispenser3 = null;

            try
            {
                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {                   
                    case Dispositivo.Placa_3:
                    {
                        dispenser3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                        break;
                    }
                }

                if (dispenser3 == null)
                {
                    return false;                    
                }
                else
                {
                    if (!Operar.ConectarP3(ref dispenser3))
                    {
                        return false;
                    }
                }

                #region Parâmeros de execução da purga  

                PurgaVO prmPurga = new PurgaVO();
                prmPurga.Dispenser = new List<IDispenser>();
                prmPurga.DispenserP3 = dispenser3;
                
                //prmPurga.Volume = _parametros.VolumePurga;
                prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1).ToList();
                prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1).Select(c => c.Dispositivo).ToArray();

                List<Util.ObjectColorante> listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                PurgaVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos();
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(_parametros.VolumePurga, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    
                    md.VolumeDosado[index] = _parametros.VolumePurga;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }                
                

                #endregion

                DialogResult result = DialogResult.None;
                using (fLimpezaBicos f = new fLimpezaBicos(prmPurga))
                {
                    result = f.ShowDialog();
                }

                return (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return false;
            }
            finally
            {
                try
                {
                    if (dispenser3 != null)
                    {
                        dispenser3.Disconnect();
                        dispenser3.Disconnect_Mover();
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}
			}
        }

        bool ExecutarRecircularAutomatica()
        {
            //Objetivo: Unificar iniciallização de circuito e purga
            ModBusDispenserMover_P3 dispenserP3 = null;

            IDispenser dispenser = null;
            IDispenser dispenserP2 = null;

            try
            {
                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            dispenser = new ModBusSimulador();
                            //isMotor24_P1 = true;
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            dispenser = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            dispenserP3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                    //case Dispositivo.Placa_4:
                    //    {
                    //        isMotor24_P1 = true;
                    //        dispenser = new ModBusDispenser_P4(_parametros.NomeDispositivo);
                    //        break;
                    //    }
                }

                //Identificar segunda placa
                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                        {
                            
                            dispenserP2 = null;
                            break;
                        }
                    case Dispositivo2.Simulador:
                        {
                            //isMotor24_P2 = true;
                            dispenserP2 = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo2.Placa_2:
                        {
                            dispenserP2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                            break;
                        }
                    //case Dispositivo2.Placa_4:
                    //    {
                    //        isMotor24_P2 = true;
                    //        dispenserP2 = new ModBusDispenser_P4(_parametros.NomeDispositivo2);
                    //        break;
                    //    }
                    default:
                        {
                            dispenserP2 = null;
                            break;
                        }
                }

                //if (!Operar.Conectar(ref dispenser))
                if (dispenserP3 == null)
                {
                    if (dispenser != null)
                    {
                        IDispenser idp = dispenser;
                        if (!Operar.Conectar(ref idp, false))
                        {
                            return false;
                        }
                    }
                    if (dispenserP2 != null)
                    {
                        IDispenser idp = dispenserP2;
                        if (!Operar.Conectar(ref idp, false))
                        {
                            return false;
                        }
                    }
                   
                    //
                }
                else
                {
                    if (!Operar.ConectarP3(ref dispenserP3, false))
                    {                        
                        return false;
                    }
                }
                bool Dat_usado_tcp_12 = false;
                switch ((DatPattern)_parametros.PadraoConteudoDAT)
                {
                    #region Padrão de dat configurado

                    case DatPattern.Padrao12:
                        {
                            Dat_usado_tcp_12 = true;

                            break;
                        }
                    #endregion
                }


                #region Parâmeros de execução da Recirculação Automática  

                RecircularAutoVO prmPurga = new RecircularAutoVO();

                prmPurga.DispenserP3 = dispenserP3;
                if (dispenser != null)
                {
                    prmPurga.Dispenser.Add(dispenser);
                }
                if(dispenserP2 !=  null)
                {
                    prmPurga.Dispenser.Add(dispenserP2);
                }

                prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true).ToList();
                prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true).Select(c => c.Dispositivo).ToArray();

                List<Util.ObjectColorante> listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                RecircularAutoVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new RecircularAutoVO.MDispositivos();
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;
                    Util.ObjectRecircular objRecAuto = this._listaRecircularAuto.Find(o => o.Circuito == c.Circuito);

                    if (objRecAuto != null && objRecAuto.Habilitado)
                    {
                        //Gera dados de diepensa
                        ValoresVO v = new ValoresVO();
                        if (Dat_usado_tcp_12)
                        {
                            v.PulsoHorario = int.Parse(Math.Round(UnidadeMedidaHelper.MililitroToGrama(objRecAuto.VolumeRecircular, c.MassaEspecifica) * 1000).ToString());
                            v.Volume = objRecAuto.VolumeRecircular;
                            v.MassaIdeal = 0;
                            v.MassaMedia = 0;
                            v.Aceleracao = 0;
                            v.Delay = 0;
                            v.DesvioMedio = 0;
                            v.PulsoReverso = 0;
                            v.Velocidade = 0;
                        }
                        else
                        {
                            v = Operar.Parser(objRecAuto.VolumeRecircular, calibragem, _cab.UltimoPulsoReverso);
                        }

                    
                    //Operar.Parser(objRecAuto.VolumeRecircular, calibragem, _cab.UltimoPulsoReverso);

                        int incrementa_10 = 0;
                        if (v.PulsoReverso < 10)
                        {
                            //incrementa_10 = 10 - v.PulsoReverso;
                        }
                        md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                        md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                        md.Velocidade[index] = v.Velocidade;
                        md.Aceleracao[index] = v.Aceleracao;
                        md.Delay[index] = v.Delay;
                        if (objRecAuto.isValve)
                        {
                            md.NrCircuitosValve.Add(c.Circuito);
                        }
                    }
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }

                if (dispenserP2 != null)
                {
                    listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 2).ToList();
                    md = null;
                    foreach (Util.ObjectColorante c in listColor)
                    {
                        if (md == null)
                        {
                            md = new RecircularAutoVO.MDispositivos();
                        }
                        Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                        List<ValoresVO> calibragem = _cab.Valores;

                        int index = c.Circuito - 1;
                        //if(isMotor24_P1)
                        //{
                        //    index -= 24;
                        //}
                        //else
                        {
                            index -= 16;
                        }
                        Util.ObjectRecircular objRecAuto = this._listaRecircularAuto.Find(o => o.Circuito == c.Circuito);

                        if (objRecAuto != null && objRecAuto.Habilitado)
                        {
                            //Gera dados de diepensa
                            ValoresVO v =  new ValoresVO();
                            if (Dat_usado_tcp_12)
                            {
                                v.PulsoHorario = int.Parse(Math.Round(UnidadeMedidaHelper.MililitroToGrama(objRecAuto.VolumeRecircular, c.MassaEspecifica) * 1000).ToString());
                                v.Volume = objRecAuto.VolumeRecircular;
                                v.MassaIdeal = 0;
                                v.MassaMedia = 0;
                                v.Aceleracao = 0;
                                v.Delay = 0;
                                v.DesvioMedio = 0;
                                v.PulsoReverso = 0;
                                v.Velocidade = 0;
                            }
                            else
                            {
                                v = Operar.Parser(objRecAuto.VolumeRecircular, calibragem, _cab.UltimoPulsoReverso);
                            }  
                            //Operar.Parser(objRecAuto.VolumeRecircular, calibragem, _cab.UltimoPulsoReverso);

                            int incrementa_10 = 0;
                            if (v.PulsoReverso < 10)
                            {
                                //incrementa_10 = 10 - v.PulsoReverso;
                            }
                            md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                            md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                            md.Velocidade[index] = v.Velocidade;
                            md.Aceleracao[index] = v.Aceleracao;
                            md.Delay[index] = v.Delay;
                            if (objRecAuto.isValve)
                            {
                                //if (isMotor24_P1)
                                //{
                                //    md.NrCircuitosValve.Add(c.Circuito - 24);
                                //}
                                //else
                                {
                                    md.NrCircuitosValve.Add(c.Circuito - 16);
                                }
                            }
                        }
                    }
                    if (md != null)
                    {
                        md.Colorantes = listColor.ToList();
                        prmPurga.LMDispositivos.Add(md);
                    }
                }
                #endregion

                DialogResult result = DialogResult.None;
                using (fRecircularAuto f = new fRecircularAuto(
                    prmPurga, this._listaRecircularAuto, false))
                {
                    result = f.ShowDialog();
                }

                Util.ObjectParametros.InitLoad();
                this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
                return (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return false;
            }
            finally
            {
                if (dispenserP3 != null)
                {
                    dispenserP3.Disconnect();
                    dispenserP3.Disconnect_Mover();
                }
                
                if(dispenser!= null)
                {
                    dispenser.Disconnect();
                }
                if(dispenserP2 != null)
                {
                    dispenserP2.Disconnect();
                }
            }
        }

        void ExecutarPlacaMov()
        {
            try
            {
                using (fPlacaMovManutencao f = new fPlacaMovManutencao())
                {
                    f.ShowDialog();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarRecircular()
        {
            try
            {
                using (fRecircular f = new fRecircular(Util.ObjectRecircular.List()))
                {
                    f.ShowDialog();
                }
                this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
                if (_parametros.HabilitarIdentificacaoCopo)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 20);
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarMenuAutomaticDispensa()
        {
            try
            {
                using (fPrecisaoMenu f = new fPrecisaoMenu())
                {
                    f.ShowDialog();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarPrecisao()
        {
            try
            {
                
                bool exec_precisa = false;
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            exec_precisa = false;
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            exec_precisa = false;
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            exec_precisa = true;
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            exec_precisa = false;
                            break;
                        }
                }
                if (exec_precisa)
                {
                    string txt = "";
                    using (StreamReader sr = new StreamReader(this.CLICK_Precisao))
                    {
                        txt = sr.ReadToEnd();
                        sr.Close();
                    }

                    File.Delete(this.CLICK_Precisao);
                    Negocio.Accurracy _ac = JsonConvert.DeserializeObject<Negocio.Accurracy>(txt);

                    Negocio.Accurracy acRet = null;
                    using (fPrecisao f = new fPrecisao(_ac))
                    {
                        f.ShowDialog();
                        acRet = f.acurracy;
                    }
                    if (acRet != null)
                    {
                        Negocio.Accurracy acWrit = new Accurracy();
                        acWrit.Circuito = acRet.Circuito;
                        acWrit.DelaySegBalanca = acRet.DelaySegBalanca;
                        acWrit.MessageRetorno = acRet.MessageRetorno;
                        acWrit.MinMassaAdmRecipiente = acRet.MinMassaAdmRecipiente;
                        acWrit.ModeloBalanca = acRet.ModeloBalanca;
                        acWrit.SerialPortBal = acRet.SerialPortBal;
                        acWrit.VolumeRecipiente = acRet.VolumeRecipiente;
                        acWrit.NumeroSerie = acRet.NumeroSerie;
                        acWrit.listPrecisao = new List<Precisao>();
                        double _vl = -1;
                        Precisao _pr = null;
                        for (int i = 0; i < acRet.listPrecisao.Count; i++)
                        {
                            if (_vl != acRet.listPrecisao[i].volume)
                            {
                                _vl = acRet.listPrecisao[i].volume;
                                if (_pr != null)
                                {
                                    acWrit.listPrecisao.Add(_pr);
                                }
                                _pr = new Precisao();
                                _pr.volume = acRet.listPrecisao[i].volume;
                                _pr.tentativas = acRet.listPrecisao[i].tentativas;
                                _pr.volumeDos_str = acRet.listPrecisao[i].volumeDos_str;
                                _pr.volumeDos = acRet.listPrecisao[i].volumeDos;
                                _pr.executado = acRet.listPrecisao[i].executado;
                            }
                            else
                            {
                                _pr.tentativas = acRet.listPrecisao[i].tentativas;
                                _pr.volumeDos_str += "#" + acRet.listPrecisao[i].volumeDos_str;
                                _pr.volumeDos = acRet.listPrecisao[i].volumeDos;
                                _pr.executado = acRet.listPrecisao[i].executado;
                            }

                        }

                        if (_pr != null)
                        {
                            acWrit.listPrecisao.Add(_pr);
                        }
                        
                        txt = JsonConvert.SerializeObject(acWrit);
                        if (File.Exists(this.CLICK_Precisao_Up))
                        {
                            File.Delete(this.CLICK_Precisao_Up);
                            Thread.Sleep(2000);
                        }
                        using (StreamWriter sw = new StreamWriter(this.CLICK_Precisao_Up))
                        {
                            sw.Write(txt);
                            sw.Close();
                        }
                    }
                }
                else
                {
                    File.Delete(this.CLICK_Precisao);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                if (backgroundWorker1.IsBusy && backgroundWorker1.CancellationPending == false)
                {
                    backgroundWorker1.CancelAsync();
                }
                _notifyIcon.Text =
               $"IOConnect ({Negocio.IdiomaResxExtensao.Global_MonitoramentoPausado})";
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ExecutarMonitoramento()
        {
            try
            {
                if (backgroundWorker1 == null)
                {
                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                    this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    this.backgroundWorker1.WorkerSupportsCancellation = true;
                    this.isThread = true;
                    this.backgroundWorker1.RunWorkerAsync();
                }
                else
                {

                    while (backgroundWorker1.IsBusy)
                    {
                        this.isThread = false;
                    }
                    backgroundWorker1.RunWorkerAsync();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			_notifyIcon.Text =
                $"IOConnect ({Negocio.IdiomaResxExtensao.Global_Monitorando})";
        }

        bool ExecutarInicializacaoColorantes(List<IDispenser> dispenser)
        {
            InicializacaoCircuitosVO param_ini = new InicializacaoCircuitosVO();
            param_ini.Dispenser = dispenser;


            //Recupera circuitos de colorantes habilitados
            param_ini.Circuitos = _colorantes.Where(c => c.Habilitado == true).Select(c => c.Circuito).ToArray();
            param_ini.Dispositivo = _colorantes.Where(c => c.Habilitado == true).Select(c => c.Dispositivo).ToArray();



            /* Popula objeto de parâmetros da inicialização de circuitos 
             * com valores de entrada do usuário */
            param_ini.PulsoInicial = _parametros.IniPulsoInicial;
            param_ini.PulsoLimite = _parametros.IniPulsoLimite;
            param_ini.VariacaoPulso = _parametros.IniVariacaoPulso;
            param_ini.StepVariacao = _parametros.IniStepVariacao;
            param_ini.Velocidade = _parametros.IniVelocidade;
            param_ini.Aceleracao = _parametros.IniAceleracao;
            param_ini.MovimentoInicialReverso = _parametros.IniMovimentoReverso;
            param_ini.QtdeCircuitoGrupo = _parametros.QtdeCircuitoGrupo;

            DialogResult result = DialogResult.None;
            using (Form f = new fInicializacaoCircuitos(
                param_ini, _parametros.DesabilitarInterfaceInicializacaoCircuito))
            {
                f.ShowDialog();
                result = f.DialogResult;
            }
            if(result == DialogResult.OK)
            {
                gerarEventoInicializarCircuitos(0);
            }
            else if(result == DialogResult.Cancel)
            {
                gerarEventoInicializarCircuitos(1);
            }
            else if (result == DialogResult.Abort)
            {
                gerarEventoInicializarCircuitos(2);
            }
            else
            {
                gerarEventoInicializarCircuitos(3);
            }

            return (result == DialogResult.OK);
        }

        int gerarEventoInicializarCircuitos(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Inicializar Circuitos
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.InicializarCircuitos;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        bool ExecutarInicializacaoColorantes(ModBusDispenserMover_P3 dispenser)
        {
            InicializacaoCircuitosVO param_ini =
                 new InicializacaoCircuitosVO();
            param_ini.Dispenser = new List<IDispenser>();
            param_ini.DispenserP3 = dispenser;

            param_ini.Circuitos = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1).Select(c => c.Circuito).ToArray();
            param_ini.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1).Select(c => c.Dispositivo).ToArray();



            /* Popula objeto de parâmetros da inicialização de circuitos 
             * com valores de entrada do usuário */
            param_ini.PulsoInicial = _parametros.IniPulsoInicial;
            param_ini.PulsoLimite = _parametros.IniPulsoLimite;
            param_ini.VariacaoPulso = _parametros.IniVariacaoPulso;
            param_ini.StepVariacao = _parametros.IniStepVariacao;
            param_ini.Velocidade = _parametros.IniVelocidade;
            param_ini.Aceleracao = _parametros.IniAceleracao;
            param_ini.MovimentoInicialReverso = _parametros.IniMovimentoReverso;
            param_ini.QtdeCircuitoGrupo = _parametros.QtdeCircuitoGrupo;          

            DialogResult result = DialogResult.None;
            using (Form f = new fInicializacaoCircuitos(param_ini, _parametros.DesabilitarInterfaceInicializacaoCircuito))
            {
                f.ShowDialog();
                result = f.DialogResult;
            }
            this.dtControleMaquinaLigada = DateTime.Now;
            return (result == DialogResult.OK);
        }

        bool ExecutarPurga()
        {
            //Objetivo: Unificar iniciallização de circuito e purga
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenser_P2 modBusDispenser_P2 = null;
            ModBusDispenserMover_P3 dispenser3 = null;
            ModBusDispenser_P4 modBusDispenser_P4 = null;
            Dictionary<Util.ObjectColorante, double> dict = null;
            List<Util.ObjectColorante> excedentes = null;

            try
            {
                if (_parametros.ControlarNivel)
                {
                    #region Verifica volume de colorante                   

                    //dict = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList()
                    //     .ToDictionary(c => c, v => _parametros.VolumePurga);
                    dict = new Dictionary<Util.ObjectColorante, double>();
                    List<Util.ObjectColorante> lMcol = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                    for (int i = 0; i < lMcol.Count; i++)
                    {
                        dict.Add(lMcol[i], lMcol[i].VolumePurga);
                    }

                    if (!Operar.TemColoranteSuficiente(dict, out excedentes))
                    {
                        string texto =
                            Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;

                        bool primeiro = true;
                        foreach (Util.ObjectColorante colorante in excedentes)
                        {
                            texto += (primeiro ? "" : ", ") + colorante.Nome;
                            primeiro = false;
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(texto);
                        }

                        return false;
                    }

                    excedentes = null;
                    dict = null;

                    #endregion
                }
                PurgaVO prmPurga = new PurgaVO();
                bool is24MotorP1 = false;
                bool is24MotorP2 = false;
                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            dispenser = new ModBusSimulador();
                            is24MotorP1 = true;
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            dispenser = new ModBusDispenser_P1();
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            modBusDispenser_P2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                            dispenser = modBusDispenser_P2;
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            dispenser3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                    //case Dispositivo.Placa_4:
                    //    {
                    //        is24MotorP1 = true;
                    //        modBusDispenser_P4 = new ModBusDispenser_P4(_parametros.NomeDispositivo);
                    //        dispenser = modBusDispenser_P4;
                    //        break;
                    //    }
                }
                int index_decP2 = 0; 
                //Identificar segunda placa
                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                        {
                            dispenser2 = null;
                            break;
                        }
                    case Dispositivo2.Simulador:
                        {
                            is24MotorP2 = true;
                            index_decP2 = 16;
                            dispenser2 = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo2.Placa_2:
                        {
                            index_decP2 = 16;
                            dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                            break;
                        }
                    //case Dispositivo2.Placa_4:
                    //    {
                    //        is24MotorP2 = true;
                    //        index_decP2 = 24;
                    //        dispenser2 = new ModBusDispenser_P4(_parametros.NomeDispositivo2);
                    //        break;
                    //    }
                    default:
                        {
                            dispenser2 = null;
                            break;
                        }
                }

                //if (!Operar.Conectar(ref dispenser))
                if (dispenser3 == null)
                {
                    if (!Operar.Conectar(ref dispenser))
                    {
                        //this.machine_turned_on = false;
                        if (this.machine_turned_on)
                        {
                            gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                            this.machine_turned_on = false;
                        }
                        return false;
                    }
                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Operar.ConectarP3(ref dispenser3))
                    {
                        return false;
                    }
                }

                if(!this.machine_turned_on)
                {
                    this.machine_turned_on = true;
                    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                }
                
                if (_parametros.InicializarCircuitosPurga)
                {
                    #region Inicializar circuitos de colorante

                    if (dispenser3 == null)
                    {

                        List<IDispenser> ldisp = new List<IDispenser>();
                        ldisp.Add(dispenser);
                        if (_parametros.IdDispositivo2 != 0)
                        {
                            ldisp.Add(dispenser2);
                        }
                        /*Se inicialização de circuito não foi executada com sucesso,
                        o fluxo será interrompido e a purga não será executada.*/
                        if (!ExecutarInicializacaoColorantes(ldisp))
                        {
                            Log.Logar(
                                TipoLog.Processo,
                                _parametros.PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);

                            return false;
                        }
                    }
                    else
                    {
                        if(!ExecutarInicializacaoColorantes(dispenser3))
                        {
                            Log.Logar(
                               TipoLog.Processo,
                               _parametros.PathLogProcessoDispensa,
                               Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);

                            return false;
                        }
                    }

                    #endregion
                }

                if (dispenser3 == null)
                {
                    if (!Operar.TemRecipiente(dispenser))
                        return false;
                }

                bool Dat_usado_tcp_12 = false;
                switch ((DatPattern)_parametros.PadraoConteudoDAT)
                {
                    #region Padrão de dat configurado

                    case DatPattern.Padrao12:
                        {
                            Dat_usado_tcp_12 = true;
                            break;
                        }
                    #endregion
                }

                #region Parâmeros de execução da purga  


                prmPurga.Dispenser = new List<IDispenser>();

                prmPurga.DispenserP3 = dispenser3;
                if (dispenser3 == null)
                {
                    prmPurga.Dispenser.Add(dispenser);
                    if (_parametros.IdDispositivo2 != 0)
                    {
                        prmPurga.Dispenser.Add(dispenser2);
                    }
                    //prmPurga.Volume = _parametros.VolumePurga;
                    prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                    prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).Select(c => c.Dispositivo).ToArray();
                }
                else
                {
                    //prmPurga.Volume = _parametros.VolumePurga;
                    prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1 && c.Seguidor == -1).ToList();
                    prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1 && c.Seguidor == -1).Select(c => c.Dispositivo).ToArray();
                    
                }


                //Valores de dispensa
                /*
                foreach (Colorante c in prmPurga.Colorantes)
                {
                    //Recupera calibragem da posição
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(prmPurga.Volume, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if(v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;

                    }
                    prmPurga.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                                        //prmPurga.PulsoReverso[index] = _parametros.PulsoReverso;
                    prmPurga.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    prmPurga.Velocidade[index] = _parametros.VelocidadePurga;
                    prmPurga.Aceleracao[index] = _parametros.AceleracaoPurga;
                    prmPurga.Delay[index] = _parametros.DelayPurga;
                }
                */
                List<Util.ObjectColorante> listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                PurgaVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos(is24MotorP1);
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = new ValoresVO();
                    if (Dat_usado_tcp_12)
                    {
                        v.PulsoHorario = int.Parse(Math.Round(UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000).ToString());
                        v.Volume = c.VolumePurga;
                        v.MassaIdeal = 0;
                        v.MassaMedia = 0;
                        v.Aceleracao = 0;
                        v.Delay = 0;
                        v.DesvioMedio = 0;
                        v.PulsoReverso = 0;
                        v.Velocidade = 0;
                    }
                    else
                    {
                        v = Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);
                    }
                    //Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    if (Dat_usado_tcp_12)
                    {
                        md.VolumeDosado[index] = UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000;
                    }
                    else
                    {
                        md.VolumeDosado[index] = c.VolumePurga;
                    }
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }

                listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 2).ToList();
                md = null;
                int counter_decP2 = 0;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos(is24MotorP2);
                    }
                    if (counter_decP2 < index_decP2)
                    {
                        Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                        List<ValoresVO> calibragem = _cab.Valores;

                        int index = (c.Circuito - index_decP2) - 1;

                        //Gera dados de diepensa
                        ValoresVO v = new ValoresVO();
                        if (Dat_usado_tcp_12)
                        {
                            v.PulsoHorario = int.Parse(Math.Round(UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000).ToString());
                            v.Volume = c.VolumePurga;
                            v.MassaIdeal = 0;
                            v.MassaMedia = 0;
                            v.Aceleracao = 0;
                            v.Delay = 0;
                            v.DesvioMedio = 0;
                            v.PulsoReverso = 0;
                            v.Velocidade = 0;
                        }
                        else
                        {
                            v = Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);
                        }
                        //= Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);

                        int incrementa_10 = 0;
                        if (v.PulsoReverso < 10)
                        {
                            //incrementa_10 = 10 - v.PulsoReverso;
                        }
                        md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                        md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                        md.Velocidade[index] = _parametros.VelocidadePurga;
                        md.Aceleracao[index] = _parametros.AceleracaoPurga;
                        md.Delay[index] = _parametros.DelayPurga;
                        if (Dat_usado_tcp_12)
                        {
                            md.VolumeDosado[index] = UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000;
                        }
                        else
                        {
                            md.VolumeDosado[index] = c.VolumePurga;
                        }
                        //md.VolumeDosado[index] = c.VolumePurga;
                    }
                    counter_decP2++;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }
                #endregion

                DialogResult result = DialogResult.None;
                if (_parametros.HabilitarRecirculacaoAuto)
                {
                    if (modBusDispenser_P2 != null)
                    {
                        modBusDispenser_P2.AcionaValvulas(false);
                    }
                    if (modBusDispenser_P4 != null)
                    {
                        modBusDispenser_P4.AcionaValvulas(false);
                    }
                    
                }
                using (fPurga f = new fPurga(
                    prmPurga, true, _parametros.DesabilitarInterfacePurga))
                {
                    result = f.ShowDialog();
                }

                if (_parametros.HabilitarIdentificacaoCopo)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 20);
                    }
                }

                #region Gerar Evento Purga
                if (result == DialogResult.OK)
                {
                    string detalhes_purga = "";
                    foreach (Util.ObjectColorante objC in prmPurga.Colorantes)
                    {
                        if (detalhes_purga == "")
                        {
                            detalhes_purga += objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(objC.VolumePurga, 3).ToString();
                        }
                        else
                        {
                            detalhes_purga += "," + objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(objC.VolumePurga, 3).ToString();
                        }
                    }
                    gerarEventoPurga(0, detalhes_purga);
                }
                else if (result == DialogResult.Cancel)
                {
                    gerarEventoPurga(1);
                }
                else if (result == DialogResult.Abort)
                {
                    gerarEventoPurga(2);
                }
                else
                {
                    gerarEventoPurga(3);
                }
                #endregion


                
                //Util.ObjectParametros.InitLoad();
                //this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado).ToList();
                return (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return false;
            }
            finally
            {
                if (dispenser != null)
                {
                    dispenser.Disconnect();
                    //dispenser = null;
                }
                if (dispenser2 != null)
                {
                    dispenser2.Disconnect();
                    //dispenser = null;
                }
                if (dispenser3 != null)
                {
                    dispenser3.Disconnect();
                    dispenser3.Disconnect_Mover();
                }

                excedentes = null;
                dict = null;

            }
        }

        int gerarEventoPurga(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Purga
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.Purga;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }
        int ExecutarPurgaUDCP(List<int> circuitos_udcp)
        {
            //Objetivo: Unificar iniciallização de circuito e purga
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenser_P2 modBusDispenser_P2 = null;
            ModBusDispenserMover_P3 dispenser3 = null;
            Dictionary<Util.ObjectColorante, double> dict = null;
            List<Util.ObjectColorante> excedentes = null;
            bool purge_all = false;
           

            try
            {
                if (_parametros.ControlarNivel)
                {
                    #region Verifica volume de colorante
                    dict = new Dictionary<Util.ObjectColorante, double>();
                    List<Util.ObjectColorante> lMcol = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                    for(int i = 0; i < lMcol.Count; i++)
                    {
                        bool achou = false;
                        for(int j = 0;!achou && j < circuitos_udcp.Count; j++)
                        {
                            if(lMcol[i].Circuito == circuitos_udcp[j])
                            {
                                achou = true;
                            }
                        }

                        if(!achou)
                        {
                            lMcol.RemoveAt(i--);
                        }
                    }
                    for (int i = 0; i < lMcol.Count; i++)
                    {
                        dict.Add(lMcol[i], lMcol[i].VolumePurga);
                    }

                        //dict = lMcol.ToDictionary(c => c, v => _parametros.VolumePurga);


                       
                    if (!Operar.TemColoranteSuficiente(dict, out excedentes))
                    {
                        string texto =
                            Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;

                        bool primeiro = true;
                        foreach (Util.ObjectColorante colorante in excedentes)
                        {
                            texto += (primeiro ? "" : ", ") + colorante.Nome;
                            primeiro = false;
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(texto);
                        }

                        return -1;
                    }

                    excedentes = null;
                    dict = null;

                    #endregion
                }

                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            dispenser = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            dispenser = new ModBusDispenser_P1();
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            modBusDispenser_P2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                            dispenser = modBusDispenser_P2;
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            dispenser3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                }

                //Identificar segunda placa
                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                        {
                            dispenser2 = null;
                            break;
                        }
                    case Dispositivo2.Simulador:
                        {
                            dispenser2 = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo2.Placa_2:
                        {
                            dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                            break;
                        }
                    default:
                        {
                            dispenser2 = null;
                            break;
                        }
                }

                //if (!Operar.Conectar(ref dispenser))
                if (dispenser3 == null)
                {
                    if (!Operar.Conectar(ref dispenser))
                    {
                        //this.machine_turned_on = false;
                        //if (this.machine_turned_on)
                        //{
                        //    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                        //    this.machine_turned_on = false;
                        //}
                        return -1;
                    }
                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        return -1;
                    }
                }
                else
                {
                    if (!Operar.ConectarP3(ref dispenser3))
                    {
                        return -1;
                    }
                }
                if (!this.machine_turned_on)
                {
                    this.machine_turned_on = true;
                    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                }

                if (_parametros.InicializarCircuitosPurga)
                {
                    #region Inicializar circuitos de colorante

                    if (dispenser3 == null)
                    {

                        List<IDispenser> ldisp = new List<IDispenser>();
                        ldisp.Add(dispenser);
                        if (_parametros.IdDispositivo2 != 0)
                        {
                            ldisp.Add(dispenser2);
                        }
                        /*Se inicialização de circuito não foi executada com sucesso,
                        o fluxo será interrompido e a purga não será executada.*/
                        if (!ExecutarInicializacaoColorantes(ldisp))
                        {
                            Log.Logar(
                                TipoLog.Processo,
                                _parametros.PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);

                            return -1;
                        }
                    }
                    else
                    {
                        if (!ExecutarInicializacaoColorantes(dispenser3))
                        {
                            Log.Logar(
                               TipoLog.Processo,
                               _parametros.PathLogProcessoDispensa,
                               Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);

                            return -1;
                        }
                    }

                    #endregion
                }

                if (dispenser3 == null)
                {
                    if (!Operar.TemRecipiente(dispenser))
                        return -1;
                }

                #region Parâmeros de execução da purga  

                PurgaVO prmPurga = new PurgaVO();
                prmPurga.Dispenser = new List<IDispenser>();

                prmPurga.DispenserP3 = dispenser3;
                if (dispenser3 == null)
                {
                    prmPurga.Dispenser.Add(dispenser);
                    if (_parametros.IdDispositivo2 != 0)
                    {
                        prmPurga.Dispenser.Add(dispenser2);
                    }
                    //prmPurga.Volume = _parametros.VolumePurga;
                    if (circuitos_udcp[0] == 0)
                    {
                        //purge_all = true;
                        prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                        prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).Select(c => c.Dispositivo).ToArray();
                    }
                    else
                    {
                        prmPurga.Colorantes = new List<Util.ObjectColorante>();
                        foreach(int _cir_udcp in circuitos_udcp)
                        {
                            Util.ObjectColorante _col = _colorantes.Find(o => o.Circuito == _cir_udcp && o.Habilitado == true && o.Seguidor == -1);
                            if(_col != null)
                            {
                                prmPurga.Colorantes.Add(_col);
                            }                            
                        }
                        prmPurga.Dispositivo = prmPurga.Colorantes.Select(c => c.Dispositivo).ToArray();
                    }
                }
                else
                {
                    purge_all = true;
                    //prmPurga.Volume = _parametros.VolumePurga;
                    prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1 && c.Seguidor == -1).ToList();
                    prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1 && c.Seguidor == -1).Select(c => c.Dispositivo).ToArray();

                }


                //Valores de dispensa
                /*
                foreach (Colorante c in prmPurga.Colorantes)
                {
                    //Recupera calibragem da posição
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(prmPurga.Volume, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if(v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;

                    }
                    prmPurga.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                                        //prmPurga.PulsoReverso[index] = _parametros.PulsoReverso;
                    prmPurga.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    prmPurga.Velocidade[index] = _parametros.VelocidadePurga;
                    prmPurga.Aceleracao[index] = _parametros.AceleracaoPurga;
                    prmPurga.Delay[index] = _parametros.DelayPurga;
                }
                */
                List<Util.ObjectColorante> listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                PurgaVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos();
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    md.VolumeDosado[index] = c.VolumePurga;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }

                listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 2).ToList();
                md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos();
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = (c.Circuito - 16) - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    md.VolumeDosado[index] = c.VolumePurga;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }
                #endregion
                if(prmPurga.Colorantes.Count >= _colorantes.FindAll(o=>o.Habilitado && o.Seguidor != 1).Count)
                {
                    purge_all = true;
                }
                

                DialogResult result = DialogResult.None;
                if (_parametros.HabilitarRecirculacaoAuto)
                {
                    if (modBusDispenser_P2 != null)
                    {
                        modBusDispenser_P2.AcionaValvulas(false);
                    }
                }
                using (fPurga f = new fPurga(
                    prmPurga, purge_all, _parametros.DesabilitarInterfacePurga))
                {
                    result = f.ShowDialog();
                }

                if (_parametros.HabilitarIdentificacaoCopo)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 20);
                    }
                }

                //Util.ObjectParametros.InitLoad();
                //this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado).ToList();
                if (result == DialogResult.OK)
                {
                    string detalhes_purga = "";
                    foreach (Util.ObjectColorante objC in prmPurga.Colorantes)
                    {
                        if (detalhes_purga == "")
                        {
                            detalhes_purga += objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(objC.VolumePurga, 3).ToString();
                        }
                        else
                        {
                            detalhes_purga += "," + objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(objC.VolumePurga, 3).ToString();
                        }
                    }
                    gerarEventoPurgaUDCP(0, detalhes_purga, purge_all);
                    return 0;
                }
                else if(result == DialogResult.Cancel)
                {
                    gerarEventoPurgaUDCP(1, "", purge_all);
                    return 1;
                }
                else if (result == DialogResult.Abort)
                {
                    gerarEventoPurgaUDCP(2, "", purge_all);
                    return 2;
                }
                else
                {
                    gerarEventoPurgaUDCP(3, "", purge_all);
                    return 3;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return -1;
            }
            finally
            {
                if (dispenser != null)
                {
                    dispenser.Disconnect();
                    //dispenser = null;
                }
                if (dispenser2 != null)
                {
                    dispenser2.Disconnect();
                    //dispenser = null;
                }
                if (dispenser3 != null)
                {
                    dispenser3.Disconnect();
                    dispenser3.Disconnect_Mover();
                }

                excedentes = null;
                dict = null;

            }
        }

        int ExecutarPurgaUDCP(Dictionary<int, double> demanda_udcp)
        {
            //Objetivo: Unificar iniciallização de circuito e purga
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenser_P2 modBusDispenser_P2 = null;
            ModBusDispenserMover_P3 dispenser3 = null;
            Dictionary<Util.ObjectColorante, double> dict = null;
            List<Util.ObjectColorante> excedentes = null;
            bool purge_all = false;

            try
            {
                if (_parametros.ControlarNivel)
                {
                    #region Verifica volume de colorante

                    //dict = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList()
                    //     .ToDictionary(c => c, v => _parametros.VolumePurga);

                    dict = new Dictionary<Util.ObjectColorante, double>();
                    List<Util.ObjectColorante> lcol = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                    foreach (KeyValuePair<int, double> item in demanda_udcp)
                    {
                        int circuito = item.Key;
                        if (circuito == 0)
                        {
                            dict = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList()
                                 .ToDictionary(c => c, v => item.Value);
                        }
                        else
                        {
                            Util.ObjectColorante _col = lcol.Find(o => o.Circuito == circuito);
                            if(_col != null)
                            {
                                dict.Add(_col, item.Value);
                            }
                        }
                    }

                    if (!Operar.TemColoranteSuficiente(dict, out excedentes))
                    {
                        string texto =
                            Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;

                        bool primeiro = true;
                        foreach (Util.ObjectColorante colorante in excedentes)
                        {
                            texto += (primeiro ? "" : ", ") + colorante.Nome;
                            primeiro = false;
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(texto);
                        }

                        return -1;
                    }

                    excedentes = null;
                    dict = null;

                    #endregion
                }

                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            dispenser = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            dispenser = new ModBusDispenser_P1();
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            modBusDispenser_P2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                            dispenser = modBusDispenser_P2;
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            dispenser3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                }

                //Identificar segunda placa
                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                        {
                            dispenser2 = null;
                            break;
                        }
                    case Dispositivo2.Simulador:
                        {
                            dispenser2 = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo2.Placa_2:
                        {
                            dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                            break;
                        }
                    default:
                        {
                            dispenser2 = null;
                            break;
                        }
                }

                //if (!Operar.Conectar(ref dispenser))
                if (dispenser3 == null)
                {
                    if (!Operar.Conectar(ref dispenser))
                    {
                        //this.machine_turned_on = false;
                        if (this.machine_turned_on)
                        {
                            gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                            this.machine_turned_on = false;
                        }
                        return -1;
                    }
                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        return -1;
                    }
                }
                else
                {
                    if (!Operar.ConectarP3(ref dispenser3))
                    {
                        return 01;
                    }
                }

                if (!this.machine_turned_on)
                {
                    this.machine_turned_on = true;
                    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                }
                if (_parametros.InicializarCircuitosPurga)
                {
                    #region Inicializar circuitos de colorante

                    if (dispenser3 == null)
                    {

                        List<IDispenser> ldisp = new List<IDispenser>();
                        ldisp.Add(dispenser);
                        if (_parametros.IdDispositivo2 != 0)
                        {
                            ldisp.Add(dispenser2);
                        }
                        /*Se inicialização de circuito não foi executada com sucesso,
                        o fluxo será interrompido e a purga não será executada.*/
                        if (!ExecutarInicializacaoColorantes(ldisp))
                        {
                            Log.Logar(
                                TipoLog.Processo,
                                _parametros.PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);

                            return -1;
                        }
                    }
                    else
                    {
                        if (!ExecutarInicializacaoColorantes(dispenser3))
                        {
                            Log.Logar(
                               TipoLog.Processo,
                               _parametros.PathLogProcessoDispensa,
                               Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);

                            return -1;
                        }
                    }

                    #endregion
                }

                if (dispenser3 == null)
                {
                    if (!Operar.TemRecipiente(dispenser))
                        return -1;
                }

                #region Parâmeros de execução da purga  

                PurgaVO prmPurga = new PurgaVO();
                prmPurga.Dispenser = new List<IDispenser>();

                prmPurga.DispenserP3 = dispenser3;
                if (dispenser3 == null)
                {
                    prmPurga.Dispenser.Add(dispenser);
                    if (_parametros.IdDispositivo2 != 0)
                    {
                        prmPurga.Dispenser.Add(dispenser2);
                    }
                    //prmPurga.Volume = _parametros.VolumePurga;
                    foreach (KeyValuePair<int, double> item in demanda_udcp)
                    {
                        int circuito = item.Key;
                        if (circuito == 0)
                        {
                            purge_all = true;
                            prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();                           
                            break;
                        }
                        else
                        {
                            if(prmPurga.Colorantes == null)
                            {
                                prmPurga.Colorantes = new List<Util.ObjectColorante>();
                            }

                            Util.ObjectColorante _col = _colorantes.Find(o => o.Circuito == circuito && o.Habilitado == true && o.Seguidor == -1);
                            if (_col != null)
                            {
                                prmPurga.Colorantes.Add(_col);
                            }
                        }
                    }
                    prmPurga.Dispositivo = prmPurga.Colorantes.Select(c => c.Dispositivo).ToArray();
                }
                else
                {
                    purge_all = true;
                    //prmPurga.Volume = _parametros.VolumePurga;
                    prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1 && c.Seguidor == -1).ToList();
                    prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1 && c.Seguidor == -1).Select(c => c.Dispositivo).ToArray();

                }


                //Valores de dispensa
                /*
                foreach (Colorante c in prmPurga.Colorantes)
                {
                    //Recupera calibragem da posição
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(prmPurga.Volume, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if(v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;

                    }
                    prmPurga.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                                        //prmPurga.PulsoReverso[index] = _parametros.PulsoReverso;
                    prmPurga.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    prmPurga.Velocidade[index] = _parametros.VelocidadePurga;
                    prmPurga.Aceleracao[index] = _parametros.AceleracaoPurga;
                    prmPurga.Delay[index] = _parametros.DelayPurga;
                }
                */
                List<Util.ObjectColorante> listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                PurgaVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos();
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    double valDos = 0;

                    foreach (KeyValuePair<int, double> item in demanda_udcp)
                    {
                        int circuito = item.Key;
                        if(circuito == 0)
                        {
                            valDos = item.Value;
                            break;
                        }
                        else if(c.Circuito == circuito)
                        {
                            valDos = item.Value;
                            break;
                        }
                    }

                    ValoresVO v = Operar.Parser(valDos, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    md.VolumeDosado[index] = valDos;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }

                listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 2).ToList();
                md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos();
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = (c.Circuito - 16) - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(_parametros.VolumePurga, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    md.VolumeDosado[index] = _parametros.VolumePurga;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }
                #endregion
                if (_parametros.HabilitarRecirculacaoAuto)
                {
                    if (modBusDispenser_P2 != null)
                    {
                        modBusDispenser_P2.AcionaValvulas(false);
                    }
                }
                DialogResult result = DialogResult.None;
                using (fPurga f = new fPurga(
                    prmPurga, purge_all, _parametros.DesabilitarInterfacePurga))
                {
                    result = f.ShowDialog();
                }

                if (_parametros.HabilitarIdentificacaoCopo)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 20);
                    }
                }

                //Util.ObjectParametros.InitLoad();
                //this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado).ToList();
                //return (result == DialogResult.OK);
                if (result == DialogResult.OK)
                {
                    string detalhes_purga = "";
                    foreach (Util.ObjectColorante objC in prmPurga.Colorantes)
                    {
                        try
                        {
                            double vVolDosado = 0;

                            foreach(KeyValuePair<int, double> item in demanda_udcp)
                            {
                                if(item.Key== objC.Circuito)
                                {
                                    vVolDosado = item.Value;
                                    break;
                                }
                            }
                           
                            if (detalhes_purga == "")
                            {
                                detalhes_purga += objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(vVolDosado, 3).ToString();
                            }
                            else
                            {
                                detalhes_purga += "," + objC.Circuito.ToString() + "," + objC.Nome + "," + Math.Round(vVolDosado, 3).ToString();
                            }
                        }
						catch (Exception ex)
						{
							LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
						    detalhes_purga = "";
                            break;
                        }
                    }
                    gerarEventoPurgaUDCP(0, detalhes_purga);
                    return 0;
                }
                else if (result == DialogResult.Cancel)
                {
                    gerarEventoPurgaUDCP(1, "");
                    return 1;
                }
                else if (result == DialogResult.Abort)
                {
                    gerarEventoPurgaUDCP(2, "");
                    return 2;
                }
                else
                {
                    gerarEventoPurgaUDCP(3, "");
                    return 3;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return -1;
            }
            finally
            {
                if (dispenser != null)
                {
                    dispenser.Disconnect();
                    //dispenser = null;
                }
                if (dispenser2 != null)
                {
                    dispenser2.Disconnect();
                    //dispenser = null;
                }
                if (dispenser3 != null)
                {
                    dispenser3.Disconnect();
                    dispenser3.Disconnect_Mover();
                }

                excedentes = null;
                dict = null;

            }
        }

        int gerarEventoPurgaUDCP(int result, string detalhes="", bool purgeAll = false)
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Purga Individual
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                if (purgeAll)
                {
                    objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.Purga;
                }
                else
                {
                    objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.PurgaIndividual;
                }
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        bool ExecutarPurgaIndividual()
        {
            //Objetivo: Unificar iniciallização de circuito e purga
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenser_P2 modBusDispenser_P2 = null;
            ModBusDispenserMover_P3 dispenser3 = null;
            ModBusDispenser_P4 modBusDispenser_P4 = null;
            Dictionary<Util.ObjectColorante, double> dict = null;
            List<Util.ObjectColorante> excedentes = null;
            bool is24MotorP1 = false;
            bool is24MotorP2 = false;

            try
            {
                if (_parametros.ControlarNivel)
                {
                    #region Verifica volume de colorante

                    //dict = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList()
                    //     .ToDictionary(c => c, v => _parametros.VolumePurga);

                    dict = new Dictionary<Util.ObjectColorante, double>();
                    List<Util.ObjectColorante> lMcol = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                    for (int i = 0; i < lMcol.Count; i++)
                    {
                        dict.Add(lMcol[i], lMcol[i].VolumePurga);
                    }


                    if (!Operar.TemColoranteSuficiente(dict, out excedentes))
                    {
                        string texto =
                            Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;

                        bool primeiro = true;
                        foreach (Util.ObjectColorante colorante in excedentes)
                        {
                            texto += (primeiro ? "" : ", ") + colorante.Nome;
                            primeiro = false;
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(texto);
                        }

                        return false;
                    }

                    excedentes = null;
                    dict = null;

                    #endregion
                }

                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            is24MotorP1 = true;
                            dispenser = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            is24MotorP1 = false;
                            dispenser = new ModBusDispenser_P1();
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            is24MotorP1 = false;
                            modBusDispenser_P2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                            dispenser = modBusDispenser_P2;

                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            is24MotorP1 = false;
                            dispenser3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                    //case Dispositivo.Placa_4:
                    //    {
                    //        is24MotorP1 = true;
                    //        modBusDispenser_P4 = new ModBusDispenser_P4(_parametros.NomeDispositivo);
                    //        dispenser = modBusDispenser_P4;
                    //        break;
                    //    }

                }

                //Identificar segunda placa
                int index_decP2 = 0;
                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                        {
                            is24MotorP2 = false;
                            index_decP2 = 16;
                            dispenser2 = null;
                            break;
                        }
                    case Dispositivo2.Simulador:
                        {
                            is24MotorP2 = true;
                            index_decP2 = 24;
                            dispenser2 = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo2.Placa_2:
                        {
                            is24MotorP2 = false;
                            index_decP2 = 16;
                            dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                            break;
                        }
                    //case Dispositivo2.Placa_4:
                    //    {
                    //        is24MotorP2 = true;
                    //        index_decP2 = 24;
                    //        dispenser2 = new ModBusDispenser_P4(_parametros.NomeDispositivo2);
                    //        break;
                    //    }
                    default:
                        {
                            dispenser2 = null;
                            break;
                        }
                }

                //if (!Operar.Conectar(ref dispenser))
                if (dispenser3 == null)
                {
                    if (!Operar.Conectar(ref dispenser))
                    {
                        return false;
                    }
                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Operar.ConectarP3(ref dispenser3))
                    {
                        return false;
                    }
                }


                if (dispenser3 == null)
                {
                    if (!Operar.TemRecipiente(dispenser))
                        return false;
                }
                bool Dat_usado_tcp_12 = false;
                switch ((DatPattern)_parametros.PadraoConteudoDAT)
                {
                    #region Padrão de dat configurado

                    case DatPattern.Padrao12:
                        {
                            Dat_usado_tcp_12 = true;

                            break;
                        }
                        #endregion
                }

                #region Parâmeros de execução da purga  

                PurgaVO prmPurga = new PurgaVO();
                prmPurga.Dispenser = new List<IDispenser>();
                prmPurga.DispenserP3 = dispenser3;
                if (dispenser3 == null)
                {
                    prmPurga.Dispenser.Add(dispenser);
                    if (_parametros.IdDispositivo2 != 0)
                    {
                        prmPurga.Dispenser.Add(dispenser2);
                    }
                    //prmPurga.Volume = _parametros.VolumePurga;
                    prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true ).ToList();
                    prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true).Select(c => c.Dispositivo).ToArray();
                }
                else
                {
                    //prmPurga.Volume = _parametros.VolumePurga;
                    prmPurga.Colorantes = _colorantes.Where(c => c.Habilitado == true  && c.Dispositivo == 1).ToList();
                    prmPurga.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1).Select(c => c.Dispositivo).ToArray();
                }
               


                //Valores de dispensa
                /*
                foreach (Colorante c in prmPurga.Colorantes)
                {
                    //Recupera calibragem da posição
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = Operar.Parser(prmPurga.Volume, calibragem, _cab.UltimoPulsoReverso);

                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;

                    }
                    prmPurga.Circuitos[index] = index;
                    prmPurga.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    //prmPurga.PulsoReverso[index] = _parametros.PulsoReverso;
                    prmPurga.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    prmPurga.Velocidade[index] = _parametros.VelocidadePurga;
                    prmPurga.Aceleracao[index] = _parametros.AceleracaoPurga;
                    prmPurga.Delay[index] = _parametros.DelayPurga;
                }
                */

                List<Util.ObjectColorante> listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                PurgaVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos(is24MotorP1);
                    }
                    Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                    List<ValoresVO> calibragem = _cab.Valores;

                    int index = c.Circuito - 1;

                    //Gera dados de diepensa
                    ValoresVO v = new ValoresVO();
                    if (Dat_usado_tcp_12)
                    {
                        v.PulsoHorario = int.Parse(Math.Round(UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000).ToString());
                        v.Volume = c.VolumePurga;
                        v.MassaIdeal = 0;
                        v.MassaMedia = 0;
                        v.Aceleracao = 0;
                        v.Delay = 0;
                        v.DesvioMedio = 0;
                        v.PulsoReverso = 0;
                        v.Velocidade = 0;
                    }
                    else
                    {
                        v = Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);
                    }
                    //Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);
                    //c.Volume = this._parametros.VolumePurga;
                    int incrementa_10 = 0;
                    if (v.PulsoReverso < 10)
                    {
                        //incrementa_10 = 10 - v.PulsoReverso;
                    }
                    md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                    md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                    md.Velocidade[index] = _parametros.VelocidadePurga;
                    md.Aceleracao[index] = _parametros.AceleracaoPurga;
                    md.Delay[index] = _parametros.DelayPurga;
                    if (Dat_usado_tcp_12)
                    {
                        md.VolumeDosado[index] = UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000;
                    }
                    else
                    {
                        md.VolumeDosado[index] = c.VolumePurga;
                    }
                    //md.VolumeDosado[index] = c.VolumePurga;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }

                listColor = prmPurga.Colorantes.Where(c => c.Dispositivo == 2).ToList();
                md = null;
                int counter_decP2 = 0;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new PurgaVO.MDispositivos(is24MotorP2);
                    }
                    if (counter_decP2 < index_decP2)
                    {
                        Util.ObjectCalibragem _cab = Util.ObjectCalibragem.Load(c.Circuito);

                        List<ValoresVO> calibragem = _cab.Valores;

                        //int index = (c.Circuito - 16) - 1;
                        int index = (c.Circuito - index_decP2) - 1;
                        //Gera dados de diepensa
                        ValoresVO v = new ValoresVO();
                        if (Dat_usado_tcp_12)
                        {
                            v.PulsoHorario = int.Parse(Math.Round(UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000).ToString());
                            v.Volume = c.VolumePurga;
                            v.MassaIdeal = 0;
                            v.MassaMedia = 0;
                            v.Aceleracao = 0;
                            v.Delay = 0;
                            v.DesvioMedio = 0;
                            v.PulsoReverso = 0;
                            v.Velocidade = 0;
                        }
                        else
                        {
                            v = Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);
                        }
                        //= Operar.Parser(c.VolumePurga, calibragem, _cab.UltimoPulsoReverso);
                        //c.Volume = c.VolumePurga;

                        int incrementa_10 = 0;
                        if (v.PulsoReverso < 10)
                        {
                            //incrementa_10 = 10 - v.PulsoReverso;
                        }
                        md.PulsoHorario[index] = v.PulsoHorario + incrementa_10;
                        md.PulsoReverso[index] = v.PulsoReverso + incrementa_10;
                        md.Velocidade[index] = _parametros.VelocidadePurga;
                        md.Aceleracao[index] = _parametros.AceleracaoPurga;
                        md.Delay[index] = _parametros.DelayPurga;
                        if (Dat_usado_tcp_12)
                        {
                            md.VolumeDosado[index] = UnidadeMedidaHelper.MililitroToGrama(c.VolumePurga, c.MassaEspecifica) * 1000;
                        }
                        else
                        {
                            md.VolumeDosado[index] = c.VolumePurga;
                        }
                        //md.VolumeDosado[index] = c.VolumePurga;
                    }
                    counter_decP2++;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmPurga.LMDispositivos.Add(md);
                }

                #endregion

                if (_parametros.HabilitarRecirculacaoAuto)
                {
                    if (modBusDispenser_P2 != null)
                    {
                        modBusDispenser_P2.AcionaValvulas(false);
                    }
                    if (modBusDispenser_P4 != null)
                    {
                        modBusDispenser_P4.AcionaValvulas(false);
                    }

                }
                DialogResult result = DialogResult.None;
                using (fPurgaIndividual f = new fPurgaIndividual(prmPurga))
                {
                    result = f.ShowDialog();
                }

                this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
                if (_parametros.HabilitarIdentificacaoCopo)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 20);
                    }
                }
                this.dtControleMaquinaLigada = DateTime.Now;
                return (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return false;
            }
            finally
            {
                if (dispenser != null)
                {
                    dispenser.Disconnect();
                    //dispenser = null;
                }
                if (dispenser2 != null)
                {
                    dispenser2.Disconnect();
                    //dispenser = null;
                }
                if (dispenser3 != null)
                {
                    dispenser3.Disconnect();
                    dispenser3.Disconnect_Mover();
                }

                excedentes = null;
                dict = null;
            }
        }

        bool ExecutarMonitoramentoCircuitos()
        {
            //Objetivo: Unificar iniciallização de circuito e purga
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;

            try
            {
                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                    {
                        dispenser = new ModBusSimulador();
                        break;
                    }
                    case Dispositivo.Placa_1:
                    {
                        dispenser = new ModBusDispenser_P1();
                        break;
                    }
                    case Dispositivo.Placa_2:
                    {
                        dispenser = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                        break;
                    }
                }

                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                    {
                        dispenser2 = null;
                        break;
                    }
                    case Dispositivo2.Simulador:
                    {
                        dispenser2 = new ModBusSimulador();
                        break;
                    }
                    case Dispositivo2.Placa_2:
                    {
                        dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                        break;
                    }
                    default:
                    {
                        dispenser2 = null;
                        break;
                    }
                }

                if (!Operar.Conectar(ref dispenser, false))
                {
                    return false;
                }
                if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2, false))
                {
                    return false;
                }
                if (_parametros.HabilitarTesteRecipiente)
                {
                    if (Operar.TemRecipiente(dispenser, false))
                    {
                        return false;
                    }
                }
                #region Parâmeros de execução do Monitoramento dos Circuitos  

                MonitoramentoVO prmMoni = new MonitoramentoVO();
                prmMoni.Dispenser = new List<IDispenser>();
                prmMoni.Dispenser.Add(dispenser);
                if (_parametros.IdDispositivo2 != 0)
                {
                    prmMoni.Dispenser.Add(dispenser2);
                }
                prmMoni.Colorantes = _colorantes.Where(c => c.Habilitado == true).ToList();
                prmMoni.Dispositivo = _colorantes.Where(c => c.Habilitado == true).Select(c => c.Dispositivo).ToArray();

                //Valores de dispensa               
                /*
                foreach (Colorante c in prmMoni.Colorantes)
                {
                    int index = c.Circuito - 1;

                    prmMoni.PulsoHorario[index] = _parametros.MonitPulsos;
                    prmMoni.PulsoReverso[index] = _parametros.MonitPulsos;
                    prmMoni.Velocidade[index] = _parametros.MonitVelocidade;
                    prmMoni.Aceleracao[index] = _parametros.MonitAceleracao;
                    prmMoni.Delay[index] = _parametros.MonitDelay;

                }
                */
                List<Util.ObjectColorante> listColor = prmMoni.Colorantes.Where(c => c.Dispositivo == 1).ToList();
                List<MonitoramentoVO.MDispositivos> lmd = new List<MonitoramentoVO.MDispositivos>();
                MonitoramentoVO.MDispositivos md = null;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new MonitoramentoVO.MDispositivos();
                    }
                    int index = c.Circuito - 1;

                    md.PulsoHorario[index] = _parametros.MonitPulsos;
                    md.PulsoReverso[index] = _parametros.MonitPulsos;
                    md.Velocidade[index] = _parametros.MonitVelocidade;
                    md.Aceleracao[index] = _parametros.MonitAceleracao;
                    md.Delay[index] = _parametros.MonitDelay;
                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmMoni.LMDispositivos.Add(md);
                }
                //Proxima Placa
                listColor = prmMoni.Colorantes.Where(c => c.Dispositivo == 2).ToList();
                md = null;
                int indexCount = 0;
                foreach (Util.ObjectColorante c in listColor)
                {
                    if (md == null)
                    {
                        md = new MonitoramentoVO.MDispositivos();
                    }
                    //if ((isMotor24P2 && indexCount < 24) || (!isMotor24P2 && indexCount < 16))
                    //{
                    //    int index = (c.Circuito - indexV2) - 1;

                    //    md.PulsoHorario[index] = _parametros.MonitPulsos;
                    //    md.PulsoReverso[index] = _parametros.MonitPulsos;
                    //    md.Velocidade[index] = _parametros.MonitVelocidade;
                    //    md.Aceleracao[index] = _parametros.MonitAceleracao;
                    //    md.Delay[index] = _parametros.MonitDelay;
                    //}
                    indexCount++;


                }
                if (md != null)
                {
                    md.Colorantes = listColor.ToList();
                    prmMoni.LMDispositivos.Add(md);
                }


                #endregion

                DialogResult result = DialogResult.None;
                if (_parametros.NomeDispositivo != null)
                {

                    Constants.numeroRetentativasBluetooth = 0;
                    while (Constants.numeroRetentativasBluetooth < 2)
                    {
                        using (fMonitoramentoCircuitos f = new fMonitoramentoCircuitos(prmMoni, _parametros.DesabilitarInterfaceMonitCircuito))
                        {
                            result = f.ShowDialog();
                        }
                        if (result == DialogResult.OK)
                        {
                            break;
                        }
                        Constants.numeroRetentativasBluetooth++;
                    }
                }
                else
                {
                    using (fMonitoramentoCircuitos f = new fMonitoramentoCircuitos(
                        prmMoni, _parametros.DesabilitarInterfaceMonitCircuito))
                    {
                        result = f.ShowDialog();
                    }
                }
                Util.ObjectParametros.InitLoad();

                if (result == DialogResult.OK)
                {
                    gerarEventoMonitoramentoCircuitos(0);
                }
                else if (result == DialogResult.Cancel)
                {
                    gerarEventoMonitoramentoCircuitos(1);
                }
                else if (result == DialogResult.Abort)
                {
                    gerarEventoMonitoramentoCircuitos(2);
                }
                else
                {
                    gerarEventoMonitoramentoCircuitos(3);
                }


                return (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return false;
            }
            finally
            {
                if (dispenser != null)
                {
                    dispenser.Disconnect();
                    //dispenser = null;
                }
                if (dispenser2 != null)
                {
                    dispenser2.Disconnect();
                    //dispenser = null;
                }
            }
        }

        int gerarEventoMonitoramentoCircuitos(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Monitorar Circuitos
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.MonitoramentoCircuitos;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        bool ExecutarSincronismoFormula(int tipo)
        {
            bool retorno = false;
            try
            {
                DialogResult result = DialogResult.None;
                using (fSincFormula f = new fSincFormula(tipo))
                {
                    result = f.ShowDialog();
                }

                retorno = (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        bool ExecutarGerenciadorFilaDat()
        {
            bool retorno = false;
            try
            {
                DialogResult result = DialogResult.None;
                using (fMonitoramentoFilaDat f = new fMonitoramentoFilaDat())
                {
                    result = f.ShowDialog();
                }
                this._listaRecircular = Util.ObjectRecircular.List().Where(o => o.Habilitado && !o.isAuto).ToList();
                retorno = (result == DialogResult.OK);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        void EncerrarOperacao(string descricaoDAT, string[] log, string mensagemUsuario)
        {
            try
            {
                if (File.Exists(_parametros.PathMonitoramentoDAT))
                {
                    FileHelper.Rename(
                        _parametros.PathMonitoramentoDAT,
                        _parametros.PathRepositorioDAT,
                        true,
                        descricaoDAT,
                        ".err");
                }
                else if(!string.IsNullOrEmpty(this.pathUDCP))
                {
                    string npath = _parametros.PathMonitoramentoDAT.Replace(".sig", ".dat");
                    if(File.Exists(npath))
                    {
                        FileHelper.Rename(
                            npath,
                            _parametros.PathRepositorioDAT,
                            true,
                            descricaoDAT,
                            ".err");
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			if (log.Length > 0)
            {
                Log.Logar(
                    TipoLog.Processo, _parametros.PathLogProcessoDispensa, log);
            }

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
            {
                m.ShowDialog(mensagemUsuario);
            }
        }

        void EncerrarOperacao(string descricaoDAT, string log, string mensagemUsuario)
        {
            EncerrarOperacao(
                descricaoDAT, new string[] { log }, mensagemUsuario);
        }

        void EncerrarOperacao(string descricaoDAT, string mensagemUsuario)
        {
            EncerrarOperacao(
                descricaoDAT, new string[] { }, mensagemUsuario);
        }

        #endregion
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                while (this.isThread)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        this.isThread = false;
                    }
                    else
                    {
                        if (!this.menu)
                        {
                            this.menu = true;
                            this.Invoke(new MethodInvoker(MonitoramentoEvent));
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}        
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    this.isThread = false;
                }
                else if (!(e.Error == null))
                {
                    this.isThread = false;
                }
                else
                {
                    this.isThread = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        #region Controle fMensagem Placa Movimentacao
        private bool getControlePlacaMov()
        {
            bool retorno = false;
            switch ((Dispositivo)_parametros.IdDispositivo)
            {
                case Dispositivo.Placa_3:
                    {
                        retorno = true;
                        break;
                    }

            }
            return retorno;
        }
        #endregion
        private void getMessagePlacaMov()
        {
            ModBusDispenserMover_P3 dispP3 = null;
            try
            {
                dispP3 = new ModBusDispenserMover_P3(this._parametros.NomeDispositivo, this._parametros.NomeDispositivo_PlacaMov);
                dispP3.Connect_Mover();
                Thread.Sleep(100);
                dispP3.ReadSensores_Mover();
                if(dispP3.SensorEmergencia)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Botão de Emergência pressionado!", "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia, IdiomaResx.Global_Sim, null, true, 30);
                    }
                }               
                else if(dispP3.CodAlerta > 0)
                {
                    if (dispP3.CodAlerta == 5 || dispP3.CodAlerta == 6 || dispP3.CodAlerta == 14 || dispP3.CodAlerta == 15 || dispP3.CodAlerta == 16)
                    {
                        DialogResult result = DialogResult.None;
                        using (fTratarAlertasP3 ftrataP3 = new fTratarAlertasP3(dispP3, false))
                        {
                            result = ftrataP3.ShowDialog();                           
                        }
                    }
                    else
                    {
                        using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            bool confirma = message.ShowDialog("Alerta Placa Movimentação: " + dispP3.GetDescCodAlerta(), "Sim", null, true, 30);
                        }
                    }
                }
                else if (dispP3.CodError > 0)
                {
                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.PainelControle_Erro_Posicao_PlacaMov + dispP3.GetDescCodError(), "Sim", null, true, 30);
                    }
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
			finally
            {
                try
                {
                    if (dispP3 != null)
                    {
                        dispP3.Disconnect_Mover();
                        dispP3.Disconnect();
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}
			}
        }
        private void getMessageEsponja()
        {
            try
            {
                TimeSpan? ts = DateTime.Now.Subtract(this.dtControleEsponja);
                if(ts != null && ts.HasValue && ts.Value.TotalMinutes >= _parametros.DelayEsponja)
                {
                    this.dtControleEsponja = DateTime.Now;
                    switch ((Dispositivo)_parametros.IdDispositivo)
                    {
                        case Dispositivo.Placa_2:
                            {
                                ModBusDispenser_P2 mdP2 = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                try
                                {
                                    mdP2.Connect();
                                    this.machine_turned_on = true;
                                    this.dtControleMaquinaLigada = DateTime.Now;
                                    if (!mdP2.IdentitySponge)
                                    {
                                        using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                        {
                                            //bool confirma = message.ShowDialog("Erro de Posicionamento Placa Movimentação: " + dispP3.GetDescCodError(), "Sim", "Não", true, 30);
                                            bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_Esponja, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 30);
                                        }
                                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_Falha_Esponja);
                                    }
                                   
                                }
								catch (Exception ex)
								{
									LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
								}
								finally
                                {
                                    mdP2.Disconnect();
                                }
                                break;
                            }                      
                        default:
                            {
                                if (!this.machine_turned_on)
                                {
                                    this.machine_turned_on = true;
                                    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                                }
                                this.dtControleMaquinaLigada = DateTime.Now;
                                break;
                            }
                    }
                    this.dtControleEsponja = DateTime.Now;
                }
                ts = null;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        public void setCondicaoMaquinaLigada(bool _b_lig)
        {
            try
            {
                this.dtControleMaquinaLigada = DateTime.Now;
                if(_b_lig)
                {
                    if(!this.machine_turned_on)
                    {
                        if (_parametros.LogStatusMaquina)
                        {
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_Maquina_Ligada);
                        }

                        gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                    }
                }
                this.machine_turned_on = _b_lig;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void getMaquinaLigada(bool forceConnect=false)
        {
            bool estadoAnteriorMaquina = this.machine_turned_on;
            try
            {
                TimeSpan? ts = DateTime.Now.Subtract(this.dtControleMaquinaLigada);
                if ((ts != null && ts.HasValue && ts.Value.TotalMinutes >= 10) || forceConnect)
                {
                    dtControleMaquinaLigada = DateTime.Now;
                    switch ((Dispositivo) _parametros.IdDispositivo)
                    {
                        case Dispositivo.Placa_2:
                            {
                                IDispenser disp = null; 
                                try
                                {
                                    disp = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                                    if (Operar.Conectar(ref disp, false))
                                    {
                                        if (!this.machine_turned_on && _parametros.LogStatusMaquina)
                                        {
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_Maquina_Ligada);
                                        }
                                        this.machine_turned_on = true;
                                    }
                                    else
                                    {
                                        if (_parametros.LogStatusMaquina)
                                        {
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_Maquina_Desligada);
                                        }
                                        this.machine_turned_on = false;
                                    }
                                }
								catch (Exception ex)
								{
									LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
								}
								finally
                                {
                                    try
                                    {
                                        if (disp != null)
                                        {
                                            disp.Disconnect();
                                        }
                                    }
									catch (Exception ex)
									{
										LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
									}
								}
                                break;
                            }
                        default:
                            {
                                if (!this.machine_turned_on && _parametros.LogStatusMaquina)
                                {
                                    Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_Maquina_Ligada);
                                }
                                this.machine_turned_on = true;
                                break;
                            }
                    }

                    dtControleMaquinaLigada = DateTime.Now;
                }
                ts = null;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			if (estadoAnteriorMaquina != this.machine_turned_on)
            {
                if(this.machine_turned_on)
                {
                    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaLigada);
                }
                else
                {
                    gerarEventoMAquinaOnOff((int)IOConnect.Core.PercoloreEnum.Eventos.MaquinaDesligada);
                }
            }
        }
        int gerarEventoMAquinaOnOff(int codevt)
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Inicializar Circuitos
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = codevt;
                objEvt.DETALHES = "";
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }
        private void getLimpezaBico(bool force = false)
        {
            try
            {
                if (_parametros.TipoLimpBicos == 1)
                {
                    TimeSpan? ts = DateTime.Now.Subtract(this.dtControleLimpBicos);
                    if ((ts != null && ts.HasValue && ts.Value.TotalHours >= _parametros.DelayLimpBicos) ||
                        (force && DateTime.Now.Day != this.dtControleLimpBicosPurga.Day))
                    {
                        this.dtControleLimpBicosPurga = DateTime.Now;
                        this.dtControleLimpBicos = DateTime.Now;
                        using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_LimpBicos, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 30);
                        }
                        this.dtControleLimpBicos = DateTime.Now;
                        this.dtExecutadoLimpBicos = DateTime.Now;
                        Util.ObjectLimpBicos.UpdateExecutado(this.dtExecutadoLimpBicos);
                    }
                    ts = null;
                    
                    
                }
                //Periodos Configuráveis
                else
                {                    
                    if (this.listLimpBicosConfig != null && this.listLimpBicosConfig.Count > 0)
                    {
                        DateTime? dtAgora = DateTime.Now;
                        if ((this.dtExecutadoLimpBicos.Day != dtAgora.Value.Day) || (force && DateTime.Now.Day != this.dtControleLimpBicosPurga.Day))
                        {
                            this.dtControleLimpBicosPurga = DateTime.Now;
                            foreach (Util.ObjectLimpBicos lB in this.listLimpBicosConfig)
                            {
                                double totalHorasLbc = lB.Horario.Value.Hours * 60 + lB.Horario.Value.Minutes;
                                double totalHorasDia = dtAgora.Value.Hour * 60 + dtAgora.Value.Minute;
                                if (totalHorasDia >= totalHorasLbc)
                                {
                                    this.dtControleLimpBicos = DateTime.Now;
                                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_LimpBicos, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 30);
                                    }
                                    this.dtControleLimpBicos = DateTime.Now;
                                    this.dtExecutadoLimpBicos = DateTime.Now.AddMinutes(1);
                                    Util.ObjectLimpBicos.UpdateExecutado(this.dtExecutadoLimpBicos);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Util.ObjectLimpBicos lB in this.listLimpBicosConfig)
                            {
                                double totalHorasExec = this.dtExecutadoLimpBicos.Hour * 60 + this.dtExecutadoLimpBicos.Minute;
                                double totalHorasLbc = lB.Horario.Value.Hours * 60 + lB.Horario.Value.Minutes;
                                double totalHorasDia = dtAgora.Value.Hour * 60 + dtAgora.Value.Minute;
                                if (totalHorasExec <= totalHorasLbc && totalHorasDia >= totalHorasLbc)
                                {
                                    this.dtControleLimpBicos = DateTime.Now;
                                    using (fMensagem message = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        bool confirma = message.ShowDialog(Negocio.IdiomaResxExtensao.Global_LimpBicos, Negocio.IdiomaResxExtensao.Global_Sim, null, true, 30);
                                    }
                                    this.dtControleLimpBicos = DateTime.Now;
                                    this.dtExecutadoLimpBicos = DateTime.Now.AddMinutes(1);
                                    Util.ObjectLimpBicos.UpdateExecutado(this.dtExecutadoLimpBicos);
                                    break;
                                }
                            }
                        }
                        dtAgora = null;
                    }
                 
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void MonitProd()
        {
            try
            {
                #region Monitoramento de Produção
                try
                {
                    if (this.isMonitProducao)
                    {
                        //Caso HD Externo
                        if (isHDProducao)
                        {
                            int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitProducao).TotalMinutes;
                            if (tempoDelay >= this.timerDelayMonitProducao)
                            {
                                this.dtMonitProducao = DateTime.Now;
                                List<Negocio.Producao.ProducaoBD> lProdIntegracao = this.Prod.getListProducaoIntegrado(false);
                                if (lProdIntegracao != null)
                                {
                                    foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegracao)
                                    {
                                        this.Prod.upDateProducaoIntegracao(prodBD.id, 2);
                                    }
                                    List<Negocio.Producao.ProducaoBD> lProdIntegrado = this.Prod.getListProducaoIntegrado(2);
                                    foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegrado)
                                    {
                                        this.Prod.deleteProducaoIntegrado(prodBD.id);
                                        this.Prod.insertHistProducao(prodBD.conteudo, prodBD.dataHora);
                                    }
                                }
                            }
                        }
                        //Caso Programado para algum horário especifico
                        else if (isProgProducao)
                        {
                            double timeNow = DateTime.Now.TimeOfDay.TotalMinutes;
                            if (timeNow > this.tsProgProducao.TotalMinutes)
                            {
                                if (!this.transmitiuProgProducao)
                                {
                                    bool transmitiuProg = true;

                                    List<Negocio.Producao.ProducaoBD> lProdIntegracao = this.Prod.getListProducaoIntegrado(false);
                                    TCPProducao tcpProd = null;
                                    if (lProdIntegracao != null)
                                    {
                                        foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegracao)
                                        {
                                            if (tcpProd == null)
                                            {
                                                tcpProd = new TCPProducao(this._parametros);
                                            }
                                            string imei = prodBD.conteudo.Split('|')[0];
                                            string msgC = prodBD.dataHora + "|" + prodBD.conteudo;

                                            //byte[] ptW = montaPacoteProducao(imei, msgC);
                                            //if (isWriteTCP(ptW, imei))
                                            if (tcpProd.SendProducaoToTCP(imei, msgC))
                                            {
                                                Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Transmitiu produção TCP :" + msgC);
                                                this.Prod.upDateProducaoIntegracao(prodBD.id, true);
                                                // if (!this.Prod.upDateProducaoIntegracao(prodBD.id, true))
                                                // {
                                                //     transmitiuProg = false;
                                                //     break;
                                                //}
                                            }
                                            tcpProd.CloseProducaoTCP();
                                            if (File.Exists(_parametros.PathMonitoramentoDAT) || this.isNotifyMouseUp)
                                            {
                                                break;
                                            }

                                        }
                                        if (transmitiuProg || lProdIntegracao.Count <= 0)
                                        {
                                            this.transmitiuProgProducao = true;
                                        }
                                        List<Negocio.Producao.ProducaoBD> lProdIntegrado = this.Prod.getListProducaoIntegrado(true);
                                        foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegrado)
                                        {
                                            this.Prod.deleteProducaoIntegrado(prodBD.id);
                                            this.Prod.insertHistProducao(prodBD.conteudo, prodBD.dataHora);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                this.transmitiuProgProducao = false;
                            }

                        }
                        //Caso seja Online ou de um tempo especifo para transmitir 
                        else if (isProgProducaoOnLine)
                        {
                            int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitProducao).TotalMinutes;
                            if (tempoDelay >= this.timerDelayMonitProducao)
                            {
                                this.dtMonitProducao = DateTime.Now;
                                List<Negocio.Producao.ProducaoBD> lProdIntegracao = this.Prod.getListProducaoIntegrado(false);
                                TCPProducao tcpProd = null;
                                if (lProdIntegracao != null)
                                {
                                    foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegracao)
                                    {
                                        string imei = prodBD.conteudo.Split('|')[0];
                                        string msgC = prodBD.dataHora + "|" + prodBD.conteudo;
                                        if (tcpProd == null)
                                        {
                                            tcpProd = new TCPProducao(this._parametros);
                                        }

                                        //byte[] ptW = montaPacoteProducao(imei, msgC);                                    
                                        //if (isWriteTCP(ptW, imei))
                                        if (tcpProd.SendProducaoToTCP(imei, msgC))
                                        {
                                            this.timerDelayMonitProducao = 0;
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Transmitiu produção TCP :" + msgC);
                                            this.Prod.upDateProducaoIntegracao(prodBD.id, true);
                                        }
                                        else
                                        {
                                            this.timerDelayMonitProducao = 1;
                                        }
                                        tcpProd.CloseProducaoTCP();
                                        if (File.Exists(_parametros.PathMonitoramentoDAT) || this.isNotifyMouseUp)
                                        {
                                            break;
                                        }

                                    }

                                    List<Negocio.Producao.ProducaoBD> lProdIntegrado = this.Prod.getListProducaoIntegrado(true);
                                    foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegrado)
                                    {
                                        this.Prod.deleteProducaoIntegrado(prodBD.id);
                                        this.Prod.insertHistProducao(prodBD.conteudo, prodBD.dataHora);
                                    }
                                }
                            }
                        }
                        //Caso seja um tempo especifo para transmitir 
                        else
                        {
                            int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitProducao).TotalMinutes;
                            if (tempoDelay >= this.timerDelayMonitProducao)
                            {
                                this.dtMonitProducao = DateTime.Now;
                                List<Negocio.Producao.ProducaoBD> lProdIntegracao = this.Prod.getListProducaoIntegrado(false);
                                TCPProducao tcpProd = null;
                                if (lProdIntegracao != null)
                                {
                                    foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegracao)
                                    {
                                        string imei = prodBD.conteudo.Split('|')[0];
                                        string msgC = prodBD.dataHora + "|" + prodBD.conteudo;
                                        if (tcpProd == null)
                                        {
                                            tcpProd = new TCPProducao(this._parametros);
                                        }
                                        //byte[] ptW = montaPacoteProducao(imei, msgC);
                                        //if (isWriteTCP(ptW, imei))
                                        if (tcpProd.SendProducaoToTCP(imei, msgC))
                                        {
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Transmitiu produção TCP :" + msgC);
                                            this.Prod.upDateProducaoIntegracao(prodBD.id, true);
                                        }
                                        if (File.Exists(_parametros.PathMonitoramentoDAT) || this.isNotifyMouseUp)
                                        {
                                            break;
                                        }
                                    }

                                    List<Negocio.Producao.ProducaoBD> lProdIntegrado = this.Prod.getListProducaoIntegrado(true);
                                    foreach (Negocio.Producao.ProducaoBD prodBD in lProdIntegrado)
                                    {
                                        this.Prod.deleteProducaoIntegrado(prodBD.id);
                                        this.Prod.insertHistProducao(prodBD.conteudo, prodBD.dataHora);
                                    }
                                }
                            }

                        }

                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				    Log.Logar(
                    TipoLog.Processo,
                    _parametros.PathLogProcessoDispensa,
                    Negocio.IdiomaResxExtensao.Log_Cod_29 + Negocio.IdiomaResxExtensao.LogProcesso_FalhaLeituraConteudoDat);

                    /*Se ocorrer algum erro de leitura, abandona a rotina para que 
                    * monitoramento seja reiniciado, a existência do dat seja novamente 
                    * verificada e uma nova tentativa de leitura seja efetuada */
                    //ExecutarMonitoramento();

                    return;
                }
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void MonitEventos()
        {
            try
            {
                #region Monitoramento de Eventos
                
                if (this.isMonitEventos)
                {
                    //Caso HD Externo
                    if (isHDEventos)
                    {
                        int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitEventos).TotalMinutes;
                        if (tempoDelay >= this.timerDelayMonitProducao)
                        {
                            this.dtMonitEventos = DateTime.Now;
                            List<Util.ObjectEventos> lEvtIntegracao = Util.ObjectEventos.getListEventosIntegrado(false);
                            if (lEvtIntegracao != null)
                            {
                                foreach (Util.ObjectEventos evtBD in lEvtIntegracao)
                                {
                                    Util.ObjectEventos.UpdateEventoIntegrado(evtBD.Id, 2);
                                }
                                Util.ObjectEventos.GenerateBkp();
                            }
                        }
                    }
                    //Caso Programado para algum horário especifico
                    else if (isProgEventos)
                    {
                        double timeNow = DateTime.Now.TimeOfDay.TotalMinutes;
                        if (timeNow > this.tsProgEventos.TotalMinutes)
                        {
                            if (!this.transmitiuProgEventos)
                            {                                    
                                List<Util.ObjectEventos> lEvtIntegracao = Util.ObjectEventos.getListEventosIntegrado(false);
                                TCPEventos tcpEvt = null;
                                if (lEvtIntegracao != null)
                                {
                                    int _index_fim = 0;
                                    bool isFailTCP = false;
                                    foreach (Util.ObjectEventos evtBD in lEvtIntegracao)
                                    {
                                        if (tcpEvt == null)
                                        {
                                            tcpEvt = new TCPEventos(_parametros);
                                        }
                                        tcpEvt.SetObjEvt(evtBD);
                                        string msgC = "";

                                        if (tcpEvt.SendEventToTCP(ref msgC))
                                        {
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Send event TCP :" + msgC);
                                            Util.ObjectEventos.UpdateEventoIntegrado(evtBD.Id, true);

                                        }
                                        else
                                        {
                                            isFailTCP = true;
                                        }
                                        tcpEvt.CloseEventTCP();
                                        _index_fim++;
                                        if (_index_fim > 50 || File.Exists(_parametros.PathMonitoramentoDAT) || this.isNotifyMouseUp || isFailTCP)
                                        {
                                            break;
                                        }
                                    }

                                    if(!isFailTCP)
                                    {                                            
                                        if(_index_fim >= lEvtIntegracao.Count)
                                        {
                                            this.transmitiuProgEventos = true;
                                            Util.ObjectEventos.GenerateBkp();
                                        }
                                            
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.transmitiuProgEventos = false;
                        }

                    }
                    //Caso seja Online ou de um tempo especifo para transmitir 
                    else if (isProgEventosOnLine)
                    {
                        int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitEventos).TotalMinutes;
                        if (tempoDelay >= this.timerDelayMonitEventos)
                        {
                            this.dtMonitEventos = DateTime.Now;

                            List<Util.ObjectEventos> lEvtIntegracao = Util.ObjectEventos.getListEventosIntegrado(false);
                            TCPEventos tcpEvt = null;
                            if (lEvtIntegracao != null)
                            {
                                int _index_fim = 0;
                                bool isFailTCP = false;
                                foreach (Util.ObjectEventos evtBD in lEvtIntegracao)
                                {
                                    if (tcpEvt == null)
                                    {
                                        tcpEvt = new TCPEventos(_parametros);
                                    }
                                    tcpEvt.SetObjEvt(evtBD);
                                    string msgC = "";

                                    if (tcpEvt.SendEventToTCP(ref msgC))
                                    {
                                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Send event TCP :" + msgC);
                                        Util.ObjectEventos.UpdateEventoIntegrado(evtBD.Id, true);
                                    }
                                    else
                                    {
                                        int tempoDelayFailEvt = (int)DateTime.Now.Subtract(this.dtFailEvt).TotalMinutes;
                                        if (tempoDelayFailEvt >= 20)
                                        {
                                            dtFailEvt = DateTime.Now;
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Fail event TCP ");
                                        }
                                        isFailTCP = true;
                                    }
                                    tcpEvt.CloseEventTCP();
                                    _index_fim++;
                                    if (_index_fim > 50 || File.Exists(_parametros.PathMonitoramentoDAT) || this.isNotifyMouseUp || isFailTCP)
                                    {
                                        break;
                                    }
                                }

                                if (!isFailTCP)
                                {  
                                    if (_index_fim >= lEvtIntegracao.Count)
                                    {
                                        Util.ObjectEventos.GenerateBkp();
                                    }

                                }
                            }
                        }
                    }
                    //Caso seja um tempo especifo para transmitir 
                    else
                    {
                        int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitEventos).TotalMinutes;
                        if (tempoDelay >= this.timerDelayMonitEventos)
                        {
                            this.dtMonitEventos = DateTime.Now;
                            List<Util.ObjectEventos> lEvtIntegracao = Util.ObjectEventos.getListEventosIntegrado(false);
                            TCPEventos tcpEvt = null;
                            if (lEvtIntegracao != null)
                            {
                                int _index_fim = 0;
                                bool isFailTCP = false;
                                foreach (Util.ObjectEventos evtBD in lEvtIntegracao)
                                {
                                    if (tcpEvt == null)
                                    {
                                        tcpEvt = new TCPEventos(_parametros);
                                    }
                                    tcpEvt.SetObjEvt(evtBD);
                                    string msgC = "";

                                    if (tcpEvt.SendEventToTCP(ref msgC))
                                    {
                                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Send event TCP :" + msgC);
                                        Util.ObjectEventos.UpdateEventoIntegrado(evtBD.Id, true);
                                    }
                                    else
                                    {
                                        int tempoDelayFailEvt = (int)DateTime.Now.Subtract(this.dtFailEvt).TotalMinutes;
                                        if (tempoDelayFailEvt >= 20)
                                        {
                                            dtFailEvt = DateTime.Now;
                                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Fail event TCP ");
                                        }
                                        isFailTCP = true;
                                    }
                                    tcpEvt.CloseEventTCP();
                                    _index_fim++;
                                    if (_index_fim > 50 || File.Exists(_parametros.PathMonitoramentoDAT) || this.isNotifyMouseUp || isFailTCP)
                                    {
                                        break;
                                    }
                                }

                                if (!isFailTCP)
                                {
                                    if (_index_fim >= lEvtIntegracao.Count)
                                    {
                                        Util.ObjectEventos.GenerateBkp();
                                    }

                                }
                            }
                        }
                    }
                }

                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void MonitBkpCalibragem()
        {
            #region Monitoramento de Bkp Calibragem
            
            try
            {
                if (this.isMonitBkpCalibragem)
                {
                    int tempoDelay = (int)DateTime.Now.Subtract(this.dtMonitBkpCalibragem).TotalMinutes;
                    if (tempoDelay >= this.timerDelayMonitBkpCalibragem)
                    {
                        this.dtMonitBkpCalibragem = DateTime.Now;
                        string dir = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "bkp";
                        string[] files = Directory.GetFiles(dir);
                        if (files != null && files.Length > 0)
                        {
                            for (int i = 0; i < files.Length; i++)
                            {
                                string namefile = files[i];
                                string FileName = Path.GetFileName(namefile);
                                if (FileName.Contains(".zip"))
                                {
                                    string msgError = "";
                                    bool enviouFTP = FTPClient.EnviarArquivoFTP(namefile, _parametros.UrlSincBkpCalibragem + "/" + FileName, "Percolore_FTP", "avsb", ref msgError);
                                    if (!enviouFTP)
                                    {
                                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Fail BKP Calibracao FTP:" + msgError);
                                        break;
                                    }
                                    else
                                    {
                                        File.Delete(namefile);
                                    }
                                }
                                else
                                {
                                    File.Delete(namefile);
                                }
                            }
                        }
                        this.dtMonitBkpCalibragem = DateTime.Now;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    return;
            }

            #endregion
        }
        
        private void GenerateEventoOnLine()
        {
            try
            {
                #region Monitoramento de Eventos OnLine
                
                if (this.isMonitEventos)
                {
                    getMaquinaOnLine();
                }
                else
                {
                    this.machine_OnLine = false;
                    TimeSpan? ts = DateTime.Now.Subtract(this.dtControleMaquinaOnLine);
                    if (ts != null && ts.HasValue && ts.Value.TotalMinutes >= 60)
                    {
                        this.dtControleMaquinaOnLine = DateTime.Now;

                        gerarEventoMAquinaOnLine((int)IOConnect.Core.PercoloreEnum.Eventos.OffLine_MSP);

                    }
                }

                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void getMaquinaOnLine(bool forceConnect = false)
        {
          
            try
            {
                TimeSpan? ts = DateTime.Now.Subtract(this.dtControleMaquinaOnLine);
                if ((ts != null && ts.HasValue && ts.Value.TotalMinutes >= 60) || forceConnect)
                {
                    this.dtControleMaquinaOnLine = DateTime.Now;

                    string nameRemoteAccess = _parametros.NameRemoteAccess;
                    if(nameRemoteAccess.Contains(".exe"))
                    {
                        string trab = nameRemoteAccess.Substring(0, nameRemoteAccess.IndexOf(".exe"));
                        nameRemoteAccess = trab;
                    }
                    if(nameRemoteAccess.Contains(Path.DirectorySeparatorChar))
                    {
                        string[] array = nameRemoteAccess.Split(Path.DirectorySeparatorChar);
                        nameRemoteAccess = array[array.Length - 1];
                    }
                    Percolore.IOConnect.Core.RunRemoteAccessHelper runRemoteAccess = new Percolore.IOConnect.Core.RunRemoteAccessHelper(nameRemoteAccess);
                    if (runRemoteAccess.isRunRemoteAccess())
                    {
                        this.machine_OnLine = CheckInternet.TestInternet(_parametros.IpSincToken, _parametros.TimeoutPingTcp);
                    }
                    else
                    {
                        this.machine_OnLine = false;
                    }
                    runRemoteAccess = null;


                    if (this.machine_OnLine)
                    {
                        gerarEventoMAquinaOnLine((int)IOConnect.Core.PercoloreEnum.Eventos.OnLine_MSP);
                    }
                    else
                    {
                        gerarEventoMAquinaOnLine((int)IOConnect.Core.PercoloreEnum.Eventos.OffLine_MSP);
                    }

                    this.dtControleMaquinaOnLine = DateTime.Now;
                }
                ts = null;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        int gerarEventoMAquinaOnLine(int codevt)
        {
            int retorno = 0;
            try
            {
                #region gravar Evento OnLine ou OffLine
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = codevt;
                objEvt.DETALHES = "";
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private bool getRecirculacao(bool force = false)
        {
            bool retorno = false;
            #region Recirculação
            if (!_parametros.HabilitarRecirculacaoAuto && _parametros.HabilitarRecirculacao)
            {
                bool existeRecirculacao = false;
                try
                {
                    TimeSpan? ts = DateTime.Now.Subtract(this.dtRecircular);
                    if (ts != null && ts.HasValue && ts.Value.TotalHours < 0)
                    {
                        this.dtRecircular = DateTime.Now;
                    }
                    if ((ts != null && ts.HasValue && ConvertDoubletoInt(ts.Value.TotalHours) >= _parametros.DelayMonitRecirculacao) )
                    {
                        this.dtRecircular = DateTime.Now;
                        DateTime? dtAgora = DateTime.Now;
                        //foreach (Util.ObjectRecircular _rec in this._listaRecircular)
                        for (int iRec = 0; iRec < this._listaRecircular.Count; iRec++)
                        {
                            Util.ObjectRecircular _rec = this._listaRecircular[iRec];
                            TimeSpan? ts2 = DateTime.Now.Subtract(_rec.DtInicio);
                            if (ts2 != null && ts2.HasValue && ConvertDoubletoInt(ts2.Value.TotalDays) >= _rec.Dias)
                            {
                                if (_rec.VolumeDosado < _rec.VolumeDin)
                                {
                                    existeRecirculacao = true;
                                }
                                else
                                {
                                    this._listaRecircular[iRec].DtInicio = dtAgora.Value;
                                    this._listaRecircular[iRec].VolumeDosado = 0;
                                    Util.ObjectRecircular.Persist(this._listaRecircular[iRec]);

                                }
                            }
                            ts2 = null;
                        }
                        dtAgora = null;
                    }
                    ts = null;
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}

				if (existeRecirculacao || (force && DateTime.Now.Day != this.dtRecircularPurga.Day))
                {
                    this.dtRecircularPurga = DateTime.Now;
                    bool confirma = true;
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        //confirma = m.ShowDialog("Usuário desja Recircular os produtos?", Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.PainelControle_DesejaRecircular, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30, true, false);
                    }
                    if (confirma)
                    {
                        ShowMenu(OpcaoMenu.Recircular);
                    }
                    else
                    {
                        //Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { "Usuário cancelou Recirculação" });
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.Log_Cod_28 + Negocio.IdiomaResxExtensao.PainelControle_CancelouRecircular });
                    }
                    this.dtRecircular = DateTime.Now;
                    this.menu = false;
                    retorno = true;
                }
            }
            #endregion
            return retorno;
        }

        private bool getRecirculacaoAuto(bool force = false)
        {
            bool retorno = false;
            #region Recirculação Automática
            if (_parametros.HabilitarRecirculacaoAuto)
            {
                bool existeRecirculacao = false;
                try
                {
                    TimeSpan? ts = DateTime.Now.Subtract(this.dtRecircularAuto);
                    if (ts != null && ts.HasValue && ts.Value.TotalMinutes < 0)
                    {
                        this.dtRecircularAuto = DateTime.Now;
                        this.qtdTentativasRecirculaAUto = 0;
                    }
                    if ((ts != null && ts.HasValue && ConvertDoubletoInt(ts.Value.TotalMinutes) >= _parametros.DelayNotificacaotRecirculacaoAuto) || force)
                    {
                        this.dtRecircularAuto = DateTime.Now;
                        DateTime? dtAgora = DateTime.Now;
                        for (int iRec = 0; iRec < this._listaRecircularAuto.Count; iRec++)
                        {
                            Util.ObjectRecircular _rec = this._listaRecircularAuto[iRec];
                            TimeSpan? ts2 = DateTime.Now.Subtract(_rec.DtInicio);
                            if (ts2 != null && ts2.HasValue && ConvertDoubletoInt(ts2.Value.TotalMinutes) >= _parametros.DelayMonitRecirculacaoAuto)
                            {
                                existeRecirculacao = true;
                            }
                            ts2 = null;
                        }
                        dtAgora = null;
                    }
                    ts = null;
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}

				if (existeRecirculacao)
                {
                    bool confirma = true;
                    if (this.qtdTentativasRecirculaAUto < _parametros.QtdNotificacaotRecirculacaoAuto)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.PainelControle_DesejaRecircularAuto/*"Usuário deseja Recircular Automaticamente os produtos?"*/, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30, true, true);
                            //confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.PainelControle_DesejaRecircular, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                    }
                    if (confirma)
                    {
                        ShowMenu(OpcaoMenu.RecircularAutomatica);
                    }
                    else
                    {
                        this.qtdTentativasRecirculaAUto++;
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.Log_Cod_28 + Negocio.IdiomaResxExtensao.PainelControle_CancelouRecircularAuto/*"Usuário cancelou Recirculação Automática"*/ });
                        //Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, new string[] { Negocio.IdiomaResxExtensao.Log_Cod_28 + Negocio.IdiomaResxExtensao.PainelControle_CancelouRecircular });
                    }
                    this.dtRecircularAuto = DateTime.Now;
                    this.menu = false;
                    retorno = true;
                }
            }
            #endregion
            return retorno;
        }


    }
}