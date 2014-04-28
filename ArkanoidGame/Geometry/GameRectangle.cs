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
        /*
         * UL                         UR
         *  ---------------------------
         *  |                         |
         *  |                         |
         *  |                         |
         *  ---------------------------
         * DL                         DR
         */

        private Vector2D positionVectorUL;
        private Vector2D positionVectorUR;
        private Vector2D positionVectorDL;


        /// <summary>
        /// Должина на правоаголникот
        /// </summary>
        public double Width { get; private set; }

        private Vector2D GetPositionVectorDR()
        {
            return (positionVectorDL + (positionVectorUR - positionVectorUL));
        }

        public Vector2D TopMostPoint
        {
            get
            {
                Vector2D temp = PositionUL;
                if (PositionUR.Y < temp.Y)
                    temp = PositionUR;
                if (PositionDL.Y < temp.Y)
                    temp = PositionDL;
                if (PositionDR.Y < temp.Y)
                    temp = PositionDR;

                return temp;
            }
        }

        public Vector2D BottomMostPoint
        {
            get
            {
                Vector2D temp = PositionUL;
                if (PositionUR.Y > temp.Y)
                    temp = PositionUR;
                if (PositionDL.Y > temp.Y)
                    temp = PositionDL;
                if (PositionDR.Y > temp.Y)
                    temp = PositionDR;

                return temp;
            }
        }

        public Vector2D RightMostPoint
        {
            get
            {
                Vector2D temp = PositionUL;
                if (PositionUR.X > temp.X)
                    temp = PositionUR;
                if (PositionDL.X > temp.X)
                    temp = PositionDL;
                if (PositionDR.X > temp.X)
                    temp = PositionDR;

                return temp;
            }
        }

        public Vector2D LeftMostPoint
        {
            get
            {
                Vector2D temp = PositionUL;
                if (PositionUR.X < temp.X)
                    temp = PositionUR;
                if (PositionDL.X < temp.X)
                    temp = PositionDL;
                if (PositionDR.X < temp.X)
                    temp = PositionDR;

                return temp;
            }
        }

        /// <summary>
        /// Висина на правоаголникот
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Креира нов правоаголник со позиција на темето
        /// во горниот лев агол специфицирана со радиус векторот positionUL,
        /// темето во горниот десен агол со радиус вектор positionUR
        /// и темето во долниот лев агол со радиус вектор positionDL
        /// Притоа радиус векторот се однесува на темето кое се наоѓа во горниот лев агол.
        /// </summary>
        /// <param name="positionUL"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GameRectangle(Vector2D positionUL, Vector2D positionUR, Vector2D positionDL)
        {
            this.positionVectorUL = new Vector2D(positionUL);
            this.positionVectorUR = new Vector2D(positionUR);
            this.positionVectorDL = new Vector2D(positionDL);

            this.Width = Math.Abs(PositionUL.X - positionUR.X);
            this.Height = Math.Abs(PositionUL.Y - positionDL.Y);
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
            /* Ќе сметаме дека правоаголникот секогаш е претставен како
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
            Vector2D pointDL = positionVectorDL;
            Vector2D pointDR = GetPositionVectorDR();

            //центар на кругот
            Vector2D center = gameCircle.Position;
            double radius = gameCircle.Radius;

            if (center.X >= pointUL.X && center.X <= pointUR.X && center.Y >= pointUL.Y
                && center.Y <= pointDL.Y)
            {
                Vector2D HUM = (pointUL + pointUR) / 2; //горна хоризонтала средна точка
                Vector2D VLM = (pointUL + pointDL) / 2; //лева вертикала средна точка
                Vector2D HDM = (pointDL + pointDR) / 2; //долна хоризонтала средна точка
                Vector2D VRM = (pointUR + pointDR) / 2; //десна вертикала средна точка

                //Ако векторот помеѓу центарот и сите овие точки има поголема должина од радиусот на кругот
                //тогаш кругот е во правоаголникот. Врати го неговиот центар во тој случај

                if ((HUM - center).Magnitude() > radius && (VLM - center).Magnitude() > radius &&
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
            /* Ќе сметаме дека правоаголникот секогаш е претставен како
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
            Vector2D pointDL = positionVectorDL;
            Vector2D pointDR = GetPositionVectorDR();


            //темиња на вториот правоаголник
            Vector2D pointUL2 = rect.positionVectorUL;
            Vector2D pointUR2 = rect.positionVectorUR;
            Vector2D pointDL2 = rect.positionVectorDL;
            Vector2D pointDR2 = rect.GetPositionVectorDR();

            //Провери вториот дали се содржи целосно во првиот. Врати ги темињата на внатрешниот во тој случај
            if (pointUL2.X > pointUL.X && pointUR2.X < pointUR.X && pointDL2.Y < pointDL.Y)
            {
                points.Add(pointUL2);
                points.Add(pointUR2);
                points.Add(pointDL2);
                points.Add(pointDR2);
                return true;
            }
            else if (pointUL.X > pointUL2.X && pointUR.X < pointUR2.X && pointDL.Y < pointDL2.Y)
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

        /// <summary>
        /// Позиција на темето во долниот лев агол
        /// </summary>
        public Vector2D PositionDL
        {
            get { return new Vector2D(positionVectorDL); }
            private set { this.positionVectorDL = value; }
        }

        /// <summary>
        /// Позиција на темето во долниот десен агол
        /// </summary>
        public Vector2D PositionDR
        {
            get { return new Vector2D(GetPositionVectorDR()); }
        }


        public RectangleF GetBoundingRectangle
        {
            get
            {
                Vector2D leftMost = this.LeftMostPoint;
                Vector2D topMost = this.TopMostPoint;
                Vector2D bottomMost = this.BottomMostPoint;
                Vector2D rightMost = this.RightMostPoint;

                //Се додава 0.1 на секоја страна со цел да се намалат шансите за false negatives при детекција на
                //можни судири.
                return new RectangleF((float)Math.Max(0, leftMost.X - 0.1), (float)Math.Max(0, topMost.Y - 0.1),
                    (float)(rightMost.X - leftMost.X) + 0.2f, (float)(bottomMost.Y - topMost.Y) + 0.2f);
            }
        }
    }
}
