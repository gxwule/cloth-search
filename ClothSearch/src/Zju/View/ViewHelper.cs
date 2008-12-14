using System;
using System.Collections.Generic;
using Zju.Util;
using Zju.Image;
using Zju.Domain;
using System.Windows.Media.Imaging;

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

        private static Dictionary<RecallLevel, int> recallLevelToIndexMap;

        public static ImageMatcher ImageMatcher
        {
            get
            {
                if (imageMatcher == null)
                {
                    imageMatcher = new ImageMatcher();
                    imageMatcher.LuvInit(ViewConstants.LuvFileName);
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
        /// 
        /// </summary>
        /// <param name="picName"></param>
        /// <returns>null if any errors.</returns>
        public static BitmapImage NewBitmapImage(string picName)
        {
            BitmapImage bi = null;
            try
            {
                bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(picName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
            }
            catch (System.Exception e)
            {
                // maybe log something here.
                bi = null;
            }

            return bi;
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
                //ClothUtil.Log.WriteLine("begin ExtractColorVector");
                cloth.ColorVector = ImageMatcher.ExtractColorVector(cloth.Path, ViewConstants.IgnoreColors);
                //ClothUtil.Log.WriteLine("end ExtractColorVector");
            }
            
            if (cloth.TextureVector == null)
            {
                //ClothUtil.Log.WriteLine("begin ExtractTextureVector");
                cloth.TextureVector = ImageMatcher.ExtractTextureVector(cloth.Path);
                //ClothUtil.Log.WriteLine("end ExtractTextureVector");
            }

            if (cloth.GaborVector == null)
            {
                //ClothUtil.Log.WriteLine("begin ExtractGaborVector");
                cloth.GaborVector = ImageMatcher.ExtractGaborVector(cloth.Path);
                //ClothUtil.Log.WriteLine("end ExtractGaborVector");
            }

            if (cloth.CooccurrenceVector == null)
            {
                //ClothUtil.Log.WriteLine("begin ExtractCooccurrenceVector");
                cloth.CooccurrenceVector = ImageMatcher.ExtractCooccurrenceVector(cloth.Path);
                //ClothUtil.Log.WriteLine("end ExtractCooccurrenceVector");
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

        public static int RecallLevelToIndex(RecallLevel rLevel)
        {
            if (recallLevelToIndexMap == null)
            {
                recallLevelToIndexMap = new Dictionary<RecallLevel,int>();
                recallLevelToIndexMap[RecallLevel.Default] = 0;
                recallLevelToIndexMap[RecallLevel.Recall1] = 1;
                recallLevelToIndexMap[RecallLevel.Recall2] = 2;
                recallLevelToIndexMap[RecallLevel.Recall3] = 3;
            }

            int index = 0;
            if (recallLevelToIndexMap.ContainsKey(rLevel))
            {
                index = recallLevelToIndexMap[rLevel];
            }

            return index;
        }
    }
}
