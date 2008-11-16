using System.Collections.Generic;
using Perst;
using Zju.Domain;
using Zju.Util;

namespace Zju.Dao
{
    public class ClothDao : IClothDao
    {
        /// <summary>
        /// Save or update the cloth into the database if not exit, otherwise update it.
        /// Whether a cloth exists is decided by the Oid of the cloth.
        /// </summary>
        public void SaveOrUpdate(Cloth cloth)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            saveOrUpdateWithoutCommit(storage, root, cloth);
            
            storage.Commit();
        }

        public void SaveOrUpdateAll(List<Cloth> clothes)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            int nCloth = 0;
            foreach (Cloth cloth in clothes)
            {
                saveOrUpdateWithoutCommit(storage, root, cloth);

                if (0 == ++nCloth % DbConstants.ComitLimit)
                {
                    storage.Commit();
                }
            }
            
            storage.Commit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of a cloth.</param>
        public void Delete(int oid)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            Cloth cloth = (Cloth)root.ClothOidIndex.Get(oid);
            if (null != cloth)
            {
                // Console.WriteLine("delete");
                root.ColorIndex.Remove(cloth);
                root.ShapeIndex.Remove(cloth);
                root.ClothOidIndex.Remove(cloth);
                root.PatternIndex.Remove(cloth);

                storage.Commit();
            }
        }

        /// <summary>
        /// Find the Cloth object by cloth name.
        /// </summary>
        /// <param name="oid">Oid of a cloth.</param>
        /// <returns>Cloth with the cloth name; null if not found.</returns>
        public Cloth FindByOid(int oid)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            return (Cloth)root.ClothOidIndex.Get(oid);
        }

        /// <summary>
        /// Find all Cloth objects.
        /// </summary>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        public List<Cloth> FindAll()
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            List<Cloth> clothes = new List<Cloth>();
            foreach (Cloth cloth in root.ClothOidIndex)
            {
                clothes.Add(cloth);
            }
            return clothes;
        }

        public List<Cloth> FindAllByPattern(string pattern)
        {
            List<Cloth> clothes = new List<Cloth>();

            if (!string.IsNullOrEmpty(pattern))
            {
                Storage storage = DaoHelper.Instance.DbStorage;
                ClothRoot root = (ClothRoot)storage.Root;

                foreach (Cloth cloth in root.PatternIndex.GetPrefix(pattern))
                {
                    clothes.Add(cloth);
                }
            }
            
            return clothes;
        }

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
        public List<Cloth> FindAllByColors(ColorEnum colors, ColorEnum notColors)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            List<Cloth> clothes = new List<Cloth>();
            BitIndex colorIndex = root.ColorIndex;
            // Console.WriteLine(colorIndex.Count);
            if (colorIndex.Count > 0)
            {
                foreach (Cloth cloth in colorIndex.Select((int)colors, (int)notColors))
                {
                    clothes.Add(cloth);
                }
            }
            
            return clothes;
        }

        /// <summary>
        /// Find list of Cloth object by colors.
        /// NOTES: The clothes to be selected should contains all colors in <code>colors</code>.
        /// </summary>
        /// <param name="colors">The colors contained by clothes.</param>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        public List<Cloth> FindAllByColors(ColorEnum colors)
        {
            return FindAllByColors(colors, 0);
        }

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
        public List<Cloth> FindAllByShapes(ShapeEnum shapes, ShapeEnum notShapes)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            List<Cloth> clothes = new List<Cloth>();
            BitIndex shapeIndex = root.ShapeIndex;
            if (shapeIndex.Count > 0)
            {
                foreach (Cloth cloth in shapeIndex.Select((int)shapes, (int)notShapes))
                {
                    clothes.Add(cloth);
                }
            }
            return clothes;
        }

        /// <summary>
        /// Find list of Cloth object by shapes.
        /// NOTES: The clothes to be selected should contains all shapes in <code>shapes</code>.
        /// <param name="shapes">The shapes contained by clothes.</param>
        /// <returns>Cloth objects list; empty list if none found.</returns>
        public List<Cloth> FindAllByShapes(ShapeEnum shapes)
        {
            return FindAllByShapes(shapes, 0);
        }
/*
        public List<Cloth> FindAllByColorsAndShapes(ColorEnum colors, ColorEnum notColors, ShapeEnum shapes, ShapeEnum notShapes)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            BitIndex<Cloth> colorIndex = root.ColorIndex;
            BitIndex<Cloth> shapeIndex = root.ShapeIndex;

            Query<Cloth> query = storage.CreateQuery<Cloth>();
            query.Prepare("(colors and ? = ?) and (colors and ?) = 0 and (shapes and ? = ?) and (shapes and ?) = 0");
            query.AddIndex(colorIndex);
            query.AddIndex(shapeIndex);

        }*/

        private void saveOrUpdateWithoutCommit(Storage storage, ClothRoot root, Cloth cloth)
        {
            FieldIndex clothOidIndex = root.ClothOidIndex;
            FieldIndex patternIndex = root.PatternIndex;
            BitIndex colorIndex = root.ColorIndex;
            BitIndex shapeIndex = root.ShapeIndex;
            if (clothOidIndex.Contains(cloth))
            {
                colorIndex.Remove(cloth);
                shapeIndex.Remove(cloth);
                patternIndex.Remove(cloth);
            }
            else
            {
                // this method called for generate the Oid, or the key of ClothOidIndex will always be 0.
                storage.MakePersistent(cloth);
                clothOidIndex.Set(cloth);
            }
            colorIndex[cloth] = (int)cloth.Colors;
            shapeIndex[cloth] = (int)cloth.Shapes;
            patternIndex.Put(cloth);
        }
    }
}
