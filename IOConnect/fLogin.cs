using Percolore.Core.Logging;

namespace Percolore.IOConnect
{
	public partial class fLogin : Form
    {
        private Util.ObjectParametros _parametros = null;
        Util.ObjectUser usuario;

        public Util.ObjectUser Usuario
        {
            get { return usuario; }
        }

        public fLogin()
        {
            InitializeComponent();
           
        }

        private void fLogin_Load(object sender, EventArgs e)
        {
            try
            {
                _parametros = Util.ObjectParametros.Load();
                updateTeclado();
                lblMensagem.Text = string.Empty;
                pct01.Image = Properties.IOConnect.Login_164x164;
                lblUsuario.Text = Negocio.IdiomaResxExtensao.fAutenticacao_lblUsuario;
                lblSenha.Text = Negocio.IdiomaResxExtensao.fAutenticacao_lblSenha;
                btnEntrar.Text = Negocio.IdiomaResxExtensao.fAutenticacao_btnEntrar;
                btnCancelar.Text = Negocio.IdiomaResxExtensao.fAutenticacao_btnCancelar;
               
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void updateTeclado()
        {
            try
            {
                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;
                txtSenha.isTecladoShow = chb_tec;
                txtUsuario.isTecladoShow = chb_tec;
                txtSenha.isTouchScrenn = chb_touch;
                txtUsuario.isTouchScrenn = chb_touch;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string u = txtUsuario.Text.Trim();
            string s = txtSenha.Text.Trim();

            if (u.Length == 0 || s.Length == 0)
            {
                lblMensagem.Text = Negocio.IdiomaResxExtensao.fAutenticacao_Mensagem_UsuarioSenhaNaoInformado;
                return;
            }

            try
            {
                usuario = Util.ObjectUser.Load(u, s);
                if (usuario == null)
                {
                    lblMensagem.Text = Negocio.IdiomaResxExtensao.fAutenticacao_Mensagem_UsuarioSenhaInvalido;
                    return;
                }

                Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);

			    #region Exception

			    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(string.Format(Negocio.IdiomaResxExtensao.fAutenticacao_Falha_ValidarDadosUsuario,ex.Message));
                }

                Close();

                #endregion
            }
        }

        private void txtSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {                   
                    btnEntrar_Click(null, null);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    txtSenha.Select();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
    }
}