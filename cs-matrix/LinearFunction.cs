using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    /// <summary>
    /// Let V be a vector space 
    /// 
    /// A linear function f: V -> U, is y = f(x) \in U, \forall x \in V, such that the vector addition and scalar multiplication are preserved,
    /// In other words, a linear function f: V -> U has the following properties:
    ///   f(a + b) = f(a) + f(b) \forall a, b \in V
    ///   f(c * a) = c * f(a) \forall a \in V, c is a scalar value
    ///   
    /// V is known as the domain of f
    /// U is known as the co-domain of f
    ///   
    /// The kernel of f(x) is {x | f(x) = 0, x \in V }, represented as Ker f(x)
    /// Ker f(x) is a vector space as it contains { 0 }
    /// if Ker f(x) = { 0 }, then f(x) is one-to-one (i.e. f(x1) = f(x2) if and only if x1=x2) and onto (i.e. for each y \in U, we can find x \in U, such that f(x) = y)
    /// 
    /// 
    /// f(x) = A * x, is a linear function, where A is a matrix
    /// If A is invertiable, then f(x) is one-to-one (i.e. f(x1) = f(x2) if and only if x1=x2) and onto (i.e. for each y \in U, we can find x \in U, such that f(x) = y)
    /// </summary>
    public class LinearFunction
    {
        protected RowSpace mRowSpace = null;

        public LinearFunction(IMatrix A)
        {
            mRowSpace = new RowSpace(A);
        }

        /// <summary>
        /// The basis of the kernel of f(x)
        /// The kernel of f(x) is {x | f(x) = 0, x \in V }, represented as Ker f(x)
        /// </summary>
        public List<IVector> KernelBasis
        {
            get
            {
                return mRowSpace.NullBasis;
            }
        }

        /// <summary>
        /// The dimension of the kernel of f(x)
        /// </summary>
        public int KernelDim
        {
            get
            {
                return mRowSpace.Nullity;
            }
        }
    }
}
