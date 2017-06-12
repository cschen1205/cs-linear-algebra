using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public interface IVector
    {
        double this[int index]
        {
            get;
            set;
        }

        bool HasValue(int k);

        int Dimension
        {
            get;
        }

        IVector Clone();
        void Copy(IVector rhs);

        IVector Zero(int length, double default_value);
        IVector Zero(int[] keys, double default_value);
        
        double Multiply(IVector rhs);
        IVector Multiply(IMatrix rhs);
        IVector Multiply(double scalar);
        

        IVector ProjectAlong(IVector rhs);
        IVector ProjectAlong(IVector rhs, out double sigma);
        IVector ProjectOrthogonal(IVector rhs);

        /// <summary>
        /// Return the component of the vector perpendicular to all the vectors in the vector space spanned by vlist
        /// </summary>
        /// <param name="vlist">a set of mutually orthogonal vectors (i.e., perpendicular to each other)</param>
        /// <returns>the vector perpendicular to all the vectors in the vector space spanned by vlist</returns>
        IVector ProjectOrthogonal(IEnumerable<IVector> vlist);

        /// <summary>
        /// Return the component of the vector perpendicular to all the vectors in the vector space spanned by vlist
        /// </summary>
        /// <param name="vlist">a set of mutually orthogonal vectors (i.e., perpendicular to each other)</param>
        /// <param name="alpha">a dictionary storing the project length along the calculation</param>
        /// <returns>the vector perpendicular to all the vectors in the vector space spanned by vlist</returns>
        IVector ProjectOrthogonal(List<IVector> vlist, out Dictionary<int, double> alpha);

        IVector Add(IVector rhs);
        IVector Minus(IVector rhs);
        IVector Divide(IVector rhs);
        IVector Pow(double scalar);
        IVector Sqrt();
        IVector Log();
        double Sum();

        int[] Keys { get; }
        IEnumerable<int> NonEmptyKeys { get; }
        int ID { get; set; }

        bool IsEmpty { get; }
        double DefaultValue { get; }

        IVector Normalize();

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
