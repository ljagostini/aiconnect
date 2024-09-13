
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            try
            {
                DialogResult = DialogResult.OK;
            }
            catch
            { }
        }

        private void fPlacaInput_Load(object sender, EventArgs e)
        {
            try
            {
                //btnRefresh_Click(null, null);
            }
            catch
            { }
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
                    //ModBusDispenser_P2.StatusSensores stSensor = mP2.getStatusSensores();

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
                    //mP2.sa
                }
                catch (Exception exc1)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog("Error:" + exc1.Message);
                    }
                }
                finally
                {
                    mP2.Disconnect();
                }
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" +exc.Message);
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
                catch (Exception exc1)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog("Error:" + exc1.Message);
                    }
                }
                finally
                {
                    mP2.Disconnect();
                }
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + exc.Message);
                }
            }
        }

        private void gbSensores_Enter(object sender, EventArgs e)
        {

        }
    }
}
