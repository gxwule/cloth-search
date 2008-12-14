using System;
using Perst;
using Zju.Util;
using Zju.Domain;

namespace Zju.Dao
{
    /// <summary>
    /// Singleton.
    /// </summary>
    public sealed class DaoHelper
    {
        private Storage storage;

        private static DaoHelper helper;

        static DaoHelper()
        {
            helper = new DaoHelper();
            Storage storage = StorageFactory.Instance.CreateStorage();

            storage.Open(DbConstants.DataBaseFilePath, DbConstants.PagePoolSize);
            ClothRoot root = (ClothRoot)storage.Root;
            if (root == null)
            {
                root = new ClothRoot(storage);
                storage.Root = root;

                // persist root object.
                storage.Commit();
            }

            helper.storage = storage;
        }

        public Storage DbStorage
        {
            get
            {
                return storage;
            }
        }

        public static void CloseDb()
        {
            if (helper != null && helper.storage != null && helper.storage.IsOpened())
            {
                helper.storage.Close();
            }
        }

        private DaoHelper()
        {

        }

        ~DaoHelper()
        {
            // Commit modified objects and close db. The better practice is to close db explicitly.
            CloseDb();
        }

        public static DaoHelper Instance
        {
            get
            {
                return helper;
            }
        }
    }
}
