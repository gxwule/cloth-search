using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Searcher
{
    public class HSVColorSearcher : ColorSearcher
    {
        public HSVColorSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(picParam, limit, calcDist, wrappedSearcher, maxResult)
        {

        }

        public HSVColorSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IClothDao clothDao, int maxResult)
            : base(picParam, limit, calcDist, clothDao, maxResult)
        {

        }

        public override float[] GetVector(Cloth cloth)
        {
            return cloth.HSVColorVector;
        }
    }
}
