﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    /// <summary>
    /// Singular Value Decomposition
    /// </summary>
    /// <typeparam name="Val"></typeparam>
    public class SVD
    {
        /// <summary>
        /// Factorize A = U * Sigma * V.transpose
        /// Where:
        /// A is a m x n matrix
        /// U is a m x m orthogonal matrix, U.inverse = U.transpose
        /// V is a n x n orthogonal matrix, V.inverse = V.transpose
        /// Sigma is a m x n diagonal matrix
        /// 
        /// The method works like this:
        ///    1. C = A.transpose * A = (V * Sigma.transpose * U.transpose) * (U * Sigma * V.transpose) = V * Sigma.transpose * Sigma * V.transpose)
        ///    2. Eigen decompose C (which is now a symmetric matrix) : C = V' * T' * V'.transpose, where V' = V (since V' is orthogonal matrix for symmetric matrix C) and T' = Sigma.transpose * Sigma
        ///    3. We have V = V' and Sigma = Sqrt(T) 
        ///    4. By A * V = U * Sigma * V.transpose * V = U * Sigma, we can solve U = A * V * Sigma.inverse
        /// </summary>
        /// <param name="A"></param>
        /// <param name="U"></param>
        /// <param name="Sigma"></param>
        /// <param name="V"></param>
        public static void Factorize(IMatrix A, out IMatrix U, out IMatrix Sigma, out IMatrix Vstar)
        {
            int m = A.RowCount;
            int n = A.ColCount;

            IMatrix At = A.Transpose();
            IMatrix C = At.Multiply(A); // C is a n x n symmetric matrix now

            IMatrix T, V;
            QRAlgorithm.Factorize(C, out T, out V);

            Vstar = V.Transpose();

            Sigma = new SparseMatrix(m, n, A.DefaultValue);
            int D = System.Math.Min(m, n);
            for (int i = 0; i < D; ++i)
            {
                Sigma[i, i] = System.Math.Sqrt(T[i, i]);
            }

            IMatrix SigmaInv = new SparseMatrix(m, n, A.DefaultValue);
            for (int i = 0; i < D; ++i)
            {
                double entry = Sigma[i, i];
                if (entry != 0)
                {
                    SigmaInv[i, i] = (1.0 / entry);
                }
            }
            SigmaInv = SigmaInv.Transpose();

            U = A.Multiply(V).Multiply(SigmaInv);
        }
    }
}
