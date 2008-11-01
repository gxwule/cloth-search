using System;
using Perst;

namespace Zju.Domain
{
    class ClothRoot : Persistent
    {
        /// <summary>
        /// Color index to find Cloth objects.
        /// </summary>
        private Index<String, Cloth> colorName;

        /// <summary>
        /// Shape index to find Cloth objects.
        /// </summary>
        private Index<String, Cloth> shapeName;
        
        /// <summary>
        /// Unique cloth name to identify a cloth.
        /// </summary>
        private FieldIndex<String, Cloth> clothName;


        public Index ColorName
        {
            get { return colorName; }
            set { colorName = value; }
        }

        public Index ShapeName
        {
            get { return shapeName; }
            set { shapeName = value; }
        }

        public FieldIndex ClothName
        {
            get { return clothName; }
            set { clothName = value; }
        }

        public ClothRoot()
        {

        }

        public ClothRoot(Storage storage) : base(storage)
        {
            colorName = storage.CreateIndex<String, Cloth>(false);
            shapeName = storage.CreateIndex<String, Cloth>(false);
            clothName = storage.CreateFieldIndex<String, Cloth>("name", true);
        }
    }
}
