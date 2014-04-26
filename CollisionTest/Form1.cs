using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CollisionTest
{
    public partial class Form1 : Form
    {
        private GameRectangle rectangle1;
        private GameRectangle rectangle2;
        private GameCircle circle;
        private GameCircle circle2;

        private bool mouseDown;

        public bool IsHit(IGeometricShape shape, Point cursor)
        {
            if (!mouseDown)
                return false;

            if (shape.ShapeType == GeometricShapeTypes.Circle)
            {
                GameCircle circle = (GameCircle)shape;
                Vector2D vec = new Vector2D(cursor.X, cursor.Y);
                return (vec - circle.Position).Magnitude() <= circle.Radius;
            }
            else
            {
                GameRectangle rectangle = (GameRectangle)shape;
                if (cursor.X >= rectangle.PositionUL.X && cursor.X <= rectangle.PositionUR.X
                    && cursor.Y >= rectangle.PositionUL.Y
                    && cursor.Y <= rectangle.PositionUL.Y + rectangle.Height)
                {
                    return true;
                }

                return false;
            }
        }

        public Form1()
        {
            mouseDown = false;
            InitializeComponent();
            rectangle1 = new GameRectangle(new Vector2D(30, 10), new Vector2D(120, 10),
                90, 30);
            rectangle2 = new GameRectangle(new Vector2D(100, 10), new Vector2D(190, 10), 90, 50);
            circle = new GameCircle(new Vector2D(200, 200), 20);
            circle2 = new GameCircle(new Vector2D(200, 250), 20);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Blue, (float)rectangle1.PositionUL.X, (float)rectangle1.PositionUL.Y,
                (float)rectangle1.Width, (float)rectangle1.Height);
            g.FillRectangle(Brushes.Red, (float)rectangle2.PositionUL.X, (float)rectangle2.PositionUL.Y,
                (float)rectangle2.Width, (float)rectangle2.Height);
            g.FillEllipse(Brushes.Green, (float)(circle.Position.X - circle.Radius),
                (float)(circle.Position.Y - circle.Radius), (float)(2 * circle.Radius),
                (float)(2 * circle.Radius));

            List<Vector2D> points = new List<Vector2D>();
            rectangle1.Intersects(rectangle2, out points);
            List<Vector2D> tempPoints = new List<Vector2D>();
            rectangle1.Intersects(circle, out tempPoints);
            points.AddRange(tempPoints);
            rectangle2.Intersects(circle, out tempPoints);
            points.AddRange(tempPoints);

            foreach (Vector2D point in points)
            {
                g.DrawEllipse(new Pen(Brushes.Black, 3), (float)point.X - 10, (float)point.Y - 10, 10, 10);
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            mouseDown = !mouseDown;
        }

        public void UpdateObjects()
        {
            
            if (IsHit(rectangle1, PointToClient(MousePosition)))
            {
                rectangle1 = new GameRectangle(new Vector2D((2 * PointToClient(MousePosition).X - rectangle1.Width) / 2,
                    (2 * PointToClient(MousePosition).Y - rectangle1.Height) / 2),
                    new Vector2D((2 * PointToClient(MousePosition).X - rectangle1.Width) / 2 + rectangle1.Width,
                        (2 * PointToClient(MousePosition).Y - rectangle1.Height) / 2),
                    rectangle1.Width, rectangle1.Height);
            }

            if (IsHit(rectangle2, PointToClient(MousePosition)))
            {
                rectangle2 = new GameRectangle(new Vector2D((2 * PointToClient(MousePosition).X - rectangle2.Width) / 2,
                    (2 * PointToClient(MousePosition).Y - rectangle2.Height) / 2),
                    new Vector2D((2 * PointToClient(MousePosition).X - rectangle2.Width) / 2 + rectangle2.Width,
                        (2 * PointToClient(MousePosition).Y - rectangle2.Height) / 2),
                    rectangle2.Width, rectangle2.Height);
            }

            if (IsHit(circle, PointToClient(MousePosition)))
            {
                circle = new GameCircle(new Vector2D(PointToClient(MousePosition).X,
                    PointToClient(MousePosition).Y), circle.Radius);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.UpdateObjects();
            this.Refresh();
        }
    }
}
