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
            return new RGBSeparateColorSearcher(new PicParam(colorVector), colorMDLimit, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicRGBColor(float[] colorVector)
        {
            return new RGBColorSearcher(new PicParam(colorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicHSVColor(float[] colorVector)
        {
            return new HSVColorSearcher(new PicParam(colorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicHSVAynsColor(float[] colorVector)
        {
            return new HSVAynsColorSearcher(new PicParam(colorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicHLSColor(float[] colorVector)
        {
            return new HLSColorSearcher(new PicParam(colorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicDaubechiesWavelet(float[] textureVector)
        {
            return new DaubechiesWaveletSearcher(new PicParam(textureVector), textureMDLimit, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicGabor(float[] gaborVector)
        {
            return new GaborSearcher(new PicParam(gaborVector), gaborMDLimit, ClothUtil.CalcGaborDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByPicCooccurrence(float[] cooccurrenceVector)
        {
            return new CooccurrenceSearcher(new PicParam(cooccurrenceVector), cooccurrenceMDLimit, ClothUtil.CalcEuclidDistance, clothDao, 200)
                .Search();
        }

        public List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, float[] colorVector)
        {
            return new RGBSeparateColorSearcher(new PicParam(colorVector), colorMDLimit, ClothUtil.CalcEuclidDistance,
                new TextSearcher(new TextParam(words, colors, shapes), clothDao), 200).Search();
        }

        public List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, float[] textureVector)
        {
            return new DaubechiesWaveletSearcher(new PicParam(textureVector), textureMDLimit, ClothUtil.CalcEuclidDistance,
                new TextSearcher(new TextParam(words, colors, shapes), clothDao), 200).Search();
        }

        public List<Cloth> SearchTest(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            //return new RGBColorSearcher(new PicParam(keyCloth.RGBColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100).Search();
            return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector), float.MaxValue, ClothUtil.CalcEuclidDistance,
                new RGBColorSearcher(new PicParam(keyCloth.RGBColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100), 200).Search();
        }

        public List<Cloth> SearchTest2(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            //return new HSVAynsColorSearcher(new PicParam(keyCloth.HSVAynsColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100).Search();
            return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector), float.MaxValue, ClothUtil.CalcEuclidDistance,
                new HSVAynsColorSearcher(new PicParam(keyCloth.HSVAynsColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100), 200).Search();
        }

        public List<Cloth> SearchTest3(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            //return new HSVColorSearcher(new PicParam(keyCloth.HSVColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100).Search();
            return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector), float.MaxValue, ClothUtil.CalcEuclidDistance,
                new HSVColorSearcher(new PicParam(keyCloth.HSVColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100), 200).Search();
        }

        public List<Cloth> SearchTest4(Cloth keyCloth)
        {
            ClothUtil.ExtractFeaturesNecessary(keyCloth, false);
            //return new HLSColorSearcher(new PicParam(keyCloth.HLSColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100).Search();
            return new DaubechiesWaveletSearcher(new PicParam(keyCloth.DaubechiesWaveletVector), float.MaxValue, ClothUtil.CalcEuclidDistance,
                new HLSColorSearcher(new PicParam(keyCloth.HLSColorVector), float.MaxValue, ClothUtil.CalcEuclidDistance, clothDao, 100), 200).Search();
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
