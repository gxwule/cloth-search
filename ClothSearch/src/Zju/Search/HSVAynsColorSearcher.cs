using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Search
{
    public class HSVAynsColorSearcher : ColorSearcher
    {
        public HSVAynsColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.HSVAynsColorVector;
        }
    }
}
