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
        private static ImageMatcher imageMatcher;

        public static ImageMatcher ImageMatcher
        {
            get
            {
                if (imageMatcher == null)
                {
                    imageMatcher = new ImageMatcher();
                    imageMatcher.luvInit(ViewConstants.LuvFileName);
                }
                return imageMatcher;
            }
        }

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

            if (cloth.ColorVector == null)
            {
                cloth.ColorVector = ImageMatcher.ExtractColorVector(cloth.Path, ViewConstants.IgnoreColors);
            }
            
            if (cloth.TextureVector == null)
            {
                cloth.TextureVector = ImageMatcher.ExtractTextureVector(cloth.Path);
            }
        }

        /// <summary>
        /// Extract pattern string from the picture name. I.e.
        /// C;\a\bcd.jpg -> bcd
        /// </summary>
        /// <param name="picName"></param>
        /// <returns></returns>
        public static string ExtractPattern(string picName)
        {
            if (string.IsNullOrEmpty(picName))
            {
                return null;
            }

            int i = picName.LastIndexOf('.');
            int j = picName.LastIndexOfAny(new char[] {'/', '\\'});

            j = j == -1 ? 0 : j;
            i = i == -1 ? picName.Length : i;

            return i - j - 1 > 0 ? picName.Substring(j + 1, i - j - 1) : null;
        }
    }
}
