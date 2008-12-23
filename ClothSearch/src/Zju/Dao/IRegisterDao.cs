using System;
using Zju.Domain;

namespace Zju.Dao
{
    public interface IRegisterDao
    {
        void Insert(RegisterInfo registerInfo);

        void Update(RegisterInfo registerInfo, RegisterInfo newRegisterInfo);

        void UpdateNoneIndex(RegisterInfo registerInfo);

        RegisterInfo FindByOid(int oid);

        RegisterInfo FindByName(string name);
    }
}
