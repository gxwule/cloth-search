using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Search
{
    public class HSVAynsColorSearcher : ColorSearcher
    {
        public HSVAynsColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public HSVAynsColorSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.HSVAynsColorVector;
        }
    }
}
