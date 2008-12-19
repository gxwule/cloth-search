using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Search
{
    public class HSVColorSearcher : ColorSearcher
    {
        public HSVColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.HSVColorVector;
        }
    }
}
