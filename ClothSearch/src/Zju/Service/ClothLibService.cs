using System;
using System.Collections;
using System.Collections.Generic;
using Zju.Domain;
using Zju.Dao;
using Zju.Util;
using System.Threading;


namespace Zju.Service
{
    /// <summary>
    /// It is thread safe.
    /// </summary>
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

        public List<Cloth> FindAll()
        {
            return clothDao.FindAll();
        }

        public void AsynImportClothPics(ImportArgus argus)
        {
            if (argus.PicNames == null || argus.PicNames.Count == 0)
            {
                return;
            }
            if (argus.PicNames.Count < argus.ThreadNum)
            {
                argus.ThreadNum = argus.PicNames.Count;
            }

            ParameterizedThreadStart threadDelegate = new ParameterizedThreadStart(importClothPics);
            Thread importPicsThread = new Thread(threadDelegate);
            importPicsThread.IsBackground = true;
            importPicsThread.Start(argus);
        }

        private void importClothPics(Object obj)
        {
            ImportArgus argus = (ImportArgus)obj;
            List<String> picNames = argus.PicNames;

            // show progress win
            argus.BefDel(picNames.Count);

            Queue picNameQueue = Queue.Synchronized(new Queue(picNames));

            // finished pictures
            //nFinished = 0;

            ParameterizedThreadStart threadDelegate = new ParameterizedThreadStart(importClothPicThread);
            Thread[] threads = new Thread[argus.ThreadNum];
            for (int i = 0; i < threads.Length; ++ i )
            {
                threads[i] = new Thread(threadDelegate);
                threads[i].IsBackground = true;
                threads[i].Start(new ImportThreadArgus(picNameQueue, argus.Step, argus.StopImport, argus.StepDel));
            }

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }

            //Thread.Sleep(10000);

            argus.AfterDel();
        }

        private void importClothPicThread(Object obj)
        {
            ImportThreadArgus argus = (ImportThreadArgus)obj;

            List<Cloth> clothes = new List<Cloth>(argus.Step);

            while (!argus.StopImport())
            {
                String picName = dequeuePicName(argus.PicNameQueue);
                if (picName == null)
                {
                    break;
                }
                Cloth cloth = ClothUtil.GenerateClothObject(picName);

                clothes.Add(cloth);
                if (clothes.Count % argus.Step == 0)
                {
                    InsertAll(clothes);
                    argus.StepDel(clothes.Count);
                    clothes.Clear();
                }

            }
            if (clothes.Count > 0)
            {
                InsertAll(clothes);
                argus.StepDel(clothes.Count);
            }

        }

        private String dequeuePicName(Queue picQueue)
        {
            String name = null;
            try
            {
                while (String.IsNullOrEmpty(name))
                {
                    name = (String)picQueue.Dequeue();
                }
            }
            catch (System.InvalidOperationException e)
            {
                // empty queue.
                return null;
            }

            return name;
        }

        private class ImportThreadArgus
        {
            internal Queue PicNameQueue;
            internal IntArgDelegate StepDel;
            internal ShouldStop StopImport;
            internal int Step;

            internal ImportThreadArgus(Queue picNameQueue, int step,
                ShouldStop stopImport, IntArgDelegate stepDel)
            {
                this.PicNameQueue = picNameQueue;
                this.Step = step;
                this.StopImport = stopImport;
                this.StepDel = stepDel;
            }
        }

        #endregion

        public IClothDao ClothDao
        {
            set { this.clothDao = value; }
        }
    }

    public class ImportArgus
    {
        public List<String> PicNames;
        public int Step;
        public int ThreadNum;
        public ShouldStop StopImport;
        public IntArgDelegate BefDel;
        public IntArgDelegate StepDel;
        public NoArgDelegate AfterDel;

        public ImportArgus(List<String> picNames, int step, int threadNum, ShouldStop stopImport,
            IntArgDelegate befDel, IntArgDelegate stepDel, NoArgDelegate afterDel)
        {
            this.PicNames = picNames;
            this.Step = step;
            this.ThreadNum = threadNum;
            this.StopImport = stopImport;
            this.BefDel = befDel;
            this.StepDel = stepDel;
            this.AfterDel = afterDel;
        }
    }

    public delegate void IntArgDelegate(int argi);

    public delegate void NoArgDelegate();

    public delegate bool ShouldStop();
}
