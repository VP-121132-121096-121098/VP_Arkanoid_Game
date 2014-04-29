using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameRenderer
    {
        /// <summary>
        /// Исцртување на една слика
        /// </summary>
        /// <param name="?"></param>
        /// <param name="g"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        void Render(GameBitmap bitmap, Graphics g, int frameWidth, int frameHeight);

        /// <summary>
        /// Претворање на должина од должина во игра на должина на екран
        /// </summary>
        double ToScreenLength(Vector2D vec);

        /// <summary>
        /// Претворање на должина од должина во игра на должина на екран
        /// </summary>
        double ToGameLength(Vector2D vec);

        /// <summary>
        /// Претворање на должина од должина во игра на должина на екран.
        /// Параметрите се координатите на отсечката.
        /// </summary>
        double ToScreenLength(double x1, double y1, double x2, double y2);

        /// <summary>
        /// Претворање на должина од должина на екран во должина во игра.
        /// Параметрите се координатите на отсечката.
        /// </summary>
        double ToGameLength(double x1, double y1, double x2, double y2);

        int FrameHeight { get; }
        int FrameWidth { get; }

        /// <summary>
        /// Исцртување на сите bitmaps редоследно според индексот во листата.
        /// Се исцртува користејќи го Graphics објектот. frameWidth и frameHeight 
        /// се ширини и висини на рамките на кои се црта
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void Render(IList<IList<GameBitmap>> bitmaps, Graphics g, int frameWidth, int frameHeight);

        /// <summary>
        /// Ги претвора координатите од прозорецот во координати од играта
        /// </summary>
        /// <param name="vectorInScreenCoordinates"></param>
        /// <returns></returns>
        Vector2D ToGameCoordinates(Vector2D vectorInScreenCoordinates);

        /// <summary>
        /// Ги претвора координатите од прозорецот во координати од играта
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void ToGameCoordinates(ref float x, ref float y);

        /// <summary>
        /// Ги претвора координатите од прозорецот во координати од играта
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        Point ToGameCoordinates(Point pointInScreenCoordinates);
        
        int VirtualGameHeight { get; }
        int VirtualGameWidth { get; }

        /// <summary>
        /// Ги претвора координатите од играта во координати од прозорецот
        /// </summary>
        /// <param name="vectorInGameCoordinates"></param>
        /// <returns></returns>
        Vector2D ToScreenCoordinates(Vector2D vectorInGameCoordinates);

        /// <summary>
        /// Ги претвора координатите од играта во координати од прозорецот
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void ToScreenCoordinates(ref float x, ref float y);

        /// <summary>
        /// Ги претвора координатите од играта во координати од прозорецот
        /// </summary>
        /// <param name="pointInGameCoordinates"></param>
        /// <returns></returns>
        Point ToScreenCoordinates(Point pointInGameCoordinates);

        void DrawRectangle(Pen pen, RectangleF rectangle, Graphics g, int frameWidth, int frameHeight);
    }
}
