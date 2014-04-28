using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public class ArkanoidMainMenuState : IGameState
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

        public ArkanoidMainMenuState(IGame game)
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
}
