using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Searcher
{
    public interface IBaseSearcher
    {
        /// <summary>
        /// Search results in some data collection, return the certain number of results.
        /// </summary>
        List<Cloth> Search();
    }
}
