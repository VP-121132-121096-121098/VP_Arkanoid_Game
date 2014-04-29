using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public abstract class AbstractBrick : IGameObject
    {
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

        public AbstractBrick(Vector2D positionUL, Vector2D positionUR, Vector2D positionDL)
        {
            this.PositionUL = positionUL;
            this.PositionUR = positionUR;
            this.PositionDL = positionDL;
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
            return new GameRectangle(this.Position, this.Position + new Vector2D(this.ObjectWidth, 0),
               this.Position + new Vector2D(0, this.ObjectHeight));
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
            throw new NotImplementedException();
        }
    }
}
