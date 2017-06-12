using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public static class MatrixUtils
    {
        /// <summary>
        /// The method works by using Gaussian elimination to covert the matrix A to a upper triangular matrix, U, and computes the
        /// determinant as the product_i(U_ii) * (-1)^c, where c is the number of row exchange operations that coverts A to U
        /// </summary>
        /// <param name="A">The matrix for which to calculate determinant</param>
        /// <returns>The determinant of A</returns>
        public static double GetDeterminant(IMatrix A)
        {
            int colCount = A.ColCount;
            int rowCount = A.RowCount;
            Debug.Assert(colCount == rowCount);

            double det = 1;

            int rowExchangeOpCount = 0;
            IMatrix C = GaussianElimination.GetEchelonForm(A, out rowExchangeOpCount);
            
            foreach(int c in C.ColKeys)
            {
                det *= C[c, c];
            }

            return det * (rowExchangeOpCount % 2 == 0 ? 1 : -1);
        }

        /// <summary>
        /// Convert a list of column vectors into a matrix
        /// </summary>
        /// <param name="R"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static IMatrix GetMatrix(List<IVector> R, double default_value)
        {
            int n = R.Count;
            int m = R[0].Dimension;

            IMatrix T = new SparseMatrix(m, n, default_value);
            for (int c = 0; c < R.Count; ++c)
            {
                IVector Rcol = R[c];
                foreach (int r in Rcol.Keys)
                {
                    T[r, c] = Rcol[r];
                }
            }
            return T;
        }

        /// <summary>
        /// Convert a list of column vectors into a matrix
        /// </summary>
        /// <param name="R"></param>
        /// <returns></returns>
        public static IMatrix GetMatrix(List<IVector> R)
        {
            return GetMatrix(R, 0);
        }

        public static List<IVector> GetColumnVectors(IMatrix A)
        {
            int n = A.ColCount;
            int rowCount = A.RowCount;

            List<IVector> Acols = new List<IVector>();

            for (int c = 0; c < n; ++c)
            {
                IVector Acol = new SparseVector(rowCount, A.DefaultValue);
                for (int r = 0; r < rowCount; ++r)
                {
                    Acol[r] = A[r, c];
                }
                Acols.Add(Acol);
            }
            return Acols;
        }
    }
}
