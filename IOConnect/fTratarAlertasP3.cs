using Percolore.Core.UserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Percolore.IOConnect
{
    public partial class fTratarAlertasP3 : Form
    {
        private ModBusDispenserMover_P3 modBusDispenserMover_P3 = null;
        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        Util.ObjectParametros _parametros = null;
        bool desativarUI;
        private bool isThread = false;
        private bool isRunning = false;
        private int counterFalha = 0;
        // 
        private int tipoActionP3 = -1;
        private int passosActionP3 = 0;

        public fTratarAlertasP3(ModBusDispenserMover_P3 _modBusDispenserMover_P3, bool desativarUI = false)
        {
            InitializeComponent();
            this.modBusDispenserMover_P3 = _modBusDispenserMover_P3;
            this._parametros = Util.ObjectParametros.Load();
            /* Se a exibição da interface for desabilitada
             * não é necessário configurar propriedades dos controles */
            this.desativarUI = desativarUI;
            if (this.desativarUI)
            {
                return;
            }
            lblStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_lblTitulo;
            lblSubStatus.Text = "";
            //Redimensiona e posiciona form
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            //lblStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01;
            //lblSubStatus.Text = Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg01;
            btnIniciar.Text = Negocio.IdiomaResxExtensao.Global_Iniciar;
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            btnAbortar.Text = Negocio.IdiomaResxExtensao.Global_Abortar;

            //Habilitar controles
            lblStatus.Visible = true;
            progressBar.Visible = false;
            btnIniciar.Enabled = true;
            btnCancelar.Enabled = true;
            btnAbortar.Visible = false;
        }

        private void fTratarAlertasP3_Load(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
             * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = !desativarUI;

            /* Se a exibição da interface for desabilitada,
             * reduz form a tamanho zero e incia automaticamente a dispensa */
            if (desativarUI)
            {
                this.Width = 0;
                this.Height = 0;               
            }
            Iniciar_Click(sender, e);
        }

        private void fTratarAlertasP3_FormClosed(object sender, FormClosedEventArgs e)
        {
            PausarMonitoramento();

            switch (this.DialogResult)
            {
                #region Log de processo

                case DialogResult.OK:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Concluido);

                        //Persiste data e hora de execução da purga
                        Util.ObjectParametros.SetExecucaoPurga(DateTime.Now);
                        break;
                    }
                case DialogResult.Cancel:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Cancelado);
                        break;
                    }
                case DialogResult.Abort:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Abortado);
                        break;
                    }
                case DialogResult.No:
                    {
                        Log.Logar(
                            TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Falha);
                        break;
                    }

                    #endregion
            }

        }

        void Iniciar_Click(object sender, EventArgs e)
        {
            try
            {
                lblSubStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_lblSubStatus ;
                progressBar.Visible = true;
                btnIniciar.Enabled = false;
                btnCancelar.Visible = false;
                btnAbortar.Visible = true;
                
                this.counterFalha = 0;
                
                this.tipoActionP3 = -1;
                this.passosActionP3 = 0;
              
                
                this.tipoActionP3 = getActionP3();
              
                if (this.tipoActionP3 >= 0)
                {
                    ExecutarMonitoramento();
                    Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Iniciado);
                }
                else
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                Falha(ex);
            }
        }

        private int getActionP3()
        {
            int retorno = -1;
            switch(modBusDispenserMover_P3.CodAlerta)
            {
                case 5:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_5;
                        retorno = 5;
                        break;
                    }
                case 6:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_6;
                        retorno = 6;
                        break;
                    }
                case 14:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_14;
                        retorno = 14;
                        break;
                    }
                case 15:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_15;
                        retorno = 15;
                        break;
                    }
                case 16:
                    {
                        lblSubStatus.Text = Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_16;
                        retorno = 16;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }


            return retorno;
        }
        
        void Abortar_Click(object sender, EventArgs e)
        {
            PausarMonitoramento();

            //Define status da operação
            DialogResult = DialogResult.Abort;
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        
        #region Métodos privados

        void Falha(Exception ex)
        {
            PausarMonitoramento();

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
            {
                m.ShowDialog(
                    Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
            }

            DialogResult = DialogResult.No;
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
            }
            catch
            { }

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
            catch
            { }


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
                                Thread.Sleep(500);
                            }
                        }
                        catch
                        {
                        }
                    }
                    Thread.Sleep(500);
                }

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

        // void Monitoramento_Event(object sender, EventArgs e)
        void MonitoramentoEvent()
        {
            try
            {   
                trataActionP3();                
                this.isRunning = false;
                this.counterFalha = 0;
                ////Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                if (this.counterFalha > 0)
                {
                    Falha(ex);
                }
                this.isRunning = false;
                this.counterFalha++;
            }
        }

        void trataActionP3()
        {
            try
            {
                switch (this.tipoActionP3)
                {
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

        void trataPassosAction_05()
        {
            switch (this.passosActionP3)
            {
                case 0:
                    {
                        this.modBusDispenserMover_P3.DescerBico();
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this.modBusDispenserMover_P3.ReadSensores_Mover();
                        if (this.modBusDispenserMover_P3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            this.modBusDispenserMover_P3.SensorBaixoBicos )
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            (!this.modBusDispenserMover_P3.SensorBaixoBicos ))
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        else if(this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        break;
                    }
                default:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenserMover_P3.FecharGaveta(true));
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        if (this.modBusDispenserMover_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenserMover_P3.ReadSensores_Mover();
                            if (this.modBusDispenserMover_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                                this.modBusDispenserMover_P3.SensorBaixoBicos && this.modBusDispenserMover_P3.SensorGavetaFechada)
                            {
                                PausarMonitoramento();
                                Thread.Sleep(1000);
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                                (!this.modBusDispenserMover_P3.SensorBaixoBicos || !this.modBusDispenserMover_P3.SensorGavetaFechada))
                            {
                                PausarMonitoramento();
                                Thread.Sleep(1000);
                                DialogResult = DialogResult.No;
                                Close();
                            }
                            else if (this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                            {
                                PausarMonitoramento();
                                Thread.Sleep(1000);
                                DialogResult = DialogResult.No;
                                Close();
                            }
                        }
                        break;
                    }
                default:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
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
                        this.modBusDispenserMover_P3.ValvulaPosicaoDosagem();
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {

                        this.modBusDispenserMover_P3.ReadSensores_Mover();
                        if (this.modBusDispenserMover_P3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            this.modBusDispenserMover_P3.SensorValvulaDosagem)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            (!this.modBusDispenserMover_P3.SensorValvulaDosagem))
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        else if (this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        break;
                    }
                default:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
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
                        this.modBusDispenserMover_P3.DescerBico();
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        this.modBusDispenserMover_P3.ReadSensores_Mover();
                        if (this.modBusDispenserMover_P3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            this.modBusDispenserMover_P3.SensorBaixoBicos)
                        {
                            //this.passosActionP3 = 2;
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            (!this.modBusDispenserMover_P3.SensorBaixoBicos))
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        else if (this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        break;
                    }

                case 2:
                    {
                        this.modBusDispenserMover_P3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 3;
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenserMover_P3.ReadSensores_Mover();
                        if (this.modBusDispenserMover_P3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            this.modBusDispenserMover_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            (!this.modBusDispenserMover_P3.SensorValvulaRecirculacao))
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        else if (this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        break;
                    }
                default:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
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
                        Task task = Task.Factory.StartNew(() => this.modBusDispenserMover_P3.FecharGaveta(true));
                        this.passosActionP3 = 1;
                        break;
                    }
                case 1:
                    {
                        if (this.modBusDispenserMover_P3.TerminouProcessoDuplo)
                        {
                            this.modBusDispenserMover_P3.ReadSensores_Mover();
                            if (this.modBusDispenserMover_P3.IsNativo == 1)
                            {
                                Thread.Sleep(1000);
                            }
                            else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                                this.modBusDispenserMover_P3.SensorGavetaFechada && this.modBusDispenserMover_P3.SensorBaixoBicos)
                            {
                                //this.passosActionP3 = 2;
                                PausarMonitoramento();
                                Thread.Sleep(1000);
                                DialogResult = DialogResult.OK;
                                Close();
                            }
                            else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                                (!this.modBusDispenserMover_P3.SensorGavetaFechada || !this.modBusDispenserMover_P3.SensorBaixoBicos))
                            {
                                PausarMonitoramento();
                                Thread.Sleep(1000);
                                DialogResult = DialogResult.No;
                                Close();
                            }
                            else if (this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                            {
                                PausarMonitoramento();
                                Thread.Sleep(1000);
                                DialogResult = DialogResult.No;
                                Close();
                            }
                        }
                        break;
                    }

                case 2:
                    {
                        this.modBusDispenserMover_P3.ValvulaPosicaoRecirculacao();
                        this.passosActionP3 = 3;
                        break;
                    }
                case 3:
                    {
                        this.modBusDispenserMover_P3.ReadSensores_Mover();
                        if (this.modBusDispenserMover_P3.IsNativo == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            this.modBusDispenserMover_P3.SensorValvulaRecirculacao)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else if ((this.modBusDispenserMover_P3.IsNativo == 0 || this.modBusDispenserMover_P3.IsNativo == 2) &&
                            (!this.modBusDispenserMover_P3.SensorValvulaRecirculacao))
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        else if (this.modBusDispenserMover_P3.SensorEmergencia || this.modBusDispenserMover_P3.CodError > 0)
                        {
                            PausarMonitoramento();
                            Thread.Sleep(1000);
                            DialogResult = DialogResult.No;
                            Close();
                        }
                        break;
                    }
                default:
                    {
                        PausarMonitoramento();
                        Thread.Sleep(1000);
                        DialogResult = DialogResult.No;
                        Close();
                        break;
                    }
            }
        }
        #endregion

    }
}
