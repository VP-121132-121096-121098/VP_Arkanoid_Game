using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.GameLogic
{
    public class Vector2D
    {
        //koordinati
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D(float x, float y)
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
        public static float operator *(Vector2D vec1, Vector2D vec2)
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
            return new Vector2D(vec.X * (float)scalar, vec.Y * (float)scalar);
        }

        public static Vector2D operator *(double scalar, Vector2D vec)
        {
            return vec * scalar;
        }

        public static Vector2D operator -(Vector2D vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }

        //division with scalar
        public static Vector2D operator /(Vector2D vec, double scalar)
        {
            return new Vector2D(vec.X / (float)scalar, vec.Y / (float)scalar);
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(this * this);
        }
    }
}
