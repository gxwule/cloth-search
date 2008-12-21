using System;
using System.Collections.Generic;
using Zju.Dao;

namespace Zju.Searcher
{
    public abstract class ColorSearcher : PicSearcher
    {
        public ColorSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IClothDao clothDao, int maxResult)
            : base(picParam, limit, calcDist, clothDao, maxResult)
        {
           
        }

        public ColorSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(picParam, limit, calcDist, wrappedSearcher, maxResult)
        {

        }
    }
}
