using System;
using Perst;
using Zju.Domain;

namespace Zju.Dao
{
    public class RegisterDao : IRegisterDao
    {

        public void Insert(RegisterInfo registerInfo)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            FieldIndex registerInfoOidIndex = root.RegisterInfoOidIndex;
            if (registerInfo == null || registerInfoOidIndex.Contains(registerInfo))
            {
                return;
            }

            storage.BeginThreadTransaction(TransactionMode.Exclusive);
            try
            {
                // this method called for generate the Oid, or the key of RegisterInfoOidIndex will always be 0.
                storage.MakePersistent(registerInfo);
                registerInfoOidIndex.Set(registerInfo);
                root.RegisterInfoNameIndex.Set(registerInfo);
                storage.EndThreadTransaction();
            }
            catch (Exception)
            {
                // do some log
                storage.RollbackThreadTransaction();
            }
        }

        public void Update(RegisterInfo registerInfo, RegisterInfo newRegisterInfo)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            FieldIndex registerInfoOidIndex = root.RegisterInfoOidIndex;
            FieldIndex registerInfoNameIndex = root.RegisterInfoNameIndex;

            if (null == registerInfo || !registerInfoOidIndex.Contains(registerInfo))
            {
                return;
            }

            storage.BeginThreadTransaction(TransactionMode.Exclusive);
            try
            {
                if (registerInfo.Name != newRegisterInfo.Name)
                {
                    registerInfoNameIndex.Remove(registerInfo);
                    registerInfo.Name = newRegisterInfo.Name;
                    registerInfoNameIndex.Set(registerInfo);
                }
                
                registerInfo.SearchTime = newRegisterInfo.SearchTime;
                registerInfo.UpdateTime = newRegisterInfo.UpdateTime;
                registerInfo.LoginTime = newRegisterInfo.LoginTime;
                registerInfo.Md5Key = newRegisterInfo.Md5Key;
                registerInfo.Modify();

                storage.EndThreadTransaction();
            }
            catch (Exception)
            {
                // do some log
                storage.RollbackThreadTransaction();
            }
        }

        public void UpdateNoneIndex(RegisterInfo registerInfo)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            FieldIndex registerInfoOidIndex = root.RegisterInfoOidIndex;

            if (null == registerInfo || !registerInfoOidIndex.Contains(registerInfo))
            {
                return;
            }

            storage.BeginThreadTransaction(TransactionMode.Exclusive);
            try
            {
                registerInfo.Modify();

                storage.EndThreadTransaction();
            }
            catch (Exception)
            {
                // do some log
                storage.RollbackThreadTransaction();
            }
        }

        public RegisterInfo FindByOid(int oid)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            return (RegisterInfo)root.RegisterInfoOidIndex.Get(oid);
        }

        public RegisterInfo FindByName(string name)
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            ClothRoot root = (ClothRoot)storage.Root;

            return (RegisterInfo)root.RegisterInfoNameIndex.Get(name);
        }
    }
}
