using Percolore.Core;
using Percolore.Core.AccessControl;
using Percolore.Core.Security.Token;
using Percolore.Core.UserControl;
using Percolore.Core.Util;
using System.Text;

namespace Percolore.Gerador
{
	public partial class vTokenAcesso : Form
    {
        public vTokenAcesso()
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

            /* Lista aplicações da Percolore */
            cboAplicacao.DataSource = new AppPercolore().AppList;
            cboAplicacao.DisplayMember = "Name";
            cboAplicacao.ValueMember = "Guid";
            
            /* Lista perfis de acesso*/
            cboPerfil.DataSource = EnumHelper.ToList<Profile>();
        }

        #region Eventos

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
            string seriaInput = txtSerial.Text.Trim();
            AppIdentifier appInput = (AppIdentifier)cboAplicacao.SelectedItem;
            int hourInput = txtPrazoExpiracao.ToInt();
            int profileInput = (int)cboPerfil.SelectedItem;

            #region Validação

            StringBuilder validacao = new StringBuilder();

            if (seriaInput.Length < 4)
                validacao.AppendLine("O número serial deve ter 4 dígitos.");

            if (appInput == null)
                validacao.AppendLine("É necessário selecionar a aplicação para qual o token será gerado.");

            if (hourInput < 1 || hourInput > 720)
                validacao.AppendLine("O prazo de expiração deve ser um valor entre 1 e 720.");

            if (profileInput == 0)
                validacao.AppendLine("É necessário selecionar o perfil de usuário do token.");

            if (validacao.Length > 0)
            {
                using (Mensagem m = new Mensagem(Mensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(validacao.ToString());
                }

                return;
            }

            #endregion

            try
            {
                TokenTargetIdentity target = new TokenTargetIdentity(seriaInput, appInput.Guid);
                AccessTokenModel model =
                    new AccessTokenModel(
                        DateTimeOffset.UtcNow.AddHours(hourInput), profileInput);
                ITokenHandler<AccessTokenModel> handler = new TokenHandler<AccessTokenModel>();
                txtToken.Text = handler.Create(target, model);

            }
            catch (Exception ex)
            {
                using (Mensagem m = new Mensagem(Mensagem.TipoMensagem.Erro))
                {
                    string msg = "Falha ao gerar token de acesso."
                        + Environment.NewLine + Environment.NewLine
                        + $"Descrição [{ex.Message}]";

                    m.ShowDialog(msg);
                }
            }
        }

        #endregion

        #region Override CreateParams [Corrigir cintilação]

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        #endregion
    }
}