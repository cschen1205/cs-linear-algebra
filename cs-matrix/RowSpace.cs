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
    public class RowSpace<Key,Val> 
    {
        private IMatrix<Key, Val> mEchelonA;
        private IMatrix<Key, Val> mM; 

        public RowSpace(IMatrix<Key, Val> A)
        {
            int numRowExOperations = 0;
            mEchelonA = GaussianElimination.GetEchelonForm(A, out mM, out numRowExOperations);
        }

        public IMatrix<Key, Val> Echelon
        {
            get { return mEchelonA; }
        }

        public IMatrix<Key, Val> M
        {
            get { return mM; }
        }

        /// <summary>
        /// The basis, which is the set of independent vectors that span the row space of A
        /// </summary>
        public List<IVector<Key, Val>> Basis
        {
            get
            {
                List<IVector<Key, Val>> basis = new List<IVector<Key, Val>>();
                foreach (Key row in mEchelonA.RowKeys)
                {
                    IVector<Key, Val> v = mEchelonA[row];
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
        public List<IVector<Key, Val>> NullBasis
        {
            get
            {
                List<IVector<Key, Val>> result = new List<IVector<Key,Val>>();
                foreach (Key row in mEchelonA.RowKeys)
                {
                    IVector<Key, Val> v = mEchelonA[row];
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

                foreach (Key row in mEchelonA.RowKeys)
                {
                    IVector<Key, Val> v = mEchelonA[row];
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
