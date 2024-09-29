using Percolore.Core;
using Percolore.Core.Logging;
using System.ComponentModel;
using System.Text;

namespace Percolore.IOConnect
{
	public partial class fDispensaSimultanea : Form
    {
        Util.ObjectParametros _parametros;
        List<IDispenser> _dispenser;
        private ModBusDispenserMover_P3 modBusDispenser_P3 = null;
        List<ValoresVO[]> _valores;
        List<ValoresVO[]> _valores2;
        Dictionary<int, double> _demanda;
        string _descCor = string.Empty;
        string _codigoCor { get; set; }
        readonly bool desativarUI;

        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private bool isThread = false;
        private bool isRunning = false;
        private bool placa1Seq = true;
        private bool placa2Seq = true;

        int counterP1 = 0;
        int counterUltimoP1 = 1;
        int counterP1_arr = 0;

        int counterP2 = 0;
        int counterUltimoP2 = 0;

        int counterP2_arr = 0;

        private List<Util.ObjectColorante> colorantes = new List<Util.ObjectColorante>();

        private bool existe_Base_P1 = false;
        private bool existe_Base_P2 = false;

        private bool terminou_Base_P1 = false;
        private bool terminou_Base_P2 = false;


        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;
        private int qtdTrocaRecipiente = 0;
        private int counterFalha = 0;
        private bool isNewOperacao = false;
        private int thread_Sleep = 500;

        private int counterFalhaConexao = 0;

        private int i_Step = 0;

        private bool isDatUDCP = false;

        public fDispensaSimultanea(
            List<IDispenser> dispenser, List<ValoresVO[]> valores, List<ValoresVO[]> valores2,
            Dictionary<int, double> demanda, List<Util.ObjectColorante> colorantesSim, string descCor, bool desativarUI = false, string codigoCor="", 
            ModBusDispenserMover_P3 _modBusDispenser_P3 = null, bool tembaseP1 = false, bool temBaseP2 = false)
        {
            InitializeComponent();

            this._parametros = Util.ObjectParametros.Load();
            this._dispenser = dispenser;
            this._valores = valores;
            this._valores2 = valores2;
            this._demanda = demanda;
            this._descCor = descCor;
            this._codigoCor = codigoCor;
            this.colorantes = colorantesSim;
            this.modBusDispenser_P3 = _modBusDispenser_P3;

            this.existe_Base_P1 = tembaseP1;
            this.existe_Base_P2 = temBaseP2;

            this.placa1Seq = this._parametros.HabilitarDispensaSequencialP1;
            this.placa2Seq = this._parametros.HabilitarDispensaSequencialP2;

            /* Se a exibição da interface for desabilitada
             * não é necessário configurar propriedades dos controles */
            this.desativarUI = desativarUI;

            switch ((DatPattern)_parametros.PadraoConteudoDAT)
            {
                case DatPattern.PadraoUDCP:
                    {
                        this.isDatUDCP = true;
                        break;
                    }
            }
             
            if (this.desativarUI)
            {
                return;
            }

            //Redimensiona e posiciona
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            btnIniciar.Text = Negocio.IdiomaResxExtensao.Global_Iniciar;
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            btnAbortar.Text = Negocio.IdiomaResxExtensao.Global_Abortar;
            lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
            lblSubStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblSubStatus_Msg01;

            //Habilitar controle
            lblSubStatus.Visible = true;
            progressBar.Visible = false;
            btnIniciar.Enabled = true;
            btnCancelar.Enabled = true;
            btnAbortar.Visible = false;

            try
            {
                richTextBox1.Clear();
                if (this._valores != null && this._valores.Count > 0)
                {
                    for (int i = 0; i < this._valores.Count; i++)
                    {
                        ValoresVO[] vl_ = this._valores[i];
                        for (int j = 0; j < vl_.Length; j++)
                        {
                            ValoresVO vl = vl_[j];
                            if (vl != null)
                            {
                                richTextBox1.AppendText("Color.:" + descCor + Environment.NewLine);
                                richTextBox1.AppendText("Acc:" + vl.Aceleracao + Environment.NewLine);
                                richTextBox1.AppendText("Delay:" + vl.Delay + Environment.NewLine);
                                richTextBox1.AppendText("DesvioMedio:" + vl.DesvioMedio + Environment.NewLine);
                                richTextBox1.AppendText("MassaIdeal:" + vl.MassaIdeal + Environment.NewLine);
                                richTextBox1.AppendText("MassaMedia:" + vl.MassaMedia + Environment.NewLine);
                                richTextBox1.AppendText("PulsoHorario:" + vl.PulsoHorario + Environment.NewLine);
                                richTextBox1.AppendText("PulsoReverso:" + vl.PulsoReverso + Environment.NewLine);
                                richTextBox1.AppendText("Velocidade:" + vl.Velocidade + Environment.NewLine);
                                richTextBox1.AppendText("Volume:" + vl.Volume + Environment.NewLine + Environment.NewLine);
                            }
                        }
                    }
                }

                if (this._valores2 != null && this._valores2.Count > 0)
                {
                    for (int i = 0; i < this._valores2.Count; i++)
                    {
                        ValoresVO[] vl_ = this._valores2[i];
                        for (int j = 0; j < vl_.Length; j++)
                        {
                            ValoresVO vl = vl_[j];
                            if (vl != null)
                            {
                                richTextBox1.AppendText("Color.:" + descCor + Environment.NewLine);
                                richTextBox1.AppendText("Acc:" + vl.Aceleracao + Environment.NewLine);
                                richTextBox1.AppendText("Delay:" + vl.Delay + Environment.NewLine);
                                richTextBox1.AppendText("DesvioMedio:" + vl.DesvioMedio + Environment.NewLine);
                                richTextBox1.AppendText("MassaIdeal:" + vl.MassaIdeal + Environment.NewLine);
                                richTextBox1.AppendText("MassaMedia:" + vl.MassaMedia + Environment.NewLine);
                                richTextBox1.AppendText("PulsoHorario:" + vl.PulsoHorario + Environment.NewLine);
                                richTextBox1.AppendText("PulsoReverso:" + vl.PulsoReverso + Environment.NewLine);
                                richTextBox1.AppendText("Velocidade:" + vl.Velocidade + Environment.NewLine);
                                richTextBox1.AppendText("Volume:" + vl.Volume + Environment.NewLine + Environment.NewLine);
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

        #region Eventos

        void FormLoad(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
             * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = !desativarUI;
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
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			/* Se a exibição da interface for desabilitada,
             * reduz form a tamanho zero e incia automaticamente a dispensa */
			if (desativarUI)
            {
                this.Width = 0;
                this.Height = 0;
                Iniciar_Click(sender, e);
            }
        }

        void Iniciar_Click(object sender, EventArgs e)
        {
            lblSubStatus.Visible = false;
            progressBar.Visible = true;
            progressBar.Visible = true;
            btnIniciar.Enabled = false;
            btnCancelar.Visible = false;
            btnAbortar.Visible = true;
            lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg02;
            this.counterUltimoP1 = this._valores.Count;
            this.counterUltimoP2 = (this._valores2 != null && this._valores2.Count > 0) ? this._valores2.Count : 0;
            

           
            try
            {
                RessetarTempoMonitoramentoCircuitos();
                this.tipoActionP3 = -1;
                this.passosActionP3 = 0;
                if (this.modBusDispenser_P3 != null)
                {
                    bool isPosicao = false;
                    this.modBusDispenser_P3.ReadSensores_Mover();
                    if (!(this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2))
                    {
                        //lblStatus.Text = "Condição de placa movimentação incorreta!";
                        lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                        lblSubStatus.Text = "";
                        return;
                    }

                    if (!this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada)
                    {
                        this.modBusDispenser_P3.AbrirGaveta(true);
                        for (int _i = 0; _i < 20; _i++)
                        {
                            if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                            {
                                _i = 20;
                            }
                            Thread.Sleep(500);
                        }
                        isPosicao = true;
                    }
                    if (!this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
                    {
                        this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                        isPosicao = true;
                    }
                    if (isPosicao)
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if ((!this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada) || (!this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao))
                        {
                            //lblStatus.Text = "Condição de placa movimentação incorreta!";
                            lblStatus.Text = Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta;
                            lblSubStatus.Text = "";
                            return;
                        }
                    }

                    this.tipoActionP3 = getActionP3();
                    if(this.tipoActionP3 >= 0)
                    {
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, "Dispensa Simultanea P3 tratamento:" + this.tipoActionP3.ToString());
                    }
                    lblSubStatus.Text = "";
                    lblSubStatus.Visible = true;
                    this.thread_Sleep = 500;
                }
                else
                {
                    this.tipoActionP3 = 0;
                }
                if (this.tipoActionP3 >= 0)
                {
                                    
                    ExecutarMonitoramento();
                }
                Log.Logar(
                    TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_ProcessoDispensaIniciado);
            }
            catch (Exception ex)
            {
                Falha(ex);
            }
        }

        private int getActionP3()
        {
            int retorno = -1;
            if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
                && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 1;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 2;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
                && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 3;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 4;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorBaixoBicos && !this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 5;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 6;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
                && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
                && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 7;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 8;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
               && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 9;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 10;
            }
            else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos && this.modBusDispenser_P3.SensorAltoBicos
               && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
               && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 11;
            }
            else if (this.modBusDispenser_P3.SensorBaixoBicos && !this.modBusDispenser_P3.SensorAltoBicos
               && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && this.modBusDispenser_P3.SensorValvulaDosagem
               && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 12;
            }
            else if (this.modBusDispenser_P3.SensorBaixoBicos && !this.modBusDispenser_P3.SensorAltoBicos
              && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada && !this.modBusDispenser_P3.SensorValvulaDosagem
              && this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 13;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorBaixoBicos &&
                !this.modBusDispenser_P3.SensorAltoBicos && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada &&
                this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 14;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos &&
              this.modBusDispenser_P3.SensorAltoBicos && !this.modBusDispenser_P3.SensorGavetaAberta && this.modBusDispenser_P3.SensorGavetaFechada &&
              this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 15;
            }
            else if (this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo && !this.modBusDispenser_P3.SensorBaixoBicos &&
              this.modBusDispenser_P3.SensorAltoBicos && this.modBusDispenser_P3.SensorGavetaAberta && !this.modBusDispenser_P3.SensorGavetaFechada &&
              this.modBusDispenser_P3.SensorValvulaDosagem && !this.modBusDispenser_P3.SensorValvulaRecirculacao)
            {
                retorno = 16;
            }
            return retorno;
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        void Abortar_Click(object sender, EventArgs e)
        {
            try
            {
                PausarMonitoramento();

                //Interrompe e retoma operação da placa
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; _dispenser != null && i < _dispenser.Count; i++)
                    {
                        _dispenser[i].Halt();
                        _dispenser[i].UnHalt();
                    }
                    Thread.Sleep(1000);
                    if (_dispenser.Count > 1)
                    {
                        if (_dispenser[0].IsReady && _dispenser[1].IsReady)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_dispenser[0].IsReady)
                        {
                            break;
                        }
                    }
                }
            
                if (modBusDispenser_P3 != null)
                {
                    modBusDispenser_P3.Halt();
                    modBusDispenser_P3.UnHalt();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			//Status do processo
			DialogResult = DialogResult.Abort;
        }
        
        void Monitoramento_Event()
        {
            try
            {               
                if (modBusDispenser_P3 != null)
                {
                    trataActionP3();
                }
                else
                {
                    /* Verifica se dispenser está liberado.
                    * isso representa que o processo foi concluído. */
                    for (int i = 0; _dispenser != null && i < _dispenser.Count; i++)
                    {
                        if (!_dispenser[i].IsReady)
                        {
                            this.isRunning = false;
                            return;
                        }
                    }

                  
                    if (((counterP1 >= counterUltimoP1 && existe_Base_P1 && terminou_Base_P1) || (counterP1 >= counterUltimoP1 && !existe_Base_P1)) && 
                        ((counterP2 >= counterUltimoP2 && existe_Base_P2 && terminou_Base_P2) || (counterP2 >= counterUltimoP2 && !existe_Base_P2)))
                    {
                        if (_dispenser[0].IsReady)
                        {
                            this.isRunning = false;
                            PausarMonitoramento();

                            List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();
                            if (this._valores != null && this._valores.Count > 0)
                            {
                                for (int i = 0; i < this._valores.Count; i++)
                                {   
                                    ValoresVO[] vl_ = this._valores[i];
                                   
                                    for (int j = 0; j < vl_.Length; j++)
                                    {
                                        bool permiteArmazenar_nivel = true;
                                        
                                        if (lCol != null && lCol.Count > 0)
                                        {
                                            Util.ObjectColorante nCol = lCol.Find(o => o.Circuito == j+1);
                                            if (nCol != null)
                                            {
                                                permiteArmazenar_nivel = false;
                                            }
                                        }

                                        if (permiteArmazenar_nivel)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(j + 1, vl_[j].PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(j + 1, vl_[j].Volume);

                                            }
                                        }
                                    }
                                }
                            }

                            if (this._valores2 != null && this._valores2.Count > 0)
                            {
                                int index_dec2 = 16;
                                if((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                                {
                                    index_dec2 = 16;
                                }
                                for (int i = 0; i < this._valores2.Count; i++)
                                {
                                    ValoresVO[] vl_ = this._valores2[i];
                                    for (int j = 0; j < vl_.Length; j++)
                                    {
                                        bool permiteArmazenar_nivel = true;
                                        
                                        if (lCol != null && lCol.Count > 0)
                                        {
                                            Util.ObjectColorante nCol = lCol.Find(o => o.Circuito == j + 1 + index_dec2);
                                            if (nCol != null)
                                            {
                                                permiteArmazenar_nivel = false;
                                            }
                                        }

                                        if (permiteArmazenar_nivel)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(j + 1 + index_dec2, vl_[j].PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(j + 1 + index_dec2, vl_[j].Volume);
                                            }
                                        }
                                    }
                                }
                            }

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                    if (!placa1Seq && counterP1 < counterUltimoP1)
                    {
                        if ((existe_Base_P1 && !terminou_Base_P1) || (existe_Base_P2 && !terminou_Base_P2))
                        {
                            if (existe_Base_P1)
                            {
                                _dispenser[0].Dispensar(this._valores[0], i_Step);
                                counterP1 = 1;
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else if(existe_Base_P2)
                            {
                                _dispenser[1].Dispensar(this._valores2[0], i_Step);
                                counterP2 = 1;
                                terminou_Base_P2 = true;
                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            _dispenser[0].Dispensar(this._valores[counterP1++], i_Step);
                            Thread.Sleep(1000);
                            if (!placa2Seq && counterP2 < counterUltimoP2)
                            {
                                if (_dispenser.Count > 1)
                                {                                    
                                    _dispenser[1].Dispensar(this._valores2[counterP2++], i_Step);
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        if (placa2Seq)
                        {
                            if (_dispenser.Count > 1)
                            {
                                if (counterP2 < counterUltimoP2)
                                {
                                    Thread.Sleep(1000);
                                    bool achou = false;
                                    int index_dec2 = 16;
                                    if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador)
                                    {
                                        index_dec2 = 16;
                                    }
                                    for (int i = counterP2_arr; i < index_dec2; i++)
                                    {
                                        ValoresVO[] vl = this._valores2[counterP2];
                                        ValoresVO v = vl[i];
                                        if (v != null && v.PulsoHorario > 0)
                                        {
                                            counterP2_arr = i;
                                            achou = true;
                                            break;
                                        }
                                    }
                                    if (achou)
                                    {
                                        ValoresVO[] vl = this._valores2[counterP2];
                                        ValoresVO v = vl[counterP2_arr];
                                        Task task = Task.Factory.StartNew(() => _dispenser[1].Dispensar(
                                               counterP2_arr,
                                               v.PulsoHorario,
                                               v.Velocidade,
                                               v.Aceleracao,
                                               v.Delay,
                                               v.PulsoReverso, 
                                               i_Step)
                                           );
                                        Thread.Sleep(1000);
                                        counterP2_arr++;
                                    }
                                    else
                                    {
                                        counterP2_arr = 0;
                                        counterP2++;
                                    }
                                }
                                else
                                {
                                    counterP2 = counterUltimoP2;
                                }
                            }
                            else
                            {
                                counterP2 = counterUltimoP2;
                            }
                        }

                    }
                    else if (!placa2Seq && counterP2 < counterUltimoP2)
                    {
                        #region inicio da segunda Condição
                        if ((existe_Base_P1 && !terminou_Base_P1) || (existe_Base_P2 && !terminou_Base_P2))
                        {
                            if (existe_Base_P1)
                            {
                                _dispenser[0].Dispensar(this._valores[0], i_Step);
                                counterP1 = 1;
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else if (existe_Base_P2)
                            {
                                _dispenser[1].Dispensar(this._valores2[0], i_Step);
                                counterP2 = 1;
                                terminou_Base_P2 = true;
                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            if (_dispenser.Count > 1)
                            {
                                _dispenser[1].Dispensar(this._valores2[counterP2++], i_Step);
                                Thread.Sleep(1000);
                            }
                            if (!placa1Seq && counterP1 < counterUltimoP1)
                            {
                                _dispenser[0].Dispensar(this._valores[counterP1++], i_Step);
                                Thread.Sleep(1000);
                            }
                        }

                        if (placa1Seq)
                        {
                            if (counterP1 < counterUltimoP1)
                            {
                                Thread.Sleep(1000);
                                bool achou = false;
                                int index_dec = 16;
                                if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                                {
                                    index_dec = 16;
                                }
                                for (int i = counterP1_arr; i < index_dec; i++)
                                {
                                    ValoresVO[] vl = this._valores[counterP1];
                                    ValoresVO v = vl[i];
                                    if (v != null && v.PulsoHorario > 0)
                                    {
                                        counterP1_arr = i;
                                        achou = true;
                                        break;
                                    }
                                }
                                if (achou)
                                {
                                    ValoresVO[] vl = this._valores[counterP1];
                                    ValoresVO v = vl[counterP1_arr];
                                    Task task = Task.Factory.StartNew(() => _dispenser[0].Dispensar(
                                           counterP1_arr,
                                           v.PulsoHorario,
                                           v.Velocidade,
                                           v.Aceleracao,
                                           v.Delay,
                                           v.PulsoReverso, 
                                           i_Step)
                                       );
                                    Thread.Sleep(1000);
                                    counterP1_arr++;
                                }
                                else
                                {
                                    counterP1++;
                                    counterP1_arr = 0;
                                }
                            }
                            else
                            {
                                counterP1 = counterUltimoP1;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        bool passouPlaca1 = false;
                        if (placa1Seq)
                        {
                            if (existe_Base_P1 && !terminou_Base_P1)
                            {
                                if (existe_Base_P1)
                                {
                                    _dispenser[0].Dispensar(this._valores[0], i_Step);
                                    counterP1 = 1;
                                    terminou_Base_P1 = true;
                                    Thread.Sleep(1000);
                                    passouPlaca1 = true;
                                }                                
                            }
                            else
                            {
                                if (counterP1 < this._valores.Count)
                                {
                                    _dispenser[0].Dispensar(this._valores[counterP1++], i_Step);
                                    passouPlaca1 = true;
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        if (placa2Seq && !passouPlaca1)
                        {
                            if (_dispenser.Count > 1)
                            {
                                if (existe_Base_P2 && !terminou_Base_P2)
                                {
                                    if (existe_Base_P2)
                                    {
                                        _dispenser[1].Dispensar(this._valores2[0], i_Step);
                                        counterP2 = 1;
                                        terminou_Base_P2 = true;
                                        Thread.Sleep(1000);
                                    }
                                }
                                else
                                {
                                    if (_dispenser.Count > 1)
                                    {
                                        _dispenser[1].Dispensar(this._valores2[counterP2++], i_Step);
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(this.thread_Sleep);
                    this.counterFalhaConexao = 0;
                    if (this.isDatUDCP)
                    {
                        this.Invoke(new MethodInvoker(OnResumeFormulaUDCP));
                    }
                }
                
                this.isRunning = false;
            }
            catch (Exception ex)
            {
                if ((this.counterFalhaConexao > _parametros.QtdTentativasConexao) || (this.modBusDispenser_P3 != null))
                {
                    Falha(ex);
                }
                else
                {
                    ForceOpenConnection();
                }

                this.isRunning = false;
                this.counterFalhaConexao++;
            }
        }

        void ForceOpenConnection()
        {
            try
            {
                if (this.modBusDispenser_P3 != null)
                {
                    this.modBusDispenser_P3.Disconnect();
                    Thread.Sleep(1000);
                    this.modBusDispenser_P3.Disconnect_Mover();
                    Thread.Sleep(1000);
                    this.modBusDispenser_P3.Connect();
                    Thread.Sleep(1000);
                    this.modBusDispenser_P3.Connect_Mover();
                }
                else
                {
                    for (int i = 0; i < this._dispenser.Count; i++)
                    {
                        this._dispenser[i].Disconnect();
                        Thread.Sleep(1000);
                        this._dispenser[i].Connect();
                        Thread.Sleep(1000);
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ProcessoDispensaSimultanea_FormClosed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();

            switch (this.DialogResult)
            {
                #region Status do processo           

                case DialogResult.OK:
                    {
                        #region Sucesso

                        Operar.AbaterColorante(_demanda);                      

                        string desc =
                            string.IsNullOrEmpty(this._descCor) ? string.Empty : " - " + this._descCor;

                        Log.Logar(
                         TipoLog.Processo,
                         _parametros.PathLogProcessoDispensa,
                          Negocio.IdiomaResxExtensao.Log_Cod_02 + Negocio.IdiomaResxExtensao.Global_DosagemDescricaoCorantes + desc);

                       

                        if (_parametros.ControlarNivel)
                        {
                            Log.Logar(
                                TipoLog.Processo,
                                 Util.ObjectParametros.Load().PathLogProcessoDispensa,
                                Negocio.IdiomaResxExtensao.Log_Cod_03 + Negocio.IdiomaResxExtensao.Global_BaixaColoranteEfetuada);
                        }

                        string log = "";
                        foreach (KeyValuePair<int, double> d in this._demanda)
                            log += $"{d.Key},{Math.Round(d.Value, 3)},";

                        log = log.Remove(log.Length - 1);
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, log);
                        Log.Logar(TipoLog.ControleDispensa, _parametros.PathLogControleDispensa, log);


                        desc =
                           string.IsNullOrEmpty(this._codigoCor) ? string.Empty : " - " + this._codigoCor;

                        Log.Logar(
                           TipoLog.Processo,
                           _parametros.PathLogProcessoDispensa,
                           Negocio.IdiomaResxExtensao.Log_Cod_04 + Negocio.IdiomaResxExtensao.Global_DosagemConcluida + desc);
                        
                        if(!_parametros.DisablePopUpDispDat)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Log_Cod_04 + Negocio.IdiomaResxExtensao.Global_DosagemConcluida);
                            }
                        }

                        break;

                        #endregion
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_07 + Negocio.IdiomaResxExtensao.Global_DosagemCancelada);

                        if (!_parametros.DisablePopUpDispDat)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Log_Cod_07 + Negocio.IdiomaResxExtensao.Global_DosagemCancelada);
                            }
                        }
                        break;
                    }
                case DialogResult.Abort:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_08 + Negocio.IdiomaResxExtensao.Global_DosagemAbortada);

                        if (!_parametros.DisablePopUpDispDat)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Log_Cod_08 + Negocio.IdiomaResxExtensao.Global_DosagemAbortada);
                            }
                        }

                        /*Se processo for abortado a purga deve ser realizada novamente
                       
                        if (_parametros.ExigirExecucaoPurga)
                        {
                            Util.ObjectParametros.SetExecucaoPurga(DateTime.MinValue);
                        }
                        */
                        break;
                    }
                case DialogResult.No:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Log_Cod_06 + Negocio.IdiomaResxExtensao.Global_FalhaDosagem);
                        if (!_parametros.DisablePopUpDispDat)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Log_Cod_06 + Negocio.IdiomaResxExtensao.Global_FalhaDosagem);
                            }
                        }


                        /* Se ocorrer falha na execução do processo a purga deve ser realizada novamente 
                        if (_parametros.ExigirExecucaoPurga)
                        {
                            Util.ObjectParametros.SetExecucaoPurga(DateTime.MinValue);
                        }
                        */
                        break;
                    }

                    #endregion
            }
        }

        #endregion

        //Função para identificar o reinicio do Monitoramento dos circuitos
        void RessetarTempoMonitoramentoCircuitos()
        {
            Constants.countTimerDelayMonit = 0;
        }

        #region Métodos privados

        void Falha(Exception ex)
		{
            LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			PausarMonitoramento();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
                    Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
            }

            DialogResult = DialogResult.No;
        }      

        void Emergencia()
        {
            PausarMonitoramento();
            try
            {
                this.modBusDispenser_P3.Halt();
                this.modBusDispenser_P3.UnHalt();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                //m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + "Botão de Emergência pressionado!");
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia);
            }

            DialogResult = DialogResult.No;
        }

        void DispensaForceHalt()
        {
            PausarMonitoramento();
            try
            {
                this.modBusDispenser_P3.Halt();
                this.modBusDispenser_P3.UnHalt();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + Negocio.IdiomaResxExtensao.PlacaMov_Disp_Parou, true);
            }

            DialogResult = DialogResult.No;
        }

        void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                if (backgroundWorker1 != null && backgroundWorker1.IsBusy && backgroundWorker1.CancellationPending == false)
                {
                    backgroundWorker1.CancelAsync();
                }
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
                    this.isRunning = false;
                    this.backgroundWorker1.RunWorkerAsync();
                }
                else
                {

                    while (backgroundWorker1.IsBusy)
                    {
                        this.isThread = false;
                        this.isRunning = true;
                    }
                    this.isThread = true;
                    this.isRunning = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
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
                        if (!this.isRunning)
                        {
                            this.isRunning = true;
                            this.Invoke(new MethodInvoker(Monitoramento_Event));
                            if (this.modBusDispenser_P3 == null)
                            {
                                Thread.Sleep(1500);
                            }
                        }
                    }

                    Thread.Sleep(this.thread_Sleep);
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

        #endregion

        #region OnResumeFormula UDCP
        void OnResumeFormulaUDCP()
        {
            try
            {               
                string npath_udcp = _parametros.PathMonitoramentoDAT;
                string[] arrayStr = this._parametros.PathMonitoramentoDAT.Split(Path.DirectorySeparatorChar);
                string pathUDCP = "";
                for (int i = 0; i < arrayStr.Length - 1; i++)
                {
                    pathUDCP += arrayStr[i] + Path.DirectorySeparatorChar;
                }
                npath_udcp = npath_udcp.Replace(".sig", ".dat");
                if (File.Exists(npath_udcp))
                {
                    if (File.Exists(_parametros.PathMonitoramentoDAT))
                    {                 
                        File.Delete(_parametros.PathMonitoramentoDAT);                    
                    }
                    string conteudoDAT = File.ReadAllText(npath_udcp, Encoding.GetEncoding("iso-8859-1"));
                    if(conteudoDAT.Contains("@EXT") && conteudoDAT.Contains("RESUME_FORMULA"))
                    {               
                        Abortar_Click(null, null);
                    }                    
                }                          
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
        #endregion

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
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
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
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;                                
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }

                               
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count-1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        

                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                this.counterFalha = 0;
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if(this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                //Movimentar Válvula Recuar Bicos
                                //this.passosActionP3 = 18;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));                            
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }

                              
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
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
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this.modBusDispenser_P3.ReadSensores_Mover();
                                if (this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    //this.passosActionP3 = 20;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                                    Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 21;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }

                             
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 2;
                            }
                            else
                            {
                                this.modBusDispenser_P3.ReadSensores_Mover();
                                if (this.modBusDispenser_P3.SensorCopo && this.modBusDispenser_P3.SensorEsponja)
                                {
                                    //Movimentar fechar gaveta
                                    //this.passosActionP3 = 20;
                                    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                                    Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                                    //Thread.Sleep(1000);
                                    this.passosActionP3 = 21;
                                }
                            }
                        }
                        this.counterFalha = 0;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }

                                
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //Dosar
                            this.passosActionP3 = 6;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if(this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }

                                
                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //this.passosActionP3 = 1;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if(this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));                            
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 =9;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //this.passosActionP3 = 1;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //this.passosActionP3 = 1;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //this.passosActionP3 = 18;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                            this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 19;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        lblSubStatus.Text = "";
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //this.passosActionP3 = 1;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                //this.passosActionP3 = 18;
                                this.counterFalha = 0;
                                lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                                this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                                //Thread.Sleep(1000);
                                this.passosActionP3 = 19;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 3;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 3;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
                        {
                            this.passosActionP3 = 11;
                        }
                        else
                        {
                            this.passosActionP3 =9;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(2, 1))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                //m.ShowDialog("Trocar recipiente!");
                                m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                this.passosActionP3 = 3;
                            }
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                        {
                            this.passosActionP3 = 1;
                        }

                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}

                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 3;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        this.qtdTrocaRecipiente = 0;
                        //this.passosActionP3 = 18;
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem;
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
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
                                this.passosActionP3 = 18;
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                this.passosActionP3 = 6;

                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //    DialogResult = DialogResult.OK;
                                //    Close();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || (!this.isNewOperacao && !this.modBusDispenser_P3.SensorBaixoBicos)))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 17;
                        }
                        else
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        break;
                    }
                case 17:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            DialogResult = DialogResult.OK;
                            Close();
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            //this.passosActionP3 = 6;
                            this.isNewOperacao = false;
                            this.passosActionP3 = 2;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorGavetaFechada)
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&                                
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
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
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;

                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
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
                        //this.passosActionP3 = 1;                        
                        this.counterFalha = 0;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 1:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 2;
                        break;
                    }
                case 2:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(2, 1))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                                {
                                    //m.ShowDialog("Trocar recipiente!");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente, true);
                                    this.passosActionP3 = 3;
                                }
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 1;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (!this.modBusDispenser_P3.SensorCopo || !this.modBusDispenser_P3.SensorEsponja)
                        {
                            this.qtdTrocaRecipiente++;
                            this.passosActionP3 = 2;
                        }
                        else
                        {
                            this.qtdTrocaRecipiente = 0;
                            //this.passosActionP3 = 4;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 5;
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 5;
                        break;
                    }
                case 5:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(5, 4))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Dosar
                                //this.passosActionP3 = 6;
                                this.isNewOperacao = false;
                                this.passosActionP3 = 7;
                                lblSubStatus.Text = "";
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
                            {
                                this.passosActionP3 = 4;
                            }
                        }
                        break;
                    }

                case 6:
                    {
                        lblSubStatus.Text = "";
                        this.isNewOperacao = false;
                        this.passosActionP3 = 7;
                        break;
                    }
                //tratar o termino da dispesna 
                case 7:
                    {
                        if (this.modBusDispenser_P3.IsEmergencia)
                        {
                            Emergencia();
                            break;
                        }
                        if (this.modBusDispenser_P3.IsHalt)
                        {
                            DispensaForceHalt();
                            break;
                        }
                        else if (!this.modBusDispenser_P3.IsReady)
                        {
                            this.counterFalha = 0;
                            Thread.Sleep(500);
                            break;
                        }
                        if (counterP1 >= counterUltimoP1)
                        {
                            if (this.modBusDispenser_P3.IsReady)
                            {
                                //this.isRunning = false;
                                //PausarMonitoramento();

                                if (this._valores != null && this._valores.Count > 0)
                                {
                                    for (int i = 0; i < this._valores.Count; i++)
                                    {
                                        ValoresVO[] vl_ = this._valores[i];
                                        for (int j = 0; j < vl_.Length; j++)
                                        {
                                            ValoresVO vl = vl_[j];
                                            if (vl != null && vl.PulsoHorario > 0)
                                            {
                                                Util.ObjectCalibragem.UpdatePulsosRev(i + 1, vl.PulsoReverso);
                                                Util.ObjectRecircular.UpdateVolumeDosado(i + 1, vl.Volume);

                                            }
                                        }
                                    }
                                }


                                this.passosActionP3 = 8;
                                this.counterFalha = 0;
                                break;
                            }
                        }
                        else
                        {
                            counterUltimoP1 = 16;
                            if (this.existe_Base_P1 && !terminou_Base_P1)
                            {
                                ValoresVO[] v = this._valores[0];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                terminou_Base_P1 = true;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                ValoresVO[] v = this._valores[this._valores.Count - 1];
                                this.modBusDispenser_P3.Dispensar(v, i_Step);
                                Thread.Sleep(1000);
                                counterP1 = counterUltimoP1;
                            }
                        }
                        break;
                    }
                //Dosado agora é abrir gaveta
                case 8:
                    {
                        lblStatus.Text = Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01;
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.AbrirGaveta(true));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 9;
                        break;
                    }
                case 9:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(9, 8))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                                {
                                    //m.ShowDialog("Usuário retirar o recipiente");
                                    m.ShowDialog(Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente, true);
                                }
                                this.passosActionP3 = 10;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                !this.modBusDispenser_P3.SensorGavetaAberta)
                            {
                                this.passosActionP3 = 8;
                            }
                        }
                        break;
                    }
                case 10:
                    {

                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if (!this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo)
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((!this.isNewOperacao && this.modBusDispenser_P3.SensorEsponja && !this.modBusDispenser_P3.SensorCopo) || (this.isNewOperacao && this.modBusDispenser_P3.SensorCopo))
                        {
                            //this.passosActionP3 = 14;
                            lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                            if (!this.isNewOperacao)
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                            }
                            else
                            {
                                Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                            }
                            //Thread.Sleep(1000);
                            this.passosActionP3 = 15;
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
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(true));
                        }
                        else
                        {
                            Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        }
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 15;
                        break;
                    }
                case 15:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(15, 14))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada &&
                                (this.isNewOperacao || (!this.isNewOperacao && this.modBusDispenser_P3.SensorBaixoBicos)))
                            {
                                //this.passosActionP3 = 16;
                                //this.counterFalha = 0;
                                //if (!this.isNewOperacao)
                                //{
                                //    lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao;
                                //    this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
                                //    //Thread.Sleep(1000);
                                //    this.passosActionP3 = 17;
                                //}
                                //else
                                //{
                                //    PausarMonitoramento();
                                //}
                                PausarMonitoramento();
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada || !this.modBusDispenser_P3.SensorBaixoBicos))
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
                            this.modBusDispenser_P3.ValvulaPosicaoRecirculacao();
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
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(17, 16))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaRecirculacao)
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
                        this.modBusDispenser_P3.ValvulaPosicaoDosagem();
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 19;
                        break;
                    }
                case 19:
                    {
                        this.modBusDispenser_P3.ReadSensores_Mover();
                        if (btEmergenciaCodErro(19, 18))
                        {
                            Thread.Sleep(500);
                        }
                        else if (this.modBusDispenser_P3.IsNativo == 1)
                        {
                            //Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && this.modBusDispenser_P3.SensorValvulaDosagem)
                        {

                            this.passosActionP3 = 1;
                        }
                        else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) && !this.modBusDispenser_P3.SensorValvulaDosagem)
                        {
                            this.passosActionP3 = 18;
                        }
                        break;
                    }
                case 20:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav;
                        Task task = Task.Factory.StartNew(() => this.modBusDispenser_P3.FecharGaveta(false));
                        //Thread.Sleep(1000);
                        this.passosActionP3 = 21;
                        break;

                    }
                case 21:
                    {
                        if (this.modBusDispenser_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenser_P3.ReadSensores_Mover();
                            if (btEmergenciaCodErro(21, 20))
                            {
                                Thread.Sleep(500);
                            }
                            else if (this.modBusDispenser_P3.IsNativo == 1)
                            {
                                //Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                this.modBusDispenser_P3.SensorGavetaFechada)
                            {
                                //Abrir gaveta
                                this.passosActionP3 = 18;
                                this.counterFalha = 0;
                            }
                            else if ((this.modBusDispenser_P3.IsNativo == 0 || this.modBusDispenser_P3.IsNativo == 2) &&
                                (!this.modBusDispenser_P3.SensorGavetaFechada))
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
            if (this.modBusDispenser_P3.SensorEmergencia)
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
            else if (this.modBusDispenser_P3.CodError > 0)
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
                    //string msg = "Usuário ocorreu algum erro na movimentação da placa. Deseja continuar este processo?" + Environment.NewLine + "Descrição: " + this.modBusDispenser_P3.GetDescCodError();
                    string msg = Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos + this.modBusDispenser_P3.GetDescCodError();
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