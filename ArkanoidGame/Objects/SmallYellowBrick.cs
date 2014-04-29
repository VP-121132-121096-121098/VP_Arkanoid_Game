using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class SmallYellowBrick : AbstractBrick
    {
       


       public override void InitTextures()
        {
            ObjectTextures = new List<GameBitmap>();
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\element_yellow_square.png", Position.X,
                Position.Y, ObjectWidth, ObjectHeight, "element_yellow_square.png"));
        }

        public SmallYellowBrick(Vector2D positionVector, int virtualGameWidth, int virtualGameHeight)
            : base()
        {
            this.GameWidth = virtualGameWidth;
            this.GameHeight = virtualGameHeight;
            this.Position = new Vector2D(positionVector);
            ObjectWidth = 100;
            ObjectHeight = 80;
            Velocity = new Vector2D(0, 0);

            this.Health = 100;
            this.DamageEffect = 100;
            this.InitTextures();
        }


    }
}

