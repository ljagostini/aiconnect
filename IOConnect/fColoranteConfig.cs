using Percolore.Core.UserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Percolore.Core;
using Percolore.Core.Util;

namespace Percolore.IOConnect
{
    public partial class fColoranteConfig : Form
    {
        public List<Util.ObjectColorante> _colorantes = null;
        private Util.ObjectParametros _parametros = null;
        private CheckBox[] _circuito = null;
        private UTextBox[] _NomeCorante = null;
        private ComboBox[] _Seguidor = null;
        private ComboBox[] _Step = null;
        //Volume Purga é nivel Bico Individual
        private UTextBox[] _VolumePurga = null;

        

        public fColoranteConfig()
        {
            InitializeComponent();
            try
            {
                _parametros = Util.ObjectParametros.Load();
                _colorantes = Util.ObjectColorante.List();

                _NomeCorante = new UTextBox[] {
                    txtColorante0, txtColorante1, txtColorante2, txtColorante3,
                    txtColorante4, txtColorante5, txtColorante6, txtColorante7,
                    txtColorante8, txtColorante9, txtColorante10, txtColorante11,
                    txtColorante12, txtColorante13, txtColorante14, txtColorante15,

                    txtColorante16, txtColorante17, txtColorante18, txtColorante19,
                    txtColorante20, txtColorante21, txtColorante22, txtColorante23,
                    txtColorante24, txtColorante25, txtColorante26, txtColorante27,
                    txtColorante28, txtColorante29, txtColorante30, txtColorante31
                };

                _circuito = new CheckBox[] {
                    chkCircuito00, chkCircuito01, chkCircuito02, chkCircuito03,
                    chkCircuito04, chkCircuito05, chkCircuito06, chkCircuito07,
                    chkCircuito08, chkCircuito09, chkCircuito10, chkCircuito11,
                    chkCircuito12, chkCircuito13, chkCircuito14, chkCircuito15,
                    chkCircuito16, chkCircuito17, chkCircuito18, chkCircuito19,
                    chkCircuito20, chkCircuito21, chkCircuito22, chkCircuito23,
                    chkCircuito24, chkCircuito25, chkCircuito26, chkCircuito27,
                    chkCircuito28, chkCircuito29, chkCircuito30, chkCircuito31
                };
                _Seguidor = new ComboBox[]
                {
                    cmbSeguidor0,cmbSeguidor1,cmbSeguidor2,cmbSeguidor3,
                    cmbSeguidor4,cmbSeguidor5,cmbSeguidor6,cmbSeguidor7,
                    cmbSeguidor8,cmbSeguidor9,cmbSeguidor10,cmbSeguidor11,
                    cmbSeguidor12,cmbSeguidor13,cmbSeguidor14,cmbSeguidor15,
                    cmbSeguidor16,cmbSeguidor17,cmbSeguidor18,cmbSeguidor19,
                    cmbSeguidor20,cmbSeguidor21,cmbSeguidor22,cmbSeguidor23,
                    cmbSeguidor24,cmbSeguidor25,cmbSeguidor26,cmbSeguidor27,
                    cmbSeguidor28,cmbSeguidor29,cmbSeguidor30,cmbSeguidor31
                };
                _Step = new ComboBox[]
                {
                    cmbStep0,cmbStep1,cmbStep2,cmbStep3,
                    cmbStep4,cmbStep5,cmbStep6,cmbStep7,
                    cmbStep8,cmbStep9,cmbStep10,cmbStep11,
                    cmbStep12,cmbStep13,cmbStep14,cmbStep15,
                    cmbStep16,cmbStep17,cmbStep18,cmbStep19,
                    cmbStep20,cmbStep21,cmbStep22,cmbStep23,
                    cmbStep24,cmbStep25,cmbStep26,cmbStep27,
                    cmbStep28,cmbStep29,cmbStep30,cmbStep31
                };
                _VolumePurga = new UTextBox[] 
                {
                    txtPurga0, txtPurga1, txtPurga2, txtPurga3,
                    txtPurga4, txtPurga5, txtPurga6, txtPurga7,
                    txtPurga8, txtPurga9, txtPurga10, txtPurga11,
                    txtPurga12, txtPurga13, txtPurga14, txtPurga15,

                    txtPurga16, txtPurga17, txtPurga18, txtPurga19,
                    txtPurga20, txtPurga21, txtPurga22, txtPurga23,
                    txtPurga24, txtPurga25, txtPurga26, txtPurga27,
                    txtPurga28, txtPurga29, txtPurga30, txtPurga31
                };
                btnTeclado.Text = string.Empty;
                btnTeclado.Image = Imagem.GetTeclado_32x32();
            }
            catch
            { }
        }

        private void fColoranteConfig_Load(object sender, EventArgs e)
        {
            this.btnSair.Text = string.Empty;
            this.btnSair.Image = Imagem.GetSair_32x32();

            this.btnGravar.Text = string.Empty;
            this.btnGravar.Image = Imagem.GetGravar_32x32();
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.ColoranteConfig_lblTitulo;

            this.lblColoranteCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
            this.blColoranteNome.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
            this.lblColoranteSeguidor.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteSeguidor;
            this.lblColorantePurga.Text = Negocio.IdiomaResxExtensao.ProdutoNivelBicoIndividual + "(ml)";

            this.lblColoranteCircuito02.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
            this.blColoranteNome02.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
            this.lblColoranteSeguidor02.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteSeguidor;
            this.lblColorantePurga02.Text = Negocio.IdiomaResxExtensao.ProdutoNivelBicoIndividual + "(ml)";

            this.lblColoranteCircuito03.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
            this.blColoranteNome03.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
            this.lblColoranteSeguidor03.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteSeguidor;
            this.lblColorantePurga03.Text = Negocio.IdiomaResxExtensao.ProdutoNivelBicoIndividual + "(ml)";

            this.lblColoranteCircuito04.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito;
            this.blColoranteNome04.Text = Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome;
            this.lblColoranteSeguidor04.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteSeguidor;
            this.lblColorantePurga04.Text = Negocio.IdiomaResxExtensao.ProdutoNivelBicoIndividual + "(ml)";

            updateTeclado();

            try
            {
                switch((Dispositivo)_parametros.IdDispositivo2)
                {
                    case Dispositivo.Placa_2:
                        {
                            gpPlaca2.Enabled = true;
                            gpPlaca2.Visible = true;
                            break;
                        }
                    //case Dispositivo.Placa_4:
                    //    {
                    //        gpPlaca2.Enabled = true;
                    //        gpPlaca2.Visible = true;
                    //        break;
                    //    }
                    case Dispositivo.Simulador:
                        {
                            gpPlaca2.Enabled = true;
                            gpPlaca2.Visible = true;
                            break;
                        }
                    default:
                        {
                            gpPlaca2.Enabled = false;
                            gpPlaca2.Visible = false;
                            break;
                        }
                }
            }
            catch
            { }
            atualizaCombos();
        }

        private void updateTeclado()
        {
            try
            {
                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;
                for(int i = 0; i < _VolumePurga.Length; i++)
                {
                    _VolumePurga[i].isTecladoShow = chb_tec;
                    _VolumePurga[i].isTouchScrenn = chb_touch;
                }
            }
            catch
            { }
        }

        private void atualizaCombos()
        {
            try
            {  
                

                cmbSeguidor0.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor0.ValueMember = "Value";
                cmbSeguidor0.DisplayMember = "Display";

                cmbSeguidor1.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor1.ValueMember = "Value";
                cmbSeguidor1.DisplayMember = "Display";

                cmbSeguidor2.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor2.ValueMember = "Value";
                cmbSeguidor2.DisplayMember = "Display";

                cmbSeguidor3.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor3.ValueMember = "Value";
                cmbSeguidor3.DisplayMember = "Display";

                cmbSeguidor4.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor4.ValueMember = "Value";
                cmbSeguidor4.DisplayMember = "Display";

                cmbSeguidor5.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor5.ValueMember = "Value";
                cmbSeguidor5.DisplayMember = "Display";

                cmbSeguidor6.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor6.ValueMember = "Value";
                cmbSeguidor6.DisplayMember = "Display";

                cmbSeguidor7.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor7.ValueMember = "Value";
                cmbSeguidor7.DisplayMember = "Display";

                cmbSeguidor8.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor8.ValueMember = "Value";
                cmbSeguidor8.DisplayMember = "Display";


                cmbSeguidor9.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor9.ValueMember = "Value";
                cmbSeguidor9.DisplayMember = "Display";


                cmbSeguidor10.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor10.ValueMember = "Value";
                cmbSeguidor10.DisplayMember = "Display";


                cmbSeguidor11.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor11.ValueMember = "Value";
                cmbSeguidor11.DisplayMember = "Display";

                cmbSeguidor12.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor12.ValueMember = "Value";
                cmbSeguidor12.DisplayMember = "Display";

                cmbSeguidor13.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor13.ValueMember = "Value";
                cmbSeguidor13.DisplayMember = "Display";

                cmbSeguidor14.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor14.ValueMember = "Value";
                cmbSeguidor14.DisplayMember = "Display";

                cmbSeguidor15.DataSource = getCombobox(true).DefaultView;
                cmbSeguidor15.ValueMember = "Value";
                cmbSeguidor15.DisplayMember = "Display";

                cmbSeguidor16.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor16.ValueMember = "Value";
                cmbSeguidor16.DisplayMember = "Display";

                cmbSeguidor17.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor17.ValueMember = "Value";
                cmbSeguidor17.DisplayMember = "Display";

                cmbSeguidor18.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor18.ValueMember = "Value";
                cmbSeguidor18.DisplayMember = "Display";

                cmbSeguidor19.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor19.ValueMember = "Value";
                cmbSeguidor19.DisplayMember = "Display";

                cmbSeguidor20.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor20.ValueMember = "Value";
                cmbSeguidor20.DisplayMember = "Display";

                cmbSeguidor21.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor21.ValueMember = "Value";
                cmbSeguidor21.DisplayMember = "Display";

                cmbSeguidor22.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor22.ValueMember = "Value";
                cmbSeguidor22.DisplayMember = "Display";

                cmbSeguidor23.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor23.ValueMember = "Value";
                cmbSeguidor23.DisplayMember = "Display";

                cmbSeguidor24.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor24.ValueMember = "Value";
                cmbSeguidor24.DisplayMember = "Display";

                cmbSeguidor25.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor25.ValueMember = "Value";
                cmbSeguidor25.DisplayMember = "Display";

                cmbSeguidor26.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor26.ValueMember = "Value";
                cmbSeguidor26.DisplayMember = "Display";

                cmbSeguidor27.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor27.ValueMember = "Value";
                cmbSeguidor27.DisplayMember = "Display";

                cmbSeguidor28.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor28.ValueMember = "Value";
                cmbSeguidor28.DisplayMember = "Display";

                cmbSeguidor29.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor29.ValueMember = "Value";
                cmbSeguidor29.DisplayMember = "Display";

                cmbSeguidor30.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor30.ValueMember = "Value";
                cmbSeguidor30.DisplayMember = "Display";

                cmbSeguidor31.DataSource = getCombobox(false).DefaultView;
                cmbSeguidor31.ValueMember = "Value";
                cmbSeguidor31.DisplayMember = "Display";

               



                cmbStep0.DataSource = getComboboxStep().DefaultView;
                cmbStep0.ValueMember = "Value";
                cmbStep0.DisplayMember = "Display";

                cmbStep1.DataSource = getComboboxStep().DefaultView;
                cmbStep1.ValueMember = "Value";
                cmbStep1.DisplayMember = "Display";

                cmbStep2.DataSource = getComboboxStep().DefaultView;
                cmbStep2.ValueMember = "Value";
                cmbStep2.DisplayMember = "Display";

                cmbStep3.DataSource = getComboboxStep().DefaultView;
                cmbStep3.ValueMember = "Value";
                cmbStep3.DisplayMember = "Display";

                cmbStep4.DataSource = getComboboxStep().DefaultView;
                cmbStep4.ValueMember = "Value";
                cmbStep4.DisplayMember = "Display";

                cmbStep5.DataSource = getComboboxStep().DefaultView;
                cmbStep5.ValueMember = "Value";
                cmbStep5.DisplayMember = "Display";

                cmbStep6.DataSource = getComboboxStep().DefaultView;
                cmbStep6.ValueMember = "Value";
                cmbStep6.DisplayMember = "Display";

                cmbStep7.DataSource = getComboboxStep().DefaultView;
                cmbStep7.ValueMember = "Value";
                cmbStep7.DisplayMember = "Display";

                cmbStep8.DataSource = getComboboxStep().DefaultView;
                cmbStep8.ValueMember = "Value";
                cmbStep8.DisplayMember = "Display";

                cmbStep9.DataSource = getComboboxStep().DefaultView;
                cmbStep9.ValueMember = "Value";
                cmbStep9.DisplayMember = "Display";

                cmbStep10.DataSource = getComboboxStep().DefaultView;
                cmbStep10.ValueMember = "Value";
                cmbStep10.DisplayMember = "Display";

                cmbStep11.DataSource = getComboboxStep().DefaultView;
                cmbStep11.ValueMember = "Value";
                cmbStep11.DisplayMember = "Display";

                cmbStep12.DataSource = getComboboxStep().DefaultView;
                cmbStep12.ValueMember = "Value";
                cmbStep12.DisplayMember = "Display";

                cmbStep13.DataSource = getComboboxStep().DefaultView;
                cmbStep13.ValueMember = "Value";
                cmbStep13.DisplayMember = "Display";

                cmbStep14.DataSource = getComboboxStep().DefaultView;
                cmbStep14.ValueMember = "Value";
                cmbStep14.DisplayMember = "Display";

                cmbStep15.DataSource = getComboboxStep().DefaultView;
                cmbStep15.ValueMember = "Value";
                cmbStep15.DisplayMember = "Display";

                cmbStep16.DataSource = getComboboxStep().DefaultView;
                cmbStep16.ValueMember = "Value";
                cmbStep16.DisplayMember = "Display";

                cmbStep17.DataSource = getComboboxStep().DefaultView;
                cmbStep17.ValueMember = "Value";
                cmbStep17.DisplayMember = "Display";

                cmbStep18.DataSource = getComboboxStep().DefaultView;
                cmbStep18.ValueMember = "Value";
                cmbStep18.DisplayMember = "Display";

                cmbStep19.DataSource = getComboboxStep().DefaultView;
                cmbStep19.ValueMember = "Value";
                cmbStep19.DisplayMember = "Display";

                cmbStep20.DataSource = getComboboxStep().DefaultView;
                cmbStep20.ValueMember = "Value";
                cmbStep20.DisplayMember = "Display";

                cmbStep21.DataSource = getComboboxStep().DefaultView;
                cmbStep21.ValueMember = "Value";
                cmbStep21.DisplayMember = "Display";

                cmbStep22.DataSource = getComboboxStep().DefaultView;
                cmbStep22.ValueMember = "Value";
                cmbStep22.DisplayMember = "Display";

                cmbStep23.DataSource = getComboboxStep().DefaultView;
                cmbStep23.ValueMember = "Value";
                cmbStep23.DisplayMember = "Display";

                cmbStep24.DataSource = getComboboxStep().DefaultView;
                cmbStep24.ValueMember = "Value";
                cmbStep24.DisplayMember = "Display";

                cmbStep25.DataSource = getComboboxStep().DefaultView;
                cmbStep25.ValueMember = "Value";
                cmbStep25.DisplayMember = "Display";

                cmbStep26.DataSource = getComboboxStep().DefaultView;
                cmbStep26.ValueMember = "Value";
                cmbStep26.DisplayMember = "Display";

                cmbStep27.DataSource = getComboboxStep().DefaultView;
                cmbStep27.ValueMember = "Value";
                cmbStep27.DisplayMember = "Display";

                cmbStep28.DataSource = getComboboxStep().DefaultView;
                cmbStep28.ValueMember = "Value";
                cmbStep28.DisplayMember = "Display";

                cmbStep29.DataSource = getComboboxStep().DefaultView;
                cmbStep29.ValueMember = "Value";
                cmbStep29.DisplayMember = "Display";

                cmbStep30.DataSource = getComboboxStep().DefaultView;
                cmbStep30.ValueMember = "Value";
                cmbStep30.DisplayMember = "Display";

                cmbStep31.DataSource = getComboboxStep().DefaultView;
                cmbStep31.ValueMember = "Value";
                cmbStep31.DisplayMember = "Display";

                

                _colorantes = Util.ObjectColorante.List();

                for (int i = 0; i < _colorantes.Count; i++)
                {
                    if(!_colorantes[i].Habilitado)
                    {
                        _circuito[i].Enabled = false;
                        _circuito[i].Text = _colorantes[i].Circuito.ToString();
                        _circuito[i].BackColor = Cores.Parar;
                        _Seguidor[i].SelectedIndex = 0;
                        _Seguidor[i].Enabled = false;
                        _Step[i].SelectedIndex = 0;
                        _Step[i].Enabled = false;
                        _NomeCorante[i].Enabled = false;
                        _NomeCorante[i].Text = _colorantes[i].Nome;
                    }
                    else
                    {
                        if (_colorantes[i].Seguidor > 0)
                        {
                            _circuito[i].Enabled = false;
                            _circuito[i].Text = _colorantes[i].Circuito.ToString();
                            _circuito[i].BackColor = Cores.Seguidor_Tom_01;
                            _Seguidor[i].SelectedValue = _colorantes[i].Seguidor.ToString();
                            _Seguidor[i].Enabled = true;                            
                            _NomeCorante[i].Enabled = false;
                            _NomeCorante[i].Text = _colorantes[i].Nome;
                        }
                        else
                        {

                            _circuito[i].Enabled = false;
                            _circuito[i].Text = _colorantes[i].Circuito.ToString();
                            _circuito[i].BackColor = Cores.Seguir;
                            _Seguidor[i].SelectedValue = _colorantes[i].Seguidor.ToString();
                            _Seguidor[i].Enabled = true;
                            _NomeCorante[i].Enabled = false;
                            _NomeCorante[i].Text = _colorantes[i].Nome;
                        }
                        _Step[i].SelectedValue = _colorantes[i].Step.ToString();
                        _Step[i].Enabled = true;
                    }
                    _VolumePurga[i].Text = _colorantes[i].VolumeBicoIndividual.ToString();
                }

            }
            catch
            { }
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
                    else if(i == 0)
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
                        dr["Value"] = (i+16).ToString();
                        dr["Display"] = (i+16).ToString();
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

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {            
            
            for (int i = 0; i < _colorantes.Count; i++)
            {
                try
                {
                    double _value = 0;
                    if(double.TryParse(_VolumePurga[i].Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _value) && _value > 0)
                    {
                        _colorantes[i].VolumeBicoIndividual = _value;
                    }
                    else
                    {
                        _colorantes[i].VolumeBicoIndividual = 5;
                    }
                }
                catch
                { 
                    _colorantes[i].VolumeBicoIndividual = 5; 
                }
            }

            
            this.DialogResult = DialogResult.OK;

        }

        private void cmbSeguidor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;
                int index = Convert.ToInt32(cmb.Tag.ToString());
                if (cmb.SelectedIndex == 0)
                {
                    _circuito[index].BackColor = Cores.Seguir;
                    _colorantes[index].Seguidor = -1;
                    _colorantes[index].Habilitado = true;
                }
                else
                {
                    _circuito[index].BackColor = Cores.Seguidor_Tom_01;
                    _colorantes[index].Seguidor = Convert.ToInt32(cmb.SelectedValue.ToString());
                    _colorantes[index].Habilitado = false;
                }
            }
            catch
            { }
        }
        
        private void cmbStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;
                int index = Convert.ToInt32(cmb.Tag.ToString());
                _colorantes[index].Step = Convert.ToInt32(cmb.SelectedValue.ToString());
                
            }
            catch
            { }
        }

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }
    }
}
