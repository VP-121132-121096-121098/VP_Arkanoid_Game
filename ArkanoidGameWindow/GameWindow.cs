
namespace ArkanoidGameWindow
{
    public partial class GameWindow : ArkanoidGame.AbstractGameWindow
    {
        public GameWindow(bool fullScreen)
        {
            InitializeComponent();
            if (fullScreen)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
        }

        public override ArkanoidGame.GamePanel GamePanel
        {
            get
            {
                return this.gamePanel;
            }
            protected set
            {
                this.GamePanel = value;
            }
        }
    }
}
