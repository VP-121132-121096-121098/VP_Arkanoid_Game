using ArkanoidGame.Framework;
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

        private IList<IList<GameBitmap>> bitmaps; //слики од позадината и сите опции заедно

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            Game.Renderer.Render(bitmaps, graphics, frameWidth, frameHeight);
        }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            Point cursor = Game.CursorIngameCoordinates;

            GameBitmap startGame = menuOptions["start game"];

            if (cursor.X >= startGame.X && cursor.X <= startGame.X + startGame.WidthInGameUnits
                && cursor.Y >= startGame.Y && cursor.Y <= startGame.Y + startGame.HeightInGameUnits)
            {
                bitmaps[1][0] = readyStrings["start game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    Game.GameState = new ArkanoidGamePlayState(Game);
                    Thread.Sleep(100);

                    foreach (KeyValuePair<string, GameBitmap> bitmap in readyStrings)
                    {
                        RendererCache.RemoveBitmapFromMainMemory(bitmap.Value.PictureID);
                    }

                    return 100;
                }
            }
            else
            {
                bitmaps[1][0] = readyStrings["start game"];
            }

            GameBitmap quitGame = menuOptions["quit game"];
            if (cursor.X >= quitGame.X && cursor.X <= quitGame.X + quitGame.WidthInGameUnits
                && cursor.Y >= quitGame.Y && cursor.Y <= quitGame.Y + quitGame.HeightInGameUnits)
            {
                bitmaps[1][1] = readyStrings["quit game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    return 0;
                }
            }
            else
            {
                bitmaps[1][1] = readyStrings["quit game"];
            }

            return 100;
        }

        /// <summary>
        /// Слика која ќе се прикажува како позадина на менито
        /// </summary>
        public Bitmap MenuBackground { get; private set; }

        public IGame Game { get; set; }

        public ArkanoidStateMainMenu(IGame game)
        {
            MenuBackground = null;
            bitmaps = new List<IList<GameBitmap>>();
            bitmaps.Add(new List<GameBitmap>());
            bitmaps[0].Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
                game.VirtualGameHeight));

            // додади ги сите опции во меморија
            readyStrings = new Dictionary<string, GameBitmap>();
            readyStrings.Add("start game", new GameBitmap(StaticStringFactory.CreateOrangeString("start game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("quit game", new GameBitmap(StaticStringFactory.CreateOrangeString("quit game"),
                (game.VirtualGameWidth - 500) / 2, 880, 550, 90));
            readyStrings.Add("start game hover", new GameBitmap(StaticStringFactory.CreateBlueString("start game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("quit game hover", new GameBitmap(StaticStringFactory.CreateBlueString("quit game"),
                (game.VirtualGameWidth - 500) / 2, 880, 550, 90));

            menuOptions = new Dictionary<string, GameBitmap>();
            menuOptions.Add("start game", readyStrings["start game"]);
            menuOptions.Add("quit game", readyStrings["quit game"]);

            bitmaps.Add(new List<GameBitmap>());
            bitmaps[1].Add(menuOptions["start game"]);
            bitmaps[1].Add(menuOptions["quit game"]);

            this.Game = game;
        }

        public bool IsTimesynchronizationImportant
        {
            get { return false; }
        }

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; }
    }

    public class ArkanoidGamePlayState : IGameState
    {
        public ArkanoidGamePlayState(IGame game)
        {
            this.Game = game;
            BitmapsToRender = new List<IList<GameBitmap>>();
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
            Game.Renderer.Render(BitmapsToRender, graphics, frameWidth, frameHeight);
        }

        public long ElapsedTime { get; private set; }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].OnUpdate(gameObjects, ElapsedTime);
                if (i + 1 >= BitmapsToRender.Count)
                    BitmapsToRender.Add(gameObjects[i].ObjectTextures);
                else
                    BitmapsToRender[i + 1] = gameObjects[i].ObjectTextures;
            }

            ElapsedTime++; //поминал еден период

            if (KeyStateInfo.GetAsyncKeyState(Keys.Escape).IsPressed)
            {
                return 0;
            }

            return 100;
        }

        public IGame Game { get; private set; }

        public bool IsTimesynchronizationImportant
        {
            get { return true; }
        }

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; }
    }

    public class GameArkanoid : IGame
    {
        //Стартна позиција на играчот (1750, 2010);

        private readonly object gameStateLock = new Object();

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
            this.GameState.OnDraw(graphics, frameWidth, frameHeight);
        }

        //во милисекунди
        private int gameUpdatePeriod;

        /// <summary>
        /// Креира нова игра и ја поставува во почетна состојба initialState.
        /// gameUpdatePeriod е во милисекунди и означува на колкав период
        /// се повикува методот update()
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="gameUpdatePeriod"></param>
        public GameArkanoid(IGameState initialState, int gameUpdatePeriod)
        {
            VirtualGameWidth = 3840;
            VirtualGameHeight = 2160;
            this.Renderer = new GameRenderer(VirtualGameWidth, VirtualGameHeight);
            CursorIngameCoordinates = Cursor.Position;
            this.gameUpdatePeriod = gameUpdatePeriod;
            GameState = initialState;
            Name = "Arkanoid";
            GameObjects = new List<IGameObject>();
        }

        public int OnUpdate(Point cursorPanelCoordinates)
        {
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
    }
}
