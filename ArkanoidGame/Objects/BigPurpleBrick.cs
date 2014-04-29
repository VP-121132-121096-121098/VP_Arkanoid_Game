using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class BigPurpleBrick : AbstractBrick
    {
        //tezina inicijalno na 400 pa ako e pogodena ednas,se namaluva na pola 


        public override void OnUpdate(long gameElapsedTime)
        {
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
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\element_purple_rectangle.png", Position.X,
                Position.Y, ObjectWidth, ObjectHeight));
        }

        public BigPurpleBrick(Vector2D positionVector, int virtualGameWidth, int virtualGameHeight)
            : base()
        {
            this.GameWidth = virtualGameWidth;
            this.GameHeight = virtualGameHeight;
            this.Position = new Vector2D(positionVector);
            ObjectWidth = 200;
            ObjectHeight = 80;
            Velocity = new Vector2D(0, 0);

            this.Health = 200;
            this.DamageEffect = 200;
            this.InitTextures();
        }


    }
}
