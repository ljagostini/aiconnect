using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace Percolore.Core.UserControl
{
	public enum ProgressBarDisplayStyle
    {
        Percentage,
        ValueAndMax,
        Text,
        TextAndPercentage
    }

    public partial class UProgressBar : ProgressBar
    {
        #region Propriedades

        private Orientation orientacao;
        private ProgressBarDisplayStyle displayStyle;

        public Orientation Orientacao
        {
            get { return orientacao; }
            set
            {
                /*Ao mudar orientação, inverte automaticamente os 
                 * valores de height e size*/
                int h = ClientSize.Height;
                int w = ClientSize.Width;
                ClientSize = new Size(h, w);
                orientacao = value;
            }
        }
        public Color ProgressBackColor { get; set; }
        public int BorderSize { get; set; }
        public Color BorderColor { get; set; }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Refresh();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                Refresh();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public ProgressBarDisplayStyle DisplayStyle
        {
            get { return displayStyle; }
            set
            {
                if (displayStyle == value)
                    return;

                displayStyle = value;
                Refresh();
            }
        }

        #endregion

        public UProgressBar()
        {
            this.SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.AllPaintingInWmPaint,
                true);

            //[Inicializa propriedades]
            BackColor = Color.White;
            ProgressBackColor = SystemColors.ButtonFace;
            BorderSize = 1;
            BorderColor = SystemColors.ActiveBorder;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            SolidBrush brush = null;
            int AREA_FUNDO_WIDTH = e.ClipRectangle.Width;
            int AREA_FUNDO_HEIGHT = e.ClipRectangle.Height;
            int AREA_PROGRESSO_WIDTH = 0;
            int AREA_PROGRESSO_HEIGHT = 0;
            int X = 0;
            int Y = 0;

            if (orientacao == Orientation.Horizontal)
            {
                #region [Aplica orientação horizontal]

                //Desenha progress bar vazio
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);

                //Define área de progresso
                AREA_PROGRESSO_WIDTH = (int)(AREA_FUNDO_WIDTH * ((double)base.Value / Maximum)) - (BorderSize * 2);
                AREA_PROGRESSO_HEIGHT = AREA_FUNDO_HEIGHT - (BorderSize * 2);

                //Define posicionamento
                X = BorderSize;
                Y = BorderSize;

                #endregion
            }
            else
            {
                #region [Aplica orientação vertical]

                //Desenha progress bar vazio
                ProgressBarRenderer.DrawVerticalBar(e.Graphics, e.ClipRectangle);

                //Define área de progresso
                AREA_PROGRESSO_WIDTH = AREA_FUNDO_WIDTH - (BorderSize * 2);
                AREA_PROGRESSO_HEIGHT = (int)(AREA_FUNDO_HEIGHT * ((double)base.Value / Maximum)) - (BorderSize * 2);

                //Define posicionamento
                X = BorderSize;
                Y = (AREA_FUNDO_HEIGHT - BorderSize) - AREA_PROGRESSO_HEIGHT;

                #endregion
            }

            //[Área de fundo]
            brush = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(brush, 0, 0, AREA_FUNDO_WIDTH, AREA_FUNDO_HEIGHT);

            //[Área de progresso]
            brush = new SolidBrush(ProgressBackColor);
            e.Graphics.FillRectangle(brush, X, Y, AREA_PROGRESSO_WIDTH, AREA_PROGRESSO_HEIGHT);

            if (BorderSize > 0)
            {
                #region [Borda]

                ControlPaint.DrawBorder(
                    e.Graphics, e.ClipRectangle,
                    BorderColor, BorderSize, ButtonBorderStyle.Solid,
                    BorderColor, BorderSize, ButtonBorderStyle.Solid,
                    BorderColor, BorderSize, ButtonBorderStyle.Solid,
                    BorderColor, BorderSize, ButtonBorderStyle.Solid);

                #endregion
            }

            #region Define Texto

            string newText = string.Empty;
            switch (DisplayStyle)
            {
                case ProgressBarDisplayStyle.ValueAndMax:
                    {
                        newText = string.Format("{0}/{1}", Value, Maximum);
                        break;
                    }
                case ProgressBarDisplayStyle.Percentage:
                    {
                        newText = Math.Floor(((float)Value / Maximum) * 100).ToString(CultureInfo.InvariantCulture) + '%';
                        break;
                    }
                case ProgressBarDisplayStyle.Text:
                    {
                        newText = Text;
                        break;
                    }
                case ProgressBarDisplayStyle.TextAndPercentage:
                    {
                        newText =
                            Text + ' ' + '(' + Math.Floor(((float)Value / Maximum) * 100).ToString(CultureInfo.InvariantCulture) + '%' + ')';
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(newText))
            {
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

                SizeF textSize = e.Graphics.MeasureString(newText, Font);
                brush = new SolidBrush(this.ForeColor);
                e.Graphics.DrawString(
                    newText, Font, brush, (Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
            }

            #endregion
        }
    }
}