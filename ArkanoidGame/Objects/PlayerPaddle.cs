using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public double PaddleWidth { get; set; }
        public double PaddleHeight { get; private set; }

        public int ObjectWidth { get; private set; }
        public int ObjectHeight { get; private set; }

        public void OnUpdate(long gameElapsedTime)
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

            if (Position.X > ObjectWidth - 10 - PaddleWidth)
                Position.X = ObjectWidth - 10 - PaddleWidth;
            else if (Position.X < 5)
                Position.X = 5;

            ObjectTextures[0].X = Position.X;
            ObjectTextures[0].Y = Position.Y;
        }

        public PlayerPaddle(Vector2D positionVector, int virtualGameWidth, int virutalGameHeight)
        {
            this.ObjectWidth = virtualGameWidth;
            this.ObjectHeight = virutalGameHeight;
            this.Position = new Vector2D(positionVector);
            PaddleWidth = 400;
            PaddleHeight = 85;
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\paddleRed.png", positionVector.X,
                positionVector.Y, 400, 85));
            velocity = new Vector2D(0, 0);
            maxVelocity = new Vector2D(70, 0);
            acceleration = new Vector2D(10, 0);
        }


        public void OnResolutionChanged(int newWidth, int newHeight)
        {
            /* бидејќи поради бонусите ширината на овој
             * објект може да се менува, оваа функционалност е 
             * имплементирана во OnDraw методот
             */
        }


        public IList<GameBitmap> ObjectTextures { get; private set; }
    }
}
