using Fractions;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Percolore.IOConnect
{
	public partial class fGerenciarNivel : Form
    {
        Util.ObjectParametros _parametros = null;
        List<Util.ObjectColorante> colorantes = null;
        static Image imgZerar = Imagem.GetZerar_32x32_02();
        static Image imgAdicionar = Imagem.GetAdicionar_32x32_02();

        public fGerenciarNivel()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer,
                true);

            _parametros = Util.ObjectParametros.Load();

            #region Globalização
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            lblTitulo.Text = $"IOConnect {version.Major}.{version.Minor}.{version.Build} | " + Negocio.IdiomaResxExtensao.GerenciarNivel_lblTitulo;
            version = null;

            this.btnZerarTodos.Text = " " + Negocio.IdiomaResxExtensao.GerenciarNivel_btnZerarTodos;
            this.btnAbastecerTodos.Text = " " + Negocio.IdiomaResxExtensao.GerenciarNivel_btnAbastecerTodos;

            #endregion

            #region Imagens dos botões

            btnSair.Text = string.Empty;
            btnSair.Image = Imagem.GetSair_32x32();
            btnAbastecerTodos.Image = Imagem.GetAdicionar_32x32();
            btnZerarTodos.Image = Imagem.GetZerar_32x32();

            #endregion

            #region Configurar datagrid

            //[Cria colunas]               
            dg.ColumnCount = 3;

            dg.Columns[0].Name = "colCircuito";
            dg.Columns[0].Visible = false;

            dg.Columns[1].Name = "colNomeColorante";
            dg.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dg.Columns[2].Name = "colDescricaoCircuito";
            dg.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewProgressColumn progressColumn = new DataGridViewProgressColumn();
            dg.Columns.Add(progressColumn);
            dg.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewImageColumn adicionarColumn = new DataGridViewImageColumn();
            adicionarColumn.Image = imgAdicionar;
            dg.Columns.Add(adicionarColumn);
            dg.Columns[4].Name = "colAbastecer";
            dg.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DataGridViewImageColumn zerarColumn = new DataGridViewImageColumn();
            zerarColumn.Image = imgZerar;
            dg.Columns.Add(zerarColumn);
            dg.Columns[5].Name = "colZerar";
            dg.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            #endregion
        }

        #region Eventos

        private void GerenciarVolumeColorante_Load(object sender, EventArgs e)
        {
            /* É necessário indicar topMost aqui para que o form seja 
             * redesenhando em primeiro plano sobre qualquer processo em execução */
            TopMost = true;

#if DEBUG
            TopMost = false;
#endif

            try
            {
                ListarColorantes();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados + ex.Message);
                }
            }
        }

        private void btnSair_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //[Recupera circuito do colorante selecionado]
            int circuito = (int)dg[0, e.RowIndex].Value;

            //[Recupera colorante selecionado]
            Util.ObjectColorante colorante = this.colorantes.SingleOrDefault(c => c.Circuito == circuito);

            //[Verifica coluna selecionada]
            if (e.ColumnIndex == dg.Columns["colAbastecer"].Index)
            {
                #region [Edição de colorante]

                DialogResult result = DialogResult.None;
                using (fNivel f = new fNivel(colorante))
                {
                    result = f.ShowDialog();
                }

                if (result == DialogResult.OK)
                {                    
                    ListarColorantes();
                    string detalhes = "";
                    foreach (Util.ObjectColorante _col in this.colorantes)
                    {
                        if (detalhes == "")
                        {
                            detalhes += _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                        }
                        else
                        {
                            detalhes += "," + _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                        }
                    }
                    gerarEventoAbastecimento(0, detalhes);
                }

                #endregion
            }
            else if (e.ColumnIndex == dg.Columns["colZerar"].Index)
            {
                #region [Zerar colorante]

                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
                {
                    string pergunta = Negocio.IdiomaResxExtensao.GerenciarNivel_Confirmar_ZerarNivel;
                    if (!m.ShowDialog(pergunta, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao))
                    {
                        return;
                    }
                }

                //Zera volume do colorante
                colorante.Volume = 0;

                try
                {
                    //Persistir
                    Util.ObjectColorante.Persist(colorante);
                    ListarColorantes();

                    Log.Logar(
                        TipoLog.Processo,
                        _parametros.PathLogProcessoDispensa,
                        $"{Negocio.IdiomaResxExtensao.Global_CircuitoColoranteZerado} {colorante.Nome}.");

                    string detalhes = "";
                    foreach (Util.ObjectColorante _col in this.colorantes)
                    {
                        if (detalhes == "")
                        {
                            detalhes += _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                        }
                        else
                        {
                            detalhes += "," + _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                        }
                    }
                    gerarEventoAbastecimento(0, detalhes);
                }
				catch (Exception ex)
				{
					LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
				
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                    {
                        string mensagem =
                            Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message;
                        m.ShowDialog(mensagem);
                    }
                }

                #endregion
            }
        }

        private void btnAbastecerTodos_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.None;
            using (fNivel f = new fNivel(this.colorantes))
            {
                result = f.ShowDialog();
            }

            if (result == DialogResult.OK)
            {               
                ListarColorantes();
                string detalhes = "";
                foreach(Util.ObjectColorante _col in this.colorantes)
                {
                    if (detalhes == "")
                    {
                        detalhes += _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                    }
                    else
                    {
                        detalhes += "," + _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                    }
                }

                gerarEventoAbastecimento(0, detalhes);
            }
        }
               
        private void btnZerarTodos_Click(object sender, EventArgs e)
        {
            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Confirmacao))
            {
                string pergunta = Negocio.IdiomaResxExtensao.GerenciarNivel_Confirmar_ZerarNivel;
                if (!m.ShowDialog(pergunta, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao))
                {
                    return;
                }
            }

            //Zera nível de todos os colorantes
            this.colorantes.ForEach(c => c.Volume = 0);

            try
            {
                //Persistir
                Util.ObjectColorante.Persist(this.colorantes);
                ListarColorantes();
                Log.Logar(TipoLog.Processo, _parametros.PathLogProcessoDispensa, Negocio.IdiomaResxExtensao.Global_TodosCircuitosColoranteZerados);
                string detalhes = "";
                foreach (Util.ObjectColorante _col in this.colorantes)
                {
                    if (detalhes == "")
                    {
                        detalhes += _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                    }
                    else
                    {
                        detalhes += "," + _col.Circuito.ToString() + "," + _col.Nome + "," + Math.Round(_col.Volume, 3).ToString();
                    }
                }
                gerarEventoAbastecimento(0, detalhes);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                {
                    string mensagem = Negocio.IdiomaResxExtensao.Global_Falha_GravarDados + ex.Message;
                    m.ShowDialog(mensagem);
                }
            }

        }

        #endregion

        #region Métodos

        private void ListarColorantes()
        {
            try
            {
                //Colorantes habilitados
                this.colorantes = Util.ObjectColorante.List().Where(c => c.Habilitado == true && c.Seguidor == -1).ToList();

                if ((this.colorantes == null) || this.colorantes.Count == 0)
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        m.ShowDialog(Negocio.IdiomaResxExtensao.GerenciarNivel_Informacao_ColorantesDesabilitados);
                    }

                    return;
                }

                dg.Rows.Clear();

                int idUnMedida = _parametros.UnidadeMedidaNivelColorante;
                //double maximoConfigurado = _parametros.VolumeMaximo;
                double maximoConfigurado = 0;

                string unAbreviacao;
                if (idUnMedida == (int)Percolore.IOConnect.Core.UnidadeMedida.Grama)
                {
                    unAbreviacao = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Grama;
                }
                else if (idUnMedida == (int)Percolore.IOConnect.Core.UnidadeMedida.Onca)
                {
                    unAbreviacao = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Onca;
                }
                else if (idUnMedida == (int)Percolore.IOConnect.Core.UnidadeMedida.Fraction)
                {
                    unAbreviacao = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Fraction;
                }
                else
                { 
                    unAbreviacao = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Mililitro; 
                }

                foreach (Util.ObjectColorante c in this.colorantes)
                {
                    maximoConfigurado = c.NivelMaximo;
                    double maximo = maximoConfigurado;
                    double atual = c.Volume;

                    if (idUnMedida == (int)Percolore.IOConnect.Core.UnidadeMedida.Grama)
                    {
                        maximo = maximoConfigurado * c.MassaEspecifica;
                        atual = atual * c.MassaEspecifica;
                    }
                    else if (idUnMedida == (int)Percolore.IOConnect.Core.UnidadeMedida.Onca)
                    {                        
                        maximo = Math.Round((Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToOnca(c.NivelMaximo) / 48), 2);
                        atual = Math.Round((Percolore.IOConnect.Core.UnidadeMedidaHelper.MililitroToOnca(atual) / 48), 2);
                    }

                    Informacoes inf = new Informacoes(maximo, atual, unAbreviacao, idUnMedida);

                    object[] row = new object[]{
                        c.Circuito,
                        $"{Negocio.IdiomaResxExtensao.Global_Colorante}: {c.Nome}",
                        $"{Negocio.IdiomaResxExtensao.Global_Circuito}: {c.Circuito.ToString()}",
                        inf 
                    };

                    dg.Rows.Add(row);
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			    throw;
            }
        }

        int gerarEventoAbastecimento(int result, string detalhes = "")
        {
            int retorno = 0;
            try
            {
                #region gravar Evento Inicializar Circuitos
                Util.ObjectEventos objEvt = new Util.ObjectEventos();
                objEvt.DATAHORA = DateTime.Now;
                objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.Abastecimento;
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


        #endregion
    }

    internal struct Informacoes
    {
        public double NivelMaximo { get; set; }
        public double NivelAtual { get; set; }
        public string UnidadeAbreviada { get; set; }

        public int unMedida { get; set; }

        public Informacoes(double maximo, double atual, string unidade, int unMed)
        {
            NivelMaximo = maximo;
            NivelAtual = atual;
            UnidadeAbreviada = unidade;
            unMedida = unMed;
        }
    }

    #region ProgressBarColumn

    internal class DataGridViewProgressColumn : DataGridViewImageColumn
    {
        public DataGridViewProgressColumn()
        {
            CellTemplate = new DataGridViewProgressCell();
        }
    }

    class DataGridViewProgressCell : DataGridViewImageCell
    {
        //Used to make custom cell consistent with a DataGridViewImageCell
        static Image emptyImage;

        static DataGridViewProgressCell()
        {
            emptyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public DataGridViewProgressCell()
        {
            this.ValueType = typeof(int);
        }

        // Method required to make the Progress Cell consistent with the default Image Cell. 
        // The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
        protected override object GetFormattedValue(object value,
                            int rowIndex, ref DataGridViewCellStyle cellStyle,
                            TypeConverter valueTypeConverter,
                            TypeConverter formattedValueTypeConverter,
                            DataGridViewDataErrorContexts context)
        {
            return emptyImage;
        }

        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue,
            string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            Informacoes Inf = (Informacoes)value;

            Brush BARRA_BRUSH_BACKCOLOR = new SolidBrush(Color.Gainsboro);
            Brush TEXTO_BRUSH_FORECOLOR = new SolidBrush(Color.White);
            Brush PROGRESSO_BRUSH_BACKCOLOR = new SolidBrush(Color.FromArgb(228, 81, 76));

            //Barra
            int BARRA_MARGEM_FONT = 10; //[Espaço entre limites da fonte e limites da barra]
            int BARRA_WIDTH = cellBounds.Width - cellStyle.Padding.Horizontal;
            int BARRA_HEIGHT = cellStyle.Font.Height + cellStyle.Padding.Vertical + BARRA_MARGEM_FONT;
            int BARRA_X = cellBounds.X + cellStyle.Padding.Left;
            int BARRA_Y = cellBounds.Y + ((cellBounds.Height - BARRA_HEIGHT) / 2);

            //Progresso
            double PROGRESSO_PERCENT = Math.Round(Inf.NivelAtual) / Math.Round(Inf.NivelMaximo);
            int PROGRESSO_WIDTH = (int)(PROGRESSO_PERCENT * BARRA_WIDTH);

            //Texto da barra            
            string TEXTO = $"{Inf.NivelAtual.ToString($"0.### {Inf.UnidadeAbreviada}")} de {Inf.NivelMaximo.ToString($"0.### {Inf.UnidadeAbreviada}")}";
            if(Inf.unMedida == (int)Percolore.IOConnect.Core.UnidadeMedida.Fraction)
            {
                Util.ObjectParametros parametros = Util.ObjectParametros.Load();
                var valueVolMax = new Fraction((int)Inf.NivelMaximo, parametros.ValorFraction);                
                var valueVol = new Fraction((int)Inf.NivelAtual, parametros.ValorFraction);

                //TEXTO = $"{Inf.NivelAtual.ToString($"0.### {Inf.UnidadeAbreviada}")} de {Inf.NivelMaximo.ToString($"0.### {Inf.UnidadeAbreviada}")}";
                TEXTO = valueVol.ToString("m", new CultureInfo("de-DE")) + " " + Inf.UnidadeAbreviada + " de " + 
                    valueVolMax.ToString("m", new CultureInfo("de-DE")) + " " + Inf.UnidadeAbreviada;
            }

            try
            {
                //Desenha célula da grid
                base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, Inf.NivelAtual, formattedValue, errorText,
                    cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

                //Desenha barra
                g.FillRectangle(BARRA_BRUSH_BACKCOLOR, BARRA_X, BARRA_Y, BARRA_WIDTH, BARRA_HEIGHT);

                //Desenha progresso
                g.FillRectangle(PROGRESSO_BRUSH_BACKCOLOR, BARRA_X, BARRA_Y, PROGRESSO_WIDTH, BARRA_HEIGHT);

                //Desenha texto
                g.DrawString(
                      TEXTO,
                      cellStyle.Font,
                      TEXTO_BRUSH_FORECOLOR,
                      BARRA_X + (BARRA_WIDTH / 2) - (g.MeasureString(TEXTO, cellStyle.Font).Width / 2),
                      BARRA_Y + (BARRA_MARGEM_FONT / 2) - 2); /* (11/09/2016) O "-2" foi adicionado ao cálculo com objetivo de centralizar verticalmente a fonte dentro da área da barra de progresso */

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
    }

    #endregion
}