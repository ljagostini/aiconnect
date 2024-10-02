using Percolore.Core.Logging;
using Percolore.Core.Util;
using System.Data;

namespace Percolore.IOConnect
{
	public partial class fProdutoConfig : Form
    {
        public Util.ObjectColorante _colorante = null;
        private Util.ObjectParametros _parametros = null;
        public fProdutoConfig(Util.ObjectColorante colorante)
        {
            InitializeComponent();
            this._colorante = colorante;
            this._parametros = Util.ObjectParametros.Load();
        }

        private void fProdutoConfig_Load(object sender, EventArgs e)
        {
            this.btnSair.Text = string.Empty;
            this.btnSair.Image = Imagem.GetSair_32x32();

            this.btnGravar.Text = string.Empty;
            this.btnGravar.Image = Imagem.GetGravar_32x32();
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.ColoranteConfig_lblTitulo;

          
            chkCircuito.Text = this._colorante.Circuito.ToString("D2");
            txtColorante.Text = this._colorante.Nome;
            txtMassaEsp.Text = this._colorante.MassaEspecifica.ToString();
            txtPurga.Text = this._colorante.VolumePurga.ToString();
            txtVolumeMinimo.Text = this._colorante.NivelMinimo.ToString();
            txtVolumeMaximo.Text = this._colorante.NivelMaximo.ToString();

            chkBase.Checked = this._colorante.IsBase;

            cmbCorrespondencia.SelectedIndex = this._colorante.Correspondencia;
           

            if (this._colorante.Dispositivo == 1)
            {
                cmbSeguidor.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor.ValueMember = "Value";
                cmbSeguidor.DisplayMember = "Display";
            }
            else
            {
                cmbSeguidor.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor.ValueMember = "Value";
                cmbSeguidor.DisplayMember = "Display";
            }

            cmbStep.DataSource = getComboboxStep().DefaultView;
            cmbStep.ValueMember = "Value";
            cmbStep.DisplayMember = "Display";
            chkBicoIndividual.Checked = this._colorante.IsBicoIndividual;
            txtVolumeBicoIndividual.Text = this._colorante.VolumeBicoIndividual.ToString();

            this.Refresh();

            cmbSeguidor.SelectedValue = this._colorante.Seguidor.ToString();
            this.Refresh();


            chkCircuito.Checked = this._colorante.Habilitado;

            chkCircuito.BackColor = Cores.Parar;
            chkCircuito.FlatAppearance.CheckedBackColor = Cores.Seguir;

            if (!this._colorante.Habilitado)
            {
                txtColorante.Enabled = false;
                txtMassaEsp.Enabled = false;
                txtPurga.Enabled = false;
                txtVolumeMaximo.Enabled = false;
                txtVolumeMinimo.Enabled = false;
                cmbCorrespondencia.Enabled = false;
                chkBase.Enabled = false;

                cmbSeguidor.SelectedIndex = 0;
                cmbSeguidor.Enabled = false;

                cmbStep.SelectedIndex = 0;
                cmbStep.Enabled = false;

                chkBicoIndividual.Enabled = false;
                txtVolumeBicoIndividual.Enabled = false;
            }
            else
            {
                if (this._colorante.Seguidor > 0)
                {  
                    chkCircuito.FlatAppearance.CheckedBackColor = Cores.Seguidor_Tom_01;
                    chkCircuito.BackColor = Cores.Seguidor_Tom_01;

                   
                    cmbSeguidor.Enabled = true;
                   
                }
                else
                {

                    //chkCircuito.Enabled = false;
                  
                    
                    cmbSeguidor.Enabled = true;
                  
                }
            }

            pnlColor.BackColor = this._colorante.corCorante;

            lblColoranteCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
            lblColoranteNome.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
            lblColoranteBase.Text = "Base";
            lblColoranteSeguidor.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteSeguidor;
            lblColorantePurga.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabParametrosPurga;            
            blColoranteMassa.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteMassa;
            lblColoranteCorrespondencia.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCorrespondencia;
            lblNivelMinimo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMinimo;
            lblNivelMaximo.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMaximo;
            lblColoranteStep.Text = "Step";

            lblColor.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCor;

            lblColoranteBicoIndividual.Text = Negocio.IdiomaResxExtensao.ProdutoBicoIndividual;
            lblNivelBicoIndividual.Text = Negocio.IdiomaResxExtensao.ProdutoNivelBicoIndividual;




            updateTeclado();
        }

        private void updateTeclado()
        {
            try
            {
                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;

                txtColorante.isTecladoShow = chb_tec;
                txtColorante.isTouchScrenn = chb_touch;

                txtMassaEsp.isTecladoShow = chb_tec;
                txtMassaEsp.isTouchScrenn = chb_touch;

                txtPurga.isTecladoShow = chb_tec;
                txtPurga.isTouchScrenn = chb_touch;

                txtVolumeMaximo.isTecladoShow = chb_tec;
                txtVolumeMaximo.isTouchScrenn = chb_touch;

                txtVolumeMinimo.isTecladoShow = chb_tec;
                txtVolumeMinimo.isTouchScrenn = chb_touch;

                txtVolumeBicoIndividual.isTecladoShow = chb_tec;
                txtVolumeBicoIndividual.isTouchScrenn = chb_touch;

            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private DataTable getCombobox(bool isP1)
        {
            DataTable dtRetorno = new DataTable();
            dtRetorno.Columns.Add("Value");
            dtRetorno.Columns.Add("Display");
            if (isP1)
            {
                for (int i = -1; i < 17; i++)
                {
                    if (i == -1)
                    {
                        DataRow dr = dtRetorno.NewRow();
                        dr["Value"] = i.ToString();
                        dr["Display"] = "Não";
                        dtRetorno.Rows.Add(dr);
                    }
                    else if (i == 0)
                    {

                    }
                    else
                    {
                        DataRow dr = dtRetorno.NewRow();
                        dr["Value"] = i.ToString();
                        dr["Display"] = i.ToString();
                        dtRetorno.Rows.Add(dr);
                    }
                }
            }
            else
            {
                for (int i = -1; i < 25; i++)
                {
                    if (i == -1)
                    {
                        DataRow dr = dtRetorno.NewRow();
                        dr["Value"] = i.ToString();
                        dr["Display"] = "Não";
                        dtRetorno.Rows.Add(dr);
                    }
                    else if (i == 0)
                    {

                    }
                    else
                    {
                        DataRow dr = dtRetorno.NewRow();
                        dr["Value"] = (i + 16).ToString();
                        dr["Display"] = (i + 16).ToString();
                        dtRetorno.Rows.Add(dr);
                    }
                }
            }

            return dtRetorno;
        }

        private DataTable getCombobox24Motores()
        {
            DataTable dtRetorno = new DataTable();
            dtRetorno.Columns.Add("Value");
            dtRetorno.Columns.Add("Display");

            for (int i = -1; i < 25; i++)
            {
                if (i == -1)
                {
                    DataRow dr = dtRetorno.NewRow();
                    dr["Value"] = i.ToString();
                    dr["Display"] = "Não";
                    dtRetorno.Rows.Add(dr);
                }
                else if (i == 0)
                {

                }
                else
                {
                    DataRow dr = dtRetorno.NewRow();
                    dr["Value"] = i.ToString();
                    dr["Display"] = i.ToString();
                    dtRetorno.Rows.Add(dr);
                }
            }


            return dtRetorno;
        }

        private DataTable getComboboxStep()
        {
            DataTable dtRetorno = new DataTable();
            dtRetorno.Columns.Add("Value");
            dtRetorno.Columns.Add("Display");
            DataRow dr_z = dtRetorno.NewRow();
            dr_z["Value"] = 0.ToString();
            dr_z["Display"] = "Não";
            dtRetorno.Rows.Add(dr_z);
            for (int i = 2; i <= 32; i *= 2)
            {
                DataRow dr = dtRetorno.NewRow();
                dr["Value"] = i.ToString();
                dr["Display"] = i.ToString();
                dtRetorno.Rows.Add(dr);
            }


            return dtRetorno;
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            try
            {
                this._colorante.Habilitado = chkCircuito.Checked;
                this._colorante.IsBase = chkBase.Checked;
                int correspondente = 0;
                int.TryParse(cmbCorrespondencia.Text, out correspondente);
                this._colorante.Correspondencia = correspondente;
                this._colorante.MassaEspecifica = txtMassaEsp.ToDouble();
                if (this._colorante.Habilitado)
                {
                    this._colorante.NivelMaximo = txtVolumeMaximo.ToDouble();
                    this._colorante.NivelMinimo = txtVolumeMinimo.ToDouble();
                }
                else
                {
                    this._colorante.NivelMaximo = 0;
                    this._colorante.NivelMinimo = 0;
                }
               
                
                this._colorante.Nome = txtColorante.Text;
                this._colorante.Seguidor = Convert.ToInt32(cmbSeguidor.SelectedValue.ToString());
                this._colorante.Step = Convert.ToInt32(cmbStep.SelectedValue.ToString());
                this._colorante.VolumePurga = txtPurga.ToDouble();
                this._colorante.corCorante = pnlColor.BackColor;
                string _rgb = this._colorante.corCorante.R + ";" + this._colorante.corCorante.G + ";" + this._colorante.corCorante.B + ";";
                this._colorante.ColorRGB = _rgb;

                if (chkBase.Checked)
                {
                    this._colorante.IsBicoIndividual = false;
                }
                else
                {
                    this._colorante.IsBicoIndividual = chkBicoIndividual.Checked;
                }
                this._colorante.VolumeBicoIndividual = txtVolumeBicoIndividual.ToDouble();

                this.DialogResult = DialogResult.OK;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void chkCircuito_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)sender;

                txtMassaEsp.Enabled = (chk.Checked);
                txtColorante.Enabled = (chk.Checked);
                cmbCorrespondencia.Enabled = (chk.Checked);
                chkBase.Enabled = (chk.Checked);

                txtMassaEsp.Enabled = (chk.Checked);
                txtPurga.Enabled = (chk.Checked);
                txtVolumeMaximo.Enabled = (chk.Checked);
                txtVolumeMinimo.Enabled = (chk.Checked);

                cmbSeguidor.Enabled = (chk.Checked);
                cmbStep.Enabled = (chk.Checked);

                if (!chk.Checked)
                {
                    chkCircuito.BackColor = Cores.Parar;                   
                }
                else
                {
                    if(cmbSeguidor.SelectedValue.ToString() != "-1")
                    {   
                        chkCircuito.FlatAppearance.CheckedBackColor = Cores.Seguidor_Tom_01;
                        chkCircuito.BackColor = Cores.Seguidor_Tom_01;
                    }
                    else
                    {
                        chkCircuito.BackColor = Cores.Seguir;
                    }
                }
                chkBicoIndividual.Enabled = (chk.Checked);
                txtVolumeBicoIndividual.Enabled = (chk.Checked);
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void pnlColor_Click(object sender, EventArgs e)
        {
            try
            {
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.CustomColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(this._colorante.corCorante.R, _colorante.corCorante.G, _colorante.corCorante.B)) };
                colorDialog.FullOpen = true;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    pnlColor.BackColor = colorDialog.Color;
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}
    }
}