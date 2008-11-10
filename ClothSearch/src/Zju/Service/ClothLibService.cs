using System;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;


namespace Zju.Service
{
    public class ClothLibService : IClothLibService
    {
        private IClothDao clothDao;

        public ClothLibService()
        {

        }

        public ClothLibService(IClothDao clothDao)
        {
            this.clothDao = clothDao;
        }

        #region IClothLibService Members

        public void AddCloth(Cloth cloth)
        {
            if (0 == cloth.UpdateTime.Ticks)
            {
                cloth.UpdateTime = DateTime.Now;
            }
            clothDao.SaveOrUpdate(cloth);
        }

        public void AddClothes(List<Cloth> clothes)
        {
            foreach (Cloth cloth in clothes)
            {
                if (0 == cloth.UpdateTime.Ticks)
                {
                    cloth.UpdateTime = DateTime.Now;
                }
            }
            clothDao.SaveOrUpdateAll(clothes);
        }

        #endregion

        public IClothDao ClothDao
        {
            set { this.clothDao = value; }
        }
    }
}
