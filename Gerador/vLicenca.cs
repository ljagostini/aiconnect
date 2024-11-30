using Percolore.Core.Security.License;
using Percolore.Core.UserControl;
using Percolore.Core.Util;

namespace Percolore.Gerador
{
	public partial class vLicenca : Form
    {
        public vLicenca()
        {
            InitializeComponent();

            //Posicionamento
            int X = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
            int Y = 30;
            this.Location = new Point(X, Y);

            #region Configura imagem e texto dos botões

            btnSair.Text = string.Empty;
            btnSair.Image =
                ImageHelper.Base64ToImage(Properties.Resources.Sair);

            #endregion

            txtKey.HabilitarTecladoVirtual = vPainelControle.HabilitarTecladoVirtual;
            txtLicence.HabilitarTecladoVirtual = vPainelControle.HabilitarTecladoVirtual;
        }

        private void vPainelControle_Paint(object sender, PaintEventArgs e)
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

        private void btnSair_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnGerar_Click(object sender, System.EventArgs e)
        {
            string key = txtKey.Text.Trim();
            if (key.Length != 12)
            {
                using (Mensagem m = new Mensagem(Mensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog("Chave informada é inválida");
                }

                txtKey.Focus();
                return;
            }

            //5a4fe5ab944b
            MACLicenseKey macLicenseKey = new MACLicenseKey(key);
            txtLicence.Text = macLicenseKey.License;
        }
    }
}