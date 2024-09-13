using Percolore.Core;
using Percolore.Core.Security.Token;
using Percolore.Core.UserControl;
using Percolore.Core.Util;
using System.Text;

namespace Percolore.Gerador
{
	public partial class vTokenValidadeManutencao : Form
    {
        public vTokenValidadeManutencao()
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

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            /* Data e hora fixa pra todos os tokens de validade gerados 
             * Motivo: a data gerada é de validade; portanto; deve funcionar plenamente
             * por todo o último dia. */
            const string horaValidade = "23:59:59";

            string seriaInput = txtSerial.Text.Trim();
            AppIdentifier appInput = (AppIdentifier)cboAplicacao.SelectedItem;
            int hourInput = txtPrazoExpiracao.ToInt();
            string validadeInput = 
                mskValidadeManutencao.Text + " " + horaValidade;           
            DateTimeOffset dtValidade;

            #region Validação

            StringBuilder validacao = new StringBuilder();

            if (seriaInput.Length < 4)
                validacao.AppendLine("O número serial deve ter 4 dígitos.");

            if (appInput == null)
                validacao.AppendLine("É necessário selecionar a aplicação para qual o token será gerado.");

            if (hourInput < 1 || hourInput > 720)
                validacao.AppendLine("O prazo de expiração deve ser um valor entre 1 e 720.");

            if (DateTimeOffset.TryParse(validadeInput, out dtValidade))
            {
                if (dtValidade.UtcDateTime <= DateTimeOffset.Now)
                {
                    validacao.AppendLine("A data de validade da manutenção deve ser maior que a data atual.");
                }
            }
            else
            {
                validacao.AppendLine("Data inválida.");
            }

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
                TokenTargetIdentity target =
                    new TokenTargetIdentity(seriaInput, appInput.Guid);

                MaintenanceValidityTokenModel model =
                    new MaintenanceValidityTokenModel(
                        DateTimeOffset.UtcNow.AddHours(hourInput), dtValidade);

                ITokenHandler<MaintenanceValidityTokenModel> handler =
                    new TokenHandler<MaintenanceValidityTokenModel>();

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

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}

        #endregion
    }
}