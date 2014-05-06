using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ArkanoidGame
{


    public class GameArkanoid : IGame
    {
        //Komentar za proba
        //

        //Стартна позиција на играчот (1750, 2010);

        /// <summary>
        /// Дали update ќе се извршува на една или повеќе нишки
        /// </summary>
        public bool IsMultithreadingEnabled { get; private set; }

        private int ButtonMWaitNFrames;

        /// <summary>
        /// Вклучи или исклучи во зависност од тоа дали е пристисната буквата M
        /// </summary>
        private void EnableOrDisableMultithreading()
        {
            if (ButtonMWaitNFrames > 0)
                ButtonMWaitNFrames--;

            IKeyState keyState = KeyStateInfo.GetAsyncKeyState(Keys.M);
            if (keyState.IsPressed && ButtonMWaitNFrames == 0)
            {
                IsMultithreadingEnabled = !IsMultithreadingEnabled;
                ButtonMWaitNFrames = 10;
            }
        }

        private readonly object gameStateLock = new Object();

        public bool IsRendererEnabled { get; set; }

        public string Name { get; private set; }

        /// <summary>
        /// Позиција на курсорот изразена како координати од играта,
        /// не од прозорецот на играта.
        /// </summary>
        public Point CursorIngameCoordinates { get; private set; }

        public IGameState GameState { get; set; }

        public IList<IGameObject> GameObjects { get; private set; }

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            if (!IsRendererEnabled)
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, frameWidth, frameHeight);
                return;
            }

            this.GameState.OnDraw(graphics, frameWidth, frameHeight, 
                GraphicsDetails == Interfaces.GraphicsDetails.Low);

            if (IsMultithreadingEnabled)
                Renderer.Render(textMultithreading, graphics, frameWidth, frameHeight,
                    false);
        }

        //во милисекунди
        public int GameUpdatePeriod { get; set; }

        private GameBitmap textMultithreading;

        public void ReloadResources()
        {
            Bitmap textMT = StaticStringFactory.CreateOrangeString("Multithreading");
            textMultithreading = new GameBitmap(textMT, VirtualGameWidth - 400 - 10, 5, 400,
                60);
        }

        /// <summary>
        /// Креира нова игра и ја поставува во почетна состојба ArkanoidStateMainMenu.
        /// </summary>
        private GameArkanoid()
        {
            this.ButtonMWaitNFrames = 0;
            this.IsMultithreadingEnabled = false;
            IsRendererEnabled = false;

            graphicsDetails = Interfaces.GraphicsDetails.Low;
            RendererCache.PreferQualityOverPerformance = false;

            VirtualGameWidth = 3840;
            VirtualGameHeight = 2160;

            this.Renderer = new GameRenderer(VirtualGameWidth, VirtualGameHeight);
            CursorIngameCoordinates = Cursor.Position;
            this.GameUpdatePeriod = 0;
            GameState = new ArkanoidMainMenuState(this);
            Name = "Arkanoid";
            GameObjects = new List<IGameObject>();

            ReloadResources();

            IsRendererEnabled = true;
        }

        static GameArkanoid()
        {
            instance = null;
            lockObject = new object();
        }

        private static GameArkanoid instance;
        private static readonly object lockObject;

        public static GameArkanoid GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                        instance = new GameArkanoid();
                }
            }

            return instance;
        }

        public int OnUpdate(Point cursorPanelCoordinates)
        {
            EnableOrDisableMultithreading();

            this.CursorIngameCoordinates = Renderer.ToGameCoordinates(cursorPanelCoordinates);

            int returnValue = GameState.OnUpdate(GameObjects);

            IsTimesynchronizationImportant = GameState.IsTimesynchronizationImportant;

            return returnValue;
        }


        /* Играта ќе има посебни единици за должина, посебни просторот за цртање.
         * Играта е правоаголник со димензии 3840 x 2160 кои се преведуваат во 
         * координати за прикажување на екран во зависност од димензиите на прозорецот.
         */
        public int VirtualGameWidth { get; private set; }

        public int VirtualGameHeight { get; private set; }

        /// <summary>
        /// Пример во главното мени не е важно дали ќе задоцни времето во играта. Пример корисникот
        /// отворил нова форма од главното мени, но формата е отворена од методот update, па 
        /// целото време во кое што е отворена формата ќе се смета за еден период. Со ова
        /// property му кажуваме на главниот loop дека нема потреба да повикува update 10000 
        /// пати (ќе има голем лаг во овој случај) бидејќи во спротивно сликата ќе биде
        /// замрзната подолго време. Од друга страна додека се игра многу е важно
        /// времето да биде синхронизирано, инаку ќе дојде до различна брзина на анимациите
        /// на различни компјутери. Ако хардверот не може да ги изврши сите пресметки
        /// во дадениот период тогаш не се рендерира и ќе дојде до секцање во играта,
        /// но бројот на поминати периоди најверојатно ќе остане ист.
        /// </summary>
        public bool IsTimesynchronizationImportant { get; private set; }


        public IGameRenderer Renderer { get; private set; }


        public bool IsControllerMouse { get; set; }

        public GraphicsDetails GraphicsDetails
        {
            get
            {
                return graphicsDetails;
            }
            set
            {
                if (value == Interfaces.GraphicsDetails.VeryHigh)
                {
                    RendererCache.PreferQualityOverPerformance = true;
                    this.graphicsDetails = value;
                }
                else if (value == Interfaces.GraphicsDetails.High)
                {
                    RendererCache.PreferQualityOverPerformance = false;
                    this.graphicsDetails = value;
                }
                else
                {
                    RendererCache.PreferQualityOverPerformance = false;
                    this.graphicsDetails = value;
                }
            }
        }

        private GraphicsDetails graphicsDetails;
    }
}
