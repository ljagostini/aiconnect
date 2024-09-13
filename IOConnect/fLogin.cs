using Percolore.Core.UserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            catch
            { }
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
            catch
            { }
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
            catch
            { }
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
            catch
            { }
        }
    }
}
