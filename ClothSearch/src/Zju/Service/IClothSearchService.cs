using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Service
{
    public interface IClothSearchService
    {
        List<Cloth> SearchByText(String words, ColorEnum colors, ShapeEnum shapes);

        List<Cloth> SearchByPicRGBSeparateColor(Cloth keyCloth);

        List<Cloth> SearchByPicRGBColor(Cloth keyCloth);

        List<Cloth> SearchByPicHSVColor(Cloth keyCloth);

        List<Cloth> SearchByPicHSVAynsColor(Cloth keyCloth);

        List<Cloth> SearchByPicHLSColor(Cloth keyCloth);

        List<Cloth> SearchByPicDaubechiesWavelet(Cloth keyCloth);

        List<Cloth> SearchByPicGabor(Cloth keyCloth);

        List<Cloth> SearchByPicCooccurrence(Cloth keyCloth);

        List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, Cloth keyCloth);

        List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, Cloth keyCloth);

        List<Cloth> SearchByPicGaborAndHSVAsyColor(Cloth keyCloth);

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
