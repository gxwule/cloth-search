using System;
using System.Collections.Generic;
using Zju.Util;
using Zju.Dao;
using Zju.Domain;

namespace Zju.Service
{
    public class ClothSearchService : IClothSearchService
    {
        private IClothDao clothDao;

        public ClothSearchService()
        {

        }

        public ClothSearchService(IClothDao clothDao)
        {
            this.clothDao = clothDao;
        }

        #region IClothSearchService Members

        public List<Cloth> SearchByText(string words, ColorEnum colors, ShapeEnum shapes)
        {
            List<Cloth> clothesByColor = clothDao.FindAllByColors(colors);
            List<Cloth> clothesByShape = clothDao.FindAllByShapes(shapes);

            List<List<Cloth>> clothLists = new List<List<Cloth>>();
            clothLists.Add(clothesByColor);
            clothLists.Add(clothesByShape);

            return intersectClothLists(clothLists);
        }

        #endregion

        /// <summary>
        /// Each cloth list in <code>clothLists</code> should not contain duplicate element, or the algorithm will be acted correctly.
        /// </summary>
        /// <param name="clothLists"></param>
        /// <returns></returns>
        private List<Cloth> intersectClothLists(List<List<Cloth>> clothLists)
        {
            Dictionary<Cloth, int> tc = new Dictionary<Cloth, int>();
            foreach (List<Cloth> clothList in clothLists)
            {
                foreach (Cloth cloth in clothList)
                {
                    if (!tc.ContainsKey(cloth))
                    {
                        tc[cloth] = 1;
                    }
                    else
                    {
                        tc[cloth]++;
                    }
                }
            }

            int nLists = clothLists.Count;
            List<Cloth> result = new List<Cloth>();
            foreach (KeyValuePair<Cloth, int> kvp in tc)
            {
                if (kvp.Value == nLists)
                {
                    result.Add(kvp.Key);
                }
            }

            return result;
        }

        private List<Cloth> unionClothLists(List<List<Cloth>> clothLists)
        {
            HashSet<Cloth> hs = new HashSet<Cloth>();
            foreach (List<Cloth> clothList in clothLists)
            {
                foreach (Cloth cloth in clothList)
                {
                    hs.Add(cloth);
                }
            }

            List<Cloth> result = new List<Cloth>();
            foreach (Cloth cloth in hs)
            {
                result.Add(cloth);
            }

            return result;
        }

        public IClothDao ClothDao
        {
            set { this.clothDao = value; }
        }
    }
}
