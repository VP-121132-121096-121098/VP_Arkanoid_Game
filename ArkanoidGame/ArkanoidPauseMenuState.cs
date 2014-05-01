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
        
        private IDictionary<string, GameBitmap> menuOptions1; //различни опции во менито
        private IDictionary<string, GameBitmap> readyStrings1; //стрингови

        public IList<IList<GameBitmap>> BitmapsToRender { get; private set; } //слики од позадината и сите опции заедно
        private IList<IList<GameBitmap>> bitmapsToRenderCopy; /* на Draw се праќа копија од листа
                                                               * за да се избегнат проблемите со нејзино модифицирање
                                                               */

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            Game.Renderer.Render(bitmapsToRenderCopy, graphics, frameWidth, frameHeight);
        }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            Point cursor = Game.CursorIngameCoordinates;

            GameBitmap startNewGame = menuOptions1["start new game"];

            if (MenuOptionHover(cursor, startNewGame))
            {
                BitmapsToRender[1][0] = readyStrings1["start new game hover"];
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
                BitmapsToRender[1][0] = readyStrings1["start new game"];
            }
            GameBitmap resumeGame = menuOptions1["resume game"];
            if (MenuOptionHover(cursor, resumeGame))
            {
                BitmapsToRender[1][1] = readyStrings1["resume game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    Game.GameState = gamePlayState;
                }
            }
            else
            {
                BitmapsToRender[1][1] = readyStrings1["resume game"];
            }
       /*  GameBitmap quitGame = menuOptions1["quit game"];
            if (MenuOptionHover(cursor, quitGame))
            {
                BitmapsToRender[1][1] = readyStrings1["quit game hover"];
                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    return 0;
                }
            }
            else
            {
                BitmapsToRender[1][1] = readyStrings1["quit game"];
            }

           GameBitmap controls = menuOptions1["controls"];
            if (MenuOptionHover(cursor, controls))
            {

                if (Game.IsControllerMouse)
                {
                    BitmapsToRender[1][2] = readyStrings1["Game controls mouse hover"];
                }
                else
                {
                    BitmapsToRender[1][2] = readyStrings1["Game controls keyboard hover"];
                }


                if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                {
                    //Ако се притисне глушецот на quit game тогаш излези нормално
                    if (BitmapsToRender[1][2] == readyStrings1["Game controls mouse hover"])
                    {
                        BitmapsToRender[1][2] = readyStrings1["Game controls keyboard hover"];
                        Game.IsControllerMouse = false;
                    }
                    else
                    {
                        BitmapsToRender[1][2] = readyStrings1["Game controls mouse hover"];
                        Game.IsControllerMouse = true;
                    }
                }
            }
            else if (Game.IsControllerMouse)
            {
                BitmapsToRender[1][2] = readyStrings1["Game controls mouse"];
            }
            else
            {
                BitmapsToRender[1][2] = readyStrings1["Game controls keyboard"];
            }

         */   //Посебна readonly копија за рендерерот
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
            readyStrings1 = new Dictionary<string, GameBitmap>();

            

            readyStrings1.Add("start new game", new GameBitmap(StaticStringFactory.CreateOrangeString("start new game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));
            readyStrings1.Add("start new game hover", new GameBitmap(StaticStringFactory.CreateBlueString("start new game"),
                (game.VirtualGameWidth - 550) / 2, 750, 600, 90));

            readyStrings1.Add("resume game", new GameBitmap(StaticStringFactory.CreateOrangeString("resume game"),
                (game.VirtualGameWidth - 450) / 2, 600, 500, 90));
            readyStrings1.Add("resume game hover", new GameBitmap(StaticStringFactory.CreateBlueString("resume game"),
               (game.VirtualGameWidth - 450) / 2, 600, 500, 90));
            
         /*  readyStrings1.Add("quit game", new GameBitmap(StaticStringFactory.CreateOrangeString("quit game"),
             (game.VirtualGameWidth - 550) / 2, 850, 600, 90));
            readyStrings1.Add("quit game hover", new GameBitmap(StaticStringFactory.CreateBlueString("quit game"),
                (game.VirtualGameWidth - 550) / 2, 850, 600, 90));
            
            readyStrings1.Add("Game controls mouse", new GameBitmap(StaticStringFactory
                .CreateOrangeString("Controls: mouse"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            readyStrings1.Add("Game controls mouse hover", new GameBitmap(StaticStringFactory
                .CreateBlueString("Controls: mouse"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));

            readyStrings1.Add("Game controls keyboard", new GameBitmap(StaticStringFactory
                .CreateOrangeString("Controls: keyboard"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            readyStrings1.Add("Game controls keyboard hover", new GameBitmap(StaticStringFactory
                .CreateBlueString("Controls: keyboard"), (game.VirtualGameWidth - 750) / 2.0, 920,
                750, 90));
            */
            
            menuOptions1 = new Dictionary<string, GameBitmap>();
            menuOptions1.Add("start new game", readyStrings1["start new game"]);
            menuOptions1.Add("resume game", readyStrings1["resume game"]);
          // menuOptions1.Add("quit game", readyStrings1["quit game"]);
          //  menuOptions1.Add("controls", readyStrings1["Game controls mouse"]);

            BitmapsToRender.Add(new List<GameBitmap>());
            BitmapsToRender[1].Add(menuOptions1["start new game"]);
            BitmapsToRender[1].Add(menuOptions1["resume game"]);
          //  BitmapsToRender[1].Add(menuOptions1["quit game"]);
         //   BitmapsToRender[1].Add(menuOptions1["controls"]);

            this.Game = game;
            Game.IsControllerMouse = true;
        }

        /// <summary>
        /// времето во играта во меѓувреме си тече нормално
        /// </summary>
        public bool IsTimesynchronizationImportant
        {
            get { return true; }
        }
    }
}
