using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Search
{
    public delegate float DelCalcDist(float[] v1, float[] v2);

    public abstract class BaseSearcher : IBaseSearcher
    {
        protected IBaseSearcher wrappedSearcher;
        protected IClothDao clothDao;
        //protected List<Cloth> clothes;

        protected const int DEFAULT_MAX_RESULT = 200;

        public int MaxResult
        {
            get;
            set;
        }

        #region IBaseSearcher Members

        public BaseSearcher()
            : this(null, DEFAULT_MAX_RESULT)
        {
            
        }

        public BaseSearcher(IBaseSearcher wrappedSearcher)
            : this(wrappedSearcher, DEFAULT_MAX_RESULT)
        {
          
        }

        public BaseSearcher(IBaseSearcher wrappedSearcher, int maxResult)
        {
            this.wrappedSearcher = wrappedSearcher;
            this.clothDao = new ClothDao();
            this.MaxResult = maxResult;
        }

        public abstract List<Cloth> Search(BaseParam param);

        #endregion
    }
}
