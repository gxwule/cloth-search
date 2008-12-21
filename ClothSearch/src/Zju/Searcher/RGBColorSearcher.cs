using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Searcher
{
    public class RGBColorSearcher : ColorSearcher
    {
        public RGBColorSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(picParam, limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public RGBColorSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IClothDao clothDao, int maxResult)
            : base(picParam, limit, calcDist, clothDao, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.RGBColorVector;
        }
    }
}
