using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Service
{
    public interface IClothLibService
    {
        void Insert(Cloth cloth);

        void InsertAll(List<Cloth> clothes);

        void Update(Cloth cloth, Cloth newCloth);

        void Delete(int oid);

        void Delete(Cloth cloth);

        List<Cloth> findAll();
    }
}
