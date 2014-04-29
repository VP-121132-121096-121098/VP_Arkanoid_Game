using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class BigRedBrick:ArkanoidGame.Interfaces.IGameObject
    {
        //tezina inicijalno na 400 pa ako e pogodena ednas,se namaluva na pola 
        public int Weight
        {
            get;
            set;
        }
        public System.Drawing.RectangleF Rectangle
        {
            get { return GetGeometricShape().GetBoundingRectangle; }
        }

        public Interfaces.IGeometricShape GetGeometricShape()
        {
            return new GameRectangle(this.Position, this.Position + new Vector2D(this.ObjectWidth, 0),
               this.Position + new Vector2D(0, this.ObjectHeight));
        }

        public void OnUpdate(long gameElapsedTime)
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

        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }

        public Geometry.Vector2D Position
        {
            get;
            set;
        }

        public double ObjectWidth
        {
            get;
            private set;
        }

        public double ObjectHeight
        {
            get;
            private set;
        }

        public IList<Renderer.GameBitmap> ObjectTextures
        {
            get;
            private set;
        }

        public Geometry.Vector2D Velocity
        {
            get;
            private set;
        }

        public Interfaces.GameObjectType ObjectType
        {
            get { return GameObjectType.RedBrick; }
        }

        private void InitTextures()
        {
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\element_red_rectangle.png", Position.X,
                Position.Y, ObjectWidth, ObjectHeight));
        }

        public BigRedBrick(Vector2D positionVector,int virtualGameWidth,int virtualGameHeight)
        {
            this.GameWidth = virtualGameWidth;
            this.GameHeight = virtualGameHeight;
            this.Position = new Vector2D(positionVector);
            ObjectWidth = 180;
            ObjectHeight = 65;
            Velocity = new Vector2D(0, 0);

            this.Weight = 400;
            this.InitTextures();
        }

        
    }
}
