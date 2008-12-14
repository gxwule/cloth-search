using System.Collections.Generic;
using Perst;
using Zju.Domain;
using Zju.Util;
using System;

namespace Zju.Dao
{
    public class ClothDao : IClothDao
    {
        /// <summary>
        /// Save or update the cloth into the database if not exit, otherwise update it.
        /// Whether a cloth exists is decided by the Oid of the cloth.
        /// </summary>
        public void Insert(Cloth cloth)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            //storage.BeginThreadTransaction(Storage.EXCLU)
            ClothRoot root = (ClothRoot)storage.Root;

            insertWithoutCommit(storage, root, cloth);
            
            storage.Commit();
        }

        public void InsertAll(List<Cloth> clothes)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            int nCloth = 0;
            foreach (Cloth cloth in clothes)
            {
                insertWithoutCommit(storage, root, cloth);

                if (0 == ++nCloth % DbConstants.ComitLimit)
                {
                    storage.Commit();
                }
            }
            
            storage.Commit();
        }

        public void Update(Cloth cloth, Cloth newCloth)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            FieldIndex clothOidIndex = root.ClothOidIndex;
            
            if (null == cloth || !clothOidIndex.Contains(cloth))
            {
                return;
            }

            FieldIndex patternIndex = root.PatternIndex;
            BitIndex colorIndex = root.ColorIndex;
            BitIndex shapeIndex = root.ShapeIndex;

            if (cloth.Pattern != newCloth.Pattern)
            {
                patternIndex.Remove(cloth);
                cloth.Pattern = newCloth.Pattern;
                if (cloth.Pattern != null)
                {
                    patternIndex.Put(cloth);
                }
            }

            if (cloth.Colors != newCloth.Colors)
            {
                colorIndex.Remove(cloth);
                cloth.Colors = newCloth.Colors;
                colorIndex[cloth] = (int)cloth.Colors;
            }

            if (cloth.Shapes != newCloth.Shapes)
            {
                shapeIndex.Remove(cloth);
                cloth.Shapes = newCloth.Shapes;
                shapeIndex[cloth] = (int)cloth.Shapes;
            }

            if (cloth.Name != newCloth.Name)
            {
                cloth.Name = newCloth.Name;
            }
            
            if (newCloth.Path != null && cloth.Path != newCloth.Path)
            {
                cloth.Path = newCloth.Path;
            }
            
            if (newCloth.ColorVector != null && cloth.ColorVector != newCloth.ColorVector)
            {
                cloth.ColorVector = newCloth.ColorVector;
            }

            if (newCloth.TextureVector != null && cloth.TextureVector != newCloth.TextureVector)
            {
                cloth.TextureVector = newCloth.TextureVector;
            }

            if (newCloth.GaborVector != null && cloth.GaborVector != newCloth.GaborVector)
            {
                cloth.GaborVector = newCloth.GaborVector;
            }

            if (newCloth.CooccurrenceVector != null && cloth.CooccurrenceVector != newCloth.CooccurrenceVector)
            {
                cloth.CooccurrenceVector = newCloth.CooccurrenceVector;
            }

            cloth.UpdateTime = (0 == newCloth.UpdateTime.Ticks) ? DateTime.UtcNow : newCloth.UpdateTime;

            cloth.Modify();

            storage.Commit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of a cloth.</param>
        public void Delete(int oid)
        {
            if (oid > 0)
            {
                Storage storage = DaoHelper.Instance.DbStorage;
                ClothRoot root = (ClothRoot)storage.Root;

                Cloth cloth = (Cloth)root.ClothOidIndex.Get(oid);
                if (null != cloth)
                {
                    removeFromIndexes(cloth, root);

                    storage.Commit();
                }
            }
            
        }

        public void Delete(Cloth cloth)
        {
            if (null != cloth && cloth.Oid > 0)
            {
                Storage storage = DaoHelper.Instance.DbStorage;
                ClothRoot root = (ClothRoot)storage.Root;

                removeFromIndexes(cloth, root);

                storage.Commit();
            }
        }

        private void removeFromIndexes(Cloth cloth, ClothRoot root)
        {
            root.ColorIndex.Remove(cloth);
            root.ShapeIndex.Remove(cloth);
            root.ClothOidIndex.Remove(cloth);
            root.PatternIndex.Remove(cloth);
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

        private void insertWithoutCommit(Storage storage, ClothRoot root, Cloth cloth)
        {
            FieldIndex clothOidIndex = root.ClothOidIndex;
            if (cloth == null || clothOidIndex.Contains(cloth))
            {
                return;
            }

            // this method called for generate the Oid, or the key of ClothOidIndex will always be 0.
            storage.MakePersistent(cloth);
            clothOidIndex.Put(cloth);
            if (cloth.Pattern != null)
            {
                root.PatternIndex.Put(cloth);
            }
            
            root.ColorIndex[cloth] = (int)cloth.Colors;
            root.ShapeIndex[cloth] = (int)cloth.Shapes;
        }
    }
}
