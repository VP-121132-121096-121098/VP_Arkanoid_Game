using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class BigBrick : AbstractBrick
    {


        public override void InitTextures()
        {
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\" + picture, Position.X,
                Position.Y, ObjectWidth, ObjectHeight));
        }

        public BigBrick(Vector2D positionVector, int virtualGameWidth, int virtualGameHeight, string picture)
            : base(new Vector2D(positionVector), new Vector2D(positionVector.X + 200, positionVector.Y), //+ висината
            new Vector2D(positionVector.X, positionVector.Y + 80) /* + висината */ , picture/*slikata*/)
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