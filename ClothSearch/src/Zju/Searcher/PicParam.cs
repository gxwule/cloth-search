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

        public PicParam(float[] feature)
        {
            this.Feature = feature;
        }

        public float[] Feature
        {
            get;
            set;
        }
    }
}
