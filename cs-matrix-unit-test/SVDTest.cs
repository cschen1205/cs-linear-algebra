using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cs_matrix;
using Xunit;

namespace SimuKit.Math.LinAlg.UT
{
    public class SVDTest
    {
        public static double[][] Data = new double[][]{
            new double[] { 25, 15, -5},
            new double[] {15, 18, 0},
            new double[] {-5, 0, 11}
        };

        public static IMatrix A = new SparseMatrix(Data);

        [Fact]
        public void TestSVD()
        {
            IMatrix Sigma, U, Vstar;
            SVD.Factorize(A, out U, out Sigma, out Vstar);

        }

        [Fact]
        public void TestMatrixInverse()
        {
            IMatrix Ainv = SVDSolver.Invert(A);
            IMatrix Ipi = A.Multiply(Ainv);

            for (int row = 0; row < 2; ++row)
            {
                Console.WriteLine(Ipi[row]);
            }

            Assert.Equal(A.Identity(3), Ipi);

            Ipi = Ainv.Multiply(A);

            Assert.Equal(A.Identity(3), Ipi);
        }
    }
}
