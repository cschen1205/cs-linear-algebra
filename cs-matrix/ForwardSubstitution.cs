using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class ForwardSubstitution
    {
        /// <summary>
        /// Solve x such that R * x = c
        /// </summary>
        /// <param name="R">a lower triangular matrix</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IVector Solve(IMatrix R, IVector c)
        {
            int n = R.RowCount;
            IVector x = c.Zero(n, c.DefaultValue);
            for (int r = 0; r < n; ++r)
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
