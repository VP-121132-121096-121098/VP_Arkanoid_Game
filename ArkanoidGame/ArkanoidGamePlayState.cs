using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Objects;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public class ArkanoidGamePlayState : IGameState
    {
        private bool debugMode;

        public ArkanoidGamePlayState(IGame game)
        {
            debugMode = false;
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
            EnableOrDisableDebugMode();

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

        private void EnableOrDisableDebugMode()
        {
            if (KeyStateInfo.GetAsyncKeyState(Keys.D).IsPressed)
            {
                debugMode = !debugMode;
            }
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
                    ThreadPool.QueueUserWorkItem(delegate(object gameObject)
                    {
                        try
                        {
                            UpdateObject((IGameObject)gameObject);
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
}
