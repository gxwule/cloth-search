using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Search
{
    public class RGBSeparateColorSearcher : ColorSearcher
    {
        public RGBSeparateColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public RGBSeparateColorSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.RGBSeparateColorVector;
        }
    }
}
