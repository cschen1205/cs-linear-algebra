using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class Orthogonalization<Key, Val>
    {


        /// <summary>
        /// Convert a set of vectors to a set of orthogonal vectors
        /// </summary>
        /// <param name="vlist"></param>
        /// <returns>The resultant set of orthogonal vectors (i.e. vectors which are mutually perpendicular to each other)</returns>
        public static List<IVector<Key, Val>> Orthogonalize(IEnumerable<IVector<Key, Val>> vlist)
        {
            List<IVector<Key, Val>> vstarlist = new List<IVector<Key, Val>>();
            foreach (IVector<Key, Val> v in vlist)
            {
                IVector<Key, Val> vstar = v.ProjectOrthogonal(vstarlist);
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
        public static List<IVector<Key, Val>> Orthogonalize(IEnumerable<IVector<Key, Val>> V, IEnumerable<IVector<Key, Val>> W)
        {
            List<IVector<Key, Val>> vstarlist = VectorUtils<Key, Val>.RemoveZeroVectors(Orthogonalize(V));
            foreach (IVector<Key, Val> w in W)
            {
                IVector<Key, Val> wstar = w.ProjectOrthogonal(vstarlist);
                vstarlist.Add(wstar);
            }

            return vstarlist;
        }

        public static List<IVector<int, Val>> Orthogonalize(IEnumerable<IVector<int, Val>> vlist, out List<IVector<int, Val>> R)
        {
            List<IVector<int, Val>> vstarlist = new List<IVector<int, Val>>();
            R = new List<IVector<int, Val>>();
            int D = vlist.Count();
            foreach (IVector<int, Val> v in vlist)
            {
                Dictionary<int, double> alpha;
                IVector<int, Val> vstar = v.ProjectOrthogonal(vstarlist, out alpha);
                vstarlist.Add(vstar);

                IVector<int, Val> r = vstar.Zero(D, vstar.DefaultValue);

                foreach (int alpha_key in alpha.Keys)
                {
                    r[alpha_key] = (dynamic)alpha[alpha_key];
                }

                R.Add(r);

            }
            return vstarlist;
        }
    }
}
