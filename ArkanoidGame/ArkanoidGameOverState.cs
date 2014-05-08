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
    public class ArkanoidGameOverState : IGameState
    {
        public ArkanoidGameOverState(IGame game, long playerScore)
        {
            //ќе има само позадина составена од повеќе слоеви
            //излез кон главното мени ќе биде на Escape, па нема потреба од ховер ефекти и слично

            RendererCache.RemoveAllBitmapsFromMainMemory();

            this.Game = game;
            this.BitmapsToRender = null;

            List<GameBitmap> background = new List<GameBitmap>();
            background.Add(new GameBitmap("\\Resources\\Images\\background.jpg", 0, 0, game.VirtualGameWidth,
              game.VirtualGameHeight));

            Bitmap gameOverText = StaticStringFactory.CreateOrangeString("Game over");
            GameBitmap gameOver = new GameBitmap(gameOverText, (Game.VirtualGameWidth - 500) / 2,
                (Game.VirtualGameHeight - 100) / 2 - 180, 500, 100);
            gameOver.DrawLowSpec = true;
            background.Add(gameOver);
            
            Bitmap scoreText = StaticStringFactory.CreateOrangeString("Score: " + playerScore);
            GameBitmap score = new GameBitmap(scoreText, (Game.VirtualGameWidth - 900) / 2,
               (Game.VirtualGameHeight - 100) / 2, 900, 150);
            score.DrawLowSpec = true;
            background.Add(score);

            BitmapsToRender = new List<IList<GameBitmap>>();
            BitmapsToRender.Add(background);
        }

        public void OnDraw(System.Drawing.Graphics graphics, int frameWidth, int frameHeight, bool lowSpec)
        {
            Game.Renderer.Render(BitmapsToRender, graphics, frameWidth, frameHeight, lowSpec);
        }

        public int OnUpdate(IList<IGameObject> gameObjects)
        {
            IKeyState escState = KeyStateInfo.GetAsyncKeyState(Keys.Escape);
            if (escState.IsPressed)
            {
                this.Game.GameState = new ArkanoidMainMenuState(this.Game);
            }

            return 100;
        }

        public IGame Game { get; private set; }

        public bool IsTimesynchronizationImportant { get { return false; } }

        public IList<IList<Renderer.GameBitmap>> BitmapsToRender { get; private set; }
    }
}
