using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public static class GaussianElimination
    {
        /// The method works by using Gaussian elimination to covert the matrix A to the echelon form of A
        /// The computational Complexity is O(n^3)
        /// </summary>
        /// <param name="A">The original matrix</param>
        /// <param name="numRowExOperations">The number of elementary row exchange operations during the Gaussian elimination</param>
        /// <returns>The echelon form of the original matrix</returns>
        public static IMatrix GetEchelonForm(IMatrix A, out int numRowExOperations)
        {
            IMatrix B = A.Clone();
            int rowCount = B.RowCount;
            int colCount = B.ColCount;

            numRowExOperations = 0;

            List<int> newRows = new List<int>();
            HashSet<int> remainingRows = new HashSet<int>();

            List<int> oldRows = B.RowKeys.ToList();

            foreach(int k in oldRows)
            {
                remainingRows.Add(k);
            }


            foreach(int c in B.ColKeys)
            {
                List<int> nonZeroRows = GetRowsWithNonZeroColumnEntry(B, remainingRows, c);
                if (nonZeroRows.Count > 0)
                {
                    int pivot = GetPivot(B, nonZeroRows, c);

                    newRows.Add(pivot);
                    remainingRows.Remove(pivot);

                    foreach (int r in nonZeroRows)
                    {
                        double multiplier = B[r][c] / B[pivot][c];

                        B[r] = B[r].Minus(B[pivot].Multiply(multiplier));
                    }
                }
            }

            foreach (int r in remainingRows)
            {
                newRows.Add(r);
            }

            for (int i = 0; i < newRows.Count; ++i)
            {
                int newRow = newRows[i];
                int oldRow = oldRows[i];

                if(!newRow.Equals(oldRow))
                {
                    IVector temp = B[newRow];
                    B[newRow] = B[oldRow];
                    B[oldRow] = temp;

                    int newRowIndex = i;
                    int oldRowIndex = newRows.IndexOf(oldRow);

                    Swap(newRows, newRowIndex, oldRowIndex);

                    numRowExOperations++;
                }
            }

            return B;
        }

        /// The method works by using Gaussian elimination to covert the matrix A to the echelon form of A
        /// The computational Complexity is O(n^3)
        /// </summary>
        /// <param name="A">The original matrix</param>
        /// <param name="M">An invertiable matrix, which originally starts as an identity matrix and arrived by undergoing the same elementary operations as A</param>
        /// <param name="numRowExOperations">The number of elementary row exchange operations during the Gaussian elimination</param>
        /// <returns>The echelon form of the original matrix, which is M*A</returns>
        public static IMatrix GetEchelonForm(IMatrix A, out IMatrix M, out int numRowExOperations)
        {
            IMatrix B = A.Clone();
            int rowCount = B.RowCount;
            int colCount = B.ColCount;

            M = A.Identity(rowCount);
            
            numRowExOperations = 0;

            List<int> newRows = new List<int>();
            HashSet<int> remainingRows = new HashSet<int>();

            List<int> oldRows = B.RowKeys.ToList();

            foreach (int k in oldRows)
            {
                remainingRows.Add(k);
            }


            foreach (int c in B.ColKeys)
            {
                List<int> nonZeroRows = GetRowsWithNonZeroColumnEntry(B, remainingRows, c);
                if (nonZeroRows.Count > 0)
                {
                    int pivot = GetPivot(B, nonZeroRows, c);

                    newRows.Add(pivot);
                    remainingRows.Remove(pivot);

                    foreach (int r in nonZeroRows)
                    {
                        double multiplier = B[r][c] / B[pivot][c];

                        B[r] = B[r].Minus(B[pivot].Multiply(multiplier));
                        M[r] = M[r].Minus(M[pivot].Multiply(multiplier));
                    }
                }
            }

            foreach (int r in remainingRows)
            {
                newRows.Add(r);
            }

            for (int i = 0; i < newRows.Count; ++i)
            {
                int newRow = newRows[i];
                int oldRow = oldRows[i];

                if (!newRow.Equals(oldRow))
                {
                    IVector temp = B[newRow];
                    B[newRow] = B[oldRow];
                    B[oldRow] = temp;

                    temp = M[newRow];
                    M[newRow] = M[oldRow];
                    M[oldRow] = temp;

                    int newRowIndex = i;
                    int oldRowIndex = newRows.IndexOf(oldRow);

                    Swap(newRows, newRowIndex, oldRowIndex);

                    numRowExOperations++;
                }
            }

            return B;
        }

        private static void Swap(List<int> v, int i, int j)
        {
            int temp = v[i];
            v[i] = v[j];
            v[j] = temp;
        }

        private static List<int> GetRowsWithNonZeroColumnEntry(IMatrix B, HashSet<int> remainingRows, int c)
        {
            List<int> result = new List<int>();
            foreach (int r in remainingRows)
            {
                if (B.HasValue(r, c))
                {
                    result.Add(r);
                }
            }

            return result;
        }

        /// <summary>
        /// Partial pivoting
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Val"></typeparam>
        /// <param name="B"></param>
        /// <param name="nonZeroRows"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int GetPivot(IMatrix B, List<int> nonZeroRows, int c)
        {
            double maxVal = double.MinValue;
            int pivot = 0;
            foreach (int r in nonZeroRows)
            {
                double val = B[r, c];
                if (val > maxVal)
                {
                    maxVal = val;
                    pivot = r;
                }
            }

            return pivot;
        }
    }
}
