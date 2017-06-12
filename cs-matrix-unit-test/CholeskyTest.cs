using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cs_matrix;
using Xunit;

namespace cs_matrix_unit_test
{
    public class CholeskyTest
    {
        public static double[][] Data = new double[][]{
            new double[] { 25, 15, -5},
            new double[] {15, 18, 0},
            new double[] {-5, 0, 11}
        };

        public static IMatrix A = new SparseMatrix(Data);

        [Fact]
        public void TestCholesky()
        {
            IMatrix L;
            Cholesky.Factorize(A, out L);

            Assert.Equal(A, L.Multiply(L.Transpose()));
        }

        [Fact]
        public void TestCholeskySolver()
        {
            IVector x = new SparseVector(new double[] { 2, 4, 1 });
            IVector b = A.Multiply(x);

            IVector x_pi = CholeskySolver.Solve(A, b);
            Assert.Equal(x, x_pi);
        }

        [Fact]
        public void TestMatrixInverse()
        {
            IMatrix Ainv = CholeskySolver.Invert(A);
            IMatrix Ipi = A.Multiply(Ainv);


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
