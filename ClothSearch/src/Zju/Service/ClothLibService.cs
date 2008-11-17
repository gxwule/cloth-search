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

        public void Insert(Cloth cloth)
        {
            if (0 == cloth.UpdateTime.Ticks)
            {
                cloth.UpdateTime = DateTime.UtcNow;
            }
            clothDao.Insert(cloth);
        }

        public void InsertAll(List<Cloth> clothes)
        {
            foreach (Cloth cloth in clothes)
            {
                if (0 == cloth.UpdateTime.Ticks)
                {
                    cloth.UpdateTime = DateTime.UtcNow;
                }
            }
            clothDao.InsertAll(clothes);
        }

        public void Update(Cloth cloth, Cloth newCloth)
        {
            clothDao.Update(cloth, newCloth);
        }

        public void Delete(int oid)
        {
            if (oid > 0)
            {
                clothDao.Delete(oid);
            }
        }

        public void Delete(Cloth cloth)
        {
            clothDao.Delete(cloth);
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
