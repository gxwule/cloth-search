using System;
using Perst;

namespace Zju.Domain
{
    public class ClothRoot : Persistent
    {
        /// <summary>
        /// Color index to find Cloth objects.
        /// </summary>
        private BitIndex colorIndex;

        /// <summary>
        /// Shape index to find Cloth objects.
        /// </summary>
        private BitIndex shapeIndex;
        
        /// <summary>
        /// Unique cloth Oid to identify a cloth.
        /// </summary>
        private FieldIndex clothOidIndex;

        private FieldIndex patternIndex;

        private FieldIndex pathIndex;

        private FieldIndex colorNumIndex;

        public BitIndex ColorIndex
        {
            get { return colorIndex; }
        }

        public BitIndex ShapeIndex
        {
            get { return shapeIndex; }
        }

        public FieldIndex ClothOidIndex
        {
            get { return clothOidIndex; }
        }

        public FieldIndex PatternIndex
        {
            get { return patternIndex; }
        }

        public FieldIndex PathIndex
        {
            get { return pathIndex; }
            set { pathIndex = value; }
        }

        public FieldIndex ColorNumIndex
        {
            get { return colorNumIndex; }
            set { colorNumIndex = value; }
        }

        public ClothRoot()
        {

        }

        public ClothRoot(Storage storage) : base(storage)
        {
            colorIndex = storage.CreateBitIndex();
            shapeIndex = storage.CreateBitIndex();
            clothOidIndex = storage.CreateFieldIndex(typeof(Cloth), "Oid", true);
            patternIndex = storage.CreateFieldIndex(typeof(Cloth), "Pattern", false);
            pathIndex = storage.CreateFieldIndex(typeof(Cloth), "Path", true);
            colorNumIndex = storage.CreateFieldIndex(typeof(Cloth), "ColorNum", false);
        }
    }
}
