using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public interface IMatrix<Key, Val>
    {
        int RowCount
        {
            get;
        }

        int ColCount
        {
            get;
        }

        IMatrix<Key, Val> Multiply(IMatrix<Key, Val> rhs);
        IVector<Key, Val> Multiply(IVector<Key, Val> rhs);
        IMatrix<Key, Val> Multiply(double scalar);

        IMatrix<Key, Val> Add(IMatrix<Key, Val> rhs);
        IMatrix<Key, Val> Minus(IMatrix<Key, Val> rhs);

        IVector<Key, Val> this[Key rowKey]
        {
            get;
            set;
        }

        IVector<Key, Val> GetColumn(Key colKey);

        Val this[Key row, Key col]
        {
            get;
            set;
        }

        bool HasValue(Key row, Key col);

        IMatrix<Key, Val> Clone();
        void Copy(IMatrix<Key, Val> rhs);
       
        Key[] RowKeys { get; }
        Key[] ColKeys { get; }

        IMatrix<Key, Val> Zero(int rowCount, int colCount, Val default_val);
        IMatrix<Key, Val> Zero(Key[] rows, Key[] cols, Val default_val);

        IMatrix<Key, Val> Identity(int dimension);
        IMatrix<Key, Val> Identity(Key[] attr);

        IMatrix<Key, Val> Transpose();

        IEnumerable<IVector<Key, Val>> NonEmptyRows { get; }
        
        Val DefaultValue { get; }

        IMatrix<Key, Val> SubMatrix(Key[] rows, Key[] cols);

        bool IsSymmetric { get; }

        /// <summary>
        /// Scalar Multiply each row by the corresponding row entry in the column vector W
        /// </summary>
        /// <param name="W"></param>
        /// <returns></returns>
        IMatrix<Key, Val> ScalarMultiply(IVector<Key, Val> W);
        IMatrix<Key, Val> ScalarMultiply(double[] W);
    }
}
