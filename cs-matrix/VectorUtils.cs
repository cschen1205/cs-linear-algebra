using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public static class VectorUtils<Key, Val>
    {

        public static List<IVector<Key, Val>> Normalize(IEnumerable<IVector<Key, Val>> vlist)
        {
            List<IVector<Key, Val>> vstarlist = new List<IVector<Key, Val>>();
            foreach (IVector<Key, Val> v in vlist)
            {
                vstarlist.Add(v.Normalize());
            }

            return vstarlist;
        }

        public static List<IVector<Key, Val>> Normalize(IEnumerable<IVector<Key, Val>> vlist, out List<double> norms)
        {
            norms = new List<double>();
            List<IVector<Key, Val>> vstarlist = new List<IVector<Key, Val>>();
            foreach (IVector<Key, Val> v in vlist)
            {
                norms.Add(v.Norm(2));
                vstarlist.Add(v.Normalize());
            }

            return vstarlist;
        }

        public static List<IVector<Key, Val>> RemoveZeroVectors(IEnumerable<IVector<Key, Val>> vlist)
        {
            List<IVector<Key, Val>> vstarlist = new List<IVector<Key, Val>>();
            foreach (IVector<Key, Val> v in vlist)
            {
                if (!v.IsEmpty)
                {
                    vstarlist.Add(v);
                }
            }

            return vstarlist;
        }
    }
}
