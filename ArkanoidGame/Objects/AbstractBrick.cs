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
        public abstract void OnUpdate(long gameElapsedTime);
        

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

        public int Weight
        {
            get;
            set;
        }

        public int GameWidth { get; set; }
        public int GameHeight { get;  set; }

        public void OnCollisionDetected(IDictionary<IGameObject, IList<Vector2D>> collisionArguments)
        {
            throw new NotImplementedException();
        }
    }
}
