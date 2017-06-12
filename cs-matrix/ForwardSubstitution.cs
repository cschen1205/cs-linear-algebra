using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class ForwardSubstitution<Val>
    {
        /// <summary>
        /// Solve x such that R * x = c
        /// </summary>
        /// <param name="R">a lower triangular matrix</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IVector<int, Val> Solve(IMatrix<int, Val> R, IVector<int, Val> c)
        {
            int n = R.RowCount;
            IVector<int, Val> x = c.Zero(n, c.DefaultValue);
            for (int r = 0; r < n; ++r)
            {
                if ((dynamic)R[r][r] != 0)
                {
                    x[r] = (c[r] - (dynamic)R[r].Multiply(x)) / R[r][r];
                }
            }
            return x;
        }
    }
}
