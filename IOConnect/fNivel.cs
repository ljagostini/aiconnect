using Percolore.Core.Logging;
using Percolore.Core.Util;
using System.Data;

namespace Percolore.IOConnect
{
	/* Considerações - 05/09/2016
     * Esta tela controla a edição de nível de colorante.
     * 
     * Independentemente da unidade de medida em que o usuário 
     * informar a quantidade e da exibição em mililitros estar
     * desabilitada, a quantidade será definida primeiramento 
     * para esta unidade e após convertida para as demais.
     */
	public partial class fNivel : Form
    {
        List<Util.ObjectColorante> _colorantes = null;
        Util.ObjectParametros _parametros = null;
        //UnidadeMedida _unidade;

        List<ObjDropBox> lDropBox = new List<ObjDropBox>();

        private List<Util.ObjectAbastecimento> listAbastecimento = new List<Util.ObjectAbastecimento>();

        public fNivel(List<Util.ObjectColorante> colorantes)
        {
            InitializeComponent();
            this._colorantes = colorantes;
            Inicializar();
           
        }

        public fNivel(Util.ObjectColorante colorante)
        {
            InitializeComponent();
            this._colorantes = new List<Util.ObjectColorante> { colorante };
            Inicializar();

        }

        #region Eventos

        private void EditarVolumeColorante_Paint(object sender, PaintEventArgs e)
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
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            ObjDropBox opd = lDropBox[cboUnidade.SelectedIndex];
            int _id_unidade = Convert.ToInt32(opd.value);
            bool isOk = true;
            double valorCount = txtDecimal.ToDouble() + txtOncaY.ToInt() + txtOnca48.ToDouble();
            switch (_id_unidade)
            {
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro:
                    {
                        if (valorCount == 0)
                        {
                            isOk = false;
                        }
                        break;
                    }
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Onca:
                    {
                        if (valorCount == 0)
                        {
                            isOk = false;
                        }
                        break;
                    }
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Shot:
                    {
                        if (valorCount == 0)
                        {
                            isOk = false;
                        }
                        break;
                    }
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Grama:
                    {
                        if (valorCount == 0)
                        {
                            isOk = false;
                        }
                        break;
                    }
                default:
                    {
                        if (_id_unidade <= 0)
                        {
                            isOk = false;
                        }
                        break;
                    }
            }
           
            if (isOk)
            {
                switch (_id_unidade)
                {
                    case (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro:
                        {
                            double mililitro = txtDecimal.ToDouble();
                            _colorantes.ForEach(c => c.Volume += mililitro);
                            break;
                        }
                    case (int)Percolore.IOConnect.Core.UnidadeMedida.Onca:
                        {
                            double mililitro = Percolore.IOConnect.Core.UnidadeMedidaHelper.OncaFracionadaToMililitro(
                                txtOncaY.ToInt(), txtOnca48.ToDouble());

                            _colorantes.ForEach(c => c.Volume += mililitro);
                            break;
                        }
                    case (int)Percolore.IOConnect.Core.UnidadeMedida.Shot:
                        {
                            double mililitro = Percolore.IOConnect.Core.UnidadeMedidaHelper.ShotToMililitro(
                                txtDecimal.ToDouble(), _parametros.ValorShot);

                            _colorantes.ForEach(c => c.Volume += mililitro);
                            break;
                        }
                    case (int)Percolore.IOConnect.Core.UnidadeMedida.Grama:
                        {
                            double grama = txtDecimal.ToDouble();

                            _colorantes.ForEach
                                (
                                    c => c.Volume +=
                                        Percolore.IOConnect.Core.UnidadeMedidaHelper.GramaToMililitro(grama, c.MassaEspecifica)
                                );
                            break;
                        }
                    default:
                        {
                            if (_id_unidade > 0)
                            {
                                Util.ObjectAbastecimento abast = this.listAbastecimento.Find(o => o.Id == _id_unidade);
                                if (abast != null)
                                {
                                    if (abast.UnMed == (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro)
                                    {
                                        double mililitro = Convert.ToDouble(abast.Conteudo);
                                        _colorantes.ForEach(c => c.Volume += mililitro);
                                    }
                                    else
                                    {
                                        double grama = Convert.ToDouble(abast.Conteudo);
                                        _colorantes.ForEach
                                            (
                                            c => c.Volume += Percolore.IOConnect.Core.UnidadeMedidaHelper.GramaToMililitro(grama, c.MassaEspecifica)
                                            );
                                    }
                                }

                            }
                            break;
                        }
                }

                if (_parametros.ControlarNivel)
                {
                    #region Nível não pode exceder ao máximo configurado

                    string excedentes = string.Empty;
                    bool primeiro = true;

                    foreach (Util.ObjectColorante c in this._colorantes)
                    {
                        //if (c.Volume > _parametros.VolumeMaximo)
                        if (c.Volume > c.NivelMaximo)
                        {
                            excedentes += (primeiro ? "" : ", ") + c.Nome;
                            primeiro = false;
                        }
                    }

                    if (!string.IsNullOrEmpty(excedentes))
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            string mensagem =
                                $"{Negocio.IdiomaResxExtensao.Nivel_Informacao_ExcedeNivelMaximo} " + excedentes;
                            m.ShowDialog(mensagem);
                        }

                        return;
                    }

                    #endregion
                }

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    if (!m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Nivel_Confirmar_AbastecimentoColorante, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao))
                    {
                        return;
                    }
                }

                try
                {
                    Util.ObjectColorante.Persist(this._colorantes);

                    #region Log de processo

                    string log = string.Empty;
                    if (this._colorantes.Count == 1)
                        log = $"{Negocio.IdiomaResxExtensao.Global_CircuitoColoranteAbastecido} {_colorantes[0].Nome}.";
                    else
                        log = Negocio.IdiomaResxExtensao.Global_TodosCircuitosColoranteAbastecidos;

                    Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, log);

                    #endregion

                    //Define diálogo e fecha form
                    DialogResult = DialogResult.OK;
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        string mensagem =
                            Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message;
                        m.ShowDialog(mensagem);
                    }

                    DialogResult = DialogResult.Abort;
                }
            }
            else
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Nivel_Informacao_NivelMaiorZero);
                }
            }
        }

        private void EditarNivelColorante_Load(object sender, EventArgs e)
        {
            /* Necessário para que formulário abra efetivamente em primeiro plano */
            this.TopMost = true;
            cboUnidade.SelectedIndex = -1;
        }

        private void cboUnidade_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pnlDecimal.Visible = false;
            txtDecimal.Text = string.Empty;
            pnlOnca.Visible = false;
            txtOncaY.Text = string.Empty;
            txtOnca48.Text = string.Empty;

            ObjDropBox opd = lDropBox[cboUnidade.SelectedIndex];
            int _id_unidade = Convert.ToInt32(opd.value);
            switch(_id_unidade)
            {
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro:
                    {
                        lblDecimal.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Mililitro;
                        pnlDecimal.Visible = true;
                        break;
                    }                
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Onca:
                    {
                        pnlOnca.Visible = true;
                        break;
                    }
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Shot:
                    {
                        lblDecimal.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Shot;
                        pnlDecimal.Visible = true;
                        break;
                    }
                case (int)Percolore.IOConnect.Core.UnidadeMedida.Grama:
                    {
                        lblDecimal.Text = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Grama;
                        pnlDecimal.Visible = true;
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
            

            //_unidade = ((UnidadeMedida)cboUnidade.SelectedItem);
            /*
            _unidade = (UnidadeMedida)((ComboBoxItem)(cboUnidade.SelectedItem)).Value;
            switch (_unidade)
            {
                case UnidadeMedida.Mililitro:
                    {
                        lblDecimal.Text =
                            Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Mililitro;
                        pnlDecimal.Visible = true;
                        break;
                    }
                case UnidadeMedida.Onca:
                    {
                        pnlOnca.Visible = true;
                        break;
                    }
                case UnidadeMedida.Shot:
                    {
                        lblDecimal.Text =
                            Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Shot;
                        pnlDecimal.Visible = true;
                        break;
                    }
                case UnidadeMedida.Grama:
                    {
                        lblDecimal.Text =
                            Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Grama;
                        pnlDecimal.Visible = true;
                        break;
                    }
                default:
                    break;
            }
            */
        }

        #endregion

        #region Métodos 

        void Inicializar()
        {
            //[Posicionamento]
            int X = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
            int Y = 30;
            this.Location = new Point(X, Y);

#if DEBUG
            this.TopMost = false;
#endif

            #region Configura botões

            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnTeclado.Text = string.Empty;
            btnTeclado.Image = Imagem.GetTeclado_32x32();
            btnGravar.Text = string.Empty;
            btnGravar.Image = Imagem.GetGravar_32x32();

            #endregion

            _parametros = Util.ObjectParametros.Load();

            //Globalização
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.Nivel_lblTitulo;
            this.lblUnidadeMedida.Text = Negocio.IdiomaResxExtensao.Nivel_lblUnidadeMedida;

            #region Listar unidades de medida configuradas

            /* Lista de unidades de medida que devem ser ignoradas 
            IList<UnidadeMedida> d = new List<UnidadeMedida>();
            if (!_parametros.HabilitarMililitro) d.Add(UnidadeMedida.Mililitro);
            if (!_parametros.HabilitarShot) d.Add(UnidadeMedida.Shot);
            if (!_parametros.HabilitarOnca) d.Add(UnidadeMedida.Onca);
            if (!_parametros.HabilitarGrama) d.Add(UnidadeMedida.Grama);

            //Resource da aplicação
            ResourceManager resource = null;
            if (_parametros.IdIdioma == 2)
            {
                resource = new ResourceManager(typeof(Properties.IOConnect_esp));
            }
            else if (_parametros.IdIdioma == 3)
            {
                resource = new ResourceManager(typeof(Properties.IOConnect_eng));
            }
            else
            {
                resource = new ResourceManager(typeof(Properties.IOConnect));
            }

            //cboUnidade.DataSource = EnumHelper.ToList(d);
            cboUnidade.DataSource = EnumHelper.ToComboItemList(resource, d);
            */

            this.listAbastecimento = Util.ObjectAbastecimento.List();
            foreach(Util.ObjectAbastecimento objAbast in listAbastecimento)
            {
                objAbast.Id += 4;
            }

            DataTable dtUnMed = new DataTable();
            dtUnMed.Columns.Add("UnMed");
            dtUnMed.Columns.Add("descricao");

            if (_parametros.HabilitarMililitro)
            {
                DataRow drM = dtUnMed.NewRow();
                drM["UnMed"] = (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro + "";
                drM["descricao"] = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Mililitro;
                dtUnMed.Rows.Add(drM);

                ObjDropBox odp = new ObjDropBox();
                odp.display = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Mililitro;
                odp.value = (int)Percolore.IOConnect.Core.UnidadeMedida.Mililitro + "";
                lDropBox.Add(odp);

            }
            if (_parametros.HabilitarShot)
            {
                DataRow drS = dtUnMed.NewRow();
                drS["UnMed"] = (int)Percolore.IOConnect.Core.UnidadeMedida.Shot + "";
                drS["descricao"] = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Shot;
                dtUnMed.Rows.Add(drS);

                ObjDropBox odp = new ObjDropBox();
                odp.display = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Shot;
                odp.value = (int)Percolore.IOConnect.Core.UnidadeMedida.Shot + "";
                lDropBox.Add(odp);
            }
            if (_parametros.HabilitarOnca)
            {
                DataRow drO = dtUnMed.NewRow();
                drO["UnMed"] = (int)Percolore.IOConnect.Core.UnidadeMedida.Onca + "";
                drO["descricao"] = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Onca;
                dtUnMed.Rows.Add(drO);

                ObjDropBox odp = new ObjDropBox();
                odp.display = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Onca;
                odp.value = (int)Percolore.IOConnect.Core.UnidadeMedida.Onca + "";
                lDropBox.Add(odp);
            }
            if (_parametros.HabilitarGrama)
            {
                DataRow drG = dtUnMed.NewRow();
                drG["UnMed"] = (int)Percolore.IOConnect.Core.UnidadeMedida.Grama + "";
                drG["descricao"] = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Grama;
                dtUnMed.Rows.Add(drG);

                ObjDropBox odp = new ObjDropBox();
                odp.display = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Grama;
                odp.value = (int)Percolore.IOConnect.Core.UnidadeMedida.Grama + "";
                lDropBox.Add(odp);
            }

            for(int i = 0; this.listAbastecimento != null && i < this.listAbastecimento.Count; i++)
            {
                DataRow drA = dtUnMed.NewRow();
                drA["UnMed"] = this.listAbastecimento[i].Id + "";
                drA["descricao"] = this.listAbastecimento[i].Nome;
                dtUnMed.Rows.Add(drA);

                ObjDropBox odp = new ObjDropBox();
                odp.display = this.listAbastecimento[i].Nome;
                odp.value = this.listAbastecimento[i].Id + "";
                lDropBox.Add(odp);
            }

            cboUnidade.DisplayMember = "descricao";
            cboUnidade.ValueMember = "UnMed";

            cboUnidade.DataSource = dtUnMed.DefaultView;
            


            #endregion

            updateTeclado();
        }

        private void updateTeclado()
        {
            bool ischek = false;
            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTecladoVirtual;
            }
            txtDecimal.isTecladoShow = ischek;
            txtOnca48.isTecladoShow = ischek;
            txtOncaY.isTecladoShow = ischek;

            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTouchScrenn;
            }
            txtDecimal.isTouchScrenn = ischek;
            txtOnca48.isTouchScrenn = ischek;
            txtOncaY.isTouchScrenn = ischek;
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

        private class ObjDropBox
        {
            public string display{ get; set; }
            public string value { get; set; }

        }
    }
}
