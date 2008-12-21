using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;
using Zju.Dao;

namespace Zju.Search
{
    public class CooccurrenceSearcher : TextureSearcher
    {
        public CooccurrenceSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : this(limit, ClothUtil.CalcManhattanDistance, wrappedSearcher, maxResult)
        {
           
        }

        public CooccurrenceSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public CooccurrenceSearcher(float limit, ClothDao clothDao, int maxResult)
            : this(limit, ClothUtil.CalcManhattanDistance, clothDao, maxResult)
        {

        }

        public CooccurrenceSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {

        }


        public override float[] GetVector(Cloth cloth)
        {
            return cloth.CooccurrenceVector;
        }
    }
}
