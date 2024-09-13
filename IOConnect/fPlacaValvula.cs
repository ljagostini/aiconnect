using Percolore.Core;
using Percolore.IOConnect.Modbus;
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
    public partial class fPlacaValvula : Form
    {
        public fPlacaValvula()
        {
            InitializeComponent();
        }
        private void fPlacaValvula_Load(object sender, EventArgs e)
        {

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(refreshStatus));
        }


        private void refreshStatus()
        {
            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                if ((Dispositivo)parametros.IdDispositivo == Dispositivo.Placa_2)
                {
                    ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);
                    try
                    {
                        mP2.Connect();
                        Task<StatusValvulas> task = Task.Factory.StartNew(() => mP2.getStatusValvulas());
                        //ModBusDispenser_P2.StatusSensores stSensor = mP2.getStatusSensores();

                        StatusValvulas stVal = task.Result;
                        if (stVal.Circuito_1)
                        {
                            txtValve1.Text = "R";
                        }
                        else
                        {
                            txtValve1.Text = "D";
                        }
                        if (stVal.Circuito_2)
                        {
                            txtValve2.Text = "R";
                        }
                        else
                        {
                            txtValve2.Text = "D";
                        }
                        if (stVal.Circuito_3)
                        {
                            txtValve3.Text = "R";
                        }
                        else
                        {
                            txtValve3.Text = "D";
                        }
                        if (stVal.Circuito_4)
                        {
                            txtValve4.Text = "R";
                        }
                        else
                        {
                            txtValve4.Text = "D";
                        }
                        if (stVal.Circuito_5)
                        {
                            txtValve5.Text = "R";
                        }
                        else
                        {
                            txtValve5.Text = "D";
                        }
                        if (stVal.Circuito_6)
                        {
                            txtValve6.Text = "R";
                        }
                        else
                        {
                            txtValve6.Text = "D";
                        }
                        if (stVal.Circuito_7)
                        {
                            txtValve7.Text = "R";
                        }
                        else
                        {
                            txtValve7.Text = "D";
                        }
                        if (stVal.Circuito_8)
                        {
                            txtValve8.Text = "R";
                        }
                        else
                        {
                            txtValve8.Text = "D";
                        }
                        if (stVal.Circuito_9)
                        {
                            txtValve9.Text = "R";
                        }
                        else
                        {
                            txtValve9.Text = "D";
                        }
                        if (stVal.Circuito_10)
                        {
                            txtValve10.Text = "R";
                        }
                        else
                        {
                            txtValve10.Text = "D";
                        }
                        if (stVal.Circuito_11)
                        {
                            txtValve11.Text = "R";
                        }
                        else
                        {
                            txtValve11.Text = "D";
                        }
                        if (stVal.Circuito_12)
                        {
                            txtValve12.Text = "R";
                        }
                        else
                        {
                            txtValve12.Text = "D";
                        }
                        if (stVal.Circuito_13)
                        {
                            txtValve13.Text = "R";
                        }
                        else
                        {
                            txtValve13.Text = "D";
                        }
                        if (stVal.Circuito_14)
                        {
                            txtValve14.Text = "R";
                        }
                        else
                        {
                            txtValve14.Text = "D";
                        }
                        if (stVal.Circuito_15)
                        {
                            txtValve15.Text = "R";
                        }
                        else
                        {
                            txtValve15.Text = "D";
                        }
                        if (stVal.Circuito_16)
                        {
                            txtValve16.Text = "R";
                        }
                        else
                        {
                            txtValve16.Text = "D";
                        }
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }
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
                else
                {
                    ModBusDispenser_P4 mP2 = new ModBusDispenser_P4(parametros.NomeDispositivo);
                    try
                    {
                        mP2.Connect();
                        Task<StatusValvulas> task = Task.Factory.StartNew(() => mP2.getStatusValvulas());
                        //ModBusDispenser_P2.StatusSensores stSensor = mP2.getStatusSensores();

                        StatusValvulas stVal = task.Result;
                        if (stVal.Circuito_1)
                        {
                            txtValve1.Text = "R";
                        }
                        else
                        {
                            txtValve1.Text = "D";
                        }
                        if (stVal.Circuito_2)
                        {
                            txtValve2.Text = "R";
                        }
                        else
                        {
                            txtValve2.Text = "D";
                        }
                        if (stVal.Circuito_3)
                        {
                            txtValve3.Text = "R";
                        }
                        else
                        {
                            txtValve3.Text = "D";
                        }
                        if (stVal.Circuito_4)
                        {
                            txtValve4.Text = "R";
                        }
                        else
                        {
                            txtValve4.Text = "D";
                        }
                        if (stVal.Circuito_5)
                        {
                            txtValve5.Text = "R";
                        }
                        else
                        {
                            txtValve5.Text = "D";
                        }
                        if (stVal.Circuito_6)
                        {
                            txtValve6.Text = "R";
                        }
                        else
                        {
                            txtValve6.Text = "D";
                        }
                        if (stVal.Circuito_7)
                        {
                            txtValve7.Text = "R";
                        }
                        else
                        {
                            txtValve7.Text = "D";
                        }
                        if (stVal.Circuito_8)
                        {
                            txtValve8.Text = "R";
                        }
                        else
                        {
                            txtValve8.Text = "D";
                        }
                        if (stVal.Circuito_9)
                        {
                            txtValve9.Text = "R";
                        }
                        else
                        {
                            txtValve9.Text = "D";
                        }
                        if (stVal.Circuito_10)
                        {
                            txtValve10.Text = "R";
                        }
                        else
                        {
                            txtValve10.Text = "D";
                        }
                        if (stVal.Circuito_11)
                        {
                            txtValve11.Text = "R";
                        }
                        else
                        {
                            txtValve11.Text = "D";
                        }
                        if (stVal.Circuito_12)
                        {
                            txtValve12.Text = "R";
                        }
                        else
                        {
                            txtValve12.Text = "D";
                        }
                        if (stVal.Circuito_13)
                        {
                            txtValve13.Text = "R";
                        }
                        else
                        {
                            txtValve13.Text = "D";
                        }
                        if (stVal.Circuito_14)
                        {
                            txtValve14.Text = "R";
                        }
                        else
                        {
                            txtValve14.Text = "D";
                        }
                        if (stVal.Circuito_15)
                        {
                            txtValve15.Text = "R";
                        }
                        else
                        {
                            txtValve15.Text = "D";
                        }
                        if (stVal.Circuito_16)
                        {
                            txtValve16.Text = "R";
                        }
                        else
                        {
                            txtValve16.Text = "D";
                        }
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }
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
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + exc.Message);
                }
            }

        }

        private void btnDosar_Click(object sender, EventArgs e)
        {

            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                if ((Dispositivo) parametros.IdDispositivo == Dispositivo.Placa_2)
                {
                    ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);
                    try
                    {
                        int posicao = int.Parse(((Button)sender).Tag.ToString());
                        mP2.Connect();
                        mP2.AcionaValvula(false, posicao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }

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
                else
                {
                    ModBusDispenser_P4 mP2 = new ModBusDispenser_P4(parametros.NomeDispositivo);
                    try
                    {
                        int posicao = int.Parse(((Button)sender).Tag.ToString());
                        mP2.Connect();
                        mP2.AcionaValvula(false, posicao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }

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
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + exc.Message);
                }
            }
        }

        private void btnRecircular_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                if ((Dispositivo)parametros.IdDispositivo == Dispositivo.Placa_2)
                {
                    ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);
                    try
                    {
                        int posicao = int.Parse(((Button)sender).Tag.ToString());
                        mP2.Connect();
                        mP2.AcionaValvula(true, posicao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }

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
                else
                {
                    ModBusDispenser_P4 mP2 = new ModBusDispenser_P4(parametros.NomeDispositivo);
                    try
                    {
                        int posicao = int.Parse(((Button)sender).Tag.ToString());
                        mP2.Connect();
                        mP2.AcionaValvula(true, posicao);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }

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
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + exc.Message);
                }
            }
        }

        private void btn_Dispense_V_All_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                if ((Dispositivo)parametros.IdDispositivo == Dispositivo.Placa_2)
                {
                    ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);
                    try
                    {
                        mP2.Connect();
                        mP2.AcionaValvulas(false);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }

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
                else
                {
                    ModBusDispenser_P4 mP2 = new ModBusDispenser_P4(parametros.NomeDispositivo);
                    try
                    {
                        mP2.Connect();
                        mP2.AcionaValvulas(false);
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }

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
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + exc.Message);
                }
            }
        }

        private void btn_Recircule_V_All_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                if ((Dispositivo)parametros.IdDispositivo == Dispositivo.Placa_2)
                {
                    ModBusDispenser_P2 mP2 = new ModBusDispenser_P2(parametros.NomeDispositivo);
                    try
                    {
                        mP2.Connect();
                        mP2.AcionaValvulas(true);

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }
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
                else
                {
                    ModBusDispenser_P4 mP2 = new ModBusDispenser_P4(parametros.NomeDispositivo);
                    try
                    {
                        mP2.Connect();
                        mP2.AcionaValvulas(true);

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog("Command executed");
                        }
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
            }
            catch (Exception exc)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog("Error:" + exc.Message);
                }
            }
        }
    }
}
