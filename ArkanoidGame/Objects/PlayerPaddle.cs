using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
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

        public Vector2D Velocity { get; private set; } /* брзината ќе биде изминат пат (виртуелни единици во секунда) */
        private Vector2D maxVelocity;
        private Vector2D maxAcceleration; /* забрзување -> виртуелна единица во секунда на квадрат */

        private Point MouseLastPosition;

        public Vector2D Position { get; set; }

        public double ObjectWidth { get; set; }
        public double ObjectHeight { get; private set; }

        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }

        public void OnUpdate(long gameElapsedTime)
        {
            if (MouseLastPosition == null)
            {
                MouseLastPosition = GameArkanoid.GetInstance().CursorIngameCoordinates;
            }

            IKeyState leftArrowState = KeyStateInfo.GetAsyncKeyState(Keys.Left);
            IKeyState rightArrowState = KeyStateInfo.GetAsyncKeyState(Keys.Right);

            Vector2D velocity_0 = Velocity;

            if (!GameArkanoid.GetInstance().IsControllerMouse)
            {
                ReadKeyboardInput(leftArrowState, rightArrowState);
            }
            else
            {
                ReadMouseInput(GameArkanoid.GetInstance().CursorIngameCoordinates);
            }


            Position += (Velocity + velocity_0) / 2;

            if (Position.X > GameWidth - 10 - ObjectWidth)
                Position.X = GameWidth - 10 - ObjectWidth;
            else if (Position.X < 5)
                Position.X = 5;

            ObjectTextures[0].X = Position.X;
            ObjectTextures[0].Y = Position.Y;
        }

        private void ReadMouseInput(Point cursor)
        {
            Velocity = new Vector2D(cursor.X - (Position.X + Position.X + ObjectWidth) / 2.0, 0) / 10.0;
        }

        private void ReadKeyboardInput(IKeyState leftArrowState, IKeyState rightArrowState)
        {
            if (leftArrowState.IsPressed && rightArrowState.IsPressed)
            {
                if (Velocity.Magnitude() < maxAcceleration.Magnitude() * 1.5)
                    Velocity = new Vector2D(0, 0);
                else if (Velocity.X > 0)
                    Velocity -= 5 * maxAcceleration;
                else if (Velocity.X < 0)
                    Velocity += 5 * maxAcceleration;
            }
            else if (leftArrowState.IsPressed)
            {
                if (Velocity.X > 0)
                {
                    this.Velocity -= 5 * maxAcceleration;
                    if (this.Velocity.X < 0)
                        this.Velocity.X = 0;
                }
                else
                {
                    this.Velocity -= maxAcceleration;
                }

                if (Velocity.Magnitude() > maxVelocity.Magnitude())
                {
                    Velocity = -maxVelocity;
                }
            }
            else if (rightArrowState.IsPressed)
            {

                if (Velocity.X < 0)
                {
                    this.Velocity += 5 * maxAcceleration;
                    if (this.Velocity.X > 0)
                        this.Velocity.X = 0;
                }
                else
                {
                    this.Velocity += maxAcceleration;
                }


                if (Velocity.Magnitude() > maxVelocity.Magnitude())
                {
                    Velocity = maxVelocity;
                }
            }
            else if (!rightArrowState.IsPressed && !leftArrowState.IsPressed)
            {
                if (Velocity.Magnitude() < maxAcceleration.Magnitude())
                    Velocity = new Vector2D(0, 0);
                else if (Velocity.X > 0)
                    Velocity -= maxAcceleration;
                else if (Velocity.X < 0)
                    Velocity += maxAcceleration;
            }
        }

        private void InitTextures()
        {
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\paddleRed.png", Position.X,
                Position.Y, ObjectWidth, ObjectHeight));
        }

        public PlayerPaddle(Vector2D positionVector, int virtualGameWidth, int virutalGameHeight)
        {
            this.GameWidth = virtualGameWidth;
            this.GameHeight = virutalGameHeight;
            this.Position = new Vector2D(positionVector);
            ObjectWidth = 400;
            ObjectHeight = 85;
            Velocity = new Vector2D(0, 0);
            maxVelocity = new Vector2D(75, 0);
            maxAcceleration = new Vector2D(2, 0);

            this.InitTextures();
        }


        public IList<GameBitmap> ObjectTextures { get; private set; }

        public bool IsBall { get { return false; } }
        public bool IsPlayerPaddle { get { return true; } }

        public IList<IGeometricShape> GetGeometricShape()
        {
            throw new NotImplementedException();
        }

        public void OnCollisionDetected(IList<IGameObject> collidingObjects)
        {
            throw new NotImplementedException();
        }

        public RectangleF Rectangle
        {
            get { return new RectangleF((float)Position.X, (float)Position.Y, 
                (float)ObjectWidth, (float)ObjectHeight); }
        }
    }
}
