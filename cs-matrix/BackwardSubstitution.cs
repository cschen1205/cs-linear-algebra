using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class BackwardSubstitution
    {
        /// <summary>
        /// Solve x such that R * x = c
        /// </summary>
        /// <param name="R">a upper triangular matrix</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IVector Solve(IMatrix R, IVector c)
        {
            int n = R.RowCount;
            IVector x = c.Zero(n, c.DefaultValue);
            for (int r = n - 1; r >= 0; --r)
            {
                if (R[r][r] != 0)
                {
                    x[r] = (c[r] - R[r].Multiply(x)) / R[r][r];
                }
            }
            return x;
        }
    }
}
