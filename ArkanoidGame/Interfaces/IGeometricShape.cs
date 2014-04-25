using ArkanoidGame.Framework;
using System;
using System.Collections.Generic;
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
        /// Враќа точно (true) ако овој објект го допира или го сече објектот 
        /// испратен како аргумент, во спротивно враќа неточно (false).
        /// </summary>
        /// <param name="geometricShape"></param>
        /// <returns></returns>
        bool Intersects(IGeometricShape geometricShape);

        /// <summary>
        /// Вектор кој ја одредува локација на објектот (радиус вектор 
        /// на точката чии координати ја одредуваат позиција на објектот).
        /// Пример за кругот таквата точка е неговиот центар, за правоаголникот
        /// може да биде темето во горниот лев агол итн.
        /// </summary>
        Vector2D Position { get; }
    }
}
