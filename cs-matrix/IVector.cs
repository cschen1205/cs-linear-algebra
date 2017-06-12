using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public interface IVector<Key, Val>
    {
        Val this[Key index]
        {
            get;
            set;
        }

        bool HasValue(Key k);

        int Dimension
        {
            get;
        }

        IVector<Key, Val> Clone();
        void Copy(IVector<Key, Val> rhs);

        IVector<Key, Val> Zero(int length, Val default_value);
        IVector<Key, Val> Zero(Key[] keys, Val default_value);
        
        double Multiply(IVector<Key, Val> rhs);
        IVector<Key, Val> Multiply(IMatrix<Key, Val> rhs);
        IVector<Key, Val> Multiply(double scalar);
        

        IVector<Key, Val> ProjectAlong(IVector<Key, Val> rhs);
        IVector<Key, Val> ProjectAlong(IVector<Key, Val> rhs, out double sigma);
        IVector<Key, Val> ProjectOrthogonal(IVector<Key, Val> rhs);

        /// <summary>
        /// Return the component of the vector perpendicular to all the vectors in the vector space spanned by vlist
        /// </summary>
        /// <param name="vlist">a set of mutually orthogonal vectors (i.e., perpendicular to each other)</param>
        /// <returns>the vector perpendicular to all the vectors in the vector space spanned by vlist</returns>
        IVector<Key, Val> ProjectOrthogonal(IEnumerable<IVector<Key, Val>> vlist);

        /// <summary>
        /// Return the component of the vector perpendicular to all the vectors in the vector space spanned by vlist
        /// </summary>
        /// <param name="vlist">a set of mutually orthogonal vectors (i.e., perpendicular to each other)</param>
        /// <param name="alpha">a dictionary storing the project length along the calculation</param>
        /// <returns>the vector perpendicular to all the vectors in the vector space spanned by vlist</returns>
        IVector<Key, Val> ProjectOrthogonal(List<IVector<Key, Val>> vlist, out Dictionary<int, double> alpha);

        IVector<Key, Val> Add(IVector<Key, Val> rhs);
        IVector<Key, Val> Minus(IVector<Key, Val> rhs);
        IVector<Key, Val> Divide(IVector<Key, Val> rhs);
        IVector<Key, Val> Pow(double scalar);
        IVector<Key, Val> Sqrt();
        IVector<Key, Val> Log();
        double Sum();

        Key[] Keys { get; }
        IEnumerable<Key> NonEmptyKeys { get; }
        Key ID { get; set; }

        bool IsEmpty { get; }
        Val DefaultValue { get; }

        IVector<Key, Val> Normalize();

        /// <summary>
        /// L1 : sum of absoluate values of each dimension
        /// L2 : square root of sum of squars of each dimension
        /// Ln : (sum_i (this[i]^n)^(1/n)
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        double Norm(int level);

        
    }
}
