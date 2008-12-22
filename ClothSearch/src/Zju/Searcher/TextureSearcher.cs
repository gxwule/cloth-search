using System;
using System.Collections.Generic;
using Zju.Dao;
using Zju.Domain;

namespace Zju.Searcher
{
    public abstract class TextureSearcher : PicSearcher
    {
        public TextureSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IClothDao clothDao, int maxResult)
            : base(picParam, limit, calcDist, clothDao, maxResult)
        {
           
        }

        public TextureSearcher(PicParam picParam, float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(picParam, limit, calcDist, wrappedSearcher, maxResult)
        {

        }
    }
}
