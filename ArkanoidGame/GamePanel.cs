using ArkanoidGame.Framework;
using System.Drawing;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public class GamePanel : Panel
    {
        public GamePanel() { }

        public GameFramework GameFramework { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            if (GameFramework == null)
            {
                //base.OnPaint(e);
                e.Graphics.FillRectangle(Brushes.Black, 0, 0, Width, Height);
            }
            else
            {
                GameFramework.Draw(e.Graphics, this.Width, this.Height);
            }
        }
    }
}
