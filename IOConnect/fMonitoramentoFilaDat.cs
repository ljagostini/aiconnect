using Newtonsoft.Json;
using Percolore.Core.Util;
using Percolore.IOConnect.Negocio;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Percolore.IOConnect
{
	public partial class fMonitoramentoFilaDat : Form
    {
        private Util.ObjectParametros _parametros = Util.ObjectParametros.Load();
        private const int CONST_GRID_ROW_HEIGHT = 50;
        public fMonitoramentoFilaDat()
        {
            InitializeComponent();
        }

        private void fMonitoramentoFilaDat_Load(object sender, EventArgs e)
        {
            btnCancelar.Text = Negocio.IdiomaResxExtensao.Global_Cancelar;
            lblTitulo.Text = Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_lblStatus;
            try
            {
                btn_Consulta_Dat.Image = Imagem.Get_Monit_Fila_Dat_Refresh();
                btn_Consulta_Dat.Text = string.Empty;
            }
            catch
            { }
            LoadGrid();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch
            { }
        }

        private void LoadGrid()
        {
            try
            {
                dataGrid.Rows.Clear();
                ((DataGridViewImageColumn)dataGrid.Columns["ColDispensar"]).Image = Properties.IOConnect.Dispensar_32x32;
                ((DataGridViewImageColumn)dataGrid.Columns["ColExcluir"]).Image = Properties.IOConnect.Excluir_32x32_Escuro;
                string pathFile = _parametros.PathMonitoramentoFilaDAT;
                if (pathFile.Contains(@"\"))
                {
                    string[] arrPath = pathFile.Split((char)92);
                    if (arrPath.Length > 0)
                    {

                        string file = arrPath[arrPath.Length - 1];
                        if (file.Contains("."))
                        {
                            string[] arrFile = file.Split('.');
                            int indexof = pathFile.IndexOf(file);
                            string path = pathFile.Substring(0, indexof);
                            string[] fileEntries = Directory.GetFiles(path, "*." + arrFile[arrFile.Length - 1]);
                            if (fileEntries != null && fileEntries.Length > 0)
                            {
                                foreach (string namePathFile in fileEntries)
                                {
                                    //Linha
                                    DataGridViewRow r = new DataGridViewRow();
                                    r.Height = CONST_GRID_ROW_HEIGHT;

                                    //Células
                                    DataGridViewTextBoxCell colpathFile = new DataGridViewTextBoxCell();
                                    DataGridViewTextBoxCell colTerminal = new DataGridViewTextBoxCell();
                                    DataGridViewTextBoxCell colCodFormula = new DataGridViewTextBoxCell();
                                    DataGridViewTextBoxCell colCorFormula = new DataGridViewTextBoxCell();
                                    DataGridViewTextBoxCell colDataGerada = new DataGridViewTextBoxCell();
                                    DataGridViewTextBoxCell colIdBaseDados = new DataGridViewTextBoxCell();
                                    DataGridViewImageCell ColDispensar = new DataGridViewImageCell();
                                    DataGridViewImageCell ColExcluir = new DataGridViewImageCell();

                                    //Arquivo
                                    dataGridViewTextBoxColumn1.HeaderText = Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_ColunaGrid_Path;
                                    colpathFile.Value = namePathFile;


                                    string[] arrayNameFile = namePathFile.Split((char)92);

                                    string colunasNameFile = arrayNameFile[arrayNameFile.Length - 1];

                                    colunasNameFile = colunasNameFile.Substring(0, colunasNameFile.Length - 4);

                                    
                                    string[] arraycolunasNameFile = colunasNameFile.Split('-');


                                    colTerminal.Value = arraycolunasNameFile[0];
                                   // Negocio.IdiomaResxExtensao.fGerenciarFormula_GridColumn_CodigoCor
                                    colCodFormula.Value = arraycolunasNameFile[1];

                                    CodForumula.HeaderText = Negocio.IdiomaResxExtensao.fGerenciarFormula_GridColumn_CodigoCor;

                                    colCorFormula.Value = arraycolunasNameFile[2];
                                    CorFormula.HeaderText  = Negocio.IdiomaResxExtensao.fGerenciarFormula_GridColumn_DescCor;

                                    string dataG = arraycolunasNameFile[3];
                                    string _data = dataG.Substring(0, 2) + "/" + dataG.Substring(2, 2) + "/" + dataG.Substring(4, 4) + " " + dataG.Substring(8, 2) + ":" + dataG.Substring(10, 2) + ":" + 
                                        dataG.Substring(12, 2);
                                    colDataGerada.Value = _data;

                                    string idBaseDados = "0";
                                    try
                                    {
                                        if(arraycolunasNameFile.Length > 4)
                                        {
                                            idBaseDados = arraycolunasNameFile[4];
                                        }
                                    }
                                    catch
                                    { }

                                    colIdBaseDados.Value = idBaseDados;

                                    r.Cells.Add(colpathFile);
                                    r.Cells.Add(colTerminal);
                                    r.Cells.Add(colCodFormula);
                                    r.Cells.Add(colCorFormula);
                                    r.Cells.Add(colDataGerada);
                                    r.Cells.Add(colIdBaseDados);
                                    r.Cells.Add(ColDispensar);
                                    r.Cells.Add(ColExcluir);
                                    dataGrid.Rows.Add(r);
                                }
                            }
                        }
                    }
                }
                if(dataGrid.Rows.Count == 0)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch
            {

            }
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            try
            {
               
                string nomePathDat = dataGrid.SelectedRows[0].Cells[0].Value.ToString();
                int idBaseDados = 0;
                int.TryParse(dataGrid.SelectedRows[0].Cells[5].Value.ToString(), out idBaseDados);
                

                /* Executa ação selecionada */
                if (e.ColumnIndex == dataGrid.Columns["ColExcluir"].Index)
                {
                    bool confirma = false;
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                        confirma = m.ShowDialog(Negocio.IdiomaResxExtensao.Global_Confirmar_ExcluirDados, Negocio.IdiomaResxExtensao.Global_Sim, Negocio.IdiomaResxExtensao.Global_Nao);
                    }
                    if (confirma)
                    {
                        #region Excluir
                        FileHelper.Rename(
                        nomePathDat,
                        _parametros.PathRepositorioDAT,
                        true,
                         Negocio.IdiomaResxExtensao.Global_Informacao_ArquivoDatDeletado,
                        ".err");
                        if (_parametros.TipoBaseDados == "1")
                        {
                            if (idBaseDados > 0)
                            {
                                DeleteBaseDados(idBaseDados);
                            }
                        }
                        LoadGrid();
                        #endregion
                    }
                }
                else if (e.ColumnIndex == dataGrid.Columns["ColDispensar"].Index)
                {
                    if (!Operar.TemPurgaPendente())
                    {
                        if (_parametros.TipoBaseDados == "1")
                        {
                            if (idBaseDados > 0)
                            {
                                DeleteBaseDados(idBaseDados);
                            }
                        }
                        #region Dispensar
                        File.Move(nomePathDat, _parametros.PathMonitoramentoDAT);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        #endregion
                    }
                    else
                    {
                        using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Erro))
                        {
                            m.ShowDialog(Negocio.IdiomaResxExtensao.Global_PurgaPendente);
                        }
                    }
                }
               
            }
            catch
            { }
        }

        private bool DeleteBaseDados(int id)
        {
            bool retorno = false;
            try
            { 
                string text = "";
                MyModel adloc = new MyModel();
                MyModel adlocReturn = new MyModel();
                adloc.Output = "0";
                adloc.User = "adm";
                adloc.Pass = "adm2019";
                

                adloc.listValuesEntrada.Add(id.ToString());

				string strParameter = JsonConvert.SerializeObject(adloc);
				string urlReq = _parametros.PathBasesDados + "DatIOConnect/DatIOConnect_DeleteId/";

				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					HttpContent content = new StringContent(strParameter, Encoding.UTF8, "application/json");
					HttpResponseMessage response = client.PostAsync(urlReq, content).Result;

					if (response.IsSuccessStatusCode)
					{
						text = response.Content.ReadAsStringAsync().Result;
					}
				}

                if (text != null && text.Length > 1)
                {
                    adlocReturn = JsonConvert.DeserializeObject<MyModel>(text);
                    if (adlocReturn.Output == "1;OK;")
                    {
                        var serializedResult = JsonConvert.SerializeObject(adlocReturn.ListObjetoSaida);
                        string _strRet = JsonConvert.DeserializeObject<List<string>>(serializedResult)[0];

                        if (_strRet == "1")
                        {
                            retorno = true;
                        }
                    }
                }
            }
            catch
            {
            }
            return retorno;
        }

        private void btn_Consulta_Dat_Click(object sender, EventArgs e)
        {
            try
            {
                LoadGrid();
            }
            catch
            { }
        }
    }
}
