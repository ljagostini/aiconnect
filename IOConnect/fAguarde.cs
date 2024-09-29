using Percolore.Core.Logging;
using Percolore.IOConnect.Util;
using System.ComponentModel;

namespace Percolore.IOConnect
{
	public partial class fAguarde : Form
    {
        //private int inicializacao = -1;
        private bool isThread = false;
        private bool statusPaint = false;
        private bool isRunning = false;

        public Brush _Brush = Brushes.Gray;
        public int _TimerDelay = 500;

        public event CloseWindows OnClosedEvent = null;

        public event OpenWindows OnOpenWindows = null;

        private DateTime _dtInicio = DateTime.Now;
      
        private string mensagemLBL = "";


        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.isRunning = value; }
        }

        public fAguarde(string mensagem)
        {
            InitializeComponent();
            this.mensagemLBL = mensagem;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            lblMensagem.Width = this.Width;
            this.Location = new Point(0, 30);
        }

        public fAguarde(string mensagem, Color _backColor)
        {
            InitializeComponent();
            this.mensagemLBL = mensagem;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.BackColor = _backColor;
            lblMensagem.Width = this.Width;
            this.Location = new Point(0, 30);
        }

        private void fAguarde_Load(object sender, EventArgs e)
        {
            try
            {
                Invalidate();
                lblMensagem.Text = this.mensagemLBL;
                if (this.OnOpenWindows != null)
                {
                    this.OnOpenWindows();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fAguarde_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.OnClosedEvent != null)
                {
                    this.OnClosedEvent();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        public void PausarMonitoramento()
        {
            try
            {
                this.isThread = false;
                Thread.Sleep(10);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

		}

        public void ExecutarMonitoramento()
        {
            try
            {
                this.isThread = true;
                bkgWorker.RunWorkerAsync();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
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
                        if (!this.statusPaint)
                        {
                            this.statusPaint = true;
                            this.Invoke(new MethodInvoker(MonitoramentoEvent));
                        }
                    }

                    Thread.Sleep(this._TimerDelay);
                }

                this.Invoke(new MethodInvoker(ClosedWindowEvent));
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

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
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void ClosedWindowEvent()
        {
            try
            {
                Thread.Sleep(2000);
                this.Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void MonitoramentoEvent()
        {
            
            this.isRunning = true;
            this.statusPaint = false;
        }
    }
}