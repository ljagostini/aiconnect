using Percolore.Core;
using Percolore.Core.Logging;

namespace Percolore.IOConnect
{
	public partial class fCalibracao : Form
    {
        private Util.ObjectCalibragem _calibracao;
        public ValoresVO valores = null;
        Util.ObjectColorante colorante = null;

        public fCalibracao(Util.ObjectCalibragem _cal)
        {
            _calibracao = _cal;
            InitializeComponent();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            double volume = 0;
            int pulsos = 0;
            int acel = 0;
            int velocidade = 0;
            int delay = 0;
            int pulsoRev = 0;

            double.TryParse(txtVolume.Text, out volume);
            int.TryParse(txtPulsos.Text, out pulsos);
            int.TryParse(txtAceleracao.Text, out acel);
            int.TryParse(txtDelay.Text, out delay);
            int.TryParse(txtVelocidade.Text, out velocidade);
            int.TryParse(txtPulsoReverso.Text, out pulsoRev);
            if (volume > 0 && pulsos > 0 && acel > 0 && velocidade > 0 && delay > 0 && pulsoRev >= 0 )
            {
                bool exist_volume = false;
                for(int i = 0; i < this._calibracao.Valores.Count; i++ )
                {
                    if(volume == this._calibracao.Valores[i].Volume)
                    {
                        exist_volume = true;
                        break;
                    }
                }

                if (!exist_volume)
                {
                    this.valores = new ValoresVO();

                    this.valores.Aceleracao = acel;
                    this.valores.Delay = delay;
                    this.valores.DesvioMedio = 0;
                    this.valores.MassaIdeal = colorante.MassaEspecifica * volume;
                    this.valores.MassaMedia = 0;
                    this.valores.PulsoHorario = pulsos;
                    this.valores.PulsoReverso = pulsoRev;
                    this.valores.Velocidade = velocidade;
                    this.valores.Volume = volume;

                   DialogResult = DialogResult.OK;
                }
                else
                {
                    using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                    {
                       m.ShowDialog("Já existe este Volume na base de dados.", Negocio.IdiomaResxExtensao.Global_Sim);
                    }
                }
            }
            else
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog("Usuário deve inserir valores nos campos.", Negocio.IdiomaResxExtensao.Global_Sim);
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void fCalibracao_Load(object sender, EventArgs e)
        {
            try
            {
                Util.ObjectParametros _parametros = Util.ObjectParametros.Load();

                colorante = Util.ObjectColorante.Load(_calibracao.Motor);
                lblNomeColorante.Text = "Colorante: " + colorante.Nome;

                bool chb_tec = _parametros.HabilitarTecladoVirtual;
                bool chb_touch = _parametros.HabilitarTouchScrenn;

                txtAceleracao.isTecladoShow = chb_tec;
                txtAceleracao.isTouchScrenn = chb_touch;

                txtDelay.isTecladoShow = chb_tec;
                txtDelay.isTouchScrenn = chb_touch;

                txtPulsos.isTecladoShow = chb_tec;
                txtPulsos.isTouchScrenn = chb_touch;

                txtVelocidade.isTecladoShow = chb_tec;
                txtVelocidade.isTouchScrenn = chb_touch;

                txtVolume.isTecladoShow = chb_tec;
                txtVolume.isTouchScrenn = chb_touch;

                txtPulsoReverso.isTecladoShow = chb_tec;
                txtPulsoReverso.isTouchScrenn = chb_touch;

                lblTitulo.Text = Negocio.IdiomaResxExtensao.fCalibracao_lblTitulo;
                lblVolume.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume;
                lblAceleracao.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblAceleracao; ;
                lblDelay.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemDelay; ;
                lblPulsos.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemPulsos;
                lblPusloRev.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemPulsosRev;
                lblVelocidade.Text = Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVelocidade;
                btnSair.Text = Negocio.IdiomaResxExtensao.PainelControle_Menu_Sair;
                btnConfirmar.Text = Negocio.IdiomaResxExtensao.Global_Confirma;
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtVolume_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double pulsos_vol = this._calibracao.Valores[0].PulsoHorario / this._calibracao.Valores[0].Volume;
                int nPulso = int.Parse(Math.Round((pulsos_vol * txtVolume.ToDouble())).ToString());
                txtPulsos.Text = nPulso.ToString();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void txtVelocidade_TextChanged(object sender, EventArgs e)
        {
            int valorVelocidade = 0;
            if (int.TryParse(txtVelocidade.Text, out valorVelocidade))
            {
                Util.ObjectParametros prametros = Util.ObjectParametros.Load();
                double AG = prametros.Aceleracao;
                double VG = prametros.Velocidade;
                double V = valorVelocidade;

                double FATOR = (VG / V) * (VG / V);
                double AC = (1 / FATOR) * VG;
                int newAceleracao = int.Parse(Math.Round(AC).ToString());

                txtAceleracao.Text = newAceleracao.ToString();
            }
            else
            {
                txtAceleracao.Text = "0";
            }
        }
    }
}