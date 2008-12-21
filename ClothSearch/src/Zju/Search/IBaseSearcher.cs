using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Search
{
    public interface IBaseSearcher
    {
        /// <summary>
        /// Search results in some data collection, return the results with its number below the <code>maxResult</code>.
        /// </summary>
        /// <param name="param">Search conditions.</param>
        List<Cloth> Search(BaseParam param);
    }
}
