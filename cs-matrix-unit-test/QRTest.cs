using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cs_matrix;
using Xunit;

namespace SimuKit.Math.LinAlg.UT
{
    public class QRTest
    {
        public static double[][] Data = new double[][]{
                new double[] { 12, -51, 4},
                new double[] { 6, 167, -68},
                new double[] { -4, 24, -41}
            };
        public static SparseMatrix A = new SparseMatrix(Data);

        [Fact]
        public void TestQR()
        {
            IMatrix Q, R;
            QR.Factorize(A, out Q, out R);

            IMatrix Api = Q.Multiply(R);

            for (int r = 0; r < A.RowCount; ++r)
            {
                for (int c = 0; c < A.ColCount; ++c)
                {
                    Assert.True(System.Math.Abs(A[r, c] - Api[r, c]) < 1e-10);
                }
            }

            Assert.Equal(A, Q.Multiply(R));
        }

        [Fact]
        public void TestQRSolver()
        {
            IVector x = new SparseVector(new double[] { 2, 4, 1 });
            IVector b = A.Multiply(x);

            IVector x_pi = QRSolver.Solve(A, b);
            Assert.Equal(x, x_pi);
        }

        [Fact]
        public void TestMatrixInverse()
        {
            //A = new SparseMatrix(2, 2);

            //A[0, 0] = 4;
            //A[0, 1] = 7;
            //A[1, 0] = 2;
            //A[1, 1] = 6;

            IMatrix Ainv = QRSolver.Invert(A);
            IMatrix Ipi = A.Multiply(Ainv);

            //for (int row = 0; row < 2; ++row)
            //{
            //    Console.WriteLine(Ipi[row]);
            //}

            Assert.Equal(A.Identity(3), Ipi);

            Ipi = Ainv.Multiply(A);

            //for (int row = 0; row < 2; ++row)
            //{
            //    Console.WriteLine(Ipi[row]);
            //}

            Assert.Equal(A.Identity(3), Ipi);
        }
    }
}
