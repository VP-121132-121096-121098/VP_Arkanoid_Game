using ArkanoidGame.Framework;
using ArkanoidGame.GameLogic;
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
        // V(t) = V0 + a * t
        // a = (V(t) - V0) / t
        // X(t) = X0 + V0 * t + a * t^2 / 2 //Добиена со интегрирање по t на првата формула
        // Од втората и третата следува X(t) = X0 + (V + V0) * t / 2

        private Vector2D velocity; /* брзината ќе биде изминат пат (% од ширината
                                    * на прозорецот) во секунда */
        private Vector2D maximumVelocity;
        private Vector2D acceleration;

        private long SecondInFileTime { get { return 10000000; } }

        private readonly Vector2D maxAcceleration;

        public void OnUpdate(IEnumerable<IGameObject> objects)
        {
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
            else if (lastFrameWidth != frameWidth || lastFrameHeight != frameHeight)
            {
                objectTexture = StaticBitmapFactory.GetBitmapFromMainMemory("PaddleTexture", width, height);
            }
            lastFrameWidth = frameWidth;
            lastFrameHeight = frameHeight;

            graphics.DrawImage(objectTexture, Position.X, Position.Y,
                width, height);
        }

        public Vector2D Position { get; set; }


        public float PaddleWidth { get; set; } //in %
        public float PaddleHeight { get; private set; } //in %

        private int lastFrameWidth;
        private int lastFrameHeight;

        private Bitmap objectTexture;

        public PlayerPaddle(int positionX, int positionY)
        {
            this.Position = new Vector2D(positionX, positionY);
            PaddleWidth = 9.7f;
            PaddleHeight = 3.5f;
            objectTexture = null;
            velocity = new Vector2D(0, 0);
            maximumVelocity = new Vector2D(90, 0);
            acceleration = new Vector2D(0, 0);
            maxAcceleration = new Vector2D(150, 0);
        }
    }
}
