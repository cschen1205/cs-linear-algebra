using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class QR<Val>
    {
        /// <summary>
        /// Decompose A = QR
        /// </summary>
        /// <param name="A">A m x n matrix, m >= n</param>
        /// <param name="Q">A m x n orthogonal matrix, this is a column orthonormal matrix and has a property that Q.transpose * Q = I as the column vectors in Q are orthogonal normal vectors </param>
        /// <param name="R">A n x n matrix, which is upper triangular matrix if A is invertiable</param>
        public static void Factorize(IMatrix<int, Val> A, out IMatrix<int, Val> Q, out IMatrix<int, Val> R)
        {
            List<IVector<int, Val>> Rcols;
            List<IVector<int, Val>> Acols = MatrixUtils<int, Val>.GetColumnVectors(A);

            List<IVector<int, Val>> vstarlist = Orthogonalization<int, Val>.Orthogonalize(Acols, out Rcols);

            List<double> norms;
            List<IVector<int, Val>> qlist = VectorUtils<int, Val>.Normalize(vstarlist, out norms);

            Q = MatrixUtils<int, Val>.GetMatrix(qlist, A.DefaultValue);


            R = MatrixUtils<int, Val>.GetMatrix(Rcols, A.DefaultValue);
            foreach(int r in R.RowKeys)
            {
                R[r] = R[r].Multiply(norms[r]);
            }
        }
    }
}
