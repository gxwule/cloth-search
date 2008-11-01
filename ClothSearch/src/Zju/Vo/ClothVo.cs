using System;
using System.Collections.Generic;

namespace Zju.Vo
{
    public class ClothVo
    {
#region field declaration
        /// <summary>
        /// Name of the cloth.
        /// </summary>
        private String name;

        /// <summary>
        /// Pattern of the cloth, maybe the file name.
        /// </summary>
        private String pattern;

        /// <summary>
        /// Path of the image in the system, absolute or relative.
        /// </summary>
        private String path;

        /// <summary>
        /// Names of colors of the cloth.
        /// </summary>
        private HashSet<String> colorNames;

        /// <summary>
        /// Names of shapes of the cloth.
        /// </summary>
        private HashSet<String> shapeNames;
#endregion

#region getter & setter
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public String Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }

        public String Path
        {
            get { return path; }
            set { path = value; }
        }

        public HashSet<String> ColorNames
        {
            get { return colorNames; }
            set { colorNames = value; }
        }

        public HashSet<String> ShapeNames
        {
            get { return shapeNames; }
            set { shapeNames = value; }
        }
#endregion
        
        public ClothVo()
        {

        }

        public ClothVo(String name, String pattern, String path, HashSet<String> colorNames, HashSet<String> shapeNames)
        {
            this.name = name;
            this.pattern = pattern;
            this.path = path;
            this.colorNames = colorNames;
            this.shapeNames = shapeNames;
        }
    }
}
