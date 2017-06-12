using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_matrix
{
    public class SparseVector<Key, Val> : IVector<Key, Val>
    {
        private Dictionary<Key, Val> mInternal = new Dictionary<Key, Val>();
        private Key[] mKeys;
        private static double EPSILON = 1e-20;
        private Val mDefaultVal;

        public SparseVector(Val[] v)
        {
            int length = v.Length;
            mKeys = new Key[length];
            for (int i = 0; i < length; ++i)
            {
                mKeys[i] = (dynamic)i;
            }
            mDefaultVal = (dynamic)0;
            for (int i = 0; i < length; ++i)
            {
                this[(dynamic)i] = v[i];
            }
        }

        public SparseVector(int length, Val default_value)
        {
            mKeys = new Key[length];
            for (int i = 0; i < length; ++i)
            {
                mKeys[i] = (dynamic)i;
            }
            mDefaultVal = default_value;
        }

        public SparseVector(Key[] keys, Val default_val)
        {
            mKeys = (Key[])keys.Clone();
            mDefaultVal = default_val;
        }

        public SparseVector(int length)
        {
            mKeys = new Key[length];
            for (int i = 0; i < length; ++i)
            {
                mKeys[i] = (dynamic)i;
            }
            mDefaultVal = (dynamic)0;
        }

        public SparseVector(Key[] keys)
        {
            mKeys = (Key[])keys.Clone();
            mDefaultVal = (dynamic)0;
        }

        public virtual Val this[Key index]
        {
            get
            {
                Val val;
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

        private static bool Equals(Val val1, Val val2)
        {
            double df = (dynamic)val1 - (dynamic)val2;
            return System.Math.Abs(df) <= EPSILON;
        }

        public virtual int Dimension
        {
            get { return mKeys.Length; }
        }


        public virtual IVector<Key, Val> Clone()
        {
            SparseVector<Key, Val> clone = new SparseVector<Key, Val>(mKeys, mDefaultVal);
            clone.Copy(this);
            return clone;
        }

        public void Copy(IVector<Key, Val> rhs)
        {
            SparseVector<Key, Val> rhs2 = rhs as SparseVector<Key, Val>;
            mInternal.Clear();
            foreach (KeyValuePair<Key, Val> entry in rhs2.mInternal)
            {
                mInternal[entry.Key] = entry.Value;
            }
            mID = rhs2.mID;
        }

        public virtual double Multiply(IVector<Key, Val> rhs)
        {
            double productSum = 0;
            foreach (KeyValuePair<Key, Val> entry in mInternal)
            {
                productSum += (dynamic)entry.Value * (dynamic)rhs[entry.Key]; 
            }
            return productSum;
        }

        public virtual IVector<Key, Val> Pow(double scalar)
        {
            IVector<Key, Val> result = this.Zero(mKeys, mDefaultVal);
            foreach (KeyValuePair<Key, Val> entry in mInternal)
            {
                result[entry.Key] = (dynamic)System.Math.Pow((dynamic)entry.Value, scalar);
            }
            return result;
        }

        public virtual double Sum()
        {
            double sum = 0;
            foreach (KeyValuePair<Key, Val> entry in mInternal)
            {
                sum += (dynamic)entry.Value;
            }
            return sum;
        }

        public virtual IVector<Key, Val> Zero(int length, Val default_value)
        {
            return new SparseVector<Key, Val>(length, default_value);
        }


        public IVector<Key, Val> Zero(Key[] keys, Val default_value)
        {
            return new SparseVector<Key, Val>(keys, default_value);
        }

        public IVector<Key, Val> Multiply(IMatrix<Key, Val> rhs)
        {
            return rhs.Transpose().Multiply(this);
        }


        public Key[] Keys
        {
            get
            {
                return mKeys;
            }
        }

        public IEnumerable<Key> NonEmptyKeys
        {
            get
            {
                return mInternal.Keys;
            }
        }


        public bool HasValue(Key k)
        {
            return mInternal.ContainsKey(k);
        }


        public IVector<Key, Val> Multiply(double scalar)
        {
            IVector<Key, Val> result = this.Clone();
            foreach (Key k in result.Keys)
            {
                result[k] = (dynamic)result[k] * scalar;
            }
            return result;
        }


        public IVector<Key, Val> Add(IVector<Key, Val> rhs)
        {
            IVector<Key, Val> result = Zero(mKeys, mDefaultVal);
            foreach (Key key in mKeys)
            {
                result[key] = (dynamic)this[key] + (dynamic)rhs[key];
            }

            return result;
        }

        public IVector<Key, Val> Minus(IVector<Key, Val> rhs)
        {
            IVector<Key, Val> result = Zero(mKeys, mDefaultVal);
            foreach (Key key in mKeys)
            {
                result[key] = (dynamic)this[key] - (dynamic)rhs[key];
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


        public IVector<Key, Val> ProjectAlong(IVector<Key, Val> rhs)
        {
            double norm_a = rhs.Multiply(rhs);
            
            if(norm_a <= EPSILON)
            {
                return Zero(mKeys, mDefaultVal);
            }
            double sigma = Multiply(rhs) / norm_a;
            return rhs.Multiply(sigma);
        }

        public IVector<Key, Val> ProjectAlong(IVector<Key, Val> rhs, out double sigma)
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


        public IVector<Key, Val> ProjectOrthogonal(IVector<Key, Val> rhs)
        {
            return this.Minus(ProjectAlong(rhs));
        }

        /// <summary>
        /// Return the component of the vector perpendicular to all the vectors in the vector space spanned by vlist
        /// </summary>
        /// <param name="vlist">a set of vectors perpendicular to each other</param>
        /// <returns>the vector perpendicular to all the vectors in the vector space spanned by vlist</returns>
        public IVector<Key, Val> ProjectOrthogonal(IEnumerable<IVector<Key, Val>> vlist)
        {
            IVector<Key, Val> b = this;
            foreach(IVector<Key, Val> v in vlist)
            {
                b = b.Minus(b.ProjectAlong(v));
            }

            return b;
        }

        public Val DefaultValue
        {
            get { return mDefaultVal; }
        }


        public IVector<Key, Val> Normalize()
        {
            double norm = Norm(2); // L2 norm is the cartesian distance
            if (Equals(norm, 0))
            {
                return this.Zero(mKeys, mDefaultVal);
            }
            SparseVector<Key, Val> clone = this.Clone() as SparseVector<Key, Val>;
            
            List<Key> keys = clone.mInternal.Keys.ToList();
            foreach (Key k in keys)
            {
                clone[k] = clone[k] / (dynamic)norm;
            }
            return clone;
        }

        public double Norm(int level)
        {
            if (level == 1)
            {
                double sum = 0;
                foreach (Val val in this.mInternal.Values)
                {
                    sum += System.Math.Abs((dynamic)val);
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
                foreach (Val val in this.mInternal.Values)
                {
                    sum += System.Math.Pow(System.Math.Abs((dynamic)val), level);
                }
                return System.Math.Pow(sum, 1.0 / level);
            }
        }


        public IVector<Key, Val> ProjectOrthogonal(List<IVector<Key, Val>> vlist, out Dictionary<int, double> alpha)
        {
            IVector<Key, Val> b = this;
            double sigma;

            alpha = new Dictionary<int, double>();
            alpha[vlist.Count] = 1;
            
            for (int i=0; i < vlist.Count; ++i)
            {
                IVector<Key, Val> v = vlist[i];
                IVector<Key, Val> b_along = b.ProjectAlong(v, out sigma);
                b = b.Minus(b_along);
                alpha[i] = sigma;
            }

            return b;
        }

        public override bool Equals(object obj)
        {
            SparseVector<Key, Val> rhs = obj as SparseVector<Key, Val>;
            if (Dimension != rhs.Dimension)
            {
                return false;
            }

            foreach (Key k in Keys)
            {
                if(System.Math.Abs((dynamic)this[k] - (dynamic)rhs[k]) > 1e-10)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 31;
            foreach (Val v in mInternal.Values)
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
            foreach (Key k in mInternal.Keys)
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

        protected Key mID = default(Key);
        public Key ID
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


        public IVector<Key, Val> Divide(IVector<Key, Val> rhs)
        {
            IVector<Key, Val> result = Zero(mKeys, mDefaultVal);
            foreach(KeyValuePair<Key, Val> k in mInternal)
            {
                result[k.Key] = (dynamic)k.Value / (dynamic)rhs[k.Key];
            }
            return result;
        }

        public IVector<Key, Val> Sqrt()
        {
            IVector<Key, Val> result = Zero(mKeys, mDefaultVal);
            foreach (KeyValuePair<Key, Val> k in mInternal)
            {
                result[k.Key] = (dynamic) System.Math.Sqrt((dynamic)k.Value);
            }
            return result;
        }

        public IVector<Key, Val> Log()
        {
            IVector<Key, Val> result = Zero(mKeys, mDefaultVal);
            foreach (KeyValuePair<Key, Val> k in mInternal)
            {
                result[k.Key] = (dynamic)System.Math.Log((dynamic)k.Value);
            }

            return result;
        }
    }
}
