using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    /// <summary>
    /// The vector space V spanned by the rows vectors in A
    /// V is the vector space spanned by the row vectors in A
    /// dim V = rank A 
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Val"></typeparam>
    public class RowSpace
    {
        private IMatrix mEchelonA;
        private IMatrix mM; 

        public RowSpace(IMatrix A)
        {
            int numRowExOperations = 0;
            mEchelonA = GaussianElimination.GetEchelonForm(A, out mM, out numRowExOperations);
        }

        public IMatrix Echelon
        {
            get { return mEchelonA; }
        }

        public IMatrix M
        {
            get { return mM; }
        }

        /// <summary>
        /// The basis, which is the set of independent vectors that span the row space of A
        /// </summary>
        public List<IVector> Basis
        {
            get
            {
                List<IVector> basis = new List<IVector>();
                foreach (int row in mEchelonA.RowKeys)
                {
                    IVector v = mEchelonA[row];
                    if (v.IsEmpty)
                    {
                        break;
                    }
                    basis.Add(v);
                }
                return basis;
            }
        }

        /// <summary>
        /// The basis for the null space of A
        /// The null space of matrix A is { v | A*v = 0 }
        /// </summary>
        public List<IVector> NullBasis
        {
            get
            {
                List<IVector> result = new List<IVector>();
                foreach (int row in mEchelonA.RowKeys)
                {
                    IVector v = mEchelonA[row];
                    if (v.IsEmpty)
                    {
                        result.Add(mM[row]);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// The rank of A, which is the dimension of the row space 
        /// </summary>
        public int Rank
        {
            get
            {
                int rank = 0;

                foreach (int row in mEchelonA.RowKeys)
                {
                    IVector v = mEchelonA[row];
                    if (v.IsEmpty)
                    {
                        break;
                    }
                    rank++;
                }

                return rank;
            }
        }

        /// <summary>
        /// Return the dimension of the null space of A
        /// The null space of matrix A is { v | A*v = 0 }
        /// </summary>
        public int Nullity
        {
            get
            {
                ///By Rank-Nullity theorem: for any n-colum matrix A, nullity(A) + rank(A) = n
                return mEchelonA.ColCount - Rank;
            }
        }
    }
}
