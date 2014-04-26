using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Geometry
{
    public class GameRectangle : IGeometricShape
    {
        private Vector2D positionVectorUL;
        private Vector2D positionVectorUR;

        /// <summary>
        /// Должина на правоаголникот
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Висина на правоаголникот
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Креира нов правоаголник со позиција на темето
        /// во горниот лев агол специфицирана со радиус векторот positionUL
        /// и темето во горниот десен агол со радиус вектор positionUR
        /// Притоа радиус векторот се однесува на темето кое се наоѓа во горниот лев агол.
        /// </summary>
        /// <param name="positionUL"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GameRectangle(Vector2D positionUL, Vector2D positionUR, double width, double height)
        {
            this.positionVectorUL = new Vector2D(positionUL);
            this.positionVectorUR = new Vector2D(positionUR);
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Враќа правоаголник (Rectangle)
        /// </summary>
        public GeometricShapeTypes ShapeType
        {
            get { return GeometricShapeTypes.Rectangle; }
        }

        /// <summary>
        /// Дали правоаголникот се сече или се допира со некоја друга геометриска фигура.
        /// Враќа и листа од некои точки со кои тој се сече.
        /// </summary>
        /// <param name="geometricShape"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool Intersects(IGeometricShape geometricShape, out List<Vector2D> points)
        {
            points = new List<Vector2D>();

            if (geometricShape.ShapeType == GeometricShapeTypes.Rectangle)
            {
                return this.Intersects((GameRectangle)geometricShape, points);
            }
            else if (geometricShape.ShapeType == GeometricShapeTypes.Circle)
            {
                return this.Intersects((GameCircle)geometricShape, points);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Провери дали се сече или допира со круг
        /// </summary>
        /// <param name="gameCircle"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private bool Intersects(GameCircle gameCircle, List<Vector2D> points)
        {
            /* Логички ќе сметаме дека правоаголникот секогаш е претставен како
             * 
             * UL                         UR
             *  ---------------------------
             *  |                         |
             *  |                         |
             *  |                         |
             *  ---------------------------
             * DL                         DR
             * 
             */

            ISet<Vector2D> pointsSet = new HashSet<Vector2D>();

            //темиња на правоаголникот
            Vector2D pointUL = positionVectorUL;
            Vector2D pointUR = positionVectorUR;
            Vector2D pointDL = new Vector2D(positionVectorUL + new Vector2D(0, Height));
            Vector2D pointDR = new Vector2D(positionVectorUR + new Vector2D(0, Height));

            //центар на кругот
            Vector2D center = gameCircle.Position;
            double radius = gameCircle.Radius;

            if(center.X >= pointUL.X && center.X <= pointUR.X && center.Y >= pointUL.Y
                && center.Y <= pointDL.Y)
            {
                Vector2D HUM = (pointUL + pointUR) / 2; //горна хоризонтала средна точка
                Vector2D VLM = (pointUL + pointDL) / 2; //лева вертикала средна точка
                Vector2D HDM = (pointDL + pointDR) / 2; //долна хоризонтала средна точка
                Vector2D VRM = (pointUR + pointDR) / 2; //десна вертикала средна точка

                //Ако векторот помеѓу центарот и сите овие точки има поголема должина од радиусот на кругот
                //тогаш кругот е во правоаголникот. Врати го неговиот центар во тој случај

                if((HUM - center).Magnitude() > radius && (VLM - center).Magnitude() > radius &&
                    (HDM - center).Magnitude() > radius && (VRM - center).Magnitude() > radius)
                {
                    points.Add(center);
                    return true;
                }
            }

            //Провери ги сите четири страни дали се сечат со кругот
            List<Vector2D> temp = GeometricAlgorithms.IntersectLineCircle(pointUL, pointUR, center, radius);
            foreach (Vector2D vec in temp)
            {
                pointsSet.Add(vec);
            }

            temp = GeometricAlgorithms.IntersectLineCircle(pointUL, pointDL, center, radius);
            foreach (Vector2D vec in temp)
            {
                pointsSet.Add(vec);
            }
            
            temp = GeometricAlgorithms.IntersectLineCircle(pointDL, pointDR, center, radius);
            foreach (Vector2D vec in temp)
            {
                pointsSet.Add(vec);
            }
            
            temp = GeometricAlgorithms.IntersectLineCircle(pointDR, pointUR, center, radius);
            foreach (Vector2D vec in temp)
            {
                pointsSet.Add(vec);
            }

            points.AddRange(pointsSet);
            return points.Count > 0;
        }

        /// <summary>
        /// Провери дали се сече или допира со правоаголник
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private bool Intersects(GameRectangle rect, List<Vector2D> points)
        {
            /* Логички ќе сметаме дека правоаголникот секогаш е претставен како
             * 
             * UL                         UR
             *  ---------------------------
             *  |                         |
             *  |                         |
             *  |                         |
             *  ---------------------------
             * DL                         DR
             * 
             */

            //темиња на првиот правоаголник
            Vector2D pointUL = positionVectorUL;
            Vector2D pointUR = positionVectorUR;
            Vector2D pointDL = new Vector2D(positionVectorUL + new Vector2D(0, Height));
            Vector2D pointDR = new Vector2D(positionVectorUR + new Vector2D(0, Height));


            //темиња на вториот правоаголник
            Vector2D pointUL2 = rect.positionVectorUL;
            Vector2D pointUR2 = rect.positionVectorUR;
            Vector2D pointDL2 = new Vector2D(rect.positionVectorUL + new Vector2D(0, rect.Height));
            Vector2D pointDR2 = new Vector2D(rect.positionVectorUR + new Vector2D(0, rect.Height));

            //Провери вториот дали се содржи целосно во првиот. Врати ги темињата на внатрешниот во тој случај
            if (pointUL2.X > pointUL.X && pointUR2.X < pointUR.X && pointDL2.Y < pointDL.Y)
            {
                points.Add(pointUL2);
                points.Add(pointUR2); 
                points.Add(pointDL2);
                points.Add(pointDR2);
                return true;
            } else if (pointUL.X > pointUL2.X && pointUR.X < pointUR2.X && pointDL.Y < pointDL2.Y)
            {
                //обратната ситуација
                points.Add(pointUL);
                points.Add(pointUR);
                points.Add(pointDL);
                points.Add(pointDR);
                return true;
            }

            //провери дали двете вертикални од вториот ја сечат горната хоризонтална страна од првиот
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointUL2, pointDL2, pointUL, pointUR)));
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(pointUR2, pointDR2, pointUL, pointUR)));

            //провери дали двете вертикали од вториот ја сечат долната хоризонтала од првиот
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointUL2, pointDL2, pointDL, pointDR)));
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointUR2, pointDR2, pointDL, pointDR)));

            //провери дали двете хоризонтали од вториот ја сечат левата вертикала од првиот
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointUL2, pointUR2,
                pointUL, pointDL)));
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointDL2, pointDR2,
                pointUL, pointDL)));

            //провери дали двете хоризонтали од вториот ја сечат десната вертикала од првиот
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointUL2, pointUR2,
                pointUR, pointDR)));
            points.AddRange((GeometricAlgorithms.IntersectLineSegments(
                pointDL2, pointDR2,
                pointUR, pointDR)));

            //ако е најдена барем една заедничка точка, врати true
            return points.Count > 0;
        }

        /// <summary>
        /// Позиција на горното лево теме на правоаголникот
        /// (радиус вектор на темето во горниот лев агол).
        /// </summary>
        public Vector2D PositionUL
        {
            get { return new Vector2D(positionVectorUL); }
            private set { this.positionVectorUL = value; }
        }

        /// <summary>
        /// Позиција на горното десно теме на правоаголникот
        /// (радиус вектор на темето во горниот десен агол).
        /// </summary>
        public Vector2D PositionUR
        {
            get { return new Vector2D(positionVectorUR); }
            private set { this.positionVectorUR = value; }
        }
    }
}
