using Percolore.Core.Persistence.Xml;
using Percolore.Core.UserControl;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Percolore.IOConnect
{
    public partial class fNumeroSerie : Form
    {
        private string _serial = string.Empty;
        private Util.ObjectParametros _parametros;

        public string Serial
        {
            get { return _serial; }
        }

        public fNumeroSerie()
        {
            InitializeComponent();

            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);

            //Globalização
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.NumeroSerie_lblTitulo;
            this.lblNumeroSerie.Text = Negocio.IdiomaResxExtensao.NumeroSerie_lblNumeroSerie;

            btnCancelar.Text = string.Empty;
            btnCancelar.Image = Imagem.GetCancelar_48x48();
            btnConfirmar.Text = string.Empty;
            btnConfirmar.Image = Imagem.GetConfirmar_48x48();
            _parametros = Util.ObjectParametros.Load();
            updateTeclado();
        }

        private void updateTeclado()
        {
            bool ischek = false;
            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTecladoVirtual;
            }
            txtSerial.isTecladoShow = ischek;

            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTouchScrenn;
            }
            txtSerial.isTouchScrenn = ischek;
        }

        private void btnCancelar_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnConfirmar_Click(object sender, System.EventArgs e)
        {
            _serial = txtSerial.Text.Trim();
            if (_serial.Length != 4)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_NumeroSerialInvalido);
                }

                txtSerial.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtSerial_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnConfirmar_Click(null, null);
                }
            }
            catch
            { }
        }
    }
}
