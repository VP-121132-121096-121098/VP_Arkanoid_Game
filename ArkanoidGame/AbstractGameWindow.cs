using ArkanoidGame.Framework;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGame
{

    /// <summary>
    /// Не е апстрактна класа поради дизајнерот на Visual Sutdio, но треба да биде
    /// </summary>
    public partial class AbstractGameWindow : Form
    {
        private GameFramework gameFramework;
        private bool formShown;

        public AbstractGameWindow()
        {
            InitializeComponent();
            formShown = false;
            gameFramework = null;
        }

        /// <summary>
        /// Панела на која ќе се исцртува играта
        /// </summary>
        public virtual GamePanel GamePanel
        {
            get { throw new NotImplementedException(); }
            protected set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Стартувај ја играта
        /// </summary>
        /// <param name="framework"></param>
        public virtual void StartGameFramework(GameFramework framework)
        {
            this.gameFramework = framework;
            while (!formShown)
                Thread.Sleep(10);
            gameFramework.StartGame(this.GamePanel);
            this.CloseAsync();
        }


        /// <summary>
        /// Close во cross-thread верзија
        /// </summary>
        public virtual void CloseAsync()
        {
            try
            {
                {
                    if (!InvokeRequired)
                    {
                        this.Close();
                        return;
                    }

                    this.Invoke(new Action(() => { this.Close(); }));
                }
            }
            catch (ObjectDisposedException)
            {
                return; /* Претходно бил повикан Dispose() методот.
                         * Не прави ништо во овој случај, формата е веќе затворена */
            }
            catch (Exception)
            {
                if (!this.IsDisposed)
                {
                    this.CloseAsync();
                }
            }
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Form.Shown event.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnShown(EventArgs e)
        {
            this.formShown = true;
            base.OnShown(e);
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Form.Closing event.
        /// </summary>
        /// <param name="e">A System.ComponentModel.CancelEventArgs
        /// that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            gameFramework.TerminateGame();
            base.OnClosing(e);
        }
    }
}
