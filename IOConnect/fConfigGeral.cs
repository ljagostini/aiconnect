using Percolore.Core;
using Percolore.Core.AccessControl;
using Percolore.Core.Logging;

namespace Percolore.IOConnect
{
	public partial class fConfigGeral : Form
    {
        Authentication authentication;
        public Util.ObjectParametros _parametros = null;
        public fConfigGeral(Authentication auth, Util.ObjectParametros _par)
        {
            InitializeComponent();
            this.authentication = auth;
            this._parametros = _par;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            this._parametros.HabilitarTecladoVirtual = chkTecladoVirtual.Checked;
            this._parametros.HabilitarFormulaPersonalizada = chkFormulasPersonalizadas.Checked;
            this._parametros.HabilitarTesteRecipiente = chkTesteRecipiente.Checked;
            this._parametros.HabilitarIdentificacaoCopo = chkHabilitarIndentificacaoCopo.Checked;
            this._parametros.HabilitarDispensaSequencial = chkDispensaSequencial.Checked;
            this._parametros.HabilitarTouchScrenn = chkTouchScrenn.Checked;
            this._parametros.TreinamentoCal = chk_TreinamentoCal.Checked;
            this._parametros.DelayEsponja = txtDelayEsponja.ToInt();
            this._parametros.ViewMessageProc = chkViewMessageProc.Checked;
            this._parametros.NameRemoteAccess = txt_NameRemoteAccess.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void fConfigGeral_Load(object sender, EventArgs e)
        {
            Permissions permissions = this.authentication.Permissions;

            chkDispensaSequencial.Enabled =
                permissions.HabilitarDispensaSequencial;

            chkFormulasPersonalizadas.Enabled =
                permissions.HabilitarFormulaPersonalizada;

            chkTesteRecipiente.Checked =
                permissions.HabilitarTesteRecipiente;

            this.chkViewMessageProc.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkViewMessageProc;
            this.lblGeralFuncinamentoSoftware.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblGeralFuncinamentoSoftware;
            this.chkTecladoVirtual.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkTecladoVirtual;
            this.chkDispensaSequencial.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkDispensaSequencial;
            this.chkFormulasPersonalizadas.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkFormulasPersonalizadas;
            this.chkTesteRecipiente.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkTesteRecipiente;
            this.chkHabilitarIndentificacaoCopo.Text = Negocio.IdiomaResxExtensao.Configuracoes_chkIdenditicacaoCopo;

            this.lblDelayEsponja.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblDelayEsponja;
            this.lbl_NameRemoteAccess.Text = Negocio.IdiomaResxExtensao.NameRemoteAccess;

            chkTecladoVirtual.Checked = _parametros.HabilitarTecladoVirtual;
            chkDispensaSequencial.Checked = _parametros.HabilitarDispensaSequencial;

            chk_TreinamentoCal.Checked = _parametros.TreinamentoCal;

            chkFormulasPersonalizadas.Checked = _parametros.HabilitarFormulaPersonalizada;
            chkTesteRecipiente.Checked = _parametros.HabilitarTesteRecipiente;
            chkHabilitarIndentificacaoCopo.Checked = _parametros.HabilitarIdentificacaoCopo;

            txtDelayEsponja.Text = _parametros.DelayEsponja.ToString();

            chkTouchScrenn.Checked = _parametros.HabilitarTouchScrenn;

            chkViewMessageProc.Checked = _parametros.ViewMessageProc;

            txt_NameRemoteAccess.Text = _parametros.NameRemoteAccess;
            
            updateTeclado();

            Dispositivo dispositivo =(Dispositivo)_parametros.IdDispositivo;

            if (dispositivo == Dispositivo.Placa_1)
            {
                chkDispensaSequencial.Checked = true;
                chkDispensaSequencial.Enabled = false;
                chkTesteRecipiente.Checked = false;
                chkTesteRecipiente.Enabled = false;
                chkHabilitarIndentificacaoCopo.Checked = false;
                chkHabilitarIndentificacaoCopo.Enabled = false;
            }
            else
            {
                chkDispensaSequencial.Enabled = true;
                chkTesteRecipiente.Enabled = true;
                chkHabilitarIndentificacaoCopo.Enabled = true;
            }

            this.btnRessetData.Text = Negocio.IdiomaResxExtensao.Configuracoes_btnRessetarDatas;
            this.btnRessetData.BackColor = Cores.Seguir;

            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnConfirmar.Text = string.Empty;
            btnConfirmar.Image = Imagem.GetGravar_32x32();
        }

        private void updateTeclado()
        {
            bool chb_tec = _parametros.HabilitarTecladoVirtual;
            bool chb_touch = _parametros.HabilitarTouchScrenn;

            txt_NameRemoteAccess.isTecladoShow = chb_tec;
            txtDelayEsponja.isTecladoShow = chb_tec;

            txt_NameRemoteAccess.isTouchScrenn = chb_touch;
            txtDelayEsponja.isTouchScrenn = chb_touch;
        }

        private void btnRessetData_Click(object sender, EventArgs e)
        {
            try
            {
                if (Util.ObjectParametros.SetExecucaoPurga(DateTime.Now))
                {
                    if (Util.ObjectRecircular.UpdateRessetDate(DateTime.Now))
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Global_Sucesso_RessetData);
                        }
                        Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_Sucesso_RessetData);
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Global_Error_RessetData);
                        }
                    }
                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        m.ShowDialog(
                            Negocio.IdiomaResxExtensao.Global_Error_RessetData);
                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_RessetData + ex.Message);
                }
            }
        }
    }
}