using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace cs_matrix
{
    public class SparseMatrix<Key, Val> : IMatrix<Key, Val>
    {
        private Val mDefaultVal;

        private Dictionary<Key, SparseVector<Key, Val>> mInternal = new Dictionary<Key, SparseVector<Key, Val>>();

        private Key[] mColKeys;
        private Key[] mRowKeys;

        public SparseMatrix(int rowCount, int colCount)
        {
            mRowKeys = new Key[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = (dynamic)r;
            }

            mColKeys = new Key[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = (dynamic)c;
            }

            mDefaultVal = (dynamic)0;
        }

        public SparseMatrix(int rowCount, int colCount, Val default_val)
        {
            mRowKeys = new Key[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = (dynamic)r;
            }

            mColKeys = new Key[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = (dynamic)c;
            }

            mDefaultVal = default_val;
        }

        public SparseMatrix(Val[][] matrix)
        {
            int rowCount = matrix.Length;
            int colCount = matrix[0].Length;

            mRowKeys = new Key[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = (dynamic)r;
            }

            mColKeys = new Key[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = (dynamic)c;
            }

            mDefaultVal = (dynamic)0;

            for (int r = 0; r < rowCount; ++r)
            {
                for (int c = 0; c < colCount; ++c)
                {
                    this[(dynamic)r, (dynamic)c] = matrix[r][c];
                }
            }
        }

        public SparseMatrix(Val[,] matrix)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            mRowKeys = new Key[rowCount];
            for (int r = 0; r < rowCount; ++r)
            {
                mRowKeys[r] = (dynamic)r;
            }

            mColKeys = new Key[colCount];
            for (int c = 0; c < colCount; ++c)
            {
                mColKeys[c] = (dynamic)c;
            }

            mDefaultVal = (dynamic)0;

            for (int r = 0; r < rowCount; ++r)
            {
                for (int c = 0; c < colCount; ++c)
                {
                    this[(dynamic)r, (dynamic)c] = matrix[r, c];
                }
            }
        }

        public SparseMatrix(Key[] rows, Key[] cols, Val default_val)
        {
            mRowKeys = (Key[])rows.Clone();
            mColKeys = (Key[])cols.Clone();

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

        public virtual IMatrix<Key, Val> Multiply(IMatrix<Key, Val> rhs)
        {
            Debug.Assert(this.ColCount == rhs.RowCount);

            SparseVector<Key, Val> row1;
            IVector<Key, Val> col2;

            IMatrix<Key, Val> result = this.Zero(mRowKeys, rhs.ColKeys, mDefaultVal);

            foreach (KeyValuePair<Key, SparseVector<Key, Val>> entry in mInternal)
            {
                row1 = entry.Value;
                foreach (Key c2 in rhs.ColKeys)
                {
                    col2 = rhs.GetColumn(c2);
                    result[entry.Key, c2] = (dynamic)row1.Multiply(col2);
                }
            }

            return result;
        }

        public IVector<Key, Val> Multiply(IVector<Key, Val> rhs)
        {
            Debug.Assert(this.ColCount == rhs.Dimension);

            SparseVector<Key, Val> row1;
            IVector<Key, Val> result = rhs.Zero(mRowKeys, mDefaultVal);
            foreach (KeyValuePair<Key, SparseVector<Key, Val>> entry in mInternal)
            {
                row1 = entry.Value;
                result[entry.Key] = (dynamic)row1.Multiply(rhs);
            }
            return result;
        }

        public Val this[Key row, Key col]
        {
            get
            {
                SparseVector<Key, Val> rowVector;
                if (mInternal.TryGetValue(row, out rowVector))
                {
                    return rowVector[col];
                }
                return mDefaultVal;
            }
            set
            {
                SparseVector<Key, Val> rowVector;

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
                    rowVector = new SparseVector<Key, Val>(mColKeys, mDefaultVal);
                    rowVector.ID = row;
                    mInternal[row] = rowVector;
                }

                rowVector[col] = value;
            }
        }

        public IMatrix<Key, Val> Clone()
        {
            SparseMatrix<Key, Val> clone = new SparseMatrix<Key, Val>(mRowKeys, mColKeys, mDefaultVal);
            clone.Copy(this);
            return clone;
        }

        public void Copy(IMatrix<Key, Val> rhs)
        {
            mInternal.Clear();

            SparseMatrix<Key, Val> rhs2 = rhs as SparseMatrix<Key, Val>;
            foreach (KeyValuePair<Key, SparseVector<Key, Val>> entry in rhs2.mInternal)
            {
                mInternal[entry.Key] = entry.Value.Clone() as SparseVector<Key, Val>;
            }
        }

        public virtual IMatrix<Key, Val> Zero(int rowCount, int colCount, Val default_val)
        {
            return new SparseMatrix<Key, Val>(rowCount, colCount, default_val);
        }

        public virtual IMatrix<Key, Val> Zero(Key[] rows, Key[] cols, Val default_val)
        {
            return new SparseMatrix<Key, Val>(rows, cols, default_val);
        }

        public IVector<Key, Val> this[Key row]
        {
            get
            {
                SparseVector<Key, Val> rowData;
                if (mInternal.TryGetValue(row, out rowData))
                {
                    return rowData;
                }
                return new SparseVector<Key, Val>(mColKeys, mDefaultVal);
            }
            set
            {
                value.ID = row;
                mInternal[row] = value as SparseVector<Key, Val>;
            }
        }


        public Key[] RowKeys
        {
            get
            {
                return mRowKeys; 
            }
        }

        public Key[] ColKeys
        {
            get { return mColKeys; }
        }


        public bool HasValue(Key row, Key col)
        {
            SparseVector<Key, Val> rowVector;
            if (!mInternal.TryGetValue(row, out rowVector))
            {
                return false;
            }
            return rowVector.HasValue(col);
        }


        public IMatrix<Key, Val> Multiply(double scalar)
        {
            SparseMatrix<Key, Val> result = Zero(mRowKeys, mColKeys, mDefaultVal) as SparseMatrix<Key, Val>;
            foreach(KeyValuePair<Key, SparseVector<Key, Val>> k in mInternal)
            {
                result[k.Key] = k.Value.Multiply(scalar);
            }
            return result;
        }

        public IMatrix<Key, Val> Add(IMatrix<Key, Val> rhs)
        {
            Debug.Assert(RowCount == rhs.RowCount);
            Debug.Assert(ColCount == rhs.ColCount);

            HashSet<Key> allRows = new HashSet<Key>();
            foreach (Key k in mInternal.Keys)
            {
                allRows.Add(k);
            }
            foreach (Key k in rhs.RowKeys)
            {
                allRows.Add(k);
            }

            IMatrix<Key, Val> result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (Key k in allRows)
            {
                IVector<Key, Val> row1 = this[k];
                IVector<Key, Val> row2 = rhs[k];
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

        public IMatrix<Key, Val> Minus(IMatrix<Key, Val> rhs)
        {
            Debug.Assert(RowCount == rhs.RowCount);
            Debug.Assert(ColCount == rhs.ColCount);

            HashSet<Key> allRows = new HashSet<Key>();
            foreach (Key k in mInternal.Keys)
            {
                allRows.Add(k);
            }
            foreach (Key k in rhs.RowKeys)
            {
                allRows.Add(k);
            }

            IMatrix<Key, Val> result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (Key k in allRows)
            {
                IVector<Key, Val> row1 = this[k];
                IVector<Key, Val> row2 = rhs[k];
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


        public IMatrix<Key, Val> Identity(int dimension)
        {
            IMatrix<Key, Val> I = this.Zero(dimension, dimension, mDefaultVal);
            Val one = (dynamic)1;

            for (int i = 0; i < dimension; ++i)
            {
                Key k = (dynamic)i;
                I[k, k] = one;
            }

            return I;
        }

        public IMatrix<Key, Val> Identity(Key[] keys)
        {
            IMatrix<Key, Val> I = this.Zero(keys, keys, mDefaultVal);
            Val one = (dynamic)1;

            for (int i = 0; i < keys.Length; ++i)
            {
                Key k = keys[i];
                I[k, k] = one;
            }

            return I;
        }


        public IMatrix<Key, Val> Transpose()
        {
            IMatrix<Key, Val> clone = Zero(mColKeys, mRowKeys, mDefaultVal);
            foreach (Key col in mColKeys)
            {
                foreach (Key row in mRowKeys)
                {
                    clone[col, row] = this[row, col];
                }
            }

            return clone;
        }


        public IEnumerable<IVector<Key, Val>> NonEmptyRows
        {
            get
            {
                return mInternal.Values;
            }
        }

        public Val DefaultValue
        {
            get { return mDefaultVal; }
        }

        public override bool Equals(object obj)
        {
            SparseMatrix<Key, Val> rhs = obj as SparseMatrix<Key, Val>;
            if (RowCount != rhs.RowCount || ColCount != rhs.ColCount)
            {
                return false;
            }

            foreach (Key row in mRowKeys)
            {
                foreach (Key col in mColKeys)
                {
                    if (System.Math.Abs((dynamic)this[row, col] - (dynamic)rhs[row, col]) > 1e-10)
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
            foreach (Key row in mInternal.Keys)
            {
                SparseVector<Key, Val> rowVec = mInternal[row];
                foreach (Key col in rowVec.Keys)
                {
                    hash = hash * 31 + rowVec[col].GetHashCode();
                }
            }

            return hash;
        }


        public IVector<Key, Val> GetColumn(Key colKey)
        {
            SparseVector<Key, Val> colVec = new SparseVector<Key, Val>(mRowKeys, mDefaultVal);
            foreach (Key k in mRowKeys)
            {
                colVec[k] = this[k, colKey];
            }
            return colVec;
        }


        public IMatrix<Key, Val> SubMatrix(Key[] rows, Key[] cols)
        {
            IMatrix<Key, Val> sm = new SparseMatrix<Key, Val>(rows, cols, mDefaultVal);
            foreach (Key row in rows)
            {
                foreach (Key col in cols)
                {
                    sm[row, col] = this[row, col];
                }
            }

            return sm;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Key row in mRowKeys)
            {
                sb.Append("{");
                bool first = true;
                foreach (Key col in mColKeys)
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

                foreach (SparseVector<Key, Val> rowVec in NonEmptyRows)
                {
                    Key row = rowVec.ID;
                    foreach (Key col in rowVec.NonEmptyKeys)
                    {
                        if (row.Equals(col)) continue;
                        double diff = System.Math.Abs((dynamic)rowVec[col] - (dynamic)this[col, row]);
                        
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




        public IMatrix<Key, Val> ScalarMultiply(IVector<Key, Val> W)
        {
            IMatrix<Key, Val> result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (Key row in mInternal.Keys)
            {
                double scalar = (dynamic)W[row];
                result[row] = mInternal[row].Multiply(scalar);
            }
            return result;
        }

        public IMatrix<Key, Val> ScalarMultiply(double[] W)
        {
            IMatrix<Key, Val> result = this.Zero(mRowKeys, mColKeys, mDefaultVal);
            foreach (Key row in mInternal.Keys)
            {
                double scalar = W[(dynamic)row];
                result[row] = mInternal[row].Multiply(scalar);
            }
            return result;
        }
    }
}
