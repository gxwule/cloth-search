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
        /// </summary>
        public static float[] ColorMDLimits = {600.0f, 800.0f, 1000.0f, 1500.0f, float.MaxValue - 1};

        public static float[] TextureMDLimits = { 70.0f, 120.0f, 180.0f, 300.0f, float.MaxValue - 1 };

        public static float[] GaborMDLimits = { 10000.0f, 30000.0f, 80000.0f, 300000.0f, float.MaxValue - 1 };

        public static float[] CooccurrenceMDLimits = { 0.5f, 0.9f, 1.2f, 2.0f, float.MaxValue - 1 };
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
