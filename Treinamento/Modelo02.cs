namespace Percolore.Treinamento
{
	public partial class frmModelo02 : Form
    {
        bool centralizarConteudo = false;
        bool exibirMolduraImagens = true;
        Conteudo conteudo = null;

        public frmModelo02(Conteudo conteudo)
        {
            this.conteudo = conteudo;
            Inicializar();
        }

        public frmModelo02(
            Conteudo conteudo, bool centralizarConteudo, bool exibirMolduraImagens)
        {
            this.conteudo = conteudo;
            this.centralizarConteudo = centralizarConteudo;
            this.exibirMolduraImagens = exibirMolduraImagens;
            Inicializar();
        }

        void Inicializar()
        {
            InitializeComponent();

            if (this.centralizarConteudo)
            {
                lblTitulo.TextAlign = ContentAlignment.TopCenter;
                lblTexto.TextAlign = ContentAlignment.TopCenter;
            }

            lblTitulo.Text = this.conteudo.Etapa;
            lblTexto.Text = this.conteudo.Texto;

            int SCREEN_WIDTH = Screen.PrimaryScreen.Bounds.Width;
            int SCREEN_HEIGHT = Screen.PrimaryScreen.Bounds.Height;

            flowLayoutPanel.MaximumSize = new Size(SCREEN_WIDTH, SCREEN_HEIGHT);

            if (this.conteudo.Imagens.Count > 0)
            {
                #region Adiciona imagens dinâmicamente

                const int MARGEM_PADRAO = 15;

                int PADDING_PADRAO = 0;
                if (this.exibirMolduraImagens)
                    PADDING_PADRAO = 5;

                int SOMA_MARGEM_PADRAO = this.conteudo.Imagens.Count * MARGEM_PADRAO;
                int SOMA_PADDING_PADRAO = this.conteudo.Imagens.Count * PADDING_PADRAO;
                int SOMA_WIDTH_IMAGENS = 0;

                foreach (Image imagem in this.conteudo.Imagens)
                {
                    PictureBox pct = new PictureBox();
                    pct.Margin = new Padding(MARGEM_PADRAO);
                    pct.Padding = new Padding(PADDING_PADRAO);

                    if (this.exibirMolduraImagens)
                        pct.BackColor = Color.White;
                    else
                        pct.BackColor = this.BackColor;

                    pct.SizeMode = PictureBoxSizeMode.AutoSize;
                    pct.Image = imagem;
                    flowLayoutPanel.Controls.Add(pct);

                    SOMA_WIDTH_IMAGENS += imagem.Width;
                }

                int MARGEM_PANEL =
                    (SCREEN_WIDTH - (SOMA_WIDTH_IMAGENS + SOMA_MARGEM_PADRAO + SOMA_PADDING_PADRAO)) / 2;
                flowLayoutPanel.Margin = new Padding(MARGEM_PANEL, 0, 0, 0);

                #endregion
            }
        }
    }
}