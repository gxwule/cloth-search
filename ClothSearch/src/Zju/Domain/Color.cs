using System;
using Perst;

namespace Zju.Domain
{
    /// <summary>
    /// Color of the cloth: black, white, blue, red, pink, darkred.
    /// </summary>
    public class Color : Persistent
    {
        /// <summary>
        /// Name of the color.
        /// </summary>
        private String name;

        /// <summary>
        /// Name of the color.
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Color()
        {

        }
/*
        public Color(String name)
        {
            assignFileds(name);
        }
*/
        public Color(Storage storage, String name) : base(storage)
        {
            this.name = name;
        }
/*
        private void assignFileds(String name)
        {
            this.name = name;
        }
*/
    }
}
