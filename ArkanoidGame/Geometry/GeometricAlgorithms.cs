using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Geometry
{
    public class GeometricAlgorithms
    {
        private static int sgn(double x)
        {
            if (x < 0)
                return -1;
            else
                return 1;
        }

        /// <summary>
        /// Враќа листа на точки на пресек или допир на права и кружница. Ако не постојат такви, тогаш
        /// оваа функција враќа празна листа.
        /// </summary>
        /// <param name="lineP1"></param>
        /// <param name="lineP2"></param>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        /// <returns></returns>
        public static List<Vector2D> IntersectLineCircle(Vector2D lineP1, Vector2D lineP2,
            Vector2D circleCenter, double circleRadius)
        {
            /* http://mathworld.wolfram.com/Circle-LineIntersection.html */

            lineP1 = new Vector2D(lineP1 - circleCenter);
            lineP2 = new Vector2D(lineP2 - circleCenter);

            List<Vector2D> pointsOfInteresction = new List<Vector2D>();

            double dx = lineP2.X - lineP1.X;
            double dy = lineP2.Y - lineP1.Y;
            double dr = Math.Sqrt(dx * dx + dy * dy);
            double D = lineP1.X * lineP2.Y - lineP2.X * lineP1.Y;
            double r = circleRadius;

            //точки на пресек
            if (r * r * dr * dr - D * D < 0)
                return new List<Vector2D>(); //во овој случај нема пресечни точки

            double sq = Math.Sqrt(r * r * dr * dr - D * D);
            double x = (D * dy + sgn(dy) * dx * sq) / (dr * dr);
            double y = (-D * dx + Math.Abs(dy) * sq) / (dr * dr);

            Vector2D point = new Vector2D(x, y);

            //Ако точката се наоѓа помеѓу двете точки од отсечката, тогаш е пресечна точка
            if ((point - lineP1).Magnitude() + (point - lineP2).Magnitude() <= (lineP2 - lineP1).Magnitude())
            {
                pointsOfInteresction.Add(point + circleCenter);
            }

            x = (D * dy + sgn(dy) * dx * sq) / (dr * dr);
            y = (-D * dx - Math.Abs(dy) * sq) / (dr * dr);

            point = new Vector2D(x, y);

            //Ако точката се наоѓа помеѓу двете точки од отсечката, тогаш е пресечна точка
            if ((point - lineP1).Magnitude() + (point - lineP2).Magnitude() <= (lineP2 - lineP1).Magnitude())
            {
                pointsOfInteresction.Add(point + circleCenter);
            }

            x = (D * dy - sgn(dy) * dx * sq) / (dr * dr);
            y = (-D * dx + Math.Abs(dy) * sq) / (dr * dr);

            point = new Vector2D(x, y);

            //Ако точката се наоѓа помеѓу двете точки од отсечката, тогаш е пресечна точка
            if ((point - lineP1).Magnitude() + (point - lineP2).Magnitude() <= (lineP2 - lineP1).Magnitude())
            {
                pointsOfInteresction.Add(point + circleCenter);
            }

            x = (D * dy + sgn(dy) * dx * sq) / (dr * dr);
            y = (-D * dx + Math.Abs(dy) * sq) / (dr * dr);

            point = new Vector2D(x, y);

            //Ако точката се наоѓа помеѓу двете точки од отсечката, тогаш е пресечна точка
            if ((point - lineP1).Magnitude() + (point - lineP2).Magnitude() <= (lineP2 - lineP1).Magnitude())
            {
                pointsOfInteresction.Add(point + circleCenter);
            }

            return pointsOfInteresction;
        }

        /// <summary>
        /// Проверува дали се сечат или преклопуваат две отсечки 
        /// претставени со радиус векторите на нивните темиња.
        /// Притоа line1P1 и line1P2 се радиус вектори на двете темиња на првата отсечка,
        /// а line2P1 и line2P2 се радиус вектори на двете темиња на втората отсечка.
        /// Ако двете отсечки имаат заеднички точки ги враќа истите (не сите ако се преклопуваат),
        /// во спротивно враќа празна листа.
        /// </summary>
        public static List<Vector2D> IntersectLineSegments(Vector2D line1P1, Vector2D line1P2,
            Vector2D line2P1, Vector2D line2P2)
        {
            List<Vector2D> returnList = new List<Vector2D>();

            Vector2D p = line1P1;
            Vector2D q = line2P1;
            Vector2D r = line1P2 - line1P1;
            Vector2D s = line2P2 - line2P1;

            Vector3D crossRS = ((Vector3D)r).CrossProduct(s); //r x s
            Vector3D crossQ_PR = ((Vector3D)(q - p)).CrossProduct(r); //(q - p) x r
            Vector3D crossQ_PS = ((Vector3D)(q - p)).CrossProduct(s); //(q - p) x s

            if (crossRS.Z != 0)
            {
                double t = crossQ_PS.Z / crossRS.Z;
                double u = crossQ_PR.Z / crossRS.Z;

                if (0 <= t && t <= 1 && 0 <= u && u <= 1)
                {
                    //заедничка точка за двете отсечки е p + t * r
                    returnList.Add(p + t * r);
                }
                else
                {
                    //инаку не се сечат
                    return returnList;
                }
            }

            if (crossRS.Z == 0)
            {
                if (crossQ_PR.Z == 0)
                {
                    //колинеарни
                    if (!(0 <= (q - p) * r && (q - p) * r <= r * r) 
                        && !(0 <= (p - q) * s && (p - q) * s <= s * s))
                    {
                        //не се сечат
                        return returnList;
                    }

                    //провери дали се преклопувачки
                    double distancePQ = (q - p).Magnitude();
                    double distanceQ_PR = ((p + r) - q).Magnitude();
                    if (distancePQ + distanceQ_PR <= r.Magnitude())
                    {
                        returnList.Add(new Vector2D(q)); //Q е пресечна точка
                    }

                    double distanceP_QS = ((q + s) - p).Magnitude();
                    double distanceQS_PR = ((q + s) - (p + r)).Magnitude();
                    if (distanceP_QS + distanceQS_PR <= r.Magnitude())
                    {
                        returnList.Add(new Vector2D(q + s)); //Q + S е пресечна точка
                    }

                    //Ако не е ниту Q, ниту Q + S, тогаш имаме ваква ситуација
                    // Q______P_________P+R_________Q+S
                    
                    returnList.Add(p);
                    returnList.Add(p + r);
                }
                else
                {
                    //не се сечат
                    return returnList;
                }
            }

            return returnList;
        }

        public static List<Vector2D> IntersectCircles(GameCircle circle1, GameCircle circle2)
        {
            List<Vector2D> listPoints = new List<Vector2D>();
            ISet<Vector2D> setPoints = new HashSet<Vector2D>();

            Vector2D P0 = circle1.Position; //центар на првиот круг
            double r0 = circle1.Radius; //радиус на првиот круг
            Vector2D P1 = circle2.Position; //центар на вториот круг
            double r1 = circle2.Radius; //радиус на вториот круг

            double d = (P1 - P0).Magnitude(); //растојание помеѓу двата центри
            if (d > r0 + r1)
            {
                return listPoints; /* Растојанието помѓу центрите е поголемо од збирот на радиусите.
                                    * Значи не може да се допираат ниту да се сечат.
                                    */
            }

            if (d < Math.Abs(r0 - r1))
            {
                listPoints.Add(circle1.Position);
                return listPoints;
                /* Во овој случај едниот круг целосно се содржи во другиот */
            }

            double a = (r0 * r0 - r1 * r1 + d * d) / (2 * d); /* растојание од центарот на првата кружница
                                                               * до местото каде што нормалата h спуштена од пресечните
                                                               * точки ја сече правата P0-P1.
                                                               */

            Vector2D P2 = P0 + (P1 - P0) * a / d; /* точката каде што нормалата h ја сече правата P0-P1 */
            
            // Правецот на нормалата h е било кој вектор кој што скаларно помножен со P0->P1 дава 0.
            // Нека Pt = (P1 - P0) (вектор)
            Vector2D Pt = (P1 - P0);

            double h = Math.Sqrt(r0 * r0 - a * a);

            double p3x = P2.X + h * (P1.Y - P0.Y) / d;
            double p3y = P2.Y - h * (P1.X - P0.X) / d;
            setPoints.Add(new Vector2D(p3x, p3y));

            p3x = P2.X - h * (P1.Y - P0.Y) / d;
            p3y = P2.Y + h * (P1.X - P0.X) / d;
            setPoints.Add(new Vector2D(p3x, p3y));

            listPoints.AddRange(setPoints);
            return listPoints;
        }

        private static double Distance(Vector2D point1, Vector2D point2)
        {
            return (point2 - point1).Magnitude();
        }

        private static bool IsPointOnLine(Vector2D point, Vector2D linePoint1, Vector2D linePoint2)
        {
            return point.Y == (linePoint2.Y - linePoint1.Y) / (linePoint2.X - linePoint1.X)
                * (point.X - linePoint1.X) + linePoint1.Y;
        }
    }
}
