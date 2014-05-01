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

        private ISet<IGameObject> ballsInPlay;

        private List<GameBitmap> background;

        private void RotateBricks()
        {
            QuadTree<IGameObject> quadTree = this.quadtree;
            List<IGameObject> area = quadtree.Query(new RectangleF(1000, 600, 1000, 600));
            Vector2D center = new Vector2D(1500, 900);

            if (!Game.IsMultithreadingEnabled)
            {
                foreach (IGameObject obj in area)
                {
                    if (obj.ObjectType != GameObjectType.Brick)
                        continue;


                    RotateSingleBrick(center, obj);
                }
            }
            else
            {
                //паралелен дел
                using (CountdownEvent e = new CountdownEvent(1))
                {
                    // fork work: 
                    foreach (IGameObject obj in area)
                    {
                        // Dynamically increment signal count.
                        e.AddCount();
                        ThreadPool.QueueUserWorkItem(delegate(object gameObject)
                        {
                            try
                            {
                                RotateSingleBrick(center, obj);
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
        }

        private static void RotateSingleBrick(Vector2D center, IGameObject obj)
        {
            GameRectangle rotator = new GameRectangle(obj.PositionUL, obj.PositionUR, obj.PositionDL);
            rotator.RotateAroundPointDeg(center, 1);
            obj.PositionUL.X = rotator.PositionUL.X;
            obj.PositionUL.Y = rotator.PositionUL.Y;
            obj.PositionUR.X = rotator.PositionUR.X;
            obj.PositionUR.Y = rotator.PositionUR.Y;
            obj.PositionDL.X = rotator.PositionDL.X;
            obj.PositionDL.Y = rotator.PositionDL.Y;
        }

        public ArkanoidGamePlayState(IGame game)
        {
            lockCollisionDetection = new object();
            debugMode = false;
            quadtree = null;
            ButtonDWaitNFrames = 0;
            this.Game = game;
            BitmapsToRender = new List<IList<GameBitmap>>();
            bitmapsToRenderCopy = new List<IList<GameBitmap>>();
            background = new List<GameBitmap>();
            background.Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
              game.VirtualGameHeight));

            ballsInPlay = new HashSet<IGameObject>();

            //додади го играчот на позиција (1750, 2010).
            PlayerPaddle player = new PlayerPaddle(new Vector2D(1750, 2010), Game.VirtualGameWidth,
                Game.VirtualGameHeight);
            Game.GameObjects.Add(player);

            //додади ја топката
            BlueBall ball = new BlueBall(new Vector2D((player.Position.X * 2 + player.ObjectWidth) / 2,
               player.Position.Y - 45), 50);
            Game.GameObjects.Add(ball);
            ballsInPlay.Add(ball);

            CreateBricks();



            /* //proba da dodadam golema crvena cigla
             BigRedBrick grb = new BigRedBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                 Game.VirtualGameHeight);
           
             //proba da dodadam mala crvena cigla
            
             //proba da dodadam golema zolta cigla
             */


            ElapsedTime = 0;
        }

        private void CreateBricks()
        {   
           // Random r=new Random;
            //int opcija=r.Next(1);
            double y = 100;

            GameBitmap redBrickTexture = new GameBitmap(RendererCache.GetBitmapFromFile(
                "\\Resources\\Images\\element_red_rectangle.png"), new Vector2D(0, 0),
                new Vector2D(200, 0), new Vector2D(0, 100));

            for (int i = 0; i < 6; i++)
            {

                BigBrick grb = new BigBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, redBrickTexture);
                double offset = 100;
                while (offset + grb.ObjectWidth < Game.VirtualGameWidth - 20)
                {

                    GameBitmap temp = new GameBitmap(redBrickTexture.UniqueKey, offset, y, 200,
                    80);

                    Game.GameObjects.Add(new BigBrick(new Vector2D(offset, y), Game.VirtualGameWidth,
                Game.VirtualGameHeight, temp));
                    offset += grb.ObjectWidth + 120;
                }

                y += grb.ObjectHeight + 80;
            }


            

          /*  GameBitmap greyBrickTexture = new GameBitmap(RendererCache.GetBitmapFromFile(
                "\\Resources\\Images\\element_grey_square.png"), new Vector2D(0, 0),
                new Vector2D(150, 0), new Vector2D(0, 150));

            for (int i = 0; i < 6; i++)
            {

                SmallBrick sgb = new SmallBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, greyBrickTexture);
                double offset = 100;
                while (offset + sgb.ObjectWidth < Game.VirtualGameWidth - 20)
                {

                    GameBitmap temp = new GameBitmap(greyBrickTexture.UniqueKey, offset, y, 200,
                    80);

                    Game.GameObjects.Add(new BigBrick(new Vector2D(offset, y), Game.VirtualGameWidth,
                Game.VirtualGameHeight, temp));
                    offset += sgb.ObjectWidth + 120;
                }

                y += sgb.ObjectHeight + 80;
            }*/

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
            BigBrick gyb = new BigBrick(new Vector2D(325, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, "element_yellow_rectangle.png");
            Game.GameObjects.Add(gyb);
            //proba da dodadam mala zolta cigla
            SmallBrick syb = new SmallBrick(new Vector2D(530, 100), Game.VirtualGameWidth,
                 Game.VirtualGameHeight, "element_yellow_square.png");
            Game.GameObjects.Add(syb);
            //proba da dodadam golema violetova cigla
            BigBrick gpb = new BigBrick(new Vector2D(640, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, "element_purple_rectangle.png");
            Game.GameObjects.Add(gpb);
            //proba da dodadam mala zolta cigla
            SmallBrick spb = new SmallBrick(new Vector2D(850, 100), Game.VirtualGameWidth,
                 Game.VirtualGameHeight, "element_purple_square.png");
            Game.GameObjects.Add(spb);
            //golema zelena cigla
            BigBrick bgb = new BigBrick(new Vector2D(960, 100), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_green_rectangle.png");
            Game.GameObjects.Add(bgb);
            // mala zelena cigla
            SmallBrick sgb = new SmallBrick(new Vector2D(1165, 100), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_green_square.png");
            Game.GameObjects.Add(sgb);
            // golema siva
            BigBrick bgr = new BigBrick(new Vector2D(Game.VirtualGameWidth - 360, 100), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_grey_rectangle.png");
            Game.GameObjects.Add(bgr);
            //mala siva
            SmallBrick sgg = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 470, 100), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_grey_square.png");
            Game.GameObjects.Add(sgg);
            SmallBrick ypp = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 580, 100), Game.VirtualGameWidth,
                 Game.VirtualGameHeight, "element_green_square.png");
            Game.GameObjects.Add(ypp);
            BigBrick gbb = new BigBrick(new Vector2D(Game.VirtualGameWidth - 790, 100), Game.VirtualGameWidth,
                Game.VirtualGameHeight, "element_yellow_rectangle.png");
            Game.GameObjects.Add(gbb);
            BigBrick bbb = new BigBrick(new Vector2D(Game.VirtualGameWidth - 1000, 100), Game.VirtualGameWidth,
             Game.VirtualGameHeight, "element_purple_rectangle.png");
            Game.GameObjects.Add(bbb);

            SmallBrick sybb = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 1330, 100), Game.VirtualGameWidth,
             Game.VirtualGameHeight, "element_yellow_square.png");
            Game.GameObjects.Add(sybb);
            SmallBrick nivo2cigla1 = new SmallBrick(new Vector2D(1170, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_grey_square.png");
            Game.GameObjects.Add(nivo2cigla1);
            BigBrick nivo2cigla2 = new BigBrick(new Vector2D(1280, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_grey_rectangle.png");
            Game.GameObjects.Add(nivo2cigla2);
            BigBrick nivo2cigla3 = new BigBrick(new Vector2D(1490, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_blue_rectangle.png");
            Game.GameObjects.Add(nivo2cigla3);
            SmallBrick nivo2cigla4 = new SmallBrick(new Vector2D(1700, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_blue_square.png");
            Game.GameObjects.Add(nivo2cigla4);
            SmallBrick nivo2cigla5 = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 1860, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_blue_square.png");
            Game.GameObjects.Add(nivo2cigla5);
            BigBrick nivo2cigla6 = new BigBrick(new Vector2D(Game.VirtualGameWidth - 1750, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_blue_rectangle.png");
            Game.GameObjects.Add(nivo2cigla6);
            BigBrick nivo2cigla7 = new BigBrick(new Vector2D(Game.VirtualGameWidth - 1540, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_grey_rectangle.png");
            Game.GameObjects.Add(nivo2cigla7);
            SmallBrick nivo2cigla8 = new SmallBrick(new Vector2D(Game.VirtualGameWidth - 1330, 300), Game.VirtualGameWidth, Game.VirtualGameHeight, "element_grey_square.png");
            Game.GameObjects.Add(nivo2cigla8);*/
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

            RemoveDeadObjects(gameObjects);

            if (Game.IsMultithreadingEnabled)
            {
                UpdateObjectsMT(gameObjects);
            }
            else
            {
                UpdateObjectsST(gameObjects);
            }

            RotateBricks();

            //Спреми ги текстурите
            BitmapsToRender = new List<IList<GameBitmap>>();
            BitmapsToRender.Add(background);

            foreach (IGameObject obj in gameObjects)
            {
                BitmapsToRender.Add(obj.ObjectTextures);
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

        private void RemoveDeadObjects(IList<IGameObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                IGameObject obj = objects[i];
                if (obj.Health <= 0)
                {
                    if (obj.ObjectType == GameObjectType.Ball)
                    {
                        ballsInPlay.Remove(obj);
                    }

                    objects.RemoveAt(i);
                }

            }
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
                            PassCollisionArgumentsToObject((IGameObject)gameObject);
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
