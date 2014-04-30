using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Renderer
{
    public class OldGameRenderer : IGameRenderer
    {
        /// <summary>
        /// Ширина и висина на прозорецот
        /// </summary>
        public int FrameHeight { get; private set; }
        public int FrameWidth { get; private set; }

        public int VirtualGameWidth { get; private set; }
        public int VirtualGameHeight { get; private set; }

        /// <summary>
        /// Креира нов рендерер. Како аргументи се праќаат должина и ширина на виртуелниот простор
        /// од играта.
        /// </summary>
        /// <param name="virtualGameWidth"></param>
        /// <param name="virtualGameHeight"></param>
        public OldGameRenderer(int virtualGameWidth, int virtualGameHeight)
        {
            FrameHeight = FrameWidth = 0;
            this.VirtualGameHeight = virtualGameHeight;
            this.VirtualGameWidth = virtualGameWidth;
        }

        /// <summary>
        /// Ги претвора координатите од прозорецот во координати од играта
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point ToGameCoordinates(Point pointInScreenCoordinates)
        {
            int x, y;
            GameUnitConversion.ConvertGameUnits(pointInScreenCoordinates.X,
                pointInScreenCoordinates.Y, out x, out y, FrameWidth, FrameHeight, VirtualGameWidth,
                VirtualGameHeight);
            return new Point(x, y);
        }

        /// <summary>
        /// Ги претвора координатите од прозорецот во координати од играта
        /// </summary>
        /// <param name="vectorInScreenCoordinates"></param>
        /// <returns></returns>
        public Vector2D ToGameCoordinates(Vector2D vectorInScreenCoordinates)
        {
            double x, y;
            GameUnitConversion.ConvertGameUnits(vectorInScreenCoordinates.X,
                vectorInScreenCoordinates.Y, out x, out y, FrameWidth, FrameHeight, VirtualGameWidth,
                VirtualGameHeight);
            return new Vector2D(x, y);
        }

        /// <summary>
        /// Ги претвора координатите од прозорецот во координати од играта
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ToGameCoordinates(ref float x, ref float y)
        {
            double tempX, tempY;
            GameUnitConversion.ConvertGameUnits(x,
                y, out tempX, out tempY, FrameWidth, FrameHeight, VirtualGameWidth,
                VirtualGameHeight);
            x = (float)tempX;
            y = (float)tempY;
        }

        /// <summary>
        /// Ги претвора координатите од играта во координати од прозорецот
        /// </summary>
        /// <param name="pointInGameCoordinates"></param>
        /// <returns></returns>
        public Point ToScreenCoordinates(Point pointInGameCoordinates)
        {
            int x, y;
            GameUnitConversion.ConvertGameUnits(pointInGameCoordinates.X, pointInGameCoordinates.Y,
                out x, out y, VirtualGameWidth, VirtualGameHeight, FrameWidth, FrameHeight);
            return new Point(x, y);
        }

        /// <summary>
        /// Претворање на должина од должина во игра на должина на екран
        /// </summary>
        public double ToScreenLength(double x1, double y1, double x2, double y2)
        {
            Vector2D vec1 = new Vector2D(x1, y1);
            vec1 = ToScreenCoordinates(vec1);
            Vector2D vec2 = new Vector2D(x2, y2);
            vec2 = ToScreenCoordinates(vec2);
            return (vec2 - vec1).Magnitude();
        }

        /// <summary>
        /// Претворање на должина од должина во игра на должина на екран
        /// </summary>
        public double ToScreenLength(Vector2D vec)
        {
            Vector2D vec1 = new Vector2D(0, 0);
            Vector2D vec2 = ToScreenCoordinates(vec);
            return (vec2 - vec1).Magnitude();
        }

        /// <summary>
        /// Претворање на должина од должина на екран во должина во игра
        /// </summary>
        public double ToGameLength(double x1, double y1, double x2, double y2)
        {
            Vector2D vec1 = new Vector2D(x1, y1);
            vec1 = ToGameCoordinates(vec1);
            Vector2D vec2 = new Vector2D(x2, y2);
            vec2 = ToGameCoordinates(vec2);
            return (vec2 - vec1).Magnitude();
        }

        /// <summary>
        /// Претворање на должина од должина на екран во должина во игра
        /// </summary>
        public double ToGameLength(Vector2D vec)
        {
            Vector2D vec1 = new Vector2D(0, 0);
            Vector2D vec2 = ToGameCoordinates(vec);
            return (vec2 - vec1).Magnitude();
        }

        /// <summary>
        /// Ги претвора координатите од играта во координати од прозорецот
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ToScreenCoordinates(ref float x, ref float y)
        {
            double tempX, tempY;
            GameUnitConversion.ConvertGameUnits(x, y,
                out tempX, out tempY, VirtualGameWidth, VirtualGameHeight, FrameWidth, FrameHeight);
            x = (float)tempX;
            y = (float)tempY;
        }

        /// <summary>
        /// Ги претвора координатите од играта во координати од прозорецот
        /// </summary>
        /// <param name="vectorInGameCoordinates"></param>
        /// <returns></returns>
        public Vector2D ToScreenCoordinates(Vector2D vectorInGameCoordinates)
        {
            double x, y;
            GameUnitConversion.ConvertGameUnits(vectorInGameCoordinates.X, vectorInGameCoordinates.Y,
                out x, out y, VirtualGameWidth, VirtualGameHeight, FrameWidth, FrameHeight);
            return new Vector2D(x, y);
        }

        // -Pi <= x <= Pi
        private double FindRotationAngle(Vector2D pointInGame)
        {
            Vector2D pointToScreen = ToScreenCoordinates(pointInGame);
            return Math.Acos(pointInGame * pointToScreen / (pointInGame.Magnitude() * pointToScreen.Magnitude()));
        }

        /// <summary>
        /// Исцртување на сите bitmaps редоследно според индексот во листата.
        /// Се исцртува користејќи го Graphics објектот. frameWidth и frameHeight 
        /// се ширини и висини на рамките на кои се црта
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Render(IList<IList<GameBitmap>> bitmaps, Graphics g, int frameWidth, int frameHeight)
        {
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;

            foreach (IList<GameBitmap> bitmapList in bitmaps)
            {
                if (bitmapList == null)
                    continue;

                foreach (GameBitmap bitmap in bitmapList)
                {
                    //најди го центарот на сликата
                    List<Vector2D> centerGamePosition = GeometricAlgorithms.IntersectLineSegments(
                        bitmap.PositionUL, bitmap.PositionDR,
                        bitmap.PositionDL, bitmap.PositionUR);
                    Vector2D center = ToScreenCoordinates(centerGamePosition[0]);

                    double width = new Vector2D(0 + bitmap.WidthInGameUnits, 0).Magnitude() 
                        * frameWidth / VirtualGameWidth;
                    double height = new Vector2D(0, 0 + bitmap.HeightInGameUnits).Magnitude() 
                        * frameHeight / VirtualGameHeight;

                    if (bitmap.IsSquare)
                    {
                        //ако е квадрат, тогаш прикажи го како квадрат
                        double mid = (height + width) / 2;
                        height = width = mid;
                    }

                    Vector2D positionUL = ToScreenCoordinates(bitmap.PositionUL);
                    Vector2D diagonal = positionUL - center;
                    diagonal = diagonal / diagonal.Magnitude() * Math.Sqrt(width * width / 4 + height * height / 4);
                    positionUL = center + diagonal;

                    Vector2D positionUR = ToScreenCoordinates(bitmap.PositionUR);
                    diagonal = positionUR - center;
                    diagonal = diagonal / diagonal.Magnitude() * Math.Sqrt(width * width / 4 + height * height / 4);
                    positionUR = center + diagonal;

                    Vector2D positionDL = ToScreenCoordinates(bitmap.PositionDL);
                    diagonal = positionDL - center;
                    diagonal = diagonal / diagonal.Magnitude() * Math.Sqrt(width * width / 4 + height * height / 4);
                    positionDL = center + diagonal;

                    Point[] vertices = new Point[] { positionUL, positionUR, positionDL };

                    Bitmap temp = RendererCache.GetBitmapFromMainMemory(bitmap.UniqueKey,
                        (int)width, (int)height);

                    if (temp == null)
                        continue;

                    g.DrawImage(temp, vertices);
                }
            }
        }

        /// <summary>
        /// Исцртување на една слика
        /// </summary>
        /// <param name="?"></param>
        /// <param name="g"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        public void Render(GameBitmap bitmap, Graphics g, int frameWidth, int frameHeight)
        {
            List<GameBitmap> list = new List<GameBitmap>(1);
            List<IList<GameBitmap>> list2 = new List<IList<GameBitmap>>();
            list2.Add(list);
            list.Add(bitmap);
            this.Render(list2, g, frameWidth, frameHeight);
        }

        public void DrawCircle(Vector2D center, float radius, Graphics g, Color color, int frameWidth, int frameHeight)
        {
            float x = (float)center.X;
            float y = (float)center.Y;

            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;

            this.ToScreenCoordinates(ref x, ref y);
            radius = (float)ToScreenLength(x, y, x + radius, y);
            g.DrawEllipse(new Pen(new SolidBrush(color), 2), x - radius, y - radius, 2 * radius, 2 * radius);
        }

        public void DrawRectangle(Pen pen, RectangleF rectangle, Graphics g, int frameWidth, int frameHeight)
        {
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;

            float x = rectangle.X;
            float y = rectangle.Y;
            this.ToScreenCoordinates(ref x, ref y);
            float width = (float)ToScreenLength(x, y, x + rectangle.Width, y);
            float height = (float)ToScreenLength(x, y, x, y + rectangle.Height);
            g.DrawRectangle(pen, x, y, width, height);
        }
    }
}
