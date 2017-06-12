using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class QRSolver
    {
        /// <summary>
        /// Solve x for Ax = b, where A is a invertible matrix
        /// 
        /// The method works as follows:
        ///  1. QR factorization A = Q * R
        ///  2. Let: c = Q.transpose * b, we have R * x = c, where R is a triangular matrix if A is n x n
        ///  3. Solve x using backward substitution
        /// </summary>
        /// <param name="A">An m x n matrix with linearly independent columns, where m >= n</param>
        /// <param name="b">An m x 1 column vector </param>
        /// <returns>An n x 1 column vector, x, such that Ax = b when m = n and Ax ~ b when m > n </returns>
        public static IVector Solve(IMatrix A, IVector b)
        {
            // Q is a m x m matrix, R is a m x n matrix
            // Q = [Q1 Q2] Q1 is a m x n matrix, Q2 is a m x (m-n) matrix
            // R = [R1; 0] R1 is a n x n upper triangular matrix matrix
            // A = Q * R = Q1 * R1
            IMatrix Q, R;
            QR.Factorize(A, out Q, out R);
            IVector c = Q.Transpose().Multiply(b);

            IVector x = BackwardSubstitution.Solve(R, c);
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
        ///   2. Decompose C : C = Q * R, we have Q * R * x = A.transpose * b
        ///   3. Multiply both side by Q.transpose = Q.inverse, we have Q.transpose * Q * R * x = Q.transpose * A.transpose * b
        ///   4. Since Q.tranpose * Q = I, we have R * x = Q.transpose * A.transpose * b
        ///   5. Solve x from R * x = Q.transpose * A.transpose * b using backward substitution
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IVector SolveLeastSquare(IMatrix A, IVector b)
        {
            IMatrix At = A.Transpose();
            IMatrix C = At.Multiply(A); //C is a n x n matrix
            
            IMatrix Q, R;
            QR.Factorize(C, out Q, out R);

            IMatrix Qt = Q.Transpose();

            IVector d = Qt.Multiply(At).Multiply(b);

            IVector x = BackwardSubstitution.Solve(R, d);

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
