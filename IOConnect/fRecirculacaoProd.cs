using Percolore.Core.Util;
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
    public partial class fRecirculacaoProd : Form
    {
        public Util.ObjectRecircular _recirculacao = null;
        private Util.ObjectParametros _parametros = null;
       
        public fRecirculacaoProd(Util.ObjectRecircular recirculacao)
        {
            InitializeComponent();
            _recirculacao = recirculacao;
            this._parametros = Util.ObjectParametros.Load();
        }

        private void fRecirculacaoProd_Load(object sender, EventArgs e)
        {
            this.btnSair.Text = string.Empty;
            this.btnSair.Image = Imagem.GetSair_32x32();

            this.btnGravar.Text = string.Empty;
            this.btnGravar.Image = Imagem.GetGravar_32x32();
            this.lblTitulo.Text = Negocio.IdiomaResxExtensao.Configuracoes_tabRecircular;

            lbl_TituloRecCircuito.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblTituloRecCircuito;
            lblRecVolDin.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblRecVolDin;
            lblRecPerDias.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblRecPerDias;
            lblRecVol.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblRecVol;

            lblRecisValve.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblRecValula;
            lblRecAutomatico.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblRecAutomatico;

            lblRecCircuito.Text = this._recirculacao.Circuito.ToString("D2"); 
            if(this._recirculacao.Habilitado)
            {
                lblRecCircuito.BackColor = Cores.Seguir;
            }
            else
            {
                lblRecCircuito.BackColor = Cores.Parar;
            }
            txtRecirculacaoVolDin.Text = this._recirculacao.VolumeDin.ToString();
            txtRecirculacaoDias.Text = this._recirculacao.Dias.ToString();
            txtRecirculacaoVol.Text = this._recirculacao.VolumeRecircular.ToString();
            chkRecirculacaoisValve.Checked = this._recirculacao.isValve;
            chkRecirculacaoisAutomatico.Checked = this._recirculacao.isAuto;
            updateTeclado();
        }

        private void updateTeclado()
        {
            try
            {
                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;

                txtRecirculacaoVolDin.isTecladoShow = chb_tec;
                txtRecirculacaoVolDin.isTouchScrenn = chb_touch;

                txtRecirculacaoDias.isTecladoShow = chb_tec;
                txtRecirculacaoDias.isTouchScrenn = chb_touch;

                txtRecirculacaoVol.isTecladoShow = chb_tec;
                txtRecirculacaoVol.isTouchScrenn = chb_touch;



            }
            catch
            { }
        }

        private void btnTeclado_Click(object sender, EventArgs e)
        {
            KeyboardHelper.Show();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            try
            {
                this._recirculacao.Dias = txtRecirculacaoDias.ToInt();
                this._recirculacao.VolumeDin = txtRecirculacaoVolDin.ToDouble();
                this._recirculacao.VolumeRecircular = txtRecirculacaoVol.ToDouble();
                this._recirculacao.isValve = chkRecirculacaoisValve.Checked;
                this._recirculacao.isAuto = chkRecirculacaoisAutomatico.Checked;
                if(this._recirculacao.isAuto)
                {
                    this._recirculacao.isValve = true;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch
            { }
        }

        private void lblRecCircuito_Click(object sender, EventArgs e)
        {
            try
            {  
                bool habilitado = !this._recirculacao.Habilitado;
                this._recirculacao.Habilitado = habilitado;

                if (this._recirculacao.Habilitado)
                {
                    lblRecCircuito.BackColor = Cores.Seguir;
                }
                else
                {
                    lblRecCircuito.BackColor = Cores.Parar;
                }

            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
        }
    }
}
