using Percolore.Core.Util;
using System.Drawing;
using System.Windows.Forms;

namespace Percolore.Core.UserControl
{
	/// <summary>
	/// Interface para exibição de mensagens.
	/// </summary>
	public partial class Mensagem : Form
    {
        private bool yes = false;

        public enum TipoMensagem
        {
            Informacao, Confirmacao, Erro
        };

        public Mensagem(TipoMensagem tipo)
        {
            InitializeComponent();

            /* É necessário indicar topMost aqui para que o form seja 
             * * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = true;


            //[Adequa largura do form à da tela]
            this.Width = Screen.PrimaryScreen.Bounds.Width;

            //[Posicionamento do form]
            this.Location = new Point(0, 30);

            Color ForeColor = Color.FromArgb(250, 250, 250);
            Color formBackColor = Color.FromArgb(75, 178, 89);
            Color buttonbackColor = Color.FromArgb(71, 165, 77);

            Image _imagem = null;

            switch (tipo)
            {
                case TipoMensagem.Erro:
                    {
                        #region Mensagem de erro

                        formBackColor = Color.FromArgb(50, 50, 50);
                        buttonbackColor = Color.FromArgb(30, 30, 30);
                        _imagem =
                           ImageHelper.Base64ToImage(Properties.Imagens.MensagemErro);

                        break;

                        #endregion
                    }
                case TipoMensagem.Informacao:
                    {
                        #region Mensagem de informação

                        formBackColor = Color.FromArgb(75, 178, 89);
                        buttonbackColor = Color.FromArgb(71, 165, 77);
                        _imagem =
                            ImageHelper.Base64ToImage(Properties.Imagens.MesnagemInformacao);

                        break;

                        #endregion
                    }
                case TipoMensagem.Confirmacao:
                    {
                        #region Mensagem de confirmação

                        formBackColor = Color.FromArgb(210, 15, 0);
                        buttonbackColor = Color.FromArgb(195, 15, 0);
                        _imagem =
                            ImageHelper.Base64ToImage(Properties.Imagens.MensagemInterrogacao);


                        break;

                        #endregion
                    }
            }

            #region Aplica parâmetros aos controles

            BackColor = formBackColor;
            pct01.Image = _imagem;

            btSave.BackColor = buttonbackColor;
            btSave.FlatAppearance.BorderColor = buttonbackColor;

            btCancel.BackColor = buttonbackColor;
            btCancel.FlatAppearance.BorderColor = buttonbackColor;

            lblText.ForeColor = ForeColor;
            btCancel.ForeColor = ForeColor;
            btSave.ForeColor = ForeColor;
            btSave.FlatAppearance.BorderColor = ForeColor;

            #endregion
        }

        public bool ShowDialog(string message)
        {
            lblText.Text = message;
            btSave.Visible = false;
            btCancel.Text = "Ok";
            
            //Centraliza botão de cancelmento
            btCancel.Left = (this.Width / 2) - (btCancel.Width / 2);

            base.ShowDialog();
            return yes;
        }

        public bool ShowDialog(string message, string action)
        {
            lblText.Text = message;
            btSave.Text = action;
            base.ShowDialog();

            return yes;
        }

        public bool ShowDialog(string message, string action, string nonaction)
        {
            lblText.Text = message;
            btSave.Text = action;
            btCancel.Text = nonaction;
            base.ShowDialog();

            return yes;
        }


        void Cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void Salvar_Click(object sender, EventArgs e)
        {
            yes = true;
            this.Close();
        }

        private void Mensagens_Load(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
           * redesenhando em primeiro plano sobre outra aplicação quando
           * houver outra aplicação rodando. */
            this.TopMost = true;
        }
    }
}
