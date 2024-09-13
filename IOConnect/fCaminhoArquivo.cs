using Percolore.Core.Persistence.Xml;
using Percolore.Core.UserControl;
using Percolore.Core.Util;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Percolore.IOConnect
{
    public partial class fCaminhoArquivo : Form
    {
        Util.ObjectParametros _parametros = null;
        string _currentPathFile;
        string _filePath = string.Empty;

        public string FilePath
        {
            get { return _filePath; }
        }

        public fCaminhoArquivo(string currentPathFile)
        {
            InitializeComponent();

            //Posicionamento
            int X = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
            int Y = 30;
            this.Location = new Point(X, Y);

            _parametros = Util.ObjectParametros.Load();
            _currentPathFile = currentPathFile;

            //Globalização
            /*
            this.lblTitulo.Text = Properties.IOConnect.CaminhoArquivo_lblTitulo;
            this.lblDiretorio.Text = Properties.IOConnect.CaminhoArquivo_lblDiretorio;
            this.lblNome.Text = Properties.IOConnect.CaminhoArquivo_lblNome;
            */
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.CaminhoArquivo_lblTitulo;
            this.lblDiretorio.Text = Negocio.IdiomaResxExtensao.CaminhoArquivo_lblDiretorio;
            this.lblNome.Text = Negocio.IdiomaResxExtensao.CaminhoArquivo_lblNome;
           

            if (!string.IsNullOrEmpty(_currentPathFile))
            {
                txtDiretorio.Text = Path.GetDirectoryName(currentPathFile);
                txtArquivo.Text = Path.GetFileName(currentPathFile);
            }

            #region Configura imagem e texto dos botões

            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnConfirmar.Text = string.Empty;
            btnConfirmar.Image = Imagem.GetConfirmar_32x32();
            btnEditarDiretorio.Text = "...";

            #endregion            
        }

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

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            string diretorio = txtDiretorio.Text;
            string arquivo = txtArquivo.Text;

            if (FileHelper.ContainsInvalidChars(arquivo))
                arquivo = FileHelper.RemoveInvalidChars(arquivo);

            if (string.IsNullOrEmpty(txtArquivo.Text))
            {
                using (fMensagem f = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    f.ShowDialog(
                         Negocio.IdiomaResxExtensao.Global_Informacao_ArquivoNaoInformadoOuInvalido);
                }

                return;
            }

            _filePath = Path.Combine(diretorio, arquivo);
            DialogResult = DialogResult.OK;
        }

        private void btnEditarDiretorio_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (d.ShowDialog() == DialogResult.OK)
                    txtDiretorio.Text = d.SelectedPath;
            }
        }

        private void fCaminhoArquivo_Load(object sender, EventArgs e)
        {
            try
            {

                Util.ObjectParametros _parametros = Util.ObjectParametros.Load();

                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;


                txtArquivo.isTecladoShow = chb_tec;
                txtArquivo.isTouchScrenn = chb_touch;

                txtDiretorio.isTecladoShow = chb_tec;
                txtDiretorio.isTouchScrenn = chb_touch;

            }
            catch
            { }
        }
    }
}
