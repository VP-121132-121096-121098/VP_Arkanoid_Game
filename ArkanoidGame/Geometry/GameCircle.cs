using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Geometry
{
    public class GameCircle : IGeometricShape
    {
        public GameCircle(Vector2D position, double radius) 
        {
            this.Position = new Vector2D(position);
            this.Radius = radius;
        }

        public GeometricShapeTypes ShapeType
        {
            get { return GeometricShapeTypes.Circle; }
        }

        private Vector2D positionVector;

        /// <summary>
        /// Должина на радиус на кругот
        /// </summary>
        public double Radius { get; private set; }

        /// <summary>
        /// Координати на центарот на кругот
        /// </summary>
        public Vector2D Position
        {
            get { return new Vector2D(positionVector); }
            set { this.positionVector = new Vector2D(value); }
        }

        public bool Intersects(IGeometricShape geometricShape, out List<Vector2D> points)
        {
            if (geometricShape.ShapeType == GeometricShapeTypes.Rectangle)
            {
                GameRectangle temp = (GameRectangle)geometricShape;
                return temp.Intersects(this, out points);
            } if (geometricShape.ShapeType == GeometricShapeTypes.Circle)
            {
                GameCircle circle = (GameCircle)geometricShape;
                points = GeometricAlgorithms.IntersectCircles(this, circle);
                return points.Count > 0;
            }

            throw new NotImplementedException();
        }
    }
}
