using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;
using Zju.Dao;

namespace Zju.Search
{
    public abstract class PicSearcher : BaseSearcher
    {
        protected float limit;
        protected DelCalcDist calcDist;

        protected static readonly DelCalcDist DEFAULT_CALC_DIST_DELEGATE = new DelCalcDist(ClothUtil.CalcManhattanDistance);

        public PicSearcher(float limit, DelCalcDist calcDist, IBaseSearcher wrappedSearcher, int maxResult)
            : base(wrappedSearcher, maxResult)
        {
            this.limit = limit;
            this.calcDist = calcDist;
        }

        public PicSearcher(float limit, DelCalcDist calcDist, ClothDao clothDao, int maxResult)
            : base(clothDao, maxResult)
        {
            this.limit = limit;
            this.calcDist = calcDist;
        }

        public override List<Cloth> Search(BaseParam param)
        {
            List<Cloth> clothes = null;
            if (wrappedSearcher != null)
            {
                clothes = wrappedSearcher.Search(param);
            }
            else if (clothDao != null)
            {
                if (!(param is PicParam))
                {
                    throw new ArgumentException("The parameter must be of PicParam in PicSearcher.");
                }

                clothes = clothDao.FindAll();
            }

            if (null == clothes)
            {
                throw new NullReferenceException("Both wrappedSearcher and clothDao are null, or some error happened.");
            }

            float[] featureVector = ((PicParam)param).Feature;
            SortedDictionary<float, List<Cloth>> sortClothes = new SortedDictionary<float, List<Cloth>>();
            foreach (Cloth cloth in clothes)
            {
                float md = calcDist(featureVector, GetVector(cloth));
                if (md <= limit)
                {
                    if (!sortClothes.ContainsKey(md))
                    {
                        sortClothes[md] = new List<Cloth>();
                    }
                    sortClothes[md].Add(cloth);
                }
            }

            List<Cloth> resultClothes = new List<Cloth>();
            foreach (List<Cloth> cs in sortClothes.Values)
            {
                resultClothes.AddRange(cs);
            }

            if (resultClothes.Count > MaxResult)
            {
                return resultClothes.GetRange(0, MaxResult);
            }

            return resultClothes;
        }

        public float Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        /// <summary>
        /// Get the related vector of this search type.
        /// </summary>
        /// <param name="cloth"></param>
        /// <returns></returns>
        public abstract float[] GetVector(Cloth cloth);
    }
}
