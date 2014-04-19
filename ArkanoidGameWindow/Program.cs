using ArkanoidGame;
using ArkanoidGame.Framework;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGameWindow
{
    static class Program
    {
        private static void StartGameWindow(GameWindow window)
        {
            Application.Run(window);
        }

        private static void StartNewGame(GameWindow window)
        {
            int gameUpdatePeriod = 16; //60 FPS
            //int gameUpdatePeriod = 9; //debugging;
            GameArkanoid game = new GameArkanoid(null, gameUpdatePeriod);
            game.GameState = new ArkanoidStateMainMenu(game);
            window.StartGameFramework(new GameFramework(game, gameUpdatePeriod));
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DialogResult result = MessageBox.Show("Дали сакате играта да се стартува на целиот екран (fullscreen)?",
                "Fullscreen мод?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            bool fullScreen = result == DialogResult.Yes;

            GameWindow window = new GameWindow(fullScreen);
            Thread windowThread = new Thread(() => Program.StartGameWindow(window));
            windowThread.Start();

            Thread gameThread = new Thread(() => Program.StartNewGame(window));
            gameThread.Start();
        }
    }
}
