using Percolore.Core.Util;
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
    public partial class fMensagem : Form
    {
        System.ComponentModel.BackgroundWorker backgroundWorker1 = null;
        private int delay = 30;
        private DateTime? dtInicio = null;
        private bool isThread, isRunning;
        private bool yes = false;
        private bool defaultReturn = true;

        public enum TipoMensagem
        {
            Informacao, Confirmacao, Erro
        };

        public fMensagem(TipoMensagem tipo)
        {
            InitializeComponent();

            /* É necessário indicar topMost aqui para que o form seja 
             * * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = true;


            //[Adequa largura do form à da tela]
            this.Width = Screen.PrimaryScreen.Bounds.Width;

            //[Posicionamento do form]
            this.Location = new Point(0, 30);

            Color ForeColor = Color.FromArgb(250, 250, 250);
            Color formBackColor = Color.FromArgb(75, 178, 89);
            Color buttonbackColor = Color.FromArgb(71, 165, 77);

            Image _imagem = null;

            switch (tipo)
            {
                case TipoMensagem.Erro:
                    {
                        #region fMensagem de erro

                        formBackColor = Color.FromArgb(50, 50, 50);
                        buttonbackColor = Color.FromArgb(30, 30, 30);
                        _imagem = ImageHelper.Base64ToImage(Properties.Imagens.MensagemErro);

                        break;

                        #endregion
                    }
                case TipoMensagem.Informacao:
                    {
                        #region fMensagem de informação

                        formBackColor = Color.FromArgb(75, 178, 89);
                        buttonbackColor = Color.FromArgb(71, 165, 77);
                        _imagem = ImageHelper.Base64ToImage(Properties.Imagens.MesnagemInformacao);

                        break;

                        #endregion
                    }
                case TipoMensagem.Confirmacao:
                    {
                        #region fMensagem de confirmação

                        formBackColor = Color.FromArgb(210, 15, 0);
                        buttonbackColor = Color.FromArgb(195, 15, 0);
                        _imagem = ImageHelper.Base64ToImage(Properties.Imagens.MensagemInterrogacao);


                        break;

                        #endregion
                    }
            }

            #region Aplica parâmetros aos controles

            BackColor = formBackColor;
            pct01.Image = _imagem;

            btSave.BackColor = buttonbackColor;
            btSave.FlatAppearance.BorderColor = buttonbackColor;

            btCancel.BackColor = buttonbackColor;
            btCancel.FlatAppearance.BorderColor = buttonbackColor;

            lblText.ForeColor = ForeColor;
            btCancel.ForeColor = ForeColor;
            btSave.ForeColor = ForeColor;
            btSave.FlatAppearance.BorderColor = ForeColor;
            

            #endregion
        }

        public bool ShowDialog(string message, bool _topMost = false)
        {
            lblText.Text = message;
            btSave.Visible = false;
            btCancel.Text = "Ok";

            //Centraliza botão de cancelmento
            btCancel.Left = (this.Width / 2) - (btCancel.Width / 2);
            if(_topMost)
            {
                this.TopMost = true;
            }
            base.ShowDialog();
            return yes;
        }

        public bool ShowDialog(string message, string action, bool _topMost = false)
        {
            lblText.Text = message;
            btSave.Text = action;
            if (_topMost)
            {
                this.TopMost = true;
            }
            base.ShowDialog();

            return yes;
        }

        public bool ShowDialog(string message, string action, string nonaction, bool _topMost = false)
        {
            lblText.Text = message;
            btSave.Text = action;
            btCancel.Text = nonaction;
            if (_topMost)
            {
                this.TopMost = true;
            }
            base.ShowDialog();

            return yes;
        }

        public bool ShowDialog(string message, string action, string nonaction, bool isTimer, int _delay, bool _topMost = true, bool _defaultReturn = true)
        {
            lblText.Text = message;
            btSave.Text = action;
            if(string.IsNullOrEmpty( nonaction))
            {
               
                btCancel.Visible = false;
                btCancel.Enabled = false;
            }
            else
            {
                btCancel.Text = nonaction;
            }
            this.defaultReturn = _defaultReturn;
            this.delay = _delay;
            lblDelay.Visible = true;
            try
            {
                lblDelay.Text = this.delay.ToString();
                lblDelay.Visible = true;
                this.dtInicio = DateTime.Now;
                if (_topMost)
                {
                    this.TopMost = true;
                }
                ExecutarThread();
                Thread.Sleep(1000);
            }
            catch
            { }
            base.ShowDialog();            

            return yes;
        }
             

        void Cancelar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.isThread)
                {
                    PausarThread();
                }
            }
            catch
            { }
            this.Close();
        }

        void Salvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.isThread)
                {
                    PausarThread();

                }
            }
            catch
            { }
            yes = true;
            this.Close();
        }

        private void Mensagens_Load(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
           * redesenhando em primeiro plano sobre outra aplicação quando
           * houver outra aplicação rodando. */
            this.TopMost = true;
        }

        private void ExecutarThread()
        {
            try
            {
                if (backgroundWorker1 == null)
                {
                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
                    this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    this.backgroundWorker1.WorkerSupportsCancellation = true;
                    this.isRunning = false;
                    this.isThread = true;
                    this.dtInicio = DateTime.Now;
                    this.backgroundWorker1.RunWorkerAsync();
                }
                else
                {

                    while (backgroundWorker1.IsBusy)
                    {
                        this.isThread = false;
                        this.isRunning = true;
                    }
                    this.isRunning = false;
                    this.isThread = true;
                    
                    this.dtInicio = DateTime.Now;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch
            { }


        }

        private void PausarThread()
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

        void MonitoramentoEvent()
        {
            try
            {
                TimeSpan? ts = DateTime.Now.Subtract(this.dtInicio.Value);
                if(ts != null && ts.HasValue)
                {
                    lblDelay.Text = (this.delay - (int)ts.Value.TotalSeconds).ToString();
                    if (ts.Value.TotalSeconds >= this.delay)
                    {
                        PausarThread();
                        Thread.Sleep(500);
                        if (!this.defaultReturn)
                        {
                            yes = false;
                        }
                        else
                        {
                            yes = true;
                        }
                        this.Close();                        
                    }
                }
                
                ts = null;
                this.isRunning = false;
            }
            catch
            {
                this.isRunning = false;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
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
                            this.isRunning = false;
                        }
                    }
                    Thread.Sleep(500);
                }

            }
            catch
            {
            }

        }
                
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
    }
}
