using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zju.Util
{
    public sealed class DbConstants
    {
        public const String DataBaseFilePath = "E:/projects/ClothSearch/codes/trunk/data/cloth.dbs";
        public const int PagePoolSize = 48 * 1024 * 1024;
        public const int ComitLimit = 100000;
    }

    public sealed class SearchConstants
    {
        /// <summary>
        /// Limit of Manhattan distance of color vectors between two cloth when searching.
        /// The cloth as searched should less or equal to the limit.
        /// 500, 2000, 6000
        /// </summary>
        public static float[] ColorMDLimits = { float.MaxValue - 1, 686.0f, 1064.0f, 1656.0f, float.MaxValue - 1 };

        public static float[] TextureMDLimits = { float.MaxValue - 1, 80.5f, 132.6f, 220.8f, float.MaxValue - 1 };

        public static float[] GaborMDLimits = { float.MaxValue - 1, 10700.0f, 42500.0f, 135000.0f, float.MaxValue - 1 };

        public static float[] CooccurrenceMDLimits = { float.MaxValue - 1, 0.6f, 0.94f, 1.59f, float.MaxValue - 1 };

        public static int[] IgnoreColors = { -1 };


        public const string LuvFileName = "E:/projects/ClothSearch/codes/trunk/data/luv.dat";
    }

    [Flags]
    public enum ColorEnum
    {
        NONE = 0x0,
        BLACK = 0x1,
        WHITE = 0x2,
        BLUE = 0x4,
        RED = 0x8,
        PINK = 0x10,
        DARKRED = 0x20,
    };

    [Flags]
    public enum ShapeEnum
    {
        NONE = 0x0,
        STRIPE = 0x1,
        SQUARE = 0x2,
        CIRCLE = 0x4, 
        TRIANGLE = 0x8,
        SPECIAL = 0x10,
    };
}
