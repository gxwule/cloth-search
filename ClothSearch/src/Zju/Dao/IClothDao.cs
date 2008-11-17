using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Util;

namespace Zju.Dao
{
    public interface IClothDao
    {
        /// <summary>
        /// Save or update the cloth into the database if not exit, otherwise update it.
        /// Whether a cloth exists is decided by the Oid of the cloth.
        /// NOTES: The <code>cloth</code> should NOT be null.
        /// </summary>
        void Insert(Cloth cloth);

        /// <summary>
        /// NOTES: The <code>clothes</code> should NOT be null.
        /// </summary>
        /// <param name="clothes"></param>
        void InsertAll(List<Cloth> clothes);

        void Update(Cloth oldCloth, Cloth newCloth);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of a cloth.</param>
        void Delete(int oid);

        void Delete(Cloth cloth);

        /// <summary>
        /// Find the Cloth object by cloth name.
        /// </summary>
        /// <param name="oid">Oid of a cloth.</param>
        /// <returns>Cloth with the cloth name; null if not found.</returns>
        Cloth FindByOid(int oid);

        /// <summary>
        /// Find all Cloth objects.
        /// </summary>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        List<Cloth> FindAll();

        /// <summary>
        /// Find list of Cloth objects by the prefix of pattern.
        /// </summary>
        /// <param name="pattern">The prefix of pattern.</param>
        /// <returns></returns>
        List<Cloth> FindAllByPattern(string pattern);

        /// <summary>
        /// Find list of Cloth object by colors.
        /// NOTES: There are two rules for clothes to be selected:
        /// <list type="number">
        /// <item>It should contains all colors in <code>colors</code>.</item>
        /// <item>It should not contains any colors in <code>notColors</code>.</item>
        /// </list>
        /// </summary>
        /// <param name="colors">The colors contained by clothes.</param>
        /// <param name="notColors">The colors NOT contained by clothes.</param>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        List<Cloth> FindAllByColors(ColorEnum colors, ColorEnum notColors);

        /// <summary>
        /// Find list of Cloth object by colors.
        /// NOTES: The clothes to be selected should contains all colors in <code>colors</code>.
        /// </summary>
        /// <param name="colors">The colors contained by clothes.</param>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        List<Cloth> FindAllByColors(ColorEnum colors);

        /// <summary>
        /// Find list of Cloth object by shapes.
        /// NOTES: There are two rules for clothes to be selected:
        /// <list type="number">
        /// <item>It should contains all shapes in <code>shapes</code>.</item>
        /// <item>It should not contains any shapes in <code>notShapes</code>.</item>
        /// </list>
        /// </summary>
        /// <param name="shapes">The shapes contained by clothes.</param>
        /// <param name="notShapes">The shapes NOT contained by clothes.</param>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        List<Cloth> FindAllByShapes(ShapeEnum shapes, ShapeEnum notShapes);

        /// <summary>
        /// Find list of Cloth object by shapes.
        /// NOTES: The clothes to be selected should contains all shapes in <code>shapes</code>.
        /// <param name="shapes">The shapes contained by clothes.</param>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        List<Cloth> FindAllByShapes(ShapeEnum shapes);

        /*
        public List<Cloth> FindAllByColorsAndShapes(ColorEnum colors, ColorEnum notColors, ShapeEnum shapes, ShapeEnum notShapes);
        */
    }
}
