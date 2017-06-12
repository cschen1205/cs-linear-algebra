using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using cs_matrix;
using Xunit;

namespace cs_matrix_unit_test
{
    public class MatrixTest
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
        public void TestSymmetricMatrix()
        {
            Random random = new Random();
            int rowCount = random.Next(2, 20);
            int colCount = random.Next(2, 20);

            IMatrix C = new SparseMatrix(rowCount, colCount);
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < colCount; ++j)
                {
                    if (random.NextDouble() < 0.2)
                    {
                        C[i, j] = random.NextDouble() * 10 - 5;
                    }
                }
            }

            IMatrix M = C.Multiply(C.Transpose());
            Assert.True(M.IsSymmetric);
        }

        [Fact]
        public void TestSparseMatrix()
        {
            Assert.Equal(3, A.RowCount);
            Assert.Equal(3, A.ColCount);

            for (int r = 0; r < A.RowCount; ++r)
            {
                for (int c = 0; c < A.ColCount; ++c)
                {
                    Assert.Equal(A[r, c], Data[r][c]);
                }
            }
        }

        [Fact]
        public void TestGetColumnVectors()
        {
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);

            for (int r = 0; r < A.RowCount; ++r)
            {
                for (int c = 0; c < A.ColCount; ++c)
                {
                    Assert.Equal(Data[r][c], Acols[c][r]);
                }
            }
        }

        [Fact]
        public void TestGetMatrix()
        {
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);
            IMatrix Api = MatrixUtils.GetMatrix(Acols, 0);

            for (int r = 0; r < A.RowCount; ++r)
            {
                for (int c = 0; c < A.ColCount; ++c)
                {
                    Assert.Equal(Data[r][c], Api[r, c]);
                }
            }
        }

        [Fact]
        public void TestOrthogonalization()
        {
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);
            List<IVector> vstarlist = Orthogonalization.Orthogonalize(Acols);
            for (int i = 0; i < vstarlist.Count-1; ++i)
            {
                for (int j = i + 1; j < vstarlist.Count; ++j)
                {
                    Assert.True(vstarlist[i].Multiply(vstarlist[j]) < 1e-10);
                }
            }

        }

        [Fact]
        public void TestOrthoNormal()
        {
            List<IVector> Rcols;
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);
            List<IVector> vstarlist = Orthogonalization.Orthogonalize(Acols, out Rcols);

            for (int i = 0; i < Rcols.Count; ++i)
            {
                int count = 0;
                for (int j = 0; j < Rcols[i].Dimension; ++j)
                {
                    count += (System.Math.Abs(Rcols[i][j]) > 1e-10) ? 1 : 0;
                }

                Assert.Equal(i + 1, count);
            }

            for (int i = 0; i < Acols.Count; ++i)
            {
                IVector pi = new SparseVector(Acols[i].Dimension);
                for (int j = 0; j < vstarlist.Count; ++j)
                {
                    pi = pi.Add(vstarlist[j].Multiply(Rcols[i][j]));
                }
                
                for (int d = 0; d < Acols[i].Dimension; ++d)
                {
                    Assert.Equal(Acols[i][d], pi[d]);
                }
                Assert.True(Acols[i].Equals(pi));
            }

            List<double> norms;
            List<IVector> qlist = VectorUtils.Normalize(vstarlist, out norms);

            for (int i = 0; i < qlist.Count - 1; ++i)
            {
                for (int j = i + 1; j < vstarlist.Count; ++j)
                {
                    Assert.True(qlist[i].Multiply(qlist[j]) < 1e-10);
                }
                Assert.True(qlist[i].Norm(2) - 1 < 1e-10);
            }

            for (int i = 0; i < vstarlist.Count; ++i)
            {
                Assert.Equal(norms[i], vstarlist[i].Norm(2));
            }

            IMatrix R = MatrixUtils.GetMatrix(Rcols);
            foreach (int r in R.RowKeys)
            {
                R[r] = R[r].Multiply(norms[r]);
            }

            for (int i = 0; i < A.RowCount; ++i)
            {
                for (int j = 0; j < A.ColCount; ++j)
                {
                    Assert.Equal(R[i][j], Rcols[j][i] * norms[i]);
                }
            }
        }

        [Fact]
        public void TestNorm()
        {
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);
            double norm = System.Math.Sqrt(Acols[0].Multiply(Acols[0]));
            Assert.Equal(norm, Acols[0].Norm(2));
        }

        [Fact]
        public void TestMinus()
        {
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);
            IVector pi = Acols[0].Minus(Acols[1]);

            for (int i = 0; i < pi.Dimension; ++i)
            {
                Assert.Equal(Data[i][0] - Data[i][1], pi[i]);
            }
        }

        [Fact]
        public void TestZero()
        {
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);
            for (int i = 0; i < Acols.Count; ++i)
            {
                Assert.False(Acols[i].IsEmpty);
            }
        }
    }
}
