using Newtonsoft.Json;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Util;
using Percolore.IOConnect.Negocio;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Percolore.IOConnect
{
	public partial class fSobre : Form
    {
        string _serial;
        string _guid;
        private Util.ObjectParametros parametros;

        public fSobre()
        {
            InitializeComponent();
#if DEBUG
            this.TopMost = false;
#endif
            //Posicionamento
            int X = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
            int Y = 30;
            this.Location = new Point(X, Y);        

            //Imagens
            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();

            //Popula campos
            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            {
                _serial = percRegistry.GetSerialNumber();
            }

            string dataInstalacao = string.Empty;
            using (IOConnectRegistry icntRegistry = new IOConnectRegistry())
            {
                long timestamp = icntRegistry.GetDatainstalacao();
                dataInstalacao =
                    DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime.ToShortDateString();
            }

            AssemblyInfo info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            _guid = info.Guid;

            lblSerial.Text = _serial;
            lblVersao.Text = info.AssemblyComercialVersion;
            lblDataInstalação.Text = dataInstalacao;

            Globalizar();



            this.parametros = Util.ObjectParametros.Load();

            cboIdioma.DataSource = EnumHelper.ToList<Idioma>();           
            cboIdioma.SelectedIndex = this.parametros.IdIdioma - 1;

            /* Lista dispositivos */
            ResourceManager resource = null;
            if(this.parametros.IdIdioma == 2)
            {
                resource = new ResourceManager(typeof(Properties.IOConnect_esp));
            }
            if (parametros.IdIdioma == 3)
            {
                resource = new ResourceManager(typeof(Properties.IOConnect_eng));
            }
            else
            {
                resource = new ResourceManager(typeof(Properties.IOConnect));
            }
            
            cboDispositivo.DisplayMember = "Display";
            cboDispositivo.ValueMember = "Value";
            cboDispositivo.DataSource = EnumHelper.ToComboItemList<Dispositivo>(resource);
            cboDispositivo.SelectedValue = this.parametros.IdDispositivo;
            cboDispositivo.Enabled = false;
        }

        #region Eventos

        private void EditarVolumeColorante_Paint(object sender, PaintEventArgs e)
        {
            Color COR_BORDA = pnlBarraTitulo.BackColor;
            int BORDER_SIZE = 1;

            Form frm = (Form)sender;
            ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
            COR_BORDA, BORDER_SIZE, ButtonBorderStyle.Solid,
            COR_BORDA, BORDER_SIZE, ButtonBorderStyle.Solid,
            COR_BORDA, BORDER_SIZE, ButtonBorderStyle.Solid,
            COR_BORDA, BORDER_SIZE, ButtonBorderStyle.Solid);
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void lnkRedefineManutencao_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (fValidadeLicensa vToken = new fValidadeLicensa(_serial, _guid))
            {
                if (vToken.ShowDialog() == DialogResult.OK)
                {
                    Manutencao manutencao = new Manutencao();
                    string data = manutencao.Validade.LocalDateTime.ToShortDateString();
                    string restante = manutencao.GetLabelTempoRestante();
                    lblValidadeManutencao.Text = $"{data} - {restante}";
                }
            }
        }

        private void cboIdioma_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Util.ObjectParametros.SetIdIdioma(cboIdioma.SelectedIndex + 1);
            Util.ObjectParametros.InitLoad();
			this.parametros = Util.ObjectParametros.Load();
			Init.DefineCultura();
			Negocio.IdiomaResx.GetIDiomaREsx(parametros.IdIdioma); // Reload do idioma para o sistema em geral
			Util.ObjectMensagem.LoadMessage();
			Globalizar(); // Reload do idioma para a tela Sobre
        }       

        #endregion

        void Globalizar()
        {
            //Util.ObjectParametros parametros = Util.ObjectParametros.Load();
            this.parametros = Util.ObjectParametros.Load();
            /*
            /*
            //Portugues e Espanhol
            if (parametros.IdIdioma == 1 || parametros.IdIdioma == 2)
            {
                this.label_0.Text = Properties.IOConnect.Sobre_Label_0000;
                this.lblLegendaSerial.Text = Properties.IOConnect.Sobre_Label_0001;
                this.label_3.Text = Properties.IOConnect.Sobre_Label_0003;
                this.label_4.Text = Properties.IOConnect.Sobre_Label_0004;
                this.label_5.Text = Properties.IOConnect.Sobre_Label_0005;
                this.label_6.Text = Properties.IOConnect.Sobre_Label_0006;
                this.lnkRedefinirManutencao.Text =
                    Properties.IOConnect.Sobre_lnkRedefinirManutencao;
                this.label_7.Text = Properties.IOConnect.Sobre_Label_0007;
            }
            //Ingles
            else if (parametros.IdIdioma == 3)
            {
                this.label_0.Text = Properties.IOConnect_eng.Sobre_Label_0000;
                this.lblLegendaSerial.Text = Properties.IOConnect_eng.Sobre_Label_0001;
                this.label_3.Text = Properties.IOConnect_eng.Sobre_Label_0003;
                this.label_4.Text = Properties.IOConnect_eng.Sobre_Label_0004;
                this.label_5.Text = Properties.IOConnect_eng.Sobre_Label_0005;
                this.label_6.Text = Properties.IOConnect_eng.Sobre_Label_0006;
                this.lnkRedefinirManutencao.Text =
                    Properties.IOConnect_eng.Sobre_lnkRedefinirManutencao;
                this.label_7.Text = Properties.IOConnect_eng.Sobre_Label_0007;
            }
            */
            //Negocio.IdiomaResx.GetIDiomaREsx(parametros.IdIdioma);

            //Negocio.IdiomaResxExtensao.SetIDiomaRex();

            this.label_0.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0000;
            this.lblLegendaSerial.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0001;
            this.label_3.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0003;
            this.label_4.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0004;
            this.label_5.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0005;
            this.label_6.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0006;
            this.lnkRedefinirManutencao.Text = Negocio.IdiomaResxExtensao.Sobre_lnkRedefinirManutencao;
            this.label_7.Text = Negocio.IdiomaResxExtensao.Sobre_Label_0007;


            /* A data de manutenção é carregada na rotina de globalização pois
             * utiliza o método GetLabelTempoRestante que carrega o label de tempo 
             * restante de acordo com o idioma selecionado.
             * Se não estiver aqui, quando alterado o idioma, a label de tempo 
             * não será atualizada com o idioma correto. */
            Manutencao manutencao = new Manutencao();
            string data = manutencao.Validade.LocalDateTime.ToShortDateString();
            string restante = manutencao.GetLabelTempoRestante();
            lblValidadeManutencao.Text = $"{data} - {restante}";
        }

        private void btnStatusDump_Click(object sender, EventArgs e)
        {
            try
            {
                StatusDump stD = new StatusDump();

                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                string strVersion = $"IOConnect "+ $"{version.Major}.{version.Minor}.{version.Build}";
                version = null;

                stD.VersionIOConnect = strVersion;
                stD.NumeroSerie = _serial;
                List<Util.ObjectColorante> lCol = Util.ObjectColorante.List();
                lCol = lCol.FindAll(o => o.Habilitado).ToList().OrderBy(o=>o.Circuito).ToList();
                foreach (Util.ObjectColorante _col in lCol)
                {
                    stD.listProdutos.Add(_col);
                }
                foreach(Util.ObjectColorante _col in stD.listProdutos)
                {
                    stD.listCalibragem.Add(Util.ObjectCalibragem.Load(_col.Circuito));
                }
                string PathFile = Path.Combine(Environment.CurrentDirectory, "status.json");
                if (File.Exists(PathFile))
                {
                    File.Delete(PathFile);
                    Thread.Sleep(2000);
                }
                string jsp = JsonConvert.SerializeObject(stD);
                using (StreamWriter sW = new StreamWriter(File.Open(PathFile, FileMode.Create), Encoding.GetEncoding("ISO-8859-1")))
                {
                    sW.WriteLine(jsp);
                    sW.Close();
                }
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    m.ShowDialog("Exportação de Status realizada com sucesso! ");
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
    }
}