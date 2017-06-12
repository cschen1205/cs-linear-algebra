using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class SVDSolver
    {
        public static IMatrix Invert(IMatrix A) // pseudo matrix inversion
        {
			// A is a m x n matrix
			// U is a m x m orthogonal matrix
			// Sigma is a m x n matrix
			// V is a n x n orthogonal matrix
            IMatrix U, Sigma, Vstar;
            SVD.Factorize(A, out U, out Sigma, out Vstar);

            IMatrix V = Vstar.Transpose();
            IMatrix Uinv = U.Transpose();

			int m = Sigma.RowCount;
			int n = Sigma.ColCount;
			//SigmaInv is obtained by replacing every non-zero diagonal entry by its reciprocal and transposing the resulting matrix
            IMatrix SigmaInv = new SparseMatrix(m, n, Sigma.DefaultValue);
            for (int i = 0; i < n; ++i) // m >= n
            {
                double entry = Sigma[i, i];
                if (entry != 0)
                {
                    SigmaInv[i, i] = (1.0 / entry);
                }
            }
			SigmaInv = SigmaInv.Transpose();

            IMatrix Ainv = V.Multiply(SigmaInv).Multiply(Uinv); 
            return Ainv;
        }
    }
}
