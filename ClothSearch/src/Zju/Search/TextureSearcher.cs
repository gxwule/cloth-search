using System;
using System.Collections.Generic;
using System.Text;

namespace Zju.Search
{
    public abstract class TextureSearcher : PicSearcher
    {
        public TextureSearcher(float limit, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, wrappedSearcher, maxResult)
        {
           
        }

        public TextureSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(limit, calcDist, wrappedSearcher, maxResult)
        {

        }
    }
}
