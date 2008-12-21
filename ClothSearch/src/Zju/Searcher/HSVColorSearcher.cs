using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Search
{
    public class HSVColorSearcher : ColorSearcher
    {
        public HSVColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public HSVColorSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.HSVColorVector;
        }
    }
}
