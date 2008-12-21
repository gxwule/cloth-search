using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;
using Zju.Dao;

namespace Zju.Search
{
    public class DaubechiesWaveletSearcher : TextureSearcher
    {
        public DaubechiesWaveletSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : this(limit, ClothUtil.CalcManhattanDistance, wrappedSearcher, maxResult)
        {
           
        }

        public DaubechiesWaveletSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public DaubechiesWaveletSearcher(float limit, ClothDao clothDao, int maxResult)
            : this(limit, ClothUtil.CalcManhattanDistance, clothDao, maxResult)
        {

        }

        public DaubechiesWaveletSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {

        }


    
        public override float[] GetVector(Cloth cloth)
        {
            return cloth.DaubechiesWaveletVector;
        }
    }
}
