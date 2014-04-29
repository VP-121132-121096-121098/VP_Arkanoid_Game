using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class SmallPurpleBrick : AbstractBrick
    {
        //tezina inicijalno na 200 ,pola od tezinata na golemata ciglicka


        public override void OnUpdate(long gameElapsedTime)
        {
            /*if (MouseLastPosition == null)
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
            }*/


            Position += (Velocity) / 2;

            if (Position.X > GameWidth - 10 - ObjectWidth)
                Position.X = GameWidth - 10 - ObjectWidth;
            else if (Position.X < 5)
                Position.X = 5;

            ObjectTextures[0].X = Position.X;
            ObjectTextures[0].Y = Position.Y;
        }


        public override void InitTextures()
        {
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\element_purple_square.png", Position.X,
                Position.Y, ObjectWidth, ObjectHeight));
        }

        public SmallPurpleBrick(Vector2D positionVector, int virtualGameWidth, int virtualGameHeight)
            : base()
        {
            this.GameWidth = virtualGameWidth;
            this.GameHeight = virtualGameHeight;
            this.Position = new Vector2D(positionVector);
            ObjectWidth = 100;
            ObjectHeight = 80;
            Velocity = new Vector2D(0, 0);

            this.Weight = 200;
            this.InitTextures();
        }


    }
}

