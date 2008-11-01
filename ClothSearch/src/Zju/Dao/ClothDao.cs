using System;
using System.Collections.Generic;
using Perst;
using Zju.Domain;
using Zju.Vo;
using Zju.Util;

namespace Zju.Dao
{
    public class ClothDao
    {
        public void Insert(ClothVo clothVo)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            InsertWithoutCommit(root, clothVo);

            storage.Commit();
        }

        public void InsertAll(List<ClothVo> clothVos)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            int nCloth = 0;
            foreach (ClothVo clothVo in clothVos)
            {
                InsertWithoutCommit(root, clothVo);
                if (0 == ++nCloth % Constants.ComitLimit)
                {
                    storage.Commit();
                    root = (ClothRoot)storage.Root;
                }
            }

            storage.Commit();
        }

        /// <summary>
        /// Insert the cloth into the database if not exit, otherwise update it.
        /// </summary>
        public void insertOrUpdate(ClothVo clothVo)
        {
            Storage storage = DaoHelper.Instance.DbStorage;

            InsertOrUpdateWithoutCommit(DaoHelper.Instance.DbStorage, clothVo);

            storage.Commit();
        }

        public void insertOrUpdateAll(List<ClothVo> clothVos)
        {
            Storage storage = DaoHelper.Instance.DbStorage;

            int nCloth = 0;
            foreach (ClothVo clothVo in clothVos)
            {
                InsertOrUpdateWithoutCommit(DaoHelper.Instance.DbStorage, clothVo);
                if (0 == ++nCloth % Constants.ComitLimit)
                {
                    storage.Commit();
                }
            }
            
            storage.Commit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clothName">Name of a cloth.</param>
        public void Delete(String clothName)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            Cloth cloth = root.ClothName.Get(clothName);
            if (null != cloth)
            {
                RemoveFromColorAndShapeIndexes(root, clothName);
                root.ClothName.Remove(cloth.Name);

                storage.Commit();
            }
        }

        public ClothVo find(String clothName)
        {
            return null;
        }

        public List<ClothVo> findAll()
        {
            return null;
        }

        public List<ClothVo> findAllByColors(HashSet<String> colors)
        {
            return null;
        }

        public List<ClothVo> findAllByShapes(HashSet<String> shapes)
        {
            return null;
        }

        /// <summary>
        /// Insert a Cloth object into database without commit.
        /// <code>root</code> and <code>vo</code> are not checked in the function, 
        /// so they both cannot be null, or errors got.
        /// </summary>
        /// <param name="root">Object contains references to indexes.</param>
        /// <param name="clothVo">ClothVo object contains a cloth to be inserted.</param>
        private void InsertWithoutCommit(ClothRoot root, ClothVo clothVo)
        {
            Cloth cloth = convertClothVoToDomain(clothVo);

            root.ClothName.Put(cloth);
            // add colors and shapes index.
            putColorAndShapeIndexes(root, cloth);
        }

        /// <summary>
        /// Insert a Cloth object into database without commit if not exists, otherwise update.
        /// <code>storage</code> and <code>vo</code> are not checked in the function, 
        /// so they both cannot be null, or errors got.
        /// </summary>
        /// <param name="storage">The Storage object in Perst.</param>
        /// <param name="clothVo">ClothVo object contains a cloth to be inserted or updated.</param>
        private void InsertOrUpdateWithoutCommit(Storage storage, ClothVo clothVo)
        {
            ClothRoot root = (ClothRoot)storage.Root;
            Cloth cloth = root.ClothName.get(clothVo.Name);
            if (null != cloth)
            {
                // update
                // remove colors and shapes index first.
                RemoveFromColorAndShapeIndexes(root, cloth);

                updateClothVoToDomain(storage, cloth, clothVo);

                cloth.Modify();
                
                // add colors and shapes index.
                putColorAndShapeIndexes(root, cloth);
            }
            else
            {
                // insert
                InsertWithoutCommit(root, clothVo);
            }
        }

        /// <summary>
        /// Put entries associated with the Cloth object from the color and shape indexes.
        /// <code>root</code> and <code>vo</code> are not checked in the function, 
        /// so they both cannot be null, or errors got.
        /// </summary>
        /// <param name="root">Object contains references to indexes.</param>
        /// <param name="cloth">Cloth object whose colors and shapes to be put.</param>
        private void putColorAndShapeIndexes(ClothRoot root, Cloth cloth)
        {
            foreach (Color color in cloth.Colors)
            {
                root.ColorName.put(color.Name, cloth);
            }
            foreach (Shape shape in cloth.Shapes)
            {
                root.ShapeName.put(shape.Name, cloth);
            }
        }

        /// <summary>
        /// Remove entries associated with the Cloth object from the color and shape indexes.
        /// <code>root</code> and <code>vo</code> are not checked in the function, 
        /// so they both cannot be null, or errors got.
        /// </summary>
        /// <param name="root">Object contains references to indexes.</param>
        /// <param name="cloth">Cloth object whose colors and shapes to be removed.</param>
        private void RemoveFromColorAndShapeIndexes(ClothRoot root, Cloth cloth)
        {
            foreach (Color color in cloth.Colors)
            {
                root.ColorName.Remove(color.Name, cloth);
            }
            foreach (Shape shape in cloth.Shapes)
            {
                root.ShapeName.Remove(shape.Name, cloth);
            }
        }

        /// <summary>
        /// Convert a ClothVo object into a Cloth domain object.
        /// <code>storage</code> and <code>clothVo</code> are not checked in the function, 
        /// so they both cannot be null, or errors got.
        /// </summary>
        /// <param name="storage">The Storage object in Perst.</param>
        /// <param name="clothVo">The ClothVo object to be converted.</param>
        /// <returns>The Cloth domain object converted from the ClothVo object.</returns>
        private Cloth convertClothVoToDomain(Storage storage, ClothVo clothVo)
        {
            Cloth cloth = new Cloth(storage, clothVo.Name, clothVo.Pattern, clothVo.Path);

            if (null != clothVo.ColorNames)
            {
                foreach (String colorName in clothVo.ColorNames)
                {
                    cloth.Colors.Add(new Color(storage, colorName));
                }
            }

            if (null != clothVo.ShapeNames)
            {
                foreach (String shapeName in clothVo.ShapeNames)
                {
                    cloth.Shapes.Add(new Shape(storage, shapeName));
                }
            }

            return cloth;
        }

        /// <summary>
        /// Update the Cloth object <code>cloth</code> with the ClothVo object <code>clothVo</code>.
        /// <code>storage</code>, <code>cloth</code> and <code>clothVo</code> are not checked in the function, 
        /// so they all cannot be null, or errors got.
        /// </summary>
        /// <param name="storage">The Storage object in Perst.</param>
        /// <param name="cloth">The Cloth object to be updated.</param>
        /// <param name="clothVo">The ClothVo object contains new close information.</param>
        /// <returns>The Cloth domain object has been updated.</returns>
        private Cloth updateClothVoToDomain(Storage storage, Cloth cloth, ClothVo clothVo)
        {
            cloth.Name = clothVo.Name;
            cloth.Pattern = clothVo.Pattern;
            cloth.Path = clothVo.Path;
            cloth.Colors.Clear();
            cloth.Shapes.Clear();
            if (null != clothVo.ColorNames)
            {
                foreach (String colorName in clothVo.ColorNames)
                {
                    cloth.Colors.Add(new Color(storage, colorName));
                }
            }

            if (null != clothVo.ShapeNames)
            {
                foreach (String shapeName in clothVo.ShapeNames)
                {
                    cloth.Shapes.Add(new Shape(storage, shapeName));
                }
            }

            return cloth;
        }
    }
}
