using ArkanoidGame;
using ArkanoidGame.Framework;
using ArkanoidGame.Renderer;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
            //int gameUpdatePeriod = 16; //~60 FPS
            int gameUpdatePeriod = 9; //debugging;
            window.StartGameFramework(new GameFramework(GameArkanoid.GetInstance(), gameUpdatePeriod));
        }

        /// <summary>
        /// Креирај фолдер наменет за привремено зачувување на ресурси
        /// креирани во за време на извршување на играта
        /// </summary>
        static void CreateCacheDirectory()
        {
            DirectoryInfo cacheDirectory = new DirectoryInfo(
                string.Format("{0}\\Resources\\Cache", System.Environment.CurrentDirectory));
            cacheDirectory.Create();
            TextWriter writer = new StreamWriter(
                string.Format("{0}\\Resources\\Cache\\DO_NOT_DELETE.txt", System.Environment.CurrentDirectory));
            writer.WriteLine("This folder is needed for storing temporary images."
                + "\nPetar Kjimov");
            writer.Close();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CreateCacheDirectory();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Настан кој ќе се повика пред да терминира процесот
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            DialogResult result = MessageBox.Show("Дали сакате играта да се стартува на целиот екран (fullscreen)?",
                "Fullscreen мод?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            bool fullScreen = result == DialogResult.Yes;

            GameWindow window = new GameWindow(fullScreen);
            Thread windowThread = new Thread(() => Program.StartGameWindow(window));
            windowThread.Start();

            Thread gameThread = new Thread(() => Program.StartNewGame(window));
            gameThread.Start();
        }

        /// <summary>
        /// Бришење на фолдерот Resources\Cache пред да терминира процесот.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnProcessExit(object sender, EventArgs e)
        {
            DirectoryInfo cacheDirectory = new DirectoryInfo(
                string.Format("{0}\\Resources\\Cache", System.Environment.CurrentDirectory));
            try
            {
                cacheDirectory.Delete(true); //пробај да го избришеш фолдерот и сите фајлови во него
            }
            catch(Exception) { }
        }
    }
}
