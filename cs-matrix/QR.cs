using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class QR
    {
        /// <summary>
        /// Decompose A = QR
        /// </summary>
        /// <param name="A">A m x n matrix, m >= n</param>
        /// <param name="Q">A m x n orthogonal matrix, this is a column orthonormal matrix and has a property that Q.transpose * Q = I as the column vectors in Q are orthogonal normal vectors </param>
        /// <param name="R">A n x n matrix, which is upper triangular matrix if A is invertiable</param>
        public static void Factorize(IMatrix A, out IMatrix Q, out IMatrix R)
        {
            List<IVector> Rcols;
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);

            List<IVector> vstarlist = Orthogonalization.Orthogonalize(Acols, out Rcols);

            List<double> norms;
            List<IVector> qlist = VectorUtils.Normalize(vstarlist, out norms);

            Q = MatrixUtils.GetMatrix(qlist, A.DefaultValue);


            R = MatrixUtils.GetMatrix(Rcols, A.DefaultValue);
            foreach(int r in R.RowKeys)
            {
                R[r] = R[r].Multiply(norms[r]);
            }
        }
    }
}
