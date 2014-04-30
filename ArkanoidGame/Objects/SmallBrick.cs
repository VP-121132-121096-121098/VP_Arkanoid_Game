using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class SmallBrick : AbstractBrick
    {



        public override void InitTextures()
        {
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\"+picture, Position.X,
                Position.Y, ObjectWidth, ObjectHeight));
        }

        public SmallBrick(Vector2D positionVector, int virtualGameWidth, int virtualGameHeight,string p)
            : base(new Vector2D(positionVector), positionVector + new Vector2D(200, 0), //+ висината
            positionVector + new Vector2D(0, 80) /* + висината */,p )
        {
            this.GameWidth = virtualGameWidth;
            this.GameHeight = virtualGameHeight;
            this.Position = new Vector2D(positionVector);

            ObjectWidth = 100;
            ObjectHeight = 80;
            Velocity = new Vector2D(0, 0);

            this.Health = 200;
            this.DamageEffect = 200;
            this.InitTextures();
        }

    }
}

