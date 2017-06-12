using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public static class MatrixUtils<Key, Val>
    {
        /// <summary>
        /// The method works by using Gaussian elimination to covert the matrix A to a upper triangular matrix, U, and computes the
        /// determinant as the product_i(U_ii) * (-1)^c, where c is the number of row exchange operations that coverts A to U
        /// </summary>
        /// <param name="A">The matrix for which to calculate determinant</param>
        /// <returns>The determinant of A</returns>
        public static double GetDeterminant(IMatrix<Key, Val> A)
        {
            int colCount = A.ColCount;
            int rowCount = A.RowCount;
            Debug.Assert(colCount == rowCount);

            double det = 1;

            int rowExchangeOpCount = 0;
            IMatrix<Key, Val> C = GaussianElimination.GetEchelonForm(A, out rowExchangeOpCount);
            
            foreach(Key c in C.ColKeys)
            {
                det *= (dynamic)C[c, c];
            }

            return det * (rowExchangeOpCount % 2 == 0 ? 1 : -1);
        }

        /// <summary>
        /// Convert a list of column vectors into a matrix
        /// </summary>
        /// <param name="R"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static IMatrix<int, Val> GetMatrix(List<IVector<int, Val>> R, Val default_value)
        {
            int n = R.Count;
            int m = R[0].Dimension;

            IMatrix<int, Val> T = new SparseMatrix<int, Val>(m, n, default_value);
            for (int c = 0; c < R.Count; ++c)
            {
                IVector<int, Val> Rcol = R[c];
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
        public static IMatrix<int, Val> GetMatrix(List<IVector<int, Val>> R)
        {
            return GetMatrix(R, (dynamic)0);
        }

        public static List<IVector<int, Val>> GetColumnVectors(IMatrix<int, Val> A)
        {
            int n = A.ColCount;
            int rowCount = A.RowCount;

            List<IVector<int, Val>> Acols = new List<IVector<int, Val>>();

            for (int c = 0; c < n; ++c)
            {
                IVector<int, Val> Acol = new SparseVector<int, Val>(rowCount, A.DefaultValue);
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
