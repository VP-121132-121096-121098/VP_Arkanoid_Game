using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Objects;
using ArkanoidGame.Quadtree;
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

        private QuadTree<IGameObject> quadtree; //со оваа структура може да се подели рамнината на делови

        //за секој објект се чува посебна листа од објекти со кои се судрил и во кои точки се судрил со тие објекти
        private IDictionary<IGameObject, IDictionary<IGameObject, IList<Vector2D>>> collisionArguments;

        private readonly object lockCollisionDetection;

        public ArkanoidGamePlayState(IGame game)
        {
            lockCollisionDetection = new object();
            debugMode = false;
            quadtree = null;
            ButtonDWaitNFrames = 0;
            this.Game = game;
            BitmapsToRender = new List<IList<GameBitmap>>();
            bitmapsToRenderCopy = new List<IList<GameBitmap>>();
            List<GameBitmap> background = new List<GameBitmap>();
            background.Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
                game.VirtualGameHeight));
            BitmapsToRender.Add(background);

            //додади го играчот на позиција (1750, 2010).
            PlayerPaddle player = new PlayerPaddle(new Vector2D(1750, 2010), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
            Game.GameObjects.Add(player);
            BlueBall ball = new BlueBall(new Vector2D((player.Position.X * 2 + player.ObjectWidth) / 2,
               player.Position.Y - 45), 50);
            Game.GameObjects.Add(ball);
            /*BigBrick grb = new BigBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, "element_red_rectangle.png");
            Game.GameObjects.Add(grb);
            SmallBrick srb = new SmallBrick(new Vector2D(220, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, "element_red_square.png");
            Game.GameObjects.Add(srb);
            SmallBrick smbb = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 1110, 100), Game.VirtualGameWidth,
              Game.VirtualGameHeight, "element_purple_square.png");
            Game.GameObjects.Add(smbb);
            SmallBrick smgb = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 1220, 100), Game.VirtualGameWidth,
             Game.VirtualGameHeight, "element_green_square.png");
            Game.GameObjects.Add(smgb);
           /* //proba da dodadam golema crvena cigla
            BigRedBrick grb = new BigRedBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
           
            //proba da dodadam mala crvena cigla
            
            //proba da dodadam golema zolta cigla
            BigYellowBrick gyb = new BigYellowBrick(new Vector2D(325, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
            Game.GameObjects.Add(gyb);
            //proba da dodadam mala zolta cigla
            SmallYellowBrick syb = new SmallYellowBrick(new Vector2D(530, 100), Game.VirtualGameWidth,
                 Game.VirtualGameHeight);
            Game.GameObjects.Add(syb);
            //proba da dodadam golema violetova cigla
            BigPurpleBrick gpb = new BigPurpleBrick(new Vector2D(640, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
            Game.GameObjects.Add(gpb);
            //proba da dodadam mala zolta cigla
            SmallPurpleBrick spb = new SmallPurpleBrick(new Vector2D(850, 100), Game.VirtualGameWidth,
                 Game.VirtualGameHeight);
            Game.GameObjects.Add(spb);
            //golema zelena cigla
            BigGreenBrick bgb = new BigGreenBrick(new Vector2D(960, 100), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(bgb);
            // mala zelena cigla
          SmallGreenBrick sgb = new SmallGreenBrick(new Vector2D(1165, 100), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(sgb); 
            // golema siva
           BigRedBrick bgr = new BigRedBrick(new Vector2D(Game.VirtualGameWidth-360, 100), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(bgr); 
           //mala siva
           SmallRedBrick sgg = new SmallRedBrick(new Vector2D(Game.VirtualGameWidth - 470, 100), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(sgg);
           SmallYellowBrick ypp = new SmallYellowBrick(new Vector2D(Game.VirtualGameWidth - 580, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
           Game.GameObjects.Add(ypp);
           BigYellowBrick gbb = new BigYellowBrick(new Vector2D(Game.VirtualGameWidth - 790, 100), Game.VirtualGameWidth,
              Game.VirtualGameHeight);
           Game.GameObjects.Add(gbb);
           BigPurpleBrick bbb = new BigPurpleBrick(new Vector2D(Game.VirtualGameWidth - 1000, 100), Game.VirtualGameWidth,
            Game.VirtualGameHeight);
           Game.GameObjects.Add(bbb);
           
           SmallYellowBrick sybb = new SmallYellowBrick(new Vector2D(Game.VirtualGameWidth - 1330, 100), Game.VirtualGameWidth,
            Game.VirtualGameHeight);
           Game.GameObjects.Add(sybb);
           SmallGreyBrick nivo2cigla1 = new SmallGreyBrick(new Vector2D(1170, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla1);
           BigGreyBrick nivo2cigla2 = new BigGreyBrick(new Vector2D(1280, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla2);
           BigBlueBrick nivo2cigla3 = new BigBlueBrick(new Vector2D(1490, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla3);
           SmallBlueBrick nivo2cigla4 = new SmallBlueBrick(new Vector2D(1700, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla4);
           SmallBlueBrick nivo2cigla5 = new SmallBlueBrick(new Vector2D(Game.VirtualGameWidth - 1860, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla5);
           BigBlueBrick nivo2cigla6 = new BigBlueBrick(new Vector2D(Game.VirtualGameWidth - 1750, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla6);
           BigGreyBrick nivo2cigla7 = new BigGreyBrick(new Vector2D(Game.VirtualGameWidth - 1540, 300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla7);
           SmallGreyBrick nivo2cigla8 = new SmallGreyBrick(new Vector2D(Game.VirtualGameWidth - 1330,300), Game.VirtualGameWidth, Game.VirtualGameHeight);
           Game.GameObjects.Add(nivo2cigla8);*/


            ElapsedTime = 0;
        }

#if DEBUG
        private void DebugDrawCollisionArguments(IDictionary<IGameObject, IList<Vector2D>> args,
            Graphics graphics, int frameWidth, int frameHeight)
        {
            foreach (KeyValuePair<IGameObject, IList<Vector2D>> arg in args)
            {
                foreach (Vector2D point in arg.Value)
                {
                    Game.Renderer.DrawCircle(point, 20, graphics, Color.Aqua, frameWidth, frameHeight);
                }
            }
        }
#endif

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            Game.Renderer.Render(bitmapsToRenderCopy, graphics, frameWidth, frameHeight);

#if DEBUG
            if (debugMode && quadtree != null)
            {
                Point cursor = Game.CursorIngameCoordinates;
                QuadTreeRenderer<IGameObject> treeRenderer = new QuadTreeRenderer<IGameObject>(quadtree);
                treeRenderer.Render(Game.Renderer, graphics, frameWidth, frameHeight,
                    new RectangleF(cursor.X, cursor.Y, 30, 30), Color.Aqua);

                IDictionary<IGameObject, IDictionary<IGameObject, IList<Vector2D>>> collisions = collisionArguments;
                foreach (KeyValuePair<IGameObject, IDictionary<IGameObject, IList<Vector2D>>> collision in collisions)
                {
                    if (collision.Key.ObjectType == GameObjectType.Ball)
                    {
                        treeRenderer.Render(Game.Renderer, graphics, frameWidth, frameHeight,
                            collision.Key.Rectangle, Color.Red);
                        this.DebugDrawCollisionArguments(collision.Value, graphics, frameWidth, frameHeight);
                    }
                }
            }
#endif
        }

        public long ElapsedTime { get; private set; }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
#if DEBUG
            EnableOrDisableDebugMode();
#endif

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

        private int ButtonDWaitNFrames;

        private void EnableOrDisableDebugMode()
        {
            ButtonDWaitNFrames = Math.Max(0, ButtonDWaitNFrames - 1);

            if (KeyStateInfo.GetAsyncKeyState(Keys.D).IsPressed && ButtonDWaitNFrames == 0)
            {
                debugMode = !debugMode;
                ButtonDWaitNFrames = 10;
            }
        }

        /// <summary>
        /// Повикува Update на сите објекти од еден единствен thread.
        /// </summary>
        /// <param name="gameObjects"></param>
        private void UpdateObjectsST(IList<IGameObject> gameObjects)
        {
            foreach (IGameObject obj in gameObjects)
                UpdateObject(obj);

            InitQuadTree(gameObjects);
            InitCollisionArguments(gameObjects);

            foreach (IGameObject obj in gameObjects)
                CheckForCollisions(obj);

            foreach (IGameObject obj in gameObjects)
            {
                PassCollisionArgumentsToObject(obj);
            }
        }

        private void InitCollisionArguments(IList<IGameObject> gameObjects)
        {
            collisionArguments = new Dictionary<IGameObject, IDictionary<IGameObject, IList<Vector2D>>>();
            foreach (IGameObject obj in gameObjects)
            {
                collisionArguments.Add(obj, new Dictionary<IGameObject, IList<Vector2D>>());
            }
        }

        private void CheckForCollisions(IGameObject obj)
        {
            List<IGameObject> objectsInArea = quadtree.Query(obj.Rectangle);

            IGeometricShape geometricShape = obj.GetGeometricShape();
            foreach (IGameObject gameObject in objectsInArea)
            {
                if (gameObject == obj)
                    continue;

                List<Vector2D> temp = null;
                if (geometricShape.Intersects(gameObject.GetGeometricShape(), out temp))
                {
                    collisionArguments[obj].Add(gameObject, temp);
                }
            }
        }

        private void PassCollisionArgumentsToObject(IGameObject obj)
        {
            obj.OnCollisionDetected(collisionArguments[obj]);
        }

        private void InitQuadTree(IList<IGameObject> gameObjects)
        {
            quadtree = new QuadTree<IGameObject>(new RectangleF(0, 0, (float)Game.VirtualGameWidth + 0.1f,
                (float)Game.VirtualGameHeight + 0.1f));
            foreach (IGameObject obj in gameObjects)
                quadtree.Insert(obj);
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

            //не може паралелно да се одвива оваа операција
            InitQuadTree(gameObjects);
            InitCollisionArguments(gameObjects);

            //паралелен дел
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
                            CheckForCollisions((IGameObject)gameObject);
                            PassCollisionArgumentsToObject((IGameObject)gameObject);
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
            bitmapsToRenderCopy = tempList;
        }

        public IGame Game { get; private set; }

        public bool IsTimesynchronizationImportant
        {
            get { return true; }
        }

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; } //текстури за секој објект

        private IList<IList<GameBitmap>> bitmapsToRenderCopy; /* Копија од листата BitmapsToRender. Бидејќи
                                                               * листата BitmapsToRender може да се менува,
                                                               * а Draw е посебен thread, за да се избегне 
                                                               * синхронизација помеѓу нишките (што ќе го забави
                                                               * времето на извршување), може да се прати
                                                               * копија од листата на слики од последното извршување 
                                                               * на OnUpdate(). Бидејќи ќе пратиме копија ако дојде 
                                                               * до отстранување на некоја слика од BitmapsToRender, таа
                                                               * промена ќе се прикаже на првиот повик на Draw 
                                                               * по копирањето 
                                                               * на сликите во листата rendererBitmaps.
                                                               */
    }
}
