using System;
using System.Collections.Generic;
using System.Text;

namespace Zju.Search
{
    public abstract class ColorSearcher : PicSearcher
    {
        public ColorSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, wrappedSearcher, maxResult)
        {
           
        }

        public ColorSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }
    }
}
