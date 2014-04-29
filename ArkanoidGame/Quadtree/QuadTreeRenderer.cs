using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ArkanoidGame.Quadtree
{
    public static class Utility
    {
        static Random m_rand = new Random(DateTime.Now.Millisecond);

        public static Color RandomColor
        {
            get
            {
                return Color.FromArgb(
                    255,
                    m_rand.Next(255),
                    m_rand.Next(255),
                    m_rand.Next(255));

            }
        }
    }

    /// <summary>
    /// Class draws a QuadTree
    /// </summary>
    public class QuadTreeRenderer<T> where T : IGameObject
    {
        /// <summary>
        /// Create the renderer, give the QuadTree to render.
        /// </summary>
        /// <param name="quadTree"></param>
        public QuadTreeRenderer(QuadTree<T> quadTree)
        {
            m_quadTree = quadTree;
        }

        QuadTree<T> m_quadTree;

        /// <summary>
        /// Hashtable contains a colour for every node in the quad tree so that they are
        /// rendered with a consistant colour.
        /// </summary>
        Dictionary<QuadTreeNode<T>, Color> m_dictionary = new Dictionary<QuadTreeNode<T>, Color>();

        /// <summary>
        /// Get the colour for a QuadTreeNode from the hash table or else create a new colour
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Color GetColor(QuadTreeNode<T> node)
        {
            if (m_dictionary.ContainsKey(node))
                return m_dictionary[node];

            Color color = Utility.RandomColor;
            m_dictionary.Add(node, color);
            return color;
        }

        /// <summary>
        /// Render the QuadTree into the given Graphics context
        /// </summary>
        /// <param name="graphics"></param>
        public void Render(Graphics graphics)
        {
            m_quadTree.ForEach(delegate(QuadTreeNode<T> node)
            {

                // draw the contents of this quad
                /*if (node.Contents != null)
                {
                    foreach (T item in node.Contents)
                    {
                        using (Brush b = new SolidBrush(Color.Yellow))
                            graphics.FillEllipse(b, Rectangle.Round(item.Rectangle));
                    }
                }*/

                // draw this quad

                // Draw the border
                Color color = GetColor(node);
                graphics.DrawRectangle(Pens.Black, Rectangle.Round(node.Bounds));

                // draw the inside of the border in a distinct colour
                using (Pen p = new Pen(color))
                {
                    Rectangle inside = Rectangle.Round(node.Bounds);
                    inside.Inflate(-1, -1);
                    graphics.DrawRectangle(p, inside);
                }

            });

        }

        /// <summary>
        /// Render the QuadTree with the given renderer
        /// </summary>
        public void Render(IGameRenderer renderer, Graphics g, int frameWidth, int frameHeight,
            RectangleF area, Color boundsColor)
        {
            List<T> selectedItems = this.m_quadTree.Query(area);

            if (selectedItems != null)
            {
                foreach (T obj in selectedItems)
                {
                    using (Pen p = new Pen(boundsColor, 2))
                        renderer.DrawRectangle(p, obj.Rectangle, g, frameWidth, frameHeight);
                }
            }

            m_quadTree.ForEach(delegate(QuadTreeNode<T> node)
            {                
                // draw this quad

                // Draw the border
                Color color = GetColor(node);
                renderer.DrawRectangle(Pens.Black, node.Bounds,
                    g, frameWidth, frameHeight);

                // draw the inside of the border in a distinct colour
                using (Pen p = new Pen(color))
                {
                    RectangleF inside = node.Bounds;
                    inside.Inflate(-1, -1);
                    renderer.DrawRectangle(p, inside,
                        g, frameWidth, frameHeight);
                }

            });

        }
    }
}
