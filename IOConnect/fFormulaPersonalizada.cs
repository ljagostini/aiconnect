using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Util;
using System.Resources;

namespace Percolore.IOConnect
{
	public partial class fFormulaPersonalizada : Form
    {
        Util.ObjectParametros _parametros = null;
        Util.ObjectFormula _formula = null;
        Util.ObjectColorante _colorante = null;
        double _mililitros = 0;
        Percolore.IOConnect.Core.UnidadeMedida? _unidade = null;

        public fFormulaPersonalizada(Util.ObjectFormula formula = null)
        {
            InitializeComponent();

#if DEBUG
            this.TopMost = false;
#endif

            _parametros = Util.ObjectParametros.Load();
            _formula = (formula == null) ? new Util.ObjectFormula() : formula;

            //Posicionamento
            int X = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
            int Y = 30;
            this.Location = new Point(X, Y);

            #region Imagens dos botões

            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnTeclado.Text = string.Empty;
            btnTeclado.Image = Imagem.GetTeclado_32x32();
            btnGravar.Text = string.Empty;
            btnGravar.Image = Imagem.GetGravar_32x32();
            btnDispensar.Text = string.Empty;
            btnDispensar.Image = Imagem.GetDispensar_32x32();
            btnAdicionar.Text = string.Empty;
            btnAdicionar.Image = Imagem.GetAdicionar_32x32();

            #endregion

            #region ListView

            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.View = View.Details;
            listView.Font = new Font("Segoe UI Light", 18);
            listView.HeaderStyle = ColumnHeaderStyle.None;
            listView.MultiSelect = false;

            listView.Columns.Add("IdCorante", 0);
            listView.Columns.Add("Corante", (listView.Width * 30) / 100, HorizontalAlignment.Left);
            listView.Columns.Add("Quantidade", (listView.Width * 70) / 100, HorizontalAlignment.Left);

            #endregion

            #region Unidades de medida configuradas

            /* Unidades de medida configuradas */
            IList<Percolore.IOConnect.Core.UnidadeMedida> d = new List<Percolore.IOConnect.Core.UnidadeMedida>();
            if (!_parametros.HabilitarMililitro) d.Add(Percolore.IOConnect.Core.UnidadeMedida.Mililitro);
            if (!_parametros.HabilitarShot) d.Add(Percolore.IOConnect.Core.UnidadeMedida.Shot);
            if (!_parametros.HabilitarOnca) d.Add(Percolore.IOConnect.Core.UnidadeMedida.Onca);
            if (!_parametros.HabilitarGrama) d.Add(Percolore.IOConnect.Core.UnidadeMedida.Grama);

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

            /* Popula combo */
            cboUnidade.DisplayMember = "Display";
            cboUnidade.ValueMember = "Value";
            cboUnidade.DataSource = EnumHelper.ToComboItemList(resource, d);
            cboUnidade.SelectedIndex = -1;

            #endregion

            #region Colorantes habilitados

            cmbCorante.DataSource =
                Util.ObjectColorante.List().Where(c => c.Habilitado == true && c.Seguidor == -1).ToList(); ;
            cmbCorante.ValueMember = "Circuito";
            cmbCorante.DisplayMember = "Nome";
            cmbCorante.SelectedItem = null;

            #endregion        

            txtNomeFormula.Text = _formula.Nome;
            _formula.Itens = _formula.Itens.OrderBy(o => o.Colorante.Circuito).ToList();
            ListarItensFormula(_formula.Itens);

            pnlQuantidade.Enabled = false;
            lblUnidadesConvertidas.Text = string.Empty;

            //Globalização
            this.label_10.Text = Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0010;
            this.label_11.Text = Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0011;
            this.label_12.Text = Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0012;
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0009;
            /*
            //Portugues e Espanhol
            if (_parametros.IdIdioma == 1 || _parametros.IdIdioma == 2)
            {
                this.label_10.Text = Properties.IOConnect.FormulaPersonalizada_Label_0010;
                this.label_11.Text = Properties.IOConnect.FormulaPersonalizada_Label_0011;
                this.label_12.Text = Properties.IOConnect.FormulaPersonalizada_Label_0012;
                this.lblTitulo.Text = Properties.IOConnect.FormulaPersonalizada_Label_0009;
            }
            else if(_parametros.IdIdioma == 3)
            {
                this.label_10.Text = Properties.IOConnect_eng.FormulaPersonalizada_Label_0010;
                this.label_11.Text = Properties.IOConnect_eng.FormulaPersonalizada_Label_0011;
                this.label_12.Text = Properties.IOConnect_eng.FormulaPersonalizada_Label_0012;
                this.lblTitulo.Text = Properties.IOConnect_eng.FormulaPersonalizada_Label_0009;
            }
            */

            updateTeclado();
        }

        private void updateTeclado()
        {
            bool ischek = false;
            if(_parametros != null)
            {
                ischek = _parametros.HabilitarTecladoVirtual;
            }
            txtUnDecimal.isTecladoShow = ischek;
            txtUnOnca48.isTecladoShow = ischek;
            txtUnOncaY.isTecladoShow = ischek;
            txtNomeFormula.isTecladoShow = ischek;

            if (_parametros != null)
            {
                ischek = _parametros.HabilitarTouchScrenn;
            }

            txtUnDecimal.isTouchScrenn = ischek;
            txtUnOnca48.isTouchScrenn = ischek;
            txtUnOncaY.isTouchScrenn = ischek;
            txtNomeFormula.isTouchScrenn = ischek;

        }


        #region Eventos

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            Util.ObjectColorante colorante = (Util.ObjectColorante)cmbCorante.SelectedItem;

            #region Validação

            if (colorante == null)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Informacao_NenhumColorante);
                }

                cmbCorante.Focus();
                return;
            }


            bool existe = (_formula.Itens.FirstOrDefault(f => f.IdColorante == colorante.Circuito) != null);
            if (existe)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informacao_ColoranteExistente);
                }

                cmbCorante.Focus();
                return;
            }

            if (_mililitros == 0)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    m.ShowDialog(Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informacao_QuantidadeNaoInformada);

                return;
            }

            #endregion

            Util.ObjectFormulaItem item = new Util.ObjectFormulaItem()
            {
                Mililitros = _mililitros,
                IdColorante = colorante.Circuito,
                Colorante = colorante
            };

            _formula.Itens.Add(item);
            _formula.Itens = _formula.Itens.OrderBy(o => o.Colorante.Circuito).ToList();
            ListarItensFormula(_formula.Itens);

            item = null;
            _colorante = null;
            cmbCorante.SelectedItem = null;
            ResetaQuantidade();
            pnlQuantidade.Enabled = false;
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            #region Validação

            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
            {
                bool confirma =
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Confirmar_ExcluirDados,
                        Negocio.IdiomaResxExtensao.Global_Sim,
                        Negocio.IdiomaResxExtensao.Global_Nao);

                if (!confirma)
                {
                    return;
                }
            }

            #endregion

            int _idCorante =
                int.Parse(listView.SelectedItems[0].SubItems[0].Text);

            Util.ObjectFormulaItem item =
                _formula.Itens.FirstOrDefault(f => f.IdColorante == _idCorante);

            if (item != null)
            {
                _formula.Itens.Remove(item);
                _formula.Itens = _formula.Itens.OrderBy(o => o.Colorante.Circuito).ToList();
                ListarItensFormula(_formula.Itens);
            }
        }

        private void btnGravar_Click(object sender, System.EventArgs e)
        {
            _formula.Nome = txtNomeFormula.Text;

            #region Validação

            if (string.IsNullOrEmpty(_formula.Nome))
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informaco_InformarFormula);
                }

                txtNomeFormula.Focus();
                return;
            }

            if (_formula.Itens.Count == 0)
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informaco_InformarColorante);
                }

                return;
            }

            #endregion

            Util.ObjectFormula.Persist(_formula);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDispensar_Click(object sender, EventArgs e)
        {
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenserMover_P3 dispenserP3 = null;

            Log.Logar(
                TipoLog.Processo,
                _parametros.PathLogProcessoDispensa,
                Negocio.IdiomaResxExtensao.Global_DispensaExecutadaEdicaoFormula);

            try
            {
                if (_parametros.ControlarExecucaoPurga)
                {
                    #region [Controle de execução de purga]

                    if (Operar.TemPurgaPendente())
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(
                                Negocio.IdiomaResxExtensao.Global_PurgaPendente);
                        }

                        return;
                    }

                    #endregion
                }

                if (_formula == null || _formula.Itens == null || _formula.Itens.Count <= 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog("Usuário não foi permitido realizar a dosagem! Por favor informe pelo menos um item de Fórmula.");
                    }

                    return;
                }

                if (_parametros.ControlarNivel)
                {
                    #region [Se controle de nível habilitado, verifica nível de colorante suficiente]

                    List<Util.ObjectColorante> excedentes = null;
                    if (!Operar.TemColoranteSuficiente(_formula, out excedentes))
                    {
                        string texto = Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo;

                        bool primeiro = true;
                        foreach (Util.ObjectColorante colorante in excedentes)
                        {
                            texto += (primeiro ? "" : ", ") + colorante.Nome;
                            primeiro = false;
                        }

                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(texto);
                        }

                        return;
                    }

                    #endregion
                }

                #region [Gera valores de dispensa]

                ValoresVO[] valores = new ValoresVO[16];
                ValoresVO[] valores2 = new ValoresVO[16];
           
                ValoresVO[] valores_b = new ValoresVO[16];
                ValoresVO[] valores2_b = new ValoresVO[16];

                List<Negocio.ListColoranteSeguidor> lColorSeguidor_P1 = null;
                List<Negocio.ListColoranteSeguidor> lColorSeguidor_P2 = null;

                bool isDosarBase_P1 = false;
                bool isDosarBase_P2 = false;

                bool _exist_Seg_P1 = false;
                bool _exist_Seg_P2 = false;

                List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();

                _formula.Itens = _formula.Itens.OrderBy(o => o.Colorante.Circuito).ToList();

                List<Util.ObjectColorante> _colorantes = Util.ObjectColorante.List();
                List<Util.ObjectColorante> colorantesHabilitados =
                       _colorantes.Where(w => w.Habilitado == true && w.Seguidor == -1).ToList();
                //Formula formula = (Formula)cmbFormula.SelectedItem;

                foreach (Util.ObjectFormulaItem item in _formula.Itens)
                {
                    #region Verifica dados de dispensa

                    int POSICAO_CIRCUITO = item.Colorante.Circuito;
                    int dispositivo = 1;// colorantesHabilitados.Where(c => c.Correspondencia == POSICAO_CIRCUITO).Select(c => c.Dispositivo);

                    foreach (Util.ObjectColorante col in colorantesHabilitados)
                    {
                        if (col.Circuito == POSICAO_CIRCUITO)
                        {
                            dispositivo = col.Dispositivo;

                        }
                    }

                    List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == POSICAO_CIRCUITO).ToList();

                    if (dispositivo == 1)
                    {
                        if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                        {
                            _exist_Seg_P1 = true;
                        }

                    }
                    else if (dispositivo == 2)
                    {
                        if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                        {
                            _exist_Seg_P2 = true;
                        }

                    }

                    #endregion
                }

                foreach (Util.ObjectFormulaItem item in _formula.Itens)
                {
                    int dispositivo = 1;// colorantesHabilitados.Where(c => c.Correspondencia == POSICAO_CIRCUITO).Select(c => c.Dispositivo);
                    bool isBase = false;
                    foreach (Util.ObjectColorante col in colorantesHabilitados)
                    {
                        if (col.Circuito == item.Colorante.Circuito)
                        {
                            dispositivo = col.Dispositivo;
                            isBase = col.IsBase;
                        }
                    }
                    double VOLUME = item.Mililitros;
                    if (isBase && !_parametros.HabilitarDispensaSequencial)
                    {
                        VOLUME = item.Mililitros / 2;
                    }

                    //[Recupera calibragem do circuito
                    Util.ObjectCalibragem c = Util.ObjectCalibragem.Load(item.Colorante.Circuito);
                    List<ValoresVO> calibragem = c.Valores;

                    List<Util.ObjectColorante> ncol = new List<Util.ObjectColorante>();

                    if (isBase && !_parametros.HabilitarDispensaSequencial)
                    {
                        ncol = lCol.FindAll(o => o.Seguidor == item.Colorante.Circuito).ToList();
                    }
                    else
                    {
                        if(item.Colorante.IsBicoIndividual && VOLUME > item.Colorante.VolumeBicoIndividual)
                        {
                            ncol = lCol.FindAll(o => o.Seguidor == item.Colorante.Circuito).ToList();
                            if (ncol.Count > 0)
                            {
                                VOLUME = VOLUME / (ncol.Count + 1);
                            }
                        }
                        else {
                            if (!item.Colorante.IsBicoIndividual)
                            {
                                ncol = lCol.FindAll(o => o.Seguidor == item.Colorante.Circuito).ToList();
                                if (ncol.Count > 0)
                                {
                                    VOLUME = VOLUME / (ncol.Count + 1);
                                }
                            }
                        }                        
                    }
                    //[Define valores para dispensa]

                    ValoresVO valoresRetornados = Operar.Parser(VOLUME, calibragem, c.UltimoPulsoReverso);
                    if (dispositivo == 1)
                    {
                        if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                        {
                            int[] n_motor = new int[ncol.Count];

                            for (int i = 0; i < ncol.Count; i++)
                            {
                                n_motor[i] = (ncol[i].Circuito);
                            }
                            if (lColorSeguidor_P1 == null)
                            {
                                lColorSeguidor_P1 = new List<Negocio.ListColoranteSeguidor>();
                            }
                            Negocio.ListColoranteSeguidor m_col = new Negocio.ListColoranteSeguidor();
                            m_col.Circuito = item.Colorante.Circuito;
                            m_col.isbase = isBase;
                            m_col.lCircuitoSeguidores = n_motor.ToList();
                            m_col.Qtd_Circuito = m_col.lCircuitoSeguidores.Count + 1;

                            m_col.myValoresPrincipal = valoresRetornados;
                            for (int i = 0; i < n_motor.Length; i++)
                            {
                                Util.ObjectCalibragem c_col = Util.ObjectCalibragem.Load(n_motor[i]);
                                List<ValoresVO> calibragem_col = c_col.Valores;
                                ValoresVO valoresRetornados_col = Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                Negocio.ListValoresVOCircuitos lvO = new Negocio.ListValoresVOCircuitos();
                                lvO.circuito = n_motor[i];
                                lvO.myValores = valoresRetornados_col;
                                //m_col.myValores = valoresRetornados;
                                m_col.myValores.Add(lvO);

                            }

                            lColorSeguidor_P1.Add(m_col);
                            if (isBase)
                            {
                                isDosarBase_P1 = true;
                            }
                        }
                        else
                        {
                            if (_exist_Seg_P1 && !isBase)
                            {
                                valores[item.Colorante.Circuito - 1] = valoresRetornados;
                            }
                            else if (!_exist_Seg_P1)
                            {
                                valores[item.Colorante.Circuito - 1] = valoresRetornados;
                            }
                            //valores[item.Colorante.Circuito - 1] = valoresRetornados;
                            if (isBase && !_parametros.HabilitarDispensaSequencial)
                            {
                                valores_b[item.Colorante.Circuito - 1] = valoresRetornados;
                                isDosarBase_P1 = true;
                            }
                        }
                    }
                    else if (dispositivo == 2)
                    {
                        if (!_parametros.HabilitarDispensaSequencial && ncol != null && ncol.Count > 0)
                        {
                            int[] n_motor = new int[ncol.Count];
                            for (int i = 0; i < ncol.Count; i++)
                            {
                                n_motor[i] = (ncol[i].Circuito - 16);
                            }
                            if (lColorSeguidor_P2 == null)
                            {
                                lColorSeguidor_P2 = new List<Negocio.ListColoranteSeguidor>();
                            }
                            Negocio.ListColoranteSeguidor m_col = new Negocio.ListColoranteSeguidor();
                            m_col.Circuito = item.Colorante.Circuito - 16;
                            m_col.isbase = isBase;
                            m_col.lCircuitoSeguidores = n_motor.ToList();

                            m_col.myValoresPrincipal = valoresRetornados;
                            for (int i = 0; i < n_motor.Length; i++)
                            {
                                Util.ObjectCalibragem c_col = Util.ObjectCalibragem.Load(n_motor[i]+16);
                                List<ValoresVO> calibragem_col = c_col.Valores;
                                ValoresVO valoresRetornados_col = Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                Negocio.ListValoresVOCircuitos lvO = new Negocio.ListValoresVOCircuitos();
                                lvO.circuito = n_motor[i];
                                lvO.myValores = valoresRetornados_col;
                                //m_col.myValores = valoresRetornados;
                                m_col.myValores.Add(lvO);

                            }
                            m_col.Qtd_Circuito = m_col.lCircuitoSeguidores.Count + 1;
                            lColorSeguidor_P2.Add(m_col);
                            if(isBase)
                            {
                                isDosarBase_P2 = true;
                            }
                        }
                        else
                        {
                            if (_exist_Seg_P2 && !isBase)
                            {
                                valores2[item.Colorante.Circuito - 1] = valoresRetornados;
                            }
                            else if (!_exist_Seg_P2)
                            {
                                valores2[item.Colorante.Circuito - 1] = valoresRetornados;
                            }
                            //valores2[(item.Colorante.Circuito - 16) - 1] = valoresRetornados;
                            if (isBase && !_parametros.HabilitarDispensaSequencial)
                            {
                                valores2_b[(item.Colorante.Circuito - 16) - 1] = valoresRetornados;
                                isDosarBase_P2 = true;
                            }
                        }
                    }
                }

                #endregion

                //Define o modo de execução do dispenser
                switch ((Dispositivo)_parametros.IdDispositivo)
                {
                    case Dispositivo.Simulador:
                        {
                            dispenser = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo.Placa_1:
                        {
                            dispenser = new ModBusDispenser_P1();
                            break;
                        }
                    case Dispositivo.Placa_2:
                        {
                            dispenser = new ModBusDispenser_P2(_parametros.NomeDispositivo);
                            break;
                        }
                    case Dispositivo.Placa_3:
                        {
                            dispenserP3 = new ModBusDispenserMover_P3(_parametros.NomeDispositivo, _parametros.NomeDispositivo_PlacaMov);
                            break;
                        }
                    //case Dispositivo.Placa_4:
                    //    {
                    //        dispenser = new ModBusDispenser_P4(_parametros.NomeDispositivo);
                    //        break;
                    //    }
                }

                //Identificar segunda placa
                switch ((Dispositivo2)_parametros.IdDispositivo2)
                {
                    case Dispositivo2.Nenhum:
                        {
                            dispenser2 = null;
                            break;
                        }
                    case Dispositivo2.Simulador:
                        {
                            dispenser2 = new ModBusSimulador();
                            break;
                        }
                    case Dispositivo2.Placa_2:
                        {
                            dispenser2 = new ModBusDispenser_P2(_parametros.NomeDispositivo2);
                            break;
                        }
                    //case Dispositivo2.Placa_4:
                    //    {
                    //        dispenser2 = new ModBusDispenser_P4(_parametros.NomeDispositivo2);
                    //        break;
                    //    }
                    default:
                        {
                            dispenser2 = null;
                            break;
                        }
                }
                if (dispenserP3 == null)
                {
                    if (!Operar.Conectar(ref dispenser))
                        return;

                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                        return;

                    if (!Operar.TemRecipiente(dispenser))
                        return;
                }
                else
                {
                    if(!Operar.ConectarP3(ref dispenserP3))
                    {
                        return;
                    }
                }
                //Gera demanda
                Dictionary<int, double> demanda = new Dictionary<int, double>();
                foreach (Util.ObjectFormulaItem item in _formula.Itens)
                {
                    demanda.Add(item.Colorante.Circuito, item.Mililitros);
                }
                bool confirmaVolumeMinimoDat = true;
                if (!_parametros.DesabilitarVolumeMinimoDat)
                {
                    string str_l = "";
                    foreach (KeyValuePair<int, double> item in demanda)
                    {
                        if (item.Value < _parametros.VolumeMinimoDat)
                        {
                            str_l += item.Key + ";" + item.Value;
                        }
                    }
                    if (str_l.Length > 0)
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                        {
                            //Confirmar dispensar com valores inferiores ao mínimo permitido de 0,0385 ml?
                            //string msg = string.Format("Confirmar dispensar com valores inferiores ao mínimo permitido de {0} ml?", _parametros.VolumeMinimoDat.ToString());
                            string msg = string.Format(Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolume, _parametros.VolumeMinimoDat.ToString());
                            confirmaVolumeMinimoDat = m.ShowDialog(msg, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                        }
                        if (confirmaVolumeMinimoDat)
                        {
                            //string msg = string.Format("Dispensar com Valores Inferior ao Mínimo de Volume permitido: {0}", str_l);
                            string msg = string.Format(Negocio.IdiomaResxExtensao.Log_Cod_30 + Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolumeLog, str_l);
                            Log.Logar(
                                  TipoLog.Processo,
                                  _parametros.PathLogProcessoDispensa,
                                  msg);
                        }
                    }
                }

                if (confirmaVolumeMinimoDat)
                {
                    string desc = string.IsNullOrEmpty(txtNomeFormula.Text) ? string.Empty : txtNomeFormula.Text.Trim();

                    if (_parametros.HabilitarDispensaSequencial)
                    {
                        #region Parâmetros da dispensa sequencial]

                        //bool is24MotoresP1 = false;
                        //bool is24MotoresP2 = false;
                        //if ((Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador /*|| (Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4*/)
                        //{
                        //    is24MotoresP1 = true;
                        //}
                        //if ((Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador /*|| (Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4*/)
                        //{
                        //    is24MotoresP2 = true;
                        //}
                        DispensaSequencialVO paramDispSeq = new DispensaSequencialVO();

                      
                        paramDispSeq.Dispenser = new List<IDispenser>();
                        if (dispenserP3 == null)
                        {
                            paramDispSeq.Dispenser.Add(dispenser);
                            if (_parametros.IdDispositivo2 != 0)
                            {
                                paramDispSeq.Dispenser.Add(dispenser2);
                            }
                        }
                        else
                        {
                            paramDispSeq.modBusDispenser_P3 = dispenserP3;
                        }
                        paramDispSeq.Demanda = demanda;
                        paramDispSeq.DescricaoCor = desc;

                        foreach (KeyValuePair<int, double> item in demanda)
                        {
                            paramDispSeq.Colorantes.Add(Util.ObjectColorante.Load(item.Key));
                        }
                        int countP2 = 0;
                        foreach (Util.ObjectColorante colorante in paramDispSeq.Colorantes)
                        {
                            int index = colorante.Circuito - 1;
                            if (colorante.Dispositivo == 1)
                            {
                                paramDispSeq.PulsoHorario[index] = valores[index].PulsoHorario;
                                paramDispSeq.Velocidade[index] = valores[index].Velocidade;
                                paramDispSeq.Aceleracao[index] = valores[index].Aceleracao;
                                paramDispSeq.Delay[index] = valores[index].Delay;
                                paramDispSeq.PulsoReverso[index] = valores[index].PulsoReverso;
                            }
                            else
                            {
                                if (countP2 < 16)
                                {
                                    //if (is24MotoresP1)
                                    //{
                                    //    index = index - 24;
                                    //    paramDispSeq.PulsoHorario2[index] = valores2[index].PulsoHorario;
                                    //    paramDispSeq.Velocidade2[index] = valores2[index].Velocidade;
                                    //    paramDispSeq.Aceleracao2[index] = valores2[index].Aceleracao;
                                    //    paramDispSeq.Delay2[index] = valores2[index].Delay;
                                    //    paramDispSeq.PulsoReverso2[index] = valores2[index].PulsoReverso;
                                    //}
                                    //else
                                    {
                                        index = index - 16;
                                        paramDispSeq.PulsoHorario2[index] = valores2[index].PulsoHorario;
                                        paramDispSeq.Velocidade2[index] = valores2[index].Velocidade;
                                        paramDispSeq.Aceleracao2[index] = valores2[index].Aceleracao;
                                        paramDispSeq.Delay2[index] = valores2[index].Delay;
                                        paramDispSeq.PulsoReverso2[index] = valores2[index].PulsoReverso;
                                    }
                                }
                                
                                countP2++;
                            }
                        }

                        #endregion

                        DialogResult result = DialogResult.None;
                        using (Form f = new fDispensaSequencial(paramDispSeq, _parametros.HabilitarDispensaSequencialP1, _parametros.HabilitarDispensaSequencialP2,
                            paramDispSeq.Demanda))
                        {
                            result = f.ShowDialog();
                        }
                        if (result == DialogResult.OK)
                        {
                            string delathes = "";
                            
                            foreach (KeyValuePair<int, double> item in paramDispSeq.Demanda)
                            {
                                Util.ObjectFormulaItem itemF = _formula.Itens.Find(o => o.Colorante.Circuito == item.Key);
                                if (itemF != null)
                                {
                                    if (delathes == "")
                                    {
                                        delathes += item.Key.ToString() + "," + itemF.Colorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                    }
                                    else
                                    {
                                        delathes += "," + item.Key.ToString() + "," + itemF.Colorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                    }
                                }
                            }

                            gerarEventoFormulaPersonalizada(0, delathes);
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            gerarEventoFormulaPersonalizada(1);
                        }
                        else if (result == DialogResult.Abort)
                        {
                            gerarEventoFormulaPersonalizada(2);
                        }
                        else
                        {
                            gerarEventoFormulaPersonalizada(3);
                        }
                    }
                    else
                    {
                        List<IDispenser> ldisp = null;
                        if (dispenserP3 == null)
                        {
                            ldisp = new List<IDispenser>();
                            ldisp.Add(dispenser);
                            if (_parametros.IdDispositivo2 != 0)
                            {
                                ldisp.Add(dispenser2);
                            }
                        }

                        List<Util.ObjectColorante> colSimultanea = new List<Util.ObjectColorante>();
                        foreach (KeyValuePair<int, double> item in demanda)
                        {
                            colSimultanea.Add(Util.ObjectColorante.Load(item.Key));
                        }

                        List<ValoresVO[]> v_Val_DosP1_B = new List<ValoresVO[]>();
                        List<ValoresVO[]> v_Val_DosP2_B = new List<ValoresVO[]>();
                        if (lColorSeguidor_P1 != null)
                        {
                            List<ValoresVO[]> v_Val_Dos_B = new List<ValoresVO[]>();

                            lColorSeguidor_P1 = lColorSeguidor_P1.OrderBy(o => o.Qtd_Circuito).ToList();

                            if (isDosarBase_P1)
                            {
                                ValoresVO[] v_variavel_B = valores_b;
                                List<Negocio.ListColoranteSeguidor> col_SegBase = new List<Negocio.ListColoranteSeguidor>();
                                for (int _i_mSeg = 0; _i_mSeg < lColorSeguidor_P1.Count; _i_mSeg++)
                                {
                                    Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P1[_i_mSeg];
                                    if (_cSeguidor.isbase)
                                    {
                                        col_SegBase.Add(_cSeguidor);
                                        lColorSeguidor_P1.RemoveAt(_i_mSeg--);
                                    }
                                }

                                int qtd_Max = 0;
                                for (int _m_i = 0; _m_i < 16; _m_i++)
                                {
                                    if (v_variavel_B[_m_i] != null)
                                    {
                                        qtd_Max++;
                                    }
                                }
                                if (qtd_Max > 5)
                                {
                                    v_Val_Dos_B.Add(v_variavel_B);
                                    v_variavel_B = new ValoresVO[16];
                                    qtd_Max = 0;
                                }
                                for (int _i_segbase = 0; _i_segbase < col_SegBase.Count; _i_segbase++)
                                {
                                    Negocio.ListColoranteSeguidor _cSeguidor = col_SegBase[_i_segbase];
                                    if ((qtd_Max + _cSeguidor.Qtd_Circuito) > 5)
                                    {
                                        v_Val_Dos_B.Add(v_variavel_B);
                                        qtd_Max = 0;
                                        v_variavel_B = new ValoresVO[16];
                                    }
                                    v_variavel_B[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                    qtd_Max++;
                                    foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                    {
                                        Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                        if (lvo_local != null)
                                        {
                                            v_variavel_B[_c_list - 1] = lvo_local.myValores;
                                        }
                                        else
                                        {
                                            v_variavel_B[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                        }
                                        //v_variavel_B[_c_list - 1] = _cSeguidor.myValores;
                                        qtd_Max++;
                                    }
                                    if ((_i_segbase + 1) >= col_SegBase.Count)
                                    {
                                        v_Val_Dos_B.Add(v_variavel_B);
                                    }
                                }

                                for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                {
                                    v_Val_DosP1_B.Add(v_Val_Dos_B[_i]);
                                }
                            }

                            ValoresVO[] vSeg = new ValoresVO[16];
                            bool ainda_tem_ckt = true;
                            int qtd_nCirc = 0;
                            //foreach (Negocio.ListColoranteSeguidor _cSeguidor in lColorSeguidor_P1)
                            for (int _v_lCol = 0; _v_lCol < lColorSeguidor_P1.Count; _v_lCol++)
                            {
                                Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P1[_v_lCol];
                                if (qtd_nCirc + _cSeguidor.Qtd_Circuito > 5)
                                {
                                    v_Val_DosP1_B.Add(vSeg);
                                    vSeg = new ValoresVO[16];
                                    qtd_nCirc = 0;
                                }
                                qtd_nCirc = _cSeguidor.Qtd_Circuito;
                                vSeg[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                int _i_seg = _cSeguidor.Qtd_Circuito;
                                foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                {
                                    Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                    if (lvo_local != null)
                                    {
                                        vSeg[_c_list - 1] = lvo_local.myValores;
                                    }
                                    else
                                    {
                                        vSeg[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                    }
                                    //vSeg[_c_list - 1] = _cSeguidor.myValores;
                                }
                                bool aghou_5 = false;
                                if (ainda_tem_ckt)
                                {
                                    ainda_tem_ckt = false;
                                    for (int _t_seg = _i_seg; !aghou_5 && _t_seg < 5; _t_seg++)
                                    {
                                        for (int _v_VO = 0; !aghou_5 && _v_VO < valores.Length; _v_VO++)
                                        {
                                            if (valores[_v_VO] != null)
                                            {
                                                ainda_tem_ckt = true;
                                                //vSeg[_v_VO] = vValores[_v_VO];
                                                vSeg[_v_VO] = new ValoresVO();
                                                vSeg[_v_VO].Aceleracao = valores[_v_VO].Aceleracao;
                                                vSeg[_v_VO].Delay = valores[_v_VO].Delay;
                                                vSeg[_v_VO].DesvioMedio = valores[_v_VO].DesvioMedio;
                                                vSeg[_v_VO].MassaIdeal = valores[_v_VO].MassaIdeal;
                                                vSeg[_v_VO].MassaMedia = valores[_v_VO].MassaMedia;
                                                vSeg[_v_VO].PulsoHorario = valores[_v_VO].PulsoHorario;
                                                vSeg[_v_VO].PulsoReverso = valores[_v_VO].PulsoReverso;
                                                vSeg[_v_VO].Velocidade = valores[_v_VO].Velocidade;
                                                vSeg[_v_VO].Volume = valores[_v_VO].Volume;
                                                valores[_v_VO] = null;
                                                _i_seg++;
                                                qtd_nCirc++;
                                                if (_i_seg > 4)
                                                {
                                                    aghou_5 = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (qtd_nCirc >= 5)
                                {
                                    v_Val_DosP1_B.Add(vSeg);
                                    vSeg = new ValoresVO[16];
                                    qtd_nCirc = 0;
                                }
                                else if ((_v_lCol + 1) >= lColorSeguidor_P1.Count)
                                {
                                    v_Val_DosP1_B.Add(vSeg);
                                }
                            }
                            bool sobrou_algum_ckt_seg = false;
                            for (int _v_VO = 0; !sobrou_algum_ckt_seg && _v_VO < valores.Length; _v_VO++)
                            {
                                if (valores[_v_VO] != null)
                                {
                                    sobrou_algum_ckt_seg = true;
                                }
                            }
                            if (sobrou_algum_ckt_seg)
                            {
                                v_Val_DosP1_B.Add(valores);
                            }

                            if (isDosarBase_P1)
                            {
                                for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                {
                                    v_Val_DosP1_B.Add(v_Val_Dos_B[_i]);
                                }
                            }

                        }
                        else
                        {
                            if (isDosarBase_P1)
                            {
                                v_Val_DosP1_B.Add(valores_b);
                            }
                            v_Val_DosP1_B.Add(valores);
                        }
                        if (lColorSeguidor_P2 != null)
                        {
                            List<ValoresVO[]> v_Val_Dos_B = new List<ValoresVO[]>();
                            lColorSeguidor_P2 = lColorSeguidor_P2.OrderBy(o => o.Qtd_Circuito).ToList();
                            if (isDosarBase_P2)
                            {
                                ValoresVO[] v_variavel_B = valores2_b;
                                List<Negocio.ListColoranteSeguidor> col_SegBase_2 = new List<Negocio.ListColoranteSeguidor>();
                                for (int _i_mSeg = 0; _i_mSeg < lColorSeguidor_P2.Count; _i_mSeg++)
                                {
                                    Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P2[_i_mSeg];
                                    if (_cSeguidor.isbase)
                                    {
                                        col_SegBase_2.Add(_cSeguidor);
                                        lColorSeguidor_P2.RemoveAt(_i_mSeg--);
                                    }
                                }
                                int qtd_Max = 0;
                                for (int _m_i = 0; _m_i < 16; _m_i++)
                                {
                                    if (v_variavel_B[_m_i] != null)
                                    {
                                        qtd_Max++;
                                    }
                                }
                                if (qtd_Max > 5)
                                {
                                    v_Val_Dos_B.Add(v_variavel_B);
                                    v_variavel_B = new ValoresVO[16];
                                    qtd_Max = 0;
                                }
                                for (int _i_segbase = 0; _i_segbase < col_SegBase_2.Count; _i_segbase++)
                                {
                                    Negocio.ListColoranteSeguidor _cSeguidor = col_SegBase_2[_i_segbase];
                                    if ((qtd_Max + _cSeguidor.Qtd_Circuito) > 5)
                                    {
                                        v_Val_Dos_B.Add(v_variavel_B);
                                        qtd_Max = 0;
                                        v_variavel_B = new ValoresVO[16];
                                    }
                                    v_variavel_B[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                    qtd_Max++;
                                    foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                    {
                                        Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                        if (lvo_local != null)
                                        {
                                            v_variavel_B[_c_list- 1] = lvo_local.myValores;
                                        }
                                        else
                                        {
                                            v_variavel_B[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                        }

                                        //v_variavel_B[_c_list - 1] = _cSeguidor.myValores;
                                        qtd_Max++;
                                    }
                                    if ((_i_segbase + 1) >= col_SegBase_2.Count)
                                    {
                                        v_Val_Dos_B.Add(v_variavel_B);
                                    }
                                }
                                for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                {
                                    v_Val_DosP2_B.Add(v_Val_Dos_B[_i]);
                                }

                            }

                            ValoresVO[] vSeg = new ValoresVO[16];
                            bool ainda_tem_ckt = true;
                            int qtd_nCirc = 0;
                            //foreach (Negocio.ListColoranteSeguidor _cSeguidor in lColorSeguidor_P2)
                            for (int _v_lCol = 0; _v_lCol < lColorSeguidor_P2.Count; _v_lCol++)
                            {
                                //ValoresVO[] vSeg = new ValoresVO[16];
                                Negocio.ListColoranteSeguidor _cSeguidor = lColorSeguidor_P2[_v_lCol];
                                if (qtd_nCirc + _cSeguidor.Qtd_Circuito > 5)
                                {
                                    v_Val_DosP2_B.Add(vSeg);
                                    vSeg = new ValoresVO[16];
                                    qtd_nCirc = 0;
                                }
                                qtd_nCirc = _cSeguidor.Qtd_Circuito;
                                vSeg[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                int _i_seg = _cSeguidor.Qtd_Circuito;
                                foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                {
                                    Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                    if (lvo_local != null)
                                    {
                                        vSeg[_c_list - 1] = lvo_local.myValores;
                                    }
                                    else
                                    {
                                        vSeg[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                    }
                                    //vSeg[(_c_list - 16) - 1] = _cSeguidor.myValores;
                                }
                                bool aghou_5 = false;
                                if (ainda_tem_ckt)
                                {
                                    ainda_tem_ckt = false;
                                    for (int _t_seg = _i_seg; !aghou_5 && _t_seg < 5; _t_seg++)
                                    {
                                        for (int _v_VO = 0; !aghou_5 && _v_VO < valores2.Length; _v_VO++)
                                        {
                                            if (valores2[_v_VO] != null)
                                            {
                                                //vSeg[_v_VO] = vValores2[_v_VO];
                                                vSeg[_v_VO] = new ValoresVO();
                                                vSeg[_v_VO].Aceleracao = valores2[_v_VO].Aceleracao;
                                                vSeg[_v_VO].Delay = valores2[_v_VO].Delay;
                                                vSeg[_v_VO].DesvioMedio = valores2[_v_VO].DesvioMedio;
                                                vSeg[_v_VO].MassaIdeal = valores2[_v_VO].MassaIdeal;
                                                vSeg[_v_VO].MassaMedia = valores2[_v_VO].MassaMedia;
                                                vSeg[_v_VO].PulsoHorario = valores2[_v_VO].PulsoHorario;
                                                vSeg[_v_VO].PulsoReverso = valores2[_v_VO].PulsoReverso;
                                                vSeg[_v_VO].Velocidade = valores2[_v_VO].Velocidade;
                                                vSeg[_v_VO].Volume = valores2[_v_VO].Volume;
                                                valores2[_v_VO] = null;
                                                ainda_tem_ckt = true;
                                                _i_seg++;
                                                qtd_nCirc++;
                                                if (_i_seg > 4)
                                                {
                                                    aghou_5 = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                //v_Val_DosP2_B.Add(vSeg);
                                if (qtd_nCirc >= 5)
                                {
                                    v_Val_DosP2_B.Add(vSeg);
                                    vSeg = new ValoresVO[16];
                                    qtd_nCirc = 0;
                                }
                                else if ((_v_lCol + 1) >= lColorSeguidor_P2.Count)
                                {
                                    v_Val_DosP2_B.Add(vSeg);
                                }
                            }
                            bool sobrou_algum_ckt_seg = false;
                            for (int _v_VO = 0; !sobrou_algum_ckt_seg && _v_VO < valores2.Length; _v_VO++)
                            {
                                if (valores2[_v_VO] != null)
                                {
                                    sobrou_algum_ckt_seg = true;
                                }
                            }
                            if (sobrou_algum_ckt_seg)
                            {
                                v_Val_DosP2_B.Add(valores2);
                            }

                            if (isDosarBase_P2)
                            {
                                for (int _i = 0; _i < v_Val_Dos_B.Count; _i++)
                                {
                                    v_Val_DosP2_B.Add(v_Val_Dos_B[_i]);
                                }
                            }
                        }
                        else
                        {
                            if (isDosarBase_P2)
                            {
                                v_Val_DosP2_B.Add(valores2_b);
                            }
                            bool exist_prod = false;
                            for (int _i_p_l = 0; !exist_prod && _i_p_l < valores2.Length; _i_p_l++)
                            {
                                if (valores2[_i_p_l] != null)
                                {
                                    exist_prod = true;
                                }
                            }
                            if (exist_prod)
                            {
                                v_Val_DosP2_B.Add(valores2);
                            }
                        }
                        DialogResult result = DialogResult.None;
                        using (Form f =
                            new fDispensaSimultanea(ldisp, v_Val_DosP1_B, v_Val_DosP2_B, demanda, colSimultanea, desc, false, "", dispenserP3, isDosarBase_P1, isDosarBase_P2))
                        {
                            result = f.ShowDialog();
                        }

                        if (result == DialogResult.OK)
                        {
                            string delathes = "";
                            
                            foreach (KeyValuePair<int, double> item in demanda)
                            {
                                Util.ObjectFormulaItem itemF = _formula.Itens.Find(o => o.Colorante.Circuito == item.Key);
                                if (itemF != null)
                                {
                                    if (delathes == "")
                                    {
                                        delathes += item.Key.ToString() + "," + itemF.Colorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                    }
                                    else
                                    {
                                        delathes += "," + item.Key.ToString() + "," + itemF.Colorante.Nome + "," + Math.Round(item.Value, 3).ToString();
                                    }
                                }
                            }

                            gerarEventoFormulaPersonalizada(0, delathes);
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            gerarEventoFormulaPersonalizada(1);
                        }
                        else if (result == DialogResult.Abort)
                        {
                            gerarEventoFormulaPersonalizada(2);
                        }
                        else
                        {
                            gerarEventoFormulaPersonalizada(3);
                        }
                    }
                }
                else
                {
                    #region usuário cancelou
                    string mensagemUsuario = Negocio.IdiomaResxExtensao.Log_Cod_24 + Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat;
                    Log.Logar(TipoLog.ControleDispensa, _parametros.PathLogProcessoDispensa, mensagemUsuario);
                    #endregion
                }

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(
                        Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso + ex.Message);
                }
            }
            finally
            {
                try
                {
                    if (dispenser != null)
                    {
                        dispenser.Disconnect();
                    }
                    if (dispenser2 != null)
                    {
                        dispenser2.Disconnect();
                    }
                    if (dispenserP3 != null)
                    {
                        dispenserP3.Disconnect();
                        dispenserP3.Disconnect_Mover();
                    }
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				}
			}
        }

        int gerarEventoFormulaPersonalizada(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Formula Personalizada
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.FormulaPersonalizada;
                objEvt.DETALHES = result.ToString() + ";" + detalhes;
                objEvt.INTEGRADO = false;
                using (PercoloreRegistry percRegistry = new PercoloreRegistry())
                {
                    objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
                }
                retorno = Util.ObjectEventos.InsertEvento(objEvt);
                #endregion
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}

			return retorno;
        }

        private void Quantidade_KeyUp(object sender, KeyEventArgs e)
        {
            switch (_unidade)
            {
                #region Conversão para mililitros

                case Percolore.IOConnect.Core.UnidadeMedida.Grama:
                    {
                        _mililitros = Percolore.IOConnect.Core.UnidadeMedidaHelper.GramaToMililitro(
                            txtUnDecimal.ToDouble(), _colorante.MassaEspecifica);

                        break;
                    }
                case Percolore.IOConnect.Core.UnidadeMedida.Mililitro:
                    {
                        _mililitros = txtUnDecimal.ToDouble();
                        break;
                    }
                case Percolore.IOConnect.Core.UnidadeMedida.Onca:
                    {
                        _mililitros = Percolore.IOConnect.Core.UnidadeMedidaHelper.OncaFracionadaToMililitro(
                            txtUnOncaY.ToInt(), txtUnOnca48.ToDouble());
                        break;
                    }
                case Percolore.IOConnect.Core.UnidadeMedida.Shot:
                    {
                        _mililitros = Percolore.IOConnect.Core.UnidadeMedidaHelper.ShotToMililitro(
                            txtUnDecimal.ToDouble(), _parametros.ValorShot);

                        break;
                    }
                default:
                    break;

                    #endregion
            }

            lblUnidadesConvertidas.Text = string.Empty;
            if (_mililitros > 0)
            {
                #region Exibição de unidades convertidas

                List<string> unidades_convertidas = new List<string>();

                if (_parametros.HabilitarMililitro)
                {
                    #region Mililitros

                    if (_unidade != Percolore.IOConnect.Core.UnidadeMedida.Mililitro)
                    {
                        unidades_convertidas.Add(
                            Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatMililitro(_mililitros, true, Application.CurrentCulture));
                    }

                    #endregion
                }

                if (_parametros.HabilitarGrama)
                {
                    #region Grama

                    if (_unidade != Percolore.IOConnect.Core.UnidadeMedida.Grama)
                    {
                        double grama =
                            Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToGrama(_mililitros, _colorante.MassaEspecifica);
                        unidades_convertidas.Add(
                            Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatGrama(grama, true, Application.CurrentCulture));
                    }

                    #endregion
                }

                if (_parametros.HabilitarOnca)
                {
                    #region Onça

                    if (_unidade != Percolore.IOConnect.Core.UnidadeMedida.Onca)
                    {
                        int y;
                        double _48;
                        Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToOncaFracionada(_mililitros, out y, out _48);
                        string onca =
                            Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatarOnca(y, _48, Application.CurrentCulture);
                        unidades_convertidas.Add(onca);
                    }

                    #endregion
                }

                if (_parametros.HabilitarShot)
                {
                    #region Shot

                    if (_unidade != Percolore.IOConnect.Core.UnidadeMedida.Shot)
                    {
                        double shot =
                            Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToShot(_mililitros, _parametros.ValorShot);
                        unidades_convertidas.Add(
                            Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatShot(shot, true, Application.CurrentCulture));
                    }

                    #endregion
                }

                lblUnidadesConvertidas.Text = string.Join(", ", unidades_convertidas);

                #endregion
            }

        }

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormulaEditar_Paint(object sender, PaintEventArgs e)
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

        private void cmbCorante_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ResetaQuantidade();
            _colorante = (Util.ObjectColorante)cmbCorante.SelectedItem;
        }

        private void cboUnidade_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pnlUnDecimal.Visible = false;
            pnlUnOnca.Visible = false;
            txtUnDecimal.Text = string.Empty;
            txtUnOnca48.Text = string.Empty;
            txtUnOncaY.Text = string.Empty;
            lblUnidadesConvertidas.Text = string.Empty;
            _mililitros = 0;
            _unidade = (Percolore.IOConnect.Core.UnidadeMedida)((ComboBoxItem)(cboUnidade.SelectedItem)).Value;

            switch (_unidade)
            {
                case Percolore.IOConnect.Core.UnidadeMedida.Mililitro:
                    {
                        lblDecimal.Text =
                            Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Mililitro;
                        pnlUnDecimal.Visible = true;
                        break;
                    }
                case Percolore.IOConnect.Core.UnidadeMedida.Onca:
                    {
                        pnlUnOnca.Visible = true;
                        break;
                    }
                case Percolore.IOConnect.Core.UnidadeMedida.Shot:
                    {
                        lblDecimal.Text =
                            Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Shot;
                        pnlUnDecimal.Visible = true;
                        break;
                    }
                case Percolore.IOConnect.Core.UnidadeMedida.Grama:
                    {
                        lblDecimal.Text =
                            Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Grama;
                        pnlUnDecimal.Visible = true;
                        pnlUnDecimal.BringToFront();
                        break;
                    }
                default:
                    break;
            }
        }

        #endregion

        #region Métodos

        void ListarItensFormula(List<Util.ObjectFormulaItem> itens)
        {
            listView.Items.Clear();
            foreach (Util.ObjectFormulaItem item in itens)
            {
                List<string> unidades_configuradas = new List<string>();

                #region Converte unidades de medida em tempo de execução para exibir na listView (Refatorar)

                /* Considerações pra refatoração - 08/09/2016
                 * Foi feito desta forma pois na listView é inserido o objeto FormulaItem 
                 * e um item de fórmula e, sendo assim possui apenas a quantidade em mililitros, 
                 * sendo assim, a cada vez que a grid é preenchida é necessário efetuar conversão
                 * de mililitros para todas as unidades de medida configuradas, montar uma string
                 * e inserir diretamente na lista view junto ao objeto FormulaItem                                  
                 */

                if (_parametros.HabilitarMililitro)
                {
                    unidades_configuradas.Add(
                        Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatMililitro(item.Mililitros, true, Application.CurrentCulture));
                }

                if (_parametros.HabilitarGrama)
                {
                    double gramas = Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToGrama(
                        item.Mililitros, item.Colorante.MassaEspecifica);

                    unidades_configuradas.Add(
                        Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatGrama(gramas, true, Application.CurrentCulture));
                }

                if (_parametros.HabilitarShot)
                {
                    double shot = Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToShot(
                        item.Mililitros, _parametros.ValorShot);

                    unidades_configuradas.Add(
                        Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatShot(shot, true, Application.CurrentCulture));
                }

                if (_parametros.HabilitarOnca)
                {
                    int y;
                    double _48;
                    Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToOncaFracionada(item.Mililitros, out y, out _48);
                    unidades_configuradas.Add(
                        Percolore.IOConnect.Core.UnidadeMedidaHelper.FormatarOnca(y, _48, Application.CurrentCulture));
                }

                #endregion

                string[] linha =
                    new string[] {
                        item.IdColorante.ToString(), item.Colorante.Nome, string.Join(", ", unidades_configuradas) };

                listView.Items.Add(new ListViewItem(linha));
            }
        }

        void ResetaQuantidade()
        {
            _unidade = null;
            _mililitros = 0;
            cboUnidade.SelectedItem = null;
            lblUnidadesConvertidas.Text = string.Empty;
            txtUnOnca48.Text = string.Empty;
            txtUnOncaY.Text = string.Empty;
            txtUnDecimal.Text = string.Empty;
            pnlQuantidade.Enabled = true;
            cboUnidade.Focus();
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