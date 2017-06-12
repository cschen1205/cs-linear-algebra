using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class SparseVector : IVector
    {
        private Dictionary<int, double> mInternal = new Dictionary<int, double>();
        private int[] mKeys;
        private static double EPSILON = 1e-20;
        private double mDefaultVal;

        public SparseVector(double[] v)
        {
            int length = v.Length;
            mKeys = new int[length];
            for (int i = 0; i < length; ++i)
            {
                mKeys[i] = i;
            }
            mDefaultVal = 0;
            for (int i = 0; i < length; ++i)
            {
                this[i] = v[i];
            }
        }

        public SparseVector(int length, double default_value)
        {
            mKeys = new int[length];
            for (int i = 0; i < length; ++i)
            {
                mKeys[i] = i;
            }
            mDefaultVal = default_value;
        }

        public SparseVector(int[] keys, double default_val)
        {
            mKeys = (int[])keys.Clone();
            mDefaultVal = default_val;
        }

        public SparseVector(int length)
        {
            mKeys = new int[length];
            for (int i = 0; i < length; ++i)
            {
                mKeys[i] = i;
            }
            mDefaultVal = 0;
        }

        public SparseVector(int[] keys)
        {
            mKeys = (int[])keys.Clone();
            mDefaultVal = 0;
        }

        public virtual double this[int index]
        {
            get
            {
                double val;
                if(mInternal.TryGetValue(index, out val))
                {
                    return val;
                }
                return mDefaultVal;
            }
            set
            {
#if SK_LINALG_CHECK_ARRAY_BOUNDARY
                if (!mKeys.Contains(index))
                {
                    throw new IndexOutOfRangeException();
                }
#endif

                if (Equals(value, mDefaultVal))
                {
                    mInternal.Remove(index);
                }
                else
                {
                    mInternal[index] = value;
                }
            }
        }

        private static bool Equals(double val1, double val2)
        {
            double df = val1 - val2;
            return System.Math.Abs(df) <= EPSILON;
        }

        public virtual int Dimension
        {
            get { return mKeys.Length; }
        }


        public virtual IVector Clone()
        {
            SparseVector clone = new SparseVector(mKeys, mDefaultVal);
            clone.Copy(this);
            return clone;
        }

        public void Copy(IVector rhs)
        {
            SparseVector rhs2 = rhs as SparseVector;
            mInternal.Clear();
            foreach (KeyValuePair<int, double> entry in rhs2.mInternal)
            {
                mInternal[entry.Key] = entry.Value;
            }
            mID = rhs2.mID;
        }

        public virtual double Multiply(IVector rhs)
        {
            double productSum = 0;
            foreach (KeyValuePair<int, double> entry in mInternal)
            {
                productSum += entry.Value * rhs[entry.Key]; 
            }
            return productSum;
        }

        public virtual IVector Pow(double scalar)
        {
            IVector result = this.Zero(mKeys, mDefaultVal);
            foreach (KeyValuePair<int, double> entry in mInternal)
            {
                result[entry.Key] = System.Math.Pow(entry.Value, scalar);
            }
            return result;
        }

        public virtual double Sum()
        {
            double sum = 0;
            foreach (KeyValuePair<int, double> entry in mInternal)
            {
                sum += entry.Value;
            }
            return sum;
        }

        public virtual IVector Zero(int length, double default_value)
        {
            return new SparseVector(length, default_value);
        }


        public IVector Zero(int[] keys, double default_value)
        {
            return new SparseVector(keys, default_value);
        }

        public IVector Multiply(IMatrix rhs)
        {
            return rhs.Transpose().Multiply(this);
        }


        public int[] Keys
        {
            get
            {
                return mKeys;
            }
        }

        public IEnumerable<int> NonEmptyKeys
        {
            get
            {
                return mInternal.Keys;
            }
        }


        public bool HasValue(int k)
        {
            return mInternal.ContainsKey(k);
        }


        public IVector Multiply(double scalar)
        {
            IVector result = this.Clone();
            foreach (int k in result.Keys)
            {
                result[k] = result[k] * scalar;
            }
            return result;
        }


        public IVector Add(IVector rhs)
        {
            IVector result = Zero(mKeys, mDefaultVal);
            foreach (int key in mKeys)
            {
                result[key] = this[key] + rhs[key];
            }

            return result;
        }

        public IVector Minus(IVector rhs)
        {
            IVector result = Zero(mKeys, mDefaultVal);
            foreach (int key in mKeys)
            {
                result[key] = this[key] - rhs[key];
            }

            return result;
        }

        


        public bool IsEmpty
        {
            get 
            {
                return mInternal.Count == 0 || Multiply(this) <= EPSILON;
            }
        }


        public IVector ProjectAlong(IVector rhs)
        {
            double norm_a = rhs.Multiply(rhs);
            
            if(norm_a <= EPSILON)
            {
                return Zero(mKeys, mDefaultVal);
            }
            double sigma = Multiply(rhs) / norm_a;
            return rhs.Multiply(sigma);
        }

        public IVector ProjectAlong(IVector rhs, out double sigma)
        {
            double norm_a = rhs.Multiply(rhs);

            sigma = 0;
            if (norm_a <= EPSILON)
            {
                return Zero(mKeys, mDefaultVal);
            }
            sigma = Multiply(rhs) / norm_a;
            return rhs.Multiply(sigma);
        }


        public IVector ProjectOrthogonal(IVector rhs)
        {
            return this.Minus(ProjectAlong(rhs));
        }

        /// <summary>
        /// Return the component of the vector perpendicular to all the vectors in the vector space spanned by vlist
        /// </summary>
        /// <param name="vlist">a set of vectors perpendicular to each other</param>
        /// <returns>the vector perpendicular to all the vectors in the vector space spanned by vlist</returns>
        public IVector ProjectOrthogonal(IEnumerable<IVector> vlist)
        {
            IVector b = this;
            foreach(IVector v in vlist)
            {
                b = b.Minus(b.ProjectAlong(v));
            }

            return b;
        }

        public double DefaultValue
        {
            get { return mDefaultVal; }
        }


        public IVector Normalize()
        {
            double norm = Norm(2); // L2 norm is the cartesian distance
            if (Equals(norm, 0))
            {
                return this.Zero(mKeys, mDefaultVal);
            }
            SparseVector clone = this.Clone() as SparseVector;
            
            List<int> keys = clone.mInternal.Keys.ToList();
            foreach (int k in keys)
            {
                clone[k] = clone[k] / norm;
            }
            return clone;
        }

        public double Norm(int level)
        {
            if (level == 1)
            {
                double sum = 0;
                foreach (double val in this.mInternal.Values)
                {
                    sum += System.Math.Abs(val);
                }
                return sum;
            }
            else if (level == 2)
            {
                return System.Math.Sqrt(Multiply(this));
            }
            else
            {
                double sum = 0;
                foreach (double val in this.mInternal.Values)
                {
                    sum += System.Math.Pow(System.Math.Abs(val), level);
                }
                return System.Math.Pow(sum, 1.0 / level);
            }
        }


        public IVector ProjectOrthogonal(List<IVector> vlist, out Dictionary<int, double> alpha)
        {
            IVector b = this;
            double sigma;

            alpha = new Dictionary<int, double>();
            alpha[vlist.Count] = 1;
            
            for (int i=0; i < vlist.Count; ++i)
            {
                IVector v = vlist[i];
                IVector b_along = b.ProjectAlong(v, out sigma);
                b = b.Minus(b_along);
                alpha[i] = sigma;
            }

            return b;
        }

        public override bool Equals(object obj)
        {
            SparseVector rhs = obj as SparseVector;
            if (Dimension != rhs.Dimension)
            {
                return false;
            }

            foreach (int k in Keys)
            {
                if(System.Math.Abs(this[k] - rhs[k]) > 1e-10)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 31;
            foreach (double v in mInternal.Values)
            {
                hash = hash * 31 + v.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool first = true;
            foreach (int k in mInternal.Keys)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.AppendFormat("\"{0}\":{1}", k, mInternal[k]);
            }
            sb.Append("}");
            return sb.ToString();
        }

        protected int mID = 0;
        public int ID
        {
            get
            {
                return mID;
            }
            set
            {
                mID = value;
            }
        }


        public IVector Divide(IVector rhs)
        {
            IVector result = Zero(mKeys, mDefaultVal);
            foreach(KeyValuePair<int, double> k in mInternal)
            {
                result[k.Key] = k.Value / rhs[k.Key];
            }
            return result;
        }

        public IVector Sqrt()
        {
            IVector result = Zero(mKeys, mDefaultVal);
            foreach (KeyValuePair<int, double> k in mInternal)
            {
                result[k.Key] =  System.Math.Sqrt(k.Value);
            }
            return result;
        }

        public IVector Log()
        {
            IVector result = Zero(mKeys, mDefaultVal);
            foreach (KeyValuePair<int, double> k in mInternal)
            {
                result[k.Key] = System.Math.Log(k.Value);
            }

            return result;
        }
    }
}
