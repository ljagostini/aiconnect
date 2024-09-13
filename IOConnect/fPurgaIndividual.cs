using Percolore.Core.Persistence.Xml;
using Percolore.Core.UserControl;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Percolore.Core;
using System.Linq;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.IOConnect.Util;

namespace Percolore.IOConnect
{
    public partial class fPurgaIndividual : Form
    {
        Util.ObjectParametros _parametros = null;
        PurgaVO _prmPurga;
        private List<Util.ObjectColorante> _colorantes = null;
        private Button[] _circuitos = null;
        int INDEX_CKT;
        int INDEX_ULTIMO_CKT;
        int INDEX_DISP;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        bool execPurga = false;
        int index_PurgaIndividual = -1;

        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private int qtdTrocaRecipiente = 0;
        private int counterFalha = 0;
        private bool isNewOperacao = false;
        private ToolTip _toolTipProducts = new ToolTip();

        private int i_Step = 0;

        fAguarde _fAguarde = null;

        public fPurgaIndividual(PurgaVO parametrosPurga)
        {
            InitializeComponent();

            this._toolTipProducts.ToolTipIcon = ToolTipIcon.None;
            this._toolTipProducts.IsBalloon = true;
            this._toolTipProducts.ShowAlways = true;

            this._prmPurga = parametrosPurga;
            this._parametros = Util.ObjectParametros.Load();

            this._colorantes = Util.ObjectColorante.List();
            this._circuitos = new Button[] { btn_ckt_01, btn_ckt_02, btn_ckt_03, btn_ckt_04, btn_ckt_05, btn_ckt_06, btn_ckt_07, btn_ckt_08, btn_ckt_09, btn_ckt_10, btn_ckt_11, btn_ckt_12, btn_ckt_13, btn_ckt_14, btn_ckt_15, btn_ckt_16,
            btn_ckt_17, btn_ckt_18, btn_ckt_19, btn_ckt_20, btn_ckt_21, btn_ckt_22, btn_ckt_23, btn_ckt_24, btn_ckt_25, btn_ckt_26, btn_ckt_27, btn_ckt_28, btn_ckt_29, btn_ckt_30, btn_ckt_31, btn_ckt_32, btn_ckt_33, btn_ckt_34, btn_ckt_35,
            btn_ckt_36, btn_ckt_37, btn_ckt_38, btn_ckt_39, btn_ckt_40, btn_ckt_41, btn_ckt_42, btn_ckt_43, btn_ckt_44, btn_ckt_45, btn_ckt_46, btn_ckt_47, btn_ckt_48};


            //Redimensiona e posiciona form
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            lblStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividualMonit_lblStatus_Msg01;
            lblSubStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividual_lblSubStatus_Msg01;

           
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            btn_Fechar.Text = Negocio.IdiomaResxExtensao.Global_Fechar;
            gbIndividual.Text = Negocio.IdiomaResxExtensao.PurgarIndividual_gbIndividual;

            //Habilitar controles
            lblStatus.Visible = true;
            progressBar.Visible = false;
            this.INDEX_CKT = -1;
            this.INDEX_ULTIMO_CKT = -1;
            gbIndividual.Enabled = true;
        }

        private void fPurgaIndividual_Load(object sender, EventArgs e)
        {
            try
            {
                List<Util.ObjectColorante> lcol = Util.ObjectColorante.List();
                foreach (Util.ObjectColorante _col in lcol)
                {
                    if (_col.Step > 0)
                    {
                        i_Step = _col.Step;
                        break;
                    }
                }
            }
            catch
            { }
            for (int i = 0; i <= 23; i++)
            {
                this._circuitos[i].Enabled = this._colorantes[i].Habilitado;                
                this._circuitos[i].Text = this._colorantes[i].Nome;
                if(this._colorantes[i].Habilitado && this._colorantes[i].Seguidor < 0)
                {
                    this._circuitos[i].BackColor = Color.FromArgb(6, 176, 37);
                }
                else if(this._colorantes[i].Habilitado && this._colorantes[i].Seguidor > 0)
                {
                    this._circuitos[i].BackColor = Cores.Seguidor_Tom_01;
                    this._toolTipProducts.SetToolTip(this._circuitos[i], "Seguidor do Circuito: " + _colorantes[i].Seguidor.ToString());
                }
                else
                {
                    this._circuitos[i].BackColor = Color.Red;
                }
            }
            if(this._prmPurga.LMDispositivos.Count == 1)
            {
                for (int i = 24; i <= 47; i++)
                {
                    this._circuitos[i].Visible = false;
                    this._circuitos[i].Enabled = false;                    
                }
            }
            else
            {
                for (int i = 24; i <= 47; i++)
                {
                    this._circuitos[i].Enabled = this._colorantes[i].Habilitado;
                    this._circuitos[i].Text = this._colorantes[i].Nome;
                    //if (this._colorantes[i].Habilitado)
                    //{
                    //    this._circuitos[i].BackColor = Color.FromArgb(6, 176, 37);
                    //}
                    //else
                    //{
                    //    this._circuitos[i].BackColor = Color.Red;
                    //}
                    if (this._colorantes[i].Habilitado && this._colorantes[i].Seguidor < 0)
                    {
                        this._circuitos[i].BackColor = Color.FromArgb(6, 176, 37);
                    }
                    else if (this._colorantes[i].Habilitado && this._colorantes[i].Seguidor > 0)
                    {
                        this._circuitos[i].BackColor = Cores.Seguidor_Tom_01;
                        this._toolTipProducts.SetToolTip(this._circuitos[i], "Seguidor do Circuito: " + _colorantes[i].Seguidor.ToString());
                    }
                    else
                    {
                        this._circuitos[i].BackColor = Color.Red;
                    }
                }
            }

        }

        private void fPurgaIndividual_FormClosed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();
            

            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_19 + Negocio.IdiomaResxExtensao.Global_PurgaIndividualConcluida);

                        //Persiste data e hora de execução da purga
                        //Parametros.SetExecucaoPurga(DateTime.Now);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_20 + Negocio.IdiomaResxExtensao.Global_PurgaIndividualCancelada);
                        break;
                    }
              
                case DialogResult.No:
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_21 + Negocio.IdiomaResxExtensao.Global_FalhaPurgarIndividual);
                        break;
                    }

                    #endregion
            }
            if (_parametros.ViewMessageProc)
            {
                this.Invoke(new MethodInvoker(ClosePrg));
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (this.execPurga)
            {
                gerarEventoPurgaIndividual(2);
                string msg = Negocio.IdiomaResxExtensao.Log_Cod_23 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualAbortada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome ,_prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
            }
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Circuito_Click(object sender, EventArgs e)
        {
            try
            {
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(Wait_Message));
                }
                this.execPurga = true;
                int circuito = int.Parse(((Button)sender).Tag.ToString()) +1;
                bool isSeguidor = false;

                for(int i = 0; i < _prmPurga.Colorantes.Count; i++)
                {
                    if(circuito == _prmPurga.Colorantes[i].Circuito)
                    {
                        if(_prmPurga.Colorantes[i].Seguidor > 0)
                        {
                            isSeguidor = true;
                        }
                        this.INDEX_CKT = i;
                        this.INDEX_DISP = _prmPurga.Colorantes[i].Dispositivo - 1;
                    }
                }

                if (!isSeguidor)
                {

                    if (_parametros.InicializarCircuitosPurgaIndividual)
                    {
                        this.Visible = false;

                        if (_prmPurga.DispenserP3 == null)
                        {
                            IDispenser _disp = _prmPurga.Dispenser[0];
                            if (Operar.TemRecipiente(_disp))
                            {
                                IDispenser dispenser = null;
                                dispenser = _prmPurga.Dispenser[_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo - 1];

                                string mensagemUsuario = "";
                                if (!Operar.Conectar(ref dispenser))
                                {
                                    execPurga = false;
                                    mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                    {
                                        m.ShowDialog(mensagemUsuario, true);
                                    }

                                }
                                if (execPurga)
                                {
                                    int ncirc = this.INDEX_CKT + 1;
                                    if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                                    {
                                        if ((Dispositivo2)this._parametros.IdDispositivo2 == Dispositivo2.Placa_2)
                                        {
                                            ncirc = ncirc - 16;
                                        }
                                        else
                                        {
                                            ncirc = ncirc - 24;
                                        }
                                    }
                                    if (!ExecutarInicializacaoColorantes(dispenser, ncirc, _prmPurga.Colorantes[this.INDEX_CKT].Dispositivo))
                                    {
                                        Log.Logar(
                                            TipoLog.Processo,
                                            _parametros.PathLogProcessoDispensa,
                                            Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);
                                        mensagemUsuario = Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida;
                                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                        {
                                            m.ShowDialog(mensagemUsuario, true);
                                        }
                                        execPurga = false;
                                    }
                                }
                            }
                            else
                            {
                                execPurga = false;
                            }
                        }
                        else
                        {
                            string mensagemUsuario = "";
                            ModBusDispenserMover_P3 p3 = _prmPurga.DispenserP3;
                            if (!Operar.ConectarP3(ref p3))
                            {
                                execPurga = false;
                                mensagemUsuario = Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao;
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    m.ShowDialog(mensagemUsuario, true);
                                }

                            }
                            if (execPurga)
                            {
                                int ncirc = this.INDEX_CKT + 1;
                                if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 1)
                                {
                                    if (!ExecutarInicializacaoColorantes(p3, ncirc, _prmPurga.Colorantes[this.INDEX_CKT].Dispositivo))
                                    {
                                        Log.Logar(
                                            TipoLog.Processo,
                                            _parametros.PathLogProcessoDispensa,
                                            Negocio.IdiomaResxExtensao.Log_Cod_09 + Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida);
                                        mensagemUsuario = Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida;
                                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                        {
                                            m.ShowDialog(mensagemUsuario, true);
                                        }
                                        execPurga = false;
                                    }
                                }
                            }

                        }
                    }
                    this.Visible = true;
                    if (execPurga)
                    {
                        this.tipoActionP3 = -1;
                        this.passosActionP3 = 0;
                        if (_prmPurga.DispenserP3 != null)
                        {
                            bool isPosicao = false;
                            _prmPurga.DispenserP3.ReadSensores_Mover();
                            if (!(_prmPurga.DispenserP3.IsNativo == 0 || _prmPurga.DispenserP3.IsNativo == 2))
                            {
                                //lblStatus.Text = "Condição de placa movimentação incorreta!";
                                lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                                lblSubStatus.Text = "";
                                return;
                            }
                            if (!_prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                _prmPurga.DispenserP3.AbrirGaveta(true);
                                for (int _i = 0; _i < 20; _i++)
                                {
                                    if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                                    {
                                        _i = 20;
                                    }
                                    Thread.Sleep(500);
                                }
                                //Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                isPosicao = true;
                            }
                            if (!_prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
                            {
                                _prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                                isPosicao = true;
                            }
                            if (isPosicao)
                            {
                                _prmPurga.DispenserP3.ReadSensores_Mover();
                                if ((!_prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada) || (!_prmPurga.DispenserP3.SensorValvulaDosagem && !_prmPurga.DispenserP3.SensorValvulaRecirculacao))
                                {
                                    //lblStatus.Text = "Condição de placa movimentação incorreta!";
                                    lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                                    lblSubStatus.Text = "";
                                    return;
                                }
                            }
                            this.tipoActionP3 = getActionP3();
                        }
                        else
                        {
                            IDispenser _disp = _prmPurga.Dispenser[0];
                            if (Operar.TemRecipiente(_disp))
                            {
                                this.tipoActionP3 = 0;
                            }
                        }
                        if (this.tipoActionP3 >= 0)
                        {
                            this.INDEX_ULTIMO_CKT = this.INDEX_CKT;
                            this.index_PurgaIndividual = this.INDEX_CKT;
                            gbIndividual.Enabled = false;

                            progressBar.Visible = true;
                            this.Invoke(new MethodInvoker(ExecutarMonitoramento));
                        }
                    }
                }
            }
            catch
            { }

        }

        private int getActionP3()
        {
            int retorno = -1;
            if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
                && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
                && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && _prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && _prmPurga.DispenserP3.SensorBaixoBicos && !_prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
                && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
                && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
               && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && !_prmPurga.DispenserP3.SensorGavetaAberta && _prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
               && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
               && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!_prmPurga.DispenserP3.SensorEsponja && !_prmPurga.DispenserP3.SensorCopo && !_prmPurga.DispenserP3.SensorBaixoBicos && _prmPurga.DispenserP3.SensorAltoBicos
               && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
               && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (_prmPurga.DispenserP3.SensorBaixoBicos && !_prmPurga.DispenserP3.SensorAltoBicos
               && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && _prmPurga.DispenserP3.SensorValvulaDosagem
               && !_prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (_prmPurga.DispenserP3.SensorBaixoBicos && !_prmPurga.DispenserP3.SensorAltoBicos
              && _prmPurga.DispenserP3.SensorGavetaAberta && !_prmPurga.DispenserP3.SensorGavetaFechada && !_prmPurga.DispenserP3.SensorValvulaDosagem
              && _prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }
            else if (this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo && this._prmPurga.DispenserP3.SensorBaixoBicos &&
            !this._prmPurga.DispenserP3.SensorAltoBicos && !this._prmPurga.DispenserP3.SensorGavetaAberta && this._prmPurga.DispenserP3.SensorGavetaFechada &&
            this._prmPurga.DispenserP3.SensorValvulaDosagem && !this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo && !this._prmPurga.DispenserP3.SensorBaixoBicos &&
              this._prmPurga.DispenserP3.SensorAltoBicos && !this._prmPurga.DispenserP3.SensorGavetaAberta && this._prmPurga.DispenserP3.SensorGavetaFechada &&
              this._prmPurga.DispenserP3.SensorValvulaDosagem && !this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo && !this._prmPurga.DispenserP3.SensorBaixoBicos &&
              this._prmPurga.DispenserP3.SensorAltoBicos && this._prmPurga.DispenserP3.SensorGavetaAberta && !this._prmPurga.DispenserP3.SensorGavetaFechada &&
              this._prmPurga.DispenserP3.SensorValvulaDosagem && !this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }

            return retorno;
        }

        bool ExecutarInicializacaoColorantes(IDispenser dispenser, int nCircuito, int dispositivo)
        {
            InicializacaoCircuitosVO param_ini = new InicializacaoCircuitosVO();
            param_ini.Dispenser = new List<IDispenser>();
            param_ini.Dispenser.Add(dispenser);

            //Recupera circuitos de colorantes habilitados
            param_ini.Circuitos = new int[] { nCircuito };
            //   _colorantes.Where(c => c.Habilitado == true).Select(c => c.Circuito).ToArray();

            param_ini.Dispositivo = new int[] { dispositivo };

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

            return (result == DialogResult.OK);
        }

        bool ExecutarInicializacaoColorantes(ModBusDispenserMover_P3 dispenser, int nCircuito, int dispositivo)
        {
            InicializacaoCircuitosVO param_ini =
                 new InicializacaoCircuitosVO();
            param_ini.Dispenser = new List<IDispenser>();
            param_ini.DispenserP3 = dispenser;

            //Recupera circuitos de colorantes habilitados
            param_ini.Circuitos = new int[] { nCircuito };
            //   _colorantes.Where(c => c.Habilitado == true).Select(c => c.Circuito).ToArray();

            param_ini.Dispositivo = new int[] { dispositivo };

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

            return (result == DialogResult.OK);
        }
               
        #region Métodos privados

        void Falha(Exception ex)
        {
            PausarMonitoramento();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
                    Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message, true);
            }
            gerarEventoPurgaIndividual(3);
            DialogResult = DialogResult.No;
        }

        void Emergencia()
        {
            PausarMonitoramento();
            try
            {
                this._prmPurga.DispenserP3.Halt();
                this._prmPurga.DispenserP3.UnHalt();
            }
            catch
            { }

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                //m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + "Botão de Emergência pressionado!");
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia, true);
            }

            
        }

        void DispensaForceHalt()
        {
            PausarMonitoramento();
            try
            {
                this._prmPurga.DispenserP3.Halt();
                this._prmPurga.DispenserP3.UnHalt();
            }
            catch
            { }

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                //m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + "Dispensa parou!");
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Disp_Parou, true);
            }

            DialogResult = DialogResult.No;
        }

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                this.isRunning = true;
                progressBar.Visible = false;                
                lblStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividualMonit_lblStatus_Msg01;
                lblSubStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividual_lblSubStatus_Msg01;
            }
            catch
            {
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
                    this.isRunning = false;
                    this.backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    
                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                    this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    this.backgroundWorker1.WorkerSupportsCancellation = true;

                    if (!backgroundWorker1.IsBusy)
                    {
                        this.isThread = true;
                        this.isRunning = false;
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividualMonit_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.PurgarIndividual_lblSubStatus_Msg01;
                        gbIndividual.Enabled = true;
                    }
                }
            }
            catch
            {
                gbIndividual.Enabled = true;
            }
        }

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
                        try
                        {
                            if (!this.isRunning)
                            {
                                this.isRunning = true;
                                this.Invoke(new MethodInvoker(MonitoramentoEvent));
                                if(this._prmPurga.DispenserP3==null)
                                {
                                    Thread.Sleep(500);
                                }

                            }
                        }
                        catch
                        {
                        }
                    }
                    Thread.Sleep(500);
                }

                this.Invoke(new MethodInvoker(atualizaGridEnable));
            }
            catch
            {
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
            catch
            {
            }
        }
        
        private void atualizaGridEnable()
        {
            try
            {
                if (!gbIndividual.Enabled)
                {
                    Thread.Sleep(2000);
                    gbIndividual.Enabled = true;
                }
                
                if (_parametros.ViewMessageProc)
                {
                    this.Invoke(new MethodInvoker(ClosePrg));
                }
            }
            catch
            {

            }
        }

        #endregion

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.execPurga)
                {
                    string msg = string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualAbortada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome , _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                    Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                    gerarEventoPurgaIndividual(1);
                }
                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {

            }
        }

        void MonitoramentoEvent()
        {
            try
            {
                if (_prmPurga.DispenserP3 != null)
                {
                    trataActionP3();
                    this.isRunning = false;
                }
                else
                {
                    //Verifica se dispenser está pronto
                    if (!_prmPurga.Dispenser[this.INDEX_DISP].IsReady)
                    {
                        this.isRunning = false;
                        return;
                    }

                    //Verifica se já executou todos os circuitos de colorantes]
                    if (INDEX_CKT > INDEX_ULTIMO_CKT)
                    {
                        /*
                        if (this.isThread)
                        {
                            Log.Logar(
                                    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_PurgaIndividualConcluida);
                        }
                        */
                        #region Gerar Evento Purga
                        
                        string detalhes_purga = _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito.ToString() + "," + _prmPurga.Colorantes[this.index_PurgaIndividual].Nome +
                            "," + Math.Round(_prmPurga.Colorantes[this.index_PurgaIndividual].VolumePurga, 3).ToString();                        
                        gerarEventoPurgaIndividual(0, detalhes_purga);
                        
                       
                        #endregion
                        this.isRunning = true;
                        this.execPurga = false;
                        string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                        PausarMonitoramento();
                        //DialogResult = DialogResult.OK;
                        return;
                    }
                    //Recupera posição do circuito do colorante
                    int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                    List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
                    List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == CIRCUITO).ToList();
                    

                    if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                    {
                        if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                        {
                            CIRCUITO = CIRCUITO - 24;
                        }
                        else
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                            
                    }
                    //int CIRCUITO = this.INDEX_CKT;
                    //Atualiza interface
                    lblStatus.Text =
                        Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                    lblSubStatus.Text =
                        Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                    if (ncol != null && ncol.Count > 0)
                    {
                        int[] n_motor = new int[ncol.Count + 1];

                        n_motor[0] = CIRCUITO - 1;
                        for (int i = 0; i < ncol.Count; i++)
                        {
                            if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                            {
                                //if ((Dispositivo2)this._parametros.IdDispositivo2 == Dispositivo2.Placa_2)
                                //{
                                //    n_motor[i + 1] = ((ncol[i].Circuito - 16) - 1);
                                //}
                                //else
                                //{
                                //    n_motor[i + 1] = ((ncol[i].Circuito - 24) - 1);
                                //}
                                if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                                {
                                    n_motor[i + 1] = ((ncol[i].Circuito - 16) - 1); 

                                }
                                else
                                {
                                    n_motor[i + 1] = ((ncol[i].Circuito - 16) - 1);
                                }
                            }                            
                            else
                            {
                                n_motor[i + 1] = (ncol[i].Circuito - 1);
                            }
                        }
                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.Dispenser[this.INDEX_DISP].Dispensar(
                                n_motor,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );
                    }
                    else
                    {
                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.Dispenser[this.INDEX_DISP].Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );
                    }
                    if (_parametros.ControlarNivel)
                    {
                        //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                    }
                    //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);

                    Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                   
                    //Incrementa contador
                    this.INDEX_CKT++;
                    this.isRunning = false;
                }

            }
            catch (Exception ex)
            {
                Falha(ex);
            }
        }

        int gerarEventoPurgaIndividual(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Purga Individual
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.PurgaIndividual;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
            catch
            { }
            return retorno;
        }


        private void Wait_Message()
        {
            try
            {
                if (this._fAguarde == null)
                {
                    this._fAguarde = new fAguarde(Negocio.IdiomaResxExtensao.Global_Aguarde_ProgressBar);
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
                //this.Invoke(new MethodInvoker(WaitIsrunning));
            }
            catch
            {
            }
        }

        private void ClosedProgressBar()
        {
            try
            {
                this._fAguarde = null;
            }
            catch
            { }
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
            catch
            { }
        }

        void trataActionP3()
        {
            try
            {
                switch (this.tipoActionP3)
                {
                    case 1:
                        {
                            trataPassosAction_01();
                            break;
                        }
                    case 2:
                        {
                            trataPassosAction_02();
                            break;
                        }
                    case 3:
                        {
                            trataPassosAction_03();
                            break;
                        }
                    case 4:
                        {
                            trataPassosAction_04();
                            break;
                        }
                    case 5:
                        {
                            trataPassosAction_05();
                            break;
                        }
                    case 6:
                        {
                            trataPassosAction_06();
                            break;
                        }
                    case 7:
                        {
                            trataPassosAction_07();
                            break;
                        }
                    case 8:
                        {
                            trataPassosAction_08();
                            break;
                        }
                    case 9:
                        {
                            trataPassosAction_09();
                            break;
                        }
                    case 10:
                        {
                            trataPassosAction_10();
                            break;
                        }
                    case 11:
                        {
                            trataPassosAction_11();
                            break;
                        }
                    case 12:
                        {
                            trataPassosAction_12();
                            break;
                        }
                    case 13:
                        {
                            trataPassosAction_13();
                            break;
                        }
                    case 14:
                        {
                            trataPassosAction_14();
                            break;
                        }
                    case 15:
                        {
                            trataPassosAction_15();
                            break;
                        }
                    case 16:
                        {
                            trataPassosAction_16();
                            break;
                        }
                    default:
                        {
                            break;
                        }


                }

            }
            catch
            { }
        }

        void trataPassosAction_01()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Dosador", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta();
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {                        
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {  
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;

                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                      
                        //Incrementa contador
                        this.INDEX_CKT++;

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                           

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_02()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Dosador", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta();
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Movimentar Válvula Recuar Bicos
                                //this.passosActionP3 = 18;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {                     
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 10;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_03()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta();
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this._prmPurga.DispenserP3.ReadSensores_Mover();
                                if (this._prmPurga.DispenserP3.SensorCopo && this._prmPurga.DispenserP3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    this.passosActionP3 = 20;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if(this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                      
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();                         

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        //this._prmPurga.DispenserP3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_04()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isRec = m.ShowDialog("Recipiente está na Gaveta", "Yes", "No");
                            bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                            if (!isRec)
                            {
                                //this.passosActionP3 = 1;
                                //this._prmPurga.DispenserP3.AbrirGaveta();
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this._prmPurga.DispenserP3.ReadSensores_Mover();
                                if (this._prmPurga.DispenserP3.SensorCopo && this._prmPurga.DispenserP3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    this.passosActionP3 = 20;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        //this._prmPurga.DispenserP3.FecharGaveta();
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                      
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_05()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        //this._prmPurga.DispenserP3.FecharGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {                        
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_06()
        {

            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {

                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_07()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {

                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                      
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {

                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && 
                                this._prmPurga.DispenserP3.SensorGavetaFechada && this._prmPurga.DispenserP3.SensorBaixoBicos)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || !this._prmPurga.DispenserP3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_08()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    ////m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();                          

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_09()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();                          

                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {

                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }

        }

        void trataPassosAction_10()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 3;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_11()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //m.ShowDialog("Trocar recipiente!");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                this.passosActionP3 = 3;
                            }
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                      
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();                            
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 2;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_12()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 2;
                        this.counterFalha = 0;
                        //this.passosActionP3 = 18;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isAct = m.ShowDialog("Posicionar recipiente nos bicos!");
                            bool isAct = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico, true);
                            if (isAct)
                            {
                                this.passosActionP3 = 6;
                            }
                            else
                            {
                                this.passosActionP3 = 12;
                            }

                        }

                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 3;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_13()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 2;
                        this.counterFalha = 0;
                        //this.passosActionP3 = 18;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //bool isAct = m.ShowDialog("Posicionar recipiente nos bicos!");
                            bool isAct = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico, true);
                            if (isAct)
                            {
                                //this.passosActionP3 = 18;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else
                            {
                                this.passosActionP3 = 12;
                            }

                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || (!this.isNewOperacao && !this._prmPurga.DispenserP3.SensorBaixoBicos)))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 6;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_14()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada && this._prmPurga.DispenserP3.SensorBaixoBicos)
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;                                
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || !this._prmPurga.DispenserP3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_15()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 1;
                        this.counterFalha = 0;
                        //this._prmPurga.DispenserP3.AbrirGaveta();
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                      
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada && this._prmPurga.DispenserP3.SensorBaixoBicos)
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;                                
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || !this._prmPurga.DispenserP3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {                        
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        void trataPassosAction_16()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.qtdTrocaRecipiente = 0;
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            //this.passosActionP3 = 1;
                            //this._prmPurga.DispenserP3.AbrirGaveta();
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 2;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (!this._prmPurga.DispenserP3.SensorCopo || !this._prmPurga.DispenserP3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            this.passosActionP3 = 4;
                        }
                        //Aqui solicitar copo da esponja
                        if (this.qtdTrocaRecipiente > 3)
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 4:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (_prmPurga.DispenserP3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this._prmPurga.DispenserP3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!_prmPurga.DispenserP3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        //Verifica se já executou todos os circuitos de colorantes]
                        if (INDEX_CKT > INDEX_ULTIMO_CKT)
                        {
                            string msg = Negocio.IdiomaResxExtensao.Log_Cod_22 + string.Format(Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada, _prmPurga.Colorantes[this.index_PurgaIndividual].Nome, _prmPurga.Colorantes[this.index_PurgaIndividual].Circuito);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                            //this.passosActionP3 = 8;
                            this.counterFalha = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task1 = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 9;
                            //DialogResult = DialogResult.OK;
                            break;
                        }
                        //Recupera posição do circuito do colorante
                        int CIRCUITO = _prmPurga.Colorantes[this.INDEX_CKT].Circuito;
                        if (_prmPurga.Colorantes[this.INDEX_CKT].Dispositivo == 2)
                        {
                            CIRCUITO = CIRCUITO - 16;
                        }
                        //int CIRCUITO = this.INDEX_CKT;
                        //Atualiza interface
                        lblStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 + " " + _prmPurga.Colorantes[this.INDEX_CKT].Nome;
                        lblSubStatus.Text =
                            Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 + " " + CIRCUITO.ToString("00");

                        //Dispara thread para enviar dados ao dispositivo
                        Task task = Task.Factory.StartNew(() => _prmPurga.DispenserP3.Dispensar(
                                CIRCUITO - 1,
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoHorario[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Velocidade[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Aceleracao[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].Delay[CIRCUITO - 1],
                                _prmPurga.LMDispositivos[this.INDEX_DISP].PulsoReverso[CIRCUITO - 1], i_Step)
                            );

                        if (_parametros.ControlarNivel)
                        {
                            //Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                            Operar.AbaterColorante(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                        }
                        //Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.Volume);
                        Util.ObjectRecircular.UpdateVolumeDosado(_prmPurga.Colorantes[this.INDEX_CKT].Circuito, _prmPurga.LMDispositivos[this.INDEX_DISP].VolumeDosado[CIRCUITO - 1]);
                       
                        //Incrementa contador
                        this.INDEX_CKT++;
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        //this._prmPurga.DispenserP3.AbrirGaveta(true);
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                !this._prmPurga.DispenserP3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 = 9;
                        }
                        break;
                    }
                case 11:
                    {
                        bool isNewPass = false;
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //isNewPass = m.ShowDialog("Usuário deseja realizar outra operação?", "Yes", "No");
                            isNewPass = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true);
                        }
                        if (isNewPass)
                        {
                            this.isNewOperacao = true;
                        }
                        else
                        {
                            this.isNewOperacao = false;
                        }
                        this.passosActionP3 = 12;

                        break;
                    }
                case 12:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            if (this.isNewOperacao)
                            {
                                //m.ShowDialog("Usuário utilize o copo para nova Dispensa");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa, true);
                            }
                            else
                            {
                                //m.ShowDialog("Usuário utilize o copo da Esponja ");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja, true);
                            }
                            this.passosActionP3 = 13;
                        }

                        break;
                    }
                case 13:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this._prmPurga.DispenserP3.SensorEsponja && !this._prmPurga.DispenserP3.SensorCopo) || (this.isNewOperacao && this._prmPurga.DispenserP3.SensorCopo))
                        {
                            this.passosActionP3 = 14;
                        }
                        else
                        {
                            this.passosActionP3 = 12;
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        if (!this.isNewOperacao)
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada && this._prmPurga.DispenserP3.SensorBaixoBicos)
                            {
                                this.passosActionP3 = 16;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada || !this._prmPurga.DispenserP3.SensorBaixoBicos))
                            {
                                this.passosActionP3 = 14;
                            }
                        }
                        break;
                    }
                case 16:
                    {
                        if (!this.isNewOperacao)
                        {
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                            this._prmPurga.DispenserP3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                        }                        
                        break;
                    }
                case 17:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                        }
                        else
                        {
                            this.passosActionP3 = 16;
                        }
                        break;
                    }
                case 18:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this._prmPurga.DispenserP3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this._prmPurga.DispenserP3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this._prmPurga.DispenserP3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) && !this._prmPurga.DispenserP3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this._prmPurga.DispenserP3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this._prmPurga.DispenserP3.TerminouProcessoDuplo)
                        {
                            this._prmPurga.DispenserP3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this._prmPurga.DispenserP3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                this._prmPurga.DispenserP3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this._prmPurga.DispenserP3.IsNativo == 0 || this._prmPurga.DispenserP3.IsNativo == 2) &&
                                (!this._prmPurga.DispenserP3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 20;
                            }
                        }
                        break;
                    }
            }
        }

        private bool btEmergenciaCodErro(int passosActionP3Emergencia, int passosActionP3CodError)
        {
            bool retorno = false;
            if (this._prmPurga.DispenserP3.SensorEmergencia)
            {
                retorno = true;
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    //bool isRec = m.ShowDialog("Usuário Botão de Emergência Pressionado. Deseja finalizar este processo?", "Yes", "No", true, 60);
                    bool isRec = m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia_Passos, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 60);
                    if (isRec)
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
                    }
                    else
                    {
                        this.passosActionP3 = passosActionP3Emergencia;
                    }
                }

            }
            else if (this._prmPurga.DispenserP3.CodError > 0)
            {
                retorno = true;
                if (this.counterFalha >= 2)
                {
                    PausarMonitoramento();
                    Thread.Sleep(1000);
                    DialogResult = DialogResult.No;
                    Close();
                }

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    //string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this._prmPurga.DispenserP3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this._prmPurga.DispenserP3.GetDescCodError();
                    bool isRec = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao, true, 30);
                    if (isRec)
                    {
                        this.counterFalha++;
                        this.passosActionP3 = passosActionP3CodError;
                    }
                    else
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
                    }
                }
            }
            return retorno;
        }
    }
}
