using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGame.Objects
{
    public class PlayerPaddle : IGameObject
    {
        // брзина V, забрзување a, време t, координата X, почетна брзина V0...
        // Тогаш
        // V(t) = V0 + a * dt
        // a = (V(t) - V0) / dt 
        // X(t) = X0 + V0 * dt + a * dt^2 / 2 //Добиена со интегрирање по t на првата формула
        // Од втората и третата следува X(t) = X0 + (V + V0) * dt / 2
        // Во оваа игра секогаш dt = 1, бидејќи времето се смета како број на повикувања
        // на update. (број на gameUpdatePeriods)

        private Vector2D velocity; /* брзината ќе биде изминат пат (виртуелни единици во секунда) */
        private Vector2D maxVelocity;
        private Vector2D acceleration; /* забрзување -> виртуелна единица во секунда на квадрат */

        public Vector2D Position { get; set; }

        private int gameUpdatePeriod; //во милисекунди

        private double secondIngameUpdatePeriod;

        public double PaddleWidth { get; set; }
        public double PaddleHeight { get; private set; }

        private int virtualGameWidth;
        private int virtualGameHeight;

        private Bitmap objectTexture;

        private int lastFrameWidth, lastFrameHeight;

        public void OnUpdate(IEnumerable<IGameObject> objects, long gameElapsedTime)
        {

            IKeyState leftArrowState = KeyStateInfo.GetAsyncKeyState(Keys.Left);
            IKeyState rightArrowState = KeyStateInfo.GetAsyncKeyState(Keys.Right);

            Vector2D velocity_0 = velocity;

            if (leftArrowState.IsPressed && rightArrowState.IsPressed)
            {
                if (velocity.Magnitude() < acceleration.Magnitude() * 1.5)
                    velocity = new Vector2D(0, 0);
                else if (velocity.X > 0)
                    velocity -= 1.5 * acceleration;
                else if (velocity.X < 0)
                    velocity += 1.5 * acceleration;
            }
            else if (leftArrowState.IsPressed)
            {
                this.velocity -= acceleration;
                if (velocity.X > 0)
                {
                    this.velocity -= acceleration;
                }
                if (velocity.Magnitude() > maxVelocity.Magnitude())
                {
                    velocity = -maxVelocity;
                }
            }
            else if (rightArrowState.IsPressed)
            {
                this.velocity += acceleration;
                if (velocity.X < 0)
                {
                    this.velocity += acceleration;
                }
                if (velocity.Magnitude() > maxVelocity.Magnitude())
                {
                    velocity = maxVelocity;
                }
            }
            else if (!rightArrowState.IsPressed && !leftArrowState.IsPressed)
            {
                if (velocity.Magnitude() < acceleration.Magnitude())
                    velocity = new Vector2D(0, 0);
                else if (velocity.X > 0)
                    velocity -= acceleration;
                else if (velocity.X < 0)
                    velocity += acceleration;
            }


            Position += (velocity + velocity_0) / 2;

            if (Position.X > virtualGameWidth - 10 - PaddleWidth)
                Position.X = virtualGameWidth - 10 - PaddleWidth;
            else if (Position.X < 5)
                Position.X = 5;

            //collision detection with ball
            //to be implemented
        }

        public void OnDraw(System.Drawing.Graphics graphics, int frameWidth, int frameHeight)
        {
            double realPositionX = 0, realPositionY = 0;
            GameUnitConversion.ConvertGameUnits(Position.X, Position.Y, out realPositionX, out realPositionY,
                virtualGameWidth, virtualGameHeight, frameWidth, frameHeight);

            int width = (int)Math.Round(GameUnitConversion.ConvertLength(
                PaddleWidth, virtualGameWidth, virtualGameHeight, frameWidth, frameHeight));
            int height = (int)Math.Round(GameUnitConversion.ConvertLength(
                PaddleHeight, virtualGameWidth, virtualGameHeight, frameWidth, frameHeight));

            if (lastFrameWidth == -1 || lastFrameHeight == -1)
            {
                StaticBitmapFactory.LoadBitmapIntoMainMemory("\\Resources\\Images\\paddleRed.png",
                    width, height, "PlayerPaddle");
                objectTexture = StaticBitmapFactory.GetBitmapFromMainMemory("PlayerPaddle");
            }
            else
            {
                objectTexture = StaticBitmapFactory.GetBitmapFromMainMemory("PlayerPaddle");
                if (objectTexture.Width != width || objectTexture.Height != height)
                {
                    StaticBitmapFactory.ResizeBitmap("PlayerPaddle", width, height);
                    objectTexture = StaticBitmapFactory.GetBitmapFromMainMemory("PlayerPaddle");
                }
            }

            graphics.DrawImage(objectTexture, (float)realPositionX, (float)realPositionY, width, height);
            lastFrameWidth = frameWidth;
            lastFrameHeight = frameHeight;
        }

        public PlayerPaddle(Vector2D positionVector, int gameUpdatePeriod,
            int virtualGameWidth, int virutalGameHeight)
        {
            this.virtualGameWidth = virtualGameWidth;
            this.virtualGameHeight = virutalGameHeight;
            this.Position = new Vector2D(positionVector);
            this.gameUpdatePeriod = gameUpdatePeriod;
            PaddleWidth = 380;
            PaddleHeight = 85;
            objectTexture = null;
            velocity = new Vector2D(0, 0);
            maxVelocity = new Vector2D(70, 0);
            acceleration = new Vector2D(10, 0);
            this.secondIngameUpdatePeriod = 1000.0f / gameUpdatePeriod;
            lastFrameHeight = lastFrameWidth = -1;
        }


        public void OnResolutionChanged(int newWidth, int newHeight)
        {
            /* бидејќи поради бонусите ширината на овој
             * објект може да се менува, оваа функционалност е 
             * имплементирана во OnDraw методот
             */
        }
    }
}
