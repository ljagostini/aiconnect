namespace Percolore.Treinamento
{
	public partial class frmRecipiente : Form
    {
        List<Form> listaExibicao = new List<Form>();
        int indexTela = 0;

        public frmRecipiente()
        {
            InitializeComponent();

#if DEBUG
            this.TopMost = false;
#endif

            #region Resource UI

            lblTitulo.Text = Negocio.IdiomaResx.UI_Titulo;
            btnSair.Text = string.Empty;
            btnSair.Image = Properties.Treinamento_Imagem.UI_Sair;
            btnVoltar.Text = string.Empty;
            btnVoltar.Image = Properties.Treinamento_Imagem.UI_Voltar;
            btnAvancar.Text = string.Empty;
            btnAvancar.Image = Properties.Treinamento_Imagem.UI_Avancar;

            #endregion

            #region Conteúdo

            //Abertura
            Conteudo contAbertura = new Conteudo();
            contAbertura.Texto = Negocio.IdiomaResx.Abertura_Texto;
            contAbertura.Etapa = Negocio.IdiomaResx.Abertura_Titulo;
            contAbertura.Imagens.Add(Properties.Treinamento_Imagem.Logotipo_447x109);

            //Purga
            Conteudo contPurga = new Conteudo();
            contPurga.Etapa = Negocio.IdiomaResx.Purga_001_Titulo;
            contPurga.Texto = Negocio.IdiomaResx.Purga_001_Texto;
            contPurga.Imagens.Add(Properties.Treinamento_Imagem.Purga_1);

            //Abastecimento 01
            Conteudo contAbastecimento = new Conteudo();
            contAbastecimento.Etapa = Negocio.IdiomaResx.Abastecimento_001_Titulo;
            contAbastecimento.Texto = Negocio.IdiomaResx.Abastecimento_001_Texto;
            contAbastecimento.Imagens.Add(Properties.Treinamento_Imagem.Abastecimento_1);
            contAbastecimento.Imagens.Add(Properties.Treinamento_Imagem.Abastecimento_2);

            //Abastecimento 02
            Conteudo contAbastecimento02 = new Conteudo();
            contAbastecimento02.Etapa = Negocio.IdiomaResx.Abastecimento_002_Titulo;
            contAbastecimento02.Texto = Negocio.IdiomaResx.Abastecimento_002_Texto;
            contAbastecimento02.Imagens.Add(Properties.Treinamento_Imagem.Abastecimento_3);
            contAbastecimento02.Imagens.Add(Properties.Treinamento_Imagem.Abastecimento_4);

            //Abastecimento_Registro
            Conteudo contAbastecimento_Registro = new Conteudo();
            contAbastecimento_Registro.Etapa = Negocio.IdiomaResx.RegistroAbastecimento_001_Titulo;
            contAbastecimento_Registro.Texto = Negocio.IdiomaResx.RegistroAbastecimento_001_Texto;
            contAbastecimento_Registro.Imagens.Add(Properties.Treinamento_Imagem.Abastecimento_Registro_1);

            //Dosagem
            Conteudo contDosagem = new Conteudo();
            contDosagem.Etapa = Negocio.IdiomaResx.Dosagem_001_Titulo;
            contDosagem.Texto = Negocio.IdiomaResx.Dosagem_001_Texto;
            contDosagem.Imagens.Add(Properties.Treinamento_Imagem.Dosagem_1);

            //Cuidados báscios 01
            Conteudo contCuidadosBasicos01 = new Conteudo();
            contCuidadosBasicos01.Etapa = Negocio.IdiomaResx.CuidadosBasicos_001_Titulo;
            contCuidadosBasicos01.Texto = Negocio.IdiomaResx.CuidadosBasicos_001_Texto;

            //Cuidados básicos 02
            Conteudo contCuidadosBasicos02 = new Conteudo();
            contCuidadosBasicos02.Etapa = Negocio.IdiomaResx.CuidadosBasicos_002_Titulo;
            contCuidadosBasicos02.Texto = Negocio.IdiomaResx.CuidadosBasicos_002_Texto;

            //Cuidados básicos 02
            Conteudo contCuidadosBasicos03 = new Conteudo();
            contCuidadosBasicos03.Etapa = Negocio.IdiomaResx.CuidadosBasicos_003_Titulo;
            contCuidadosBasicos03.Texto = Negocio.IdiomaResx.CuidadosBasicos_003_Texto;

            //Encerramento
            Conteudo contEncerramento = new Conteudo();
            contEncerramento.Texto = Negocio.IdiomaResx.Encerramento_001_Texto;
            contEncerramento.Etapa = Negocio.IdiomaResx.Encerramento_001_Titulo;
            contEncerramento.Imagens.Add(Properties.Treinamento_Imagem.Logotipo_447x109);

            #endregion

            #region Lista de exibição

            listaExibicao.Add(new frmModelo02(contAbertura, true, false));
            listaExibicao.Add(new frmModelo01(contPurga));
            listaExibicao.Add(new frmModelo01(contAbastecimento));
            listaExibicao.Add(new frmModelo01(contAbastecimento02));
            listaExibicao.Add(new frmModelo01(contAbastecimento_Registro));
            listaExibicao.Add(new frmModelo01(contDosagem)); 
            listaExibicao.Add(new frmModelo01(contCuidadosBasicos01));
            listaExibicao.Add(new frmModelo01(contCuidadosBasicos02));
            listaExibicao.Add(new frmModelo01(contCuidadosBasicos03));
            listaExibicao.Add(new frmModelo01(contEncerramento, true, false));

            #endregion
        }

        #region Eventos   

        private void frmInstalador_Load(object sender, EventArgs e)
        {
            ShowTela(indexTela);
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            Voltar();
        }

        private void btnAvancar_Click(object sender, EventArgs e)
        {
            Avancar();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.listaExibicao = null;
            this.Close();
        }

        private void btnConcluir_Click(object sender, EventArgs e)
        {
            this.Close();
            this.listaExibicao = null;
        }

        #endregion

        #region Métodos

        private void ShowTela(int index)
        {
            //Primeira tela
            if (this.indexTela == 0)
                btnVoltar.Enabled = false;
            else
                btnVoltar.Enabled = true;

            //Última tela
            if (this.indexTela == (this.listaExibicao.Count - 1))
                btnAvancar.Enabled = false;
            else
                btnAvancar.Enabled = true;

            Form f = listaExibicao[index];
            pnlConteudo.Controls.Clear();
            f.TopLevel = false;
            f.Width = pnlConteudo.Width;
            f.Height = pnlConteudo.Height;
            pnlConteudo.Controls.Add(f);

            f.Show();
        }

        public void Avancar()
        {
            this.indexTela++;
            ShowTela(this.indexTela);

            ////Não permite ultrapassar limite de índices
            ////if (this._indexForm < (this._listForm.Count - 1))
            //if (!this.ultimaTela)
            //{
            //    this.indexTela++;
            //    ShowTela(this.indexTela);
            //}
            //else
            //{
            //    bool t = true;


        }

        public void Voltar()
        {
            if (this.indexTela > 0)
            {
                this.indexTela--;
                ShowTela(this.indexTela);
            }
        }

        public void DesabilitarAvancar()
        {
            this.btnAvancar.Enabled = false;
        }

        #endregion
    }
}