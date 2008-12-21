using System;
using System.Collections.Generic;
using Zju.Util;

namespace Zju.Searcher
{
    public class TextParam : BaseParam
    {
        public TextParam()
        {

        }

        public TextParam(string words, ColorEnum colors, ShapeEnum shapes)
        {
            this.Words = words;
            this.Colors = colors;
            this.Shapes = shapes;
        }

        public string Words
        {
            get;
            set;
        }

        public ColorEnum Colors
        {
            get;
            set;
        }

        public ShapeEnum Shapes
        {
            get;
            set;
        }
    }
}
