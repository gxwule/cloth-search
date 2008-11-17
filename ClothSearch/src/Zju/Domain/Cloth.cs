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

        /// <summary>
        /// Update time of the cloth.
        /// </summary>
        private DateTime updateTime;

        private int[] colorVector;

        private float[] textureVector;
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

        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = value; }
        }

        public int[] ColorVector
        {
            get { return colorVector; }
            set { colorVector = value; }
        }

        public float[] TextureVector
        {
            get { return textureVector; }
            set { textureVector = value; }
        }
#endregion
        
        public Cloth()
        {
            colors = ColorEnum.NONE;
            shapes = ShapeEnum.NONE;
        }

        public Cloth(Cloth cloth)
        {
            this.name = cloth.Name;
            this.pattern = cloth.Pattern;
            this.path = cloth.Path;
            this.colors = cloth.Colors;
            this.shapes = cloth.Shapes;
            this.colorVector = cloth.ColorVector;
            this.textureVector = cloth.TextureVector;
            this.updateTime = cloth.UpdateTime;
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
            this.updateTime = DateTime.UtcNow;
        }

    } 
}
