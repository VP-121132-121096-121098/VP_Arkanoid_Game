using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArkanoidGame
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

        public void OnUpdate(IEnumerable<IGameObject> gameObjects, long gameElapsedTime)
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
            obj.OnDraw(graphics, frameWidth, frameHeight);
        }

        private IGameObject obj;

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
            this.gameUpdatePeriod = gameUpdatePeriod;
            GameState = initialState;
            Name = "Arkanoid";
            VirtualGameWidth = 3840;
            VirtualGameHeight = 2160;
            obj = new PlayerPaddle(new Vector2D(1750, 2010), gameUpdatePeriod, VirtualGameWidth, VirtualGameHeight);
        }

        public void OnUpdate()
        {
            ElapsedTime++; //поминал еден период
            GameState.OnUpdate(null, ElapsedTime);
            obj.OnUpdate(null, ElapsedTime);
        }

        public long ElapsedTime { get; set; }


        /* Играта ќе има посебни единици за должина, посебни просторот за цртање.
         * Играта е правоаголник со димензии 3840 x 2160 кои се преведуваат во 
         * координати за прикажување на екран во зависност од димензиите на прозорецот.
         */
        public int VirtualGameWidth { get; private set; }

        public int VirtualGameHeight { get; private set; }
    }
}
