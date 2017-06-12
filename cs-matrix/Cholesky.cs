using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class Cholesky
    {
        /// <summary>
        /// Decompose a positive definite matrix A into the multiplication of a lower triangular matrix and its tranpose
        /// </summary>
        /// <param name="A">A is a positive definite matrix of order n</param>
        /// <param name="L">A lower triangle such that A = L * L.transpose</param>
        public static void Factorize(IMatrix A, out IMatrix L)
        {
            int n = A.RowCount;
            Debug.Assert(n == A.ColCount);

            L = new SparseMatrix(n, n, A.DefaultValue);

            _Factorize(A, L, 0, n);
        }

        private static void _Factorize(IMatrix A, IMatrix L, int i, int n)
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

            IMatrix A_22 = A.SubMatrix(subRows, subCols);
            

            if (A.HasValue(i, i))
            {
                double a_11 = A[i, i];
               

                double l_11 = System.Math.Sqrt(a_11); // l_{11} = sqrt(a_{11})
                L[i, i] = l_11;

                // L_{21} = A_{21} / l_{11}
                IVector L_21 = new SparseVector(subRows, A.DefaultValue);
                L_21.ID = i;
                for (int j = i + 1; j < n; ++j)
                {
                    L_21[j] = A[j, i] / l_11;
                    L[j, i] = L_21[j];
                }

                int[] kk = L_21.NonEmptyKeys.ToArray();
                for (int rk = 0; rk < kk.Length; ++rk)
                {
                    int row = kk[rk];
                    for (int ck = 0; ck < kk.Length; ++ck)
                    {
                        int col = kk[ck];
                        A_22[row, col] = A_22[row, col] - L_21[row] * L_21[col];
                    }
                }
            }

            _Factorize(A_22, L, i + 1, n);
        }
    }
}
