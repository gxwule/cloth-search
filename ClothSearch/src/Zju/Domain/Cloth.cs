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

        private float[] rGBSeparateColorVector;

        private float[] rGBColorVector;

        private float[] hSVColorVector;

        private float[] hSVAynsColorVector;

        private float[] hLSColorVector;

        private float[] daubechiesWaveletVector;

        private float[] gaborVector;

        private float[] cooccurrenceVector;

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

        public float[] RGBSeparateColorVector
        {
            get { return rGBSeparateColorVector; }
            set { rGBSeparateColorVector = value; }
        }

        public float[] RGBColorVector
        {
            get { return rGBColorVector; }
            set { rGBColorVector = value; }
        }

        public float[] HSVColorVector
        {
            get { return hSVColorVector; }
            set { hSVColorVector = value; }
        }

        public float[] HSVAynsColorVector
        {
            get { return hSVAynsColorVector; }
            set { hSVAynsColorVector = value; }
        }

        public float[] HLSColorVector
        {
            get { return hLSColorVector; }
            set { hLSColorVector = value; }
        }

        public float[] DaubechiesWaveletVector
        {
            get { return daubechiesWaveletVector; }
            set { daubechiesWaveletVector = value; }
        }

        public float[] GaborVector
        {
            get { return gaborVector; }
            set { gaborVector = value; }
        }

        public float[] CooccurrenceVector
        {
            get { return cooccurrenceVector; }
            set { cooccurrenceVector = value; }
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
            this.rGBSeparateColorVector = cloth.RGBSeparateColorVector;
            this.rGBColorVector = cloth.RGBColorVector;
            this.hSVColorVector = cloth.HSVColorVector;
            this.hSVAynsColorVector = cloth.HSVAynsColorVector;
            this.hLSColorVector = cloth.HLSColorVector;
            this.daubechiesWaveletVector = cloth.DaubechiesWaveletVector;
            this.gaborVector = cloth.GaborVector;
            this.cooccurrenceVector = cloth.CooccurrenceVector;
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
