using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Search
{
    public class GaborSearcher : TextureSearcher
    {
        public GaborSearcher() : this(float.MaxValue, null, DEFAULT_MAX_RESULT)
        {
        }

        public GaborSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : this(limit, ClothUtil.CalcGaborDistance, wrappedSearcher, maxResult)
        {
           
        }

        public GaborSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }
    
        public override float[] GetVector(Cloth cloth)
        {
            return cloth.GaborVector;
        }
    }
}
