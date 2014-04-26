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
    public class GameRenderer : IGameRenderer
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
        public GameRenderer(int virtualGameWidth, int virtualGameHeight)
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
                foreach (GameBitmap bitmap in bitmapList)
                {
                    Bitmap temp = RendererCache.GetBitmapFromMainMemory(bitmap.PictureID,
                        (int)Math.Round(ToScreenLength(bitmap.X, bitmap.Y,
                        bitmap.X + bitmap.WidthInGameUnits, bitmap.Y)),
                        (int)Math.Round(ToScreenLength(bitmap.X, bitmap.Y,
                        bitmap.X, bitmap.Y + bitmap.HeightInGameUnits)));

                    if (temp == null)
                        continue;

                    Vector2D position = ToScreenCoordinates(new Vector2D(bitmap.X, bitmap.Y));
                    g.DrawImage(temp, (float)position.X, (float)position.Y);
                }
            }
        }
    }
}
