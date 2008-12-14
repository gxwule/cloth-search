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

        private float colorMDLimit;

        private float textureMDLimit;

        private float gaborMDLimit;

        private float cooccurrenceMDLimit;

        public ClothSearchService() : this(null)
        {

        }

        public ClothSearchService(IClothDao clothDao)
        {
            this.clothDao = clothDao;
            colorMDLimit = SearchConstants.ColorMDLimits[0];
            textureMDLimit = SearchConstants.TextureMDLimits[0];
        }

        #region IClothSearchService Members

        public List<Cloth> SearchByText(string words, ColorEnum colors, ShapeEnum shapes)
        {
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

        public List<Cloth> SearchByPicColor(int[] colorVector)
        {
            SortedDictionary<float, List<Cloth>> sortClothes = new SortedDictionary<float, List<Cloth>>();
            List<Cloth> allClothes = clothDao.FindAll();
            foreach (Cloth cloth in allClothes)
            {
                float md = ClothUtil.CalcManhattanDistance(colorVector, cloth.ColorVector);
                if (md <= colorMDLimit)
                {
                    if (!sortClothes.ContainsKey(md))
                    {
                        sortClothes[md] = new List<Cloth>();
                    }
                    sortClothes[md].Add(cloth);
                }
            }

            List<Cloth> clothes = new List<Cloth>();
            foreach (List<Cloth> cs in sortClothes.Values)
            {
                clothes.AddRange(cs);
            }

            if (clothes.Count > 200)
            {
                return clothes.GetRange(0, 200);
            }

            return clothes;
        }

        public List<Cloth> SearchByPicTexture(float[] textureVector)
        {
            SortedDictionary<float, List<Cloth>> sortClothes = new SortedDictionary<float, List<Cloth>>();
            List<Cloth> allClothes = clothDao.FindAll();
            foreach (Cloth cloth in allClothes)
            {
                float md = ClothUtil.CalcManhattanDistance(textureVector, cloth.TextureVector);
                if (md <= textureMDLimit)
                {
                    if (!sortClothes.ContainsKey(md))
                    {
                        sortClothes[md] = new List<Cloth>();
                    }
                    sortClothes[md].Add(cloth);
                }
            }

            List<Cloth> clothes = new List<Cloth>();
            foreach (List<Cloth> cs in sortClothes.Values)
            {
                clothes.AddRange(cs);
            }

            if (clothes.Count > 200)
            {
                return clothes.GetRange(0, 200);
            }

            return clothes;
        }

        public List<Cloth> SearchByPicGabor(float[] gaborVector)
        {
            SortedDictionary<float, List<Cloth>> sortClothes = new SortedDictionary<float, List<Cloth>>();
            List<Cloth> allClothes = clothDao.FindAll();
            foreach (Cloth cloth in allClothes)
            {
                //float md = ClothUtil.CalcManhattanDistance(gaborVector, cloth.GaborVector);
                float md = ClothUtil.CalcGaborDistance(gaborVector, cloth.GaborVector);
                if (md <= gaborMDLimit)
                {
                    if (!sortClothes.ContainsKey(md))
                    {
                        sortClothes[md] = new List<Cloth>();
                    }
                    sortClothes[md].Add(cloth);
                }
            }

            List<Cloth> clothes = new List<Cloth>();
            foreach (List<Cloth> cs in sortClothes.Values)
            {
                clothes.AddRange(cs);
            }

            if (clothes.Count > 200)
            {
                return clothes.GetRange(0, 200);
            }

            return clothes;
        }

        public List<Cloth> SearchByPicCooccurrence(float[] cooccurrenceVector)
        {
            SortedDictionary<float, List<Cloth>> sortClothes = new SortedDictionary<float, List<Cloth>>();
            List<Cloth> allClothes = clothDao.FindAll();
            foreach (Cloth cloth in allClothes)
            {
                float md = ClothUtil.CalcManhattanDistance(cooccurrenceVector, cloth.CooccurrenceVector);
                if (md <= cooccurrenceMDLimit)
                {
                    if (!sortClothes.ContainsKey(md))
                    {
                        sortClothes[md] = new List<Cloth>();
                    }
                    sortClothes[md].Add(cloth);
                }
            }

            List<Cloth> clothes = new List<Cloth>();
            foreach (List<Cloth> cs in sortClothes.Values)
            {
                clothes.AddRange(cs);
            }

            if (clothes.Count > 200)
            {
                return clothes.GetRange(0, 200);
            }

            return clothes;
        }

        public List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, int[] colorVector)
        {
            List<List<Cloth>> clothLists = new List<List<Cloth>>();

            List<Cloth> clothesByText = SearchByText(words, colors, shapes);
            if (clothesByText != null && clothesByText.Count > 0)
            {
                clothLists.Add(clothesByText);
            }

            List<Cloth> clothesByPicColor = SearchByPicColor(colorVector);
            if (clothesByPicColor != null && clothesByPicColor.Count > 0)
            {
                clothLists.Add(clothesByPicColor);
            }

            return ClothUtil.IntersectClothLists(clothLists);
        }

        public List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, float[] textureVector)
        {
            List<List<Cloth>> clothLists = new List<List<Cloth>>();

            List<Cloth> clothesByText = SearchByText(words, colors, shapes);
            if (clothesByText != null && clothesByText.Count > 0)
            {
                clothLists.Add(clothesByText);
            }

            List<Cloth> clothesByTexture = SearchByPicTexture(textureVector);
            if (clothesByTexture != null && clothesByTexture.Count > 0)
            {
                clothLists.Add(clothesByTexture);
            }

            return ClothUtil.IntersectClothLists(clothLists);
        }

        public float GetColorMDLimit()
        {
            return colorMDLimit;
        }

        public void SetColorMDLimit(float colorMDLimit)
        {
            this.colorMDLimit = colorMDLimit;
        }

        public float GetTextureMDLimit()
        {
            return textureMDLimit;
        }

        public void SetTextureMDLimit(float textureMDLimit)
        {
            this.textureMDLimit = textureMDLimit;
        }

        public float GetGaborMDLimit()
        {
            return gaborMDLimit;
        }

        public void SetGaborMDLimit(float gaborMDLimit)
        {
            this.gaborMDLimit = gaborMDLimit;
        }

        public float GetCooccurrenceMDLimit()
        {
            return cooccurrenceMDLimit;
        }

        public void SetCooccurrenceMDLimit(float cooccurrenceMDLimit)
        {
            this.cooccurrenceMDLimit = cooccurrenceMDLimit;
        }

        #endregion

        public IClothDao ClothDao
        {
            set { this.clothDao = value; }
        }
    }
}
