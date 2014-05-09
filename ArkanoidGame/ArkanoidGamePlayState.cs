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

        private int ballIncreaseSpeedPeriodCounter;
        private int multiBallPeriodsCounter;

        private readonly int multiballPeriod = 3750;

        /// <summary>
        /// Колку поени играчот освоил во играта
        /// </summary>
        private long Score { get; set; }

        private QuadTree<IGameObject> quadtree; //со оваа структура може да се подели рамнината на делови

        //за секој објект се чува посебна листа од објекти со кои се судрил и во кои точки се судрил со тие објекти
        private IDictionary<IGameObject, IDictionary<IGameObject, IList<Vector2D>>> collisionArguments;

        private readonly object lockCollisionDetection;

        private ISet<IGameObject> ballsInPlay;

        private List<GameBitmap> background;

        private void RotateBricks()
        {
            QuadTree<IGameObject> quadTree = this.quadtree;
            List<IGameObject> area = quadtree.Query(new RectangleF(Game.VirtualGameWidth / 2 - 300, 600, 600, 600));
            Vector2D center = new Vector2D(Game.VirtualGameWidth / 2, 900);

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

        /// <summary>
        /// Референца кон paddle (објектот што се контролира со глувчето или тастатурата)
        /// </summary>
        private PlayerPaddle LocalPlayer { get; set; }

        public ArkanoidGamePlayState(IGame game)
        {
            this.ballIncreaseSpeedPeriodCounter = 0;
            this.multiBallPeriodsCounter = multiballPeriod - 30;
            Score = 0;
            lockCollisionDetection = new object();
            debugMode = false;
            quadtree = null;
            ButtonDWaitNFrames = 0;
            this.Game = game;
            this.Game.GameObjects.Clear(); //бриши ги сите објекти (ако има такви)
            RendererCache.RemoveAllBitmapsFromMainMemory(); //исчисти го баферот од претходната состојба
            Game.ReloadResources();
            this.InitBrickTextures();

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
            this.LocalPlayer = player;

            //додади ја топката
            BlueBall ball = new BlueBall(new Vector2D((player.Position.X * 2 + player.ObjectWidth) / 2,
               player.Position.Y - 45), 50, player);
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

        private GameBitmap BrickTextureRedRectangle;
        private GameBitmap BrickTextureBlueRectangle;
        private GameBitmap BrickTexturePurpleRectangle;
        private GameBitmap BrickTextureYellowRectangle;
        private GameBitmap BrickTextureGreyRectangle;
        private GameBitmap BrickTextureGreenRectangle;
        private GameBitmap BrickTextureRedRectangle1;
        private GameBitmap BrickTextureBlueRectangle1;
        private GameBitmap BrickTexturePurpleRectangle1;
        private GameBitmap BrickTextureYellowRectangle1;
        private GameBitmap BrickTextureGreyRectangle1;
        private GameBitmap BrickTextureGreenRectangle1;

        /// <summary>
        /// Иницијализирај ги (вчитај ги) сите слики за циглите во меморија. Овој метод е направен
        /// поради оптимизација. Сите цигли од ист тип ќе користат иста слика (бидејќи
        /// сите од ист тип се и со исти димензии нема да има resize).
        /// </summary>
        private void InitBrickTextures()
        {
            BrickTextureRedRectangle = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_red_rectangle.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureBlueRectangle = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_blue_rectangle.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTexturePurpleRectangle = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_purple_rectangle.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureYellowRectangle = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_yellow_rectangle.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureGreyRectangle = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_grey_rectangle.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureGreenRectangle = new GameBitmap(RendererCache.GetBitmapFromFile(
                      "\\Resources\\Images\\element_green_rectangle.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureRedRectangle1 = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_red_square.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureBlueRectangle1 = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_blue_square.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTexturePurpleRectangle1 = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_purple_square.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureYellowRectangle1 = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_yellow_square.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureGreyRectangle1 = new GameBitmap(RendererCache.GetBitmapFromFile(
                       "\\Resources\\Images\\element_grey_square.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
            BrickTextureGreenRectangle1 = new GameBitmap(RendererCache.GetBitmapFromFile(
                      "\\Resources\\Images\\element_green_square.png"), new Vector2D(0, 0),
                          new Vector2D(200, 0), new Vector2D(0, 100));
        }

        private void CreateBricks()
        {
            Random r = new Random((int)DateTime.Now.Ticks);

            double y = 100;
            GameBitmap BrickTexture = null;

            for (int i = 0; i < 6; i++)
            {
                //int d = r.Next(3);
                int opcija = r.Next(6);
                if ((i % 2) == 0)
                {
                    if (opcija == 0)
                    {
                        BrickTexture = new GameBitmap(BrickTextureRedRectangle.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(200, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Red;
                    }
                    else if (opcija == 1)
                    {
                        BrickTexture = new GameBitmap(BrickTextureBlueRectangle.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(200, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Blue;
                    }
                    else if (opcija == 2)
                    {
                        BrickTexture = new GameBitmap(BrickTexturePurpleRectangle.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(200, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Purple;
                    }
                    else if (opcija == 3)
                    {
                        BrickTexture = new GameBitmap(BrickTextureYellowRectangle.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(200, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Yellow;
                    }
                    else if (opcija == 4)
                    {
                        BrickTexture = new GameBitmap(BrickTextureGreyRectangle.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(200, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Gray;
                    }
                    else
                    {
                        BrickTexture = new GameBitmap(BrickTextureGreenRectangle.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(200, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Green;
                    }


                    BigBrick grb = new BigBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                    Game.VirtualGameHeight, BrickTexture);

                    double offset = 180;
                    while (offset + grb.ObjectWidth < Game.VirtualGameWidth - 50)
                    {

                        GameBitmap temp = new GameBitmap(BrickTexture.UniqueKey, offset, y, 220,
                       100);
                        temp.ColorLowSpec = BrickTexture.ColorLowSpec;

                        Game.GameObjects.Add(new BigBrick(new Vector2D(offset, y), Game.VirtualGameWidth,
                    Game.VirtualGameHeight, temp));

                        offset += grb.ObjectWidth + 250;
                    }

                    y += grb.ObjectHeight + 100;
                }

                else
                {
                    if (opcija == 0)
                    {
                        BrickTexture = new GameBitmap(BrickTextureRedRectangle1.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(100, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Red;
                    }
                    else if (opcija == 1)
                    {
                        BrickTexture = new GameBitmap(BrickTextureBlueRectangle1.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(100, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Blue;
                    }
                    else if (opcija == 2)
                    {
                        BrickTexture = new GameBitmap(BrickTexturePurpleRectangle1.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(100, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Purple;
                    }
                    else if (opcija == 3)
                    {
                        BrickTexture = new GameBitmap(BrickTextureYellowRectangle1.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(100, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Yellow;
                    }
                    else if (opcija == 4)
                    {
                        BrickTexture = new GameBitmap(BrickTextureGreyRectangle1.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(100, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Gray;
                    }
                    else
                    {
                        BrickTexture = new GameBitmap(BrickTextureGreenRectangle1.UniqueKey, new Vector2D(0, 0),
                            new Vector2D(100, 0), new Vector2D(0, 100));
                        BrickTexture.ColorLowSpec = Color.Green;
                    }


                    SmallBrick grb = new SmallBrick(new Vector2D(20, 100), Game.VirtualGameWidth,
                    Game.VirtualGameHeight, BrickTexture);

                    double offset = 290;
                    while (offset + grb.ObjectWidth < Game.VirtualGameWidth - 50)
                    {

                        GameBitmap temp = new GameBitmap(BrickTexture.UniqueKey, offset, y, 100,
                        100);
                        temp.ColorLowSpec = BrickTexture.ColorLowSpec;

                        Game.GameObjects.Add(new SmallBrick(new Vector2D(offset, y), Game.VirtualGameWidth,
                    Game.VirtualGameHeight, temp));

                        offset += grb.ObjectWidth + 200;
                    }

                    y += grb.ObjectHeight + 140;
                }





            }
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

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight, bool lowSpec)
        {
            Game.Renderer.Render(bitmapsToRenderCopy, graphics, frameWidth, frameHeight, lowSpec);
            Game.Renderer.ShowScoreOnScreen(string.Format("Score: {0}", Score), Color.Orange, 11.2f, graphics,
                frameWidth, frameHeight);
            Game.Renderer.ShowETAMultiball((3750 - multiBallPeriodsCounter) * 16 / 1000.0f,
                Color.Orange, 11.2f, graphics, frameWidth, frameHeight);
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
                Game.GameState = new ArkanoidPauseMenuState(Game, this);
            }

            this.IncreaseSpeedOfBalls();
            this.CreateNewBalls();

            if (ballsInPlay.Count == 0)
            {
                Game.GameState = new ArkanoidGameOverState(Game, Score);
                return 100;
            }

            IEnumerator<IGameObject> iter = ballsInPlay.GetEnumerator();
            if (iter.MoveNext() && iter.Current.Velocity.Magnitude() < 0.001)
            {
                this.ballIncreaseSpeedPeriodCounter = 0;
                this.multiBallPeriodsCounter = 3750 - 30;
            }

            return 100;
        }

        /// <summary>
        /// Избриши ги објектите (циглите) со Health = 0. 
        /// </summary>
        /// <param name="objects"></param>
        private void RemoveDeadObjects(IList<IGameObject> objects)
        {
            for (int i = objects.Count - 1; i > 0; i--)
            {
                IGameObject obj = objects[i];
                if (obj.Health <= 0)
                {
                    if (obj.ObjectType == GameObjectType.Ball)
                    {
                        ballsInPlay.Remove(obj);
                        foreach (GameBitmap texture in obj.ObjectTextures)
                        {
                            RendererCache.RemoveBitmapFromMainMemory(texture.UniqueKey);
                        }
                    }

                    this.Score += obj.GetScoreForDestruction();
                    objects.RemoveAt(i);
                }

            }

            if (ballsInPlay.Count > 0 && ballsInPlay.Count + 1 == objects.Count)
            {
                //ако се уште има топчиња во игра, а бројот на објекти е бројот на топчиња + палката
                //(нема цигли), тогаш генерирај нови цигли
                this.CreateBricks();

                //multiball
                this.multiBallPeriodsCounter = multiballPeriod;
            }
        }

        /// <summary>
        /// Ја зголемува брзината на топчето на секои 1250 периоди (20 секунди) од играта.
        /// </summary>
        void IncreaseSpeedOfBalls()
        {
            ballIncreaseSpeedPeriodCounter++;

            if (ballIncreaseSpeedPeriodCounter < 1250)
                return;

            foreach (IGameObject ball in ballsInPlay)
            {
                double speed = ball.Velocity.Magnitude();
                ball.Velocity /= speed;
                speed *= 1.1;
                ball.Velocity *= speed;
            }

            this.ballIncreaseSpeedPeriodCounter = 0;
        }

        /// <summary>
        /// Креира нови топки секоја минута
        /// </summary>
        void CreateNewBalls()
        {
            multiBallPeriodsCounter++;

            if (multiBallPeriodsCounter < multiballPeriod)
            {
                return;
            }

            //направи нови топки на секои 3750 периоди (1 минута)
            this.multiBallPeriodsCounter = 0;

            //направи две нови топки за секоја што е во игра
            IEnumerator<IGameObject> it = ballsInPlay.GetEnumerator();

            if (it.MoveNext())
            {
                BlueBall ball = (BlueBall)it.Current;

                BlueBall ball2 = new BlueBall(ball.Position + new Vector2D(ball.Radius, ball.Radius),
                    ball.Radius, LocalPlayer);
                ball2.Velocity.X = ball.Velocity.X;
                ball2.Velocity.Y = ball.Velocity.Y;
                ball2.Velocity.RotateDeg(-30);
                BlueBall ball3 = new BlueBall(ball.Position + new Vector2D(-ball.Radius, -ball.Radius),
                    ball.Radius, LocalPlayer);
                ball3.Velocity.X = ball.Velocity.X;
                ball3.Velocity.Y = ball.Velocity.Y;
                ball3.Velocity.RotateDeg(30);
                Game.GameObjects.Add(ball2);
                Game.GameObjects.Add(ball3);
                ballsInPlay.Add(ball2);
                ballsInPlay.Add(ball3);
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

                //не проверуваме за судири на објектот со самиот себе, како и на цигла со
                //цигла, ниту пак на топче со топче
                if (gameObject == obj || gameObject.ObjectType == GameObjectType.Brick
                    && obj.ObjectType == GameObjectType.Brick
                    || gameObject.ObjectType == GameObjectType.Ball && obj.ObjectType == GameObjectType.Ball)
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

        /// <summary>
        /// Направи нов QuadTree и пополни го со сите објекти кои моментално постојат во играта.
        /// Се повикува при секој повик на OnUpdate
        /// </summary>
        /// <param name="gameObjects"></param>
        private void InitQuadTree(IList<IGameObject> gameObjects)
        {
            quadtree = new QuadTree<IGameObject>(new RectangleF(0, 0, (float)Game.VirtualGameWidth + 0.1f,
                (float)Game.VirtualGameHeight + 0.1f));
            foreach (IGameObject obj in gameObjects)
                quadtree.Insert(obj);
        }

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

        //Повикај OnUpdate на IGameObject obj. Тривијален метод.
        private void UpdateObject(IGameObject obj)
        {
            if (obj == null)
                return;
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
