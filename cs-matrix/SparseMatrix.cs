using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class SparseMatrix : IMatrix
    {
        private double mDefaultVal;

        private Dictionary<int, SparseVector> mInternal = new Dictionary<int, SparseVector>();

        private int[] mColKeys;
        private int[] mRowKeys;

        public SparseMatrix(int rowCount, int colCount)
        {
            mRowKeys = new int[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = r;
            }

            mColKeys = new int[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = c;
            }

            mDefaultVal = 0;
        }

        public SparseMatrix(int rowCount, int colCount, double default_val)
        {
            mRowKeys = new int[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = r;
            }

            mColKeys = new int[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = c;
            }

            mDefaultVal = default_val;
        }

        public SparseMatrix(double[][] matrix)
        {
            int rowCount = matrix.Length;
            int colCount = matrix[0].Length;

            mRowKeys = new int[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = r;
            }

            mColKeys = new int[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = c;
            }

            mDefaultVal = 0;

            for (int r = 0; r < rowCount; ++r)
            {
                for (int c = 0; c < colCount; ++c)
                {
                    this[r, c] = matrix[r][c];
                }
            }
        }

        public SparseMatrix(double[,] matrix)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            mRowKeys = new int[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = r;
            }

            mColKeys = new int[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = c;
            }

            mDefaultVal = 0;

            for (int r = 0; r < rowCount; ++r)
            {
                for (int c = 0; c < colCount; ++c)
                {
                    this[r, c] = matrix[r, c];
                }
            }
        }

        public SparseMatrix(int[] rows, int[] cols, double default_val)
        {
            mRowKeys = (int[])rows.Clone();
            mColKeys = (int[])cols.Clone();

            mDefaultVal = default_val;
        }


        public int RowCount
        {
            get { return mRowKeys.Length; }
        }

        public int ColCount
        {
            get { return mColKeys.Length; }
        }

        public virtual IMatrix Multiply(IMatrix rhs)
        {
            Debug.Assert(this.ColCount == rhs.RowCount);

            SparseVector row1;
            IVector col2;

            IMatrix result = this.Zero(mRowKeys, rhs.ColKeys, mDefaultVal);

            foreach (KeyValuePair<int, SparseVector> entry in mInternal)
            {
                row1 = entry.Value;
                foreach (int c2 in rhs.ColKeys)
                {
                    col2 = rhs.GetColumn(c2);
                    result[entry.Key, c2] = row1.Multiply(col2);
                }
            }

            return result;
        }

        public IVector Multiply(IVector rhs)
        {
            Debug.Assert(this.ColCount == rhs.Dimension);

            SparseVector row1;
            IVector result = rhs.Zero(mRowKeys, mDefaultVal);
            foreach (KeyValuePair<int, SparseVector> entry in mInternal)
            {
                row1 = entry.Value;
                result[entry.Key] = row1.Multiply(rhs);
            }
            return result;
        }

        public double this[int row, int col]
        {
            get
            {
                SparseVector rowVector;
                if (mInternal.TryGetValue(row, out rowVector))
                {
                    return rowVector[col];
                }
                return mDefaultVal;
            }
            set
            {
                SparseVector rowVector;

#if SK_LINALG_CHECK_ARRAY_BOUNDARY
                if (!mColKeys.Contains(col))
                {
                    throw new IndexOutOfRangeException();
                }

                if (!mRowKeys.Contains(row))
                {
                    throw new IndexOutOfRangeException();
                }
#endif


                if (!mInternal.TryGetValue(row, out rowVector))
                {
                    rowVector = new SparseVector(mColKeys, mDefaultVal);
                    rowVector.ID = row;
                    mInternal[row] = rowVector;
                }

                rowVector[col] = value;
            }
        }

        public IMatrix Clone()
        {
            SparseMatrix clone = new SparseMatrix(mRowKeys, mColKeys, mDefaultVal);
            clone.Copy(this);
            return clone;
        }

        public void Copy(IMatrix rhs)
        {
            mInternal.Clear();

            SparseMatrix rhs2 = rhs as SparseMatrix;
            foreach (KeyValuePair<int, SparseVector> entry in rhs2.mInternal)
            {
                mInternal[entry.Key] = entry.Value.Clone() as SparseVector;
            }
        }

        public virtual IMatrix Zero(int rowCount, int colCount, double default_val)
        {
            return new SparseMatrix(rowCount, colCount, default_val);
        }

        public virtual IMatrix Zero(int[] rows, int[] cols, double default_val)
        {
            return new SparseMatrix(rows, cols, default_val);
        }

        public IVector this[int row]
        {
            get
            {
                SparseVector rowData;
                if (mInternal.TryGetValue(row, out rowData))
                {
                    return rowData;
                }
                return new SparseVector(mColKeys, mDefaultVal);
            }
            set
            {
                value.ID = row;
                mInternal[row] = value as SparseVector;
            }
        }


        public int[] RowKeys
        {
            get
            {
                return mRowKeys; 
            }
        }

        public int[] ColKeys
        {
            get { return mColKeys; }
        }


        public bool HasValue(int row, int col)
        {
            SparseVector rowVector;
            if (!mInternal.TryGetValue(row, out rowVector))
            {
                return false;
            }
            return rowVector.HasValue(col);
        }


        public IMatrix Multiply(double scalar)
        {
            SparseMatrix result = Zero(mRowKeys, mColKeys, mDefaultVal) as SparseMatrix;
            foreach(KeyValuePair<int, SparseVector> k in mInternal)
            {
                result[k.Key] = k.Value.Multiply(scalar);
            }
            return result;
        }

        public IMatrix Add(IMatrix rhs)
        {
            Debug.Assert(RowCount == rhs.RowCount);
            Debug.Assert(ColCount == rhs.ColCount);

            HashSet<int> allRows = new HashSet<int>();
            foreach (int k in mInternal.Keys)
            {
                allRows.Add(k);
            }
            foreach (int k in rhs.RowKeys)
            {
                allRows.Add(k);
            }

            IMatrix result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (int k in allRows)
            {
                IVector row1 = this[k];
                IVector row2 = rhs[k];
                if (row1 == null)
                {
                    result[k] = row2.Clone();
                }
                else if (row2 == null)
                {
                    result[k] = row1.Clone();
                }
                else
                {
                    result[k] = row1.Add(row2);
                }
            }

            return result;
        }

        public IMatrix Minus(IMatrix rhs)
        {
            Debug.Assert(RowCount == rhs.RowCount);
            Debug.Assert(ColCount == rhs.ColCount);

            HashSet<int> allRows = new HashSet<int>();
            foreach (int k in mInternal.Keys)
            {
                allRows.Add(k);
            }
            foreach (int k in rhs.RowKeys)
            {
                allRows.Add(k);
            }

            IMatrix result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (int k in allRows)
            {
                IVector row1 = this[k];
                IVector row2 = rhs[k];
                if (row1 == null)
                {
                    result[k] = row2.Multiply(-1);
                }
                else if (row2 == null)
                {
                    result[k] = row1.Clone();
                }
                else
                {
                    result[k] = row1.Minus(row2);
                }
            }

            return result;
        }


        public IMatrix Identity(int dimension)
        {
            IMatrix I = this.Zero(dimension, dimension, mDefaultVal);
            double one = 1;

            for (int i = 0; i < dimension; ++i)
            {
                int k = i;
                I[k, k] = one;
            }

            return I;
        }

        public IMatrix Identity(int[] keys)
        {
            IMatrix I = this.Zero(keys, keys, mDefaultVal);
            double one = 1;

            for (int i = 0; i < keys.Length; ++i)
            {
                int k = keys[i];
                I[k, k] = one;
            }

            return I;
        }


        public IMatrix Transpose()
        {
            IMatrix clone = Zero(mColKeys, mRowKeys, mDefaultVal);
            foreach (int col in mColKeys)
            {
                foreach (int row in mRowKeys)
                {
                    clone[col, row] = this[row, col];
                }
            }

            return clone;
        }


        public IEnumerable<IVector> NonEmptyRows
        {
            get
            {
                return mInternal.Values;
            }
        }

        public double DefaultValue
        {
            get { return mDefaultVal; }
        }

        public override bool Equals(object obj)
        {
            SparseMatrix rhs = obj as SparseMatrix;
            if (RowCount != rhs.RowCount || ColCount != rhs.ColCount)
            {
                return false;
            }

            foreach (int row in mRowKeys)
            {
                foreach (int col in mColKeys)
                {
                    if (System.Math.Abs(this[row, col] - rhs[row, col]) > 1e-10)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 31;
            foreach (int row in mInternal.Keys)
            {
                SparseVector rowVec = mInternal[row];
                foreach (int col in rowVec.Keys)
                {
                    hash = hash * 31 + rowVec[col].GetHashCode();
                }
            }

            return hash;
        }


        public IVector GetColumn(int colKey)
        {
            SparseVector colVec = new SparseVector(mRowKeys, mDefaultVal);
            foreach (int k in mRowKeys)
            {
                colVec[k] = this[k, colKey];
            }
            return colVec;
        }


        public IMatrix SubMatrix(int[] rows, int[] cols)
        {
            IMatrix sm = new SparseMatrix(rows, cols, mDefaultVal);
            foreach (int row in rows)
            {
                foreach (int col in cols)
                {
                    sm[row, col] = this[row, col];
                }
            }

            return sm;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int row in mRowKeys)
            {
                sb.Append("{");
                bool first = true;
                foreach (int col in mColKeys)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat("{0}", this[row, col]);
                }
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        public bool IsSymmetric
        {
            get
            {
                if (RowCount != ColCount) return false;

                foreach (SparseVector rowVec in NonEmptyRows)
                {
                    int row = rowVec.ID;
                    foreach (int col in rowVec.NonEmptyKeys)
                    {
                        if (row.Equals(col)) continue;
                        double diff = System.Math.Abs(rowVec[col] - this[col, row]);
                        
                        bool equals = diff <= 1e-10;
                        
                        if (!equals)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }




        public IMatrix ScalarMultiply(IVector W)
        {
            IMatrix result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (int row in mInternal.Keys)
            {
                double scalar = W[row];
                result[row] = mInternal[row].Multiply(scalar);
            }
            return result;
        }

        public IMatrix ScalarMultiply(double[] W)
        {
            IMatrix result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (int row in mInternal.Keys)
            {
                double scalar = W[row];
                result[row] = mInternal[row].Multiply(scalar);
            }
            return result;
        }
    }
}
