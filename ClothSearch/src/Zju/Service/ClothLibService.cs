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

        public void SaveOrUpdate(Cloth cloth)
        {
            if (0 == cloth.UpdateTime.Ticks)
            {
                cloth.UpdateTime = DateTime.Now;
            }
            clothDao.SaveOrUpdate(cloth);
        }

        public void SaveOrUpdate(List<Cloth> clothes)
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

        public void Delete(int oid)
        {
            if (oid > 0)
            {
                clothDao.Delete(oid);
            }
        }

        public List<Cloth> findAll()
        {
            return clothDao.FindAll();
        }

        #endregion

        public IClothDao ClothDao
        {
            set { this.clothDao = value; }
        }
    }
}
