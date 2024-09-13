using Percolore.Core.UserControl;
using Percolore.IOConnect.Util;
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
    public partial class fDat06 : Form
    {
        private int index_selGridBasDat06;
        private List<Util.ObjectBasDat06> listBasDat06 = new List<ObjectBasDat06>();
        public Util.ObjectParametros _parametros = null;
        public fDat06()
        {
            InitializeComponent();
        }

        private void fDat06_Load(object sender, EventArgs e)
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
                chk_BAS_Dat_06_Habilitado.Text = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Habilitado;


                _parametros = Util.ObjectParametros.Load();
                txt_BAS_DAT_06_Prefixo.Text = _parametros.Dat_06_BAS_Pref;
                txt_CAN_DAT_06_Prefixo.Text = _parametros.Dat_06_CAN_Pref;
                txt_FRM_DAT_06_Prefixo.Text = _parametros.Dat_06_FRM_Pref;
                txt_UNT_DAT_06_Prefixo.Text = _parametros.Dat_06_UNT_Pref;

                txt_FRM_DAT_06_Separador.Text = _parametros.Dat_06_FRM_SEP;

                DataTable dt_UNT_1_IsPonto = new DataTable();
                dt_UNT_1_IsPonto.Columns.Add("Value");
                dt_UNT_1_IsPonto.Columns.Add("Display");
                for (int i = 0; i <= 1; i++)
                {
                    DataRow dr = dt_UNT_1_IsPonto.NewRow();
                    dr["Value"] = i.ToString();
                    if (i==0)
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula/*"Virgula"*/;
                    }
                    else
                    {
                        dr["Display"] = Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto/*"Ponto"*/;
                    }
                    dt_UNT_1_IsPonto.Rows.Add(dr);
                }
                cmb_UNT_DAT_06_1_Ponto.DataSource = dt_UNT_1_IsPonto.DefaultView;
                cmb_UNT_DAT_06_1_Ponto.DisplayMember = "Display";
                cmb_UNT_DAT_06_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_06_UNT_1_IsPonto == 0)
                {
                    cmb_UNT_DAT_06_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_UNT_DAT_06_1_Ponto.SelectedIndex = 1;
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
                cmb_UNT_DAT_06_2_Ponto.DataSource = dt_UNT_2_IsPonto.DefaultView;
                cmb_UNT_DAT_06_2_Ponto.DisplayMember = "Display";
                cmb_UNT_DAT_06_2_Ponto.ValueMember = "Value";

                if (_parametros.Dat_06_UNT_2_IsPonto == 0)
                {
                    cmb_UNT_DAT_06_2_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_UNT_DAT_06_2_Ponto.SelectedIndex = 1;
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
                cmb_CAN_DAT_06_1_Ponto.DataSource = dt_CAN_1_IsPonto.DefaultView;
                cmb_CAN_DAT_06_1_Ponto.DisplayMember = "Display";
                cmb_CAN_DAT_06_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_06_CAN_1_IsPonto == 0)
                {
                    cmb_CAN_DAT_06_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_CAN_DAT_06_1_Ponto.SelectedIndex = 1;
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
                cmb_FRM_DAT_06_1_Ponto.DataSource = dt_FRM_1_IsPonto.DefaultView;
                cmb_FRM_DAT_06_1_Ponto.DisplayMember = "Display";
                cmb_FRM_DAT_06_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_06_FRM_1_IsPonto == 0)
                {
                    cmb_FRM_DAT_06_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_FRM_DAT_06_1_Ponto.SelectedIndex = 1;
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
                cmb_BAS_DAT_06_1_Ponto.DataSource = dt_BAS_1_IsPonto.DefaultView;
                cmb_BAS_DAT_06_1_Ponto.DisplayMember = "Display";
                cmb_BAS_DAT_06_1_Ponto.ValueMember = "Value";

                if (_parametros.Dat_06_BAS_1_IsPonto == 0)
                {
                    cmb_BAS_DAT_06_1_Ponto.SelectedIndex = 0;
                }
                else
                {
                    cmb_BAS_DAT_06_1_Ponto.SelectedIndex = 1;
                }

                chk_BAS_Dat_06_Habilitado.Checked = _parametros.Dat_06_BAS_Habilitado == 1 ? true : false;

                AtualizaGridDat06();

                btnSair.Text = string.Empty;
                btnSair.Image = Imagem.GetSair_32x32();
                btnConfirmar.Text = string.Empty;
                btnConfirmar.Image = Imagem.GetGravar_32x32();

                btnAddBasDat6.Text = string.Empty;
                btnAddBasDat6.Image = Imagem.GetAdicionar_32x32();
                
            }
            catch
            { }

        }

        private void AtualizaGridDat06()
        {
            try
            {
                this.listBasDat06 = Util.ObjectBasDat06.List();

                DataTable dt = new DataTable();
                dt.Columns.Add(Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos);
                dt.Columns.Add(Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume);
                dt.Columns.Add(Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito);
                foreach (Util.ObjectBasDat06 to in this.listBasDat06)
                {
                    DataRow dr = dt.NewRow();
                    dr[Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos] = to.Name;
                    dr[Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume] = to.Volume.ToString().Replace(",", ".");
                    dr[Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito] = to.Circuito.ToString();
                    dt.Rows.Add(dr);

                }
                dgBasDat06.DataSource = dt.DefaultView;

                this.index_selGridBasDat06 = -1;

            }
            catch
            { }

        }

        private void btnAddBasDat6_Click(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectBasDat06 obj = new ObjectBasDat06();
                obj.Circuito = 0;
                obj.Name = "";
                obj.Volume = 0.0;
                fBasDat06 fbd06 = new fBasDat06(obj, true);
                DialogResult dr = fbd06.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string msg = "";
                    ObjectBasDat06 objF = fbd06.objRetorno;
                    List<ObjectBasDat06> lObj = new List<ObjectBasDat06>();
                    lObj.Add(objF);
                    if (fbd06.myAction == 1)
                    {
                        if (objF == null || !Util.ObjectBasDat06.Validate(lObj, out msg))
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos + Environment.NewLine + Environment.NewLine + msg);
                            }

                        }
                        else
                        {
                            Util.ObjectBasDat06.Persist(objF);
                            AtualizaGridDat06();
                        }

                    }

                }
            }
            catch
            {

            }
        }

        private void dgBasDat06_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.index_selGridBasDat06 = e.RowIndex;
                Util.ObjectBasDat06 obj = this.listBasDat06[this.index_selGridBasDat06];
                fBasDat06 fbd06 = new fBasDat06(obj, false);
                DialogResult dr = fbd06.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string msg = "";
                    ObjectBasDat06 objF = fbd06.objRetorno;
                    List<ObjectBasDat06> lObj = new List<ObjectBasDat06>();
                    lObj.Add(objF);
                    if (fbd06.myAction == 1)
                    {

                        if (objF == null || !Util.ObjectBasDat06.Validate(lObj, out msg))
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos + Environment.NewLine + Environment.NewLine + msg);
                            }
                        }
                        else
                        {
                            Util.ObjectBasDat06.Persist(objF);
                            AtualizaGridDat06();
                        }

                    }
                    else
                    {
                        if (objF == null || !Util.ObjectBasDat06.Validate(lObj, out msg))
                        {
                            using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                            {
                                m.ShowDialog(Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos + Environment.NewLine + Environment.NewLine + msg);
                            }
                        }
                        else
                        {
                            Util.ObjectBasDat06.Remove(objF);
                            AtualizaGridDat06();
                        }

                    }
                }
            }
            catch
            {

            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                _parametros.Dat_06_BAS_Pref = txt_BAS_DAT_06_Prefixo.Text;
                _parametros.Dat_06_CAN_Pref = txt_CAN_DAT_06_Prefixo.Text;
                _parametros.Dat_06_FRM_Pref = txt_FRM_DAT_06_Prefixo.Text;
                _parametros.Dat_06_FRM_SEP = txt_FRM_DAT_06_Separador.Text;
                _parametros.Dat_06_UNT_Pref = txt_UNT_DAT_06_Prefixo.Text;

                //_parametros.Dat_06_CAN_1_IsPonto = chk_CAN_DAT_06_1_Ponto.Checked ? 1 : 0;
                //_parametros.Dat_06_FRM_1_IsPonto = chk_FRM_DAT_06_1_Ponto.Checked ? 1 : 0;
                //_parametros.Dat_06_UNT_1_IsPonto = chk_UNT_DAT_06_1_Ponto.Checked ? 1 : 0;
                //_parametros.Dat_06_UNT_2_IsPonto = chk_UNT_DAT_06_2_Ponto.Checked ? 1 : 0;
                //_parametros.Dat_06_BAS_1_IsPonto = chk_BAS_DAT_06_1_Ponto.Checked ? 1 : 0;

                _parametros.Dat_06_UNT_1_IsPonto = cmb_UNT_DAT_06_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_06_UNT_2_IsPonto = cmb_UNT_DAT_06_2_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_06_CAN_1_IsPonto = cmb_CAN_DAT_06_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_06_FRM_1_IsPonto = cmb_FRM_DAT_06_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;
                _parametros.Dat_06_BAS_1_IsPonto = cmb_BAS_DAT_06_1_Ponto.SelectedValue.ToString() == "1" ? 1 : 0;

                _parametros.Dat_06_BAS_Habilitado  = chk_BAS_Dat_06_Habilitado.Checked ? 1 : 0;


                DialogResult = DialogResult.OK;
            }
            catch
            { }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
