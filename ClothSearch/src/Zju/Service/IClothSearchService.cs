using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Service
{
    public interface IClothSearchService
    {
        List<Cloth> SearchByText(String words, ColorEnum colors, ShapeEnum shapes);

        List<Cloth> SearchByPicColor(int[] colorVector);

        List<Cloth> SearchByPicTexture(float[] textureVector);

        List<Cloth> SearchByTextAndPicColor(String words, ColorEnum colors, ShapeEnum shapes, int[] colorVector);

        List<Cloth> SearchByTextAndPicTexture(String words, ColorEnum colors, ShapeEnum shapes, float[] textureVector);

        float GetColorMDLimit();
        void SetColorMDLimit(float colorMDLimit);

        float GetTextureMDLimit();
        void SetTextureMDLimit(float textureMDLimit);
    }
}
