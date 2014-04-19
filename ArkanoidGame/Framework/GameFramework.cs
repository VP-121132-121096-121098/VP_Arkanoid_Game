using ArkanoidGame.Interfaces;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGame.Framework
{
    public class GameFramework
    {
        /// <summary>
        /// Број на интервали од 100 наносекунди во една секунда
        /// </summary>
        public long SecondInWinFileTime { get { return 10000000; } }

        private bool IsExceptionRaised { get; set; }

        public long MillisecondInFileTime { get { return 10000; } }

        /// <summary>
        /// Колку слики во секунда треба да се рендерираат
        /// </summary>
        private long FPSTarget { get { return 60; } }

        private readonly int gameUpdatePeriod;

        /// <summary>
        /// Frames per second
        /// </summary>
        public long FPSCounter { get; private set; }
        public long PreviousFPSCounterValue { get; private set; }

        private long FPSLastUpdate; //се користи за броење FPS

        private IGame game; //играта која треба да се стартува


        /// <summary>
        /// Панелот на кој се црта
        /// </summary>
        private GamePanel gamePanel;

        /// <summary>
        /// Спречува повикување на StartGame повеќе од еднаш
        /// и ја терминира играта во случај кога е затворен прозорецот
        /// </summary>
        public bool IsFrameworkRunning { get; private set; }

        public bool IsGameRunning { get; set; }


        /// <summary>
        /// Го паузира исцртувањето, ажурирањето на логиката на 
        /// играта не смее да заостанува!!!
        /// </summary>
        public bool IsRendererRunning { get; private set; }

        /// <summary>
        /// Is GameFramework initialized?
        /// </summary>
        private bool Initialized { get; set; }

        public GameFramework(IGame game)
        {
            this.IsExceptionRaised = false;
            this.game = game;

            this.Initialized = false;
            this.IsFrameworkRunning = false;
            this.FPSCounter = 0;
            this.gameUpdatePeriod = 40; //25 updates per second
            this.PreviousFPSCounterValue = FPSTarget;
            this.FPSLastUpdate = 0;
            this.gamePanel = null;
            this.IsGameRunning = false;
            this.IsRendererRunning = true;
        }

        /// <summary>
        /// Се повикува од методот GamePanel.OnPaint.
        /// Graphics graphics е објектот од PaintEventArgs.Graphics аргументот од OnPaint методот.
        /// frameWidth и frameHeight се ширина и висина на панелот соодветно.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        public void Draw(Graphics graphics, int frameWidth, int frameHeight)
        {
            try
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                if (FPSLastUpdate == 0)
                    FPSLastUpdate = DateTime.Now.ToFileTimeUtc();

                if (!IsGameRunning)
                {
                    graphics.FillRectangle(Brushes.Black, 0, 0, gamePanel.Width, gamePanel.Height);
                }
                else
                {
                    while (!IsRendererRunning)
                        Thread.Sleep(1);
                    game.OnDraw(graphics, frameWidth, frameHeight);
                }

                //FPS Counter Logic
                long FPSFileTimeIntervalCounter = DateTime.Now.ToFileTimeUtc() - FPSLastUpdate;
                if (FPSFileTimeIntervalCounter > SecondInWinFileTime)
                {
                    PreviousFPSCounterValue = FPSCounter;
                    FPSFileTimeIntervalCounter -= SecondInWinFileTime;
                    FPSCounter = 0;

                    if (FPSFileTimeIntervalCounter > SecondInWinFileTime)
                    {
                        FPSFileTimeIntervalCounter = 0; //Ресетирај го бројачот, веројатно имало подолга пауза
                    }

                    if (FPSFileTimeIntervalCounter > 0 && PreviousFPSCounterValue > 0)
                    {
                        PreviousFPSCounterValue--;
                        //FPSCounter = 1;
                    }

                    FPSLastUpdate = DateTime.Now.ToFileTimeUtc();
                }

                //Display FPS
                graphics.DrawString(string.Format("{0} FPS", PreviousFPSCounterValue),
                    SystemFonts.CaptionFont, Brushes.Yellow, frameWidth * 0.01f, frameHeight * 0.01f);
                FPSCounter++;

                if (PreviousFPSCounterValue >= 100)
                    Thread.Sleep((int)(1000 / PreviousFPSCounterValue));
                else
                    Thread.Sleep(1);
                gamePanel.Refresh();
            }
            catch (Exception e)
            {
                this.IsExceptionRaised = true; //Спречи го другиот thread да повика invalidate

                MessageBox.Show(string.Format("Се случи критична грешка\nДетали за грешката:"
                    + "\nException: {0}\nMessage: {4}\nStack trace: {1}\nSource: {2}\nTarget site: {3}\n",
                    e.GetType().FullName, e.StackTrace, e.Source, e.TargetSite, e.Message), "Грешка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.IsFrameworkRunning = false;
            }
        }

        /// <summary>
        /// Иницијализација на потребните променливи. 
        /// </summary>
        /// <param name="window">Прозорецот на кој ќе се прикажува играта</param>
        private void InitializeFramework()
        {
            if (Initialized)
            {
                throw new SystemException("Attempting to initialize an already initialized framework.");
            }

            //Користи double buffering
            System.Reflection.PropertyInfo panelDoubleBuffered = typeof(GamePanel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            panelDoubleBuffered.SetValue(gamePanel, true, null);

            this.Initialized = true;
        }


        /// <summary>
        /// Ја стартува играта испратена како параметар. 
        /// Играта мора да го имплементира интерфејсот IGame.
        /// </summary>
        public void StartGame(GamePanel panel)
        {
            if (IsFrameworkRunning)
            {
                throw new SystemException("The game is already running!");
            }

            this.gamePanel = panel;
            gamePanel.GameFramework = this;
            this.InitializeFramework();

            this.IsFrameworkRunning = true;
            gamePanel.Invalidate();

            try
            {
                this.IsGameRunning = true;
                this.GameMainLoop();
            }
            catch (Exception e)
            {
                IsExceptionRaised = true;
                MessageBox.Show(string.Format("Се случи критична грешка\nДетали за грешката:"
                    + "\nException: {0}\nMessage: {4}\nStack trace: {1}\nSource: {2}\nTarget site: {3}\n",
                    e.GetType().FullName, e.StackTrace, e.Source, e.TargetSite, e.Message), "Грешка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.IsGameRunning = false;
                this.IsFrameworkRunning = false;
            }
        }

        private void GameMainLoop()
        {

            while (true)
            {
                if (IsExceptionRaised) /* race condition. Нема потреба од синхронизација, после   *
                                        * прикажувањето на пораката за грешка играта се исклучува */
                {
                    Thread.Sleep(gameUpdatePeriod);
                    if (!this.IsFrameworkRunning || !this.IsGameRunning)
                        break;
                    continue;
                }

                long timeUpdateBegin = DateTime.Now.ToFileTimeUtc() / MillisecondInFileTime;
                game.OnUpdate();

                long timeUpdateEnd = DateTime.Now.ToFileTimeUtc() / MillisecondInFileTime;
                int remainingTime = (int)(timeUpdateEnd - timeUpdateBegin);
                if (remainingTime > 0 && remainingTime < gameUpdatePeriod) 
                {
                    IsRendererRunning = true;
                    Thread.Sleep(remainingTime);
                }
                else
                {
                    IsRendererRunning = false;
                }

                if (!this.IsFrameworkRunning || !this.IsGameRunning)
                    break;
            }
        }

        /// <summary>
        /// Функција со која се исклучува играта и целиот frameWork
        /// </summary>
        public void TerminateGame()
        {
            this.IsFrameworkRunning = false;
        }
    }
}
