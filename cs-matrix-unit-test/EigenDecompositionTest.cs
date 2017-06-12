using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cs_matrix;
using Xunit;

namespace SimuKit.Math.LinAlg.UT
{
    public class EigenDecompositionTest
    {
        public static double[][] Data = new double[][]{
            new double[] { 25, 15, -5},
            new double[] {15, 18, 0},
            new double[] {-5, 0, 11}
        };

        public static IMatrix A = new SparseMatrix(Data);

        [Fact]
        public void TestQRAlgorithm()
        {
            IMatrix T, U;
            QRAlgorithm.Factorize(A, out T, out U, 20);
            
        }

        [Fact]
        public void TestMatrixInverse()
        {
            IMatrix Ainv = QRAlgorithm.InvertSymmetricMatrix(A);
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
