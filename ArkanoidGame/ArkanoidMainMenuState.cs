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

        private IDictionary<string, GameBitmap> menuOptions; /*Листа од опции. Бидејќи постојат плави и портокалови
                                                              * опции, ова може да се искористи за да се добијат 
                                                              * координатите на дадена опција. Пример за start game
                                                              * без разлика дали е со плава или со портокалова боја
                                                              * овде ќе стојат истите координати
                                                              */

        private IDictionary<string, GameBitmap> readyStrings; /* Бафер на сликите во кои се чува текстот од опциите.
                                                              * Бидејќи RendererCache не ги брише автоматски
                                                              * сликите од RAM, корисно е да се чува листа на сите веќе
                                                              * креирани слики со цел нивно реискористување, а потоа 
                                                              * кога нема да требаат може рачно да се повика RenderCache.Remove(со
                                                              * ID на сликата. Наместо да се креира на секое Start new game и
                                                              * Start new game hover, од овде може да се искористи слика
                                                              * што веќе е во меморија :). Се избегнува и читање од хард диск
                                                              * и губење на циклуси во бришење на старите и непотребно скалирање
                                                              * на новата слика во потребните димензии.
                                                              */

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; } //слики од позадината и сите опции заедно
        private IList<IList<GameBitmap>> bitmapsToRenderCopy; /* на Draw се праќа копија од листа
                                                               * за да се избегнат проблемите со нејзино модифицирање
                                                               * бидејќи Draw е на посебен Thread.
                                                               */

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight, bool lowSpec)
        {
            Game.Renderer.Render(bitmapsToRenderCopy, graphics, frameWidth, frameHeight, lowSpec);
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

            GameBitmap graphicDetails = menuOptions["graphic details"];
            if (MenuOptionHover(cursor, graphicDetails))
            {
                if (Game.GraphicDetails == GraphicDetails.Low)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: low hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        Game.GraphicDetails = GraphicDetails.High;
                    }
                }
                else if (Game.GraphicDetails == GraphicDetails.High)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: high hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        Game.GraphicDetails = GraphicDetails.VeryHigh;
                    }
                }
                else if (Game.GraphicDetails == GraphicDetails.VeryHigh)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: very high hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        Game.GraphicDetails = GraphicDetails.Low;
                    }
                }
            }
            else
            {
                if (Game.GraphicDetails == GraphicDetails.Low)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: low"];
                }
                else if (Game.GraphicDetails == GraphicDetails.High)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: high"];
                }
                else if (Game.GraphicDetails == GraphicDetails.VeryHigh)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: very high"];
                }
            }
            BitmapsToRender[1][2] = menuOptions["graphic details"];

            GameBitmap controls = menuOptions["controls"];
            if (MenuOptionHover(cursor, controls))
            {

                if (Game.IsControllerMouse)
                {
                    BitmapsToRender[1][3] = readyStrings["Game controls mouse hover"];
                }
                else
                {
                    BitmapsToRender[1][3] = readyStrings["Game controls keyboard hover"];
                }


                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    if (BitmapsToRender[1][3] == readyStrings["Game controls mouse hover"])
                    {
                        BitmapsToRender[1][3] = readyStrings["Game controls keyboard hover"];
                        Game.IsControllerMouse = false;
                    }
                    else
                    {
                        BitmapsToRender[1][3] = readyStrings["Game controls mouse hover"];
                        Game.IsControllerMouse = true;
                    }
                }
            }
            else if (Game.IsControllerMouse)
            {
                BitmapsToRender[1][3] = readyStrings["Game controls mouse"];
            }
            else
            {
                BitmapsToRender[1][3] = readyStrings["Game controls keyboard"];
            }

            //Направи посебна readonly копија за рендерерот (за да се избегнат проблеми со синхронизација,
            //пример модифицирање на листаta BitmapsToRender од страна на update thread-от).
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

            return 100;
        }

        private static bool MenuOptionHover(Point cursor, GameBitmap menuOptionBitmap)
        {
            return (cursor.X >= menuOptionBitmap.PositionUL.X
                && cursor.X <= menuOptionBitmap.PositionUR.X
                            && cursor.Y >= menuOptionBitmap.PositionUL.Y
                            && cursor.Y <= menuOptionBitmap.PositionDR.Y);
        }

        /// <summary>
        /// Слика која ќе се прикажува како позадина на менито
        /// </summary>
        public Bitmap MenuBackground { get; private set; }

        public IGame Game { get; set; }

        public ArkanoidMainMenuState(IGame game)
        {
            RendererCache.RemoveAllBitmapsFromMainMemory();

            this.Game = game;
            Game.IsControllerMouse = true;

            MenuBackground = null;
            BitmapsToRender = new List<IList<GameBitmap>>();
            BitmapsToRender.Add(new List<GameBitmap>());
            BitmapsToRender[0].Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
                game.VirtualGameHeight));
            bitmapsToRenderCopy = new List<IList<GameBitmap>>();

            // додади ги сите опции во меморија
            readyStrings = new Dictionary<string, GameBitmap>();
            readyStrings.Add("start game", new GameBitmap(StaticStringFactory.CreateOrangeString("start game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("quit game", new GameBitmap(StaticStringFactory.CreateOrangeString("quit game"),
                (game.VirtualGameWidth - 500) / 2, 1270, 550, 90));
            readyStrings.Add("start game hover", new GameBitmap(StaticStringFactory.CreateBlueString("start game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("quit game hover", new GameBitmap(StaticStringFactory.CreateBlueString("quit game"),
                (game.VirtualGameWidth - 500) / 2, 1270, 550, 90));
            readyStrings.Add("Game controls mouse", new GameBitmap(StaticStringFactory
                .CreateOrangeString("Controls: mouse"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            readyStrings.Add("Game controls mouse hover", new GameBitmap(StaticStringFactory
                .CreateBlueString("Controls: mouse"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));

            //остави ги деталите онака како што се поставени
            //Game.GraphicDetails = GraphicsDetails.Low;
            readyStrings.Add("Graphic details: low", new GameBitmap(
                StaticStringFactory.CreateOrangeString("Graphic details: low"), (game.VirtualGameWidth - 900) / 2.0,
                1100, 900, 90));
            readyStrings.Add("Graphic details: low hover", new GameBitmap(
                StaticStringFactory.CreateBlueString("Graphic details: low"), (game.VirtualGameWidth - 900) / 2.0,
                1100, 900, 90));
            readyStrings.Add("Graphic details: high", new GameBitmap(
                StaticStringFactory.CreateOrangeString("Graphic details: high"), (game.VirtualGameWidth - 910) / 2.0,
                1100, 910, 90));
            readyStrings.Add("Graphic details: high hover", new GameBitmap(
                StaticStringFactory.CreateBlueString("Graphic details: high"), (game.VirtualGameWidth - 910) / 2.0,
                1100, 910, 90));
            readyStrings.Add("Graphic details: very high", new GameBitmap(
                StaticStringFactory.CreateOrangeString("Graphic details: very high"), (game.VirtualGameWidth - 1000) / 2.0,
                1100, 1000, 90));
            readyStrings.Add("Graphic details: very high hover", new GameBitmap(
                StaticStringFactory.CreateBlueString("Graphic details: very high"), (game.VirtualGameWidth - 1000) / 2.0,
                1100, 1000, 90));

            readyStrings.Add("Game controls keyboard", new GameBitmap(StaticStringFactory
                .CreateOrangeString("Controls: keyboard"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            readyStrings.Add("Game controls keyboard hover", new GameBitmap(StaticStringFactory
                .CreateBlueString("Controls: keyboard"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));

            //прикажи ги опциите и на слаб хардвер.
            foreach (KeyValuePair<string, GameBitmap> bitmap in readyStrings)
            {
                bitmap.Value.DrawLowSpec = true;
            }

            menuOptions = new Dictionary<string, GameBitmap>();
            menuOptions.Add("start game", readyStrings["start game"]);
            menuOptions.Add("quit game", readyStrings["quit game"]);
            menuOptions.Add("controls", readyStrings["Game controls mouse"]);
            menuOptions.Add("graphic details", readyStrings["Graphic details: low"]);

            BitmapsToRender.Add(new List<GameBitmap>());
            BitmapsToRender[1].Add(menuOptions["start game"]);
            BitmapsToRender[1].Add(menuOptions["quit game"]);
            BitmapsToRender[1].Add(menuOptions["controls"]);
            BitmapsToRender[1].Add(menuOptions["graphic details"]);
        }

        public bool IsTimesynchronizationImportant
        {
            get { return false; }
        }
    }
}
