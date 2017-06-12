using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public static class VectorUtils
    {

        public static List<IVector> Normalize(IEnumerable<IVector> vlist)
        {
            List<IVector> vstarlist = new List<IVector>();
            foreach (IVector v in vlist)
            {
                vstarlist.Add(v.Normalize());
            }

            return vstarlist;
        }

        public static List<IVector> Normalize(IEnumerable<IVector> vlist, out List<double> norms)
        {
            norms = new List<double>();
            List<IVector> vstarlist = new List<IVector>();
            foreach (IVector v in vlist)
            {
                norms.Add(v.Norm(2));
                vstarlist.Add(v.Normalize());
            }

            return vstarlist;
        }

        public static List<IVector> RemoveZeroVectors(IEnumerable<IVector> vlist)
        {
            List<IVector> vstarlist = new List<IVector>();
            foreach (IVector v in vlist)
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
