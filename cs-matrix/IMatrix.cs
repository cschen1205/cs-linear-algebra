using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public interface IMatrix
    {
        int RowCount
        {
            get;
        }

        int ColCount
        {
            get;
        }

        IMatrix Multiply(IMatrix rhs);
        IVector Multiply(IVector rhs);
        IMatrix Multiply(double scalar);

        IMatrix Add(IMatrix rhs);
        IMatrix Minus(IMatrix rhs);

        IVector this[int rowKey]
        {
            get;
            set;
        }

        IVector GetColumn(int colKey);

        double this[int row, int col]
        {
            get;
            set;
        }

        bool HasValue(int row, int col);

        IMatrix Clone();
        void Copy(IMatrix rhs);
       
        int[] RowKeys { get; }
        int[] ColKeys { get; }

        IMatrix Zero(int rowCount, int colCount, double default_val);
        IMatrix Zero(int[] rows, int[] cols, double default_val);

        IMatrix Identity(int dimension);
        IMatrix Identity(int[] attr);

        IMatrix Transpose();

        IEnumerable<IVector> NonEmptyRows { get; }
        
        double DefaultValue { get; }

        IMatrix SubMatrix(int[] rows, int[] cols);

        bool IsSymmetric { get; }

        /// <summary>
        /// Scalar Multiply each row by the corresponding row entry in the column vector W
        /// </summary>
        /// <param name="W"></param>
        /// <returns></returns>
        IMatrix ScalarMultiply(IVector W);
        IMatrix ScalarMultiply(double[] W);
    }
}
