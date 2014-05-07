using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
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
    public class ArkanoidPauseMenuState : IGameState
    {
        
        private IDictionary<string, GameBitmap> menuOptions; //различни опции во менито
        private IDictionary<string, GameBitmap> readyStrings; //стрингови

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; } //слики од позадината и сите опции заедно
        private IList<IList<GameBitmap>> bitmapsToRenderCopy; /* на Draw се праќа копија од листа
                                                               * за да се избегнат проблемите со нејзино модифицирање
                                                               */

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight, bool lowSpec)
        {
            Game.Renderer.Render(bitmapsToRenderCopy, graphics, frameWidth, frameHeight, lowSpec);
        }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            Point cursor = Game.CursorIngameCoordinates;

            GameBitmap startNewGame = menuOptions["start new game"];

            if (MenuOptionHover(cursor, startNewGame))
            {
                BitmapsToRender[1][1] = readyStrings["start new game hover"];
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
                BitmapsToRender[1][1] = readyStrings["start new game"];
            }

            GameBitmap resumeGame = menuOptions["resume game"];
            if (MenuOptionHover(cursor, resumeGame))
            {
                BitmapsToRender[1][0] = readyStrings["resume game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    Game.GameState = gamePlayState;
                }
            }
            else
            {
                BitmapsToRender[1][0] = readyStrings["resume game"];
            }


           GameBitmap quitGame = menuOptions["quit game"];
            if (MenuOptionHover(cursor, quitGame))
            {
                BitmapsToRender[1][2] = readyStrings["quit game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    return 0;
                }
            }
            else
            {
                BitmapsToRender[1][2] = readyStrings["quit game"];
            }


            GameBitmap graphicDetails = menuOptions["graphic details"];
            if (MenuOptionHover(cursor, graphicDetails))
            {
                if (Game.GraphicDetails == GraphicsDetails.Low)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: low hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        Game.GraphicDetails = GraphicsDetails.High;
                    }
                }
                else if (Game.GraphicDetails == GraphicsDetails.High)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: high hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        Game.GraphicDetails = GraphicsDetails.VeryHigh;
                    }
                }
                else if (Game.GraphicDetails == GraphicsDetails.VeryHigh)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: very high hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        Game.GraphicDetails = GraphicsDetails.Low;
                    }
                }
            }
            else
            {
                if (Game.GraphicDetails == GraphicsDetails.Low)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: low"];
                }
                else if (Game.GraphicDetails == GraphicsDetails.High)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: high"];
                }
                else if (Game.GraphicDetails == GraphicsDetails.VeryHigh)
                {
                    menuOptions["graphic details"] = readyStrings["Graphic details: very high"];
                }
            }
            BitmapsToRender[1][3] = menuOptions["graphic details"];
        

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

        private IGameState gamePlayState;

        /// <summary>
        /// gamePlayState е за да може да се вратиме во игра од resume
        /// </summary>
        /// <param name="game"></param>
        /// <param name="gamePlayState"></param>
        public ArkanoidPauseMenuState(IGame game, IGameState gamePlayState)
        {
            this.gamePlayState = gamePlayState;
            MenuBackground = null;
            BitmapsToRender = new List<IList<GameBitmap>>();
            BitmapsToRender.Add(new List<GameBitmap>());
            BitmapsToRender[0].Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
                game.VirtualGameHeight));
            bitmapsToRenderCopy = new List<IList<GameBitmap>>();

            // додади ги сите опции во меморија
            // додади ги сите опции во меморија
            readyStrings = new Dictionary<string, GameBitmap>();


            readyStrings.Add("resume game", new GameBitmap(StaticStringFactory.CreateOrangeString("resume game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings.Add("resume game hover", new GameBitmap(StaticStringFactory.CreateBlueString("resume game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));


            readyStrings.Add("start new game", new GameBitmap(StaticStringFactory.CreateOrangeString("start new game"),
                (game.VirtualGameWidth - 750) / 2.0, 920, 750, 90));
            readyStrings.Add("start new game hover", new GameBitmap(StaticStringFactory.CreateBlueString("start new game"),
                (game.VirtualGameWidth - 750) / 2.0, 920, 750, 90));


            readyStrings.Add("quit game", new GameBitmap(StaticStringFactory.CreateOrangeString("quit game"),
             (game.VirtualGameWidth - 500) / 2, 1270, 550, 90));
            readyStrings.Add("quit game hover", new GameBitmap(StaticStringFactory.CreateBlueString("quit game"),
               (game.VirtualGameWidth - 500) / 2, 1270, 550, 90));


            //постави графика на low
//            Game.GraphicDetails = GraphicsDetails.Low;
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
        
            menuOptions.Add("resume game", readyStrings["resume game"]);
            menuOptions.Add("start new game", readyStrings["start new game"]);
            menuOptions.Add("quit game", readyStrings["quit game"]);
            menuOptions.Add("graphic details", readyStrings["Graphic details: low"]);
        

            BitmapsToRender.Add(new List<GameBitmap>());
           
            BitmapsToRender[1].Add(menuOptions["resume game"]);
            BitmapsToRender[1].Add(menuOptions["start new game"]);
            BitmapsToRender[1].Add(menuOptions["quit game"]);
            BitmapsToRender[1].Add(menuOptions["graphic details"]);
      
            this.Game = game;
            Game.IsControllerMouse = true;
        }

        /// <summary>
        /// Нема потреба од сихнронизација, додека играчот е на пауза во играта
        /// не се случува ништо. (Не е ова multiplayer)
        /// </summary>
        public bool IsTimesynchronizationImportant
        {
            get { return false; }
        }
    }
}
