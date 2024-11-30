using Percolore.Core.Util;
using Percolore.Core.Windows;
using System.Drawing;
using System.Windows.Forms;

namespace Percolore.Core.UserControl
{
	public partial class UMaskedTextBox : MaskedTextBox
    {
        public Color BorderColor { get; set; }
        public bool isTecladoShow = true;

        public bool isTouchScrenn = false;
        public bool isOpenTeclado = false;

		private bool _habilitarTecladoVirtual = true;
		/// <summary>
		/// Habilita ou desabilita o teclado virtual.
		/// </summary>
		public bool HabilitarTecladoVirtual
		{
			get { return _habilitarTecladoVirtual; }
			set { _habilitarTecladoVirtual = value; }
		}

		bool isWin10 = false;
        public UMaskedTextBox()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint,
                true);

            BackColor = SystemColors.Window;
            BorderStyle = BorderStyle.FixedSingle;
            BorderColor = SystemColors.ActiveBorder;

            isWin10 = (WMI.GetOSBuildVersion().Major == 10);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_PAINT = 0x000F;
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_PAINT:
                    {
                        Control control = Control.FromHandle(Handle);
                        Graphics g = Graphics.FromHwnd(Handle);
                        ControlPaint.DrawBorder(
                            g, control.ClientRectangle, BorderColor, ButtonBorderStyle.Solid);

                        g.Dispose();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (ReadOnly)
            {
                return;
            }
            if (isTecladoShow)
            {
                this.isOpenTeclado = KeyboardHelper.IsOpen();

				if (!_habilitarTecladoVirtual)
					return;

				if (UseSystemPasswordChar || PasswordChar != '\0')
                {
                    /* 27.06.2017 - Referente à tarefa 138
                     * No Windows 10, quando uma caixa de texto com PasswordChar habilitado 
                     * recebe uma mensagem de toque, o teclado virtual é automaticamente 
                     * exibido pelo SO.
                     * O mesmo não acontece com entrada do mouse.
                     * 
                     * A solução encontrada foi, apenas neste cenário, anular a ação do SO 
                     * encerrando o processo do teclado. */
                    if (isWin10)
                        KeyboardHelper.Kill();
                }

                /* O teclado virtual é chamado manualmente no Windows 7, 8 e 10 */
                Focus();
                if (!this.isTouchScrenn || !this.isOpenTeclado)
                {
                    KeyboardHelper.Show();
                }
            }
        }

        //protected override void OnEnabledChanged(EventArgs e)
        //{
        //    //Comentado para que não alterasse a cor da fonte
        //    base.OnEnabledChanged(e);

        //Enabled = this.Enabled;

        //if (Enabled)
        //{
        //    BackColor = _tom01;
        //    _borderColor = _tom01;
        //}
        //else
        //{
        //    BackColor = _tom02;
        //    _borderColor = _tom02;
        //}           
        // }
        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            BackColor = SystemColors.Window;
            this.Invalidate();
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            BackColor = SystemColors.Window;
            this.Invalidate();
        }

        public string ToTextNoFormatting()
        {
            TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            string _text = Text;
            TextMaskFormat = MaskFormat.IncludePromptAndLiterals;

            return _text;
        }

        public decimal ToDecimal()
        {
            TextMaskFormat = MaskFormat.IncludeLiterals;
            string _text = Text;
            TextMaskFormat = MaskFormat.IncludePromptAndLiterals;

            decimal valor;
            decimal.TryParse(_text, out valor);

            return valor;
        }
    }
}