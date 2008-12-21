using System;
using System.Collections.Generic;
using Zju.Util;
using Zju.Domain;
using Zju.Dao;
using Zju.Searcher;

namespace Zju.Service
{
    public class ClothSearchService : IClothSearchService
    {
        private IClothDao clothDao;

        private float colorMDLimit;

        private float textureMDLimit;

        private float gaborMDLimit;

        private float cooccurrenceMDLimit;

        public ClothSearchService()
        {
            colorMDLimit = SearchConstants.ColorMDLimits[0];
            textureMDLimit = SearchConstants.TextureMDLimits[0];
            clothDao = new ClothDao();
        }

        #region IClothSearchService Members

        public List<Cloth> SearchByText(string words, ColorEnum colors, ShapeEnum shapes)
        {
            return new TextSearcher(new TextParam(words, colors, shapes), clothDao).Search();
        }

        public List<Cloth> SearchByPicRGBSeparateColor(float[] colorVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(colorVector), colorMDLimit, ClothUtil.CalcManhattanDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicDaubechiesWavelet(float[] textureVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(textureVector), textureMDLimit, ClothUtil.CalcManhattanDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicGabor(float[] gaborVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(gaborVector), gaborMDLimit, ClothUtil.CalcGaborDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicCooccurrence(float[] cooccurrenceVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(cooccurrenceVector), cooccurrenceMDLimit, ClothUtil.CalcManhattanDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, float[] colorVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(colorVector), colorMDLimit, ClothUtil.CalcManhattanDistance,
                new TextSearcher(new TextParam(words, colors, shapes), clothDao), 200).Search();
        }

        public List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, float[] textureVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(textureVector), textureMDLimit, ClothUtil.CalcManhattanDistance,
                new TextSearcher(new TextParam(words, colors, shapes), clothDao), 200).Search();
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
