using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Search
{
    public class TextSearcher : BaseSearcher
    {
        public TextSearcher() : base()
        {

        }

        public TextSearcher(IBaseSearcher wrappedSearcher)
            : base(wrappedSearcher)
        {

        }

        public override List<Cloth> Search(BaseParam param)
        {
            return null;
        }
    }
}
