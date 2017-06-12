using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class Cholesky<Val>
    {
        /// <summary>
        /// Decompose a positive definite matrix A into the multiplication of a lower triangular matrix and its tranpose
        /// </summary>
        /// <param name="A">A is a positive definite matrix of order n</param>
        /// <param name="L">A lower triangle such that A = L * L.transpose</param>
        public static void Factorize(IMatrix<int, Val> A, out IMatrix<int, Val> L)
        {
            int n = A.RowCount;
            Debug.Assert(n == A.ColCount);

            L = new SparseMatrix<int, Val>(n, n, A.DefaultValue);

            _Factorize(A, L, 0, n);
        }

        private static void _Factorize(IMatrix<int, Val> A, IMatrix<int, Val> L, int i, int n)
        {
            if (i == n)
            {
                return;
            }

            int[] subRows = new int[A.RowCount - 1];
            int[] subCols = new int[A.ColCount - 1];
            for (int j = i + 1; j < n; ++j)
            {
                subRows[j - i - 1] = j;
            }
            for (int j = i + 1; j < n; ++j)
            {
                subCols[j - i - 1] = j;
            }

            IMatrix<int, Val> A_22 = A.SubMatrix(subRows, subCols);
            

            if (A.HasValue(i, i))
            {
                double a_11 = (dynamic)A[i, i];
               

                double l_11 = System.Math.Sqrt(a_11); // l_{11} = sqrt(a_{11})
                L[i, i] = (dynamic)l_11;

                // L_{21} = A_{21} / l_{11}
                IVector<int, Val> L_21 = new SparseVector<int, Val>(subRows, A.DefaultValue);
                L_21.ID = i;
                for (int j = i + 1; j < n; ++j)
                {
                    L_21[j] = (dynamic)A[j, i] / (dynamic)l_11;
                    L[j, i] = L_21[j];
                }

                int[] kk = L_21.NonEmptyKeys.ToArray();
                for (int rk = 0; rk < kk.Length; ++rk)
                {
                    int row = kk[rk];
                    for (int ck = 0; ck < kk.Length; ++ck)
                    {
                        int col = kk[ck];
                        A_22[row, col] = (dynamic)A_22[row, col] - (dynamic)L_21[row] * (dynamic)L_21[col];
                    }
                }
            }

            _Factorize(A_22, L, i + 1, n);
        }
    }
}
