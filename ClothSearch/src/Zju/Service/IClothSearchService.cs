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

        List<Cloth> SearchByPicDaubechiesWavelet(float[] textureVector);

        List<Cloth> SearchByPicGabor(float[] gaborVector);

        List<Cloth> SearchByPicCooccurrence(float[] cooccurrenceVector);

        List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, float[] colorVector);

        List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, float[] textureVector);

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
