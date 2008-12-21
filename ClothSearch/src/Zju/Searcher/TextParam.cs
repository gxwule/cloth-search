using System;
using System.Collections.Generic;
using Zju.Util;

namespace Zju.Search
{
    public class TextParam : BaseParam
    {
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
