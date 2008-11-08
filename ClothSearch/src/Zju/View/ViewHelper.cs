using System;
using System.Collections.Generic;
using Zju.Util;

namespace Zju.View
{
    public sealed class ViewConstants
    {
        public static KeyValuePair<String, ColorEnum>[] Colors
            = { new KeyValuePair<String, ColorEnum>("黑", ColorEnum.BLACK),
                  new KeyValuePair<String, ColorEnum>("白", ColorEnum.WHITE),
                  new KeyValuePair<String, ColorEnum>("蓝", ColorEnum.BLUE),
                  new KeyValuePair<String, ColorEnum>("红", ColorEnum.RED),
                  new KeyValuePair<String, ColorEnum>("粉红", ColorEnum.PINK),
                  new KeyValuePair<String, ColorEnum>("暗红", ColorEnum.DARKRED)};

        public static KeyValuePair<String, ShapeEnum>[] Shapes
            = { new KeyValuePair<String, ShapeEnum>("条纹", ShapeEnum.STRIPE),
                  new KeyValuePair<String, ShapeEnum>("方格", ShapeEnum.SQUARE),
                  new KeyValuePair<String, ShapeEnum>("圆形", ShapeEnum.CIRCLE),
                  new KeyValuePair<String, ShapeEnum>("三角形", ShapeEnum.TRIANGLE),
                  new KeyValuePair<String, ShapeEnum>("特殊形状", ShapeEnum.SPECIAL) };
    }

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
    }
}
