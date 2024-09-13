using Percolore.Core;
using Percolore.Core.AccessControl;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Persistence.Xml;
using Percolore.Core.Security.License;
using Percolore.Core.Security.Token;
using Percolore.Core.UserControl;
using Percolore.Core.Util;
using Percolore.IOConnect.Util;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace Percolore.IOConnect
{
    public partial class fAutenticacao : Form
    {
        private Authentication _authentication = null;
        private Util.ObjectParametros _parametros = null;

        Util.ObjectUser usuario;
        

        public Util.ObjectUser Usuario
        {
            get { return usuario; }
        }

        public Authentication Authentication
        {
            get { return _authentication; }
        }

        public fAutenticacao()
        {
            InitializeComponent();

            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Location = new Point(0, 30);
            _parametros = Util.ObjectParametros.Load();

            //Globalização  
            /*
            this.lblTitulo.Text = Properties.IOConnect.Autenticacao_lblTitulo;
            this.rdbSenha.Text = Properties.IOConnect.Autenticacao_rdbSenha;
            */
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.Autenticacao_lblTitulo;
            //this.rdbSenha.Text = Negocio.IdiomaResxExtensao.Autenticacao_rdbSenha;

            this.rdbSenha.Text = Negocio.IdiomaResxExtensao.fAutenticacao_lblUsuario;
            lblUsuario.Text = Negocio.IdiomaResxExtensao.Autenticacao_lblUsuario;
            lblSenha.Text = Negocio.IdiomaResxExtensao.Autenticacao_lblSenha;

            btnCancelar.Text = string.Empty;
            btnCancelar.Image = Imagem.GetCancelar_48x48();
            btnOk.Text = string.Empty;
            btnOk.Image = Imagem.GetConfirmar_48x48();

            btnCancelUser.Text = string.Empty;
            btnCancelUser.Image = Imagem.GetCancelar_48x48();
            btnConfirmaUser.Text = string.Empty;
            btnConfirmaUser.Image = Imagem.GetConfirmar_48x48();

            gbToken.Visible = true;
            gbUser.Visible = false;
          
            if(_parametros != null)
            {
                txtInput.isTecladoShow = _parametros.HabilitarTecladoVirtual;
                txtInput.isTouchScrenn = _parametros.HabilitarTouchScrenn;

                txtUsuario.isTecladoShow = _parametros.HabilitarTecladoVirtual;
                txtUsuario.isTouchScrenn = _parametros.HabilitarTouchScrenn;

                txtSenha.isTecladoShow = _parametros.HabilitarTecladoVirtual;
                txtSenha.isTouchScrenn = _parametros.HabilitarTouchScrenn;
               
            }
            
        }

        private void btnCancelar_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(btnOkClick));
        }


        private void btnOkClick()
        {
            string input = txtInput.Text.Trim();
            if (input.Length == 0)
            {
                #region Valida entrada 

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Autenticacao_DadosNaoInformados);
                    txtInput.Focus();
                }
                return;
                #endregion
            }
            #region Token

            AssemblyInfo assemblyInfo = new AssemblyInfo(Assembly.GetExecutingAssembly());
            string guid = assemblyInfo.Guid;

            string serial = string.Empty;
            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            {
                serial = percRegistry.GetSerialNumber();
            }

            ITokenHandler<AccessTokenModel> handler = new TokenHandler<AccessTokenModel>();
            AccessTokenModel model = handler.Read(input);
            TokenStatus status = handler.Validate(new TokenTargetIdentity(serial, guid), model);

            switch (status)
            {
                #region TokenStatus

                case TokenStatus.InvalidFormat:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_TokenInvalido);
                        }

                        txtInput.Focus();
                        return;
                    }
                case TokenStatus.Expired:
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_TokenExpirado);
                        };

                        txtInput.Focus();
                        return;
                    }

                    #endregion
            }

            _authentication = new Authentication((Profile)model.Profile);

            #endregion

            //AuthenticationMode mode =
            //    (rdbToken.Checked) ? AuthenticationMode.Token : AuthenticationMode.Password;

            //switch (mode)
            //{
            //    case AuthenticationMode.Token:
            //        {
            //            #region Token

            //            AssemblyInfo assemblyInfo =
            //                new AssemblyInfo(Assembly.GetExecutingAssembly());
            //            string guid = assemblyInfo.Guid;

            //            string serial = string.Empty;
            //            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            //            {
            //                serial = percRegistry.GetSerialNumber();
            //            }

            //            ITokenHandler<AccessTokenModel> handler =
            //                new TokenHandler<AccessTokenModel>();
            //            AccessTokenModel model = handler.Read(input);
            //            TokenStatus status = handler.Validate(
            //                new TokenTargetIdentity(serial, guid), model);

            //            switch (status)
            //            {
            //                #region TokenStatus

            //                case TokenStatus.InvalidFormat:
            //                    {
            //                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
            //                        {
            //                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_TokenInvalido);
            //                        }

            //                        txtInput.Focus();
            //                        return;
            //                    }
            //                case TokenStatus.Expired:
            //                    {
            //                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
            //                        {
            //                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_TokenExpirado);
            //                        };

            //                        txtInput.Focus();
            //                        return;
            //                    }

            //                    #endregion
            //            }

            //            _authentication =
            //                new Authentication((Profile)model.Profile);

            //            break;

            //            #endregion
            //        }
            //    case AuthenticationMode.Password:
            //        {
            //            #region Senha

            //            string senha = string.Empty;
            //            using (IOConnectRegistry icntRegistry = new IOConnectRegistry())
            //            {
            //                senha = icntRegistry.GetSenhaAdmnistrador();
            //            }

            //            if (txtInput.Text != senha)
            //            {
            //                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
            //                    m.ShowDialog(Negocio.IdiomaResxExtensao.Autenticacao_SenhaInvalida);
            //                txtInput.Focus();
            //                return;
            //            }

            //            _authentication =
            //                new Authentication(Profile.Administrador);

            //            break;

            //            #endregion
            //        }
            //}

            Close();
        }
        private void rdbToken_CheckedChanged(object sender, System.EventArgs e)
        {
            AtualizaGToken(true);
        }

        private void rdbSenha_CheckedChanged(object sender, System.EventArgs e)
        {
            AtualizaGToken(false);
        }

        private void AtualizaGToken(bool isToken)
        {
            if(isToken)
            {
                //txtInput.PasswordChar = '\0';
                txtInput.Text = string.Empty;
                txtInput.UseSystemPasswordChar = false;
                txtInput.Focus();
                gbToken.Visible = true;
                gbUser.Visible = false;
            }
            else
            {
                //txtInput.PasswordChar = '*';
                txtUsuario.Text = string.Empty;
                txtSenha.Text = string.Empty;
                txtUsuario.Focus();

                gbToken.Visible = false;
                gbUser.Visible = true;
            }
        }

        private void btnCancelUser_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnConfirmaUser_Click(object sender, EventArgs e)
        {
            string input = "avsb";

            string u = txtUsuario.Text.Trim();
            string s = txtSenha.Text.Trim();

            if (u.Length == 0 || s.Length == 0)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.fAutenticacao_Mensagem_UsuarioSenhaNaoInformado);
                    txtUsuario.Focus();
                }
                return;
            }

             #region Senha

            string senha = string.Empty;
            using (IOConnectRegistry icntRegistry = new IOConnectRegistry())
            {
                senha = icntRegistry.GetSenhaAdmnistrador();
            }

            if (input != senha)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Autenticacao_SenhaInvalida);
                }
                txtUsuario.Focus();
                return;
            }

            try
            {
                usuario = Util.ObjectUser.Load(u, s);
                if (usuario == null)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.fAutenticacao_Mensagem_UsuarioSenhaInvalido);
                        txtUsuario.Focus();
                    }
                    return;
                }



                Close();
            }
            catch (Exception ex)
            {
                #region Exception

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        string.Format(
                            Negocio.IdiomaResxExtensao.fAutenticacao_Falha_ValidarDadosUsuario,
                            ex.Message));
                }

                Close();

                #endregion
            }
            _authentication =
                new Authentication(Profile.Administrador);

            #endregion
         

            Close();
        }

        private void txtSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if(e.KeyChar == (char)Keys.Enter)
                {
                    btnConfirmaUser_Click(null, null);

                }
            }catch
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
