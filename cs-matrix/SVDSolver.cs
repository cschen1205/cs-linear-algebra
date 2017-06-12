using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class SVDSolver<Val>
    {
        public static IMatrix<int, Val> Invert(IMatrix<int, Val> A) // pseudo matrix inversion
        {
			// A is a m x n matrix
			// U is a m x m orthogonal matrix
			// Sigma is a m x n matrix
			// V is a n x n orthogonal matrix
            IMatrix<int, Val> U, Sigma, Vstar;
            SVD<Val>.Factorize(A, out U, out Sigma, out Vstar);

            IMatrix<int, Val> V = Vstar.Transpose();
            IMatrix<int, Val> Uinv = U.Transpose();

			int m = Sigma.RowCount;
			int n = Sigma.ColCount;
			//SigmaInv is obtained by replacing every non-zero diagonal entry by its reciprocal and transposing the resulting matrix
            IMatrix<int, Val> SigmaInv = new SparseMatrix<int, Val>(m, n, Sigma.DefaultValue);
            for (int i = 0; i < n; ++i) // m >= n
            {
                double entry = (dynamic)Sigma[i, i];
                if (entry != 0)
                {
                    SigmaInv[i, i] = (dynamic)(1.0 / entry);
                }
            }
			SigmaInv = SigmaInv.Transpose();

            IMatrix<int, Val> Ainv = V.Multiply(SigmaInv).Multiply(Uinv); 
            return Ainv;
        }
    }
}
