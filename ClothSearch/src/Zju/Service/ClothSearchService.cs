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

        public List<Cloth> SearchByPicRGBSeparateColor(Cloth keyCloth)
        {
            return new RGBSeparateColorSearcher(new PicParam(keyCloth.RGBSeparateColorVector, keyCloth.ColorNum), colorMDLimit, ClothUtil.CalcManhattanDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicRGBColor(Cloth keyCloth)
        {
            return new RGBColorSearcher(new PicParam(keyCloth.RGBColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 56)
                .Search();
        }

        public List<Cloth> SearchByPicHSVColor(Cloth keyCloth)
        {
            return new HSVColorSearcher(new PicParam(keyCloth.HSVColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 56)
                .Search();
        }

        public List<Cloth> SearchByPicHSVAynsColor(Cloth keyCloth)
        {
            return new HSVAynsColorSearcher(new PicParam(keyCloth.HSVAynsColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 56)
                .Search();
        }

        public List<Cloth> SearchByPicHLSColor(Cloth keyCloth)
        {
            return new HLSColorSearcher(new PicParam(keyCloth.HLSColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicDaubechiesWavelet(Cloth keyCloth)
        {
            return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance,
                new HSVColorSearcher(new PicParam(keyCloth.HSVColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 200), 56)
                .Search();
        }

        public List<Cloth> SearchByPicGabor(Cloth keyCloth)
        {
            return new GaborSearcher(new PicParam(keyCloth.GaborVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcGaborDistance, clothDao, 56)
                .Search();
        }

        public List<Cloth> SearchByPicGaborAndHSVAsyColor(Cloth keyCloth)
        {
            return new GaborSearcher(new PicParam(keyCloth.GaborVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance,
                new HSVAynsColorSearcher(new PicParam(keyCloth.HSVAynsColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 200), 56).Search();
        }

        public List<Cloth> SearchByPicCooccurrence(Cloth keyCloth)
        {
            return new CooccurrenceSearcher(new PicParam(keyCloth.CooccurrenceVector, keyCloth.ColorNum), cooccurrenceMDLimit, ClothUtil.CalcManhattanDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, Cloth keyCloth)
        {
            return new RGBColorSearcher(new PicParam(keyCloth.RGBColorVector, keyCloth.ColorNum), colorMDLimit, ClothUtil.CalcManhattanDistance,
                new TextSearcher(new TextParam(words, colors, shapes), clothDao), 200).Search();
        }

        public List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, Cloth keyCloth)
        {
            return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector, keyCloth.ColorNum), textureMDLimit, ClothUtil.CalcManhattanDistance,
                new TextSearcher(new TextParam(words, colors, shapes), clothDao), 200).Search();
        }

        public List<Cloth> SearchTest(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            return new RGBColorSearcher(new PicParam(keyCloth.RGBColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100).Search();
            //return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance,
              //  new RGBColorSearcher(new PicParam(keyCloth.RGBColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100), 200).Search();
        }

        public List<Cloth> SearchTest2(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            return new HSVAynsColorSearcher(new PicParam(keyCloth.HSVAynsColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100).Search();
            //return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance,
              //  new HSVAynsColorSearcher(new PicParam(keyCloth.HSVAynsColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100), 200).Search();
        }

        public List<Cloth> SearchTest3(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            return new HSVColorSearcher(new PicParam(keyCloth.HSVColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100).Search();
            //return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance,
              //  new HSVColorSearcher(new PicParam(keyCloth.HSVColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100), 200).Search();
        }

        public List<Cloth> SearchTest4(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            return new HLSColorSearcher(new PicParam(keyCloth.HLSColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100).Search();
            //return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance,
              //  new HLSColorSearcher(new PicParam(keyCloth.HLSColorVector, keyCloth.ColorNum), float.MaxValue, ClothUtil.CalcManhattanDistance, clothDao, 100), 200).Search();
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
