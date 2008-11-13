using System;
using System.Collections.Generic;
using Zju.Util;
using Zju.Image;
using Zju.Domain;

namespace Zju.View
{
    public class ColorItem
    {
        public String Name { get; set; }
        public ColorEnum Value { get; set; }
        public Boolean Selected { get; set; }
    }

    public class ShapeItem
    {
        public String Name { get; set; }
        public ShapeEnum Value { get; set; }
        public Boolean Selected { get; set; }
    }

    public sealed class ViewHelper
    {
        public static ImageMatcher ImageMatcher = new ImageMatcher();

        public static List<ColorItem> NewColorItems
        {
            get
            {
                List<ColorItem> colorItems = new List<ColorItem>();
                foreach (KeyValuePair<String, ColorEnum> pair in ViewConstants.Colors)
                {
                    colorItems.Add(new ColorItem { Name = pair.Key, Value = pair.Value, Selected = false });
                }

                return colorItems;
            }
            
        }

        public static List<ShapeItem> NewShapeItems
        {
            get
            {
                List<ShapeItem> shapeItems = new List<ShapeItem>();
                foreach (KeyValuePair<String, ShapeEnum> pair in ViewConstants.Shapes)
                {
                    shapeItems.Add(new ShapeItem { Name = pair.Key, Value = pair.Value, Selected = false });
                }

                return shapeItems;
            }

        }

        /// <summary>
        /// Extract color and texture features for the cloth picture.
        /// And save the features back into the <code>cloth</code> objects.
        /// </summary>
        /// <param name="cloth"></param>
        public static void ExtractFeatures(Cloth cloth)
        {
            if (String.IsNullOrEmpty(cloth.Path))
            {
                return;
            }

            int[] colorVector = ImageMatcher.ExtractColorVector(cloth.Path);
            cloth.ColorVector = colorVector;

            float[] textureVector = ImageMatcher.ExtractTextureVector(cloth.Path);
            cloth.TextureVector = textureVector;
        }
    }
}
