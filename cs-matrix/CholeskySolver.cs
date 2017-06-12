using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class CholeskySolver
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
        public static IVector Solve(IMatrix A, IVector b)
        {
            IMatrix L; 
            Cholesky.Factorize(A, out L);

            IMatrix U = L.Transpose(); // upper triangular matrix 

            IVector y = ForwardSubstitution.Solve(L, b);

            IVector x = BackwardSubstitution.Solve(U, y);

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
        public static IVector SolveLeastSquare(IMatrix A, IVector b)
        {
            IMatrix At = A.Transpose();
            IMatrix C = At.Multiply(A); //C is a n x n matrix
            IVector d = At.Multiply(b);

            IMatrix L;
            Cholesky.Factorize(C, out L);
            IMatrix U = L.Transpose();

            IVector z = ForwardSubstitution.Solve(L, d);
            IVector x = BackwardSubstitution.Solve(U, z);

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
        public static IVector SolveLeastNorm(IMatrix A, IVector b)
        {
            IMatrix At = A.Transpose();
            IMatrix C = A.Multiply(At);
            IVector z = Solve(C, b);
            IVector x_bar = At.Multiply(z);
            return x_bar;
        }

        public static IMatrix Invert(IMatrix A)
        {
            Debug.Assert(A.RowCount == A.ColCount);
            int n = A.RowCount;

            double one = 1;
            List<IVector> AinvCols = new List<IVector>();
            for (int i = 0; i < n; ++i)
            {
                IVector e_i = new SparseVector(n, A.DefaultValue);
                e_i[i] = one;
                IVector AinvCol = Solve(A, e_i);
                AinvCols.Add(AinvCol);
            }

            IMatrix Ainv = MatrixUtils.GetMatrix(AinvCols);

            return Ainv;
        }
    }
}
