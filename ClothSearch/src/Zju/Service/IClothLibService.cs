using System;
using System.Collections.Generic;
using Zju.Domain;

namespace Zju.Service
{
    public interface IClothLibService
    {
        void AddCloth(Cloth cloth);

        void AddClothes(List<Cloth> clothes);

        List<Cloth> findAll();
    }
}
