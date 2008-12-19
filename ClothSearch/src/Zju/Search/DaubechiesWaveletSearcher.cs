using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Search
{
    public class DaubechiesWaveletSearcher : TextureSearcher
    {
        public DaubechiesWaveletSearcher() : this(float.MaxValue, null, DEFAULT_MAX_RESULT)
        {
        }

        public DaubechiesWaveletSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : this(limit, ClothUtil.CalcManhattanDistance, wrappedSearcher, maxResult)
        {
           
        }

        public DaubechiesWaveletSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }


    
        public override float[] GetVector(Cloth cloth)
        {
            return cloth.DaubechiesWaveletVector;
        }
    }
}
