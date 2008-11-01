using System;
using Perst;

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
        private Link colors;

        /// <summary>
        /// Shapes description of the cloth.
        /// </summary>
        private Link shapes;
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

        public Link Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        public Link Shapes
        {
            get { return shapes; }
            set { shapes = value; }
        }
#endregion
        
        public Cloth()
        {

        }
/*
        public Cloth(String name, String pattern, String path)
        {
            assignFileds(name, pattern, path);
        }
*/
        public Cloth(Storage storage, String name, String pattern, String path) : base(storage)
        {
            this.name = name;
            this.pattern = pattern;
            this.path = path;
            colors = storage.CreateLink();
            shapes = storage.CreateLink();
        }
/*
        private void assignFileds(String name, String pattern, String path)
        {
            this.name = name;
            this.pattern = pattern;
            this.path = path;
        }
*/
    } 
}
