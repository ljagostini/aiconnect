using Percolore.Core.Util;
using Percolore.Core.Windows;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Percolore.Core.UserControl
{
	public partial class UComboBox : ComboBox
    {

        private Brush ArrowBrush = new SolidBrush(SystemColors.ControlText);
        private Brush DropButtonBrush = new SolidBrush(Color.Purple);

        private Color _borderColor = Color.FromArgb(251, 115, 15);
        private Brush _borderColorBrush = Brushes.Black;

        //private ButtonBorderStyle _borderStyle = ButtonBorderStyle.Solid;
        private Color _ButtonColor = Color.Yellow;
        private int _borderSize = 1;

		private bool _habilitarTecladoVirtual = true;
		/// <summary>
		/// Habilita ou desabilita o teclado virtual.
		/// </summary>
		public bool HabilitarTecladoVirtual
		{
			get { return _habilitarTecladoVirtual; }
			set { _habilitarTecladoVirtual = value; }
		}

		[Category("Appearance")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                _borderColorBrush = new SolidBrush(value);
                Invalidate(); // causes control to be redrawn
            }
        }

        [Category("Appearance")]
        [Description("Tamanho da borda")]
        public int BorderSize
        {
            get { return _borderSize; }
            set
            {
                _borderSize = value;
                this.Invalidate();
            }
        }

        public UComboBox()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);

            _borderColor = Color.Gainsboro;
            ForeColor = Color.FromArgb(120, 120, 120);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            const int WM_PAINT = 0x000F;

            switch (m.Msg)
            {
                case WM_PAINT:
                    {
                        Graphics g = this.CreateGraphics();

                        //Desenha borda em toda a extensão do controle
                        ControlPaint.DrawBorder(g, this.ClientRectangle,
                            _borderColor, _borderSize, ButtonBorderStyle.Solid,
                            _borderColor, _borderSize, ButtonBorderStyle.Solid,
                            _borderColor, _borderSize, ButtonBorderStyle.Solid,
                            _borderColor, _borderSize, ButtonBorderStyle.Solid);

                        g.Dispose();
                        break;
                    }
                default:
                    break;
            }
        }
        
        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);

            /* 27.06.2017 - Referente à tarefa 138
             * Adicionado novo comportamento: Quando uma ComboBox editável perde o foco, 
             * o teclado virtual é automaticamente encerrado */
            if (DropDownStyle == ComboBoxStyle.DropDown || DropDownStyle == ComboBoxStyle.Simple)
            {
                KeyboardHelper.Kill();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (DropDownStyle == ComboBoxStyle.DropDown || DropDownStyle == ComboBoxStyle.Simple)
            {
                if (!this.DroppedDown)
                {
					/* 27.06.2017 - Referente à tarefa 138
                     * No Windows 10, o teclado virtual é chamado em apenas um cenário:
                     * Quando uma ComboBox com estilo editável habilitado recebe entrada de toque.
                     * EM todos os outros cenários e versões do Windows o teclado tem de ser chamado
                     * manualmente.
                     * 
                     * A solução encontrada foi, apenas neste cenário, anular a ação do SO 
                     * encerrando o processo do teclado manualmente. 
                     * 
                     * O teclado virtual não deve ser chamado enquanto a caixa de listagem
                     * estiver aberta pois um clique será gerado, selecionando automaticamente 
                     * o primeiro item e fechando a mesma. */

					if (!_habilitarTecladoVirtual)
						return;

					if (WMI.GetOSBuildVersion().Major == 10)
                        KeyboardHelper.Kill();

					/* O teclado virtual é chamado manualmente no Windows 7, 8 e 10 */
					KeyboardHelper.Show();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}