using System;
using System.Collections.Generic;
using Zju.Dao;

namespace Zju.Search
{
    public abstract class ColorSearcher : PicSearcher
    {
        public ColorSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(limit, calcDist, clothDao, maxResult)
        {
           
        }

        public ColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }
    }
}
