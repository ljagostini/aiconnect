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
    public partial class fBasDat05 : Form
    {
        public int myAction = 0;
        public Util.ObjectBasDat05 objRetorno = null;
        private bool isNew = false;

        public fBasDat05(Util.ObjectBasDat05 obj, bool is_New)
        {
            InitializeComponent();
            this.isNew = is_New;
            try
            {
                this.objRetorno = obj;

            }
            catch
            { }
        }     

        private void fBasDat05_Load(object sender, EventArgs e)
        {
            try
            {
                lblProduct.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos + ":";
                lblVolume.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume + ":";
                lblCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito + ":";

                Util.ObjectParametros _parametros = Util.ObjectParametros.Load();

                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;


                txtVolume.isTecladoShow = chb_tec;
                txtVolume.isTouchScrenn = chb_touch;

                txt_Circuito.isTecladoShow = chb_tec;
                txt_Circuito.isTouchScrenn = chb_touch;

                txt_Nome.isTecladoShow = chb_tec;
                txt_Nome.isTouchScrenn = chb_touch;

                btnSair.Text = string.Empty;
                btnSair.Image = Imagem.GetSair_32x32();
                btnConfirmar.Text = string.Empty;
                btnConfirmar.Image = Imagem.GetGravar_32x32();
                btnExcluir.Text = string.Empty;
                btnExcluir.Image = Imagem.GetExcluir_32x32();

                

            }
            catch
            { }

            try
            {
                if (this.isNew)
                {
                    btnExcluir.Enabled = false;
                    btnExcluir.Visible = false;
                }

                txt_Nome.Text = this.objRetorno.Name;
                txtVolume.Text = this.objRetorno.Volume.ToString();
                txt_Circuito.Text = this.objRetorno.Circuito.ToString();




            }
            catch
            { }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (txtVolume.ToDouble() > 0 && txt_Circuito.ToInt() >= 0 && txt_Nome.Text != "")
            {
                objRetorno = new Util.ObjectBasDat05();
                objRetorno.Name = txt_Nome.Text;
                objRetorno.Circuito = txt_Circuito.ToInt();
                objRetorno.Volume = txtVolume.ToDouble();

            }
            this.myAction = 1;
            DialogResult = DialogResult.OK;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (txtVolume.ToDouble() > 0 && txt_Circuito.ToInt() >= 0 && txt_Nome.Text != "")
            {
                objRetorno = new Util.ObjectBasDat05();
                objRetorno.Name = txt_Nome.Text;
                objRetorno.Circuito = txt_Circuito.ToInt();
                objRetorno.Volume = txtVolume.ToDouble();

            }
            this.myAction = 2;
            DialogResult = DialogResult.OK;
        }

    }
}

