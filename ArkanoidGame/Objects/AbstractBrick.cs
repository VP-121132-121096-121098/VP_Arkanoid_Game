using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArkanoidGame.Objects
{
    public abstract class AbstractBrick : IGameObject

    {
       //public string picture { get; set; }
        public GameBitmap bmp { get; set; }
        public void OnUpdate(long gameElapsedTime)
        {
            Position += (Velocity) / 2;

            if (Position.X > GameWidth - 10 - ObjectWidth)
                Position.X = GameWidth - 10 - ObjectWidth;
            else if (Position.X < 5)
                Position.X = 5;
            ObjectTextures[0].PositionUL = PositionUL;
            ObjectTextures[0].PositionUR = PositionUR;
            ObjectTextures[0].PositionDL = PositionDL;
        }

        public AbstractBrick()
        {

        }

       /* public AbstractBrick(Vector2D positionUL, Vector2D positionUR, Vector2D positionDL,string p)
        {
            this.PositionUL = positionUL;
            this.PositionUR = positionUR;
            this.PositionDL = positionDL;
            this.picture = p;
        }*/

        public AbstractBrick(Vector2D positionUL, Vector2D positionUR, Vector2D positionDL, GameBitmap bmp)
        {
            this.PositionUL = positionUL;
            this.PositionUR = positionUR;
            this.PositionDL = positionDL;
            this.bmp = bmp;
            this.ObjectTextures = null;
        }

        public Vector2D PositionUL { get; set; }
        public Vector2D PositionUR { get; set; }
        public Vector2D PositionDL { get; set; }

 
        public  Vector2D Position
        {
            get;
            set;
        }

        public double ObjectWidth
        {
            get;
            set;
        }

        public double ObjectHeight
        {
            get;
            set;
        }

        public IList<Renderer.GameBitmap> ObjectTextures
        {
            get;
            set;
        }

        public Vector2D Velocity
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
            return new GameRectangle(this.PositionUL, this.PositionUR, this.PositionDL);
        }


        public GameObjectType ObjectType { get { return GameObjectType.Brick; } }
        public abstract void InitTextures();

      

        public int GameWidth { get; set; }
        public int GameHeight { get;  set; }

        public double Health
        {
            get;
            set;
        }

        public double DamageEffect
        {
            get;
            set;
        }

        public void OnCollisionDetected(IDictionary<IGameObject, IList<Vector2D>> collisionArguments)
        {
            foreach (KeyValuePair<IGameObject, IList<Vector2D>> obj in collisionArguments)
            {
                if (obj.Key.ObjectType != GameObjectType.Brick)
                {
                    Health -= obj.Key.DamageEffect;
                }
            }

            //throw new NotImplementedException();
        }


        public Vector2D PositionChange
        {
            get { throw new NotImplementedException(); }
        }


        
    }
}
