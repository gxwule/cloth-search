using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zju.Util
{
    public sealed class Constants
    {
        public const String DataBaseFilePath = "DbCloth";
        public const int PagePoolSize = 32 * 1024 * 1024;
        public const int ComitLimit = 100000;

        public static readonly String[] ColorNames = { "black", "white", "blue", "red", "pink", "darkred" };
        public static readonly String[] ShapeNames = { "stripe", "square", "circle", "triangle", "special" };
    }
}
