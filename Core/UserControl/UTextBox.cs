using Percolore.Core.Util;
using Percolore.Core.Windows;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Percolore.Core.UserControl
{
	public partial class UTextBox : TextBox
    {
        NumberFormatInfo _numberFormatInfo;
        public TipoConteudo Conteudo { get; set; }
        public Color BorderColor { get; set; }

        private bool _habilitarTecladoVirtual = true;
        /// <summary>
        /// Habilita ou desabilita o teclado virtual.
        /// </summary>
        public bool HabilitarTecladoVirtual
        {
            get { return _habilitarTecladoVirtual; }
            set { _habilitarTecladoVirtual = value; }
        }

        public bool isTecladoShow = true;
        public bool isTouchScrenn = false;
        public bool isOpenTeclado = false;

        public UTextBox()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);
            _numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;

            Conteudo = TipoConteudo.Padrao;
            ForeColor = Color.FromArgb(120, 120, 120);
            BorderColor = Color.Gainsboro;
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (ReadOnly == true)
            {
                return;
            }

            base.OnKeyPress(e);

            switch (Conteudo)
            {
                case TipoConteudo.Decimal:
                    {
                        if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 &&
                            (e.KeyChar.ToString() != _numberFormatInfo.NumberDecimalSeparator))
                        {
                            e.Handled = true;
                        }

                        break;
                    }
                case TipoConteudo.Inteiro:
                    {
                        if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
                        {
                            e.Handled = true;
                        }

                        break;
                    }
                case TipoConteudo.Percentual:
                    {
                        if (!Char.IsDigit(e.KeyChar) &&
                            e.KeyChar != (char)8 &&
                            (e.KeyChar.ToString() != _numberFormatInfo.PercentDecimalSeparator))
                        {
                            e.Handled = true;
                        }

                        break;
                    }
                case TipoConteudo.Monetario:
                    {
                        if (!Char.IsDigit(e.KeyChar) &&
                            e.KeyChar != (char)8 &&
                            (e.KeyChar.ToString() != _numberFormatInfo.CurrencyDecimalSeparator))
                        {
                            e.Handled = true;
                        }

                        break;
                    }
                default:
                    break;
            }

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case (int)WindowMessage.WM_PAINT:
                    {
                        Control control = Control.FromHandle(Handle);
                        Graphics g = Graphics.FromHwnd(Handle);

                        //BORDER
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

            if (ReadOnly == true)
            {
                return;
            }
            if (this.isTecladoShow)
            {
                this.isOpenTeclado = KeyboardHelper.IsOpen();

				if (!_habilitarTecladoVirtual)
					return;

				if (UseSystemPasswordChar || PasswordChar != '\0')
                //if (UseSystemPasswordChar || (PasswordChar != '\0' && PasswordChar != '●'))
                //if ((PasswordChar != '\0' && PasswordChar != '●'))
                {
                    /* 27.06.2017 - Referente à tarefa 138
                     * No Windows 10, quando uma caixa de texto com PasswordChar habilitado 
                     * recebe uma mensagem de toque, o teclado virtual é automaticamente 
                     * exibido pelo SO.
                     * O mesmo não acontece com entrada do mouse.
                     * 
                     * A solução encontrada foi, apenas neste cenário,  anular a ação do SO 
                     * encerrando o processo do teclado. */
                    if (WMI.GetOSBuildVersion().Major == 10)
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

        public byte ToByte()
        {
            byte outByte = 0;
            byte.TryParse(Text, out outByte);
            return outByte;
        }

        public int ToInt()
        {
            int outInt = 0;
            int.TryParse(Text, out outInt);
            return outInt;
        }

        public double ToDouble()
        {
            double outDouble = 0;
            double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out outDouble);
            return outDouble;
        }

        public decimal ToDecimal()
        {
            decimal saida = 0;

            if (Conteudo == TipoConteudo.Monetario)
            {
                NumberStyles style =
                    NumberStyles.Number | NumberStyles.AllowCurrencySymbol;

                Decimal.TryParse(this.Text, style, Application.CurrentCulture, out saida);
            }
            else
            {
                decimal.TryParse(this.Text, out saida);
            }

            return saida;
        }
    }
}