using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Geometry
{
    public class Vector3D
    {

        //koordinati
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vector3D(Vector3D vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        public Vector3D(Vector2D vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = 0;
        }

        //dot product
        public static double operator *(Vector3D vec1, Vector3D vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        //vector and scalar multiplication
        public static Vector3D operator *(Vector3D vec, double scalar)
        {
            return new Vector3D(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }

        public static Vector3D operator *(double scalar, Vector3D vec)
        {
            return (vec * scalar);
        }

        //addition
        public static Vector3D operator +(Vector3D vec1, Vector3D vec2)
        {
            return new Vector3D(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
        }

        //subtraction
        public static Vector3D operator -(Vector3D vec1, Vector3D vec2)
        {
            return new Vector3D(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
        }

        public static Vector3D operator -(Vector3D vec)
        {
            return new Vector3D(-vec.X, -vec.Y, -vec.Z);
        }

        public static Vector3D operator /(Vector3D vec1, double scalar)
        {
            return new Vector3D(vec1.X / scalar, vec1.Y / scalar, vec1.Z / scalar);
        }

        public double Magnitude()
        {
            return Math.Sqrt(this * this);
        }

        public Vector3D CrossProduct(Vector3D vec)
        {
            double x = this.Y * vec.Z - this.Z * vec.Y;
            double y = -(this.X * vec.Z - this.Z * vec.X);
            double z = this.X * vec.Y - this.Y * vec.X;
            return new Vector3D(x, y, z);
        }

        public override string ToString()
        {
            return string.Format("{0:0.00}, {1:0.00}, {2:0.00}", X, Y, Z);
        }

        public double[,] ToMatrix()
        {
            double[,] matrix = new double[3, 1];
            matrix[0, 0] = this.X;
            matrix[1, 0] = this.Y;
            matrix[2, 0] = this.Z;
            return matrix;
        }
    }
}
