using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Service
{
    public interface IClothLibService
    {
        void SaveOrUpdate(Cloth cloth);

        void SaveOrUpdate(List<Cloth> clothes);

        void Delete(int oid);

        List<Cloth> findAll();
    }
}
