using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Objects;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public class ArkanoidStateMainMenu : IGameState
    {

        private IDictionary<string, GameBitmap> menuOptions; //опции во менито
        private IDictionary<string, GameBitmap> readyStrings; //стрингови

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; } //слики од позадината и сите опции заедно
        private IList<IList<GameBitmap>> rendererBitmaps; /* на рендерерот се праќа копија од листа
                                                           * за да се избегнат проблемите со нејзино модифицирање
                                                           */

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            Game.Renderer.Render(rendererBitmaps, graphics, frameWidth, frameHeight);
        }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            Point cursor = Game.CursorIngameCoordinates;

            GameBitmap startGame = menuOptions["start game"];

            if (MenuOptionHover(cursor, startGame))
            {
                BitmapsToRender[1][0] = readyStrings["start game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    Game.GameState = new ArkanoidGamePlayState(Game);



                    foreach (KeyValuePair<string, GameBitmap> bitmap in readyStrings)
                    {
                        RendererCache.RemoveBitmapFromMainMemory(bitmap.Value.PictureID);
                    }

                    return 100;
                }
            }
            else
            {
                BitmapsToRender[1][0] = readyStrings["start game"];
            }

            GameBitmap quitGame = menuOptions["quit game"];
            if (MenuOptionHover(cursor, quitGame))
            {
                BitmapsToRender[1][1] = readyStrings["quit game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    return 0;
                }
            }
            else
            {
                BitmapsToRender[1][1] = readyStrings["quit game"];
            }

            GameBitmap controls = menuOptions["controls"];
            if (MenuOptionHover(cursor, controls))
            {

                if (Game.IsControllerMouse)
                {
                    BitmapsToRender[1][2] = readyStrings["Game controls mouse hover"];
                }
                else
                {
                    BitmapsToRender[1][2] = readyStrings["Game controls keyboard hover"];
                }


                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    if (BitmapsToRender[1][2] == readyStrings["Game controls mouse hover"])
                    {
                        BitmapsToRender[1][2] = readyStrings["Game controls keyboard hover"];
                        Game.IsControllerMouse = false;
                    }
                    else
                    {
                        BitmapsToRender[1][2] = readyStrings["Game controls mouse hover"];
                        Game.IsControllerMouse = true;
                    }
                }
            }
            else if (Game.IsControllerMouse)
            {
                BitmapsToRender[1][2] = readyStrings["Game controls mouse"];
            }
            else
            {
                BitmapsToRender[1][2] = readyStrings["Game controls keyboard"];
            }

            //Посебна readonly копија за рендерерот
            List<IList<GameBitmap>> tempList = new List<IList<GameBitmap>>();
            for (int i = 0; i < BitmapsToRender.Count; i++)
            {
                tempList.Add(new List<GameBitmap>());
                for (int j = 0; j < BitmapsToRender[i].Count; j++)
                {
                    tempList[i].Add(BitmapsToRender[i][j]);
                }
            }
            rendererBitmaps = tempList;

            return 100;
        }

        private static bool MenuOptionHover(Point cursor, GameBitmap menuOptionBitmap)
        {
            return (cursor.X >= menuOptionBitmap.X
                && cursor.X <= menuOptionBitmap.X + menuOptionBitmap.WidthInGameUnits
                            && cursor.Y >= menuOptionBitmap.Y
                            && cursor.Y <= menuOptionBitmap.Y + menuOptionBitmap.HeightInGameUnits);
        }

        /// <summary>
        /// Слика која ќе се прикажува како позадина на менито
        /// </summary>
        public Bitmap MenuBackground { get; private set; }

        public IGame Game { get; set; }

        public ArkanoidStateMainMenu(IGame game)
        {
            MenuBackground = null;
            BitmapsToRender = new List<IList<GameBitmap>>();
            BitmapsToRender.Add(new List<GameBitmap>());
            BitmapsToRender[0].Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
                game.VirtualGameHeight));
            rendererBitmaps = new List<IList<GameBitmap>>();

            // додади ги сите опции во меморија
            readyStrings = new Dictionary<string, GameBitmap>();
            readyStrings.Add("start game", new GameBitmap(StaticStringFactory.CreateOrangeString("start game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("quit game", new GameBitmap(StaticStringFactory.CreateOrangeString("quit game"),
                (game.VirtualGameWidth - 500) / 2, 1100, 550, 90));
            readyStrings.Add("start game hover", new GameBitmap(StaticStringFactory.CreateBlueString("start game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("quit game hover", new GameBitmap(StaticStringFactory.CreateBlueString("quit game"),
                (game.VirtualGameWidth - 500) / 2, 1100, 550, 90));
            readyStrings.Add("Game controls mouse", new GameBitmap(StaticStringFactory
                .CreateOrangeString("Controls: mouse"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            readyStrings.Add("Game controls mouse hover", new GameBitmap(StaticStringFactory
                .CreateBlueString("Controls: mouse"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));

            readyStrings.Add("Game controls keyboard", new GameBitmap(StaticStringFactory
                .CreateOrangeString("Controls: keyboard"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            readyStrings.Add("Game controls keyboard hover", new GameBitmap(StaticStringFactory
                .CreateBlueString("Controls: keyboard"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));

            menuOptions = new Dictionary<string, GameBitmap>();
            menuOptions.Add("start game", readyStrings["start game"]);
            menuOptions.Add("quit game", readyStrings["quit game"]);
            menuOptions.Add("controls", readyStrings["Game controls mouse"]);

            BitmapsToRender.Add(new List<GameBitmap>());
            BitmapsToRender[1].Add(menuOptions["start game"]);
            BitmapsToRender[1].Add(menuOptions["quit game"]);
            BitmapsToRender[1].Add(menuOptions["controls"]);

            this.Game = game;
            Game.IsControllerMouse = true;
        }

        public bool IsTimesynchronizationImportant
        {
            get { return false; }
        }
    }

    public class ArkanoidGamePlayState : IGameState
    {
        public ArkanoidGamePlayState(IGame game)
        {
            this.Game = game;
            BitmapsToRender = new List<IList<GameBitmap>>();
            rendererBitmaps = new List<IList<GameBitmap>>();
            List<GameBitmap> background = new List<GameBitmap>();
            background.Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
                game.VirtualGameHeight));
            BitmapsToRender.Add(background);

            //додади го играчот на позиција (1750, 2010).
            PlayerPaddle player = new PlayerPaddle(new Vector2D(1750, 2010), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
            Game.GameObjects.Add(player);

            ElapsedTime = 0;
        }

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            Game.Renderer.Render(rendererBitmaps, graphics, frameWidth, frameHeight);
        }

        public long ElapsedTime { get; private set; }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            if (Game.IsMultithreadingEnabled)
            {
                UpdateObjectsMT(gameObjects);
            }
            else
            {
                UpdateObjectsST(gameObjects);
            }

            //Спреми ги текстурите
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (i + 1 >= BitmapsToRender.Count)
                    BitmapsToRender.Add(gameObjects[i].ObjectTextures);
                else
                    BitmapsToRender[i + 1] = gameObjects[i].ObjectTextures;
            }

            //Посебна readonly копија за рендерерот
            CreateCopyForRenderer();

            ElapsedTime++; //поминал еден период

            if (KeyStateInfo.GetAsyncKeyState(Keys.Escape).IsPressed)
            {
                return 0;
            }

            return 100;
        }

        /// <summary>
        /// Повикува Update на сите објекти од еден единствен thread.
        /// </summary>
        /// <param name="gameObjects"></param>
        private void UpdateObjectsST(IList<IGameObject> gameObjects)
        {
            foreach (IGameObject obj in gameObjects)
                obj.OnUpdate(ElapsedTime);
        }

        private static ManualResetEvent resetEvent = new ManualResetEvent(false);

        /// <summary>
        /// Повикува Update на секој објект во посебен thread. Се користи ThreadPool.
        /// </summary>
        /// <param name="gameObjects"></param>
        private void UpdateObjectsMT(IList<IGameObject> gameObjects)
        {
            //CountdownEvent
            /* http://msdn.microsoft.com/en-us/library/dd997365.aspx */

            using (CountdownEvent e = new CountdownEvent(1))
            {
                // fork work: 
                foreach (IGameObject obj in gameObjects)
                {
                    // Dynamically increment signal count.
                    e.AddCount();
                    ThreadPool.QueueUserWorkItem(delegate(object state)
                     {
                         try
                         {
                             UpdateObject((IGameObject)state);
                         }
                         finally
                         {
                             e.Signal();
                         }
                     },
                     obj);
                }
                e.Signal();

                // The first element could be run on this thread. 

                // Join with work.
                e.Wait();
            }
        }

        private void UpdateObject(IGameObject obj)
        {
            obj.OnUpdate(ElapsedTime);
        }

        private void CreateCopyForRenderer()
        {
            List<IList<GameBitmap>> tempList = new List<IList<GameBitmap>>();
            for (int i = 0; i < BitmapsToRender.Count; i++)
            {
                tempList.Add(new List<GameBitmap>());
                for (int j = 0; j < BitmapsToRender[i].Count; j++)
                {
                    tempList[i].Add(BitmapsToRender[i][j]);
                }
            }
            rendererBitmaps = tempList;
        }

        public IGame Game { get; private set; }

        public bool IsTimesynchronizationImportant
        {
            get { return true; }
        }

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; } //текстури за секој објект

        private IList<IList<GameBitmap>> rendererBitmaps; /* Копија од листата BitmapsToRender. Бидејќи
                                                           * листата BitmapsToRender може да се менува,
                                                           * а Draw е посебен thread, за да се избегне 
                                                           * синхронизација помеѓу нишките (што ќе го забави
                                                           * времето на извршување), може да се прати
                                                           * копија од листата на слики од последното извршување 
                                                           * на OnUpdate(). Бидејќи ќе пратиме копија ако дојде 
                                                           * до отстранување на некоја слика од BitmapsToRender, таа
                                                           * промена ќе се прикаже на првиот повик на Draw по копирањето 
                                                           * на сликите во листата rendererBitmaps.
                                                           */
    }

    public class GameArkanoid : IGame
    {
        

        //Стартна позиција на играчот (1750, 2010);

        /// <summary>
        /// Дали update ќе се извршува на една или повеќе нишки
        /// </summary>
        public bool IsMultithreadingEnabled { get; private set; }

        /// <summary>
        /// Вклучи или исклучи во зависност од тоа дали е пристисната буквата M
        /// </summary>
        private void EnableOrDisableMultithreading()
        {
            IKeyState keyState = KeyStateInfo.GetAsyncKeyState(Keys.M);
            if (keyState.IsPressed)
                IsMultithreadingEnabled = !IsMultithreadingEnabled;
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

            this.GameState.OnDraw(graphics, frameWidth, frameHeight);
            if (IsMultithreadingEnabled)
                Renderer.Render(textMultithreading, graphics, frameWidth, frameHeight);
        }

        //во милисекунди
        public int GameUpdatePeriod { get; set; }

        private GameBitmap textMultithreading;

        /// <summary>
        /// Креира нова игра и ја поставува во почетна состојба ArkanoidStateMainMenu.
        /// </summary>
        private GameArkanoid()
        {
            this.IsMultithreadingEnabled = false;
            IsRendererEnabled = false;

            VirtualGameWidth = 3840;
            VirtualGameHeight = 2160;
            this.Renderer = new GameRenderer(VirtualGameWidth, VirtualGameHeight);
            CursorIngameCoordinates = Cursor.Position;
            this.GameUpdatePeriod = 0;
            GameState = new ArkanoidStateMainMenu(this);
            Name = "Arkanoid";
            GameObjects = new List<IGameObject>();

            Bitmap textMT = StaticStringFactory.CreateOrangeString("Multithreading");
            textMultithreading = new GameBitmap(textMT, VirtualGameWidth - 400 - 10, 5, 400,
                60);            

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

            return GameState.OnUpdate(GameObjects);
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
        public bool IsTimesynchronizationImportant
        {
            get
            {
                return GameState.IsTimesynchronizationImportant;
            }
        }


        public IGameRenderer Renderer { get; private set; }


        public bool IsControllerMouse { get; set; }
    }
}
