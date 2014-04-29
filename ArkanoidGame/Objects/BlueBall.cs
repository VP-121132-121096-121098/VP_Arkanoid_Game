using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class BlueBall : IGameObject
    {
        public BlueBall(Vector2D position, double radius)
        {
            ObjectTextures = new List<GameBitmap>();
            this.Position = position;
            this.Radius = radius;
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\ballBlue.png", this.Position - new Vector2D(0, Radius)
                - new Vector2D(Radius, 0),
                this.Position - new Vector2D(0, Radius) + new Vector2D(Radius, 0), this.Position + new Vector2D(0, Radius)
                + new Vector2D(-Radius, 0)));
            ObjectWidth = ObjectHeight = 2 * radius;
        }

        public double Health { get { return 1000; } }

        public double DamageEffect { get { return 100; } }

        public RectangleF Rectangle
        {
            get { return new GameCircle(Position, ObjectWidth / 2).GetBoundingRectangle; }
        }

        public IGeometricShape GetGeometricShape()
        {
            return new GameCircle(Position, Radius);
        }

        public double Radius { get; private set; }

        public void OnUpdate(long gameElapsedTime)
        {
            //throw new NotImplementedException();
        }

        public void OnCollisionDetected(IDictionary<IGameObject, IList<Geometry.Vector2D>> collisionArguments)
        {
            //throw new NotImplementedException();
        }

        public Vector2D Position { get; set; }

        public double ObjectWidth { get; private set; }

        public double ObjectHeight { get; private set; }

        public IList<GameBitmap> ObjectTextures { get; private set; }

        public Vector2D Velocity { get; private set; }

        public GameObjectType ObjectType
        {
            get { return GameObjectType.Ball; }
        }
    }
}
