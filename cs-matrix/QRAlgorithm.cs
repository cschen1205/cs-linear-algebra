using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    /// <summary>
    /// QR algorithm uses QR factorization to compute the eigen vector decomposition
    /// Currently only the basic QR algorithm is implemented, Hessenberg reduction will be introduced in the future
    /// </summary>
    /// <typeparam name="Val"></typeparam>
    public class QRAlgorithm
    {
        /// <summary>
        /// Given A which is a n x n symmetric matrix, we want to find U and T
        /// such that:
        ///   1. A = U * T * U.transpose
        ///   2. U is a n x n matrix whose columns are eigen vectors of A (A * x = lambda * x, where lambda is the eigen value, and x is the eigen vector)
        ///   3. T is a diagonal matrix whose diagonal entries are the eigen values
        ///   
        /// The method works in the following manner:
        ///   1. intialze A_0 = A, U_0 = I
        ///   2. iterate for k = 1, ... K, K is termination criteria
        ///   3. In each iteration k:
        ///      3.1 QR factorization to find Q_k and R_k such that A_{k-1}=Q_k * R_k
        ///      3.2 Let A_k = R_k * Q_k
        ///      3.3 Let U_k = U_{k-1} * Q_k
        ///   4. Set T = A_K, U = U_K
        ///   
        /// Note that U is an orthogonal matrix if A is a symmetric matrix, in other words, if A.transpose = A, then U.inverse = U.transpose
        /// </summary>
        /// <param name="A">The matrix to be factorized</param>
        /// <param name="K">maximum number of iterations</param>
        /// <param name="T">T is a diagonal matrix whose diagonal entries are the eigen values</param>
        /// <param name="U">U is a n x n matrix whose columns are eigen vectors of A</param>
        public static void Factorize(IMatrix A, out IMatrix T, out IMatrix U, int K = 100, double epsilon = 1e-10)
        {
            Debug.Assert(A.RowCount == A.ColCount);

            int n = A.RowCount;
            IMatrix A_k = A.Clone();
            IMatrix U_k = A.Identity(n);

            IMatrix Q_k, R_k;
            for (int k = 1; k <= K; ++k)
            {
                QR.Factorize(A_k, out Q_k, out R_k);
                A_k = R_k.Multiply(Q_k);
                U_k = U_k.Multiply(Q_k);

                double sum = 0;
                foreach (IVector rowVec in A_k.NonEmptyRows)
                {
                    int rowId = rowVec.ID;
                    foreach(int key in rowVec.NonEmptyKeys)
                    {
                        if (key == rowId)
                        {
                            continue;
                        }
                        sum += System.Math.Abs(rowVec[key]);
                    }
                }
                if (sum <= epsilon)
                {
                    break;
                }
            }

            T = new SparseMatrix(n,n, A.DefaultValue);

            for (int i = 0; i < n; ++i)
            {
                T[i, i] = A_k[i, i];
            }

            U = U_k;
        }

        /// <summary>
        /// Get the eigen values and corresponding eigen vectors for the square matrix A
        /// </summary>
        /// <param name="A"></param>
        /// <param name="eigenValues"></param>
        /// <param name="eigenVectors"></param>
        public static List<IVector>  FindEigenVectors(IMatrix A, out List<double> eigenValues, int K=100, double epsilon = 1e-10)
        {
            Debug.Assert(A.RowCount == A.ColCount);

            IMatrix T, U;
            Factorize(A, out T, out U, K, epsilon);

            int n = A.RowCount;

            eigenValues=new List<double>();

            for(int i=0; i < n; ++i)
            {
                eigenValues.Add(T[i, i]);
            }

            List<IVector> eigenVectors = MatrixUtils.GetColumnVectors(U);

            return eigenVectors;
        }

        /// <summary>
        /// Use eigen vector decomposition to invert a real symmetric matrix
        /// 
        /// The method works like this
        /// A * A.inverse = I
        /// Since A is symmetric, we have U.inverse = U.transpose (i.e. U is an orthogonal matrix, U consists of orthogonal eigenvectors)
        /// U * T * U.transpose * A.inverse = I, since A = U * T * U.transpose
        /// T * U.transpose * A.inverse = U.inverse
        /// U.transpose * A.inverse = T.inverse * U.inverse, since T.inverse = { 1 / lambda_ii } where T = { lambda_ii }
        /// A.inverse = U * T.inverse * U.inverse, since U.transpose = U.inverse
        /// A.inverse = U * T.inverse * U.transpose, since U.transpose = U.inverse
        /// </summary>
        /// <param name="A">a n x n square matrix</param>
        public static IMatrix InvertSymmetricMatrix(IMatrix A, int K = 100, double epsilon = 1e-10)
        {
            Debug.Assert(A.IsSymmetric);

            int n = A.RowCount;

            IMatrix T, U;
            Factorize(A, out T, out U, K, epsilon);
            
            IMatrix Tinv = new SparseMatrix(n, n, A.DefaultValue);
            for (int i = 0; i < n; ++i)
            {
                double lambda_ii = T[i, i];
                if (System.Math.Abs(lambda_ii) < epsilon)
                {
                    throw new Exception("The matrix is not invertiable");
                }
                Tinv[i, i] = (1 / lambda_ii);
            }

            IMatrix Uinv = U.Transpose();
            return U.Multiply(Tinv).Multiply(Uinv);
        }

        /// <summary>
        /// Get A^p = A * A * ... A for p times
        /// </summary>
        /// <param name="A"></param>
        /// <param name="p">The power term</param>
        /// <param name="K"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static IMatrix Power(IMatrix A, int p, int K = 100, double epsilon = 1e-10)
        {
            int n = A.RowCount;

            IMatrix T, U;
            Factorize(A, out T, out U, K, epsilon);

            for (int i = 0; i < n; ++i)
            {
                T[i, i] = System.Math.Pow(T[i, i], p);
            }

            return U.Multiply(T).Multiply(U.Transpose());
        }
    }
}
