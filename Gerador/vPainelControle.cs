using Percolore.Core;
using Percolore.Core.Util;
using System.Reflection;

namespace Percolore.Gerador
{
	public partial class vPainelControle : Form
    {
        public vPainelControle()
        {
            InitializeComponent();

            //Posicionamento
            int X = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
            int Y = 30;
            this.Location = new Point(X, Y);

            AssemblyInfo info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            lblTitulo.Text = $"Gerador de chaves - {info.AssemblyComercialVersion}";

            #region Configura imagem e texto dos botões

            btnSair.Text = string.Empty;
            btnSair.Image =
                ImageHelper.Base64ToImage(Properties.Resources.Sair);

            #endregion
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

        private void btnLicense_Click(object sender, EventArgs e)
        {
            using (vLicenca l = new vLicenca())
            {
                l.ShowDialog();
            }
        }

        private void btnAccessToken_Click(object sender, EventArgs e)
        {
            using (vTokenAcesso ta = new vTokenAcesso())
            {
                ta.ShowDialog();
            }
        }

        private void btnMaintenanceValidity_Click(object sender, EventArgs e)
        {
            using (vTokenValidadeManutencao ta = new vTokenValidadeManutencao())
            {
                ta.ShowDialog();
            }
        }
    }
}