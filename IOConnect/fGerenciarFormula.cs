using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Util;
using System.Reflection;

namespace Percolore.IOConnect
{
	public partial class fGerenciarFormula : Form
    {
        Util.ObjectParametros _parametros = null;

        public fGerenciarFormula()
        {
            InitializeComponent();

            _parametros = Util.ObjectParametros.Load();

            #region Imagens dos botões

            btnTeclado.Text = string.Empty;
            btnTeclado.Image = Imagem.GetTeclado_32x32();
            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnDispensar.Text = string.Empty;
            btnDispensar.Image = Imagem.GetDispensar_32x32();
            btnEditar.Text = string.Empty;
            btnNovaFormula.Text = string.Empty;
            btnNovaFormula.Image = Imagem.GetAdicionar_32x32();
            btnEditar.Image = Imagem.GetEditar_32x32();
            btnEditar.Text = string.Empty;
            btnExcluir.Text = string.Empty;
            btnExcluir.Image = Imagem.GetExcluir_32x32();

            #endregion

            //Globalização
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.label_7.Text = $"IOConnect {version.Major}.{version.Minor}.{version.Build} " + $" | {Negocio.IdiomaResxExtensao.GerenciarFormulaPersonalizada_Label_0007}";
            this.label_8.Text = Negocio.IdiomaResxExtensao.GerenciarFormulaPersonalizada_Label_0008;

        }

        #region Eventos

        private void CriarFormula_Load(object sender, EventArgs e)
        {
            #region Configura ListView

            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.View = View.Details;
            listView.Font = new Font("Segoe UI Light", 22);
            listView.HeaderStyle = ColumnHeaderStyle.None;

            listView.Columns.Add("IdCorante", 0);
            listView.Columns.Add("Corante", (listView.Width * 30) / 100, HorizontalAlignment.Left);
            listView.Columns.Add("Quantidade", (listView.Width * 70) / 100, HorizontalAlignment.Left);

            #endregion

            CarregarFormulas();
        }

        private void cmbFormula_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool selecionado = cmbFormula.SelectedItem != null;
            btnEditar.Enabled = selecionado;
            btnExcluir.Enabled = selecionado;
            btnDispensar.Enabled = selecionado;

            if (!selecionado)
            {
                listView.Items.Clear();
                return;
            }

            Util.ObjectFormula formula = (Util.ObjectFormula)cmbFormula.SelectedItem;

            //Limpa itens
            listView.Items.Clear();
            foreach (Util.ObjectFormulaItem item in formula.Itens)
            {
                string quantidade = string.Empty;

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

                string[] linha = new string[] { item.IdColorante.ToString(), item.Colorante.Nome, string.Join(", ", unidades_configuradas) };

                listView.Items.Add(new ListViewItem(linha));
            }

            formula = null;
        }

        private void btnNovaFormula_Click(object sender, EventArgs e)
        {
            using (fFormulaPersonalizada f = new fFormulaPersonalizada(null))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    CarregarFormulas();
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Util.ObjectFormula formula = Util.ObjectFormula.Load((int)cmbFormula.SelectedValue);

           
            using (fFormulaPersonalizada f = new fFormulaPersonalizada(formula))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    CarregarFormulas();
                }
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectFormula formula = Util.ObjectFormula.Load((int)cmbFormula.SelectedValue);
                Util.ObjectFormula.Delete(formula.Id);
                listView.Items.Clear();
                CarregarFormulas();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_ExcluirDados);
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDispensar_Click(object sender, EventArgs e)
        {
            IDispenser dispenser = null;
            IDispenser dispenser2 = null;
            ModBusDispenserMover_P3 dispenserP3 = null;
            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_DispensaExecutadaGerenciamentoFormula);

            //Fórmula selecionada
            Util.ObjectFormula formula = (Util.ObjectFormula)cmbFormula.SelectedItem;

            try
            {
                if (_parametros.ControlarExecucaoPurga)
                {
                    #region [Controle de execução de purga]

                    if (Operar.TemPurgaPendente())
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_PurgaPendente);
                        }
                        return;
                    }

                    #endregion
                }

                if (_parametros.ControlarNivel)
                {
                    #region [Se controle de volume habilitado, verifica volume de colorante suficiente]

                    List<Util.ObjectColorante> excedentes = null;
                    if (!Operar.TemColoranteSuficiente(formula, out excedentes))
                    {
                        string texto = Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo + " ";

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
                bool Dat_usado_tcp_12 = false;
                switch ((DatPattern)_parametros.PadraoConteudoDAT)
                {
                    #region Padrão de dat configurado

                    case DatPattern.Padrao12:
                        {
                            Dat_usado_tcp_12 = true;
                            break;
                        }
                        #endregion
                }

                #region [Gera valores de dispensa]

                //bool isMotor24P1 = false;
                //bool isMotor24P2 = false;

                ValoresVO[] valores = new ValoresVO[16];
                ValoresVO[] valores2 = new ValoresVO[16];
                if(/*(Dispositivo)_parametros.IdDispositivo == Dispositivo.Placa_4 ||*/ (Dispositivo)_parametros.IdDispositivo == Dispositivo.Simulador)
                {
                    valores = new ValoresVO[16];
                    valores2 = new ValoresVO[16];
                    //isMotor24P1 = false;
                }

                ValoresVO[] valores_b = new ValoresVO[16];
                ValoresVO[] valores2_b = new ValoresVO[16];
                if (/*(Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Placa_4 || */(Dispositivo2)_parametros.IdDispositivo2 == Dispositivo2.Simulador)
                {
                    valores_b = new ValoresVO[16];
                    valores2_b = new ValoresVO[16];
                    //isMotor24P2 = false;
                }


                List<Negocio.ListColoranteSeguidor> lColorSeguidor_P1 = null;
                List<Negocio.ListColoranteSeguidor> lColorSeguidor_P2 = null;
            

                bool isDosarBase_P1 = false;
                bool isDosarBase_P2 = false;

                List<Util.ObjectColorante> lCol = Util.ObjectColorante.List().FindAll(o => o.Seguidor > 0).ToList();

                formula.Itens = formula.Itens.OrderBy(o => o.Colorante.Circuito).ToList();

                List<Util.ObjectColorante> _colorantes = Util.ObjectColorante.List();
                List<Util.ObjectColorante> colorantesHabilitados =
                       _colorantes.Where(w => w.Habilitado == true && w.Seguidor == -1).ToList();

                bool _exist_Seg_P1 = false;
                bool _exist_Seg_P2 = false;


                foreach (Util.ObjectFormulaItem item in formula.Itens)
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
                foreach (Util.ObjectFormulaItem item in formula.Itens)
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
                    if(Dat_usado_tcp_12)
                    {
                        VOLUME = UnidadeMedidaHelper.MililitroToGrama(item.Mililitros, item.Colorante.MassaEspecifica);
                    }

                    if (isBase && !_parametros.HabilitarDispensaSequencial)
                    {
                        VOLUME = item.Mililitros / 2;
                    }

                    //[Recupera calibragem do circuito
                    Util.ObjectCalibragem c = Util.ObjectCalibragem.Load(item.Colorante.Circuito);
                    List<ValoresVO> calibragem = c.Valores;

                    //List<Util.ObjectColorante> ncol = lCol.FindAll(o => o.Seguidor == item.Colorante.Circuito).ToList();
                    List<Util.ObjectColorante> ncol = new List<Util.ObjectColorante>();

                    if (isBase && !_parametros.HabilitarDispensaSequencial)
                    {
                        ncol = lCol.FindAll(o => o.Seguidor == item.Colorante.Circuito).ToList();
                    }
                    else
                    {
                        if (item.Colorante.IsBicoIndividual && VOLUME > item.Colorante.VolumeBicoIndividual)
                        {
                            ncol = lCol.FindAll(o => o.Seguidor == item.Colorante.Circuito).ToList();
                            if (ncol.Count > 0)
                            {
                                VOLUME = VOLUME / (ncol.Count + 1);
                            }
                        }
                        else
                        {
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

                    ValoresVO valoresRetornados = new ValoresVO();
                    if (Dat_usado_tcp_12)
                    {
                        valoresRetornados.PulsoHorario = int.Parse(Math.Round(VOLUME * 1000).ToString());
                        valoresRetornados.Volume = VOLUME;
                        valoresRetornados.MassaIdeal = 0;
                        valoresRetornados.MassaMedia = 0;
                        valoresRetornados.Aceleracao = 0;
                        valoresRetornados.Delay = 0;
                        valoresRetornados.DesvioMedio = 0;
                        valoresRetornados.PulsoReverso = 0;
                        valoresRetornados.Velocidade = 0;
                    }
                    else
                    {
                        valoresRetornados = Operar.Parser(VOLUME, calibragem, c.UltimoPulsoReverso);
                    }

                    //= Operar.Parser(VOLUME, calibragem, c.UltimoPulsoReverso);
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
                                ValoresVO valoresRetornados_col = new ValoresVO();
                                if (Dat_usado_tcp_12)
                                {
                                    valoresRetornados_col.PulsoHorario = int.Parse(Math.Round(VOLUME * 1000).ToString());
                                    valoresRetornados_col.Volume = VOLUME;
                                    valoresRetornados_col.MassaIdeal = 0;
                                    valoresRetornados_col.MassaMedia = 0;
                                    valoresRetornados_col.Aceleracao = 0;
                                    valoresRetornados_col.Delay = 0;
                                    valoresRetornados_col.DesvioMedio = 0;
                                    valoresRetornados_col.PulsoReverso = 0;
                                    valoresRetornados_col.Velocidade = 0;
                                }
                                else
                                {
                                    valoresRetornados_col = Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                }
                                //= Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
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
                                //if (isMotor24P1)
                                //{
                                //    n_motor[i] = (ncol[i].Circuito - 24);
                                //}
                                //else
                                {
                                    n_motor[i] = (ncol[i].Circuito - 16);
                                }
                            }
                            if (lColorSeguidor_P2 == null)
                            {
                                lColorSeguidor_P2 = new List<Negocio.ListColoranteSeguidor>();
                            }
                            Negocio.ListColoranteSeguidor m_col = new Negocio.ListColoranteSeguidor();
                            //if (isMotor24P1)
                            //{
                            //    m_col.Circuito = item.Colorante.Circuito - 24;
                            //}
                            //else
                            {
                                m_col.Circuito = item.Colorante.Circuito - 16;
                            }
                            m_col.isbase = isBase;
                            m_col.lCircuitoSeguidores = n_motor.ToList();
                            m_col.Qtd_Circuito = m_col.lCircuitoSeguidores.Count + 1;
                            m_col.myValoresPrincipal = valoresRetornados;
                            for (int i = 0; i < n_motor.Length; i++)
                            {
                                Util.ObjectCalibragem c_col = Util.ObjectCalibragem.Load(n_motor[i]+16);
                                List<ValoresVO> calibragem_col = c_col.Valores;
                                ValoresVO valoresRetornados_col = new ValoresVO();
                                if (Dat_usado_tcp_12)
                                {
                                    valoresRetornados_col.PulsoHorario = int.Parse(Math.Round(VOLUME * 1000).ToString());
                                    valoresRetornados_col.Volume = VOLUME;
                                    valoresRetornados_col.MassaIdeal = 0;
                                    valoresRetornados_col.MassaMedia = 0;
                                    valoresRetornados_col.Aceleracao = 0;
                                    valoresRetornados_col.Delay = 0;
                                    valoresRetornados_col.DesvioMedio = 0;
                                    valoresRetornados_col.PulsoReverso = 0;
                                    valoresRetornados_col.Velocidade = 0;
                                }
                                else
                                {
                                    valoresRetornados_col = Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                }
                                //= Operar.Parser(VOLUME, calibragem_col, c_col.UltimoPulsoReverso);
                                Negocio.ListValoresVOCircuitos lvO = new Negocio.ListValoresVOCircuitos();
                                lvO.circuito = n_motor[i];
                                lvO.myValores = valoresRetornados_col;
                                //m_col.myValores = valoresRetornados;
                                m_col.myValores.Add(lvO);

                            }

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
                                //if (isMotor24P1)
                                //{
                                //    valores2_b[(item.Colorante.Circuito - 24) - 1] = valoresRetornados;
                                //}
                                //else
                                {
                                    valores2_b[(item.Colorante.Circuito - 16) - 1] = valoresRetornados;
                                }
                                    
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
                    {
                        return;
                    }

                    if (_parametros.IdDispositivo2 != 0 && !Operar.Conectar(ref dispenser2))
                    {
                        return;
                    }

                    if (!Operar.TemRecipiente(dispenser))
                    {
                        return;
                    }
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
                foreach (Util.ObjectFormulaItem item in formula.Itens)
                {
                    if (Dat_usado_tcp_12)
                    {
                        demanda.Add(item.Colorante.Circuito, UnidadeMedidaHelper.MililitroToGrama(item.Mililitros, item.Colorante.MassaEspecifica));
                    }
                    else
                    {
                        demanda.Add(item.Colorante.Circuito, item.Mililitros);
                    }
                }
                bool confirmaVolumeMinimoDat = true;
                if (!_parametros.DesabilitarVolumeMinimoDat)
                {
                    string str_l = "";
                    foreach (KeyValuePair<int, double> item in demanda)
                    {
                        if (Dat_usado_tcp_12)
                        {
                            Util.ObjectFormulaItem itenFormula = formula.Itens.Find(o => o.Colorante.Circuito == item.Key);
                            double valor = UnidadeMedidaHelper.GramaToMililitro(item.Value, itenFormula.Colorante.MassaEspecifica);
                            if (valor < _parametros.VolumeMinimoDat)
                            {
                                str_l += item.Key + ";" + item.Value;
                            }

                        }
                        else
                        {
                            if (item.Value < _parametros.VolumeMinimoDat)
                            {
                                str_l += item.Key + ";" + item.Value;
                            }
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
                            string msg = string.Format(Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolumeLog, str_l);
                            Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, msg);
                        }
                    }
                }

                if (confirmaVolumeMinimoDat)
                {

                    string desc = formula.Nome;
                    if (_parametros.HabilitarDispensaSequencial)
                    {
                        #region Parâmetros da dispensa sequencial

                        DispensaSequencialVO dispSeqVO = new DispensaSequencialVO();
                        dispSeqVO.Dispenser = new List<IDispenser>();
                        dispSeqVO.modBusDispenser_P3 = dispenserP3;
                        if (dispenserP3 == null)
                        {
                            dispSeqVO.Dispenser.Add(dispenser);
                            if (_parametros.IdDispositivo2 != 0)
                            {
                                dispSeqVO.Dispenser.Add(dispenser2);
                            }
                        }
                        dispSeqVO.Demanda = demanda;
                        dispSeqVO.DescricaoCor = desc;

                        foreach (KeyValuePair<int, double> item in demanda)
                        {
                            dispSeqVO.Colorantes.Add(Util.ObjectColorante.Load(item.Key));
                        }

                        foreach (Util.ObjectColorante colorante in dispSeqVO.Colorantes)
                        {
                            int index = colorante.Circuito - 1;

                            if (colorante.Dispositivo == 1)
                            {
                                dispSeqVO.PulsoHorario[index] = valores[index].PulsoHorario;
                                dispSeqVO.Velocidade[index] = valores[index].Velocidade;
                                dispSeqVO.Aceleracao[index] = valores[index].Aceleracao;
                                dispSeqVO.Delay[index] = valores[index].Delay;
                                dispSeqVO.PulsoReverso[index] = valores[index].PulsoReverso;
                            }
                            else if (colorante.Dispositivo == 2 && dispenserP3 == null)
                            {
                                //if (isMotor24P1)
                                //{
                                //    index = index - 24;
                                //}
                                //else
                                {
                                    index = index - 16;
                                }
                                    
                                dispSeqVO.PulsoHorario2[index] = valores2[index].PulsoHorario;
                                dispSeqVO.Velocidade2[index] = valores2[index].Velocidade;
                                dispSeqVO.Aceleracao2[index] = valores2[index].Aceleracao;
                                dispSeqVO.Delay2[index] = valores2[index].Delay;
                                dispSeqVO.PulsoReverso2[index] = valores2[index].PulsoReverso;
                            }
                        }

                        #endregion

                        DialogResult result = DialogResult.None;
                        using (Form f = new fDispensaSequencial(dispSeqVO, _parametros.HabilitarDispensaSequencialP1, _parametros.HabilitarDispensaSequencialP2, dispSeqVO.Demanda))
                        {
                            result = f.ShowDialog();
                        }
                        if (result == DialogResult.OK)
                        {
                            string delathes = "";
                            
                            foreach (KeyValuePair<int, double> item in dispSeqVO.Demanda)
                            {
                                Util.ObjectFormulaItem itemF = formula.Itens.Find(o => o.Colorante.Circuito == item.Key);
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
                        List<IDispenser> ldisp = new List<IDispenser>();
                        if (dispenserP3 == null)
                        {
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
                                int indexV2 = 16;
                                //if (isMotor24P1)
                                //{
                                //    indexV2 = 24;
                                //}
                                for (int _m_i = 0; _m_i < indexV2; _m_i++)
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
                                    //if(isMotor24P2)
                                    //{
                                    //    v_variavel_B = new ValoresVO[24];
                                    //}
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
                                        //v_variavel_B[_c_list - 1] = _cSeguidor.myValores;
                                        Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                        if (lvo_local != null)
                                        {
                                            v_variavel_B[_c_list - 1] = lvo_local.myValores;
                                        }
                                        else
                                        {
                                            v_variavel_B[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                        }
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
                                if(qtd_nCirc + _cSeguidor.Qtd_Circuito > 5)
                                {
                                    v_Val_DosP1_B.Add(vSeg);
                                    vSeg = new ValoresVO[16];
                                    //if(isMotor24P1)
                                    //{
                                    //    vSeg = new ValoresVO[24];
                                    //}
                                    qtd_nCirc = 0;
                                }
                                qtd_nCirc = _cSeguidor.Qtd_Circuito;
                                vSeg[_cSeguidor.Circuito - 1] = _cSeguidor.myValoresPrincipal;
                                int _i_seg = _cSeguidor.Qtd_Circuito;
                                foreach (int _c_list in _cSeguidor.lCircuitoSeguidores)
                                {
                                    //vSeg[_c_list - 1] = _cSeguidor.myValores;
                                    Negocio.ListValoresVOCircuitos lvo_local = _cSeguidor.myValores.Find(o => o.circuito == _c_list);
                                    if (lvo_local != null)
                                    {
                                        vSeg[_c_list - 1] = lvo_local.myValores;
                                    }
                                    else
                                    {
                                        vSeg[_c_list - 1] = _cSeguidor.myValoresPrincipal;
                                    }
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
                                    //if(isMotor24P1)
                                    //{
                                    //    vSeg = new ValoresVO[24];
                                    //}
                                    qtd_nCirc = 0;
                                }
                                else if((_v_lCol + 1) >= lColorSeguidor_P1.Count)
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
                                int indexVP2 =16;
                                //if(isMotor24P2)
                                //{
                                //    indexVP2 = 24;
                                //}
                                for (int _m_i = 0; _m_i < indexVP2; _m_i++)
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
                                    //if (isMotor24P2)
                                    //{
                                    //    v_variavel_B = new ValoresVO[24];
                                    //}
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
                                        //if (isMotor24P2)
                                        //{
                                        //    v_variavel_B = new ValoresVO[24];
                                        //}
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
                                    //if(isMotor24P2)
                                    //{
                                    //    vSeg = new ValoresVO[24];
                                    //}
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
                                    //if(isMotor24P2)
                                    //{
                                    //    vSeg = new ValoresVO[24];
                                    //}
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
                                Util.ObjectFormulaItem itemF = formula.Itens.Find(o => o.Colorante.Circuito == item.Key);
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
                    m.ShowDialog(Negocio.IdiomaResxExtensao.GerenciarFormulaPersonalizada_Falha_Dispensar + ex.Message);
                }
            }
            finally
            {
                if (dispenser != null)
                {
                    dispenser.Disconnect();
                }
                if (dispenser2 != null)
                {
                    dispenser2.Disconnect();
                }
                if(dispenserP3 != null)
                {
                    dispenserP3.Disconnect();
                    dispenserP3.Disconnect_Mover();
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

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }

        #endregion

        #region Métodos

        private void CarregarFormulas()
        {
            List<Util.ObjectFormula> formulas = null;

            try
            {
                formulas = Util.ObjectFormula.List();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados);
                }
            }

            if (formulas != null)
            {
                cmbFormula.DataSource = formulas;
                cmbFormula.ValueMember = "Id";
                cmbFormula.DisplayMember = "Nome";

                btnEditar.Enabled = false;
                btnExcluir.Enabled = false;
                btnDispensar.Enabled = false;
            }

            cmbFormula.SelectedIndex = -1;
        }

        #endregion
      
    }
}