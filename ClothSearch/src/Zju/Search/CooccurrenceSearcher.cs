using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Search
{
    public class CooccurrenceSearcher : TextureSearcher
    {
        public CooccurrenceSearcher() : this(float.MaxValue, null, DEFAULT_MAX_RESULT)
        {
        }

        public CooccurrenceSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : this(limit, ClothUtil.CalcManhattanDistance, wrappedSearcher, maxResult)
        {
           
        }

        public CooccurrenceSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }


        public override float[] GetVector(Cloth cloth)
        {
            return cloth.CooccurrenceVector;
        }
    }
}
