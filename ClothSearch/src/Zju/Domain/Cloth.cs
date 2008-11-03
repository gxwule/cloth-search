using System;
using Perst;
using Zju.Util;

namespace Zju.Domain
{
    /// <summary>
    /// Cloth information.
    /// </summary>
    public class Cloth : Persistent
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
        /// Colors description of the cloth.
        /// </summary>
        private ColorEnum colors;

        /// <summary>
        /// Shapes description of the cloth.
        /// </summary>
        private ShapeEnum shapes;
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

        public ColorEnum Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        public ShapeEnum Shapes
        {
            get { return shapes; }
            set { shapes = value; }
        }
#endregion
        
        public Cloth()
        {

        }

        public Cloth(String name, String pattern, String path, ColorEnum colors, ShapeEnum shapes)
        {
            assignFileds(name, pattern, path, colors, shapes);
        }

        public Cloth(Storage storage, String name, String pattern, String path, ColorEnum colors, ShapeEnum shapes) : base(storage)
        {
            assignFileds(name, pattern, path, colors, shapes);
        }

        private void assignFileds(String name, String pattern, String path, ColorEnum colors, ShapeEnum shapes)
        {
            this.name = name;
            this.pattern = pattern;
            this.path = path;
            this.colors = colors;
            this.shapes = shapes;
        }

    } 
}
