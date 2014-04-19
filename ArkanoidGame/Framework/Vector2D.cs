using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Framework
{
    public class Vector2D
    {
        //koordinati
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2D(Vector2D vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
        }

        public override string ToString()
        {
            return string.Format("({0:0.00}, {1:0.00})", X, Y);
        }

        //dot product
        public static double operator *(Vector2D vec1, Vector2D vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }

        //addition
        public static Vector2D operator +(Vector2D vec1, Vector2D vec2)
        {
            return new Vector2D(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        //subtraction
        public static Vector2D operator -(Vector2D vec1, Vector2D vec2)
        {
            return new Vector2D(vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        //vector and scalar multiplication
        public static Vector2D operator *(Vector2D vec, double scalar)
        {
            return new Vector2D(vec.X * scalar, vec.Y * scalar);
        }

        public static Vector2D operator *(double scalar, Vector2D vec)
        {
            return vec * scalar;
        }

        public static Vector2D operator -(Vector2D vec)
        {
            Vector2D temp = new Vector2D(-vec.X, -vec.Y);
            return temp;
        }

        //division with scalar
        public static Vector2D operator /(Vector2D vec, double scalar)
        {
            return new Vector2D(vec.X / scalar, vec.Y / scalar);
        }

        public double Magnitude()
        {
            return Math.Sqrt(this * this);
        }
    }
}
