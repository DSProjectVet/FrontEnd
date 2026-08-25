using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public class GradientPanel : Panel
    {
        public Color gradientTop { get; set; }
        public Color gradientBottom { get; set; }

        public GradientPanel()
        {
            this.Resize += GradientPanel_Resize;
        }

        private void GradientPanel_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(this.ClientRectangle, gradientTop, gradientBottom, 90F))
            {
                LinearGradientBrush linear = new LinearGradientBrush(

                    this.ClientRectangle, 
                    this.gradientTop,
                    this.gradientBottom,
                    90F

                    );

                Graphics g = e.Graphics;

                g.FillRectangle(linear, this.ClientRectangle);

                base.OnPaint(e);
            }
        }
    }
}
