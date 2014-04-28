using ArkanoidGame.Interfaces;
using System;
using System.Drawing;

namespace QuadTreeDemoApp
{
    /// <summary>
    /// An item with a position, a size and a random colour to test
    /// the QuadTree structure.
    /// </summary>
    class Item: IGameObject
    {
        /// <summary>
        /// Create an item at the given location with the given size.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        public Item(Point p, int size)
        {
            m_size = size;
            m_rectangle = new RectangleF(p.X, p.Y, m_size, m_size);
            m_color = Utility.RandomColor;
        }

        /// <summary>
        /// Bounds of this item
        /// </summary>
        RectangleF m_rectangle;

        /// <summary>
        /// the default size of this item
        /// </summary>
        int m_size = 2;

        /// <summary>
        /// Colour of the item
        /// </summary>
        Color m_color;

        /// <summary>
        /// Colour to draw the item for the QuadTree demo
        /// </summary>
        public Color Color { get { return m_color; } }

        #region IHasRect Members

        /// <summary>
        /// The rectangular bounds of this item
        /// </summary>
        public RectangleF Rectangle { get { return m_rectangle; } }

        #endregion

        public IGeometricShape GetGeometricShape()
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(long gameElapsedTime)
        {
            throw new NotImplementedException();
        }

        public ArkanoidGame.Geometry.Vector2D Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double ObjectWidth
        {
            get { throw new NotImplementedException(); }
        }

        public double ObjectHeight
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.Generic.IList<ArkanoidGame.Renderer.GameBitmap> ObjectTextures
        {
            get { throw new NotImplementedException(); }
        }

        public ArkanoidGame.Geometry.Vector2D Velocity
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBall
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPlayerPaddle
        {
            get { throw new NotImplementedException(); }
        }
    }
}
