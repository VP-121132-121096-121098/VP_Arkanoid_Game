using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArkanoidGame.Objects
{
    public class PlayerPaddle : IGameObject
    {
        private float speedX;

        private long timeLastUpdate;

        public void OnUpdate(IEnumerable<IGameObject> objects)
        {
            if (timeLastUpdate < 0)
                timeLastUpdate = DateTime.Now.ToFileTimeUtc();

            long timeNow = DateTime.Now.ToFileTimeUtc();

            //update logic

            timeLastUpdate = timeNow;
        }

        public void OnDraw(System.Drawing.Graphics graphics, int frameWidth, int frameHeight)
        {

            int width = (int)Math.Round(frameWidth * PaddleWidth / 100);
            int height = (int)Math.Round(frameHeight * PaddleHeight / 100);

            if (objectTexture == null)
            {
                StaticBitmapFactory.LoadBitmapIntoMainMemory("//Resources//Images//paddleRed.png",
                    width, height, "PaddleTexture");
                objectTexture = StaticBitmapFactory.GetBitmapFromMainMemory("PaddleTexture");
            }
            else if (previousFrameWidth != frameWidth || previousFrameHeight != frameHeight)
            {
                objectTexture = StaticBitmapFactory.GetBitmapFromMainMemory("PaddleTexture", width, height);
            }
            previousFrameWidth = frameWidth;
            previousFrameHeight = frameHeight;

            graphics.DrawImage(objectTexture, PositionX, PositionY,
                width, height);
        }

        public int PositionX { get; set; }

        public int PositionY { get; set; }


        public float PaddleWidth { get; set; } //in %
        public float PaddleHeight { get; private set; } //in %

        private int previousFrameWidth;
        private int previousFrameHeight;

        private Bitmap objectTexture;

        public PlayerPaddle(int positionX, int positionY)
        {
            this.PositionX = positionX;
            this.PositionY = positionY;
            PaddleWidth = 9.7f;
            PaddleHeight = 3.5f;
            objectTexture = null;
            speedX = 0;
            timeLastUpdate = -1;
        }
    }
}
