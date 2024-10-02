using Percolore.Core.Logging;

namespace Percolore.IOConnect
{
	public partial class fPlacaInput : Form
    {
        public fPlacaInput()
        {
            InitializeComponent();
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void fPlacaInput_Load(object sender, EventArgs e)
        {
            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(refreshStatus));
        }
                
        private void refreshStatus()
        {
            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);

                try
                {
                    mP2.Connect();
                    Task<ModBusDispenser_P2.StatusSensores> task = Task.Factory.StartNew(() => mP2.getStatusSensores());

                    ModBusDispenser_P2.StatusSensores stSensor = task.Result;
                    if (stSensor.Input_1)
                    {
                        txtInput1.Text = "1";
                    }
                    else
                    {
                        txtInput1.Text = "0";
                    }
                    if (stSensor.Input_2)
                    {
                        txtInput2.Text = "1";
                    }
                    else
                    {
                        txtInput2.Text = "0";
                    }
                    if (stSensor.Input_3)
                    {
                        txtInput3.Text = "1";
                    }
                    else
                    {
                        txtInput3.Text = "0";
                    }
                    if (stSensor.Input_4)
                    {
                        txtInput4.Text = "1";
                    }
                    else
                    {
                        txtInput4.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog("Error:" + ex.Message);
                    }
                }
                finally
                {
                    mP2.Disconnect();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + ex.Message);
                }
            }
        }

        private void btnVersion_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);
                
                try
                {
                    mP2.Connect();
                    string _strV = mP2.GetVersion();

                    txtVersionHard.Text = string.IsNullOrEmpty(_strV) ? "failure to get version...." : _strV;
                 }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog("Error:" + ex.Message);
                    }
                }
                finally
                {
                    mP2.Disconnect();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + ex.Message);
                }
            }
        }

        private void gbSensores_Enter(object sender, EventArgs e)
        {

        }
    }
}