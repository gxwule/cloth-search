using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;

namespace Zju.Search
{
    public class TextSearcher : BaseSearcher
    {
        public TextSearcher(ClothDao clothDao)
            : base(clothDao)
        {

        }

        public override List<Cloth> Search(BaseParam param)
        {
            List<Cloth> clothes = null;
            if (clothDao != null)
            {
                if (!(param is TextParam))
                {
                    throw new ArgumentException("The parameter must be of TextParam in TextSearcher.");
                }

                clothes = clothDao.FindAll();
            }

            if (null == clothes)
            {
                throw new NullReferenceException("ClothDao are null, or some error happened.");
            }

            TextParam tp = (TextParam)param;
            string words = tp.Words;
            ColorEnum colors = tp.Colors;
            ShapeEnum shapes = tp.Shapes;

            List<List<Cloth>> clothLists = new List<List<Cloth>>();

            if (colors != ColorEnum.NONE)
            {
                List<Cloth> clothesByColor = clothDao.FindAllByColors(colors);
                if (clothesByColor.Count > 0)
                {
                    clothLists.Add(clothesByColor);
                }
                else
                {
                    // empty list
                    return clothesByColor;
                }
            }


            if (shapes != ShapeEnum.NONE)
            {
                List<Cloth> clothesByShape = clothDao.FindAllByShapes(shapes);
                if (clothesByShape.Count > 0)
                {
                    clothLists.Add(clothesByShape);
                }
                else
                {
                    // empty list
                    return clothesByShape;
                }
            }


            if (!String.IsNullOrEmpty(words))
            {
                string[] patterns = words.Split(new char[] { ',', ' ', '\t' });
                List<List<Cloth>> clothListsByWords = new List<List<Cloth>>();
                foreach (string pattern in patterns)
                {
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        List<Cloth> clothesByPattern = clothDao.FindAllByPattern(pattern);

                        if (clothesByPattern.Count > 0)
                        {
                            clothListsByWords.Add(clothesByPattern);
                        }
                    }
                }
                if (clothListsByWords.Count > 0)
                {
                    clothLists.Add(ClothUtil.UnionClothLists(clothListsByWords));
                }
                else
                {
                    // empty list
                    return new List<Cloth>();
                }
            }

            return ClothUtil.IntersectClothLists(clothLists);
        }
    }
}
