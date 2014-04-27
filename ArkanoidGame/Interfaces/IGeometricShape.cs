using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public enum GeometricShapeTypes
    {
        Rectangle,
        Circle,
        Paddle
    }

    public interface IGeometricShape
    {
        /// <summary>
        /// Тип на геометриска форма
        /// </summary>
        GeometricShapeTypes ShapeType { get; }

        /// <summary>
        /// Ако постојат точки со кои се сечат двете геометриски фигури враќа true и 
        /// ги враќа точките, во спротивно враќа null.
        /// </summary>
        /// <param name="geometricShape"></param>
        /// <returns></returns>
        bool Intersects(IGeometricShape geometricShape, out List<Vector2D> points);

        /// <summary>
        /// Враќа правоаголник во кој целосно ќе се содржи геометриската фигура.
        /// </summary>
        RectangleF GetBoundingRectangle { get; }
    }
}
