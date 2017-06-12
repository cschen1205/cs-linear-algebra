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
        public static IMatrix<Key, Val> GetEchelonForm<Key, Val>(IMatrix<Key, Val> A, out int numRowExOperations)
        {
            IMatrix<Key, Val> B = A.Clone();
            int rowCount = B.RowCount;
            int colCount = B.ColCount;

            numRowExOperations = 0;

            List<Key> newRows = new List<Key>();
            HashSet<Key> remainingRows = new HashSet<Key>();

            List<Key> oldRows = B.RowKeys.ToList();

            foreach(Key k in oldRows)
            {
                remainingRows.Add(k);
            }


            foreach(Key c in B.ColKeys)
            {
                List<Key> nonZeroRows = GetRowsWithNonZeroColumnEntry(B, remainingRows, c);
                if (nonZeroRows.Count > 0)
                {
                    Key pivot = GetPivot(B, nonZeroRows, c);

                    newRows.Add(pivot);
                    remainingRows.Remove(pivot);

                    foreach (Key r in nonZeroRows)
                    {
                        double multiplier = (dynamic)B[r][c] / (dynamic)B[pivot][c];

                        B[r] = B[r].Minus(B[pivot].Multiply(multiplier));
                    }
                }
            }

            foreach (Key r in remainingRows)
            {
                newRows.Add(r);
            }

            for (int i = 0; i < newRows.Count; ++i)
            {
                Key newRow = newRows[i];
                Key oldRow = oldRows[i];

                if(!newRow.Equals(oldRow))
                {
                    IVector<Key, Val> temp = B[newRow];
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
        public static IMatrix<Key, Val> GetEchelonForm<Key, Val>(IMatrix<Key, Val> A, out IMatrix<Key, Val> M, out int numRowExOperations)
        {
            IMatrix<Key, Val> B = A.Clone();
            int rowCount = B.RowCount;
            int colCount = B.ColCount;

            M = A.Identity(rowCount);
            
            numRowExOperations = 0;

            List<Key> newRows = new List<Key>();
            HashSet<Key> remainingRows = new HashSet<Key>();

            List<Key> oldRows = B.RowKeys.ToList();

            foreach (Key k in oldRows)
            {
                remainingRows.Add(k);
            }


            foreach (Key c in B.ColKeys)
            {
                List<Key> nonZeroRows = GetRowsWithNonZeroColumnEntry(B, remainingRows, c);
                if (nonZeroRows.Count > 0)
                {
                    Key pivot = GetPivot(B, nonZeroRows, c);

                    newRows.Add(pivot);
                    remainingRows.Remove(pivot);

                    foreach (Key r in nonZeroRows)
                    {
                        double multiplier = (dynamic)B[r][c] / (dynamic)B[pivot][c];

                        B[r] = B[r].Minus(B[pivot].Multiply(multiplier));
                        M[r] = M[r].Minus(M[pivot].Multiply(multiplier));
                    }
                }
            }

            foreach (Key r in remainingRows)
            {
                newRows.Add(r);
            }

            for (int i = 0; i < newRows.Count; ++i)
            {
                Key newRow = newRows[i];
                Key oldRow = oldRows[i];

                if (!newRow.Equals(oldRow))
                {
                    IVector<Key, Val> temp = B[newRow];
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

        private static void Swap<Key>(List<Key> v, int i, int j)
        {
            Key temp = v[i];
            v[i] = v[j];
            v[j] = temp;
        }

        private static List<Key> GetRowsWithNonZeroColumnEntry<Key, Val>(IMatrix<Key, Val> B, HashSet<Key> remainingRows, Key c)
        {
            List<Key> result = new List<Key>();
            foreach (Key r in remainingRows)
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
        private static Key GetPivot<Key, Val>(IMatrix<Key, Val> B, List<Key> nonZeroRows, Key c)
        {
            double maxVal = double.MinValue;
            Key pivot = default(Key);
            foreach (Key r in nonZeroRows)
            {
                double val = (dynamic)B[r, c];
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
