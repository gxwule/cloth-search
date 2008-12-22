using System;
using System.Collections.Generic;
using System.Text;

namespace Zju.Searcher
{
    public class PicParam : BaseParam
    {
        public PicParam()
        {

        }

        public PicParam(float[] feature, int clothNum)
        {
            this.Feature = feature;
            this.ClothNum = clothNum;
        }

        public float[] Feature
        {
            get;
            set;
        }

        public int ClothNum
        {
            get;
            set;
        }
    }
}
