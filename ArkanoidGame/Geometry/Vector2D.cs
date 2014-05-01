using DotNetMatrix;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Geometry
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

        public static implicit operator Vector3D(Vector2D vec)
        {
            return new Vector3D(vec);
        }

        public static implicit operator Point(Vector2D vec)
        {
            return new Point((int)Math.Round(vec.X), (int)Math.Round(vec.Y));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            Vector2D vec = (Vector2D)obj;
            return this.X == vec.X && this.Y == vec.Y;
        }

        public static bool operator ==(Vector2D vec1, Vector2D vec2)
        {
            if (ReferenceEquals(vec1, vec2))
                return true;

            return vec1.Equals(vec2);
        }

        public static bool operator !=(Vector2D vec1, Vector2D vec2)
        {
            return !(vec1 == vec2);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() + Y.GetHashCode()) % int.MaxValue;
        }

        public GeneralMatrix ToMatrix()
        {
            double[][] matrix = new double[2][];
            matrix[0] = new double[1];
            matrix[1] = new double[1];
            matrix[0][0] = X;
            matrix[1][0] = Y;
            return new GeneralMatrix(matrix);
        }

        public void Rotate(double angleInRadians)
        {
            /* http://mathworld.wolfram.com/RotationMatrix.html */

            /* матрица на ротација
            *             |-               -|
            *             | cos(a)  -sin(a) |
            *      R(a) = |                 |
            *             | sin(a)   cos(a) |
            *             |-               -|
            */

            //V = R(a) * V0
            double[][] temp = new double[2][];
            temp[0] = new double[2];
            temp[1] = new double[2];
            temp[0][0] = Math.Cos(angleInRadians);
            temp[0][1] = -Math.Sin(angleInRadians);
            temp[1][0] = Math.Sin(angleInRadians);
            temp[1][1] = Math.Cos(angleInRadians);

            GeneralMatrix rotationMatrix = new GeneralMatrix(temp);
            GeneralMatrix newVector = rotationMatrix.Multiply(this.ToMatrix());
            this.X = newVector.GetElement(0, 0);
            this.Y = newVector.GetElement(1, 0);
        }

        public void RotateDeg(double degrees)
        {
            this.Rotate(degrees * Math.PI / 180.0);
        }
    }
}
