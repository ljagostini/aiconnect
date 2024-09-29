using Percolore.Core.Logging;
using Percolore.IOConnect.Util;
using System.Data;

namespace Percolore.IOConnect
{
	public partial class fDat05 : Form
    {
        private int index_selGridBasDat06;
        private List<Util.ObjectBasDat05> listBasDat05 = new List<Util.ObjectBasDat05>();
        public Util.ObjectParametros _parametros = null;
        public fDat05()
        {
            InitializeComponent();
        }

        private void fDat05_Load(object sender, EventArgs e)
        {
            try
            {
                lbl_UNT_Dat_06_Prefixo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Prefixo;
                lbl_CAN_Dat_06_Prefixo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Prefixo;
                lbl_FRM_Dat_06_Prefixo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Prefixo;
                lbl_BAS_Dat_06_Prefixo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Prefixo;

                lbl_UNT_Dat_06_1Campo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_1Campo;
                lbl_CAN_Dat_06_1Campo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_1Campo;
                lbl_FRM_Dat_06_1Campo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_1Campo;
                lbl_BAS_Dat_06_1Campo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_1Campo;
                lbl_UNT_Dat_06_2Campo.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_2Campo;
                lbl_FRM_Dat_06_Separador.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Separador;
                chk_BAS_Dat_05_Habilitado.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Habilitado;

                _parametros = Util.ObjectParametros.Load();
                txt_BAS_DAT_05_Prefixo.Text = _parametros.Dat_05_BAS_Pref;
                txt_CAN_DAT_05_Prefixo.Text = _parametros.Dat_05_CAN_Pref;
                txt_FRM_DAT_05_Prefixo.Text = _parametros.Dat_05_FRM_Pref;
                txt_UNT_DAT_05_Prefixo.Text = _parametros.Dat_05_UNT_Pref;

                txt_FRM_DAT_05_Separador.Text = _parametros.Dat_05_FRM_SEP;

                DataTable dt_UNT_1_IsPonto = new DataTable();
                dt_UNT_1_IsPonto.Columns.Add("Value");
                dt_UNT_1_IsPonto.Columns.Add("Display");
                for (int i = 0; i <= 1; i++)
                {
                    DataRow dr = dt_UNT_1_IsPonto.NewRow();
                    dr["Value"] = i.ToString();
                    if (i == 0)
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula/*"Virgula"*/;
                    }
                    else
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto/*"Ponto"*/;
                    }
                    dt_UNT_1_IsPonto.Rows.Add(dr);
                }
                cmb_UNT_DAT_05_1_Ponto.DataSource = dt_UNT_1_IsPonto.DefaultView;
                cmb_UNT_DAT_05_1_Ponto.DisplayMember = "Display";
                cmb_UNT_DAT_05_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_05_UNT_1_IsPonto == 0)
                {
                    cmb_UNT_DAT_05_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_UNT_DAT_05_1_Ponto.SelectedIndex = 1;
                }

                DataTable dt_UNT_2_IsPonto = new DataTable();
                dt_UNT_2_IsPonto.Columns.Add("Value");
                dt_UNT_2_IsPonto.Columns.Add("Display");
                for (int i = 0; i <= 1; i++)
                {
                    DataRow dr = dt_UNT_2_IsPonto.NewRow();
                    dr["Value"] = i.ToString();
                    if (i == 0)
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula/*"Virgula"*/;
                    }
                    else
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto/*"Ponto"*/;
                    }
                    dt_UNT_2_IsPonto.Rows.Add(dr);
                }
                cmb_UNT_DAT_05_2_Ponto.DataSource = dt_UNT_2_IsPonto.DefaultView;
                cmb_UNT_DAT_05_2_Ponto.DisplayMember = "Display";
                cmb_UNT_DAT_05_2_Ponto.ValueMember = "Value";

                if (_parametros.Dat_05_UNT_2_IsPonto == 0)
                {
                    cmb_UNT_DAT_05_2_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_UNT_DAT_05_2_Ponto.SelectedIndex = 1;
                }



                DataTable dt_CAN_1_IsPonto = new DataTable();
                dt_CAN_1_IsPonto.Columns.Add("Value");
                dt_CAN_1_IsPonto.Columns.Add("Display");
                for (int i = 0; i <= 1; i++)
                {
                    DataRow dr = dt_CAN_1_IsPonto.NewRow();
                    dr["Value"] = i.ToString();
                    if (i == 0)
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula/*"Virgula"*/;
                    }
                    else
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto/*"Ponto"*/;
                    }
                    dt_CAN_1_IsPonto.Rows.Add(dr);
                }
                cmb_CAN_DAT_05_1_Ponto.DataSource = dt_CAN_1_IsPonto.DefaultView;
                cmb_CAN_DAT_05_1_Ponto.DisplayMember = "Display";
                cmb_CAN_DAT_05_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_05_CAN_1_IsPonto == 0)
                {
                    cmb_CAN_DAT_05_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_CAN_DAT_05_1_Ponto.SelectedIndex = 1;
                }



                DataTable dt_FRM_1_IsPonto = new DataTable();
                dt_FRM_1_IsPonto.Columns.Add("Value");
                dt_FRM_1_IsPonto.Columns.Add("Display");
                for (int i = 0; i <= 1; i++)
                {
                    DataRow dr = dt_FRM_1_IsPonto.NewRow();
                    dr["Value"] = i.ToString();
                    if (i == 0)
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula/*"Virgula"*/;
                    }
                    else
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto/*"Ponto"*/;
                    }
                    dt_FRM_1_IsPonto.Rows.Add(dr);
                }
                cmb_FRM_DAT_05_1_Ponto.DataSource = dt_FRM_1_IsPonto.DefaultView;
                cmb_FRM_DAT_05_1_Ponto.DisplayMember = "Display";
                cmb_FRM_DAT_05_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_05_FRM_1_IsPonto == 0)
                {
                    cmb_FRM_DAT_05_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_FRM_DAT_05_1_Ponto.SelectedIndex = 1;
                }





                DataTable dt_BAS_1_IsPonto = new DataTable();
                dt_BAS_1_IsPonto.Columns.Add("Value");
                dt_BAS_1_IsPonto.Columns.Add("Display");
                for (int i = 0; i <= 1; i++)
                {
                    DataRow dr = dt_BAS_1_IsPonto.NewRow();
                    dr["Value"] = i.ToString();
                    if (i == 0)
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula/*"Virgula"*/;
                    }
                    else
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto/*"Ponto"*/;
                    }
                    dt_BAS_1_IsPonto.Rows.Add(dr);
                }
                cmb_BAS_DAT_05_1_Ponto.DataSource = dt_BAS_1_IsPonto.DefaultView;
                cmb_BAS_DAT_05_1_Ponto.DisplayMember = "Display";
                cmb_BAS_DAT_05_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_05_BAS_1_IsPonto == 0)
                {
                    cmb_BAS_DAT_05_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_BAS_DAT_05_1_Ponto.SelectedIndex = 1;
                }

                chk_BAS_Dat_05_Habilitado.Checked = _parametros.Dat_05_BAS_Habilitado == 1 ? true : false;

                AtualizaGridDat05();

                btnSair.Text = string.Empty;
                btnSair.Image = Imagem.GetSair_32x32();
                btnConfirmar.Text = string.Empty;
                btnConfirmar.Image = Imagem.GetGravar_32x32();
                btnAddBasDat5.Text = string.Empty;
                btnAddBasDat5.Image = Imagem.GetAdicionar_32x32();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void AtualizaGridDat05()
        {
            try
            {
                this.listBasDat05 = Util.ObjectBasDat05.List();

                DataTable dt = new DataTable();
                dt.Columns.Add(Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos);
                dt.Columns.Add(Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume);
                dt.Columns.Add(Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito);
                foreach (Util.ObjectBasDat05 to in this.listBasDat05)
                {
                    DataRow dr = dt.NewRow();
                    dr[Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos] = to.Name;
                    dr[Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume] = to.Volume.ToString().Replace(",", ".");
                    dr[Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito] = to.Circuito.ToString();
                    dt.Rows.Add(dr);

                }
                dgBasDat05.DataSource = dt.DefaultView;

                this.index_selGridBasDat06 = -1;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnAddBasDat5_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectBasDat05 obj = new Util.ObjectBasDat05();
                obj.Circuito = 0;
                obj.Name = "";
                obj.Volume = 0.0;
                fBasDat05 fbd05 = new fBasDat05(obj, true);
                DialogResult dr = fbd05.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string msg = "";
                    ObjectBasDat05 objF = fbd05.objRetorno;
                    List<ObjectBasDat05> lObj = new List<ObjectBasDat05>();
                    lObj.Add(objF);
                    if (fbd05.myAction == 1)
                    {
                        if (objF == null || !Util.ObjectBasDat05.Validate(lObj, out msg))
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos + Environment.NewLine + Environment.NewLine+ msg);
                            }
                        }
                        else
                        {
                            Util.ObjectBasDat05.Persist(objF);
                            AtualizaGridDat05();
                        }

                    }

                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void dgBasDat05_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.index_selGridBasDat06 = e.RowIndex;
                Util.ObjectBasDat05 obj = this.listBasDat05[this.index_selGridBasDat06];
                fBasDat05 fbd05 = new fBasDat05(obj, false);
                DialogResult dr = fbd05.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string msg = "";
                    ObjectBasDat05 objF = fbd05.objRetorno;
                    List<ObjectBasDat05> lObj = new List<ObjectBasDat05>();
                    lObj.Add(objF);
                    if (fbd05.myAction == 1)
                    {
                        if (objF == null || !Util.ObjectBasDat05.Validate(lObj, out msg))
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos + Environment.NewLine + Environment.NewLine + msg);
                            }
                        }
                        else
                        {
                            Util.ObjectBasDat05.Persist(objF);
                            AtualizaGridDat05();
                        }

                    }
                    else
                    {
                        if (objF == null || !Util.ObjectBasDat05.Validate(lObj, out msg))
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos + Environment.NewLine + Environment.NewLine + msg);
                            }
                        }
                        else
                        {
                            Util.ObjectBasDat05.Remove(objF);
                            AtualizaGridDat05();
                        }

                    }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                _parametros.Dat_05_BAS_Pref = txt_BAS_DAT_05_Prefixo.Text;
                _parametros.Dat_05_CAN_Pref = txt_CAN_DAT_05_Prefixo.Text;
                _parametros.Dat_05_FRM_Pref = txt_FRM_DAT_05_Prefixo.Text;
                _parametros.Dat_05_FRM_SEP = txt_FRM_DAT_05_Separador.Text;
                _parametros.Dat_05_UNT_Pref = txt_UNT_DAT_05_Prefixo.Text;

                _parametros.Dat_05_UNT_1_IsPonto = cmb_UNT_DAT_05_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_05_UNT_2_IsPonto = cmb_UNT_DAT_05_2_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_05_CAN_1_IsPonto = cmb_CAN_DAT_05_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_05_FRM_1_IsPonto = cmb_FRM_DAT_05_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_05_BAS_1_IsPonto = cmb_BAS_DAT_05_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;

                _parametros.Dat_05_BAS_Habilitado = chk_BAS_Dat_05_Habilitado.Checked ? 1 : 0;


                DialogResult = DialogResult.OK;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}