using Percolore.Core.Logging;
using Percolore.IOConnect.Util;

namespace Percolore.IOConnect
{
	public partial class fTreinamentoCalibracao : Form
    {
        int index_Tela = 0;
        public event CloseWindows OnClosedEvent = null;
        public fTreinamentoCalibracao()
        {
            InitializeComponent();
           
        }

        private void fTreinamentoCalibracao_Load(object sender, EventArgs e)
        {
            try
            {
                this.index_Tela = 1;
                lblTitulo.Text = Negocio.IdiomaResxExtensao.frmTreinCal_lblTitulo;
                btnSair.Text = Negocio.IdiomaResxExtensao.frmTreinCal_btnSair;
                btnAvancar.Text = Negocio.IdiomaResxExtensao.frmTreinCal_btnAvancar;
                btnVoltar.Text = Negocio.IdiomaResxExtensao.frmTreinCal_btnRecuar;
                
                ShowTela();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void fTreinamentoCalibracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.OnClosedEvent != null)
                {
                    this.OnClosedEvent();
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnSair_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            if(this.index_Tela > 1)
            {
                index_Tela--;
            }
            ShowTela();
        }

        private void btnAvancar_Click(object sender, EventArgs e)
        {
            if (this.index_Tela >= 8)
            {
                btnSair.Enabled = true;
            }
            else
            {
                index_Tela++;
            }
            ShowTela();
        }

        private void ShowTela()
        {
            try
            {
                switch(this.index_Tela)
                {
                    case 1:
                        {
                            //lblMessage.Text = "Realizar a tara do recipiente na balança";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_01;
                            break;
                        }
                    case 2:
                        {
                            //lblMessage.Text = "Posicionar o recipiente no bico dosador";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_02;
                            break;
                        }
                    case 3:
                        {
                            //lblMessage.Text = "Dispensar o Produto";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_03;
                            break;
                        }
                    case 4:
                        {
                            //lblMessage.Text = "Realizar a leitura do peso do recipiente na balança";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_04;
                            break;
                        }
                    case 5:
                        {
                            //lblMessage.Text = "Adicionar o peso do recipiente no sistema";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_05;
                            break;
                        }
                    case 6:
                        {
                            //lblMessage.Text = "Despejar o produto do recipiente no cânister";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_06;
                            break;
                        }
                    case 7:
                        {
                            //lblMessage.Text = "Realizar a tara do recipiente na balança";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_07;
                            break;
                        }
                    case 8:
                        {
                            //lblMessage.Text = "Repetir todo o processo até chegar há um percentual aceitável";
                            lblMessage.Text = Negocio.IdiomaResxExtensao.frmTreinCal_Message_08;
                            btnSair.Enabled = true;
                            break;
                        }                        
                    default:
                        {
                            btnSair.Enabled = true;
                            break;
                        }
                }
            }
			catch (Exception ex)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", ex);
			}
		}       
    }
}