using System;
using Perst;
using Zju.Util;
using Zju.Domain;

namespace Zju.Dao
{
    sealed class DaoHelper
    {
        private Storage storage;

        private static DaoHelper helper;

        public Storage DbStorage
        {
            get
            {
                if (storage == null)
                {
                    storage = StorageFactory.Instance.CreateStorage();
                    storage.Open(Constants.DataBaseFilePath, Constants.PagePoolSize);
                    ClothRoot root = (ClothRoot)storage.Root;
                    if (root == null)
                    {
                        root = new ClothRoot(storage);
                        storage.Root = root;

                        initData();
                    }
                }
                return storage;
            }
        }

        private void initData()
        {
            ClothRoot root = (ClothRoot)storage.Root;
            foreach (String cn in Constants.ColorNames)
            {
                Color color = new Color(storage, cn);
                root.ColorName.Put(new Key(cn), color);
            }
            foreach (String sn in Constants.ShapeNames)
            {
                Shape shape = new Shape(storage, sn);
                root.ShapeName.Put(new Key(sn), shape);
            }
            storage.Commit();
        }

        private DaoHelper()
        {

        }

        ~DaoHelper()
        {
            storage.Close();
        }

        public static DaoHelper Instance
        {
            get
            {
                if (helper == null)
                {
                    helper = new DaoHelper();
                }
                return helper;
            }
        }
    }
}
