using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class Orthogonalization
    {


        /// <summary>
        /// Convert a set of vectors to a set of orthogonal vectors
        /// </summary>
        /// <param name="vlist"></param>
        /// <returns>The resultant set of orthogonal vectors (i.e. vectors which are mutually perpendicular to each other)</returns>
        public static List<IVector> Orthogonalize(IEnumerable<IVector> vlist)
        {
            List<IVector> vstarlist = new List<IVector>();
            foreach (IVector v in vlist)
            {
                IVector vstar = v.ProjectOrthogonal(vstarlist);
                vstarlist.Add(vstar);
            }
            return vstarlist;
        }

        /// <summary>
        /// V and W are two vector spaces
        /// Return the orthogonal complement of V in W 
        /// </summary>
        /// <param name="V">The spanning set representing V</param>
        /// <param name="W">The spanning set representing W</param>
        /// <returns>The set of vectors { w_i } = Span(W) which are perpendicular to the vectors in V</returns>
        public static List<IVector> Orthogonalize(IEnumerable<IVector> V, IEnumerable<IVector> W)
        {
            List<IVector> vstarlist = VectorUtils.RemoveZeroVectors(Orthogonalize(V));
            foreach (IVector w in W)
            {
                IVector wstar = w.ProjectOrthogonal(vstarlist);
                vstarlist.Add(wstar);
            }

            return vstarlist;
        }

        public static List<IVector> Orthogonalize(IEnumerable<IVector> vlist, out List<IVector> R)
        {
            List<IVector> vstarlist = new List<IVector>();
            R = new List<IVector>();
            int D = vlist.Count();
            foreach (IVector v in vlist)
            {
                Dictionary<int, double> alpha;
                IVector vstar = v.ProjectOrthogonal(vstarlist, out alpha);
                vstarlist.Add(vstar);

                IVector r = vstar.Zero(D, vstar.DefaultValue);

                foreach (int alpha_key in alpha.Keys)
                {
                    r[alpha_key] = alpha[alpha_key];
                }

                R.Add(r);

            }
            return vstarlist;
        }
    }
}
