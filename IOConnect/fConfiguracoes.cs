using PaintMixer;
using Percolore.Core;
using Percolore.Core.AccessControl;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.UserControl;
using Percolore.Core.Util;
using Percolore.IOConnect.Util;
using System.Data;
using System.Globalization;
using System.IO.Ports;
using System.Reflection;
using System.Resources;
using System.Text;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace Percolore.IOConnect
{
	public partial class fConfiguracoes : Form
    {
        private UTextBox[] _NomeCorante = null;
        private CheckBox[] _circuito = null;
        private UTextBox[] _massaEspecifica = null;
        private ComboBox[] _correspondencia = null;
        private CheckBox[] _base = null;
        private CheckBox[] _bicoIndividual = null;

        private Label[] _nivelCKT = null;
        private UTextBox[] _nivelMinimo = null;
        private UTextBox[] _nivelMaximo = null;
        private UTextBox[] _purgaVolume = null;

        private List<ValoresVO> _historicoValores = new List<ValoresVO>();
        private Util.ObjectCalibragem _calibragem = null;
        private List<Util.ObjectColorante> _colorantes = null;
        private List<Util.ObjectColorante> _colorantesProd = null;
        private Util.ObjectParametros _parametros = null;

        private Util.ObjectCalibracaoAutomatica _calibracaoAuto = null;

        private List<Util.ObjectRecircular> _listRecircular = null;

        private Util.ObjectPlacaMovimentacao _PlacaMov = null;

        private string dataInstalacao = string.Empty;

        private bool isEnableEditGridView = false;
        private bool isEditFirstPulso = true;
        private DataTable dtCal = new DataTable();

        private DataTable dtCalAuto = new DataTable();

        fCalibracaoAutomatica fCalAuto = null;
        fAguarde _fAguarde = null;

        private bool confirmaCalNew = true;

        private Label[] _recircularCKT = null;
        private UTextBox[] _recircularVolDin = null;
        private UTextBox[] _recircularDias = null;
        private UTextBox[] _recircularVol = null;
        private CheckBox[] _recircularValve = null;
        private CheckBox[] _recircularAuto = null;

        private ToolTip _toolTip = new ToolTip();

        public List<Util.ObjectUser> listUsuarios = new List<Util.ObjectUser>();
        private int index_selGridUser;
        private ObjectUser User;

        private ToolTip _toolTipProducts = new ToolTip();
        private bool isUpdateCalDGV = false;        

        public List<Util.ObjectAbastecimento> listAbastecimento = new List<Util.ObjectAbastecimento>();
        private int index_selGridAbastecimento;

        public List<Util.ObjectLimpBicos> listLimpBicos = new List<Util.ObjectLimpBicos>();
        private int index_selGridLimpBicos;

        private bool alterTabCalibracaoSave = false;
        Authentication authentication;

        public fConfiguracoes(Authentication _authentication, ObjectUser user)
        {
            InitializeComponent();
            this.User = user;
            authentication = _authentication;
            this._toolTip.ToolTipIcon = ToolTipIcon.None;
            this._toolTip.IsBalloon = true;
            this._toolTip.ShowAlways = true;
            this.alterTabCalibracaoSave = false;

            this._toolTipProducts.ToolTipIcon = ToolTipIcon.None;
            this._toolTipProducts.IsBalloon = true;
            this._toolTipProducts.ShowAlways = true;
            
            int X = (Screen.PrimaryScreen.Bounds.Width) - (this.Width);
            int Y = 30;
            this.Location = new Point(X, Y);

            _parametros = Util.ObjectParametros.Load();
            _colorantes = Util.ObjectColorante.List();
          
            #region Imagens dos botões

            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnTeclado.Text = string.Empty;
            btnTeclado.Image = Imagem.GetTeclado_32x32();
            btnGravar.Text = string.Empty;
            btnGravar.Image = Imagem.GetGravar_32x32();

            btnPathLogProcDispensa.Image = Imagem.GetEditar_32x32();
            btnPathLogProcDispensa.Text = string.Empty;
            btnPathLogQtdeDispensa.Image = Imagem.GetEditar_32x32();
            btnPathLogQtdeDispensa.Text = string.Empty;
            btnPathLogComunicacao.Image = Imagem.GetEditar_32x32();
            btnPathLogComunicacao.Text = string.Empty;
            btnPathMonitoramentoDat.Image = Imagem.GetEditar_32x32();
            btnPathMonitoramentoDat.Text = string.Empty;

            btnPathMonitoramentoFilaDat.Image = Imagem.GetEditar_32x32();
            btnPathMonitoramentoFilaDat.Text = string.Empty;

            this.btnFunctiongeral.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralFuncinamentoSoftware;

            #endregion

            #region Configura arrays de controles

            _NomeCorante = new UTextBox[] {
                txtColorante0, txtColorante1, txtColorante2, txtColorante3,
                txtColorante4, txtColorante5, txtColorante6, txtColorante7,
                txtColorante8, txtColorante9, txtColorante10, txtColorante11,
                txtColorante12, txtColorante13, txtColorante14, txtColorante15,

                txtColorante16, txtColorante17, txtColorante18, txtColorante19,
                txtColorante20, txtColorante21, txtColorante22, txtColorante23,
                txtColorante24, txtColorante25, txtColorante26, txtColorante27,
                txtColorante28, txtColorante29, txtColorante30, txtColorante31
                };

            _circuito = new CheckBox[] {
                chkCircuito00, chkCircuito01, chkCircuito02, chkCircuito03,
                chkCircuito04, chkCircuito05, chkCircuito06, chkCircuito07,
                chkCircuito08, chkCircuito09, chkCircuito10, chkCircuito11,
                chkCircuito12, chkCircuito13, chkCircuito14, chkCircuito15,
                chkCircuito16, chkCircuito17, chkCircuito18, chkCircuito19,
                chkCircuito20, chkCircuito21, chkCircuito22, chkCircuito23,
                chkCircuito24, chkCircuito25, chkCircuito26, chkCircuito27,
                chkCircuito28, chkCircuito29, chkCircuito30, chkCircuito31

            };

            _massaEspecifica = new UTextBox[] {
                txtMassaEsp0, txtMassaEsp1, txtMassaEsp2, txtMassaEsp3,
                txtMassaEsp4, txtMassaEsp5, txtMassaEsp6, txtMassaEsp7,
                txtMassaEsp8, txtMassaEsp9, txtMassaEsp10, txtMassaEsp11,
                txtMassaEsp12, txtMassaEsp13, txtMassaEsp14, txtMassaEsp15,
                txtMassaEsp16, txtMassaEsp17, txtMassaEsp18, txtMassaEsp19,
                txtMassaEsp20, txtMassaEsp21, txtMassaEsp22, txtMassaEsp23,
                txtMassaEsp24, txtMassaEsp25, txtMassaEsp26, txtMassaEsp27,
                txtMassaEsp28, txtMassaEsp29, txtMassaEsp30, txtMassaEsp31
            };

            _correspondencia = new ComboBox[] {
                cmbCorrespondencia0, cmbCorrespondencia1, cmbCorrespondencia2, cmbCorrespondencia3,
                cmbCorrespondencia4, cmbCorrespondencia5, cmbCorrespondencia6, cmbCorrespondencia7,
                cmbCorrespondencia8, cmbCorrespondencia9, cmbCorrespondencia10, cmbCorrespondencia11,
                cmbCorrespondencia12, cmbCorrespondencia13, cmbCorrespondencia14, cmbCorrespondencia15,

                cmbCorrespondencia16, cmbCorrespondencia17, cmbCorrespondencia18, cmbCorrespondencia19,
                cmbCorrespondencia20, cmbCorrespondencia21, cmbCorrespondencia22, cmbCorrespondencia23,
                cmbCorrespondencia24, cmbCorrespondencia25, cmbCorrespondencia26, cmbCorrespondencia27,
                cmbCorrespondencia28, cmbCorrespondencia29, cmbCorrespondencia30, cmbCorrespondencia31
            };

            _base = new CheckBox[]
            {
                chkBase00, chkBase01, chkBase02, chkBase03,
                chkBase04, chkBase05, chkBase06, chkBase07,
                chkBase08, chkBase09, chkBase10, chkBase11,
                chkBase12, chkBase13, chkBase14, chkBase15,

                chkBase16, chkBase17, chkBase18, chkBase19,
                chkBase20, chkBase21, chkBase22, chkBase23,
                chkBase24, chkBase25, chkBase26, chkBase27,
                chkBase28, chkBase29, chkBase30, chkBase31
            };

            _bicoIndividual = new CheckBox[]
           {
                chkBicoInd00, chkBicoInd01, chkBicoInd02, chkBicoInd03,
                chkBicoInd04, chkBicoInd05, chkBicoInd06, chkBicoInd07,
                chkBicoInd08, chkBicoInd09, chkBicoInd10, chkBicoInd11,
                chkBicoInd12, chkBicoInd13, chkBicoInd14, chkBicoInd15,

                chkBicoInd16, chkBicoInd17, chkBicoInd18, chkBicoInd19,
                chkBicoInd20, chkBicoInd21, chkBicoInd22, chkBicoInd23,
                chkBicoInd24, chkBicoInd25, chkBicoInd26, chkBicoInd27,
                chkBicoInd28, chkBicoInd29, chkBicoInd30, chkBicoInd31
           };

            _nivelCKT = new Label[] {
                lbl_Col_CKT_01, lbl_Col_CKT_02, lbl_Col_CKT_03, lbl_Col_CKT_04,
                lbl_Col_CKT_05, lbl_Col_CKT_06, lbl_Col_CKT_07, lbl_Col_CKT_08,
                lbl_Col_CKT_09, lbl_Col_CKT_10, lbl_Col_CKT_11, lbl_Col_CKT_12,
                lbl_Col_CKT_13, lbl_Col_CKT_14, lbl_Col_CKT_15, lbl_Col_CKT_16,

                lbl_Col_CKT_17, lbl_Col_CKT_18, lbl_Col_CKT_19, lbl_Col_CKT_20,
                lbl_Col_CKT_21, lbl_Col_CKT_22, lbl_Col_CKT_23, lbl_Col_CKT_24,
                lbl_Col_CKT_25, lbl_Col_CKT_26, lbl_Col_CKT_27, lbl_Col_CKT_28,
                lbl_Col_CKT_29, lbl_Col_CKT_30, lbl_Col_CKT_31, lbl_Col_CKT_32
            };

            _nivelMinimo = new UTextBox[] {
                txtVolumeMinimo_01, txtVolumeMinimo_02, txtVolumeMinimo_03, txtVolumeMinimo_04,
                txtVolumeMinimo_05, txtVolumeMinimo_06, txtVolumeMinimo_07, txtVolumeMinimo_08,
                txtVolumeMinimo_09, txtVolumeMinimo_10, txtVolumeMinimo_11, txtVolumeMinimo_12,
                txtVolumeMinimo_13, txtVolumeMinimo_14, txtVolumeMinimo_15, txtVolumeMinimo_16,

                txtVolumeMinimo_17, txtVolumeMinimo_18, txtVolumeMinimo_19, txtVolumeMinimo_20,
                txtVolumeMinimo_21, txtVolumeMinimo_22, txtVolumeMinimo_23, txtVolumeMinimo_24,
                txtVolumeMinimo_25, txtVolumeMinimo_26, txtVolumeMinimo_27, txtVolumeMinimo_28,
                txtVolumeMinimo_29, txtVolumeMinimo_30, txtVolumeMinimo_31, txtVolumeMinimo_32
            };

            _nivelMaximo = new UTextBox[] {
                txtVolumeMaximo_01, txtVolumeMaximo_02, txtVolumeMaximo_03, txtVolumeMaximo_04,
                txtVolumeMaximo_05, txtVolumeMaximo_06, txtVolumeMaximo_07, txtVolumeMaximo_08,
                txtVolumeMaximo_09, txtVolumeMaximo_10, txtVolumeMaximo_11, txtVolumeMaximo_12,
                txtVolumeMaximo_13, txtVolumeMaximo_14, txtVolumeMaximo_15, txtVolumeMaximo_16,

                txtVolumeMaximo_17, txtVolumeMaximo_18, txtVolumeMaximo_19, txtVolumeMaximo_20,
                txtVolumeMaximo_21, txtVolumeMaximo_22, txtVolumeMaximo_23, txtVolumeMaximo_24,
                txtVolumeMaximo_25, txtVolumeMaximo_26, txtVolumeMaximo_27, txtVolumeMaximo_28,
                txtVolumeMaximo_29, txtVolumeMaximo_30, txtVolumeMaximo_31, txtVolumeMaximo_32
            };

            _purgaVolume = new UTextBox[] {
                txtPurga0, txtPurga1, txtPurga2, txtPurga3,
                txtPurga4, txtPurga5, txtPurga6, txtPurga7,
                txtPurga8, txtPurga9, txtPurga10, txtPurga11,
                txtPurga12, txtPurga13, txtPurga14, txtPurga15,

                txtPurga16, txtPurga17, txtPurga18, txtPurga19,
                txtPurga20, txtPurga21, txtPurga22, txtPurga23,
                txtPurga24, txtPurga25, txtPurga26, txtPurga27,
                txtPurga28, txtPurga29, txtPurga30, txtPurga31
            };

            _recircularCKT = new Label[] {
                lblRecCircuito01, lblRecCircuito02, lblRecCircuito03, lblRecCircuito04,
                lblRecCircuito05, lblRecCircuito06, lblRecCircuito07, lblRecCircuito08,
                lblRecCircuito09, lblRecCircuito10, lblRecCircuito11, lblRecCircuito12,
                lblRecCircuito13, lblRecCircuito14, lblRecCircuito15, lblRecCircuito16,

                lblRecCircuito17, lblRecCircuito18, lblRecCircuito19, lblRecCircuito20,
                lblRecCircuito21, lblRecCircuito22, lblRecCircuito23, lblRecCircuito24,
                lblRecCircuito25, lblRecCircuito26, lblRecCircuito27, lblRecCircuito28,
                lblRecCircuito29, lblRecCircuito30, lblRecCircuito31, lblRecCircuito32
            };
            
            _recircularVolDin = new UTextBox[] {
                txtRecirculacaoVolDin01, txtRecirculacaoVolDin02, txtRecirculacaoVolDin03, txtRecirculacaoVolDin04,
                txtRecirculacaoVolDin05, txtRecirculacaoVolDin06, txtRecirculacaoVolDin07, txtRecirculacaoVolDin08,
                txtRecirculacaoVolDin09, txtRecirculacaoVolDin10, txtRecirculacaoVolDin11, txtRecirculacaoVolDin12,
                txtRecirculacaoVolDin13, txtRecirculacaoVolDin14, txtRecirculacaoVolDin15, txtRecirculacaoVolDin16,

                txtRecirculacaoVolDin17, txtRecirculacaoVolDin18, txtRecirculacaoVolDin19, txtRecirculacaoVolDin20,
                txtRecirculacaoVolDin21, txtRecirculacaoVolDin22, txtRecirculacaoVolDin23, txtRecirculacaoVolDin24,
                txtRecirculacaoVolDin25, txtRecirculacaoVolDin26, txtRecirculacaoVolDin27, txtRecirculacaoVolDin28,
                txtRecirculacaoVolDin29, txtRecirculacaoVolDin30, txtRecirculacaoVolDin31, txtRecirculacaoVolDin32
            };

            _recircularDias = new UTextBox[] {
                txtRecirculacaoDias01, txtRecirculacaoDias02, txtRecirculacaoDias03, txtRecirculacaoDias04,
                txtRecirculacaoDias05, txtRecirculacaoDias06, txtRecirculacaoDias07, txtRecirculacaoDias08,
                txtRecirculacaoDias09, txtRecirculacaoDias10, txtRecirculacaoDias11, txtRecirculacaoDias12,
                txtRecirculacaoDias13, txtRecirculacaoDias14, txtRecirculacaoDias15, txtRecirculacaoDias16,

                txtRecirculacaoDias17, txtRecirculacaoDias18, txtRecirculacaoDias19, txtRecirculacaoDias20,
                txtRecirculacaoDias21, txtRecirculacaoDias22, txtRecirculacaoDias23, txtRecirculacaoDias24,
                txtRecirculacaoDias25, txtRecirculacaoDias26, txtRecirculacaoDias27, txtRecirculacaoDias28,
                txtRecirculacaoDias29, txtRecirculacaoDias30, txtRecirculacaoDias31, txtRecirculacaoDias32
            };

            _recircularVol = new UTextBox[] {
                txtRecirculacaoVol01, txtRecirculacaoVol02, txtRecirculacaoVol03, txtRecirculacaoVol04,
                txtRecirculacaoVol05, txtRecirculacaoVol06, txtRecirculacaoVol07, txtRecirculacaoVol08,
                txtRecirculacaoVol09, txtRecirculacaoVol10, txtRecirculacaoVol11, txtRecirculacaoVol12,
                txtRecirculacaoVol13, txtRecirculacaoVol14, txtRecirculacaoVol15, txtRecirculacaoVol16,

                txtRecirculacaoVol17, txtRecirculacaoVol18, txtRecirculacaoVol19, txtRecirculacaoVol20,
                txtRecirculacaoVol21, txtRecirculacaoVol22, txtRecirculacaoVol23, txtRecirculacaoVol24,
                txtRecirculacaoVol25, txtRecirculacaoVol26, txtRecirculacaoVol27, txtRecirculacaoVol28,
                txtRecirculacaoVol29, txtRecirculacaoVol30, txtRecirculacaoVol31, txtRecirculacaoVol32
            };

            _recircularValve = new CheckBox[] {
                chkRecirculacaoisValve01, chkRecirculacaoisValve02, chkRecirculacaoisValve03, chkRecirculacaoisValve04,
                chkRecirculacaoisValve05, chkRecirculacaoisValve06, chkRecirculacaoisValve07, chkRecirculacaoisValve08,
                chkRecirculacaoisValve09, chkRecirculacaoisValve10, chkRecirculacaoisValve11, chkRecirculacaoisValve12,
                chkRecirculacaoisValve13, chkRecirculacaoisValve14, chkRecirculacaoisValve15, chkRecirculacaoisValve16,

                chkRecirculacaoisValve17, chkRecirculacaoisValve18, chkRecirculacaoisValve19, chkRecirculacaoisValve20,
                chkRecirculacaoisValve21, chkRecirculacaoisValve22, chkRecirculacaoisValve23, chkRecirculacaoisValve24,
                chkRecirculacaoisValve25, chkRecirculacaoisValve26, chkRecirculacaoisValve27, chkRecirculacaoisValve28,
                chkRecirculacaoisValve29, chkRecirculacaoisValve30, chkRecirculacaoisValve31, chkRecirculacaoisValve32
            };

            _recircularAuto = new CheckBox[] {
                chkRecirculacaoisAutomatico01, chkRecirculacaoisAutomatico02, chkRecirculacaoisAutomatico03, chkRecirculacaoisAutomatico04,
                chkRecirculacaoisAutomatico05, chkRecirculacaoisAutomatico06, chkRecirculacaoisAutomatico07, chkRecirculacaoisAutomatico08,
                chkRecirculacaoisAutomatico09, chkRecirculacaoisAutomatico10, chkRecirculacaoisAutomatico11, chkRecirculacaoisAutomatico12,
                chkRecirculacaoisAutomatico13, chkRecirculacaoisAutomatico14, chkRecirculacaoisAutomatico15, chkRecirculacaoisAutomatico16,

                chkRecirculacaoisAutomatico17, chkRecirculacaoisAutomatico18, chkRecirculacaoisAutomatico19, chkRecirculacaoisAutomatico20,
                chkRecirculacaoisAutomatico21, chkRecirculacaoisAutomatico22, chkRecirculacaoisAutomatico23, chkRecirculacaoisAutomatico24,
                chkRecirculacaoisAutomatico25, chkRecirculacaoisAutomatico26, chkRecirculacaoisAutomatico27, chkRecirculacaoisAutomatico28,
                chkRecirculacaoisAutomatico29, chkRecirculacaoisAutomatico30, chkRecirculacaoisAutomatico31, chkRecirculacaoisAutomatico32
            };

            #endregion

            #region Controle de acesso

            Permissions permissions = authentication.Permissions;

            for (int i = 0; i < 32; i++)
            {
                _circuito[i].Enabled = permissions.HabilitarCircuitoColorante;
            }

            chkControlarNivel.Enabled =
               permissions.HabilitarControleNivelColorante;

            tabUnidadeMedida.Enabled =
                permissions.EditarInformacoesAbaUnidadeMedida;

            tabInicializarCircuito.Enabled =
                permissions.EditarInformacoesAbaInicializacaoCircuitos;

            tabParametrosPurga.Enabled =
                permissions.EditarInformacoesAbaPurga;

            if (!permissions.EditarInformacoesAbaSenha)
                tab.Controls.Remove(tabAdmin);

            #endregion

            #region Carrega lista de dispositivos

            //Resource da aplicação
            ResourceManager resource = null;
            if (_parametros.IdIdioma == 2)
            {
                resource = new ResourceManager(typeof(Properties.IOConnect_esp));
            }
            else if (_parametros.IdIdioma == 3)
            {
                resource = new ResourceManager(typeof(Properties.IOConnect_eng));
            }
            else
            {
                resource = new ResourceManager(typeof(Properties.IOConnect));
            }

            /* Popula combo */
            cmbGeralDispositivo.DisplayMember = "Display";
            cmbGeralDispositivo.ValueMember = "Value";
            cmbGeralDispositivo.DataSource =
                EnumHelper.ToComboItemList<Dispositivo>(resource);
            cmbGeralDispositivo.SelectedIndex = 0;

            cmbGeralDispositivoUser.DisplayMember = "Display";
            cmbGeralDispositivoUser.ValueMember = "Value";
            cmbGeralDispositivoUser.DataSource =
                EnumHelper.ToComboItemList<Dispositivo>(resource);
            cmbGeralDispositivoUser.SelectedIndex = 0;

            cmbGeralDispositivo_2.DisplayMember = "Display";
            cmbGeralDispositivo_2.ValueMember = "Value";
            cmbGeralDispositivo_2.DataSource =
                EnumHelper.ToComboItemList<Dispositivo2>(resource); 
            cmbGeralDispositivo_2.SelectedIndex = 0;

            cmbGeralDispositivo_2User.DisplayMember = "Display";
            cmbGeralDispositivo_2User.ValueMember = "Value";
            cmbGeralDispositivo_2User.DataSource =
                EnumHelper.ToComboItemList<Dispositivo2>(resource);
            cmbGeralDispositivo_2User.SelectedIndex = 0;

            DataTable dtTipoBaseDados = new DataTable();
            dtTipoBaseDados.Columns.Add("Valor");
            dtTipoBaseDados.Columns.Add("Descricao");
            for (int i = 0; i < 2; i++)
            {
                DataRow dr = dtTipoBaseDados.NewRow();
                dr["Valor"] = i.ToString();
                if (i == 0)
                {
                    dr["Descricao"] = "Local";
                }
                else
                {
                    dr["Descricao"] = "Web Api";
                }

                dtTipoBaseDados.Rows.Add(dr);
            }
            cmbTipoBaseDados.DisplayMember = "Descricao";
            cmbTipoBaseDados.ValueMember = "Valor";
            cmbTipoBaseDados.DataSource = dtTipoBaseDados.DefaultView;
            cmbTipoBaseDados.SelectedIndex = -1;

            DataTable dtTipoMotorPlacaMov = new DataTable();
            dtTipoMotorPlacaMov.Columns.Add("Valor");
            dtTipoMotorPlacaMov.Columns.Add("Descricao");
            for (int i = 0; i < 2; i++)
            {
                DataRow dr = dtTipoMotorPlacaMov.NewRow();
                dr["Valor"] = i.ToString();
                if (i == 0)
                {
                    dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_Item_01_Motor;
                }
                else
                {
                    dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_Item_02_Motor;
                }

                dtTipoMotorPlacaMov.Rows.Add(dr);
            }
            cb_tipo_motor_PlacaMov.DisplayMember = "Descricao";
            cb_tipo_motor_PlacaMov.ValueMember = "Valor";
            cb_tipo_motor_PlacaMov.DataSource = dtTipoMotorPlacaMov.DefaultView;
            cb_tipo_motor_PlacaMov.SelectedIndex = -1;

            DataTable dtDatPadrao = new DataTable();
            dtDatPadrao.Columns.Add("Valor");
            dtDatPadrao.Columns.Add("Descricao");
           
            DataRow drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao01;
            drDat["Valor"] = (int)DatPattern.Padrao01 + "";
            dtDatPadrao.Rows.Add(drDat);
            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao02;
            drDat["Valor"] = (int)DatPattern.Padrao02 + "";
            dtDatPadrao.Rows.Add(drDat);
            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao03;
            drDat["Valor"] = (int)DatPattern.Padrao03 + "";
            dtDatPadrao.Rows.Add(drDat);
            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao04;
            drDat["Valor"] = (int)DatPattern.Padrao04 + "";
            dtDatPadrao.Rows.Add(drDat);
            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao05;
            drDat["Valor"] = (int)DatPattern.Padrao05 + "";
            dtDatPadrao.Rows.Add(drDat);
            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao06;
            drDat["Valor"] = (int)DatPattern.Padrao06 + "";
            dtDatPadrao.Rows.Add(drDat);

            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao07;
            drDat["Valor"] = (int)DatPattern.Padrao07 + "";
            dtDatPadrao.Rows.Add(drDat);

            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadraoUDCP;
            drDat["Valor"] = (int)DatPattern.PadraoUDCP + "";
            dtDatPadrao.Rows.Add(drDat);

            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = "Sinteplast";
            drDat["Valor"] = (int)DatPattern.PadraoSinteplast + "";
            dtDatPadrao.Rows.Add(drDat);

            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = "Modo 10";
            drDat["Valor"] = (int)DatPattern.Padrao10 + "";
            dtDatPadrao.Rows.Add(drDat);

            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = "Modo 11";
            drDat["Valor"] = (int)DatPattern.Padrao11 + "";
            dtDatPadrao.Rows.Add(drDat);

            drDat = dtDatPadrao.NewRow();
            drDat["Descricao"] = "Modo 12";
            drDat["Valor"] = (int)DatPattern.Padrao12 + "";
            dtDatPadrao.Rows.Add(drDat);

            cmbDatPadrao.DisplayMember = "Descricao";
            cmbDatPadrao.ValueMember = "Valor";
            cmbDatPadrao.DataSource = dtDatPadrao.DefaultView;

            DataTable dtTipoDosagem = new DataTable();
            dtTipoDosagem.Columns.Add("Valor");
            dtTipoDosagem.Columns.Add("Descricao");

            DataRow drTipoDos = dtTipoDosagem.NewRow();
            drTipoDos["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemCircuito;
            drTipoDos["Valor"] = (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Circuito + "";
            dtTipoDosagem.Rows.Add(drTipoDos);

            drTipoDos = dtTipoDosagem.NewRow();
            drTipoDos["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemBase;
            drTipoDos["Valor"] = (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases + "";
            dtTipoDosagem.Rows.Add(drTipoDos);

            drTipoDos = dtTipoDosagem.NewRow();
            drTipoDos["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemColorante;
            drTipoDos["Valor"] = (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes + "";
            dtTipoDosagem.Rows.Add(drTipoDos);

            cmbTipoDosagemExec.DisplayMember = "Descricao";
            cmbTipoDosagemExec.ValueMember = "Valor";
            cmbTipoDosagemExec.DataSource = dtTipoDosagem.DefaultView;

            DataTable dtTipoDosagem_2 = new DataTable();
            dtTipoDosagem_2.Columns.Add("Valor");
            dtTipoDosagem_2.Columns.Add("Descricao");

            DataRow drTipoDos_2 = dtTipoDosagem_2.NewRow();
            drTipoDos_2["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemCircuito;
            drTipoDos_2["Valor"] = (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Circuito + "";
            dtTipoDosagem_2.Rows.Add(drTipoDos_2);

            drTipoDos_2 = dtTipoDosagem_2.NewRow();
            drTipoDos_2["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemBase;
            drTipoDos_2["Valor"] = (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Bases + "";
            dtTipoDosagem_2.Rows.Add(drTipoDos_2);

            drTipoDos_2 = dtTipoDosagem_2.NewRow();
            drTipoDos_2["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemColorante;
            drTipoDos_2["Valor"] = (int)Percolore.IOConnect.Core.PercoloreEnum.TipoDosagem.Colorantes + "";
            dtTipoDosagem_2.Rows.Add(drTipoDos_2);

            cmbTipoDosagemExec_2.DisplayMember = "Descricao";
            cmbTipoDosagemExec_2.ValueMember = "Valor";
            cmbTipoDosagemExec_2.DataSource = dtTipoDosagem_2.DefaultView;

            DataTable dtTipoRecAuto = new DataTable();
            dtTipoRecAuto.Columns.Add("Valor");
            dtTipoRecAuto.Columns.Add("Descricao");

            DataRow drTipoDos_3 = dtTipoRecAuto.NewRow();
            drTipoDos_3["Descricao"] = "h";
            drTipoDos_3["Valor"] = 1 + "";
            dtTipoRecAuto.Rows.Add(drTipoDos_3);

            drTipoDos_3 = dtTipoRecAuto.NewRow();
            drTipoDos_3["Descricao"] = "m";
            drTipoDos_3["Valor"] = 2 + "";
            dtTipoRecAuto.Rows.Add(drTipoDos_3);

            cbTipoTempoMonitRecirculacaoAuto.DisplayMember = "Descricao";
            cbTipoTempoMonitRecirculacaoAuto.ValueMember = "Valor";
            cbTipoTempoMonitRecirculacaoAuto.DataSource = dtTipoRecAuto.DefaultView;

            #endregion
        }

        #region Eventos

        private void Configuracoes_Load(object sender, EventArgs e)
        {
            cb_CalibragemColorante.SelectedIndex = -1;
            cb_CalibracaoAuto.SelectedIndex = -1;
            
            btnProductPar.Image = Imagem.GetConfigurar_32x32();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            lblTitulo.Text =
                $"IOConnect "
                + $"{version.Major}.{version.Minor}.{version.Build}"
                + $" | {Negocio.IdiomaResxExtensao.Configuracoes_lblTitulo}";
            version = null;

            #region Abas
            
            this.tabParametrosGerais.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabParametrosGerais;
            this.tabParametrosGeraisUser.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabParametrosGerais;
            this.tabDAT.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabDAT;
            this.tabParametrosPurga.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabParametrosPurga;
            this.tabCalibragemManual.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabCalibragemManual;
            this.tabCalibragemAuto.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabCalibragemAuto;
            this.tabInicializarCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabInicializarCircuito;
            this.tabUnidadeMedida.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabUnidadeMedida;
            this.tabLog.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabLog;
            this.tabAdmin.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabAdmin;
            this.tabMonitCircuitos.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabMonit;
            this.tabProducao.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabProducao;
            this.tabRecirculacaoProd.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabRecircular;
            this.tabLimpBicos.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabLimpBicos;
            
            this.tabProdutos.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos;
            #endregion

            try
            {
                #region Geral

                this.lblGeralComunicacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralComunicacao;
                this.lblGeralTimeout.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralTimeout;

                this.lblQtdTentativasConexao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblQtdTentativasConexao;
                
                this.lblGeralParametroDispensaGlobal.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralParametroDispensaGlobal;
                this.lblGeralVelocidade.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralVelocidade;
                this.lblGeralAceleracao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralAceleracao;
                this.lblGeralDelayReverso.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDelayReverso;
                this.lblGeralPulsoReverso.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralPulsoReverso;
                this.chkGeralSomarRevSteps.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkGeralSomarRevSteps;
               
                this.lblGeralDispositivo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDispositivo;
               
                this.chkTecladoVirtualUser.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkTecladoVirtual;

                this.lblGeralDispositivoUser.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDispositivo;

                this.lblGeralDispositivo2User.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDispositivo;

                chkHabilitarPurgaIndividual.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkHabilitarPurgaIndividual;
                chkDispensaSequencialP1.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDispensaSequencialP1;
                chkDispensaSequencialP2.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDispensaSequencialP2;
                lblNomeP1.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNomeP1;
                lblNomeP2.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNomeP2;

                lblNomeP1User.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNomeP1;
                lblNomeP2User.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNomeP2;

                lblGeralDispositivo2.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDispositivo + " 2";
                btnTesteComunicacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnTesteComunicacao;

                btnTesteComunicacaoUser.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnTesteComunicacao;

                txtTimeout.Text = _parametros.ResponseTimeout.ToString();
                txtVelocidadeGlobal.Text = _parametros.Velocidade.ToString();
                txtAceleracaoGlobal.Text = _parametros.Aceleracao.ToString();
                txtReversoDelay.Text = _parametros.DelayReverso.ToString();
                txtReversoPulsos.Text = _parametros.PulsoReverso.ToString();
                chkGeralSomarRevSteps.Checked = _parametros.SomarPulsoReverso;
              
                txt_QtdTentativasConexao.Text = _parametros.QtdTentativasConexao.ToString();

                chkTecladoVirtualUser.Checked = _parametros.HabilitarTecladoVirtual;

                chkDispensaSequencialP1.Checked = _parametros.HabilitarDispensaSequencialP1;
                chkDispensaSequencialP2.Checked = _parametros.HabilitarDispensaSequencialP2;
              
                txtPathLogProcDispensa.Text = _parametros.PathLogProcessoDispensa;
                txtPathLogQtdeDispensa.Text = _parametros.PathLogControleDispensa;
                chkLogComunicacao.Checked = _parametros.HabilitarLogComunicacao;
                txtPathLogComunicacao.Text = _parametros.PathLogComunicacao;

                chkHabilitarPurgaIndividual.Checked = _parametros.HabilitarPurgaIndividual;

                chkTouchScrennUser.Checked = _parametros.HabilitarTouchScrenn;

                /* 23.05.2017
                 * Seta dispositivo e dispara evento da combo para aplicar 
                 * regras de configuração da interface. */
                if (this.User.Tipo == 1)
                {
                    cmbGeralDispositivo.SelectedValue = _parametros.IdDispositivo;
                    cmbGeralDispositivo_SelectionChangeCommitted(sender, e);

                    cmbGeralDispositivo_2.SelectedValue = _parametros.IdDispositivo2;
                    txt_nome_Dispositivo.Text = _parametros.NomeDispositivo != null ? _parametros.NomeDispositivo : "";
                    txt_nome_Dispositivo_2.Text = _parametros.NomeDispositivo2 != null ? _parametros.NomeDispositivo2 : "";
                }
                else
                {
                    cmbGeralDispositivoUser.SelectedValue = _parametros.IdDispositivo;
                    cmbGeralDispositivo_2User.SelectedValue = _parametros.IdDispositivo2;
                    txt_nome_DispositivoUser.Text = _parametros.NomeDispositivo != null ? _parametros.NomeDispositivo : "";
                    txt_nome_Dispositivo_2User.Text = _parametros.NomeDispositivo2 != null ? _parametros.NomeDispositivo2 : "";
                }

                lblTipoDosagemExec.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTipoDosagemExec;
                lblTipoDosagemExec_2.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTipoDosagemExec;

                if (this.User.Tipo == 1)
                {
                    cmbTipoDosagemExec.SelectedValue = _parametros.TipoDosagemExec;
                }
                else
                {
                    cmbTipoDosagemExec_2.SelectedValue = _parametros.TipoDosagemExec;
                }

                #endregion

                #region Purga

                this.lblPurgaVolume.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaVolume;
                this.chkPurgaControleExecucao.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkPurgaControleExecucao;
                this.lblPurgaHoras.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaHoras;
                this.lblPurgaParâmetrosDispensa.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaParametrosDispensa;
                this.lblPurgaVelocidade.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaVelocidade;
                this.lblPurgaAceleracao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaAceleracao;
                this.lblPurgaDelay.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaDelay;
                this.chkPurgaInterface.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkPurgaInterfaceProcesso;
                this.chkExigirExecucaoPurga.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkExigirExecucaoPurga;
                this.chkPurgaSequencial.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkPurgaSequencial;

                txtPurgaPrazoExecucao.Text = _parametros.PrazoExecucaoPurga.TotalHours.ToString();
                txtPurgaVolume.Text = _parametros.VolumePurga.ToString();
                txtPurgaVelocidade.Text = _parametros.VelocidadePurga.ToString();
                txtPurgaAceleracao.Text = _parametros.AceleracaoPurga.ToString();
                txtPurgaDelay.Text = _parametros.DelayPurga.ToString();
                chkPurgaControleExecucao.Checked = _parametros.ControlarExecucaoPurga;
                chkPurgaInterface.Checked = _parametros.DesabilitarInterfacePurga;

                chkExigirExecucaoPurga.Checked = _parametros.ExigirExecucaoPurga;

                chkPurgaSequencial.Checked = _parametros.PurgaSequencial;

                #endregion

                #region Calibracao Manual
                this.lblCalibragemColoranteM.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemColorante;
                this.lblCalibragemLegendaMotorM.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMotor;
                this.lblCalibragemLegendaMassaEspecM.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMassaEspec;
                this.btnCalibragemEditarM.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemEditar;
                this.btnPrimeiraCalibracaoM.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnPrimeiraCalibracao;
                this.lblMinimoFaixasCal.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoMinimoFaixas;
                this.btn_replicar_faixa.Text = Negocio.IdiomaResxExtensao.Configuracoes_ReplicarFaixasCal;
                this.btn_add_Valores.Text = Negocio.IdiomaResxExtensao.Configuracoes_AdicionarFaixasCal;
                this.lblCalibragemLegendaUltimoPulsoRevM.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaUltimoPulsoRev;

                btn_Abrir_Gaveta.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Abrir_Gaveta;
                btn_Fechar_Gaveta.Text = Negocio.IdiomaResxExtensao.Manutencao_BT_Fechar_Gaveta;
                #endregion

                #region Calibração Automatica
                this.lblVolumeMaxRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoVolumeMaxRecipiente;
                this.lblTentativasRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoTentativaPosicionamento;
                this.lblMinMassaAdmRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPesoRecipiente;

                this.lblCalibracaoAutoColorante.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemColorante;
                this.lblCalibracaoAutoLegendaMotor.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMotor;
                this.lblCalibracaoAutoLegendaMassaEspec.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMassaEspec;
                this.btnIniciarCalAutomatica.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoIniciar;
                this.btnCalibracaoAutoEditar.Text = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoHabilitarEditFaixa;
                #endregion

                #region DAT

                //Resources
                this.lblDatCaminhoMonitoramento.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDatCaminhoMonitoramento;
                this.lblDatCaminhoRepositorio.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDatCaminhoRepositorio;
                this.lblDatBaseCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDatBaseCircuito;
                this.rdbDatBase00.Text = Negocio.IdiomaResxExtensao.Configuracoes_rdbBaseDat00;
                this.rdbDatBase01.Text = Negocio.IdiomaResxExtensao.Configuracoes_rdbDatBase01;
                this.lblDatPadraoConteudo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDatPadraoConteudo;
               
                this.chkDatUtilizarCorrespondencia.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkUtilizarCorrespondencia;
                this.chkDatInterfaceDispSequencial.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDatInterfaceDispSequencial;
                this.chkDatInterfaceDispSimultanea.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDatInterfaceDispSimultanea;
                this.lblMonitFilaDat.Text = Negocio.IdiomaResxExtensao.Configurador_lblDisableMonitFilaDat;
                this.lblDelayMonitoramentoFilaDAT.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDelayMonitoramentoFilaDAT;

                lblMonitFilaDat.Text = Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_lblStatus;
                lblDelayMonitoramentoFilaDAT.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDelayMonitoramentoFilaDAT;
                chkDisableFilaDat.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDisableFilaDat;

                gbRdDat05.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_GBPadra05;
                gbRdDat06.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_GBPadra06;
                btn_par_dat_05.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTitulo;
                btn_par_dat_06.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTitulo;

                //Dados
                txtPathMonitoramentoDat.Text = _parametros.PathMonitoramentoDAT;
                txtPathRepositorioDat.Text = _parametros.PathRepositorioDAT;
                chkDatUtilizarCorrespondencia.Checked =
                    _parametros.UtilizarCorrespondenciaDAT;

                if (_parametros.BasePosicaoCircuitoDAT == 0)
                    rdbDatBase00.Checked = true;
                else
                    rdbDatBase01.Checked = true;

                this.chkDatInterfaceDispSequencial.Checked = _parametros.DesabilitarInterfaceDispensaSequencial;
                this.chkDatInterfaceDispSimultanea.Checked = _parametros.DesabilitarInterfaceDispensaSimultanea;

                this.chkDisableFilaDat.Checked = _parametros.DesabilitarMonitoramentoFilaDAT;
                this.txtPathMonitoramentoFilaDat.Text = _parametros.PathMonitoramentoFilaDAT;
                this.txtDelayMonitoramentoFilaDAT.Text = _parametros.DelayMonitoramentoFilaDAT.ToString();

                DatPattern padrao =
                    (DatPattern)_parametros.PadraoConteudoDAT;
                switch (padrao)
                {
                    case DatPattern.Padrao01:
                        cmbDatPadrao.SelectedIndex = 0;
                        break;
                    case DatPattern.Padrao02:
                        cmbDatPadrao.SelectedIndex = 1;
                        break;
                    case DatPattern.Padrao03:
                        cmbDatPadrao.SelectedIndex = 2;
                        break;
                    case DatPattern.Padrao04:
                        cmbDatPadrao.SelectedIndex = 3;
                        break;
                    case DatPattern.Padrao05:
                        cmbDatPadrao.SelectedIndex = 4;
                        break;
                    case DatPattern.Padrao06:
                        cmbDatPadrao.SelectedIndex = 5;
                        break;
                    case DatPattern.Padrao07:
                        cmbDatPadrao.SelectedIndex = 6;
                        break;
                    case DatPattern.PadraoUDCP:
                        cmbDatPadrao.SelectedIndex = 7;
                        break;

                    case DatPattern.PadraoSinteplast:
                        cmbDatPadrao.SelectedIndex = 8;
                        break;

                    case DatPattern.Padrao10:
                        cmbDatPadrao.SelectedIndex = 9;
                        break;
                    case DatPattern.Padrao11:
                        cmbDatPadrao.SelectedIndex = 10;
                        break;
                    case DatPattern.Padrao12:
                        cmbDatPadrao.SelectedIndex = 11;
                        break;
                }

                cmbDatPadrao_SelectedIndexChanged(null, null);
                this.chkDesabilitarVolumeMinimoDat.Checked = _parametros.DesabilitarVolumeMinimoDat;
                this.txtVolumeMinimoDat.Text = _parametros.VolumeMinimoDat.ToString();
                this.utxt_DelayUDCP.Text = _parametros.DelayUDCP.ToString();
                this.utxt_ExtFileTmpUDCP.Text = _parametros.ExtFileTmpUDCP;

                if (_parametros.CreateFileTmpUDCP == 1)
                {
                    this.chb_CreateFileTmpUDCP.Checked = true;
                }
                else
                {
                    this.chb_CreateFileTmpUDCP.Checked = false;
                }
                this.chb_ProcRemoveLataUDCP.Checked = _parametros.ProcRemoveLataUDCP;
                this.chkDisablePopUpDispDat.Checked = _parametros.DisablePopUpDispDat;

                this.chkDisablePopUpDispDat.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDisablePopUpDispDat;

                this.chkDesabilitarVolumeMinimoDat.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDesabilitarVolumeMinimoDat;
                this.lblVolumeMinimoDat.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblVolumeMinimoDat;

                #endregion

                #region Produtos
                this.lblColoranteCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.blColoranteNome.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
                this.blColoranteMassa.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteMassa;
                this.lblColoranteCorrespondencia.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCorrespondencia;
                this.lblColoranteBicoIndividual.Text = Negocio.IdiomaResxExtensao.ProdutoBicoIndividual;

                this.lblColoranteCircuito02.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.blColoranteNome02.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
                this.blColoranteMassa02.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteMassa;
                this.lblColoranteCorrespondencia02.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCorrespondencia;
                this.lblColoranteBicoIndividual1.Text = Negocio.IdiomaResxExtensao.ProdutoBicoIndividual;

                this.lblColoranteCircuito03.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.blColoranteNome03.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
                this.blColoranteMassa03.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteMassa;
                this.lblColoranteCorrespondencia03.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCorrespondencia;
                this.lblColoranteBicoIndividual2.Text = Negocio.IdiomaResxExtensao.ProdutoBicoIndividual;

                this.lblColoranteCircuito04.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.blColoranteNome04.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
                this.blColoranteMassa04.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteMassa;
                this.lblColoranteCorrespondencia04.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCorrespondencia;
                this.lblColoranteBicoIndividual3.Text = Negocio.IdiomaResxExtensao.ProdutoBicoIndividual;
                for (int i = 0; i <= 15; i++)
                {
                    //[Cores para circuitos de colorante]
                    _circuito[i].BackColor = Cores.Parar;
                    _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguir;
                    _circuito[i].Enabled = true;

                    if (_colorantes[i].Seguidor < 0)
                    {
                        _circuito[i].Checked = _colorantes[i].Habilitado;
                        chkCircuito_CheckedChanged(_circuito[i], e);
                    }
                    else
                    {
                        if (_colorantes[i].Habilitado)
                        {
                            _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguidor_Tom_01;
                            _circuito[i].BackColor = Cores.Seguidor_Tom_01;
                            this._toolTipProducts.SetToolTip(_circuito[i], "Seguidor do Circuito: " + _colorantes[i].Seguidor.ToString());
                            _circuito[i].Enabled = false;
                        }
                    }

                    _NomeCorante[i].Text = _colorantes[i].Nome;
                    _massaEspecifica[i].Text = _colorantes[i].MassaEspecifica.ToString();
                    _correspondencia[i].SelectedIndex = _colorantes[i].Correspondencia;
                    _base[i].Checked = _colorantes[i].IsBase;
                    _bicoIndividual[i].Checked = _colorantes[i].IsBicoIndividual;
                }

                try
                {
                    for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                    {
                        //[Cores para circuitos de colorante]
                        _circuito[i].BackColor = Cores.Parar;
                        _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguir;
                        _circuito[i].Enabled = true;

                        if (_colorantes[i].Seguidor < 0)
                        {
                            _circuito[i].Checked = _colorantes[i].Habilitado;
                            chkCircuito_CheckedChanged(_circuito[i], e);
                        }
                        else
                        {
                            _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguidor_Tom_01;
                            _circuito[i].BackColor = Cores.Seguidor_Tom_01;
                            this._toolTipProducts.SetToolTip(_circuito[i], "Seguidor do Circuito: " + _colorantes[i].Seguidor.ToString());
                            _circuito[i].Enabled = false;
                        }
                        _NomeCorante[i].Text = _colorantes[i].Nome;
                        _massaEspecifica[i].Text = _colorantes[i].MassaEspecifica.ToString();
                        int corresp = _colorantes[i].Correspondencia;
                       
                        _correspondencia[i].SelectedIndex = corresp - 16;
                        _base[i].Checked = _colorantes[i].IsBase;
                        _bicoIndividual[i].Checked = _colorantes[i].IsBicoIndividual;
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}

				#endregion

				#region  Nível de colorante

				this.chkControlarNivel.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkControlarNivel;

                this.label51.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.label77.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.label106.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.label133.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
                this.chkControlarNivel.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkControlarNivel;
                this.lblNivelMinimo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMinimo;
                this.lblNivelMaximo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMaximo;
                this.lblNivelMinimo2.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMinimo;
                this.lblNivelMaximo2.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMaximo;
                this.lblNivelMinimo3.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMinimo;
                this.lblNivelMaximo3.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMaximo;
                this.lblNivelMinimo4.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMinimo;
                this.lblNivelMaximo4.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMaximo;

                this.chkControlarNivel.Checked = _parametros.ControlarNivel;

                for (int i = 0; i < 16; i++)
                {
                    if (!_colorantes[i].Habilitado)
                    {
                        _nivelCKT[i].BackColor = Cores.Parar;
                    }
                    else
                    {
                        if (_colorantes[i].Seguidor < 0)
                        {
                            _nivelCKT[i].BackColor = Cores.Seguir;
                        }
                        else if (_colorantes[i].Seguidor > 0)
                        {
                            _nivelCKT[i].BackColor = Cores.Seguidor_Tom_01;
                        }
                    }

                    _nivelMinimo[i].Enabled = _colorantes[i].Habilitado;
                    _nivelMaximo[i].Enabled = _colorantes[i].Habilitado;
                    _purgaVolume[i].Enabled = _colorantes[i].Habilitado;

                    if (_colorantes[i].Habilitado)
                    {
                        _nivelMinimo[i].Text = _colorantes[i].NivelMinimo.ToString();
                        _nivelMaximo[i].Text = _colorantes[i].NivelMaximo.ToString();
                        _purgaVolume[i].Text = _colorantes[i].VolumePurga.ToString();
                    }
                    else
                    {
                        _nivelMinimo[i].Text = "0";
                        _nivelMaximo[i].Text = "0";
                        _purgaVolume[i].Text = "0";
                    }
                }

                for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                {
                    if (!_colorantes[i].Habilitado)
                    {
                        _nivelCKT[i].BackColor = Cores.Parar;
                    }
                    else
                    {
                        if (_colorantes[i].Seguidor < 0)
                        {
                            _nivelCKT[i].BackColor = Cores.Seguir;
                        }
                        else if (_colorantes[i].Seguidor > 0)
                        {
                            _nivelCKT[i].BackColor = Cores.Seguidor_Tom_01;
                        }
                    }

                    _nivelMinimo[i].Enabled = _colorantes[i].Habilitado;
                    _nivelMaximo[i].Enabled = _colorantes[i].Habilitado;
                    _purgaVolume[i].Enabled = _colorantes[i].Habilitado;
                    if (_colorantes[i].Habilitado)
                    {
                        _nivelMinimo[i].Text = _colorantes[i].NivelMinimo.ToString();
                        _nivelMaximo[i].Text = _colorantes[i].NivelMaximo.ToString();
                        _purgaVolume[i].Text = _colorantes[i].VolumePurga.ToString();
                    }
                    else
                    {
                        _nivelMinimo[i].Text = "0";
                        _nivelMaximo[i].Text = "0";
                        _purgaVolume[i].Text = "0";
                    }
                }

                #endregion

                #region Inicialização de circuitos

                this.lblInicializacaoPulsoInicial.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoPulsoInicial;
                this.lblInicializacaoPulsoLimite.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoPulsoLimite;
                this.lblInicializacaoVariacaoPulso.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoVariacaoPulso;
                this.lblInicializacaoStepVariacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoStepVariacao;
                this.lblInicializacaoVelocidade.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoVelocidade;
                this.lblInicializacaoAceleracao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoAceleracao;
                this.chkInicializacaoMovimentoReverso.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkInicializacaoMovimentoReverso;
                this.chkInicializacaoExecutarAntesPurga.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkInicializacaoExecutarAntesPurga;
                this.chkInicializacaoExecutarAntesPurgaIndividual.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkInicializacaoExecutarAntesPurgaIndividual;
                this.lblInicializacaoCircuitoPorGrupo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitCircuitoPorGrupo;
                this.lblInicializacaoCircuitoPorGrupoDesc.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoCircuitoPorGrupoDesc;

                this.btnInicializacaoExecutar.Text = Negocio.IdiomaResxExtensao.InicializacaoCircuitos_btnInicializacaoExecutar;
                this.chkIncializacaoInterface.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkIncializacaoInterface;

                txtInicializacaoPulsoInicial.Text = _parametros.IniPulsoInicial.ToString();
                txtInicializacaoPulsoLimite.Text = _parametros.IniPulsoLimite.ToString();
                txtInicializacaoVariacaoPulso.Text = _parametros.IniVariacaoPulso.ToString();
                txtInicializacaoStepVariacao.Text = _parametros.IniStepVariacao.ToString();
                txtInicializacaoAceleracao.Text = _parametros.IniAceleracao.ToString();
                txtInicializacaoVelocidade.Text = _parametros.IniVelocidade.ToString();
                chkInicializacaoMovimentoReverso.Checked = _parametros.IniMovimentoReverso;
                chkInicializacaoExecutarAntesPurga.Checked = _parametros.InicializarCircuitosPurga;
                cmbIniciaizacaoQtdeCircuitosGrupo.SelectedItem = _parametros.QtdeCircuitoGrupo.ToString();
                chkIncializacaoInterface.Checked = _parametros.DesabilitarInterfaceInicializacaoCircuito;
                chkInicializacaoExecutarAntesPurgaIndividual.Checked = _parametros.InicializarCircuitosPurgaIndividual;

                #endregion

                #region Monitoramento de circuitos
                lblMonitAceleracao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitAceleracao;
                lblMonitCircuitoPorGrupo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitCircuitoPorGrupo;
                lblMonitCircuitoPorGrupoDesc.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitCircuitoPorGrupoDesc;
                lblMonitDelay.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitDelay;
                lblMonitPulsos.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitPulsos;
                lblMonitPulsos.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitPulsos;
                lblMonitTempo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitTempo;
                lblMonitTempoInicial.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitTempoInicial;
                lblMonitVelocidade.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblMonitVelocidade;
                chkMonitMovimentoReverso.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitMovimentoReverso;
                chkMonitInterface.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitInterface;
                chkMonitProcesso.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitProcesso;

                txtAceleracaoMonitCircuitos.Text = _parametros.MonitAceleracao.ToString();
                txtVelociadadeMonitCircuitos.Text = _parametros.MonitVelocidade.ToString();
                txtTempoMonitCircuitos.Text = _parametros.MonitTimerDelay.ToString();
                txtTempoIniMonitCircuitos.Text = _parametros.MonitTimerDelayIni.ToString();
                txtDelayMonitCircuitos.Text = _parametros.MonitDelay.ToString();
                txtMonitPulsos.Text = _parametros.MonitPulsos.ToString();
                chkMonitMovimentoReverso.Checked = _parametros.MonitMovimentoReverso;
                chkMonitInterface.Checked = _parametros.DesabilitarInterfaceMonitCircuito;
                cmbMonitQtdeCircuitosGrupo.SelectedItem = _parametros.QtdeMonitCircuitoGrupo.ToString();
                chkMonitProcesso.Checked = _parametros.DesabilitarProcessoMonitCircuito;
                
                #endregion

                #region Unidade de medida

                this.lblUniMedidaUnidadesHabilitadas.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadesHabilitadas;
                this.chkUniMedidaMililitro.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Mililitro;
                this.chkUniMedidaOnca.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Onca;
                this.chkUniMedidaShot.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Shot;
                this.chkUniMedidaGramas.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Grama;
                this.lblUniMedidaUnidadeExibicao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeExibicao;
                this.rdbUniMedidaMililitro.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Mililitro;
                this.rdbUniMedidaGrama.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Grama;
                this.rdbUniMedidaOnca.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Onca;

                chkUniMedidaMililitro.Checked = _parametros.HabilitarMililitro;
                chkUniMedidaOnca.Checked = _parametros.HabilitarOnca;
                chkUniMedidaShot.Checked = _parametros.HabilitarShot;
                txtUniMedidaValorShot.Text = _parametros.ValorShot.ToString();
                chkUniMedidaGramas.Checked = _parametros.HabilitarGrama;

                txtUniMedidaValorFracao.Text = _parametros.ValorFraction.ToString();

                if (_parametros.UnidadeMedidaNivelColorante == (int)Percolore.IOConnect.Core.UnidadeMedida.Grama)
                {
                    rdbUniMedidaGrama.Checked = true;
                }
                else if (_parametros.UnidadeMedidaNivelColorante == (int)Percolore.IOConnect.Core.UnidadeMedida.Onca)
                {
                    rdbUniMedidaOnca.Checked = true;
                }
                else if (_parametros.UnidadeMedidaNivelColorante == (int)Percolore.IOConnect.Core.UnidadeMedida.Fraction)
                {
                    rdbUniMedidaFracao.Checked = true;
                }
                else
                {
                    rdbUniMedidaMililitro.Checked = true;
                }

                lblUniMedidaUnidadeAbast.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbast;
                lblUniMedidaUnidadeAbastCad.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbastCad;
                lblUniMedidaUnidadeAbastNome.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbastCont;
                lblUniMedidaUnidadeAbastUnidade.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbastUnidade;
                btnClearAbastecimento.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnClearAbastecimento;
                btnExcluirAbastecimento.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirAbastecimento;
                btnNovoAbastecimento.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnNovoAbastecimento;
                btnAtualizarAbastecimento.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnAtualizarAbastecimento;

                #endregion

                #region Log

                this.lblLogProcessoDispensa.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblLogProcessoDispensa;
                this.lblLogQuantidadeDispensada.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblLogQuantidadeDispensa;
                this.chkLogComunicacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkLogComunicacao;
                this.chkLogAutomateTesterProt.Text = Negocio.IdiomaResxExtensao.Configuracao_Calibracao_LogAutomateTesterProt;
                this.chkLogAutomateBackup.Text = Negocio.IdiomaResxExtensao.LogAutomateBackup;

                txtPathLogProcDispensa.Text = _parametros.PathLogProcessoDispensa;
                txtPathLogQtdeDispensa.Text = _parametros.PathLogControleDispensa;
                chkLogComunicacao.Checked = _parametros.HabilitarLogComunicacao;
                txtPathLogComunicacao.Text = _parametros.PathLogComunicacao;
                chkLogAutomateTesterProt.Checked = _parametros.HabilitarLogAutomateTesterProt;
                chkLogAutomateBackup.Checked = _parametros.LogAutomateBackup;

                chkLogSerialMenu.Checked = Modbus.Constantes.bSerialLog;
                this.chkLogSerialMenu.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkLogSerialMenu;

                chkLogStatusMaquina.Checked = _parametros.LogStatusMaquina;

                #endregion

                #region Admin

                this.lblAdminSenha.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblAdminSenha;
                this.lblAdminNumeroSerial.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblAdminNumeroSerial;

                using (IOConnectRegistry icntRegistry = new IOConnectRegistry())
                {
                    txtSenhaAdministrador.Text = icntRegistry.GetSenhaAdmnistrador();
                }

                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    txtNumeroSerial.Text = percRegistry.GetSerialNumber();
                }
                this.lblUsuarios.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblUsuarios;
                this.lblCadastroUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCadastroUsuario;
                this.lblNomeUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNomeUsuario;
                this.lblSenhaUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblSenhaUsuario;
                this.lblTipoUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTipoUsuario;
                this.btnAtualizarUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnAtualizarUsuario;
                this.btnNovoUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnNovoUsuario;
                this.btnExcluirUsuario.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirUsuario;
                this.btnClearUsuarios.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnClearUsuarios;

                #endregion

                #region Producao

                gbProducao.Text = Negocio.IdiomaResxExtensao.Configuracoes_gbProducao;
                lblProducaoIp.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoIp;
                lblProducaoPorta.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoPorta;
                lblProducaoTipo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoTipo;
                chkMonitProducao.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitProducao;

                txtIpProducao.Text = _parametros.IpProducao.ToString();
                txtPortProducao.Text = _parametros.PortaProducao.ToString();
                ckTipoProducao.Text = _parametros.TipoProducao.ToString();
                chkMonitProducao.Checked = _parametros.DesabilitaMonitProcessoProducao;

                #endregion

                #region Sinc Formula

                gbFormula.Text = Negocio.IdiomaResxExtensao.Configuracoes_gbFormula;
                lblFormulaIp.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblFormulaIp;
                lblFormulaPorta.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblFormulaPorta;
                chkMonitSincFormula.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitSincFormula;

                chkMonitSincFormula.Checked = _parametros.DesabilitaMonitSincFormula;
                txtIpSincFormula.Text = _parametros.IpSincFormula.ToString();
                txtPortSincFormula.Text = _parametros.PortaSincFormula.ToString();
                #endregion

                #region Sinc Token

                chkMonitToken.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitToken;
                gbToken.Text = Negocio.IdiomaResxExtensao.Configuracoes_gbToken;
                lblTokenIp.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTokenIp;
                lblTokenPorta.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTokenPorta;

                chkMonitToken.Checked = _parametros.DesabilitaMonitSyncToken;
                txtIpToken.Text = _parametros.IpSincToken.ToString();
                txtPortToken.Text = _parametros.PortaSincToken.ToString();
                ckTipoEventos.Text = _parametros.TipoEventos;

                lblEventosTipo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoTipo;
                #endregion

                txtTimeoutPingTcp.Text = _parametros.TimeoutPingTcp.ToString();

                #region Sinc Bkp Calibragem
                chkMonitBkpCalibragem.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkMonitBkpCalibragem;
                gbBkpCalibragem.Text = Negocio.IdiomaResxExtensao.Configuracoes_gbBkpCalibragem;

                txtUrlSincBkpCalibragem.Text = _parametros.UrlSincBkpCalibragem.ToString();
                chkMonitBkpCalibragem.Checked = _parametros.DesabilitaMonitSyncBkpCalibragem;
                #endregion

                #region Base Dados

                gbBaseDados.Text = Negocio.IdiomaResxExtensao.Configuracoes_gbBaseDados;
                lblTipoBaseDados.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTipoBaseDados;
                lblPathBaseDados.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblPathBaseDados;

                txtPathBaseDados.Text = _parametros.PathBasesDados;
                cmbTipoBaseDados.SelectedValue = _parametros.TipoBaseDados;
                #endregion

                this._listRecircular = Util.ObjectRecircular.List();

                #region Recircular Produtos
                chkHabilitarMonitRecirculacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkHabilitarMonitRecirculacao;
                lblDelayMonitRecirculacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDelayMonitRecirculacao;
               
                #endregion

                #region Limp Bicos
                chkGeralHabLimpBicos.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkGeralHabLimpBicos;
                lblGeralDelayLimpBicos.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDelayLimpBicos;
                lblTipoLimpBicos.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTipoLimpBicos;

                lblLimpBicosPeriodosConfig.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblLimpBicosPeriodosConfig;
                lblLimpBicosCadConfig.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblLimpBicosCadConfig;
                lblLimpBicosPeriodo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblLimpBicosPeriodo;
                btnClearLimpBicosConfig.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnClearLimpBicosConfig;
                btnAtualizarLimpBicosConfig.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnAtualizarLimpBicosConfig;
                btnNovoLimpBicosConfig.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnNovoLimpBicosConfig;
                btnExcluirLimpBicosConfig.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirLimpBicosConfig;

                txtGeralDelayLimpBicos.Text = _parametros.DelayLimpBicos.ToString();
                chkGeralHabLimpBicos.Checked = _parametros.HabLimpBicos;

                #endregion

                #region Placa Mov
                this.tabPlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabPlacaMov;
                this.lblAddress_PlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Addres;
                this.lblNomePlMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_NomeDispositivo;
                this.lblTempoAlertaPlMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Tempo_Alerta;
                this.lblMotorPlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Motor;
                this.lblTipoMotorPlMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Tipo_Motor;
                this.lblVelocidade_PlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Velocidade;
                this.lblAceleracao_PlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Aceleracao;
                this.lblPulsos_PlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Pulsos;
                this.lblDelay_PlacaMov.Text = Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Delay;

                #endregion

                #region Recirculação Auto
               
                this.chkHabilitarMonitRecirculacaoAuto.Text = Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_Habilitar_Monit;
                this.lblTempoMonitRecirculacaoAuto.Text = Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Tempo_Monit;
                this.lblTempoNotifRecirculacaoAuto.Text = Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Tempo_Notif;
                this.lblTentativasRecirculacaoAuto.Text = Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Tentativas;
                this.lblTipoTempoMonitRecirculacaoAuto.Text = Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_TempoUn;

                #endregion
                
                RessetarRecirculacao();

                try
                {
                    lblDataInstalacao.Text = Negocio.IdiomaResxExtensao.Configuracoes_Data_Instalacao;

                    using (IOConnectRegistry icntRegistry = new IOConnectRegistry())
                    {
                        long timestamp = icntRegistry.GetDatainstalacao();
                        dataInstalacao = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime.ToShortDateString();
                        txtDataInstalacao.Text = dataInstalacao;
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				    txtDataInstalacao.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
                }
                
                dgvCalibracaoAuto.Columns[0].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoOperacao;
                dgvCalibracaoAuto.Columns[1].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoMotor;
                dgvCalibracaoAuto.Columns[2].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoVolume;
                dgvCalibracaoAuto.Columns[3].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoDesvioAdm;
                dgvCalibracaoAuto.Columns[4].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoIsCalibracao;
                dgvCalibracaoAuto.Columns[5].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoNumeroTentativa;
                dgvCalibracaoAuto.Columns[6].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoMediaMedicao;
                dgvCalibracaoAuto.Columns[7].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoNumeroMedia;

                DataTable dtUser = new DataTable();
                dtUser.Columns.Add("Valor");
                dtUser.Columns.Add("Descricao");
                for (int i = 1; i <= 3; i++)
                {

                    DataRow dr = dtUser.NewRow();
                    dr["Valor"] = i.ToString();
                    if (i == 1)
                    {
                        dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracao_UsuarioMaster;
                        if (this.User.Tipo == 1)
                        {
                            dtUser.Rows.Add(dr);
                        }
                    }
                    else if (i == 3)
                    {
                        dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracao_UsuarioGerente;
                        dtUser.Rows.Add(dr);
                    }
                    else if (i == 2)
                    {
                        dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracao_UsuarioTecnico;
                        dtUser.Rows.Add(dr);
                    }
                }
                cbTipoUsuario.DisplayMember = "Descricao";
                cbTipoUsuario.ValueMember = "Valor";
                cbTipoUsuario.DataSource = dtUser.DefaultView;

                cbTipoUsuario.SelectedIndex = 0;

                dgUsuario.AutoGenerateColumns = false;
                dgUsuario.ColumnCount = 1;

                dgUsuario.Columns[0].DataPropertyName = "Nome";
                dgUsuario.Columns[0].Visible = true;
                dgUsuario.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                AtualizaGridUsuario();
                
                txt_Address_PlacaMov.Text = _parametros.Address_PlacaMov.ToString();
                txt_nome_DispositivoPlMov.Text = _parametros.NomeDispositivo_PlacaMov;
                txtDelayAlertaPlacaMov.Text = _parametros.DelayAlertaPlacaMov.ToString();

                this._PlacaMov = new ObjectPlacaMovimentacao();
                this._PlacaMov.AddresModbus = _parametros.Address_PlacaMov.ToString();
                this._PlacaMov.PortaSerial = _parametros.NomeDispositivo_PlacaMov;
                this._PlacaMov._pMotor = Util.ObjectMotorPlacaMovimentacao.List();

                DataTable dtAbast = new DataTable();
                dtAbast.Columns.Add("UnMed");
                dtAbast.Columns.Add("descricao");

                DataRow drAbst = dtAbast.NewRow();
                drAbst["UnMed"] = (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro + "";
                drAbst["descricao"] = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Mililitro;
                dtAbast.Rows.Add(drAbst);

                drAbst = dtAbast.NewRow();
                drAbst["UnMed"] = (int)Percolore.IOConnect.Core.UnidadeMedida.Grama + "";
                drAbst["descricao"] = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Grama;
                dtAbast.Rows.Add(drAbst);

                cbUnidadeAbastecimento.DisplayMember = "descricao";
                cbUnidadeAbastecimento.ValueMember = "UnMed";
                cbUnidadeAbastecimento.DataSource = dtAbast.DefaultView;                   
                cbUnidadeAbastecimento.SelectedIndex = 0;

                dgAbastecimento.AutoGenerateColumns = false;
                dgAbastecimento.ColumnCount = 1;

                dgAbastecimento.Columns[0].DataPropertyName = "Nome";
                dgAbastecimento.Columns[0].Visible = true;
                dgAbastecimento.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                AtualizaGridAbastecimento();

                DataTable dtLimpBicos = new DataTable();
                dtLimpBicos.Columns.Add("Valor");
                dtLimpBicos.Columns.Add("Descricao");
                for (int i = 1; i < 3; i++)
                {
                    DataRow dr = dtLimpBicos.NewRow();
                    dr["Valor"] = i.ToString();
                    if (i == 1)
                    {
                        dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_cbTipoLimpBicosIntervalo;
                    }
                    else if (i == 2)
                    {
                        dr["Descricao"] = Negocio.IdiomaResxExtensao.Configuracoes_cbTipoLimpBicosConfig; 
                    }
                    else
                    {
                        break;
                    }

                    dtLimpBicos.Rows.Add(dr);
                }
                cbTipoLimpBicos.DisplayMember = "Descricao";
                cbTipoLimpBicos.ValueMember = "Valor";
                cbTipoLimpBicos.DataSource = dtLimpBicos.DefaultView;
                for(int i = 0; i < dtLimpBicos.Rows.Count; i++)
                {
                    if(dtLimpBicos.Rows[i]["Valor"].ToString() == _parametros.TipoLimpBicos.ToString())
                    {
                        cbTipoLimpBicos.SelectedIndex = i;
                        break;
                    }
                }
                if(_parametros.TipoLimpBicos== 1)
                {
                    gbLimpBicosConfiguravel.Visible = false;
                    gbLimpBicosConfiguravel.Enabled = false;
                    lblGeralDelayLimpBicos.Enabled = true;
                    txtGeralDelayLimpBicos.Enabled = true;
                    lblGeralDelayLimpBicos.Visible = true;
                    txtGeralDelayLimpBicos.Visible = true;
                }
                else
                {
                    gbLimpBicosConfiguravel.Visible = true;
                    gbLimpBicosConfiguravel.Enabled = true;
                    lblGeralDelayLimpBicos.Enabled = false;
                    txtGeralDelayLimpBicos.Enabled = false;
                    lblGeralDelayLimpBicos.Visible = false;
                    txtGeralDelayLimpBicos.Visible = false;
                }

                dgvLimpBicos.AutoGenerateColumns = false;
                dgvLimpBicos.ColumnCount = 1;

                dgvLimpBicos.Columns[0].DataPropertyName = "Horario";
                dgvLimpBicos.Columns[0].Visible = true;
                dgvLimpBicos.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                AtualizaGridLimpBicos();
                
                RessetarPlacaMov();
                ResetarCalibragem();
                updateTeclado();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados + ex.Message);
                }
            }

            try
            {
                //Master
                if(this.User.Tipo == 1)
                {
                    tab.Controls.Remove(tabParametrosGeraisUser);
                    tab.Controls.Remove(tabPlacaMov);
                }
                //Tecnico
                else if(this.User.Tipo == 2)
                {
                    tab.Controls.Remove(tabParametrosGerais);
                    tab.Controls.Remove(tabDAT);
                    tab.Controls.Remove(tabParametrosPurga);
                    tab.Controls.Remove(tabProdutos);
                    tab.Controls.Remove(tabInicializarCircuito);
                    tab.Controls.Remove(tabMonitCircuitos);
                    tab.Controls.Remove(tabUnidadeMedida);
                    tab.Controls.Remove(tabLog);
                    tab.Controls.Remove(tabProducao);
                    tab.Controls.Remove(tabRecirculacaoProd);
                    tab.Controls.Remove(tabPlacaMov);
                    
                    tab.Controls.Remove(tabLimpBicos);
                    tab.Controls.Remove(tabAdmin);
                }
                //Manager
                else if (this.User.Tipo == 3)
                {
                    tab.Controls.Remove(tabParametrosGerais);
                    #region Dat
                    tableLayoutPanel3.Visible = false;
                    tableLayoutPanel3.Enabled = false;
                    tableLayoutPanel4.Visible = false;
                    tableLayoutPanel4.Enabled = false;
                    chkDatUtilizarCorrespondencia.Visible = false;
                    chkDatInterfaceDispSimultanea.Visible = false;
                    chkDatInterfaceDispSequencial.Visible = false;
                    chkDisableFilaDat.Visible = false;
                    lblMonitFilaDat.Visible = false;
                    txtPathMonitoramentoFilaDat.Visible = false;
                    btnPathMonitoramentoFilaDat.Visible = false;
                    lblDelayMonitoramentoFilaDAT.Visible = false;
                    txtDelayMonitoramentoFilaDAT.Visible = false;
                    gbDatVmin.Visible = false;
                    gbRdDat05.Visible = false;
                    gbRdDat06.Visible = false;

                    chkDatUtilizarCorrespondencia.Enabled = false;
                    chkDatInterfaceDispSimultanea.Enabled = false;
                    chkDatInterfaceDispSequencial.Enabled = false;
                    chkDisableFilaDat.Enabled = false;
                    lblMonitFilaDat.Enabled = false;
                    txtPathMonitoramentoFilaDat.Enabled = false;
                    btnPathMonitoramentoFilaDat.Enabled = false;
                    lblDelayMonitoramentoFilaDAT.Enabled = false;
                    txtDelayMonitoramentoFilaDAT.Enabled = false;
                    gbDatVmin.Enabled = false;
                    gbRdDat05.Enabled = false;
                    gbRdDat06.Enabled = false;
                    #endregion

                    #region Purga
                    lblPurgaParâmetrosDispensa.Visible = false;
                    lblPurgaVelocidade.Visible = false;
                    lblPurgaAceleracao.Visible = false;
                    lblPurgaDelay.Visible = false;
                    txtPurgaVelocidade.Visible = false;
                    txtPurgaAceleracao.Visible = false;
                    txtPurgaDelay.Visible = false;
                    chkPurgaInterface.Visible = false;
                    chkExigirExecucaoPurga.Visible = false;
                    chkPurgaSequencial.Visible = false;

                    lblPurgaParâmetrosDispensa.Enabled = false;
                    lblPurgaVelocidade.Enabled = false;
                    lblPurgaAceleracao.Enabled = false;
                    lblPurgaDelay.Enabled = false;
                    txtPurgaVelocidade.Enabled = false;
                    txtPurgaAceleracao.Enabled = false;
                    txtPurgaDelay.Enabled = false;
                    chkPurgaInterface.Enabled = false;
                    chkExigirExecucaoPurga.Enabled = false;
                    chkPurgaSequencial.Enabled = false;
                    chkHabilitarPurgaIndividual.Location = new System.Drawing.Point(20, 185);

                    #endregion

                    tab.Controls.Remove(tabInicializarCircuito);
                    tab.Controls.Remove(tabMonitCircuitos);

                    tab.Controls.Remove(tabLog);
                    tab.Controls.Remove(tabProducao);
                    tab.Controls.Remove(tabRecirculacaoProd);
                    tab.Controls.Remove(tabPlacaMov);
                    
                    #region Admin
                    txtDataInstalacao.Visible = false;
                    lblDataInstalacao.Visible = false;
                    txtDataInstalacao.Enabled = false;
                    lblDataInstalacao.Enabled = false;
                    #endregion
                }
            
                if(_parametros.PathMonitoramentoDAT.Length>0)
                {
                    string strDirectNew = string.Empty;
                    string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                    for(int i =0; i < arrayStr.Length-1; i++)
                    {
                        strDirectNew += arrayStr[i] + Path.DirectorySeparatorChar;
                    }
                    if (!Directory.Exists(strDirectNew))
                    {
                        Directory.CreateDirectory(strDirectNew);
                    }
                }
            
                if(_parametros.PadraoConteudoDAT == 12)
                { 
                    tabCalibragemManual.Enabled = false;
                    tabCalibragemAuto.Enabled = false;
                }
                else
                {
                    tabCalibragemManual.Enabled = true;
                    tabCalibragemAuto.Enabled = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
              
        private void updateTeclado()
        {
            bool chb_tec = _parametros.HabilitarTecladoVirtual;
            bool chb_touch = _parametros.HabilitarTouchScrenn;

            txtTimeout.isTecladoShow = chb_tec;            
            txtVelocidadeGlobal.isTecladoShow = chb_tec;
            txtAceleracaoGlobal.isTecladoShow = chb_tec;
            txtReversoDelay.isTecladoShow = chb_tec;
            txtReversoPulsos.isTecladoShow = chb_tec;
            txtPathLogProcDispensa.isTecladoShow = chb_tec;
            txtPathLogQtdeDispensa.isTecladoShow = chb_tec;
            txtPathLogComunicacao.isTecladoShow = chb_tec;
            txt_QtdTentativasConexao.isTecladoShow = chb_tec;

            txtPurgaPrazoExecucao.isTecladoShow = chb_tec;
            txtPurgaVolume.isTecladoShow = chb_tec;
            txtPurgaVelocidade.isTecladoShow = chb_tec;
            txtPurgaAceleracao.isTecladoShow = chb_tec;
            txtPurgaDelay.isTecladoShow = chb_tec;

            txtPathMonitoramentoDat.isTecladoShow = chb_tec;
            txtPathRepositorioDat.isTecladoShow = chb_tec;

            txtGeralDelayLimpBicos.isTecladoShow = chb_tec;

            txtPathMonitoramentoFilaDat.isTecladoShow = chb_tec;
            txtDelayMonitoramentoFilaDAT.isTecladoShow = chb_tec;

            txtVolumeMinimoDat.isTecladoShow = chb_tec;

            txtInicializacaoPulsoInicial.isTecladoShow = chb_tec;
            txtInicializacaoPulsoLimite.isTecladoShow = chb_tec;
            txtInicializacaoVariacaoPulso.isTecladoShow = chb_tec;
            txtInicializacaoStepVariacao.isTecladoShow = chb_tec;
            txtInicializacaoAceleracao.isTecladoShow = chb_tec;
            txtInicializacaoVelocidade.isTecladoShow = chb_tec;

            txtAceleracaoMonitCircuitos.isTecladoShow = chb_tec;
            txtVelociadadeMonitCircuitos.isTecladoShow = chb_tec;
            txtTempoMonitCircuitos.isTecladoShow = chb_tec;
            txtTempoIniMonitCircuitos.isTecladoShow = chb_tec;
            txtDelayMonitCircuitos.isTecladoShow = chb_tec;
            txtMonitPulsos.isTecladoShow = chb_tec;

            txtPathLogProcDispensa.isTecladoShow = chb_tec;
            txtPathLogQtdeDispensa.isTecladoShow = chb_tec;
            txtPathLogComunicacao.isTecladoShow = chb_tec;

            txtSenhaAdministrador.isTecladoShow = chb_tec;
            txtNumeroSerial.isTecladoShow = chb_tec;            
            txtPortProducao.isTecladoShow = chb_tec;

            txtPortSincFormula.isTecladoShow = chb_tec;

            txtUniMedidaValorShot.isTecladoShow = chb_tec;
            txtUniMedidaValorFracao.isTecladoShow = chb_tec;

            txt_nome_Dispositivo.isTecladoShow = chb_tec;
            txt_nome_Dispositivo_2.isTecladoShow = chb_tec;

            txt_nome_DispositivoUser.isTecladoShow = chb_tec;
            txt_nome_Dispositivo_2User.isTecladoShow = chb_tec;

            txtDataInstalacao.isTecladoShow = chb_tec;

            txtCapacidadeMaxBalanca.isTecladoShow = chb_tec;
            txtMassaAdmRecipiente.isTecladoShow = chb_tec;
            txtMinMassaAdmRecipiente.isTecladoShow = chb_tec;
            txtVolumeMaxRecipiente.isTecladoShow = chb_tec;
            txtTentativasRecipiente.isTecladoShow = chb_tec;


            txtDelayMonitRecirculacao.isTecladoShow = chb_tec;

            txt_Address_PlacaMov.isTecladoShow = chb_tec;
            txt_nome_DispositivoPlMov.isTecladoShow = chb_tec;
            txtDelayAlertaPlacaMov.isTecladoShow = chb_tec;
            
            txt_Aceleracao_PlacaMov.isTecladoShow = chb_tec;
            txt_Velocidade_PlacaMov.isTecladoShow = chb_tec;
            txt_Delay_PlacaMov.isTecladoShow = chb_tec;
            txt_Pulsos_PlacaMov.isTecladoShow = chb_tec;

            txtDelayMonitRecirculacaoAuto.isTecladoShow = chb_tec;
            txtDelayNotificacaotRecirculacaoAuto.isTecladoShow = chb_tec;
            txtQtdNotificacaotRecirculacaoAuto.isTecladoShow = chb_tec;

            txtConteudoAbastecimento.isTecladoShow = chb_tec;
            txtNomeAbastecimento.isTecladoShow = chb_tec;

            txtTimeoutPingTcp.isTecladoShow = chb_tec;

            txtTimeout.isTouchScrenn = chb_touch;
            txtVelocidadeGlobal.isTouchScrenn = chb_touch;
            txtAceleracaoGlobal.isTouchScrenn = chb_touch;
            txtReversoDelay.isTouchScrenn = chb_touch;
            txtReversoPulsos.isTouchScrenn = chb_touch;
            txtPathLogProcDispensa.isTouchScrenn = chb_touch;
            txtPathLogQtdeDispensa.isTouchScrenn = chb_touch;
            txtPathLogComunicacao.isTouchScrenn = chb_touch;

            txt_QtdTentativasConexao.isTouchScrenn = chb_touch;

            txtPurgaPrazoExecucao.isTouchScrenn = chb_touch;
            txtPurgaVolume.isTouchScrenn = chb_touch;
            txtPurgaVelocidade.isTouchScrenn = chb_touch;
            txtPurgaAceleracao.isTouchScrenn = chb_touch;
            txtPurgaDelay.isTouchScrenn = chb_touch;

            txtPathMonitoramentoDat.isTouchScrenn = chb_touch;
            txtPathRepositorioDat.isTouchScrenn = chb_touch;

            txtPathMonitoramentoFilaDat.isTouchScrenn = chb_touch;
            txtDelayMonitoramentoFilaDAT.isTouchScrenn = chb_touch;

            txtVolumeMinimoDat.isTouchScrenn = chb_touch;

            txtInicializacaoPulsoInicial.isTouchScrenn = chb_touch;
            txtInicializacaoPulsoLimite.isTouchScrenn = chb_touch;
            txtInicializacaoVariacaoPulso.isTouchScrenn = chb_touch;
            txtInicializacaoStepVariacao.isTouchScrenn = chb_touch;
            txtInicializacaoAceleracao.isTouchScrenn = chb_touch;
            txtInicializacaoVelocidade.isTouchScrenn = chb_touch;

            txtAceleracaoMonitCircuitos.isTouchScrenn = chb_touch;
            txtVelociadadeMonitCircuitos.isTouchScrenn = chb_touch;
            txtTempoMonitCircuitos.isTouchScrenn = chb_touch;
            txtTempoIniMonitCircuitos.isTouchScrenn = chb_touch;
            txtDelayMonitCircuitos.isTouchScrenn = chb_touch;
            txtMonitPulsos.isTouchScrenn = chb_touch;

            txtPathLogProcDispensa.isTouchScrenn = chb_touch;
            txtPathLogQtdeDispensa.isTouchScrenn = chb_touch;
            txtPathLogComunicacao.isTouchScrenn = chb_touch;

            txtSenhaAdministrador.isTouchScrenn = chb_touch;
            txtNumeroSerial.isTouchScrenn = chb_touch;
            txtPortProducao.isTouchScrenn = chb_touch;
            txtPortSincFormula.isTouchScrenn = chb_touch;

            txtUniMedidaValorShot.isTouchScrenn = chb_touch;
            txtUniMedidaValorFracao.isTouchScrenn = chb_touch;

            for (int i = 0; i < _NomeCorante.Length; i++)
            {
                _NomeCorante[i].isTouchScrenn = chb_touch;
                _NomeCorante[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _massaEspecifica.Length; i++)
            {
                _massaEspecifica[i].isTouchScrenn = chb_touch;
                _massaEspecifica[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _nivelMinimo.Length; i++)
            {
                _nivelMinimo[i].isTouchScrenn = chb_touch;
                _nivelMinimo[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _nivelMaximo.Length; i++)
            {
                _nivelMaximo[i].isTouchScrenn = chb_touch;
                _nivelMaximo[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _purgaVolume.Length; i++)
            {
                _purgaVolume[i].isTouchScrenn = chb_touch;
                _purgaVolume[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _recircularVolDin.Length; i++)
            {
                _recircularVolDin[i].isTouchScrenn = chb_touch;
                _recircularVolDin[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _recircularDias.Length; i++)
            {
                _recircularDias[i].isTouchScrenn = chb_touch;
                _recircularDias[i].isTecladoShow = chb_tec;
            }

            for (int i = 0; i < _recircularVol.Length; i++)
            {
                _recircularVol[i].isTouchScrenn = chb_touch;
                _recircularVol[i].isTecladoShow = chb_tec;
            }

            txt_nome_Dispositivo.isTouchScrenn = chb_touch;
            txt_nome_Dispositivo_2.isTouchScrenn = chb_touch;

            txt_nome_DispositivoUser.isTouchScrenn = chb_touch;
            txt_nome_Dispositivo_2User.isTouchScrenn = chb_touch;

            txtDataInstalacao.isTouchScrenn = chb_touch;

            txtCapacidadeMaxBalanca.isTouchScrenn = chb_touch;
            txtMassaAdmRecipiente.isTouchScrenn = chb_touch;
            txtMinMassaAdmRecipiente.isTouchScrenn = chb_touch;
            txtVolumeMaxRecipiente.isTouchScrenn = chb_touch;
            txtTentativasRecipiente.isTouchScrenn = chb_touch;

            txtDelayMonitRecirculacao.isTouchScrenn = chb_touch;

            txt_Address_PlacaMov.isTouchScrenn = chb_touch;
            txt_nome_DispositivoPlMov.isTouchScrenn = chb_touch;
            txtDelayAlertaPlacaMov.isTouchScrenn = chb_touch;
            
            txt_Aceleracao_PlacaMov.isTouchScrenn = chb_touch;
            txt_Velocidade_PlacaMov.isTouchScrenn = chb_touch;
            txt_Delay_PlacaMov.isTouchScrenn = chb_touch;
            txt_Pulsos_PlacaMov.isTouchScrenn = chb_touch;

            txtDelayMonitRecirculacaoAuto.isTouchScrenn = chb_touch;
            txtDelayNotificacaotRecirculacaoAuto.isTouchScrenn = chb_touch;
            txtQtdNotificacaotRecirculacaoAuto.isTouchScrenn = chb_touch;

            txtGeralDelayLimpBicos.isTouchScrenn = chb_touch;

            txtConteudoAbastecimento.isTouchScrenn = chb_touch;
            txtNomeAbastecimento.isTouchScrenn = chb_touch;

            txtTimeoutPingTcp.isTouchScrenn = chb_touch;
        }
        
        private void cmbCorante_SelectionChangeCommitted(object sender, EventArgs e)
        {
           
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tab.SelectedTab;
            bool _closeAguarde = _parametros.ViewMessageProc;
            try
            {
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(Wait_Message));
                    if (_parametros.ViewMessageProc)
                    {
                        Application.DoEvents();
                        Thread.Sleep(200);
                        Application.DoEvents();
                    }
                }
            
                switch (tabPage.Name)
                {
                    case "tabParametrosGerais":
                    case "tabParametrosGeraisUser":
                    case "tabParametrosPurga":
                    case "tabDAT":
                    case "tabInicializarCircuito":
                    case "tabMonitCircuitos":
                    case "tabUnidadeMedida":
                    case "tabLog":
                    case "tabProducao":
                        {
                            if (!PersistirParametros())
                                return;

                            Util.ObjectParametros.InitLoad();
                            _parametros = Util.ObjectParametros.Load();

                            int i_P1 = 16;
                            int i_P2 = 0;

                            if ((Dispositivo)_parametros.IdDispositivo != Dispositivo.Placa_3)
                            {
                                Dispositivo dispP1 = (Dispositivo)_parametros.IdDispositivo;

                                Dispositivo2 dispP2 = (Dispositivo2)_parametros.IdDispositivo2;
                                
                                if (dispP2 == Dispositivo2.Placa_2 || dispP2 == Dispositivo2.Simulador)
                                {
                                    i_P2 = 16;
                                }
                            }
                            for(int i = 0; i < i_P1;i++)
                            {
                                this._colorantes[i].Dispositivo = 1;
                            }

                            for (int i = i_P1; i < this._colorantes.Count; i++)
                            {
                                this._colorantes[i].Dispositivo = 2;
                            }

                            int pTotal = i_P1 + i_P2;
                            for (int t = this._colorantes.Count; t > pTotal; t--)
                            {
                                this._colorantes[t - 1].Habilitado = false;
                                this._listRecircular[t - 1].Habilitado = false;

                            }

                            PersistirColorantesSemMensagem();
                            PersistirRecirculacaoSemMensagem();
                            if (_parametros.ViewMessageProc)
                            {
                                Application.DoEvents();
                                Thread.Sleep(200);
                                Application.DoEvents();
                            }
                            break;
                        }
                    case "tabCalibragem":
                        {
                            break;
                        }
                    case "tabCalibragemManual":
                        {
                            bool aux = true;
                            if (cb_CalibragemColorante.SelectedIndex == -1)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    m.ShowDialog(
                                        Negocio.IdiomaResxExtensao.Global_Informacao_NenhumColorante);
                                }

                                cb_CalibragemColorante.Focus();
                                aux = false;
                            }
                            if (aux)
                            {
                                PersistirCalibragem();
                                Util.ObjectParametros.InitLoad();
                            }

                            break;
                        }
                    case "tabCalibragemAuto":
                        {
                            bool aux = true;
                            if (cb_CalibracaoAuto.SelectedIndex == -1)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    m.ShowDialog(
                                        Negocio.IdiomaResxExtensao.Global_Informacao_NenhumColorante);
                                }

                                cb_CalibracaoAuto.Focus();
                                aux = false;
                            }
                            if (aux)
                            {
                                PersistirCalibracaoAuto();
                                Util.ObjectParametros.InitLoad();
                            }

                            break;
                        }
                  
                    case "tabAdmin":
                        {
                            PersistirAdmin();
                            Util.ObjectParametros.InitLoad();

                            break;
                        }
                    case "tabRecirculacaoProd":
                        {
                            for(int i= 0; i < this._listRecircular.Count; i++)
                            {
                                this._listRecircular[i].isAuto = _recircularAuto[i].Checked;
                                this._listRecircular[i].isValve = _recircularValve[i].Checked;
                                this._listRecircular[i].VolumeDin = _recircularVolDin[i].ToDouble();
                                this._listRecircular[i].VolumeRecircular = _recircularVol[i].ToDouble();
                                this._listRecircular[i].Dias = _recircularDias[i].ToInt();

                            }
                            PersistirRecirculacao();
                            Util.ObjectParametros.InitLoad();
                            _parametros = Util.ObjectParametros.Load();
                            RessetarRecirculacao();
                            
                            break;
                        }
                    case "tabPlacaMov":
                        {
                            PersistirParametros();
                            Util.ObjectParametros.InitLoad();
                            _parametros = Util.ObjectParametros.Load();
                            
                            if (_parametros.IdDispositivo2 == 0)
                            {
                                for (int i = 16; this._colorantes.Count >= 31 && i <= 31; i++)
                                {
                                    this._colorantes[i].Habilitado = false;
                                    
                                }
                                PersistirColorantesSemMensagem();
                                _colorantes = Util.ObjectColorante.List();
                                PersistirRecirculacaoSemMensagem();

                                this._colorantesProd = _colorantes.ToList();
                                atualizaDataGridProdutos();
                            }

                            break;
                        }
                   
                    case "tabLimpBicos":
                        {
                            PersistirParametros();
                            Util.ObjectParametros.InitLoad();
                            break;
                        }
                    case "tabProdutos":
                        {
                            bool isAlteradoColorante = verificaColorantes();
                            PersistirColorantes();
                            _colorantes = Util.ObjectColorante.List();
                            PersistirRecirculacaoSemMensagem();
                            if (isAlteradoColorante)
                            {
                                string detalhesCol = "";
                                foreach (Util.ObjectColorante _col in _colorantes)
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
                            }
                            Util.ObjectParametros.InitLoad();

                            break;
                        }
                    case "tabNivelProdutos":
                        {
                            PersistirColorantes();
                            _colorantes = Util.ObjectColorante.List();
                            PersistirParametros();
                            Util.ObjectParametros.InitLoad();
                            _parametros = Util.ObjectParametros.Load();
                            PersistirRecirculacaoSemMensagem();
                           
                            break;
                        }

                    default:
                        break;
                }
            
                if (_parametros.ViewMessageProc || _closeAguarde)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            
                if(_parametros.PadraoConteudoDAT == 12)
                {
                    tabCalibragemManual.Enabled = false;
                    tabCalibragemAuto.Enabled = false;
                }
                else
                {
                    tabCalibragemManual.Enabled = true;
                    tabCalibragemAuto.Enabled = true;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        bool verificaColorantes()
        {
            bool retorno = false;
            try
            {
                for (int i = 0; !retorno && i <= 15; i++)
                {
                    if (_colorantes[i].Nome != _NomeCorante[i].Text || Math.Round(_colorantes[i].MassaEspecifica, 3) != Math.Round(_massaEspecifica[i].ToDouble(), 3) ||
                        _colorantes[i].Habilitado != _circuito[i].Checked)
                    {
                        retorno = true;
                    }
                }
                for (int i = 16; !retorno && _circuito.Length >= 31 && i <= 31; i++)
                {
                    if (_colorantes[i].Nome != _NomeCorante[i].Text || Math.Round(_colorantes[i].MassaEspecifica, 3) != Math.Round(_massaEspecifica[i].ToDouble(), 3) ||
                         _colorantes[i].Habilitado != _circuito[i].Checked)
                    {
                        retorno = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private int gerarEventoAlteradoProdutos(int result, string detalhes = "")
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
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
            {
                bool sair =
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Confirmar_Sair,
                        Negocio.IdiomaResxExtensao.Global_Sim,
                        Negocio.IdiomaResxExtensao.Global_Nao);

                if (sair)
                {
                    if(this.fCalAuto != null)
                    {
                        this.fCalAuto.Close();
                    }
                    
                    this.Close();
                }
            }
        }

        private void btnPathMonitoramentoDAT_Click(object sender, EventArgs e)
        {
            using (fCaminhoArquivo f = new fCaminhoArquivo(txtPathMonitoramentoDat.Text))
            {
                if (f.ShowDialog() == DialogResult.OK)
                    txtPathMonitoramentoDat.Text = f.FilePath;
            }
        }

        private void chkCircuito_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)sender;
                int index = int.Parse(chk.Tag.ToString());

                _massaEspecifica[index].Enabled = (chk.Checked);
                _NomeCorante[index].Enabled = (chk.Checked);
                _correspondencia[index].Enabled = (chk.Checked);
                _base[index].Enabled = (chk.Checked);
                _bicoIndividual[index].Enabled = (chk.Checked);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

       
        private void chkControlarNivel_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void chkShot_CheckedChanged(object sender, EventArgs e)
        {
            txtUniMedidaValorShot.Enabled = ((CheckBox)sender).Checked;
        }

        private void btnIniExecutar_Click(object sender, EventArgs e)
        {
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenserMover_P3 modBusDispenserMover_P3 = null;

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
                case Dispositivo.Placa_3:
                    {
                        modBusDispenserMover_P3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                        break;
                    }
            }
            if (modBusDispenserMover_P3 == null)
            {
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
            }
            
            InicializacaoCircuitosVO param_ini = new InicializacaoCircuitosVO();

            try
            {
                if (modBusDispenserMover_P3 == null)
                {
                    if (!Operar.Conectar(ref dispenser))
                    {
                        return;
                    }
                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        return;
                    }
                }
                else if(!Operar.ConectarP3(ref modBusDispenserMover_P3))
                {
                    return;
                }

                param_ini.Dispenser = new List<IDispenser>();
                param_ini.DispenserP3 = modBusDispenserMover_P3;
                if (modBusDispenserMover_P3 == null)
                {
                    param_ini.Dispenser.Add(dispenser);
                    if (_parametros.IdDispositivo2 != 0)
                    {
                        param_ini.Dispenser.Add(dispenser2);
                    }
                }

                //Recupera circuitos de colorantes habilitados
                if (modBusDispenserMover_P3 == null)
                {
                    param_ini.Circuitos =
                        _colorantes.Where(c => c.Habilitado == true).Select(c => c.Circuito).ToArray();

                    param_ini.Dispositivo =
                        _colorantes.Where(c => c.Habilitado == true).Select(c => c.Dispositivo).ToArray();
                }
                else
                {
                    param_ini.Circuitos =_colorantes.Where(c => (c.Habilitado == true && c.Dispositivo == 1)).Select(c => c.Circuito).ToArray();

                    param_ini.Dispositivo = _colorantes.Where(c => c.Habilitado == true && c.Dispositivo == 1).Select(c => c.Dispositivo).ToArray();
                }

                /* Popula objeto de parâmetros da inicialização de circuitos 
                 * com valores de entrada do usuário */
                param_ini.PulsoInicial = txtInicializacaoPulsoInicial.ToInt();
                param_ini.PulsoLimite = txtInicializacaoPulsoLimite.ToInt();
                param_ini.VariacaoPulso = txtInicializacaoVariacaoPulso.ToInt();
                param_ini.StepVariacao = txtInicializacaoStepVariacao.ToDouble();
                param_ini.Velocidade = txtInicializacaoVelocidade.ToInt();
                param_ini.Aceleracao = txtInicializacaoAceleracao.ToInt();
                param_ini.MovimentoInicialReverso = chkInicializacaoMovimentoReverso.Checked;
                param_ini.QtdeCircuitoGrupo =
                    int.Parse(cmbIniciaizacaoQtdeCircuitosGrupo.SelectedItem.ToString());

                using (Form f = new fInicializacaoCircuitos(param_ini))
                {
                    f.ShowDialog();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
                }
            }
            finally
            {
                param_ini = null;

                if (dispenser != null)
                {
                    dispenser.Disconnect();
                }
                if(dispenser2 != null)
                {
                    dispenser2.Disconnect();
                }
            }
        }

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }

        private void btnPathRepositorioDat_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {                
                d.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (d.ShowDialog() == DialogResult.OK)
                {
                    txtPathRepositorioDat.Text = d.SelectedPath;
                }
            }
        }

        private void chkPurgaControleExecucao_CheckedChanged(object sender, EventArgs e)
        {
            txtPurgaPrazoExecucao.Enabled = chkPurgaControleExecucao.Checked;
        }

        private void btnPathLogProcDispensa_Click(object sender, EventArgs e)
        {
            using (fCaminhoArquivo f = new fCaminhoArquivo(txtPathLogProcDispensa.Text))
            {
                if (f.ShowDialog() == DialogResult.OK)
                    txtPathLogProcDispensa.Text = f.FilePath;
            }
        }

        private void btnPathLogQtdeDispensa_Click(object sender, EventArgs e)
        {
            using (fCaminhoArquivo f = new fCaminhoArquivo(txtPathLogQtdeDispensa.Text))
            {
                if (f.ShowDialog() == DialogResult.OK)
                    txtPathLogQtdeDispensa.Text = f.FilePath;
            }
        }

        private void btnPathLogComunicacao_Click(object sender, EventArgs e)
        {
            using (fCaminhoArquivo f = new fCaminhoArquivo(txtPathLogComunicacao.Text))
            {
                if (f.ShowDialog() == DialogResult.OK)
                    txtPathLogComunicacao.Text = f.FilePath;
            }
        }

        private void chkLogComunicacao_CheckedChanged(object sender, EventArgs e)
        {
            btnPathLogComunicacao.Enabled = chkLogComunicacao.Checked;
        }

        private void cmbGeralDispositivo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Dispositivo dispositivo =
                (Dispositivo)((ComboBoxItem)(cmbGeralDispositivo.SelectedItem)).Value;

            if (dispositivo == Dispositivo.Placa_1)
            {  
                chkInicializacaoExecutarAntesPurga.Checked = false;
                tabInicializarCircuito.Enabled = false;
            }
            else
            {               
                tabInicializarCircuito.Enabled = true;
            }

            if(dispositivo == Dispositivo.Placa_2)
            {
                btnValve.Enabled = true;
                btnInput.Enabled = true;
            }
            else
            {
                btnValve.Enabled = false;
                btnInput.Enabled = false;
            }
        }

        #endregion

        #region Métodos      

        void PersistirColorantesSemMensagem()
        {
            try
            {
                for (int i = 0; i <= 15; i++)
                {
                    _colorantes[i].Circuito = i + 1;
                    _colorantes[i].Nome = _NomeCorante[i].Text;
                    _colorantes[i].MassaEspecifica = _massaEspecifica[i].ToDouble();
                    if (_colorantes[i].Seguidor > 0)
                    {
                        _colorantes[i].Habilitado = true;
                    }
                    else
                    {
                        _colorantes[i].Habilitado = _circuito[i].Checked;
                    }

                    int correspondente = 0;
                    int.TryParse(_correspondencia[i].Text, out correspondente);
                    _colorantes[i].Correspondencia = correspondente;

                    _colorantes[i].NivelMaximo = _nivelMaximo[i].ToInt();
                    _colorantes[i].NivelMinimo = _nivelMinimo[i].ToInt();

                    _colorantes[i].IsBase = _base[i].Checked;
                    _colorantes[i].IsBicoIndividual = _bicoIndividual[i].Checked;
                    _colorantes[i].VolumePurga = _purgaVolume[i].ToDouble();
                    if (!_colorantes[i].Habilitado)
                    {
                        _colorantes[i].Step = 0;
                    }
                }

                for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                {
                    _colorantes[i].Circuito = i + 1;
                    _colorantes[i].Nome = _NomeCorante[i].Text;
                    _colorantes[i].MassaEspecifica = _massaEspecifica[i].ToDouble();
                    _colorantes[i].Habilitado = _circuito[i].Checked;

                    int correspondente = 0;
                    int.TryParse(_correspondencia[i].Text, out correspondente);
                    _colorantes[i].Correspondencia = correspondente;

                    _colorantes[i].NivelMaximo = _nivelMaximo[i].ToInt();
                    _colorantes[i].NivelMinimo = _nivelMinimo[i].ToInt();
                    _colorantes[i].IsBase = _base[i].Checked;
                    _colorantes[i].IsBicoIndividual = _bicoIndividual[i].Checked;
                    _colorantes[i].VolumePurga = _purgaVolume[i].ToDouble();
                    if (_colorantes[i].Seguidor > 0)
                    {
                        _colorantes[i].Habilitado = true;
                    }
                    else
                    {
                        _colorantes[i].Habilitado = _circuito[i].Checked;
                    }
                    if (!_colorantes[i].Habilitado)
                    {
                        _colorantes[i].Step = 0;
                    }
                }

                string msg = "";
                if (!Util.ObjectColorante.Validate(_colorantes, out msg))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                            + Environment.NewLine + Environment.NewLine
                            + msg);
                    }

                    return;
                }

                Util.ObjectColorante.Persist(_colorantes);
              
                ResetarCalibragem();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }

        void PersistirColorantes()
        {
            try
            {
                for (int i = 0; i <= 15; i++)
                {
                    _colorantes[i].Circuito = i + 1;
                    _colorantes[i].Nome = _NomeCorante[i].Text;
                    _colorantes[i].MassaEspecifica = _massaEspecifica[i].ToDouble();
                    if (_colorantes[i].Seguidor > 0)
                    {
                        _colorantes[i].Habilitado = true;
                    }
                    else
                    {
                        _colorantes[i].Habilitado = _circuito[i].Checked;
                    }

                    int correspondente = 0;
                    int.TryParse(_correspondencia[i].Text, out correspondente);
                    _colorantes[i].Correspondencia = correspondente;

                    _colorantes[i].NivelMaximo = _nivelMaximo[i].ToInt();
                    _colorantes[i].NivelMinimo = _nivelMinimo[i].ToInt();

                    _colorantes[i].IsBase = _base[i].Checked;
                    _colorantes[i].IsBicoIndividual = _bicoIndividual[i].Checked;
                    _colorantes[i].VolumePurga = _purgaVolume[i].ToDouble();
                    if(!_colorantes[i].Habilitado)
                    {
                        _colorantes[i].Step = 0;
                    }
                }

                for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                {
                    _colorantes[i].Circuito = i + 1;
                    _colorantes[i].Nome = _NomeCorante[i].Text;
                    _colorantes[i].MassaEspecifica = _massaEspecifica[i].ToDouble();
                    _colorantes[i].Habilitado = _circuito[i].Checked;
                    
                    int correspondente = 0;
                    int.TryParse(_correspondencia[i].Text, out correspondente);
                    _colorantes[i].Correspondencia = correspondente;
                    
                    _colorantes[i].NivelMaximo = _nivelMaximo[i].ToInt();
                    _colorantes[i].NivelMinimo = _nivelMinimo[i].ToInt();
                    _colorantes[i].IsBase = _base[i].Checked;
                    _colorantes[i].IsBicoIndividual = _bicoIndividual[i].Checked;
                    _colorantes[i].VolumePurga = _purgaVolume[i].ToDouble();
                    if (_colorantes[i].Seguidor > 0)
                    {
                        _colorantes[i].Habilitado = true;
                    }
                    else
                    {
                        _colorantes[i].Habilitado = _circuito[i].Checked;
                    }
                    if (!_colorantes[i].Habilitado)
                    {
                        _colorantes[i].Step = 0;
                    }
                }


                string msg = "";
                if (!Util.ObjectColorante.Validate(_colorantes, out msg))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                            + Environment.NewLine + Environment.NewLine
                            + msg);
                    }

                    return;
                }

                Util.ObjectColorante.Persist(_colorantes);

                bool confirma = false;
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    confirma = m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso
                        + Environment.NewLine
                        + Negocio.IdiomaResxExtensao.Configuracoes_Confirma_RecarregarDadosCalibragem,
                        Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                }

                if (confirma)
                    ResetarCalibragem();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }

        void PersistirColorantesPar()
        {
            try
            {
                for (int i = 0; i <= 15; i++)
                {
                    _colorantes[i].Circuito = i + 1;
                    _colorantes[i].Nome = _NomeCorante[i].Text;
                    _colorantes[i].MassaEspecifica = _massaEspecifica[i].ToDouble();
                    if (_colorantes[i].Seguidor > 0)
                    {
                        _colorantes[i].Habilitado = true;
                    }
                   

                    int correspondente = 0;
                    int.TryParse(_correspondencia[i].Text, out correspondente);
                    _colorantes[i].Correspondencia = correspondente;

                    _colorantes[i].NivelMaximo = _nivelMaximo[i].ToInt();
                    _colorantes[i].NivelMinimo = _nivelMinimo[i].ToInt();

                    _colorantes[i].IsBase = _base[i].Checked;
                    _colorantes[i].IsBicoIndividual = _bicoIndividual[i].Checked;
                    _colorantes[i].VolumePurga = _purgaVolume[i].ToDouble();
                    if (!_colorantes[i].Habilitado)
                    {
                        _colorantes[i].Step = 0;
                    }
                }

                for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                {
                    _colorantes[i].Circuito = i + 1;
                    _colorantes[i].Nome = _NomeCorante[i].Text;
                    _colorantes[i].MassaEspecifica = _massaEspecifica[i].ToDouble();
                    _colorantes[i].Habilitado = _circuito[i].Checked;

                    int correspondente = 0;
                    int.TryParse(_correspondencia[i].Text, out correspondente);
                    _colorantes[i].Correspondencia = correspondente;

                    _colorantes[i].NivelMaximo = _nivelMaximo[i].ToInt();
                    _colorantes[i].NivelMinimo = _nivelMinimo[i].ToInt();
                    _colorantes[i].IsBase = _base[i].Checked;
                    _colorantes[i].IsBicoIndividual = _bicoIndividual[i].Checked;
                    _colorantes[i].VolumePurga = _purgaVolume[i].ToDouble();
                    if (_colorantes[i].Seguidor > 0)
                    {
                        _colorantes[i].Habilitado = true;
                    }
                    
                    if (!_colorantes[i].Habilitado)
                    {
                        _colorantes[i].Step = 0;
                    }
                }


                string msg = "";
                if (!Util.ObjectColorante.Validate(_colorantes, out msg))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                            + Environment.NewLine + Environment.NewLine
                            + msg);
                    }

                    return;
                }

                Util.ObjectColorante.Persist(_colorantes);

                bool confirma = false;
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    confirma = m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso
                        + Environment.NewLine
                        + Negocio.IdiomaResxExtensao.Configuracoes_Confirma_RecarregarDadosCalibragem,
                        Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                }

                if (confirma)
                    ResetarCalibragem();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }


        bool PersistirParametros(bool isRessetCal = true)
        {
            bool retorno = true;
            Util.ObjectParametros p = Util.ObjectParametros.Load();
            
            //Geral
            p.ResponseTimeout = txtTimeout.ToInt();
            p.Velocidade = txtVelocidadeGlobal.ToInt();
            p.Aceleracao = txtAceleracaoGlobal.ToInt();
            p.DelayReverso = txtReversoDelay.ToInt();
            p.PulsoReverso = txtReversoPulsos.ToInt();
            p.SomarPulsoReverso = chkGeralSomarRevSteps.Checked;
            if (this.User.Tipo == 1)
            {
                p.HabilitarTecladoVirtual = this._parametros.HabilitarTecladoVirtual;
            }
            else 
            {
                p.HabilitarTecladoVirtual = chkTecladoVirtualUser.Checked;
            }
            
            p.QtdTentativasConexao = txt_QtdTentativasConexao.ToInt(); 

            p.HabilitarDispensaSequencialP1 = chkDispensaSequencialP1.Checked;
            p.HabilitarDispensaSequencialP2 = chkDispensaSequencialP2.Checked;

           
            if (this.User.Tipo == 1)
            {
                p.IdDispositivo = ((ComboBoxItem)(cmbGeralDispositivo.SelectedItem)).Value;
                p.IdDispositivo2 = ((ComboBoxItem)(cmbGeralDispositivo_2.SelectedItem)).Value;
                p.NomeDispositivo = txt_nome_Dispositivo.Text;
                p.NomeDispositivo2 = txt_nome_Dispositivo_2.Text;
            }
            else
            {
                p.IdDispositivo = ((ComboBoxItem)(cmbGeralDispositivoUser.SelectedItem)).Value;
                p.IdDispositivo2 = ((ComboBoxItem)(cmbGeralDispositivo_2User.SelectedItem)).Value;
                p.NomeDispositivo = txt_nome_DispositivoUser.Text;
                p.NomeDispositivo2 = txt_nome_Dispositivo_2User.Text;
            }

            if (this.User.Tipo == 1)
            {
                if (cmbTipoDosagemExec.SelectedValue == null)
				{
					MessageBox.Show("Tipo de Dosagem não selecionado", "ATENÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					cmbTipoDosagemExec.Focus();
					return false;
				}

				p.TipoDosagemExec = Convert.ToInt32(cmbTipoDosagemExec.SelectedValue.ToString());
            }
            else
            {
                p.TipoDosagemExec = Convert.ToInt32(cmbTipoDosagemExec_2.SelectedValue.ToString());
            }

            p.HabilitarPurgaIndividual = chkHabilitarPurgaIndividual.Checked;
            if (this.User.Tipo == 1)
            {
                p.HabilitarTouchScrenn = this._parametros.HabilitarTouchScrenn;
            }
            else 
            {
                p.HabilitarTouchScrenn = chkTouchScrennUser.Checked;
            }

           
            p.HabLimpBicos = chkGeralHabLimpBicos.Checked;
            p.DelayLimpBicos = txtGeralDelayLimpBicos.ToInt();
            if(p.DelayLimpBicos < 1)
            {
                p.DelayLimpBicos = 1;
            }
            p.TipoLimpBicos = Convert.ToInt32(cbTipoLimpBicos.SelectedValue.ToString());
          

            //Purga
            p.PrazoExecucaoPurga = new TimeSpan(txtPurgaPrazoExecucao.ToInt(), 0, 0);
            p.VolumePurga = txtPurgaVolume.ToDouble();
            p.VelocidadePurga = txtPurgaVelocidade.ToInt();
            p.AceleracaoPurga = txtPurgaAceleracao.ToInt();
            p.DelayPurga = txtPurgaDelay.ToInt();
            p.ControlarExecucaoPurga = chkPurgaControleExecucao.Checked;
            p.DesabilitarInterfacePurga = chkPurgaInterface.Checked;
            p.ExigirExecucaoPurga = chkExigirExecucaoPurga.Checked;
            p.PurgaSequencial = chkPurgaSequencial.Checked;

            //DAT
            p.PathMonitoramentoDAT = txtPathMonitoramentoDat.Text;
            p.PathRepositorioDAT = txtPathRepositorioDat.Text;
            p.UtilizarCorrespondenciaDAT = chkDatUtilizarCorrespondencia.Checked;
            p.BasePosicaoCircuitoDAT = (rdbDatBase00.Checked) ? 0 : 1;
            p.DesabilitarInterfaceDispensaSequencial = chkDatInterfaceDispSequencial.Checked;
            p.DesabilitarInterfaceDispensaSimultanea = chkDatInterfaceDispSimultanea.Checked;
            p.DesabilitarMonitoramentoFilaDAT= chkDisableFilaDat.Checked;
            p.PathMonitoramentoFilaDAT = txtPathMonitoramentoFilaDat.Text;
            p.DelayMonitoramentoFilaDAT = txtDelayMonitoramentoFilaDAT.ToInt();

            p.Dat_06_BAS_1_IsPonto = _parametros.Dat_06_BAS_1_IsPonto;
            p.Dat_06_BAS_Pref = _parametros.Dat_06_BAS_Pref;
            p.Dat_06_CAN_1_IsPonto = _parametros.Dat_06_CAN_1_IsPonto;
            p.Dat_06_CAN_Pref = _parametros.Dat_06_CAN_Pref;
            p.Dat_06_FRM_1_IsPonto = _parametros.Dat_06_FRM_1_IsPonto;
            p.Dat_06_FRM_Pref = _parametros.Dat_06_FRM_Pref;
            p.Dat_06_FRM_SEP = _parametros.Dat_06_FRM_SEP;
            p.Dat_06_UNT_1_IsPonto = _parametros.Dat_06_UNT_1_IsPonto;
            p.Dat_06_UNT_2_IsPonto = _parametros.Dat_06_UNT_2_IsPonto;
            p.Dat_06_UNT_Pref = _parametros.Dat_06_UNT_Pref;
            p.Dat_06_BAS_Habilitado = _parametros.Dat_06_BAS_Habilitado;

            p.Dat_05_BAS_1_IsPonto = _parametros.Dat_05_BAS_1_IsPonto;
            p.Dat_05_BAS_Pref = _parametros.Dat_05_BAS_Pref;
            p.Dat_05_CAN_1_IsPonto = _parametros.Dat_05_CAN_1_IsPonto;
            p.Dat_05_CAN_Pref = _parametros.Dat_05_CAN_Pref;
            p.Dat_05_FRM_1_IsPonto = _parametros.Dat_05_FRM_1_IsPonto;
            p.Dat_05_FRM_Pref = _parametros.Dat_05_FRM_Pref;
            p.Dat_05_FRM_SEP = _parametros.Dat_05_FRM_SEP;
            p.Dat_05_UNT_1_IsPonto = _parametros.Dat_05_UNT_1_IsPonto;
            p.Dat_05_UNT_2_IsPonto = _parametros.Dat_05_UNT_2_IsPonto;
            p.Dat_05_UNT_Pref = _parametros.Dat_05_UNT_Pref;
            p.Dat_05_BAS_Habilitado = _parametros.Dat_05_BAS_Habilitado;

            p.PadraoConteudoDAT = Convert.ToInt32(cmbDatPadrao.SelectedValue.ToString());

            p.DesabilitarVolumeMinimoDat = chkDesabilitarVolumeMinimoDat.Checked;
            p.VolumeMinimoDat = txtVolumeMinimoDat.ToDouble();

            p.CreateFileTmpUDCP = chb_CreateFileTmpUDCP.Checked ? 1 : 0;
            p.DelayUDCP = Convert.ToInt32(utxt_DelayUDCP.Text);
            p.ExtFileTmpUDCP = utxt_ExtFileTmpUDCP.Text;
            p.ProcRemoveLataUDCP = chb_ProcRemoveLataUDCP.Checked;
            p.DisablePopUpDispDat = chkDisablePopUpDispDat.Checked;

            //Nível de colorante
            p.ControlarNivel = chkControlarNivel.Checked;
            p.VolumeMinimo = 200;
            p.VolumeMaximo = 2000;

            //Inicialização de circuitos
            p.IniPulsoInicial = txtInicializacaoPulsoInicial.ToInt();
            p.IniPulsoLimite = txtInicializacaoPulsoLimite.ToInt();
            p.IniVariacaoPulso = txtInicializacaoVariacaoPulso.ToInt();
            p.IniStepVariacao = txtInicializacaoStepVariacao.ToDouble();
            p.IniAceleracao = txtInicializacaoAceleracao.ToInt();
            p.IniVelocidade = txtInicializacaoVelocidade.ToInt();
            p.IniMovimentoReverso = chkInicializacaoMovimentoReverso.Checked;
            p.InicializarCircuitosPurga = chkInicializacaoExecutarAntesPurga.Checked;
            p.QtdeCircuitoGrupo =
                int.Parse(cmbIniciaizacaoQtdeCircuitosGrupo.SelectedItem.ToString());
            p.DesabilitarInterfaceInicializacaoCircuito = chkIncializacaoInterface.Checked;
            p.InicializarCircuitosPurgaIndividual = chkInicializacaoExecutarAntesPurgaIndividual.Checked;

            //Monitoramento de circuitos
            p.MonitPulsos = txtMonitPulsos.ToInt();
            p.MonitVelocidade = txtVelociadadeMonitCircuitos.ToInt();
            p.MonitAceleracao = txtAceleracaoMonitCircuitos.ToInt();
            p.MonitDelay = txtDelayMonitCircuitos.ToInt();
            p.MonitTimerDelay = txtTempoMonitCircuitos.ToInt();
            p.MonitTimerDelayIni = txtTempoIniMonitCircuitos.ToInt();
            p.MonitMovimentoReverso = chkMonitMovimentoReverso.Checked;
            p.QtdeMonitCircuitoGrupo = int.Parse(cmbMonitQtdeCircuitosGrupo.SelectedItem.ToString());
            p.DesabilitarInterfaceMonitCircuito = chkMonitInterface.Checked;
            p.DesabilitarProcessoMonitCircuito = chkMonitProcesso.Checked;

            //Monitoramento Produção
            p.TipoProducao = ckTipoProducao.SelectedItem.ToString();
            p.IpProducao = txtIpProducao.Text;
            p.PortaProducao = txtPortProducao.ToInt().ToString();
            p.DesabilitaMonitProcessoProducao = chkMonitProducao.Checked;

            //Monitoramento Sinc Formula           
            p.IpSincFormula = txtIpSincFormula.Text;
            p.PortaSincFormula = txtPortSincFormula.ToInt().ToString();
            p.DesabilitaMonitSincFormula = chkMonitSincFormula.Checked;

            //Monitoramento Sinc Token           
            p.IpSincToken = txtIpToken.Text;
            p.PortaSincToken = txtPortToken.ToInt().ToString();
            p.DesabilitaMonitSyncToken = chkMonitToken.Checked;
            
            if (ckTipoEventos.SelectedItem == null)
			{
				MessageBox.Show("Tipo de Eventos não selecionado", "ATENÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ckTipoEventos.Focus();
				return false;
			}

			p.TipoEventos = ckTipoEventos.SelectedItem.ToString();

            //Monitoramento Sinc BkpCalibragem      
            p.UrlSincBkpCalibragem = txtUrlSincBkpCalibragem.Text;
            p.DesabilitaMonitSyncBkpCalibragem = chkMonitBkpCalibragem.Checked;

            //base Dados
            if (cmbTipoBaseDados.SelectedValue == null)
            {
                MessageBox.Show("Tipo de base de dados não selecionado.", "ATENÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTipoBaseDados.Focus();
				return false;
            }

            p.TipoBaseDados = cmbTipoBaseDados.SelectedValue.ToString();
            p.PathBasesDados = txtPathBaseDados.Text;


            p.TimeoutPingTcp = txtTimeoutPingTcp.ToInt();

            //Unidade de medida
            p.HabilitarMililitro = chkUniMedidaMililitro.Checked;
            p.HabilitarOnca = chkUniMedidaOnca.Checked;
            p.HabilitarShot = chkUniMedidaShot.Checked;
            p.ValorShot = txtUniMedidaValorShot.ToDouble();
            p.HabilitarGrama = chkUniMedidaGramas.Checked;

            p.ValorFraction = txtUniMedidaValorFracao.ToInt();

            if (rdbUniMedidaGrama.Checked)
            {
                p.UnidadeMedidaNivelColorante = (int)Percolore.IOConnect.Core.UnidadeMedida.Grama;
            }
            else if (rdbUniMedidaOnca.Checked)
            {
                p.UnidadeMedidaNivelColorante = (int)Percolore.IOConnect.Core.UnidadeMedida.Onca;
            }
            else if (rdbUniMedidaFracao.Checked)
            {
                p.UnidadeMedidaNivelColorante = (int)Percolore.IOConnect.Core.UnidadeMedida.Fraction;
            }
            else
            {
                p.UnidadeMedidaNivelColorante = (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro;
            }

            //Log
            p.PathLogProcessoDispensa = txtPathLogProcDispensa.Text;
            p.PathLogControleDispensa = txtPathLogQtdeDispensa.Text;
            p.HabilitarLogComunicacao = chkLogComunicacao.Checked;
            p.PathLogComunicacao = txtPathLogComunicacao.Text;
            p.HabilitarLogAutomateTesterProt = chkLogAutomateTesterProt.Checked;
            p.LogAutomateBackup = chkLogAutomateBackup.Checked;

            p.LogStatusMaquina = chkLogStatusMaquina.Checked;

            //Recircular
            p.HabilitarRecirculacao = chkHabilitarMonitRecirculacao.Checked;
            p.DelayMonitRecirculacao = txtDelayMonitRecirculacao.ToInt() > 0 ? txtDelayMonitRecirculacao.ToInt() : 1;

            //Placa Movimentacao
            p.Address_PlacaMov = txt_Address_PlacaMov.ToInt() > 0 ? txt_Address_PlacaMov.ToInt() : 3;
            p.NomeDispositivo_PlacaMov = txt_nome_DispositivoPlMov.Text;
            p.DelayAlertaPlacaMov = txtDelayAlertaPlacaMov.ToInt() > 0 ? txtDelayAlertaPlacaMov.ToInt() : 5;
            

            //Recircular Auto
            p.HabilitarRecirculacaoAuto = chkHabilitarMonitRecirculacaoAuto.Checked;

			if ((cbTipoTempoMonitRecirculacaoAuto.SelectedValue != null) && (cbTipoTempoMonitRecirculacaoAuto.SelectedValue.ToString() == "1"))
            {
                int delay = txtDelayMonitRecirculacaoAuto.ToInt();
                if(delay < 0 )
                {
                    delay = 1;
                }
                p.DelayMonitRecirculacaoAuto = delay > 0 ? delay * 60 : 1;
            }
            else
            {
                p.DelayMonitRecirculacaoAuto = txtDelayMonitRecirculacaoAuto.ToInt() > 0 ? txtDelayMonitRecirculacaoAuto.ToInt() : 1;
            }
            p.DelayNotificacaotRecirculacaoAuto = txtDelayNotificacaotRecirculacaoAuto.ToInt() > 0 ? txtDelayNotificacaotRecirculacaoAuto.ToInt() : 1;
            p.QtdNotificacaotRecirculacaoAuto = txtQtdNotificacaotRecirculacaoAuto.ToInt() > 0 ? txtQtdNotificacaotRecirculacaoAuto.ToInt() : 1;

            p.TempoReciAuto = Convert.ToInt32(cbTipoTempoMonitRecirculacaoAuto.SelectedValue.ToString());
            
            p.HabilitarFormulaPersonalizada = this._parametros.HabilitarFormulaPersonalizada ;
            p.HabilitarTesteRecipiente = this._parametros.HabilitarTesteRecipiente;
            p.HabilitarIdentificacaoCopo = this._parametros.HabilitarIdentificacaoCopo;
            p.HabilitarDispensaSequencial = this._parametros.HabilitarDispensaSequencial;
            p.TreinamentoCal = this._parametros.TreinamentoCal ;
            p.DelayEsponja = this._parametros.DelayEsponja ;
            p.ViewMessageProc = this._parametros.ViewMessageProc;
            p.NameRemoteAccess = this._parametros.NameRemoteAccess;

            try
            {
                string msg = "";
                if (retorno && !p.Validate(p, out msg))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                            + Environment.NewLine + Environment.NewLine
                            + msg);
                    }
                    retorno = false;
                   
                }

                if (retorno)
                {
                    Util.ObjectParametros.Persist(p);
                    if (isRessetCal)
                    {
                        bool confirma = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            confirma = m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso
                                + Environment.NewLine
                                + Negocio.IdiomaResxExtensao.Configuracoes_Confirma_RecarregarDadosCalibragem,
                                Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }

                        if (confirma)
                            ResetarCalibragem();
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }

            return retorno;
        }

        void PersistirCalibragem(bool viewSucces = true)
        {
            try
            {
                Util.ObjectCalibragem _cal = new Util.ObjectCalibragem();
                _cal.Motor = _calibragem.Motor;
                _cal.UltimoPulsoReverso = _calibragem.UltimoPulsoReverso;
                _cal.MinimoFaixas = Convert.ToInt32(cb_NumeroMinimoFaixaCal.Text);
                _cal.Valores = new List<ValoresVO>();
                for(int i = 0; i < this.dtCal.Rows.Count;i++)
                {
                    #region  Recupera entradas do usuário e insere na calibragem do corante selecionada

                    ValoresVO valores = new ValoresVO();
                    valores.Volume = double.Parse(this.dtCal.Rows[i]["Volume"].ToString());
                    valores.PulsoHorario = int.Parse(this.dtCal.Rows[i]["Pulsos"].ToString());
                    valores.PulsoReverso = int.Parse(this.dtCal.Rows[i]["PulsosRev"].ToString());
                    valores.Velocidade = int.Parse(this.dtCal.Rows[i]["Velocidade"].ToString());
                    valores.Delay = int.Parse(this.dtCal.Rows[i]["Delay"].ToString());
                    valores.MassaMedia = double.Parse(this.dtCal.Rows[i]["MassaMedia"].ToString());
                    valores.Aceleracao = int.Parse(this.dtCal.Rows[i]["Aceleracao"].ToString());

                    //Grava valor original do desvio
                    double desvio = double.Parse(this.dtCal.Rows[i]["DesvioMedio"].ToString().Replace("%", ""));
                    valores.DesvioMedio = desvio / 100;

                    _cal.Valores.Add(valores);
                    #endregion
                }
                string msg = "";
                if (!Util.ObjectCalibragem.Validate(_cal, out msg))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                            + Environment.NewLine + Environment.NewLine
                            + msg);
                    }

                    return;
                }
               
                Util.ObjectCalibragem.Add(_cal);
                _historicoValores.Clear();

                if (viewSucces)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso);
                    }
                }
                _calibragem = Util.ObjectCalibragem.Load(_cal.Motor);
                _calibragem.Valores = _calibragem.Valores.OrderByDescending(o => o.Volume).ToList();
               
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }

        void PersistirCalibracaoAuto()
        {
            try
            {
                Util.ObjectCalibracaoAutomatica _cal = new Util.ObjectCalibracaoAutomatica();
                _cal.CapacideMaxBalanca = txtCapacidadeMaxBalanca.ToDouble();
                _cal.MaxMassaAdmRecipiente = txtMassaAdmRecipiente.ToDouble();
                _cal.MinMassaAdmRecipiente = txtMinMassaAdmRecipiente.ToDouble();
                _cal.VolumeMaxRecipiente = txtVolumeMaxRecipiente.ToDouble();
                _cal.NumeroMaxTentativaRec = txtTentativasRecipiente.ToInt();

                _cal.listOperacaoAutomatica = new List<Negocio.OperacaoAutomatica>();
                for (int i = 0; i < this.dtCalAuto.Rows.Count; i++)
                {
                    #region  Recupera entradas do usuário e insere na calibragem do corante selecionada

                    Negocio.OperacaoAutomatica _op = new Negocio.OperacaoAutomatica();
                    _op.IsPrimeiraCalibracao = int.Parse(this.dtCalAuto.Rows[i]["IsPrimeiraCalibracao"].ToString());
                    _op.Motor = int.Parse(this.dtCalAuto.Rows[i]["Motor"].ToString());
                    _op.Volume = double.Parse(this.dtCalAuto.Rows[i]["Volume"].ToString());
                    _op.DesvioAdmissivel = double.Parse(this.dtCalAuto.Rows[i]["DesvioAdmissivel"].ToString());
                    _op.IsCalibracaoAutomatica = int.Parse(this.dtCalAuto.Rows[i]["IsCalibracaoAutomatica"].ToString());
                    _op.NumeroMaxTentativa = int.Parse(this.dtCalAuto.Rows[i]["NumeroMaxTentativa"].ToString());
                    _op.IsRealizarMediaMedicao = int.Parse(this.dtCalAuto.Rows[i]["IsRealizarMediaMedicao"].ToString());
                    _op.NumeroDosagemTomadaMedia = int.Parse(this.dtCalAuto.Rows[i]["NumeroDosagemTomadaMedia"].ToString());

                    _cal.listOperacaoAutomatica.Add(_op);
                    #endregion
                }
                _cal._colorante = _calibracaoAuto._colorante;
                _cal._calibragem = _calibracaoAuto._calibragem;


                Util.ObjectCalibracaoAutomatica.Update(_cal, true);
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso);
                }
                _calibracaoAuto= Util.ObjectCalibracaoAutomatica.Load(_cal._calibragem.Motor);
               

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }

        void PersistirAdmin()
        {
            StringBuilder mensagens = new StringBuilder();

            string senha = txtSenhaAdministrador.Text.Trim();
            if (senha.Length < 4)
            {
                mensagens.AppendLine(
                    Negocio.IdiomaResxExtensao.Configuracoes_Informacao_SenhaDigitos);
            }

            string serial = txtNumeroSerial.Text.Trim();
            if (serial.Length < 4)
            {
                mensagens.AppendLine(
                    Negocio.IdiomaResxExtensao.Configuracoes_Informacao_NumeroSerieDigitos);
            }

            if (mensagens.Length > 0)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(mensagens.ToString());
                }

                return;
            }

            try
            {
                using (IOConnectRegistry icntRegistry = new IOConnectRegistry())
                {
                    icntRegistry.SetSenhaAdministrador(senha);
                }

                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    percRegistry.SetSerialNumber(serial);
                    Util.ObjectEventos.UpdateEventosNumeroSerie(serial);
                }

               
                DateTimeOffset instalacao = DateTimeOffset.Parse(txtDataInstalacao.Text);
                using (IOConnectRegistry icnt = new IOConnectRegistry())
                {
                    icnt.SetDataInstalacao(instalacao.ToUnixTimeSeconds());
                }

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }

        void PersistirRecirculacao()
        {
            try
            {
                if (PersistirParametros(false))
                {
                    string msg = "";
                    if (!Util.ObjectRecircular.Validate(this._listRecircular, out msg))
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                                + Environment.NewLine + Environment.NewLine
                                + msg);
                        }
                        return;
                    }
                    DateTime dtAgora = DateTime.Now;
                    for (int i = 0; i < this._listRecircular.Count; i++)
                    {
                        this._listRecircular[i].DtInicio = dtAgora;
                        this._listRecircular[i].VolumeDosado = 0;
                    }
                    Util.ObjectRecircular.Persist(this._listRecircular);

                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso,
                            Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                    }
                    this._listRecircular = Util.ObjectRecircular.List();
                }
                Util.ObjectParametros.InitLoad();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
            }
        }

        void PersistirRecirculacaoSemMensagem()
        {
            try
            {
                DateTime dtAgora = DateTime.Now;
                for (int i = 0; i < this._listRecircular.Count; i++)
                {
                    this._listRecircular[i].DtInicio = dtAgora;
                    this._listRecircular[i].VolumeDosado = 0;
                }
                string msg = "";
                if (!Util.ObjectRecircular.Validate(this._listRecircular, out msg))
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                            + Environment.NewLine + Environment.NewLine
                            + msg);
                    }
                    return;
                }               
                Util.ObjectRecircular.Persist(this._listRecircular);
                this._listRecircular = Util.ObjectRecircular.List();
                RessetarRecirculacao();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        void ResetarCalibragem()
        {
            _calibragem = null;

            //[Recupera colorantes habilitados]
            List<Util.ObjectColorante> colorantes =
                _colorantes.Where(c => c.Habilitado == true).ToList();

            cb_CalibragemColorante.DisplayMember = "Nome";
            cb_CalibragemColorante.ValueMember = "Motor";
            cb_CalibragemColorante.DataSource = colorantes.ToList();
            cb_CalibragemColorante.SelectedIndex = -1;

            cb_CalibracaoAuto.DisplayMember = "Nome";
            cb_CalibracaoAuto.ValueMember = "Motor";
            cb_CalibracaoAuto.DataSource = colorantes.ToList();
            cb_CalibracaoAuto.SelectedIndex = -1;
            cb_NumeroMinimoFaixaCal.SelectedIndex = -1;   
        }

        private void RessetarRecirculacao()
        {
            try
            {
                #region Recirculacao
                
                chkHabilitarMonitRecirculacao.Checked = _parametros.HabilitarRecirculacao;
                txtDelayMonitRecirculacao.Text = _parametros.DelayMonitRecirculacao.ToString();

                chkHabilitarMonitRecirculacaoAuto.Checked = _parametros.HabilitarRecirculacaoAuto;
                if ((cbTipoTempoMonitRecirculacaoAuto.SelectedValue != null) && (cbTipoTempoMonitRecirculacaoAuto.SelectedValue.ToString() == "1"))
                {
                    int delay = _parametros.DelayMonitRecirculacaoAuto;
                    txtDelayMonitRecirculacaoAuto.Text = ((int)(delay / 60 )).ToString();
                }
                else
                {
                    txtDelayMonitRecirculacaoAuto.Text = _parametros.DelayMonitRecirculacaoAuto.ToString();
                }
                txtDelayNotificacaotRecirculacaoAuto.Text = _parametros.DelayMonitRecirculacaoAuto.ToString();
                txtQtdNotificacaotRecirculacaoAuto.Text = _parametros.QtdNotificacaotRecirculacaoAuto.ToString();

                if (_parametros.TempoReciAuto < 3)
                {
                    cbTipoTempoMonitRecirculacaoAuto.SelectedIndex = (_parametros.TempoReciAuto <= 0) ? 0 : _parametros.TempoReciAuto - 1;
                }

                inicializaDataGridRecircularProdutos();

                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void RessetarPlacaMov()
        {
            try
            {
                if (this._PlacaMov != null && this._PlacaMov._pMotor != null && this._PlacaMov._pMotor.Count > 0)
                {
                    
                    cb_motor_PlacaMov.DisplayMember = "NameTag";
                    cb_motor_PlacaMov.ValueMember = "Circuito";
                    cb_motor_PlacaMov.DataSource = this._PlacaMov._pMotor.ToList();
                    cb_motor_PlacaMov.SelectedIndex = -1;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        #endregion

        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TabControl tbC = (TabControl)sender;
                if (this.alterTabCalibracaoSave && tbC.SelectedTab != tabCalibragemManual)
                {
                    this.alterTabCalibracaoSave = false;
                    using (Mensagem m = new Mensagem(Mensagem.TipoMensagem.Confirmacao))
                    {
                        bool confirma = m.ShowDialog("Deseja salvar calibração", "sim", "não");
                        if(confirma)
                        {
                            PersistirCalibragem(true);
                        }
                    }
                }
                    
                if (tbC.SelectedTab == tabCalibragemManual)
                {
                    cb_CalibragemColorante.SelectedIndex = -1;
                    cb_NumeroMinimoFaixaCal.SelectedIndex = -1;
                    btnCalibragemEditarM.Enabled = false;
                    btnPrimeiraCalibracaoM.Enabled = false;
                    btn_replicar_faixa.Enabled = false;
                    if (_calibragem != null && _calibragem.Valores != null)
                    {
                        _calibragem.Valores.Clear();
                    }

                    lblCalibragemUltimoPulsoRevM.Text = "0";
                    lblCalibragemMotorM.Text = "0";
                    lblCalibragemMassaEspecificaM.Text = "0";
                    atualizaDgVCal();
                    switch ((Dispositivo)_parametros.IdDispositivo)
                    {
                        case Dispositivo.Placa_3:
                            {
                                btn_Abrir_Gaveta.Enabled = true;
                                btn_Abrir_Gaveta.Visible = true;
                                btn_Fechar_Gaveta.Enabled = true;
                                btn_Fechar_Gaveta.Visible = true;
                                break;
                            }
                        default:
                            {
                                btn_Abrir_Gaveta.Enabled = false;
                                btn_Abrir_Gaveta.Visible = false;
                                btn_Fechar_Gaveta.Enabled = false;
                                btn_Fechar_Gaveta.Visible = false;
                                break;
                            }
                    }
                }
                //Calibração Automática
                else if (tbC.SelectedTab == tabCalibragemAuto)
                {
                    cb_CalibracaoAuto.SelectedIndex = -1;

                    if (_calibragem != null && _calibragem.Valores != null)
                    {
                        _calibragem.Valores.Clear();
                    }
                    lblCalibracaoAutoMotor.Text = "0";
                    lblCalibracaoAutoMassaEspecifica.Text = "0";

                    txtCapacidadeMaxBalanca.Text = "0";
                    txtMassaAdmRecipiente.Text = "0";
                    txtMinMassaAdmRecipiente.Text = "0";
                    txtVolumeMaxRecipiente.Text = "0";
                    txtTentativasRecipiente.Text = "0";
                    dgvCalibracaoAuto.Enabled = false;
                    btnCalibracaoAutoEditar.Enabled = false;
                    btnIniciarCalAutomatica.Enabled = false;

                    if (dgvCalibracaoAuto.Rows != null)
                    {
                        dgvCalibracaoAuto.Rows.Clear();
                    }
                }
                //Produtos                
                else if (tbC.SelectedTab == tabProdutos)
                {
                    this._colorantes = Util.ObjectColorante.List();
                    this._colorantesProd = this._colorantes.ToList();
                    bool dispPlacaMov = false;
                    switch ((Dispositivo)_parametros.IdDispositivo)
                    {
                        case Dispositivo.Placa_3:
                            {
                                dispPlacaMov = true;
                                break;
                            }
                    }
                    if (_parametros.IdDispositivo2 != 0 && !dispPlacaMov)
                    {
                        gpPlaca2.Enabled = true;
                        gpPlaca2.Visible = true;
                    }
                    else
                    {
                        gpPlaca2.Enabled = false;
                        gpPlaca2.Visible = false;
                    }
                    atualizaDataGridProdutos();

                }
                //Nivel Produtos
                else if (tbC.SelectedTab == tabNivelProdutos)
                {
                    _colorantes = Util.ObjectColorante.List();
                    for (int i = 0; i <= 15; i++)
                    {
                        if (!_colorantes[i].Habilitado)
                        {
                            _nivelCKT[i].BackColor = Cores.Parar;
                        }
                        else
                        {

                            if (_colorantes[i].Seguidor < 0)
                            {
                                _nivelCKT[i].BackColor = Cores.Seguir;
                            }
                            else if (_colorantes[i].Seguidor > 0)
                            {
                                _nivelCKT[i].BackColor = Cores.Seguidor_Tom_01;
                            }
                        }

                        _nivelMinimo[i].Enabled = _colorantes[i].Habilitado;
                        _nivelMaximo[i].Enabled = _colorantes[i].Habilitado;
                        _nivelMinimo[i].Text = _colorantes[i].NivelMinimo.ToString();
                        _nivelMaximo[i].Text = _colorantes[i].NivelMaximo.ToString();
                        _purgaVolume[i].Enabled = _colorantes[i].Habilitado;
                        _purgaVolume[i].Text = _colorantes[i].VolumePurga.ToString();

                    }

                    for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                    {
                        if (!_colorantes[i].Habilitado)
                        {
                            _nivelCKT[i].BackColor = Cores.Parar;
                        }
                        else
                        {

                            if (_colorantes[i].Seguidor < 0)
                            {
                                _nivelCKT[i].BackColor = Cores.Seguir;
                            }
                            else if (_colorantes[i].Seguidor > 0)
                            {
                                _nivelCKT[i].BackColor = Cores.Seguidor_Tom_01;
                            }
                        }

                        _nivelMinimo[i].Enabled = _colorantes[i].Habilitado;
                        _nivelMaximo[i].Enabled = _colorantes[i].Habilitado;
                        _purgaVolume[i].Enabled = _colorantes[i].Habilitado;
                        _nivelMinimo[i].Text = _colorantes[i].NivelMinimo.ToString();
                        _nivelMaximo[i].Text = _colorantes[i].NivelMaximo.ToString();
                        _purgaVolume[i].Text = _colorantes[i].VolumePurga.ToString();

                    }


                    if (_parametros.IdDispositivo2 != 0)
                    {
                        gpP2ControleNivel.Enabled = true;
                        gpP2ControleNivel.Visible = true;
                    }
                    else
                    {
                        gpP2ControleNivel.Enabled = false;
                        gpP2ControleNivel.Visible = false;
                    }
                }
                //Recirculacao
                else if (tbC.SelectedTab == tabRecirculacaoProd)
                {
                    this._listRecircular = Util.ObjectRecircular.List();
                    RessetarRecirculacao();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnPathMonitoramentoFilaDat_Click(object sender, EventArgs e)
        {
            using (fCaminhoArquivo f = new fCaminhoArquivo(txtPathMonitoramentoFilaDat.Text))
            {
                if (f.ShowDialog() == DialogResult.OK)
                    txtPathMonitoramentoFilaDat.Text = f.FilePath;
            }
        }

        private void btnTesteComunicacao_Click(object sender, EventArgs e)
        {
            string msgComunication = "";
            bool exisFirtsPorta = false;
            bool exisSecondPorta = false;
            bool existStringFirstPorta = false;
            bool connectFirstPort = false;
            bool connectSecondPort = false;
            bool opennedPort = false;
            bool opennedPort2 = false;
            try
            {
                string[] arrayPorts = SerialPort.GetPortNames();
                
                if (((ComboBoxItem)(cmbGeralDispositivo.SelectedItem)).Value == 3)
                {
                    exisFirtsPorta = true;
                    foreach(string _porta in arrayPorts)
                    {
                        if(_porta == txt_nome_Dispositivo.Text)
                        {
                            existStringFirstPorta = true;
                            break;
                        }
                    }

                    if(exisFirtsPorta && existStringFirstPorta)
                    {
                        PaintMixerInterface_P2 p2 = new PaintMixerInterface_P2(1, txt_nome_Dispositivo.Text);
                        p2.Connect(2000);
                        if (p2.mb.modbusStatus)
                        {
                            connectFirstPort = true;
                        }
                        opennedPort = p2.mb.isOpen();
                        p2.Disconnect();
                        p2 = null;
                    }
                    if (connectFirstPort)
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo1;
                        int disp2 = Convert.ToInt32(cmbGeralDispositivo_2.SelectedValue.ToString());
                        if (disp2 == 3)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }

                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P2 p2 = new PaintMixerInterface_P2(1, txt_nome_Dispositivo_2.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }

                        }
                        if (disp2 == 5)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }

                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P4 p2 = new PaintMixerInterface_P4(1, txt_nome_Dispositivo_2.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }

                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                    else
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo1 + Environment.NewLine + "Open communication with the board 1: " +
                                        (opennedPort ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                }
                else if (((ComboBoxItem)(cmbGeralDispositivo.SelectedItem)).Value == 4)
                {
                    exisFirtsPorta = true;
                    foreach (string _porta in arrayPorts)
                    {
                        if (_porta == txt_nome_Dispositivo.Text)
                        {
                            existStringFirstPorta = true;
                            break;
                        }

                    }
                    if (exisFirtsPorta && existStringFirstPorta)
                    {
                        ModBusDispenserMover_P3 p3 = new ModBusDispenserMover_P3(txt_nome_Dispositivo.Text, txt_nome_DispositivoPlMov.Text);
                        p3.Connect();
                        Thread.Sleep(200);
                        p3.Connect_Mover();                        
                        connectFirstPort = true;                        
                        if(p3.isOpenMixer() && p3.isOpenMover())
                        {
                            msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                        }
                        else
                        {
                            msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivoGeral + Environment.NewLine + "Open communication with the board ";
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(msgComunication);
                        }

                        p3.Disconnect();                        
                        p3.Disconnect_Mover();
                        p3 = null;
                    }
                }

                if (((ComboBoxItem)(cmbGeralDispositivo.SelectedItem)).Value == 5)
                {
                    exisFirtsPorta = true;
                    foreach (string _porta in arrayPorts)
                    {
                        if (_porta == txt_nome_Dispositivo.Text)
                        {
                            existStringFirstPorta = true;
                            break;
                        }

                    }
                    if (exisFirtsPorta && existStringFirstPorta)
                    {
                        PaintMixerInterface_P4 p2 = new PaintMixerInterface_P4(1, txt_nome_Dispositivo.Text);
                        p2.Connect(2000);
                        if (p2.mb.modbusStatus)
                        {
                            connectFirstPort = true;
                        }
                        opennedPort = p2.mb.isOpen();
                        p2.Disconnect();
                        p2 = null;
                    }
                    if (connectFirstPort)
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo1;
                        int disp2 = Convert.ToInt32(cmbGeralDispositivo_2.SelectedValue.ToString());
                        if (disp2 == 3)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }

                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P2 p2 = new PaintMixerInterface_P2(1, txt_nome_Dispositivo_2.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }

                        }
                        if (disp2 == 5)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }

                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P4 p2 = new PaintMixerInterface_P4(1, txt_nome_Dispositivo_2.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }

                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                    else
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo1 + Environment.NewLine + "Open communication with the board 1: " +
                                        (opennedPort ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    string msg = string.Format(Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivoGeral, ex.Message);
                    m.ShowDialog(msg);
                }
            }
        }

        private void cb_CalibragemColorante_SelectionChangeCommitted(object sender, EventArgs e)
        {

            try
            {
                
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibragemColorante.SelectedItem;
                //[Recupera calibragem para ocolorante selecionado]
                _calibragem = Util.ObjectCalibragem.Load(colorante.Circuito);
                _calibragem.Valores = _calibragem.Valores.OrderByDescending(o => o.Volume).ToList();
                lblCalibragemUltimoPulsoRevM.Text = _calibragem.UltimoPulsoReverso.ToString();

                #region [Define estado da calibragem: CORANTE SELECIONADO]

                lblCalibragemMotorM.Text = colorante.Circuito.ToString();
                lblCalibragemMassaEspecificaM.Text = colorante.MassaEspecifica.ToString();
                btnCalibragemEditarM.Enabled = true;
               
                cb_NumeroMinimoFaixaCal.SelectedItem = _calibragem.MinimoFaixas.ToString();

                if (this.User.Tipo != 2)
                {
                    btn_replicar_faixa.Enabled = true;
                    cb_NumeroMinimoFaixaCal.Enabled = true;
                }
                else
                {
                    btn_replicar_faixa.Enabled = false;
                    cb_NumeroMinimoFaixaCal.Enabled = false;
                }
                btnPrimeiraCalibracaoM.Enabled = false;
                this.isEnableEditGridView = false;
                btn_add_Valores.Enabled = false;

                _historicoValores.Clear();

                atualizaDgVCal();

                #endregion
                this.alterTabCalibracaoSave = true;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados + ex.Message);
                }
            }
        }

        private void atualizaDgVCal()
        {
            try
            {
                dgvCalibracao.ReadOnly = true;
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibragemColorante.SelectedItem;
                this.dtCal = new DataTable();
                this.dtCal.Columns.Add("Volume", typeof(string));
                this.dtCal.Columns.Add("Pulsos", typeof(string));
                this.dtCal.Columns.Add("Velocidade", typeof(string));
                this.dtCal.Columns.Add("Aceleracao", typeof(string));
                this.dtCal.Columns.Add("Delay", typeof(string));
                this.dtCal.Columns.Add("PulsosRev", typeof(string));
                this.dtCal.Columns.Add("MassaIdeal", typeof(string));
                this.dtCal.Columns.Add("MassaMedia", typeof(string));
                this.dtCal.Columns.Add("DesvioMedio", typeof(string));
                this.dtCal.Columns.Add("Historico", typeof(string));

                this.dtCal.Columns.Add("Excluir", typeof(byte[]));
              

                var imageConverter = new ImageConverter();
                var b_imag_excluir = (byte[])imageConverter.ConvertTo(Properties.IOConnect.Excluir_32x32_Escuro, typeof(byte[]));

                int i = 0;
                if (_calibragem != null)
                {
                    foreach (ValoresVO _vl in _calibragem.Valores)
                    {
                        DataRow dr = this.dtCal.NewRow();
                        dr["Volume"] = _vl.Volume.ToString();
                        dr["Pulsos"] = _vl.PulsoHorario.ToString();
                        dr["Velocidade"] = _vl.Velocidade.ToString();
                        dr["Aceleracao"] = _vl.Aceleracao.ToString();
                        dr["Delay"] = _vl.Delay.ToString();
                        dr["PulsosRev"] = _vl.PulsoReverso.ToString();

                        if (i <= 7)
                        {
                            dr["MassaIdeal"] = (colorante.MassaEspecifica * _vl.Volume).ToString("N3");
                            dr["MassaMedia"] = _vl.MassaMedia.ToString("N3");
                        }
                        else if (i <= 9)
                        {
                            dr["MassaIdeal"] = (colorante.MassaEspecifica * _vl.Volume).ToString("N4");
                            dr["MassaMedia"] = _vl.MassaMedia.ToString("N4");
                        }
                        else
                        {
                            dr["MassaIdeal"] = (colorante.MassaEspecifica * _vl.Volume).ToString("N5");
                            dr["MassaMedia"] = _vl.MassaMedia.ToString("N5");
                        }

                        dr["DesvioMedio"] = string.Format("{0:P2}", _vl.DesvioMedio);

                        dr["Historico"] = "Histórico";
                        dr["Excluir"] = b_imag_excluir;
                        this.dtCal.Rows.Add(dr);
                        i++;
                    }
                }
                
                dgvCalibracao.DataSource = this.dtCal.DefaultView;                

                dgvCalibracao.Columns[0].Width = 180;
                dgvCalibracao.Columns[0].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume; 
                dgvCalibracao.Columns[1].Width = 120;
                dgvCalibracao.Columns[1].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemPulsos;
                dgvCalibracao.Columns[2].Width = 90;
                dgvCalibracao.Columns[2].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVelocidade;
                dgvCalibracao.Columns[3].Width = 90;
                dgvCalibracao.Columns[3].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblAceleracao;
                dgvCalibracao.Columns[4].Width = 80;
                dgvCalibracao.Columns[4].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemDelay;
                dgvCalibracao.Columns[5].Width = 80;
                dgvCalibracao.Columns[5].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemPulsos;
                dgvCalibracao.Columns[6].Width = 120;
                dgvCalibracao.Columns[6].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemMassaIdeal;
                dgvCalibracao.Columns[6].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvCalibracao.Columns[7].Width = 120;
                dgvCalibracao.Columns[7].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemMassa;
                dgvCalibracao.Columns[7].DefaultCellStyle.BackColor = Color.Red;
                dgvCalibracao.Columns[7].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvCalibracao.Columns[8].Width = 120;
                dgvCalibracao.Columns[8].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemDesvio;
                dgvCalibracao.Columns[9].Width = 140;
                dgvCalibracao.Columns[9].DefaultCellStyle.BackColor = Color.Red;
                dgvCalibracao.Columns[9].HeaderText = Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico0;

                dgvCalibracao.Columns[10].Width = 40;
                dgvCalibracao.Columns[10].HeaderText = "";

                foreach (DataGridViewColumn dgvc in dgvCalibracao.Columns)
                {
                    dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			this.isUpdateCalDGV = false;
        }

        private void dgvCalibracao_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (!this.isUpdateCalDGV)
                {
                    if (this.isEnableEditGridView)
                    {
                        if (e.ColumnIndex == 7 )
                        {
                            dgvCalibracao.ReadOnly = true;
                            btnPesarM(e.RowIndex);
                        }
                        else if (e.ColumnIndex == 9)
                        {
                            dgvCalibracao.ReadOnly = true;
                            HistoricoM(e.RowIndex);
                        }
                        else if (e.ColumnIndex == 10)
                        {
                            dgvCalibracao.ReadOnly = true;

                            if (_calibragem.Valores.Count > _calibragem.MinimoFaixas)
                            {
                                bool confirma = false;
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    confirma = m.ShowDialog("Usuário deseja realmente excluir a faixa.", Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                                if (confirma)
                                {
                                    _calibragem.Valores.RemoveAt(e.RowIndex);
                                    atualizaDgVCal();
                                    PersistirCalibragem();
                                }
                            }
                            else
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    m.ShowDialog("Usuário não pode excluir a faixa, a quantidade de faixas é inferior ou igual ao limite mínimo de faixas.");
                                }
                            }
                        }
                        else
                        {
                            dgvCalibracao.ReadOnly = false;
                        }
                    }
                    else
                    {
                        dgvCalibracao.ReadOnly = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnCalibragemEditarM_Click(object sender, EventArgs e)
        {
            if (this.User.Tipo == 2)
            {
                dgvCalibracao.ReadOnly = true;
                btn_add_Valores.Enabled = false;
                btn_replicar_faixa.Enabled = false;
            }
            else
            {
                dgvCalibracao.ReadOnly = false;
                btn_add_Valores.Enabled = true;
                btn_replicar_faixa.Enabled = true;
            }

            btnPrimeiraCalibracaoM.Enabled = true;
            this.isEnableEditGridView = true;          
        }

        private void dgvCalibracao_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.isUpdateCalDGV = true; 
                bool attDGV = false;
                if (this.User.Tipo != 2)
                {
                    this.dtCal.Rows[e.RowIndex][e.ColumnIndex] = this.dgvCalibracao.CurrentCell.Value;
                    dgvCalibracao.Rows[e.RowIndex].ErrorText = "";

                    if (e.RowIndex == 0 && e.ColumnIndex == 1)
                    {
                        if (this.isEditFirstPulso)
                        {
                            attDGV = true;
                            RecalcularPulsosM(e.RowIndex);
                        }
                        this.isEditFirstPulso = true;
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        attDGV = true;
                        RecalculaVelocidade(e.RowIndex);
                    }
                }
                if(attDGV)
                {                     
                    this.Invoke(new MethodInvoker( atualizaDgVCal));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        "Falha dgvCalibracao_CellEndEdit:" + ex.Message);
                }
            }
        }

        private void RecalculaVelocidade(int indexEdicao)
        {
            _calibragem.Valores[indexEdicao].MassaMedia = 0;
            _calibragem.Valores[indexEdicao].DesvioMedio = 0;
           
            //Ao Preencher automatimamente calcular Aceleraçã
            int valorVelocidade = 0;
            if (int.TryParse(this.dtCal.Rows[indexEdicao]["Velocidade"].ToString(), out valorVelocidade))
            {
                _calibragem.Valores[indexEdicao].Velocidade = valorVelocidade;
                Util.ObjectParametros prametros = Util.ObjectParametros.Load();
                double AG = prametros.Aceleracao;
                double VG = prametros.Velocidade;
                double V = valorVelocidade;

                double FATOR = (VG / V) * (VG / V);
                double AC = (1 / FATOR) * VG;
                int newAceleracao = int.Parse(Math.Round(AC).ToString());

                _calibragem.Valores[indexEdicao].Aceleracao = newAceleracao;
            }
            else
            {
                _calibragem.Valores[indexEdicao].Aceleracao = 0;
            }
        }

        private void dgvCalibracao_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (!this.isUpdateCalDGV)
                {
                    if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5)
                    {
                        int _i = 0;
                        if (!int.TryParse(e.FormattedValue.ToString(), out _i))
                        {
                            dgvCalibracao.Rows[e.RowIndex].ErrorText = "Erro campo inválido!";
                            e.Cancel = true;
                        }
                    }
                    else if (e.ColumnIndex == 6 || e.ColumnIndex == 7)
                    {
                        double _d = 0;
                        if (!double.TryParse(e.FormattedValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out _d))
                        {
                            dgvCalibracao.Rows[e.RowIndex].ErrorText = "Erro campo inválido!";
                            e.Cancel = true;
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        "Falha dgvCalibracao_CellValidating:" + ex.Message);
                }
            }
        }

        private void btn_add_Valores_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectCalibragem _cal = new Util.ObjectCalibragem();
                _cal.Motor = _calibragem.Motor;
                _cal.UltimoPulsoReverso = _calibragem.UltimoPulsoReverso;
                _cal.Valores = new List<ValoresVO>();
                _cal.Valores = _calibragem.Valores.ToList();
                fCalibracao fcal = new fCalibracao(_cal);
                if(fcal.ShowDialog() == DialogResult.OK)
                {
                    ValoresVO vo = fcal.valores;
                    _calibragem.Valores.Add(vo);
                    _calibragem.Valores = _calibragem.Valores.OrderByDescending(o => o.Volume).ToList();
                    atualizaDgVCal();
                    PersistirCalibragem();
                   
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados + ex.Message);
                }
            }
        }

        private void btnPrimeiraCalibracaoM_Click(object sender, EventArgs e)
        {
            ValoresVO valores = null;
            Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibragemColorante.SelectedItem;
            _calibragem = Util.ObjectCalibragem.Load(colorante.Circuito);
            _calibragem.Valores = _calibragem.Valores.OrderByDescending(o => o.Volume).ToList();
            bool RedefinirPulsos = false;
            using (fPrimeiraPesagem f = new fPrimeiraPesagem(_calibragem.Motor, colorante.Circuito))
            {
                if (f.ShowDialog() != DialogResult.OK)
                {
                    lblCalibragemUltimoPulsoRevM.Text = _calibragem.UltimoPulsoReverso.ToString();
                    return;
                }

                RedefinirPulsos = f.RedefinirPulsos;
                valores = f._valores;
                valores.MassaMedia = f.MassaMedia;
                valores.DesvioMedio = f.DesvioMedio;
            }

            if (RedefinirPulsos && valores != null)
            {
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(Wait_Message));
                }
                
                double desvio =
                Operar.CalcularDesvio(
                    valorMedio: valores.MassaMedia,
                    massaIdeal: valores.MassaIdeal);

                double pulsoRecalculado =
                    valores.PulsoHorario - (desvio * valores.PulsoHorario);
                int posicaoEditada = 0;
                double vl_dat = double.Parse(this.dtCal.Rows[posicaoEditada]["Volume"].ToString());

                double pulso_por_volume_100_ml = (pulsoRecalculado / valores.Volume) * vl_dat;
                this.isEditFirstPulso = true;
                this.dtCal.Rows[posicaoEditada]["Pulsos"] = (int)Math.Round(pulso_por_volume_100_ml) + "";
                
                RecalcularPulsosM(posicaoEditada);
                if (_parametros.ViewMessageProc)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                    Application.DoEvents();
                }
                atualizaDgVCal();
                if (_parametros.ViewMessageProc)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                    Application.DoEvents();
                }
                
                PersistirCalibragem(false);
                if (_parametros.ViewMessageProc)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                    Application.DoEvents();
                }
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
        }

        void RecalcularPulsosM(int indexInicio)
        {
            /* Pulsos da posição inicial a ser utilizado para recalcular demais posições */
            double pulsosFaixaInicial = double.Parse(this.dtCal.Rows[indexInicio]["Pulsos"].ToString()); 
            _calibragem.Valores[indexInicio].PulsoHorario = (int)Math.Round(pulsosFaixaInicial);

            /* Zera valores de massa e desvio da posição inicial */
            _calibragem.Valores[indexInicio].MassaMedia = 0;
            _calibragem.Valores[indexInicio].DesvioMedio = 0;
          

            for (int indexAtual = indexInicio + 1; indexAtual < _calibragem.Valores.Count; indexAtual++)
            {
                /* Recalcula pulsos das faixas abaixo da posição inicial */
                double pulsoRecalculado =
                    (_calibragem.Valores[indexAtual].Volume * pulsosFaixaInicial) / _calibragem.Valores[indexInicio].Volume;

                /* Atualiza valores recalculados na tela */
                _calibragem.Valores[indexAtual].PulsoHorario = (int)Math.Round(pulsoRecalculado);
                _calibragem.Valores[indexAtual].MassaMedia = 0;
                _calibragem.Valores[indexAtual].DesvioMedio = 0;
            }
        }

        void RecalcularPulsosFaixaM(int indexPosicao)
        {
            /* Pulsos da posição inicial a ser utilizado para recalcular demais posições */
            double pulsosFaixaInicial = double.Parse(this.dtCal.Rows[indexPosicao]["Pulsos"].ToString());
            _calibragem.Valores[indexPosicao].PulsoHorario = (int)Math.Round(pulsosFaixaInicial);

            /* Zera valores de massa e desvio da posição inicial */
            _calibragem.Valores[indexPosicao].MassaMedia = 0;
            _calibragem.Valores[indexPosicao].DesvioMedio = 0;
            
            /* Recalcula pulsos das faixas abaixo da posição inicial */
            double pulsoRecalculado =
                (_calibragem.Valores[indexPosicao].Volume * pulsosFaixaInicial) / _calibragem.Valores[indexPosicao].Volume;

            /* Atualiza valores recalculados na tela */
            _calibragem.Valores[indexPosicao].PulsoHorario = (int)Math.Round(pulsoRecalculado);
            _calibragem.Valores[indexPosicao].MassaMedia = 0;
            _calibragem.Valores[indexPosicao].DesvioMedio = 0;
        }

        private void btnPesarM(int posicaoEditada)
        {
            try
            {
                int pulsos = 0;
                int pulsosR = 0;
                int velocidade = 0;
                int delay = 0;
                int aceleracao = 0;
                
                #region Valida entradas
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibragemColorante.SelectedItem;

                int.TryParse(this.dtCal.Rows[posicaoEditada]["Pulsos"].ToString(), out pulsos);

                if (pulsos == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformePulsos);
                    }
                    
                    return;
                }

                int.TryParse(this.dtCal.Rows[posicaoEditada]["Velocidade"].ToString(), out velocidade);
                if (velocidade == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformeVelocidade);
                    }

                    return;
                }

                int.TryParse(this.dtCal.Rows[posicaoEditada]["Delay"].ToString(), out delay);
                if (delay == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformeDelay);
                    }

                    return;
                }

                int.TryParse(this.dtCal.Rows[posicaoEditada]["PulsosRev"].ToString(), out pulsosR);

                if (pulsosR == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformePulsosRev);
                    }

                    return;
                }

                int.TryParse(this.dtCal.Rows[posicaoEditada]["Aceleracao"].ToString(), out aceleracao);
                if (aceleracao == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformeAceleracao);
                    }

                    return;
                }

                #endregion

                //Valores de entradas do usuário
                ValoresVO valores = new ValoresVO()
                {
                    Volume = double.Parse(this.dtCal.Rows[posicaoEditada]["Volume"].ToString()),
                    PulsoHorario = pulsos,
                    Velocidade = velocidade,
                    Delay = delay,
                    MassaIdeal = double.Parse(this.dtCal.Rows[posicaoEditada]["MassaIdeal"].ToString()),
                    PulsoReverso = pulsosR,
                    Aceleracao = aceleracao
                };

                bool RedefinirPulsos = false;
                bool RedefinirPulsoFaixa = false;                

                using (fPesagem f = new fPesagem(_calibragem.Motor, valores))
                {
                    f.PosicaoEditada = posicaoEditada;
                    if (f.ShowDialog() != DialogResult.OK)
                    {
                        _calibragem = Util.ObjectCalibragem.Load(colorante.Circuito);
                        _calibragem.Valores = _calibragem.Valores.OrderByDescending(o => o.Volume).ToList();
                        lblCalibragemUltimoPulsoRevM.Text = _calibragem.UltimoPulsoReverso.ToString();
                        if (_parametros.TreinamentoCal)
                        {
                           
                            _parametros.TreinamentoCal = false;
                            PersistirParametros(false);
                            Util.ObjectParametros.InitLoad();
                        }
                        return;
                    }

                    RedefinirPulsos = f.RedefinirPulsos;
                    RedefinirPulsoFaixa = f.RedefinirpulsoFaixa;

                    valores.MassaMedia = f.MassaMedia;
                    valores.DesvioMedio = f.DesvioMedio;
                    
                }
                if(_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(Wait_Message));
                }

                if (RedefinirPulsos)
                {
                    double desvio =
                        Operar.CalcularDesvio(
                            valorMedio: valores.MassaMedia,
                            massaIdeal: valores.MassaIdeal);

                    double pulsoRecalculado = valores.PulsoHorario - ((int)(desvio * valores.PulsoHorario));                    

                    //Atualiza objeto de valores de entrada para que possa ser passado 
                    //* para o método que registra valores de histórico 
                    if (pulsoRecalculado <= 0)
                    {
                        if (pulsos > 0)
                        {
                            pulsoRecalculado = pulsos / 2;
                        }
                        else
                        {
                            pulsoRecalculado = 1000;
                        }
                    }

                    valores.PulsoHorario = (int)Math.Round(pulsoRecalculado);

                    this.dtCal.Rows[posicaoEditada]["Pulsos"] = (int)Math.Round(pulsoRecalculado) + "";
                    this.isEditFirstPulso = true;
                    
                    RecalcularPulsosM(posicaoEditada);
                }

                //Redefinir somente a faixa de dosagem selecionada
                else if (RedefinirPulsoFaixa)
                {
                    double desvio =
                        Operar.CalcularDesvio(
                            valorMedio: valores.MassaMedia,
                            massaIdeal: valores.MassaIdeal);

                    double pulsoRecalculado =
                        valores.PulsoHorario - ((int)(desvio * valores.PulsoHorario));

                    if (pulsoRecalculado <= 0)
                    {
                        if (pulsos > 0)
                        {
                            pulsoRecalculado = pulsos / 2;
                        }
                        else
                        {
                            pulsoRecalculado = 1000;
                        }
                    }

                    //Atualiza objeto de valores de entrada para que possa ser passado 
                    // * para o método que registra valores de histórico 
                    valores.PulsoHorario = (int)Math.Round(pulsoRecalculado);

                    this.dtCal.Rows[posicaoEditada]["Pulsos"] = (int)Math.Round(pulsoRecalculado) + "";
                    this.isEditFirstPulso = false;
                    RecalcularPulsosFaixaM(posicaoEditada);
                }
                
                _calibragem.Valores[posicaoEditada].MassaMedia = valores.MassaMedia;
                _calibragem.Valores[posicaoEditada].DesvioMedio = valores.DesvioMedio;


                //Insere valores no histórico
                _historicoValores.Add(valores);

                lblCalibragemUltimoPulsoRevM.Text = _calibragem.UltimoPulsoReverso.ToString();
                if (_parametros.ViewMessageProc)
                {                   
                    Application.DoEvents();
                    Thread.Sleep(200);
                    Application.DoEvents();
                }
                if (_parametros.TreinamentoCal)
                {
                    
                    _parametros.TreinamentoCal = false;
                    PersistirParametros(false);
                    Util.ObjectParametros.InitLoad();
                }
                atualizaDgVCal();
                if (_parametros.ViewMessageProc)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                    Application.DoEvents();
                }
                PersistirCalibragem(false);
                Util.ObjectParametros.InitLoad();
                if (_parametros.ViewMessageProc)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                    Application.DoEvents();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        "Falha ao Pesar:" + ex.Message);
                }
            }

            if (_parametros.ViewMessageProc)
            {
                this.Invoke(new MethodInvoker(ClosePrg));
            }
        }

        private void Wait_Message()
        {
            try
            {
                if (this._fAguarde == null)
                {
                    this._fAguarde = new fAguarde(Negocio.IdiomaResxExtensao.Global_Aguarde_ProgressBar, Color.FromArgb(6, 206, 37));
                    this._fAguarde.OnClosedEvent += new CloseWindows(ClosedProgressBar);
                    this._fAguarde.Show();
                    this._fAguarde.ExecutarMonitoramento();
                    this._fAguarde._TimerDelay = 330;
                    Application.DoEvents();
                }
                else
                {
                    this._fAguarde._TimerDelay = 330;
                    this._fAguarde.Focus();
                }

                Thread.Sleep(1500);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void HistoricoM(int indexEdicao)
        {
            try
            { 
                //Índíce selecionado
           
                //Verifica se existe algum registro no histórico
                if ((_historicoValores == null) || _historicoValores.Count == 0)
                    return;

                //Volume em edição
                double volume = double.Parse( this.dtCal.Rows[indexEdicao]["Volume"].ToString());

                //Seleciona valores no histórico os valores referentes ao volume em edição
                List<ValoresVO> lista = _historicoValores.Where(p => p.Volume == volume).ToList();

                if (lista.Count == 0)
                    return;

                fHistoricoValores f = new fHistoricoValores(lista);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    //Popula controles com valores do histórico
                    this._calibragem.Valores[indexEdicao].PulsoHorario = f.Valores.PulsoHorario;
                    this._calibragem.Valores[indexEdicao].PulsoReverso = f.Valores.PulsoReverso;
                    this._calibragem.Valores[indexEdicao].Velocidade = f.Valores.Velocidade;
                    this._calibragem.Valores[indexEdicao].Delay = f.Valores.Delay;
                    this._calibragem.Valores[indexEdicao].MassaIdeal = f.Valores.MassaIdeal;
                    this._calibragem.Valores[indexEdicao].MassaMedia = f.Valores.MassaMedia;
                    this._calibragem.Valores[indexEdicao].DesvioMedio = f.Valores.DesvioMedio;
                }

                f.Dispose();
                atualizaDgVCal();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        "Falha Historico:" + ex.Message);
                }
            }
        }

        private void cb_CalibracaoAuto_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibracaoAuto.SelectedItem;
                //[Recupera calibragem para ocolorante selecionado]
                _calibracaoAuto = Util.ObjectCalibracaoAutomatica.Load(colorante.Circuito);
                _calibracaoAuto._colorante = Util.ObjectColorante.Load(colorante.Circuito);
                _calibracaoAuto.listOperacaoAutomatica = _calibracaoAuto.listOperacaoAutomatica.OrderByDescending(o => o.IsPrimeiraCalibracao).ThenByDescending(o => o.Volume).ToList();

                #region [Define estado da calibragem: CORANTE SELECIONADO]

                lblCalibracaoAutoMotor.Text = _calibracaoAuto._colorante.Circuito.ToString();
                lblCalibracaoAutoMassaEspecifica.Text = _calibracaoAuto._colorante.MassaEspecifica.ToString();

                txtCapacidadeMaxBalanca.Text = _calibracaoAuto.CapacideMaxBalanca.ToString();
                txtMassaAdmRecipiente.Text = _calibracaoAuto.MaxMassaAdmRecipiente.ToString();
                txtMinMassaAdmRecipiente.Text = _calibracaoAuto.MinMassaAdmRecipiente.ToString();
                txtVolumeMaxRecipiente.Text = _calibracaoAuto.VolumeMaxRecipiente.ToString();
                txtTentativasRecipiente.Text = _calibracaoAuto.NumeroMaxTentativaRec.ToString();

                txtMinMassaAdmRecipiente.Text = _calibracaoAuto.MinMassaAdmRecipiente.ToString();
                btnCalibracaoAutoEditar.Enabled = true;
               
                dgvCalibracaoAuto.Enabled = false;
                atualizaDgvAutomatico();


                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados + ex.Message);
                }
            }
        }

        private void atualizaDgvAutomatico()
        {
            try
            {
                this.dtCalAuto = new DataTable();
                this.dtCalAuto.Columns.Add("IsPrimeiraCalibracao", typeof(int));
                this.dtCalAuto.Columns.Add("Motor", typeof(string));
                this.dtCalAuto.Columns.Add("Volume", typeof(string));
                this.dtCalAuto.Columns.Add("DesvioAdmissivel", typeof(string));
                this.dtCalAuto.Columns.Add("IsCalibracaoAutomatica", typeof(int));
                this.dtCalAuto.Columns.Add("NumeroMaxTentativa", typeof(string));
                this.dtCalAuto.Columns.Add("IsRealizarMediaMedicao", typeof(int));
                this.dtCalAuto.Columns.Add("NumeroDosagemTomadaMedia", typeof(string));
                
                if (_calibracaoAuto != null)
                {
                    
                    foreach (Negocio.OperacaoAutomatica _op in _calibracaoAuto.listOperacaoAutomatica)
                    {
                        DataRow dr = this.dtCalAuto.NewRow();
                        dr["IsPrimeiraCalibracao"] = _op.IsPrimeiraCalibracao;
                        dr["Motor"] = _op.Motor.ToString();
                        dr["Volume"] = _op.Volume.ToString();
                        dr["DesvioAdmissivel"] = _op.DesvioAdmissivel.ToString();
                        dr["IsCalibracaoAutomatica"] = _op.IsCalibracaoAutomatica;
                        dr["NumeroMaxTentativa"] = _op.NumeroMaxTentativa.ToString();
                        dr["IsRealizarMediaMedicao"] = _op.IsRealizarMediaMedicao;
                        dr["NumeroDosagemTomadaMedia"] = _op.NumeroDosagemTomadaMedia.ToString();
                       
                        this.dtCalAuto.Rows.Add(dr);
                       
                    }
                }
                if (dgvCalibracaoAuto.Rows != null)
                {
                    dgvCalibracaoAuto.Rows.Clear();
                }
                for (int i =0; i < this.dtCalAuto.Rows.Count; i++ )
                {
                    dgvCalibracaoAuto.Rows.Add(this.dtCalAuto.Rows[i][0], this.dtCalAuto.Rows[i][1], this.dtCalAuto.Rows[i][2], this.dtCalAuto.Rows[i][3], this.dtCalAuto.Rows[i][4], 
                        this.dtCalAuto.Rows[i][5], this.dtCalAuto.Rows[i][6], this.dtCalAuto.Rows[i][7]);
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("IsPrimeiraCalibracao", typeof(int));
                dt.Columns.Add("Descricao", typeof(string));
                dt.Rows.Add(0, Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoFaixasCal);
                dt.Rows.Add(1, Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPrimeiraCal);

                DataGridViewComboBoxColumn combOp = dgvCalibracaoAuto.Columns[0] as DataGridViewComboBoxColumn;
                combOp.DataSource = dt;
                combOp.DisplayMember = "Descricao";
                combOp.ValueMember = "IsPrimeiraCalibracao";
               
                DataTable _dt_aut = new DataTable();
                _dt_aut.Columns.Add("IsCalibracaoAutomatica", typeof(int));
                _dt_aut.Columns.Add("Descricao", typeof(string));
                _dt_aut.Rows.Add(0, Negocio.IdiomaResxExtensao.Global_Nao ); 
                _dt_aut.Rows.Add(1, Negocio.IdiomaResxExtensao.Global_Sim); 

                DataGridViewComboBoxColumn combAut = dgvCalibracaoAuto.Columns[4] as DataGridViewComboBoxColumn;
                combAut.DataSource = _dt_aut;
                combAut.DisplayMember = "Descricao";
                combAut.ValueMember = "IsCalibracaoAutomatica";
               
                DataTable _dt_m_medicao = new DataTable();
                _dt_m_medicao.Columns.Add("IsRealizarMediaMedicao", typeof(int));
                _dt_m_medicao.Columns.Add("Descricao", typeof(string));
                _dt_m_medicao.Rows.Add(0, Negocio.IdiomaResxExtensao.Global_Nao);
                _dt_m_medicao.Rows.Add(1, Negocio.IdiomaResxExtensao.Global_Sim);

                DataGridViewComboBoxColumn combm_medicao = dgvCalibracaoAuto.Columns[6] as DataGridViewComboBoxColumn;
                combm_medicao.DataSource = _dt_m_medicao;
                combm_medicao.DisplayMember = "Descricao";
                combm_medicao.ValueMember = "IsRealizarMediaMedicao";
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(ex.Message);
                }
            }
        }

        private void dgvCalibracaoAuto_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5 || e.ColumnIndex == 7 )
                {
                    int _i = 0;
                    if (!int.TryParse(e.FormattedValue.ToString(), out _i))
                    {
                        dgvCalibracao.Rows[e.RowIndex].ErrorText = "Erro campo inválido!";
                        e.Cancel = true;
                    }
                }
                else if (e.ColumnIndex == 3)
                {
                    double _d = 0;
                    if (!double.TryParse(e.FormattedValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out _d))
                    {
                        dgvCalibracao.Rows[e.RowIndex].ErrorText = "Erro campo inválido!";
                        e.Cancel = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        "Falha dgvCalibracaoAuto_CellValidating:" + ex.Message);
                }
            }
        }

        private void dgvCalibracaoAuto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    if (e.RowIndex == 0)
                    {
                        dgvCalibracaoAuto.Columns[e.ColumnIndex].ReadOnly = false;
                    }
                    else
                    {
                        dgvCalibracaoAuto.Columns[e.ColumnIndex].ReadOnly = true;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgvCalibracaoAuto_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                ComboBox combo = e.Control as ComboBox;
                if (combo != null)
                {
                    combo.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
                    combo.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int row = dgvCalibracaoAuto.CurrentCell.RowIndex;
                int column = dgvCalibracaoAuto.CurrentCell.ColumnIndex;

                ComboBox cb = (ComboBox)sender;
                this.dtCalAuto.Rows[row][column] = Convert.ToInt32(cb.SelectedValue.ToString());
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgvCalibracaoAuto_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.dtCalAuto.Rows[e.RowIndex][e.ColumnIndex] = this.dgvCalibracaoAuto.CurrentCell.Value;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnCalibracaoAutoEditar_Click(object sender, EventArgs e)
        {
            if(this.User.Tipo == 2)
            {
                dgvCalibracaoAuto.ReadOnly = true;
            }
            dgvCalibracaoAuto.Enabled = true;
            btnIniciarCalAutomatica.Enabled = true;
        }

        private void btn_replicar_faixa_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectColorante colorante = (Util.ObjectColorante)cb_CalibragemColorante.SelectedItem;
                fSelColorante fSel = new fSelColorante(colorante);
                if(fSel.ShowDialog() == DialogResult.OK)
                {
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(Wait_Message));
                    }
                    List<ValoresVO> v_cal = _calibragem.Valores.ToList();

                    for (int i = 0; i < fSel.listCircuitos.Count; i++)
                    {
                        Util.ObjectCalibragem.Delete(fSel.listCircuitos[i]);

                        Util.ObjectCalibragem _c = new Util.ObjectCalibragem();
                        _c.Motor = fSel.listCircuitos[i];
                        _c.UltimoPulsoReverso = _calibragem.UltimoPulsoReverso;
                        _c.Valores = v_cal.ToList();
                        Util.ObjectCalibragem.Add(_c);

                        Util.ObjectCalibracaoAutomatica _cAuto = Util.ObjectCalibracaoAutomatica.Load(_calibragem.Motor);
                        _cAuto._calibragem = _c;
                        Util.ObjectCalibracaoAutomatica.Add(_cAuto, false);
                        if ((i % 4) == 0)
                        {
                            if (_parametros.ViewMessageProc)
                            {
                                Application.DoEvents();
                                Thread.Sleep(200);
                                Application.DoEvents();
                            }
                        }
                    }
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso);
                    }
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(ClosePrg));
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message);
                }
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
        }

        private void btnIniciarCalAutomatica_Click(object sender, EventArgs e)
        {

            if (Operar.TemPurgaPendente())
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_PurgaPendente);
                }
            }
            else
            {
               
                bool confirma = true;
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.CalibracaoAuto_NovaCalibracao, Negocio.IdiomaResxExtensao.CalibracaoAuto_BotaoNovaCalibracao, Negocio.IdiomaResxExtensao.CalibracaoAuto_BotaoContinueCalibracao);
                }
                this.confirmaCalNew = confirma;
                if (this._fAguarde == null)
                {
                    this._fAguarde = new fAguarde(Negocio.IdiomaResxExtensao.Global_Aguarde_ProgressBar, Color.FromArgb(6, 206, 37));
                    this._fAguarde.OnClosedEvent += new CloseWindows(ClosedProgressBar);
                    this._fAguarde.Show();
                    this._fAguarde.ExecutarMonitoramento();
                    this._fAguarde._TimerDelay = 330;

                }
                else
                {
                    this._fAguarde._TimerDelay = 330;
                    this._fAguarde.Focus();
                }

                try
                {
                    Thread thrd = new Thread(new ThreadStart(initPRGBar));
                    thrd.Start();
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}
			}
        }

        private void initPRGBar()
        {
            try
            {
                while (!this._fAguarde.IsRunning)
                {
                    Thread.Sleep(1000);
                }
                this.Invoke(new MethodInvoker(IniciarCaulAutomatica));

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
                }
            }
        }

        private void IniciarCaulAutomatica()
        {
            try
            {
                List<Util.ObjectCalibracaoHistorico> listHistorico = new List<Util.ObjectCalibracaoHistorico>();
                List<Util.ObjectColorante> lCo = _colorantes.Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();
                if (this.confirmaCalNew)
                {
                    foreach (Util.ObjectColorante co in lCo)
                    {
                        Util.ObjectCalibracaoHistorico.DeleteFaixa(co.Circuito);
                        Thread.Sleep(10);
                    }
                    bool att = true;
                    foreach (Util.ObjectColorante co in lCo)
                    {
                        Util.ObjectCalibracaoAutomatica calAuto = Util.ObjectCalibracaoAutomatica.Load(co.Circuito);
                        calAuto._colorante = co;
                        calAuto.listOperacaoAutomatica = calAuto.listOperacaoAutomatica.OrderByDescending(o => o.IsPrimeiraCalibracao).ThenByDescending(o => o.Volume).ToList();
                        Util.ObjectCalibracaoHistorico calHist = new Util.ObjectCalibracaoHistorico();
                        calHist._calibracaoAuto = calAuto;
                        calHist.CapacideMaxBalanca = calAuto.CapacideMaxBalanca;
                        calHist.MaxMassaAdmRecipiente = calAuto.MaxMassaAdmRecipiente;
                        calHist.MinMassaAdmRecipiente = calAuto.MinMassaAdmRecipiente;
                        calHist.NumeroMaxTentativaRec = calAuto.NumeroMaxTentativaRec;
                        calHist.VolumeMaxRecipiente = calAuto.VolumeMaxRecipiente;
                        calHist.listOperacaoAutoHist = new List<Negocio.OperacaoAutoHist>();
                        for (int i = 0; i < calHist.NumeroMaxTentativaRec; i++)
                        {
                            Negocio.OperacaoAutoHist opH = new Negocio.OperacaoAutoHist();
                            opH.Motor = co.Circuito;
                            opH.Volume = 0;
                            opH.Volume_str = opH.Volume.ToString().Replace(",", ".");
                            opH.Aprovado = 0;
                            opH.Desvio = 0;
                            opH.Desvio_str = "n/a";
                            opH.Etapa = 0;
                            opH.Etapa_Tentativa = i + 1;
                            opH.Executado = 0;
                            opH.MassaIdeal = 0;
                            opH.MassaIdeal_str = "n/a";
                            opH.MassaMedBalanca = 0;
                            opH.MassaMedBalanca_str = "0";
                            opH.VolumeDosado = 0;
                            opH.VolumeDosado_str = "0";
                            calHist.listOperacaoAutoHist.Add(opH);
                        }
                        foreach (Negocio.OperacaoAutomatica opAuto in calHist._calibracaoAuto.listOperacaoAutomatica)
                        {
                            if (opAuto.IsPrimeiraCalibracao == 1)
                            {
                                for (int i = 0; i < opAuto.NumeroMaxTentativa; i++)
                                {
                                    Negocio.OperacaoAutoHist opH = new Negocio.OperacaoAutoHist();
                                    opH.Motor = co.Circuito;
                                    opH.Volume = opAuto.Volume;
                                    opH.Volume_str = opH.Volume.ToString().Replace(",", ".");
                                    opH.Aprovado = 0;
                                    opH.Desvio = opAuto.DesvioAdmissivel;
                                    opH.Desvio_str = opH.Desvio.ToString().Replace(",", ".");
                                    opH.Etapa = 1;
                                    opH.Etapa_Tentativa = i + 1;
                                    opH.Executado = 0;
                                    opH.MassaIdeal = co.MassaEspecifica * opH.Volume;
                                    opH.MassaIdeal_str = opH.MassaIdeal.ToString().Replace(",", ".");
                                    opH.MassaMedBalanca = 0;
                                    opH.MassaMedBalanca_str = "0";
                                    opH.VolumeDosado = 0;
                                    opH.VolumeDosado_str = "0";
                                    calHist.listOperacaoAutoHist.Add(opH);
                                }
                            }
                            else
                            {
                                if (opAuto.IsCalibracaoAutomatica == 1)
                                {
                                    for (int i = 0; i < opAuto.NumeroMaxTentativa; i++)
                                    {
                                        Negocio.OperacaoAutoHist opH = new Negocio.OperacaoAutoHist();
                                        opH.Motor = co.Circuito;
                                        opH.Volume = opAuto.Volume;
                                        opH.Volume_str = opH.Volume.ToString().Replace(",", ".");
                                        opH.Aprovado = 0;
                                        opH.Desvio = opAuto.DesvioAdmissivel;
                                        opH.Desvio_str = opH.Desvio.ToString().Replace(",", ".");
                                        opH.Etapa = 2;
                                        opH.Etapa_Tentativa = i + 1;
                                        opH.Executado = 0;
                                        opH.MassaIdeal = co.MassaEspecifica * opH.Volume;
                                        opH.MassaIdeal_str = opH.MassaIdeal.ToString().Replace(",", ".");
                                        opH.MassaMedBalanca = 0;
                                        opH.MassaMedBalanca_str = "0";
                                        opH.VolumeDosado = 0;
                                        opH.VolumeDosado_str = "0";
                                        calHist.listOperacaoAutoHist.Add(opH);
                                    }
                                }
                            }
                        }

                        listHistorico.Add(calHist);
                        Thread.Sleep(10);
                    }
                    foreach (Util.ObjectCalibracaoHistorico cHist in listHistorico)
                    {
                        Util.ObjectCalibracaoHistorico.Add(cHist, att);
                        Thread.Sleep(10);
                        att = false;
                    }
                }
                else
                {
                    foreach (Util.ObjectColorante co in lCo)
                    {
                        Util.ObjectCalibracaoHistorico calHist = Util.ObjectCalibracaoHistorico.Load(co.Circuito);
                        if (calHist == null || calHist.listOperacaoAutoHist == null || calHist.listOperacaoAutoHist.Count == 0)
                        {
                            Util.ObjectCalibracaoAutomatica calAuto = Util.ObjectCalibracaoAutomatica.Load(co.Circuito);
                            calAuto._colorante = co;
                            calAuto.listOperacaoAutomatica = calAuto.listOperacaoAutomatica.OrderByDescending(o => o.IsPrimeiraCalibracao).ThenByDescending(o => o.Volume).ToList();
                            if (calHist == null)
                            {
                                calHist = new Util.ObjectCalibracaoHistorico();
                            }

                            calHist._calibracaoAuto = calAuto;
                            calHist.CapacideMaxBalanca = calHist.CapacideMaxBalanca;
                            calHist.MaxMassaAdmRecipiente = calHist.MaxMassaAdmRecipiente;
                            calHist.MinMassaAdmRecipiente = calHist.MinMassaAdmRecipiente;
                            calHist.NumeroMaxTentativaRec = calHist.NumeroMaxTentativaRec;
                            calHist.VolumeMaxRecipiente = calHist.VolumeMaxRecipiente;
                            for (int i = 0; i < calHist.NumeroMaxTentativaRec; i++)
                            {
                                Negocio.OperacaoAutoHist opH = new Negocio.OperacaoAutoHist();
                                opH.Motor = co.Circuito;
                                opH.Volume = 0;
                                opH.Volume_str = opH.Volume.ToString().Replace(",", ".");
                                opH.Aprovado = 0;
                                opH.Desvio = 0;
                                opH.Desvio_str = "n/a";
                                opH.Etapa = 0;
                                opH.Etapa_Tentativa = i + 1;
                                opH.Executado = 0;
                                opH.MassaIdeal = 0;
                                opH.MassaIdeal_str = "n/a";
                                opH.MassaMedBalanca = 0;
                                opH.MassaMedBalanca_str = "0";
                                opH.VolumeDosado = 0;
                                opH.VolumeDosado_str = "0";
                                calHist.listOperacaoAutoHist.Add(opH);
                            }
                            if (calHist.listOperacaoAutoHist == null || calHist.listOperacaoAutoHist.Count == 0)
                            {
                                calHist.listOperacaoAutoHist = new List<Negocio.OperacaoAutoHist>();
                                foreach (Negocio.OperacaoAutomatica opAuto in calHist._calibracaoAuto.listOperacaoAutomatica)
                                {
                                    if (opAuto.IsPrimeiraCalibracao == 1)
                                    {
                                        for (int i = 0; i < opAuto.NumeroMaxTentativa; i++)
                                        {
                                            Negocio.OperacaoAutoHist opH = new Negocio.OperacaoAutoHist();
                                            opH.Motor = co.Circuito;
                                            opH.Volume = 0;
                                            opH.Volume_str = "0";
                                            opH.Aprovado = 0;
                                            opH.Desvio = 0;
                                            opH.Desvio_str = "n/a";
                                            opH.Etapa = 1;
                                            opH.Etapa_Tentativa = i + 1;
                                            opH.Executado = 0;
                                            opH.MassaIdeal = 0;
                                            opH.MassaIdeal_str = "n/a";
                                            opH.MassaMedBalanca = 0;
                                            opH.MassaMedBalanca_str = "0";
                                            opH.VolumeDosado = 0;
                                            opH.VolumeDosado_str = "0";
                                            calHist.listOperacaoAutoHist.Add(opH);
                                        }
                                    }
                                    else
                                    {
                                        if (opAuto.IsCalibracaoAutomatica == 1)
                                        {
                                            for (int i = 0; i < opAuto.NumeroMaxTentativa; i++)
                                            {
                                                Negocio.OperacaoAutoHist opH = new Negocio.OperacaoAutoHist();
                                                opH.Motor = co.Circuito;
                                                opH.Volume = opAuto.Volume;
                                                opH.Volume_str = opH.Volume.ToString().Replace(",", ".");
                                                opH.Aprovado = 0;
                                                opH.Desvio = opAuto.DesvioAdmissivel;
                                                opH.Desvio_str = opH.Desvio.ToString().Replace(",", ".");
                                                opH.Etapa = 2;
                                                opH.Etapa_Tentativa = i + 1;
                                                opH.Executado = 0;
                                                opH.MassaIdeal = co.MassaEspecifica * opH.Volume;
                                                opH.MassaIdeal_str = opH.MassaIdeal.ToString().Replace(",", ".");
                                                opH.MassaMedBalanca = 0;
                                                opH.MassaMedBalanca_str = "0";
                                                opH.VolumeDosado = 0;
                                                opH.VolumeDosado_str = "0";
                                                calHist.listOperacaoAutoHist.Add(opH);
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        listHistorico.Add(calHist);
                        Thread.Sleep(10);
                    }
                }
                this.Invoke(new MethodInvoker(ClosePrg));
                this.fCalAuto = new fCalibracaoAutomatica(listHistorico);
                this.fCalAuto.OnClosedEvent += new CloseWindows(fCalibracaoAutomaticaClosed);
                this.fCalAuto.ShowDialog();

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                this.Invoke(new MethodInvoker(ClosePrg));
            }
        }

        private void ClosePrg()
        {
            try
            {
                if (this._fAguarde != null)
                {
                    this._fAguarde.PausarMonitoramento();
                    this._fAguarde.Close();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ClosedProgressBar()
        {
            try
            {
                this._fAguarde = null;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fCalibracaoAutomaticaClosed()
        {
            try
            {
                this.fCalAuto = null;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnClearUsuarios_Click(object sender, EventArgs e)
        {
            try
            {
                txtNomeUsuario.Text = "";
                txtSenhaUsuario.Text = "";
                cbTipoUsuario.SelectedIndex = 0;
                AtualizaGridUsuario();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
    
        private void AtualizaGridUsuario()
        {
            try
            {
                this.listUsuarios = Util.ObjectUser.List();

                if(this.User.Tipo != 1)
                {
                    for(int i = 0; i < this.listUsuarios.Count; i++)
                    {
                        if(this.listUsuarios[i].Tipo == 1)
                        {
                            this.listUsuarios.RemoveAt(i--);
                        }
                    }
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Nome");
                foreach (Util.ObjectUser to in this.listUsuarios)
                {
                    DataRow dr = dt.NewRow();
                    dr["Nome"] = to.Nome;
                    dt.Rows.Add(dr);
                }
                dgUsuario.DataSource = dt.DefaultView;
                btnAtualizarUsuario.Enabled = false;
                btnExcluirUsuario.Enabled = false;
                btnNovoUsuario.Enabled = true;
                this.index_selGridUser = -1;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgUsuario_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.index_selGridUser = e.RowIndex;
            btnNovoUsuario.Enabled = false;
            btnExcluirUsuario.Enabled = true;
            btnAtualizarUsuario.Enabled = true;
            try
            {
                txtNomeUsuario.Text = this.listUsuarios[this.index_selGridUser].Nome;
                txtSenhaUsuario.Text = this.listUsuarios[this.index_selGridUser].Senha;
                if (this.User.Tipo == 1)
                {
                    cbTipoUsuario.SelectedIndex = this.listUsuarios[this.index_selGridUser].Tipo - 1;
                }
                else
                {
                    if(this.listUsuarios[this.index_selGridUser].Tipo == 2)
                    {
                        cbTipoUsuario.SelectedIndex = 0;
                    }
                    else
                    {
                        cbTipoUsuario.SelectedIndex = 1;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnAtualizarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNomeUsuario.Text.Length >= 3 && txtSenhaUsuario.Text.Length >= 4)
                {
                   
                    Util.ObjectUser user = new ObjectUser();
                    user.Id = this.listUsuarios[this.index_selGridUser].Id;
                    user.Nome = txtNomeUsuario.Text;
                    user.Senha = txtSenhaUsuario.Text;
                    user.Nome = txtNomeUsuario.Text;
                    
                    
                    user.Tipo = Convert.ToInt32(cbTipoUsuario.SelectedValue.ToString());
                    if(user.Tipo == 2)
                    {
                        user.Tecnico = true;
                    }
                    else
                    {
                        user.Tecnico = false;
                    }
                    Util.ObjectUser.Persist(user);
                    btnClearUsuarios_Click(null, null);
                    
                }
                else
                {
                    if (txtNomeUsuario.Text.Length < 3)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Usuario_NomeObrigatorio);
                        }
                        
                    }
                    else if (txtSenhaUsuario.Text.Length < 4)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Usuario_SenhaObrigatorio);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnNovoUsuario_Click(object sender, EventArgs e)
        {
            try
            { 
                if (txtNomeUsuario.Text.Length >= 3 && txtSenhaUsuario.Text.Length >= 4)
                {
                    Util.ObjectUser user = new ObjectUser();
                    user.Id = 0;
                    user.Nome = txtNomeUsuario.Text;
                    user.Senha = txtSenhaUsuario.Text;

                    user.Tipo = Convert.ToInt32(cbTipoUsuario.SelectedValue.ToString());
                    if (user.Tipo == 2)
                    {
                        user.Tecnico = true;
                    }
                    else
                    {
                        user.Tecnico = false;
                    }
                    
                    Util.ObjectUser.Persist(user);
                    btnClearUsuarios_Click(null, null);
                }
                else
                {
                    if (txtNomeUsuario.Text.Length < 3)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Usuario_NomeObrigatorio);
                        }

                    }
                    else if (txtSenhaUsuario.Text.Length < 4)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Usuario_SenhaObrigatorio);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnExcluirUsuario_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(Negocio.IdiomaResxExtensao.Configuracao_UsuarioPermExcluir, Negocio.IdiomaResxExtensao.Configuracao_btnExcluirUsuario, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (confirm == DialogResult.Yes)
            {
                int id = this.listUsuarios[this.index_selGridUser].Id;
                if (Util.ObjectUser.User_Delete(id))
                {
                    btnClearUsuarios_Click(null, null);
                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracao_UsuarioErrorRemove);
                    }
                }
            }
        }

        private void btn_par_dat_06_Click(object sender, EventArgs e)
        {
            try
            {
                fDat06 fd = new fDat06();
                DialogResult dr = fd.ShowDialog();
                if(dr == DialogResult.OK)
                {
                    _parametros.Dat_06_BAS_Pref = fd._parametros.Dat_06_BAS_Pref;
                    _parametros.Dat_06_CAN_Pref = fd._parametros.Dat_06_CAN_Pref;
                    _parametros.Dat_06_FRM_Pref = fd._parametros.Dat_06_FRM_Pref;
                    _parametros.Dat_06_FRM_SEP = fd._parametros.Dat_06_FRM_SEP;
                    _parametros.Dat_06_UNT_Pref = fd._parametros.Dat_06_UNT_Pref;
                    _parametros.Dat_06_CAN_1_IsPonto = fd._parametros.Dat_06_CAN_1_IsPonto;
                    _parametros.Dat_06_FRM_1_IsPonto = fd._parametros.Dat_06_FRM_1_IsPonto;
                    _parametros.Dat_06_UNT_1_IsPonto = fd._parametros.Dat_06_UNT_1_IsPonto;
                    _parametros.Dat_06_UNT_2_IsPonto = fd._parametros.Dat_06_UNT_2_IsPonto;
                    _parametros.Dat_06_BAS_1_IsPonto = fd._parametros.Dat_06_BAS_1_IsPonto;
                    _parametros.Dat_06_BAS_Habilitado = fd._parametros.Dat_06_BAS_Habilitado;

                    PersistirParametros();
                    Util.ObjectParametros.InitLoad();
                    _parametros = Util.ObjectParametros.Load();

                    if (_parametros.IdDispositivo2 == 0)
                    {                        
                        for (int i = 16; _colorantes.Count >= 32 && i <= 32; i++)
                        {
                            _colorantes[i].Habilitado = false;                                
                        }
                        
                        PersistirColorantesSemMensagem();
                        _colorantes = Util.ObjectColorante.List();
                        PersistirRecirculacaoSemMensagem();
                        this._colorantesProd = _colorantes.ToList();
                        atualizaDataGridProdutos();
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cb_motor_PlacaMov_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_motor_PlacaMov.SelectedIndex >= 0)
                {
                    Util.ObjectMotorPlacaMovimentacao pM = this._PlacaMov._pMotor[cb_motor_PlacaMov.SelectedIndex];
                    gbPlacaMov.Visible = true;
                    cb_tipo_motor_PlacaMov.SelectedIndex = pM.TipoMotor;

                    txt_Aceleracao_PlacaMov.Text = pM.Aceleracao.ToString();
                    txt_Velocidade_PlacaMov.Text = pM.Velocidade.ToString();
                    txt_Delay_PlacaMov.Text = pM.Delay.ToString();
                    txt_Pulsos_PlacaMov.Text = pM.Pulsos.ToString();

                }
                else
                {
                    gbPlacaMov.Visible = false;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cb_tipo_motor_PlacaMov_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(cb_tipo_motor_PlacaMov.SelectedIndex == 0)
                {
                    gbPlacaMovMotor.Visible = true;
                }
                else if (cb_tipo_motor_PlacaMov.SelectedIndex == 1)
                {
                    gbPlacaMovMotor.Visible = false;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnAtualizaPlacaMovMotor_Click(object sender, EventArgs e)
        {
            try
            {
                int index = cb_motor_PlacaMov.SelectedIndex;
                if (index >= 0)
                {
                    this._PlacaMov._pMotor[index].Aceleracao = txt_Aceleracao_PlacaMov.ToInt();
                    this._PlacaMov._pMotor[index].Velocidade = txt_Velocidade_PlacaMov.ToInt();
                    this._PlacaMov._pMotor[index].Delay = txt_Delay_PlacaMov.ToInt();
                    this._PlacaMov._pMotor[index].Pulsos = txt_Pulsos_PlacaMov.ToInt();

                    this._PlacaMov._pMotor[index].TipoMotor = cb_tipo_motor_PlacaMov.SelectedIndex;

                    string msg = "";
                    bool retorno = true;
                    if (retorno && !Util.ObjectMotorPlacaMovimentacao.Validate(this._PlacaMov._pMotor, out msg))
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos
                                + Environment.NewLine + Environment.NewLine
                                + msg);
                        }
                        retorno = false;

                    }

                    if (retorno)
                    {
                        Util.ObjectMotorPlacaMovimentacao.Persist(this._PlacaMov._pMotor);

                        this._PlacaMov._pMotor = Util.ObjectMotorPlacaMovimentacao.List();
                        cb_motor_PlacaMov.DisplayMember = "NameTag";
                        cb_motor_PlacaMov.ValueMember = "Circuito";
                        cb_motor_PlacaMov.DataSource = this._PlacaMov._pMotor.ToList();
                        cb_motor_PlacaMov.SelectedIndex = -1;

                        bool confirma = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            confirma = m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso );
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btn_par_dat_05_Click(object sender, EventArgs e)
        {
            try
            {
                fDat05 fd = new fDat05();
                DialogResult dr = fd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    _parametros.Dat_05_BAS_Pref = fd._parametros.Dat_05_BAS_Pref;
                    _parametros.Dat_05_CAN_Pref = fd._parametros.Dat_05_CAN_Pref;
                    _parametros.Dat_05_FRM_Pref = fd._parametros.Dat_05_FRM_Pref;
                    _parametros.Dat_05_FRM_SEP = fd._parametros.Dat_05_FRM_SEP;
                    _parametros.Dat_05_UNT_Pref = fd._parametros.Dat_05_UNT_Pref;
                    _parametros.Dat_05_CAN_1_IsPonto = fd._parametros.Dat_05_CAN_1_IsPonto;
                    _parametros.Dat_05_FRM_1_IsPonto = fd._parametros.Dat_05_FRM_1_IsPonto;
                    _parametros.Dat_05_UNT_1_IsPonto = fd._parametros.Dat_05_UNT_1_IsPonto;
                    _parametros.Dat_05_UNT_2_IsPonto = fd._parametros.Dat_05_UNT_2_IsPonto;
                    _parametros.Dat_05_BAS_1_IsPonto = fd._parametros.Dat_05_BAS_1_IsPonto;
                    _parametros.Dat_05_BAS_Habilitado = fd._parametros.Dat_05_BAS_Habilitado;

                    PersistirParametros();
                    Util.ObjectParametros.InitLoad();
                    _parametros = Util.ObjectParametros.Load();

                    if (_parametros.IdDispositivo2 == 0)
                    {

                        for (int i = 16; _colorantes.Count >= 32 && i < 32; i++)
                        {
                            _colorantes[i].Habilitado = false;
                        }
                        PersistirColorantesSemMensagem();
                        _colorantes = Util.ObjectColorante.List();
                        PersistirRecirculacaoSemMensagem();
                        this._colorantesProd = _colorantes.ToList();
                        atualizaDataGridProdutos();
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txt_Velocidade_PlacaMov_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double _vel = txt_Velocidade_PlacaMov.ToInt();
                double _pul = txt_Pulsos_PlacaMov.ToInt();
                int _del = (int)((_pul / _vel) * 1000);                
                txt_Delay_PlacaMov.Text = _del.ToString();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
                txt_Delay_PlacaMov.Text = "500";
            }
        }

        private void txt_Pulsos_PlacaMov_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double _vel = txt_Velocidade_PlacaMov.ToInt();
                double _pul = txt_Pulsos_PlacaMov.ToInt();
                int _del = (int)((_pul / _vel) * 1000);
                txt_Delay_PlacaMov.Text = _del.ToString();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    txt_Delay_PlacaMov.Text = "500";
            }
        }

        private void btn_Abrir_Gaveta_Click(object sender, EventArgs e)
        {
            btn_Fechar_Gaveta.Enabled = false;
            btn_Abrir_Gaveta.Enabled = false;
            try
            {
                this.Invoke(new MethodInvoker(AbrirGaveta));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    btn_Fechar_Gaveta.Enabled = true;
                btn_Abrir_Gaveta.Enabled = true;
            }
        }

        private void AbrirGaveta()
        {
            ModBusDispenserMover_P3 dispenserP3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
            try
            {
                dispenserP3.Disconnect_Mover();
                dispenserP3.Connect_Mover();
                dispenserP3.ReadSensores_Mover();
                if ((dispenserP3.IsNativo == 0 || dispenserP3.IsNativo == 2) && !dispenserP3.SensorEmergencia && !dispenserP3.SensorGavetaAberta)
                {
                    bool isConfirm = false;
                    Task task = Task.Factory.StartNew(() => dispenserP3.AbrirGaveta(true));
                    Thread.Sleep(500);
                    for (int i = 0; i < 20; i++)
                    {
                        if (dispenserP3.TerminouProcessoDuplo)
                        {
                            dispenserP3.ReadSensores_Mover();
                            if (dispenserP3.SensorGavetaAberta && dispenserP3.SensorAltoBicos)
                            {
                                isConfirm = true;
                                break;
                            }
                        }
                        Thread.Sleep(500);
                    }
                    if (!isConfirm)
                    {
                        MessageBox.Show("Falha no comando");
                    }
                }
                else if (dispenserP3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + dispenserP3.IsNativo.ToString());
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
                    dispenserP3.Disconnect_Mover();
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}
			}

            btn_Abrir_Gaveta.Enabled = true;
            btn_Fechar_Gaveta.Enabled = true;
        }

        private void btn_Fechar_Gaveta_Click(object sender, EventArgs e)
        {
            btn_Fechar_Gaveta.Enabled = false;
            btn_Abrir_Gaveta.Enabled = false;
            try
            {
                this.Invoke(new MethodInvoker(FecharGaveta));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    btn_Fechar_Gaveta.Enabled = true;
                btn_Abrir_Gaveta.Enabled = true;
            }
        }

        private void FecharGaveta()
        {
            ModBusDispenserMover_P3 dispenserP3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
            try
            {
                dispenserP3.Disconnect_Mover();
                dispenserP3.Connect_Mover();
                dispenserP3.ReadSensores_Mover();
                if ((dispenserP3.IsNativo == 0 || dispenserP3.IsNativo == 2) && !dispenserP3.SensorEmergencia && !dispenserP3.SensorGavetaFechada)
                {
                    bool isConfirm = false;
                    Task task = Task.Factory.StartNew(() => dispenserP3.FecharGaveta(true));
                    Thread.Sleep(500);
                    for (int i = 0; i < 20; i++)
                    {
                        if (dispenserP3.TerminouProcessoDuplo)
                        {
                            dispenserP3.ReadSensores_Mover();
                            if (dispenserP3.SensorGavetaFechada && dispenserP3.SensorBaixoBicos)
                            {
                                isConfirm = true;
                                break;
                            }
                        }
                        Thread.Sleep(500);
                    }
                    if (!isConfirm)
                    {
                        MessageBox.Show("Falha no comando");
                    }
                }
                else if (dispenserP3.SensorEmergencia)
                {
                    //MessageBox.Show("Emergência pressionado");
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia);
                }
                else
                {
                    //MessageBox.Show("Error Nativo:" + this.modBusDispenser_P3.IsNativo.ToString());
                    MessageBox.Show(Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo + dispenserP3.IsNativo.ToString());
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
                    dispenserP3.Disconnect_Mover();
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}
			}

            btn_Fechar_Gaveta.Enabled = true;
            btn_Abrir_Gaveta.Enabled = true;
        }

        private void chkLogSerialMenu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Modbus.Constantes.bSerialLog = chkLogSerialMenu.Checked;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnInput_Click(object sender, EventArgs e)
        {
            try
            {
                switch((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Placa_2:
                        {
                            fPlacaInput fIn = new fPlacaInput();
                            fIn.ShowDialog();
                            break;
                        }
                    default:
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog("Leitura das Entradas Somente para o Dispositivo Placa 2");
                            }
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void cmbDatPadrao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gbRdDat05.Visible = false;
                gbRdDat05.Enabled = false;
                gbRdDat06.Visible = false;
                gbRdDat06.Enabled = false;
                gbUDCP.Visible = false;
                gbUDCP.Enabled = false;

                if (cmbDatPadrao.SelectedIndex == 4)
                {
                    gbRdDat05.Visible = true;
                    gbRdDat05.Enabled = true;
                }
                else if (cmbDatPadrao.SelectedIndex == 5)
                {
                    gbRdDat06.Visible = true;
                    gbRdDat06.Enabled = true;
                }
                else if (cmbDatPadrao.SelectedIndex == 7)
                {
                    gbUDCP.Enabled = true;
                    gbUDCP.Visible = true;

                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void AtualizaGridAbastecimento()
        {
            try
            {
                this.listAbastecimento = Util.ObjectAbastecimento.List();
                DataTable dt = new DataTable();
                dt.Columns.Add("Nome");
                foreach (Util.ObjectAbastecimento to in this.listAbastecimento)
                {
                    DataRow dr = dt.NewRow();
                    dr["Nome"] = to.Nome;
                    dt.Rows.Add(dr);
                }
                dgAbastecimento.DataSource = dt.DefaultView;
                btnAtualizarAbastecimento.Enabled = false;
                btnExcluirAbastecimento.Enabled = false;
                btnNovoAbastecimento.Enabled = true;
                this.index_selGridAbastecimento = -1;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgAbastecimento_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.index_selGridAbastecimento = e.RowIndex;
            btnNovoAbastecimento.Enabled = false;
            btnExcluirAbastecimento.Enabled = true;
            btnAtualizarAbastecimento.Enabled = true;
            try
            {
                txtNomeAbastecimento.Text = this.listAbastecimento[this.index_selGridAbastecimento].Nome;
                txtConteudoAbastecimento.Text = this.listAbastecimento[this.index_selGridAbastecimento].Conteudo;
                cbUnidadeAbastecimento.SelectedValue = this.listAbastecimento[this.index_selGridAbastecimento].UnMed.ToString();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
     
        private void btnClearAbastecimento_Click(object sender, EventArgs e)
        {
            try
            {
                txtNomeAbastecimento.Text = "";
                txtConteudoAbastecimento.Text = "";
                cbUnidadeAbastecimento.SelectedIndex = 0;
                AtualizaGridAbastecimento();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnAtualizarAbastecimento_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNomeAbastecimento.Text.Length >= 3 && txtConteudoAbastecimento.Text.Length >= 3)
                {
                    Util.ObjectAbastecimento abast = new ObjectAbastecimento();
                    abast.Id = this.listAbastecimento[this.index_selGridAbastecimento].Id;
                    abast.Nome = txtNomeAbastecimento.Text;
                    abast.Conteudo = txtConteudoAbastecimento.Text;
                    abast.UnMed = Convert.ToInt32(cbUnidadeAbastecimento.SelectedValue.ToString());

                    Util.ObjectAbastecimento.Persist(abast);
                    btnClearAbastecimento_Click(null, null);

                }
                else
                {
                    if (txtNomeAbastecimento.Text.Length < 3)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Abastecimento_NomeObrigatorio);
                        }

                    }
                    else if (txtConteudoAbastecimento.Text.Length < 3)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Abastecimento_ConteudoObrigatorio);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnExcluirAbastecimento_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult confirm = MessageBox.Show(Negocio.IdiomaResxExtensao.Configuracao_AbastecimentoPermExcluir, Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirAbastecimento, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (confirm == DialogResult.Yes)
                {
                    int id = this.listAbastecimento[this.index_selGridAbastecimento].Id;
                    if (Util.ObjectAbastecimento.Abastecimento_Delete(id))
                    {
                        btnClearAbastecimento_Click(null, null);
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracao_AbastecimentoErrorRemove);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnNovoAbastecimento_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNomeAbastecimento.Text.Length >= 3 && txtConteudoAbastecimento.Text.Length >= 3)
                {
                    Util.ObjectAbastecimento abast = new ObjectAbastecimento();
                    abast.Id = 0;
                    abast.Nome = txtNomeAbastecimento.Text;
                    abast.Conteudo = txtConteudoAbastecimento.Text;

                    abast.UnMed = Convert.ToInt32(cbUnidadeAbastecimento.SelectedValue.ToString());
                    Util.ObjectAbastecimento.Persist(abast);
                    btnClearAbastecimento_Click(null, null);
                }
                else
                {
                    if (txtNomeAbastecimento.Text.Length < 3)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Abastecimento_NomeObrigatorio);
                        }

                    }
                    else if (txtConteudoAbastecimento.Text.Length < 3)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Abastecimento_ConteudoObrigatorio);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnDownloadMessage_Click(object sender, EventArgs e)
        {
            bool _closeAguarde = _parametros.ViewMessageProc;
            try
            {
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(Wait_Message));
                    if (_parametros.ViewMessageProc)
                    {
                        Application.DoEvents();
                        Thread.Sleep(200);
                        Application.DoEvents();
                    }
                }
            
                Util.ObjectMensagem.LoadMessage();
                string _PathFile = Path.Combine(Environment.CurrentDirectory, "message.xml");
                if (File.Exists(_PathFile))
                {
                    File.Delete(_PathFile);
                    Thread.Sleep(2000);
                }
                Negocio.MessageShowJson msgSj = new Negocio.MessageShowJson();
                Negocio.IdiomaResxExtensao.setMessageShow(ref msgSj);

                string jsp = Negocio.XmlExtension.Serialize(msgSj);

                using (StreamWriter sW = new StreamWriter(File.Open(_PathFile, FileMode.Create), Encoding.GetEncoding("ISO-8859-1")))
                {
                    sW.WriteLine(jsp);
                    sW.Close();
                }
                MessageBox.Show("File created :" + _PathFile);
            }
            catch(Exception exc)
            {
                MessageBox.Show("Download Message Error:" + exc.Message);
            }

            try
            {
                if (_parametros.ViewMessageProc || _closeAguarde)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnUploadMessage_Click(object sender, EventArgs e)
        {
            bool _closeAguarde = _parametros.ViewMessageProc;
            
            try
            {
                OpenFileDialog opFd = new OpenFileDialog();
                opFd.Multiselect = false;
                opFd.Filter = "XML File|*.xml";
                DialogResult dr = opFd.ShowDialog();
                if (dr == DialogResult.OK && opFd.FileName != null && opFd.FileName.Length > 0 && File.Exists(opFd.FileName))
                {
                    string strMsgFile = "";
                    using (StreamReader sr =  new StreamReader(opFd.FileName, System.Text.Encoding.GetEncoding("ISO-8859-1"), true))
                    {
                        strMsgFile = sr.ReadToEnd();
                        sr.Close();
                    }
                    Negocio.MessageShowJson msgJ = Negocio.XmlExtension.Deserialize<Negocio.MessageShowJson>(strMsgFile);
                    if(msgJ!= null)
                    {
                        if (_parametros.ViewMessageProc)
                        {
                            this.Invoke(new MethodInvoker(Wait_Message));
                            if (_parametros.ViewMessageProc)
                            {
                                Application.DoEvents();
                                Thread.Sleep(200);
                                Application.DoEvents();
                            }
                        }

                        Negocio.IdiomaResxExtensao.PeristMessageShoW(msgJ, _closeAguarde);
                        Util.ObjectMensagem.LoadMessage();
                        MessageBox.Show("Message uploaded successfully.");
                    }
                }
                else
                {
                    MessageBox.Show("File Inválid." );
                }
                opFd = null;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Upload Message Error:" + exc.Message);
            }

            try
            {
                if (_parametros.ViewMessageProc || _closeAguarde)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void cbTipoLimpBicos_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if(cbTipoLimpBicos.SelectedValue.ToString() == "1")
                {
                    gbLimpBicosConfiguravel.Visible = false;
                    gbLimpBicosConfiguravel.Enabled = false;
                    lblGeralDelayLimpBicos.Enabled = true;
                    txtGeralDelayLimpBicos.Enabled = true;
                    lblGeralDelayLimpBicos.Visible = true;
                    txtGeralDelayLimpBicos.Visible = true;
                }
                else
                {
                    gbLimpBicosConfiguravel.Visible = true;
                    gbLimpBicosConfiguravel.Enabled = true;
                    lblGeralDelayLimpBicos.Enabled = false;
                    txtGeralDelayLimpBicos.Enabled = false;
                    lblGeralDelayLimpBicos.Visible = false;
                    txtGeralDelayLimpBicos.Visible = false;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        private void AtualizaGridLimpBicos()
        {
            try
            {
                this.listLimpBicos = Util.ObjectLimpBicos.List();
                DataTable dt = new DataTable();
                dt.Columns.Add("Horario");
                foreach (Util.ObjectLimpBicos to in this.listLimpBicos)
                {
                    DataRow dr = dt.NewRow();
                    dr["Horario"] = to.Horario.Value.ToString(@"hh\:mm\:ss");
                    dt.Rows.Add(dr);
                }
                dgvLimpBicos.DataSource = dt.DefaultView;
                btnAtualizarLimpBicosConfig.Enabled = false;
                btnExcluirLimpBicosConfig.Enabled = false;
                btnNovoLimpBicosConfig.Enabled = true;
                this.index_selGridLimpBicos = -1;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnClearLimpBicosConfig_Click(object sender, EventArgs e)
        {
            try
            {
                txtLimpBicosPeriodoConfig.Text = "";               
                
                AtualizaGridLimpBicos();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgvLimpBicos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.index_selGridLimpBicos = e.RowIndex;
            btnNovoLimpBicosConfig.Enabled = false;
            btnExcluirLimpBicosConfig.Enabled = true;
            btnAtualizarLimpBicosConfig.Enabled = true;
            try
            {
                txtLimpBicosPeriodoConfig.Text = this.listLimpBicos[this.index_selGridLimpBicos].Horario.Value.ToString(@"hh\:mm\:ss");
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnAtualizarLimpBicosConfig_Click(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ts;
                if (txtLimpBicosPeriodoConfig.Text.Length == 8 && TimeSpan.TryParse(txtLimpBicosPeriodoConfig.Text, out ts) && (ts.Hours >= 0 && ts.Hours <= 23) && (ts.Minutes >= 0 && ts.Minutes <= 59)
                    && (ts.Seconds >= 0 && ts.Seconds <= 59))
                {
                    Util.ObjectLimpBicos per = new ObjectLimpBicos();
                    per.Id = this.listLimpBicos[this.index_selGridLimpBicos].Id;
                    per.Horario = ts;
                    Util.ObjectLimpBicos.Persist(per);
                    btnClearLimpBicosConfig_Click(null, null);

                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.LimpBicosFormatoInvalido);
                    }

                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.LimpBicosErrorAtualizarPeriodo);                    
                }
            }
        }

        private void btnExcluirLimpBicosConfig_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult confirm = MessageBox.Show(Negocio.IdiomaResxExtensao.LimpBicosExcluirPeriodo, Negocio.IdiomaResxExtensao.LimpBicosExcluir, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (confirm == DialogResult.Yes)
                {
                    int id = this.listLimpBicos[this.index_selGridLimpBicos].Id;
                    if (Util.ObjectLimpBicos.LimpBicos_Delete(id))
                    {
                        btnClearLimpBicosConfig_Click(null, null);
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.LimpBicosErrorExcluirPeriodo);
                        }

                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.LimpBicosErrorExcluirPeriodo);
                }
            }
        }

        private void btnNovoLimpBicosConfig_Click(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ts;
                if (txtLimpBicosPeriodoConfig.Text.Length == 8 && TimeSpan.TryParse(txtLimpBicosPeriodoConfig.Text, out ts) && (ts.Hours >= 0 && ts.Hours <= 23) && (ts.Minutes >= 0 && ts.Minutes <= 59)
                        && (ts.Seconds >= 0 && ts.Seconds <= 59))
                {
                    Util.ObjectLimpBicos per = new ObjectLimpBicos();
                    per.Id = 0;
                    per.Horario = ts;
                    Util.ObjectLimpBicos.Persist(per);
                    btnClearLimpBicosConfig_Click(null, null);

                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.LimpBicosFormatoInvalido);
                    }

                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.LimpBicosErrorInserirPeriodo);                    
                }
            }
        }

        private void btnTesteComunicacaoUser_Click(object sender, EventArgs e)
        {
            string msgComunication = "";
            bool exisFirtsPorta = false;
            bool exisSecondPorta = false;
            bool existStringFirstPorta = false;
            bool connectFirstPort = false;
            bool connectSecondPort = false;
            bool opennedPort = false;
            bool opennedPort2 = false;
            try
            {
                string[] arrayPorts = SerialPort.GetPortNames();

                if (((ComboBoxItem)(cmbGeralDispositivoUser.SelectedItem)).Value == 3)
                {
                    exisFirtsPorta = true;
                    foreach (string _porta in arrayPorts)
                    {
                        if (_porta == txt_nome_DispositivoUser.Text)
                        {
                            existStringFirstPorta = true;
                            break;
                        }

                    }
                    if (exisFirtsPorta && existStringFirstPorta)
                    {
                        PaintMixerInterface_P2 p2 = new PaintMixerInterface_P2(1, txt_nome_DispositivoUser.Text);
                        p2.Connect(2000);
                        if (p2.mb.modbusStatus)
                        {
                            connectFirstPort = true;
                        }
                        opennedPort = p2.mb.isOpen();

                        p2.Disconnect();
                        p2 = null;
                    }
                    if (connectFirstPort)
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo1;
                        int disp2 = Convert.ToInt32(cmbGeralDispositivo_2User.SelectedValue.ToString());
                        if (disp2 == 3)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2User.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }
                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P2 p2 = new PaintMixerInterface_P2(1, txt_nome_Dispositivo_2User.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                    else
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo1 + Environment.NewLine + "Open communication with the board 1: " +
                                        (opennedPort ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                }

                if (((ComboBoxItem)(cmbGeralDispositivoUser.SelectedItem)).Value == 5)
                {
                    exisFirtsPorta = true;
                    foreach (string _porta in arrayPorts)
                    {
                        if (_porta == txt_nome_DispositivoUser.Text)
                        {
                            existStringFirstPorta = true;
                            break;
                        }
                    }
                    if (exisFirtsPorta && existStringFirstPorta)
                    {
                        PaintMixerInterface_P4 p2 = new PaintMixerInterface_P4(1, txt_nome_DispositivoUser.Text);
                        p2.Connect(2000);
                        if (p2.mb.modbusStatus)
                        {
                            connectFirstPort = true;
                        }
                        opennedPort = p2.mb.isOpen();

                        p2.Disconnect();
                        p2 = null;
                    }
                    if (connectFirstPort)
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo1;
                        int disp2 = Convert.ToInt32(cmbGeralDispositivo_2User.SelectedValue.ToString());
                        if (disp2 == 3)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2User.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }
                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P2 p2 = new PaintMixerInterface_P2(1, txt_nome_Dispositivo_2User.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }
                        }
                        if (disp2 == 5)
                        {
                            foreach (string _porta in arrayPorts)
                            {
                                if (_porta == txt_nome_Dispositivo_2.Text)
                                {
                                    exisSecondPorta = true;
                                    break;
                                }

                            }
                            if (exisSecondPorta)
                            {
                                PaintMixerInterface_P4 p2 = new PaintMixerInterface_P4(1, txt_nome_Dispositivo_2.Text);
                                p2.Connect(2000);
                                if (p2.mb.modbusStatus)
                                {
                                    connectSecondPort = true;
                                }
                                opennedPort = p2.mb.isOpen();
                                p2.Disconnect();
                                p2 = null;

                                if (connectSecondPort)
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2;
                                }
                                else
                                {
                                    msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Open communication with the board 2: " +
                                        (opennedPort2 ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                                }
                            }
                            else
                            {
                                msgComunication += Environment.NewLine + Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 + Environment.NewLine + "Porta Inválida!";
                            }

                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                    else
                    {
                        msgComunication = Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo1 + Environment.NewLine + "Open communication with the board 1: " +
                                        (opennedPort ? Negocio.IdiomaResxExtensao.Global_Sim : Negocio.IdiomaResxExtensao.Global_Nao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(msgComunication);
                        }
                    }
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    string msg = string.Format(Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivoGeral, ex.Message);
                    m.ShowDialog(msg);
                }
            }
        }

        private void btnFunctiongeral_Click(object sender, EventArgs e)
        {
            using (fConfigGeral cGeral = new fConfigGeral(this.authentication, this._parametros))
            {
                DialogResult dr= cGeral.ShowDialog();
                if(dr == DialogResult.OK)
                {
                    this._parametros.HabilitarTecladoVirtual = cGeral._parametros.HabilitarTecladoVirtual;
                    this._parametros.HabilitarFormulaPersonalizada = cGeral._parametros.HabilitarFormulaPersonalizada;
                    this._parametros.HabilitarTesteRecipiente = cGeral._parametros.HabilitarTesteRecipiente;
                    this._parametros.HabilitarIdentificacaoCopo = cGeral._parametros.HabilitarIdentificacaoCopo;
                    this._parametros.HabilitarDispensaSequencial = cGeral._parametros.HabilitarDispensaSequencial;
                    this._parametros.HabilitarTouchScrenn = cGeral._parametros.HabilitarTouchScrenn;
                    this._parametros.TreinamentoCal = cGeral._parametros.TreinamentoCal;
                    this._parametros.DelayEsponja = cGeral._parametros.DelayEsponja;
                    this._parametros.ViewMessageProc = cGeral._parametros.ViewMessageProc;
                    this._parametros.NameRemoteAccess = cGeral._parametros.NameRemoteAccess;
                    PersistirParametros();
                    Util.ObjectParametros.InitLoad();
                    _parametros = Util.ObjectParametros.Load();
                }
            }
        }

        private void btnValve_Click(object sender, EventArgs e)
        {
            try
            {
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Placa_2:
                        {
                            fPlacaValvula fVl = new fPlacaValvula();
                            fVl.ShowDialog();
                            break;
                        }
                    default:
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog("Leitura das Válvulas Somente para o Dispositivo Placa 2");
                            }
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void atualizaDataGridProdutos()
        {
            
        }

        /// <summary>
        /// Faz a inicialização dos campos de cada válvula na tela de configurações de Recirculação.
        /// </summary>
        private void inicializaDataGridRecircularProdutos()
        {
            try
            {
                for (int i = 0; i < this._listRecircular.Count; i++)
                {
                    // Habilita ou desabilita estado dos campos de cada válvula
                    atualizaEstadoDataGridRecircularProdutos();

                    // Inicializa os campos na tela com os dados do banco de dados
                    _recircularVolDin[i].Text = this._listRecircular[i].VolumeDin.ToString();
                    _recircularDias[i].Text = this._listRecircular[i].Dias.ToString();
                    _recircularVol[i].Text = this._listRecircular[i].VolumeRecircular.ToString();
                    _recircularValve[i].Checked = this._listRecircular[i].isValve;
                    _recircularAuto[i].Checked = this._listRecircular[i].isAuto;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
            }
        }

        /// <summary>
        /// Habilita ou desabilita estado dos campos de cada válvula na tela de configurações de Recirculação.
        /// </summary>
        private void atualizaEstadoDataGridRecircularProdutos()
        {
            // Verifica se algum dos dois parâmetros de habilitação da recirculação está marcado na interface
            bool habilitar = chkHabilitarMonitRecirculacao.Checked || chkHabilitarMonitRecirculacaoAuto.Checked;

            try
            {
                for (int i = 0; i < this._listRecircular.Count; i++)
                {
                    // Define se é possível habilitar ou desabilitar a válvula com base na interface
                    _recircularCKT[i].Enabled = habilitar;

                    // Define a cor de fundo do botão de acordo com o estado da válvula
                    if (this._listRecircular[i].Habilitado)
                    {
                        _recircularCKT[i].BackColor = Cores.Seguir;
                    }
                    else
                    {
                        _recircularCKT[i].BackColor = Cores.Parar;
                    }

                    // Define se é possível habilitar ou desabilitar os campos de acordo com o estado da válvula e a interface
                    _recircularDias[i].Enabled = habilitar && this._listRecircular[i].Habilitado;
                    _recircularVol[i].Enabled = habilitar && this._listRecircular[i].Habilitado;
                    _recircularVolDin[i].Enabled = habilitar && this._listRecircular[i].Habilitado;
                    _recircularValve[i].Enabled = habilitar && this._listRecircular[i].Habilitado;
                    _recircularAuto[i].Enabled = habilitar && this._listRecircular[i].Habilitado;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
            }
        }

        private void btnProductPar_Click(object sender, EventArgs e)
        {
            try
            {
                fColoranteConfig fCol = new fColoranteConfig();
                if (fCol.ShowDialog() == DialogResult.OK)
                {
                    if (_parametros.ViewMessageProc)
                    {
                        this.Invoke(new MethodInvoker(Wait_Message));
                    }
                    bool isAlteradoColorante = false;
                    for (int i = 0; i < fCol._colorantes.Count; i++)
                    {
                        if (this._colorantes[i].Seguidor != fCol._colorantes[i].Seguidor || this._colorantes[i].Step != fCol._colorantes[i].Step)
                        {
                            isAlteradoColorante = true;
                        }
                        this._colorantes[i].Habilitado = fCol._colorantes[i].Habilitado;
                        this._colorantes[i].Seguidor = fCol._colorantes[i].Seguidor;
                        this._colorantes[i].Step = fCol._colorantes[i].Step;
                        this._colorantes[i].VolumeBicoIndividual = fCol._colorantes[i].VolumeBicoIndividual;
                    }

                    PersistirColorantesPar();
                    this._colorantes = Util.ObjectColorante.List();
                    if (_parametros.ViewMessageProc)
                    {
                        Application.DoEvents();
                        Thread.Sleep(200);
                        Application.DoEvents();
                    }
                    if (isAlteradoColorante)
                    {
                        string detalhesCol = "";
                        foreach (Util.ObjectColorante _col in this._colorantes)
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
                    }

                    Util.ObjectParametros.InitLoad();

                    this._toolTipProducts.RemoveAll();
                    if (_parametros.ViewMessageProc)
                    {
                        Application.DoEvents();
                        Thread.Sleep(200);
                        Application.DoEvents();
                    }
                    for (int i = 0; i <= 15; i++)
                    {
                        //[Cores para circuitos de colorante]
                        _circuito[i].BackColor = Cores.Parar;
                        _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguir;
                        _circuito[i].Enabled = true;
                        
                        if (_colorantes[i].Seguidor < 0)
                        {
                            
                            _circuito[i].Checked = _colorantes[i].Habilitado;
                            chkCircuito_CheckedChanged(_circuito[i], e);
                        }
                        else
                        {
                            if (_colorantes[i].Habilitado)
                            {
                                _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguidor_Tom_01;
                                _circuito[i].BackColor = Cores.Seguidor_Tom_01;
                                this._toolTipProducts.SetToolTip(_circuito[i], "Seguidor do Circuito: " + _colorantes[i].Seguidor.ToString());
                                _circuito[i].Enabled = false;
                            }
                        }


                        _NomeCorante[i].Text = _colorantes[i].Nome;
                        _massaEspecifica[i].Text = _colorantes[i].MassaEspecifica.ToString();
                        _correspondencia[i].SelectedIndex = _colorantes[i].Correspondencia;
                        _base[i].Checked = _colorantes[i].IsBase;
                        _bicoIndividual[i].Checked = _colorantes[i].IsBicoIndividual;
                    }

                    for (int i = 16; _circuito.Length >= 32 && i < 32; i++)
                    {
                        //[Cores para circuitos de colorante]
                        _circuito[i].BackColor = Cores.Parar;
                        _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguir;
                        _circuito[i].Enabled = true;
                            
                        if (_colorantes[i].Seguidor < 0)
                        {                               
                            _circuito[i].Checked = _colorantes[i].Habilitado;
                            chkCircuito_CheckedChanged(_circuito[i], e);
                        }
                        else
                        {
                                
                            _circuito[i].FlatAppearance.CheckedBackColor = Cores.Seguidor_Tom_01;
                            _circuito[i].BackColor = Cores.Seguidor_Tom_01;
                            this._toolTipProducts.SetToolTip(_circuito[i], "Seguidor do Circuito: " + _colorantes[i].Seguidor.ToString());
                            _circuito[i].Enabled = false;

                        }
                        _NomeCorante[i].Text = _colorantes[i].Nome;
                        _massaEspecifica[i].Text = _colorantes[i].MassaEspecifica.ToString();
                        int corresp = _colorantes[i].Correspondencia;

                        _correspondencia[i].SelectedIndex = corresp - 16;
                        _base[i].Checked = _colorantes[i].IsBase;
                        _bicoIndividual[i].Checked = _colorantes[i].IsBicoIndividual;
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			if (_parametros.ViewMessageProc)
            {
                this.Invoke(new MethodInvoker(ClosePrg));
            }
        }

        private void lblRecCircuito_Click(object sender, EventArgs e)
        {
            try
            {
                Label lbl = (Label)sender;
                int index = int.Parse(lbl.Tag.ToString());

                bool habilitado = !this._listRecircular[index].Habilitado;
                this._listRecircular[index].Habilitado = habilitado;

                // Habilita ou desabilita estado dos campos de cada válvula
                atualizaEstadoDataGridRecircularProdutos();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void chkHabilitarMonitRecirculacao_CheckedChanged(object sender, EventArgs e)
        {
            // Habilita ou desabilita estado dos campos de cada válvula
            atualizaEstadoDataGridRecircularProdutos();
        }
    }
}