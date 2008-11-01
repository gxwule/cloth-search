using System;
using Perst;
using Zju.Dao;

namespace Zju.Domain
{
    /// <summary>
    /// Shape of the cloth: stripe, square, circle, triangle, special
    /// </summary>
    public class Shape : Persistent
    {
        /// <summary>
        /// Name of the shape.
        /// </summary>
        private String name;

        /// <summary>
        /// Name of the shape.
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Shape()
        {

        }
/*
        public Shape(String name)
        {
            assignFileds(name);
        }
*/
        public Shape(Storage storage, String name) : base(storage)
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
