using Percolore.IOConnect.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Percolore.IOConnect
{
    public partial class fLogComunication : Form
    {
        public event CloseWindows OnClosedEvent = null;

        public fLogComunication()
        {
            InitializeComponent();
        }

        private void fLogComunication_Load(object sender, EventArgs e)
        {
            try
            {
                atualizaDGV();
            }
            catch
            { }
        }

        private void fLogComunication_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.OnClosedEvent != null)
                {
                    this.OnClosedEvent();
                }
            }
            catch
            {
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            atualizaDGV();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (Modbus.Constantes.listLogSerial != null && Modbus.Constantes.listLogSerial.Count > 0)
                {
                    Modbus.Constantes.listLogSerial.Clear();
                    atualizaDGV();
                }

            }
            catch
            { }
        }

        private void atualizaDGV()
        {
            try
            {


                DataTable dt = new DataTable();
                dt.Columns.Add("Data", typeof(string));
                dt.Columns.Add("Tipo", typeof(string));
                dt.Columns.Add("Mensagem", typeof(string));
                if (Percolore.IOConnect.Modbus.Constantes.listLogSerial.Count > 0)
                {
                    List<LogSerial> lmSer = Percolore.IOConnect.Modbus.Constantes.listLogSerial.ToList();
                    foreach (LogSerial lSer in lmSer)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Data"] = string.Format("{0:dd-MM-yyyy HH:mm:ss.fff}", lSer.dtHora);
                        dr["Tipo"] = lSer.tipoMessage;
                        dr["Mensagem"] = lSer.message;
                        dt.Rows.Add(dr);
                    }
                }

                dgvLog.DataSource = dt.DefaultView;
                dgvLog.Columns[0].HeaderText = "Data";
                dgvLog.Columns[0].Width = 150;

                dgvLog.Columns[1].HeaderText = "Tipo";
                dgvLog.Columns[1].Width = 100;
                dgvLog.Columns[1].Frozen = true;

                dgvLog.Columns[2].HeaderText = "Mensagem";
                dgvLog.Columns[2].Width = 400;


            }
            catch
            { }
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            try
            {
                string PathFile = Path.Combine(Environment.CurrentDirectory, "LogSerial.csv");
                if(File.Exists(PathFile))
                {
                    File.Delete(PathFile);
                    Thread.Sleep(1000);
                }
                using (StreamWriter sw = new StreamWriter(File.Open(PathFile, FileMode.Create), Encoding.GetEncoding("ISO-8859-1")))
                {
                    List<LogSerial> lmSer = Percolore.IOConnect.Modbus.Constantes.listLogSerial.ToList();
                    foreach (LogSerial lSer in lmSer)
                    {
                        string _str = string.Format("{0:dd-MM-yyyy HH:mm:ss.fff}", lSer.dtHora) + ";" + lSer.tipoMessage + ";" + lSer.message + ";";
                        sw.WriteLine(_str);
                    }
                    sw.Close();
                }
                    
            }
            catch
            {

            }
        }
    }
}
