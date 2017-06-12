using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    /// <summary>
    /// A vector space V is a set of vectors which has the following properties:
    /// if v \in V, then \alpha * v \in V, \forall \alpha \in \R 
    /// if v, u \in V, then v + u \in V
    /// { 0 } \belong V
    /// 
    /// The spanning set of V is a set of vectors Span(V) = {u_1, u_2, ..., u_r}, such that
    /// v = \alpha_1 * u_1 + \alpha_2 * u_2 .. + \alpha_r * u_r, \forall v \in V
    /// 
    /// There are two ways to represent vector space V:
    ///   Way 1: V = { x | Ax = 0 }
    ///   Way 2: V = { x | x = \alpha_1 * u_1 + \alpha_2 * u_2 + ... + \alpha_r * u_r }
    /// 
    /// A basis of V is any minimum spanning of V.
    /// A basis has the following properties:
    ///   Vectors in the basis are independent (i.e. \alpha_1 * u_1 + \alpha_2 * u_2 .. + \alpha_r * u_r = 0 if and only if \alpha_1 = \alpha_2 = .. = \alpha_r = 0)
    ///   Any basis has the same number of vectors 
    ///   
    /// The dimension of V is the number of vectors in its basis
    /// </summary>
    public class VectorSpace
    {
        protected List<IVector> mOrthogonalBasis = new List<IVector>();
        protected List<IVector> mOrthoNormalBasis = new List<IVector>();

        /// <summary>
        /// The vector space is represented by V = { x | Ax = 0 }
        /// In this case V is also known the null space of A
        /// </summary>
        /// <param name="A"></param>
        public VectorSpace(IMatrix A)
        {
            mOrthogonalBasis = FindNullSpace(A);
            mOrthoNormalBasis = VectorUtils.Normalize(mOrthogonalBasis);
        }

        /// <summary>
        /// Return the orthogonal basis of the null space of A
        /// </summary>
        /// <param name="A">the matrix</param>
        /// <returns>The orthogonal basis of the null space of A</returns>
        public static List<IVector> FindNullSpace(IMatrix A)
        {
            int n = A.ColCount;
            List<IVector> standardBasis = new List<IVector>();
            double one = 1;
            for (int i = 0; i < n; ++i)
            {
                int col = A.ColKeys[i];
                IVector e_i = new SparseVector(n, A.DefaultValue);
                e_i[col] = one;
                standardBasis.Add(e_i);
            }

            // return the orthogonal basis for V, which is the null space of A, using orthogonal complement
            List<IVector> wlist = Orthogonalization.Orthogonalize(A.NonEmptyRows, standardBasis);

            return VectorUtils.RemoveZeroVectors(wlist);
        }

        /// <summary>
        /// Another way to obtain the orthogonal basis for the null space of A by using the orthogonalization of A's columns
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static List<IVector> FindNullSpace2(IMatrix A)
        {
            List<IVector> R = null;
            int n = A.ColCount;
            int rowCount = A.RowCount;

            // column vectors of A
            List<IVector> Acols = MatrixUtils.GetColumnVectors(A);

            List<IVector> vstarlist = Orthogonalization.Orthogonalize(Acols, out R); 

            // T is a matrix with columns given by vectors in R
            // T is an upper triangle
            // A = (matrix with columns vstarlist) * T
            IMatrix T = MatrixUtils.GetMatrix(R, A.DefaultValue);

            IMatrix T_inverse = T.Transpose(); //T inverse is its transpose

            // let: A = (vstarlist * T)
            // then: A * T.inverse = (vstarlist * T) * T.inverse = vstarlist * (T * T.inverse) = vstarlist * I = vstarlist
            // now: if vstarlist[i] is zero vector then A * T.inverse[i] = 0
            // therefore conclude 1: T.inverse[i] is in null space if vstarlist[i] is zero vector, furthermore {T.inverse[i]} are independent (since T is invertible)
            // 
            // by: nullity(A) + rank(A) = n & count({ non-zero vstarlist[i] }) = rank(A)
            // therefore conclude 2: nullity(A) = n - rank(A) = n - count({ non-zero vstarlist[i] }) = count({ zero vstarlist[i] })
            //
            // by conclusion 1 and 2
            // we have null(A) = Span({ T.inverse[i] | i is such that vstarlist[i] is zero vector})
            List<IVector> result = new List<IVector>();
            List<IVector> Ticols = MatrixUtils.GetColumnVectors(T_inverse);
            for (int c = 0; c < vstarlist.Count; ++c)
            {
                if (vstarlist[c].IsEmpty)
                {
                    result.Add(Ticols[c]);
                }
            }

            return result;
        }

        

        /// <summary>
        /// The vector space is represented by V = { x | x = \alpha_1 * u_1 + \alpha_2 * u_2 + ... + \alpha_r * u_r }
        /// {u_1, u_2, ..., u_r} is the spanning set of V
        /// </summary>
        /// <param name="spanning_set"></param>
        public VectorSpace(IEnumerable<IVector> spanning_set)
        {
            List<IVector> vlist = Orthogonalization.Orthogonalize(spanning_set);
            mOrthogonalBasis = VectorUtils.RemoveZeroVectors(vlist);
            mOrthoNormalBasis = VectorUtils.Normalize(mOrthogonalBasis);
        }


        public List<IVector> OrthogonalBasis
        {
            get
            {
                return mOrthogonalBasis;
            }
        }

        public List<IVector> OrthoNormalBasis
        {
            get
            {
                return mOrthoNormalBasis;
            }
        }

        public int Dimension
        {
            get
            {
                return mOrthogonalBasis.Count;
            }
        }

        /// <summary>
        /// Get the point in the vector space which is closest to b
        /// </summary>
        /// <param name="b">A vector which may be outside the vector space</param>
        /// <returns>Return point in the vector space closest to b</returns>
        public IVector GetClosedPoint(IVector b)
        {
            IVector b_orth = b.ProjectOrthogonal(mOrthogonalBasis);
            return b.Minus(b_orth);
        }

    }
}
