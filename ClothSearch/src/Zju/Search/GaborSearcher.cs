using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;
using Zju.Dao;

namespace Zju.Search
{
    public class GaborSearcher : TextureSearcher
    {
        public GaborSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : this(limit, ClothUtil.CalcGaborDistance, wrappedSearcher, maxResult)
        {
           
        }

        public GaborSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public GaborSearcher(float limit, ClothDao clothDao, int maxResult)
            : this(limit, ClothUtil.CalcGaborDistance, clothDao, maxResult)
        {

        }

        public GaborSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {

        }
    
        public override float[] GetVector(Cloth cloth)
        {
            return cloth.GaborVector;
        }
    }
}
