using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Service
{
    public interface IClothSearchService
    {
        List<Cloth> SearchByText(String words, ColorEnum colors, ShapeEnum shapes);

        List<Cloth> SearchByPicRGBSeparateColor(float[] colorVector);

        List<Cloth> SearchByPicRGBColor(float[] colorVector);

        List<Cloth> SearchByPicHSVColor(float[] colorVector);

        List<Cloth> SearchByPicHSVAynsColor(float[] colorVector);

        List<Cloth> SearchByPicHLSColor(float[] colorVector);

        List<Cloth> SearchByPicDaubechiesWavelet(float[] textureVector);

        List<Cloth> SearchByPicGabor(float[] gaborVector);

        List<Cloth> SearchByPicCooccurrence(float[] cooccurrenceVector);

        List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, float[] colorVector);

        List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, float[] textureVector);

        List<Cloth> SearchTest(Cloth keyCloth);

        List<Cloth> SearchTest2(Cloth keyCloth);

        List<Cloth> SearchTest3(Cloth keyCloth);

        List<Cloth> SearchTest4(Cloth keyCloth);

        float GetColorMDLimit();
        void SetColorMDLimit(float colorMDLimit);

        float GetTextureMDLimit();
        void SetTextureMDLimit(float textureMDLimit);

        float GetGaborMDLimit();
        void SetGaborMDLimit(float gaborMDLimit);

        float GetCooccurrenceMDLimit();
        void SetCooccurrenceMDLimit(float cooccurrenceMDLimit);
    }
}
