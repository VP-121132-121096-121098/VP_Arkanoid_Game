using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.GameLogic
{
    public class ArkanoidStateMainMenu : IGameState
    {
        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            if (MenuBackground == null)
            {
                StaticBitmapFactory.LoadBitmapIntoMainMemory("\\Resources\\Images\\background.jpg", 
                    frameWidth, frameHeight, "MenuBackground");
                MenuBackground = StaticBitmapFactory.GetBitmapFromMainMemory("MenuBackground");
            }
            else if (MenuBackground.Height != frameHeight || MenuBackground.Width != frameWidth)
            {
                MenuBackground = StaticBitmapFactory.GetBitmapFromMainMemory("MenuBackground", frameWidth, frameHeight);
            }

            graphics.DrawImage(MenuBackground, 0, 0, frameWidth, frameHeight);
        }

        public void OnUpdate(IEnumerable<IGameObject> gameObjects)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Слика која ќе се прикажува како позадина на менито
        /// </summary>
        public Bitmap MenuBackground { get; private set; }

        public IGame Game { get; set; }

        public ArkanoidStateMainMenu(IGame game)
        {
            MenuBackground = null;
            this.Game = game;
        }
    }

    public class GameArkanoid : IGame
    {
        public string Name { get; private set; }

        public IGameState GameState { get; set; }

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            this.GameState.OnDraw(graphics, frameWidth, frameHeight);
        }

        /// <summary>
        /// Креира нова игра и ја поставува во почетна состојба initialState
        /// </summary>
        /// <param name="state"></param>
        public GameArkanoid(IGameState initialState)
        {
            GameState = initialState;
            Name = "Arkanoid";
        }


        public void OnUpdate()
        {
            GameState.OnUpdate(null);
        }
    }
}
