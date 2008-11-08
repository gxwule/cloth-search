using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zju.Util
{
    public sealed class DbConstants
    {
        public const String DataBaseFilePath = "E:\\projects\\ClothSearch\\codes\\trunk\\data\\cloth.dbs";
        public const int PagePoolSize = 48 * 1024 * 1024;
        public const int ComitLimit = 100000;
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
