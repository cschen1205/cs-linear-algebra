using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class CholeskySolver<Val>
    {
        /// <summary>
        /// Given we want to find x such as A * x = b
        ///   1. Decompose A = L * U, where L is the lower triangular and U is the upper triangular
        ///   2. We have L * U * x = b, which can be rewritten as L * y = b, where U * x = y
        ///   3. Solve y for L * y = b using forward substitution
        ///   4. Solve x for U * x = y using backward substitution
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IVector<int, Val> Solve(IMatrix<int, Val> A, IVector<int, Val> b)
        {
            IMatrix<int, Val> L; 
            Cholesky<Val>.Factorize(A, out L);

            IMatrix<int, Val> U = L.Transpose(); // upper triangular matrix 

            IVector<int, Val> y = ForwardSubstitution<Val>.Solve(L, b);

            IVector<int, Val> x = BackwardSubstitution<Val>.Solve(U, y);

            return x;
        }

        /// <summary>
        /// This is used for data fitting / regression 
        /// A is a m x n matrix, where m >= n
        /// b is a m x 1 column vector
        /// The method solves for x, which is a n x 1 column vectors such that A * x is closest to b
        /// 
        /// The method works as follows:
        ///   1. Let C = A.transpose * A, we have A.transpose * A * x = C * x = A.transpose * b
        ///   2. Decompose C : C = L * L.transpose = L * U, we have L * U * x = A.transpose * b
        ///   3. Let z = U * x, we have L * z = A.transpose * b
        ///   4. Solve z using forward substitution
        ///   5. Solve x from U * x = z using backward substitution
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IVector<int, Val> SolveLeastSquare(IMatrix<int, Val> A, IVector<int, Val> b)
        {
            IMatrix<int, Val> At = A.Transpose();
            IMatrix<int, Val> C = At.Multiply(A); //C is a n x n matrix
            IVector<int, Val> d = At.Multiply(b);

            IMatrix<int, Val> L;
            Cholesky<Val>.Factorize(C, out L);
            IMatrix<int, Val> U = L.Transpose();

            IVector<int, Val> z = ForwardSubstitution<Val>.Solve(L, d);
            IVector<int, Val> x = BackwardSubstitution<Val>.Solve(U, z);

            return x;
        }

        /// <summary>
        /// This is for finding the least norm in solution set {x | A * x = b}
        /// where A is m x n matrix m &lt; n, where for A * x = b has many solutions.
        /// 
        /// The least-norm problem is defined as follows:
        /// Objective: min |x|^2
        /// Subject to: A * x = b
        /// 
        /// The unique solution to the least norm can be obtained as x_bar = A.tranpose * (A * A.transpose).inverse * b
        /// We can solve by the following method
        ///   1. Solve z such that (A * A.transpose) * z = b
        ///   2. x_bar = A.transpose * z
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IVector<int, Val> SolveLeastNorm(IMatrix<int, Val> A, IVector<int, Val> b)
        {
            IMatrix<int, Val> At = A.Transpose();
            IMatrix<int, Val> C = A.Multiply(At);
            IVector<int, Val> z = Solve(C, b);
            IVector<int, Val> x_bar = At.Multiply(z);
            return x_bar;
        }

        public static IMatrix<int, Val> Invert(IMatrix<int, Val> A)
        {
            Debug.Assert(A.RowCount == A.ColCount);
            int n = A.RowCount;

            Val one = (dynamic)1;
            List<IVector<int, Val>> AinvCols = new List<IVector<int, Val>>();
            for (int i = 0; i < n; ++i)
            {
                IVector<int, Val> e_i = new SparseVector<int, Val>(n, A.DefaultValue);
                e_i[i] = one;
                IVector<int, Val> AinvCol = Solve(A, e_i);
                AinvCols.Add(AinvCol);
            }

            IMatrix<int, Val> Ainv = MatrixUtils<int, Val>.GetMatrix(AinvCols);

            return Ainv;
        }
    }
}
