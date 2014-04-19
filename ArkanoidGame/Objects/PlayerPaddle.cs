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


        public void OnUpdate()
        {
            IKeyState leftArrowState = KeyStateInfo.GetAsyncKeyState(Keys.Left);
            if (leftArrowState.IsPressed || leftArrowState.WasPressedAfterPreviousCall)
            {
                speedX = (float)Math.Max(-2.5, speedX - 0.2);
            }

            IKeyState rightArrowState = KeyStateInfo.GetAsyncKeyState(Keys.Right);
            if (rightArrowState.IsPressed || rightArrowState.WasPressedAfterPreviousCall)
            {
                speedX = (float)Math.Min(2.5, speedX + 0.2);
            }

            if (!rightArrowState.IsPressed && !rightArrowState.WasPressedAfterPreviousCall &&
                !leftArrowState.IsPressed && !leftArrowState.WasPressedAfterPreviousCall)
            {
                if (speedX < 0)
                {
                    speedX = (int)Math.Min(0, speedX + 0.2);
                }
                else if (speedX > 0)
                {
                    speedX = (int)Math.Max(0, speedX - 0.2);
                }
            }

            if (speedX < 0 && PositionX + (int)Math.Round(speedX * 2.5) <= 5)
            {
                PositionX = 5;
            }
            if (speedX > 0 && PositionX + (int)Math.Round(speedX * 2.5) >= previousFrameWidth
                - (int)Math.Round(previousFrameWidth * PaddleWidth / 100) - 5)
            {
                PositionX = previousFrameWidth
                - (int)Math.Round(previousFrameWidth * PaddleWidth / 100) - 5;
            }
            else
            {
                this.PositionX += (int)Math.Round(speedX * 2.5);
            }
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
        }
    }
}
