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

        private static Dictionary<RecallLevel, int> recallLevelToIndexMap;


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
