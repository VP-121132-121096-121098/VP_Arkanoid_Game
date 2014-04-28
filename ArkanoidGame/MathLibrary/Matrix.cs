using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLibrary
{
    public static class Matrix
    {
        //********************************************************/
        // Function for double values
        //********************************************************/
        public static double[,] Multiply(double[,] matrix1, double[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            double[,] ansMat;

            if (mCol1 == mRow2)
            {
                ansMat = new double[mRow1, mCol2];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol2; j++)
                    {
                        for (int k = 0; k < mRow2; k++)
                        {
                            ansMat[i, j] += matrix1[i, k] * matrix2[k, j];
                        }
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for multiplication");
            }
        }

        public static double[,] ScalarMultiply(double scalar, double[,] matrix)
        {
            double[,] ansMat;
            int mRow = matrix.GetLength(0);
            int mCol = matrix.GetLength(1);

            ansMat = new double[mRow, mCol];
            for (int i = 0; i < mRow; i++)
            {
                for (int j = 0; j < mCol; j++)
                {
                    ansMat[i, j] = scalar * matrix[i, j];
                }
            }
            return ansMat;
        }

        public static double[,] Add(double[,] matrix1, double[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            double[,] ansMat;

            if (mCol1 == mCol2 && mRow1 == mRow2)
            {
                ansMat = new double[mRow1, mCol1];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol1; j++)
                    {
                        ansMat[i, j] = matrix1[i, j] + matrix2[i, j];
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for Addition");
            }
        }

        public static double[,] Substract(double[,] matrix1, double[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            double[,] ansMat;

            if (mCol1 == mCol2 && mRow1 == mRow2)
            {
                ansMat = new double[mRow1, mCol1];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol1; j++)
                    {
                        ansMat[i, j] = matrix1[i, j] - matrix2[i, j];
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for Substraction");
            }
        }

        public static double Determinant(double[,] matrix)
        {
            double ans = 0;
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                if (length > 2)
                {
                    double[,] tempMat = new double[length - 1, length - 1];
                    for (int j = 0; j < length; j++)
                    {
                        int x = 0, y;
                        for (int i1 = 1; i1 < length; i1++)
                        {
                            y = 0;
                            for (int j1 = 0; j1 < length; j1++)
                            {
                                if (j1 != j)
                                {
                                    tempMat[x, y] = matrix[i1, j1];
                                    y++;
                                }
                            }
                            x++;
                        }
                        ans += Math.Pow(-1, j) * matrix[0, j] * Determinant(tempMat);
                    }
                    return ans;
                }
                else if (length == 2)
                {
                    ans = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
                    return ans;
                }
                else if (length == 1)
                {
                    return matrix[0, 0];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new MatrixException("This Matrix doesn't has Determinant");
            }
        }

        /*!
         * Transpose current matrix
         */
        public static void Transpose(double[,] matrix)
        {
            double tempVal;
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                for (int i = 0; i < length; i++)
                {
                    for (int j = i; j < length; j++)
                    {
                        tempVal = matrix[i, j];
                        matrix[i, j] = matrix[j, i];
                        matrix[j, i] = tempVal;
                    }
                }
            }
            else
            {
                throw new MatrixException("This Matrix can't be transpose");
            }
        }

        public static double[,] Inverse(double[,] matrix)
        {
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                if (length > 1)
                {
                    double[,] tempAnsMat = new double[length, length];
                    double ans = 0;

                    double[,] tempMat = new double[length - 1, length - 1];
                
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            int x = 0, y;
                            for (int i1 = 0; i1 < length; i1++)
                            {
                                if (i != i1)
                                {
                                    y = 0;
                                    for (int j1 = 0; j1 < length; j1++)
                                    {
                                        if (j1 != j)
                                        {
                                            tempMat[x, y] = matrix[i1, j1];
                                            y++;
                                        }
                                    }
                                    x++;
                                }
                            }
                            //It is saved as transpose matrix in temperary matrix
                            tempAnsMat[j, i] = Math.Pow(-1, i + j) * Determinant(tempMat);
                            if (i == 0)
                                ans += matrix[i, j] * tempAnsMat[j, i];
                        }
                    }
                    if (ans != 0)
                        return ScalarMultiply(1 / ans, tempAnsMat);
                    else
                        throw new MatrixException("This matrix Determiant is 0. no inverse matrix");
                }
                else if (length == 1)
                    return new double[,] { { 0 } };
                else
                    throw new MatrixException("This is a Null matrix");
            }
            else
            {
                throw new MatrixException("This Matrix can't be inverse");
            }
        }

        //********************************************************/
        // Function for float values
        //********************************************************/
        public static float[,] Multiply(float[,] matrix1, float[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            float[,] ansMat;

            if (mCol1 == mRow2)
            {
                ansMat = new float[mRow1, mCol2];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol2; j++)
                    {
                        for (int k = 0; k < mRow2; k++)
                        {
                            ansMat[i, j] += matrix1[i, k] * matrix2[k, j];
                        }
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for multiplication");
            }
        }

        public static float[,] ScalarMultiply(float scalar, float[,] matrix)
        {
            float[,] ansMat;
            int mRow = matrix.GetLength(0);
            int mCol = matrix.GetLength(1);

            ansMat = new float[mRow, mCol];
            for (int i = 0; i < mRow; i++)
            {
                for (int j = 0; j < mCol; j++)
                {
                    ansMat[i, j] = scalar * matrix[i, j];
                }
            }
            return ansMat;
        }

        public static float[,] Add(float[,] matrix1, float[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            float[,] ansMat;

            if (mCol1 == mCol2 && mRow1 == mRow2)
            {
                ansMat = new float[mRow1, mCol1];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol1; j++)
                    {
                        ansMat[i, j] = matrix1[i, j] + matrix2[i, j];
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for Addition");
            }
        }

        public static float[,] Substract(float[,] matrix1, float[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            float[,] ansMat;

            if (mCol1 == mCol2 && mRow1 == mRow2)
            {
                ansMat = new float[mRow1, mCol1];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol1; j++)
                    {
                        ansMat[i, j] = matrix1[i, j] - matrix2[i, j];
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for Substraction");
            }
        }

        public static float Determinant(float[,] matrix)
        {
            float ans = 0;
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                if (length > 2)
                {
                    float[,] tempMat = new float[length - 1, length - 1];
                    for (int j = 0; j < length; j++)
                    {
                        int x = 0, y;
                        for (int i1 = 1; i1 < length; i1++)
                        {
                            y = 0;
                            for (int j1 = 0; j1 < length; j1++)
                            {
                                if (j1 != j)
                                {
                                    tempMat[x, y] = matrix[i1, j1];
                                    y++;
                                }
                            }
                            x++;
                        }
                        ans += (float)Math.Pow(-1, j) * matrix[0, j] * Determinant(tempMat);
                    }
                    return ans;
                }
                else if(length == 2)
                {
                    ans = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
                    return ans;
                }
                else if (length == 1)
                {
                    return matrix[0, 0];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new MatrixException("This Matrix doesn't has Determinant");
            }
        }

        /*!
         * Transpose current matrix
         */
        public static void Transpose(float[,] matrix)
        {
            float tempVal;
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                for (int i = 0; i < length; i++)
                {
                    for (int j = i; j < length; j++)
                    {
                        tempVal = matrix[i, j];
                        matrix[i, j] = matrix[j, i];
                        matrix[j, i] = tempVal;
                    }
                }
            }
            else
            {
                throw new MatrixException("This Matrix can't be transpose");
            }
        }

        public static float[,] Inverse(float[,] matrix)
        {
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                if (length > 1)
                {
                    float[,] tempAnsMat = new float[length, length];
                    float ans = 0;

                    float[,] tempMat = new float[length - 1, length - 1];
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            int x = 0, y;
                            for (int i1 = 0; i1 < length; i1++)
                            {
                                if (i != i1)
                                {
                                    y = 0;
                                    for (int j1 = 0; j1 < length; j1++)
                                    {
                                        if (j1 != j)
                                        {
                                            tempMat[x, y] = matrix[i1, j1];
                                            y++;
                                        }
                                    }
                                    x++;
                                }
                            }
                            //It is saved as transpose matrix in temperary matrix
                            tempAnsMat[j, i] = (float)Math.Pow(-1, i + j) * Determinant(tempMat);
                            if (i == 0)
                                ans += matrix[i, j] * tempAnsMat[j, i];
                        }
                    }
                    if (ans != 0)
                        return ScalarMultiply(1 / ans, tempAnsMat);
                    else
                        throw new MatrixException("This matrix Determiant is 0. no inverse matrix");                
                }
                else if (length == 1)
                    return new float[,] { { 0 } };
                else
                    throw new MatrixException("This is a Null matrix");
            }
            else
            {
                throw new MatrixException("This Matrix can't be inverse");
            }
        }

        //********************************************************/
        // Function for int values
        //********************************************************/
        public static int[,] Multiply(int[,] matrix1, int[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            int[,] ansMat;

            if (mCol1 == mRow2)
            {
                ansMat = new int[mRow1, mCol2];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol2; j++)
                    {
                        for (int k = 0; k < mRow2; k++)
                        {
                            ansMat[i, j] += matrix1[i, k] * matrix2[k, j];
                        }
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for multiplication");
            }
        }

        public static int[,] ScalarMultiply(int scalar, int[,] matrix)
        {
            int[,] ansMat;
            int mRow = matrix.GetLength(0);
            int mCol = matrix.GetLength(1);

            ansMat = new int[mRow, mCol];
            for (int i = 0; i < mRow; i++)
            {
                for (int j = 0; j < mCol; j++)
                {
                    ansMat[i, j] = scalar * matrix[i, j];
                }
            }
            return ansMat;
        }

        public static double[,] ScalarMultiply(double scalar, int[,] matrix)
        {
            double[,] ansMat;
            int mRow = matrix.GetLength(0);
            int mCol = matrix.GetLength(1);

            ansMat = new double[mRow, mCol];
            for (int i = 0; i < mRow; i++)
            {
                for (int j = 0; j < mCol; j++)
                {
                    ansMat[i, j] = scalar * (double)matrix[i, j];
                }
            }
            return ansMat;
        }

        public static int[,] Add(int[,] matrix1, int[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            int[,] ansMat;

            if (mCol1 == mCol2 && mRow1 == mRow2)
            {
                ansMat = new int[mRow1, mCol1];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol1; j++)
                    {
                        ansMat[i, j] = matrix1[i, j] + matrix2[i, j];
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for Addition");
            }
        }

        public static int[,] Substract(int[,] matrix1, int[,] matrix2)
        {
            int mRow1 = matrix1.GetLength(0);
            int mCol1 = matrix1.GetLength(1);
            int mRow2 = matrix2.GetLength(0);
            int mCol2 = matrix2.GetLength(1);

            int[,] ansMat;

            if (mCol1 == mCol2 && mRow1 == mRow2)
            {
                ansMat = new int[mRow1, mCol1];
                for (int i = 0; i < mRow1; i++)
                {
                    for (int j = 0; j < mCol1; j++)
                    {
                        ansMat[i, j] = matrix1[i, j] - matrix2[i, j];
                    }
                }
                return ansMat;
            }
            else
            {
                throw new MatrixException("Matrices are not supported for Substraction");
            }
        }

        public static int Determinant(int[,] matrix)
        {
            int ans = 0;
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                if (length > 2)
                {
                    int[,] tempMat = new int[length - 1, length - 1];
                    for (int j = 0; j < length; j++)
                    {
                        int x = 0, y;
                        for (int i1 = 1; i1 < length; i1++)
                        {
                            y = 0;
                            for (int j1 = 0; j1 < length; j1++)
                            {
                                if (j1 != j)
                                {
                                    tempMat[x, y] = matrix[i1, j1];
                                    y++;
                                }
                            }
                            x++;
                        }
                        ans += (int)Math.Pow(-1, j) * matrix[0, j] * Determinant(tempMat);
                    }
                    return ans;
                }
                else if (length == 2)
                {
                    ans = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
                    return ans;
                }
                else if (length == 1)
                {
                    return matrix[0, 0];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new MatrixException("This Matrix doesn't has Determinant");
            }
        }

        /*!
         * Transpose current matrix
         */
        public static void Transpose(int[,] matrix)
        {
            int tempVal;
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                for (int i = 0; i < length; i++)
                {
                    for (int j = i; j < length; j++)
                    {
                        tempVal = matrix[i, j];
                        matrix[i, j] = matrix[j, i];
                        matrix[j, i] = tempVal;
                    }
                }
            }
            else
            {
                throw new MatrixException("This Matrix can't be transpose");
            }
        }

        public static double[,] Inverse(int[,] matrix)
        {
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                int length = matrix.GetLength(0);
                if (length > 1)
                {
                    double[,] tempAnsMat = new double[length, length];
                    double ans = 0;

                    int[,] tempMat = new int[length - 1, length - 1];
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < length; j++)
                        {
                            int x = 0, y;
                            for (int i1 = 0; i1 < length; i1++)
                            {
                                if (i != i1)
                                {
                                    y = 0;
                                    for (int j1 = 0; j1 < length; j1++)
                                    {
                                        if (j1 != j)
                                        {
                                            tempMat[x, y] = matrix[i1, j1];
                                            y++;
                                        }
                                    }
                                    x++;
                                }
                            }
                            //It is saved as transpose matrix in temperary matrix
                            tempAnsMat[j, i] = Math.Pow(-1, i + j) * Determinant(tempMat);
                            if (i == 0)
                                ans += matrix[i, j] * tempAnsMat[j, i];
                        }
                    }
                    if (ans != 0)
                        return ScalarMultiply(1 / ans, tempAnsMat);
                    else
                        throw new MatrixException("This matrix Determiant is 0. no inverse matrix");
                }
                else if (length == 1)
                    return new double[,] { { 0 } };
                else
                    throw new MatrixException("This is a Null matrix");
            }
            else
            {
                throw new MatrixException("This Matrix can't be inverse");
            }
        }
    }

    public class MatrixException : Exception
    {
        public MatrixException()
        {
            errorMsg = "Input matrix is wrong";
        }
        public MatrixException(string message)
        {
            errorMsg = message;
        }
        public string GetErrorMessage()
        {
            return errorMsg;
        }
        private string errorMsg;
    }
}
